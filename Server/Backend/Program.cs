using System.Text;
using System.Text.Json;
using Backend.Api;
using Backend.Components;
using Backend.Data;
using Backend.Repositories;
using Backend.Repositories.Interfaces;
using Backend.Services;
using Backend.Swagger;
using FluentValidation;
using Frontend.States;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MQTTnet;
using Shared.DTOs.Fine;
using Shared.DTOs.MQTT;
using Shared.DTOs.Request;
using Shared.DTOs.Reservation;
using Shared.DTOs.User;
using Shared.FluentValidators;
using Shared.FluentValidators.Properties;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddDbContext<SmartParkingContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SmartParking"))
);

builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        options.DocumentFilter<RemovePrefixDocumentFilter>("/api/v1");

        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "SmartParking API",
            Description = "An ASP.NET Core Minimal API for managing a smart parking system."
        });

        options.AddServer(new OpenApiServer
        {
            Url = "{protocol}://{host}:{port}/api/v{version}",
            Description = "Local Development Server",
            Variables = new Dictionary<string, OpenApiServerVariable>
            {
                ["protocol"] = new OpenApiServerVariable
                {
                    Default = "http",
                    Description = "The protocol used for the local development server.",
                    Enum = ["http", "https"]
                },
                ["host"] = new OpenApiServerVariable
                {
                    Default = "localhost",
                    Description = "The host name for the local development server."
                },
                ["port"] = new OpenApiServerVariable
                {
                    Default = "5123",
                    Description = "The port number for the local development server."
                },
                ["version"] = new OpenApiServerVariable
                {
                    Default = "1",
                    Description = "The version of the API.",
                    Enum = ["1"]
                }
            }
        });
    });
}

builder.Services.AddRateLimiter(_ => _.AddFixedWindowLimiter("default", options =>
{
    options.PermitLimit = 10;
    options.Window = TimeSpan.FromMinutes(1);
}));

builder.Services.AddCors(options =>
{
    options.AddPolicy("Restricted", policy =>
    {
        policy.WithOrigins("http://localhost:5123", "https://localhost:5123")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddHttpClient();

builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();

builder.Services.AddScoped<FineService>();
builder.Services.AddScoped<IFineRepository, FineRepository>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<RequestService>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();

builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

builder.Services.AddScoped<SlotService>();
builder.Services.AddScoped<ISlotRepository, SlotRepository>();

builder.Services.AddScoped<IValidator<UserRegisterDTO>, UserRegisterValidator>();
builder.Services.AddScoped<IValidator<UserLoginDTO>, UserLoginValidator>();
builder.Services.AddScoped<IValidator<ReservationCreateDTO>, ReservationValidator>();
builder.Services.AddScoped<IValidator<FineNewDTO>, FineCreationValidator>();
builder.Services.AddScoped<IValidator<NewRequestDTO>, NewRequestValidator>();

builder.Services.AddSingleton<EmailValidator>();
builder.Services.AddSingleton<PriceValidator>();
builder.Services.AddSingleton<PriceTypeValidator>();
builder.Services.AddSingleton<UserTypeValidator>();
builder.Services.AddSingleton<DateValidator>();
builder.Services.AddSingleton<TimeValidator>();
builder.Services.AddSingleton<DateTimeValidator>();

builder.Services.AddSingleton<AuthState>();
builder.Services.AddSingleton<AuthStateNotifier>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Frontend._Imports).Assembly);

app.UseCors("Restricted");

// Automatically map all minimal APIs
IEnumerable<IApiEndpoint> apiEndpoints = typeof(Program).Assembly
    .GetTypes()
    .Where(t => typeof(IApiEndpoint).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
    .Select(Activator.CreateInstance)
    .Cast<IApiEndpoint>();

foreach (IApiEndpoint endpoint in apiEndpoints) endpoint.MapEndpoints(app);

var mqttFactory = new MqttClientFactory();
var mqttClient = mqttFactory.CreateMqttClient();

var mqttClientOptions = new MqttClientOptionsBuilder()
    .WithTcpServer("localhost", 1883)
    .WithClientId("backend")
    .WithCredentials("backend", "backend")
    .WithCleanSession()
    .Build();

MqttClientConnectResult mqttConnect;
try {
    mqttConnect = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
} catch (Exception ex) {
    Console.WriteLine("Warning: [" + ex.GetType().Name + "] The MQTT client connection failed: " + ex.Message);
    return;
}

await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
    .WithTopic("iot/sensors/+/status")
    .Build());

await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
    .WithTopic("iot/mwbot/ack")
    .Build());

mqttClient.ApplicationMessageReceivedAsync += HandleBackendMqttMessage;

async Task HandleBackendMqttMessage(MqttApplicationMessageReceivedEventArgs e)
{
    var topic = e.ApplicationMessage.Topic;
    var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
    Console.WriteLine($"Received message on topic {topic}: {message}");

    if (topic.StartsWith("iot/sensors/") && topic.EndsWith("/status"))
    {
        SensorData? sensorData = JsonSerializer.Deserialize<SensorData>(message);
        if (sensorData == null) return;

        Console.WriteLine($"Sensor {sensorData.SlotId}, status: {sensorData.Status}");

        using (var scope = app.Services.CreateScope())
        {
            var slotService = scope.ServiceProvider.GetRequiredService<SlotService>();
            await slotService.UpdateSlotAsync(sensorData.SlotId, sensorData.Status);
        }

        return;
    }

    if (topic.Equals("iot/mwbot/ack"))
    {
        MWbotResponse? response = JsonSerializer.Deserialize<MWbotResponse>(message);
        if (response == null) return;

        using (var scope = app.Services.CreateScope())
        {
            var reservationService = scope.ServiceProvider.GetRequiredService<RequestService>();
            await reservationService.HandleMWbotAck(response);
        }
    }
}

app.Run();

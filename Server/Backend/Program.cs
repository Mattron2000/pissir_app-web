using Backend.Api;
using Backend.Components;
using Backend.Data;
using Backend.Repositories;
using Backend.Services;
using Backend.Swagger;
using FluentValidation;
using Frontend.States;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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

builder.Services.AddScoped<IValidator<UserRegisterDTO>, UserRegisterValidator>();
builder.Services.AddScoped<IValidator<UserLoginDTO>, UserLoginValidator>();
builder.Services.AddScoped<IValidator<ReservationCreateDTO>, ReservationValidator>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

builder.Services.AddSingleton<AuthState>();
builder.Services.AddSingleton<AuthStateNotifier>();

builder.Services.AddSingleton<EmailValidator>();

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

app.Run();

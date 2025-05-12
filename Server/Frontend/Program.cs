using FluentValidation;
using Frontend.States;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Shared.DTOs.User;
using Shared.FluentValidators;
using Shared.FluentValidators.Properties;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }
);

builder.Services.AddScoped<IValidator<UserRegisterDTO>, UserRegisterValidator>();
builder.Services.AddScoped<IValidator<UserLoginDTO>, UserLoginValidator>();

builder.Services.AddSingleton<AuthStateNotifier>();
builder.Services.AddSingleton<AuthState>();

builder.Services.AddSingleton<EmailValidator>();

await builder.Build().RunAsync();

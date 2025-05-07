using FluentValidation;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Shared.DTOs;
using Shared.FluentValidation;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }
);

builder.Services.AddScoped<IValidator<UserRegisterDTO>, UserValidator>();

await builder.Build().RunAsync();

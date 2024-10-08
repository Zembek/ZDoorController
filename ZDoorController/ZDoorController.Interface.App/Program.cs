﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RPiButtons.SSD1306;
using System.Device.Gpio;
using ZDoorController.Interface.App;
using ZDoorController.Interface.App.Interfaces;
using ZDoorController.Interface.App.Modules.Buttons;
using ZDoorController.Interface.App.Modules.Interfaces;
using ZDoorController.Interface.App.Modules.Photos;
using ZDoorController.Interface.App.Modules.ReedSwitches;
using ZDoorController.Interface.App.Modules.Relays;
using ZDoorController.Interface.App.Modules.Temperatures;
using ZDoorController.Interface.App.Services.FaceRecognition;

#if DEBUG
Console.WriteLine("Press any key for debug");
Console.ReadKey();
#endif

var hostBuilder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(app =>
    {
        app
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables();
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<GpioController>();
        services.AddSingleton<SSD1306Manager>();
        services.AddTransient<IPhotoModule, PhotoModule>();
        services.AddTransient<IButtonModule, ButtonModule>();
        services.AddTransient<IRelayModule, RelayModule>();
        services.AddTransient<ITemperatureModule, TemperatureModule>();
        services.AddTransient<IReedSwitchModule, ReedSwitchModule>();
        services.AddTransient<IFaceRecognitionService, AwsFaceRecognitionService>();
        services.AddHostedService<ApplicationService>();
    });

using IHost host = hostBuilder.Build();
await host.RunAsync();
Console.WriteLine("Exit app");
return;

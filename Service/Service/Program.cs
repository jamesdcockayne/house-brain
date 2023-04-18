﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service;
using Microsoft.Extensions.Configuration;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, configuration) =>
    {
        configuration.Sources.Clear();

        IHostEnvironment env = hostingContext.HostingEnvironment;

        configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((host, services) =>
    {
        if (host.HostingEnvironment.IsDevelopment())
        {
            services.AddTransient<Service.Immersion.IImmersionRelay, Service.Immersion.FakeImmersionRelay>();
            services.AddTransient<Service.Gas.IGasCallForHeatRelay, Service.Gas.FakeGasCallForHeatRelay>();
        }
        else
        {
            services.AddTransient<Service.Immersion.IImmersionRelay, Service.Immersion.ImmersionRelay>();
            services.AddTransient<Service.Gas.IGasCallForHeatRelay, Service.Gas.GasCallForHeatRelay>();
        }

        services.AddSingleton<TemperatureSensorReader>();
        services.AddSingleton<ICylinderTemperatureSensor>(x => x.GetRequiredService<TemperatureSensorReader>());
        services.AddSingleton<Service.Gas.IGasInletTemperatureSensors>(x => x.GetRequiredService<TemperatureSensorReader>());
        services.AddSingleton<Service.DhwCapacity.IColdWaterInletSensor>(x => x.GetRequiredService<TemperatureSensorReader>());
        services.AddSingleton<Service.Immersion.ICurrentSensor, Service.Immersion.CurrentSensor>();

        services.AddTransient<IWait, Waiter>();
        
        services.AddTransient<Service.Immersion.ImmersionOptimiser>();
        services.AddTransient<Service.Gas.GasHeatStrategy>();
        services.AddTransient<Service.DhwCapacity.WaterMixerCalculator>();
        services.AddTransient<Service.DhwCapacity.CapacityCalculator>();

        services.AddHostedService<Service.Immersion.ImmersionOptimiserService>();
        services.AddHostedService<Service.Gas.GasHeatSchedulerService>();

        services.AddOptions<WiringPinOptions>().BindConfiguration("Wiring").ValidateDataAnnotations().ValidateOnStart();
    })
    .UseConsoleLifetime()
    .Build();

await host.RunAsync();
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<TemperatureSensorReader>();
        services.AddSingleton<ICylinderTemperatureSensor>(x => x.GetRequiredService<TemperatureSensorReader>());
        services.AddSingleton<Service.Gas.IGasInletTemperatureSensors>(x => x.GetRequiredService<TemperatureSensorReader>());
        services.AddSingleton<Service.DhwCapacity.IColdWaterInletSensor>(x => x.GetRequiredService<TemperatureSensorReader>());
        services.AddSingleton<Service.Immersion.ICurrentSensor, Service.Immersion.CurrentSensor>();

        services.AddTransient<IWait, Waiter>();
        services.AddTransient<Service.Immersion.IImmersionRelay, Service.Immersion.ImmersionRelay>();
        services.AddTransient<Service.Gas.IGasCallForHeatRelay, Service.Gas.GasCallForHeatRelay>();
        services.AddTransient<Service.Immersion.ImmersionOptimiser>();
        services.AddTransient<Service.Gas.GasHeatStrategy>();
        services.AddTransient<Service.DhwCapacity.WaterMixerCalculator>();
        services.AddTransient<Service.DhwCapacity.CapacityCalculator>();

        services.AddHostedService<Service.Immersion.ImmersionOptimiserService>();
    })
    .Build();

await host.RunAsync();
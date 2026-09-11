using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Infrastructure.Repositories;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Desktop.ViewModels;

namespace EquipmentBorrowing.Desktop;

public partial class App : global::Avalonia.Application
{
    public static IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Configure dependency injection
        var services = new ServiceCollection();

        // Repositories (in-memory) and application services
        services.AddSingleton<IStudentRepository, InMemoryStudentRepository>();
        services.AddSingleton<IEquipmentRepository, InMemoryEquipmentRepository>();
        services.AddSingleton<IBorrowingRepository, InMemoryBorrowingRepository>();

        services.AddTransient<BorrowEquipmentService>();
        services.AddTransient<ReturnEquipmentService>();

        // ViewModels
        services.AddTransient<EquipmentViewModel>();
        services.AddTransient<ActiveBorrowingsViewModel>();

        // Register MainWindow so it can be constructed via DI if desired
        services.AddTransient<MainWindow>();

        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // resolve MainWindow via DI to allow constructor injection later
            desktop.MainWindow = Services.GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}

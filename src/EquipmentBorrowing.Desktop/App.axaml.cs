using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Desktop.ViewModels;
using EquipmentBorrowing.Desktop.Views;
using EquipmentBorrowing.Infrastructure.Persistence;
using EquipmentBorrowing.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Diagnostics;

namespace EquipmentBorrowing.Desktop;

public partial class App : Avalonia.Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();

        // 1. Register DbContext using AddDbContextFactory to support multi-threaded / concurrent view loading
        services.AddDbContextFactory<EquipmentBorrowingDbContext>(options =>
        {
            options.UseNpgsql("Host=localhost;Database=equipment_borrowing_db;Username=postgres;Password=admin123")
                   .LogTo(message => Debug.WriteLine(message), Microsoft.Extensions.Logging.LogLevel.Information);
        });

        // 2. Repositories (Transient or Scoped since they inject the thread-safe IDbContextFactory)
        services.AddTransient<IStudentRepository, EfStudentRepository>();
        services.AddTransient<IEquipmentRepository, EfEquipmentRepository>();
        services.AddTransient<IBorrowingRepository, EfBorrowingRepository>();

        // 3. Application Services
        services.AddTransient<BorrowEquipmentService>();
        services.AddTransient<ReturnEquipmentService>();

        // 4. Singletons maintain UI state across navigation views
        services.AddSingleton<EquipmentViewModel>();
        services.AddSingleton<BorrowingsViewModel>();
        services.AddSingleton<MainWindowViewModel>();

        var provider = services.BuildServiceProvider();

        // 5. Initialize Database and Seed Data using the Context Factory
        try
        {
            var contextFactory = provider.GetRequiredService<IDbContextFactory<EquipmentBorrowingDbContext>>();
            using (var context = contextFactory.CreateDbContext())
            {
                // Replaced EnsureCreated() with Migrate() to use formal EF Core migrations
                context.Database.Migrate();

                if (!context.Students.Any())
                {
                    context.Students.AddRange(
                        new Domain.Student(1, "2026-0001", "Maria Sedeno"),
                        new Domain.Student(2, "2026-0002", "John Doe")
                    );
                }

                if (!context.Equipment.Any())
                {
                    context.Equipment.AddRange(
                        new Domain.Equipment(1, "EQ-001", "Projector HD-1080"),
                        new Domain.Equipment(2, "EQ-002", "DSLR Camera Canon"),
                        new Domain.Equipment(3, "EQ-003", "Wireless Microphone")
                    );
                }

                context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Database Init Exception: {ex.Message}");
        }

        // 6. Assign Main Window to Desktop Lifetime
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = provider.GetRequiredService<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
using JournalApp.Data;
using JournalApp.Interfaces;
using JournalApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using JournalApp.ViewModels;
using JournalApp.Views;
using JournalApp.Services;

namespace JournalApp
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; }

        public new static App Current => (App)Application.Current;

        public App()
        {
            Services = ConfigureServices();
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private bool _isHandlingException;

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (_isHandlingException) return;
            _isHandlingException = true;

            try
            {
                System.IO.File.WriteAllText("crash.txt", e.Exception.ToString());
                MessageBox.Show($"An unhandled exception occurred: {e.Exception.Message}", "Crash", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                // Last resort: ignore errors during crash reporting
            }
            finally
            {
                _isHandlingException = false;
                e.Handled = true;
                Shutdown();
            }
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddDbContext<AppDbContext>(ServiceLifetime.Transient);

            services.AddTransient<IJournalRepository, JournalRepository>();
            services.AddTransient<IAnalyticsService, AnalyticsService>();
            services.AddTransient<IExportService, ExportService>();
            
            // ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<EntryViewModel>();
            services.AddTransient<TimelineViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<CalendarViewModel>();
            services.AddTransient<SettingsViewModel>();
            
            // Views
            services.AddSingleton<MainWindow>();

            return services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Ensure database is created and migrated
                var context = Services.GetRequiredService<AppDbContext>();
                context.Database.Migrate();

                var mainWindow = Services.GetRequiredService<MainWindow>();
                mainWindow.DataContext = Services.GetRequiredService<MainViewModel>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText("crash.txt", ex.ToString());
                MessageBox.Show($"Startup Error: {ex.Message}");
                Shutdown();
            }
        }
    }
}

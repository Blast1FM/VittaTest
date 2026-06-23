using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.IO;
using System.Windows;
using VittaTest.Data;
using VittaTest.Services;
using VittaTest.ViewModels;
using VittaTest.ViewModels.Dialogs;
using VittaTest.Views;

namespace VittaTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();
            var viewModel = ServiceProvider.GetRequiredService<MainViewModel>();
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddDbContextFactory<OrderAccountingDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                options.LogTo(message => Trace.WriteLine(message));
            });

            services.AddSingleton<IOrderPaymentService,  OrderPaymentService>();

            // ==================== ViewModels ====================
            services.AddTransient<MainViewModel>();
            services.AddTransient<SelectOrderViewModel>();
            services.AddTransient<SelectCashInflowViewModel>();
            services.AddTransient<PaymentsViewModel>();

            // ==================== Windows ====================
            services.AddTransient<MainWindow>(sp =>
            {
                var vm = sp.GetRequiredService<MainViewModel>();
                var window = new MainWindow();
                window.DataContext = vm;
                return window;
            });

            services.AddTransient<SelectOrderWindow>(sp =>
            {
                var vm = sp.GetRequiredService<SelectOrderViewModel>();
                var window = new SelectOrderWindow();
                window.DataContext = vm;
                return window;
            });

            services.AddTransient<SelectCashInflowWindow>(sp =>
            {
                var vm = sp.GetRequiredService<SelectCashInflowViewModel>();
                var window = new SelectCashInflowWindow();
                window.DataContext = vm;
                return window;
            });

            services.AddTransient<PaymentsWindow>(sp =>
            {
                var vm = sp.GetRequiredService<PaymentsViewModel>();
                var window = new PaymentsWindow();
                window.DataContext = vm;
                return window;
            });
        }
    }

}

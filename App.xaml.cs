using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.IO;
using System.Windows;
using VittaTest.Data;
using VittaTest.Services;
using VittaTest.ViewModels;

namespace VittaTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; } = null!;

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
            // Configuration
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

            // ViewModels
            services.AddTransient<MainViewModel>();

            // Views
            services.AddTransient<MainWindow>(sp =>
            {
                var viewModel = sp.GetRequiredService<MainViewModel>();
                var window = new MainWindow();
                window.DataContext = viewModel;
                return window;
            });
        }
    }

}

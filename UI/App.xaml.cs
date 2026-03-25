using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using UI.Core.Base;
using UI.Core.Navigation;
using UI.Features.Billing;
using UI.Features.Customers;
using UI.Features.Home;
using UI.Features.Items;

namespace UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private ServiceProvider _services = null!;
        protected override async void OnStartup(StartupEventArgs e)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            base.OnStartup(e);
            _services = BuildServices();
            await MigrateAsync(_services);

            var mainVm = _services.GetRequiredService<MainWindowViewModel>();
            var mainWindow = new MainWindow { DataContext = mainVm };
            mainWindow.Show();
        }


        private static ServiceProvider BuildServices()
        {
            var sc = new ServiceCollection();

            // ── Database ─────────────────────────────────────────────────
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "BillingApp",
                "billing.db");

            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

            sc.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlite($"Data Source={dbPath}"));

            // ── Repositories ──────────────────────────────────────────────
            sc.AddScoped<ICustomerRepository, CustomerRepository>();
            // ── Shared billing session state ──────────────────────────────
            sc.AddSingleton<ObservableCollection<BillItem>>();
            // sc.AddScoped<IItemRepository, ItemRepository>();   — add as you build them
            // sc.AddScoped<IBillRepository, BillRepository>();

            // ── Navigation ────────────────────────────────────────────────
            sc.AddSingleton<NavigationService>(sp =>
                new NavigationService(type => (ViewModelBase)sp.GetRequiredService(type)));

            // ── ViewModels ────────────────────────────────────────────────
            sc.AddSingleton<MainWindowViewModel>();
            sc.AddTransient<HomeViewModel>();
            sc.AddTransient<ManageCustomersViewModel>();
            sc.AddTransient<ManageItemsViewModel>();
            sc.AddTransient<ProductSelectionViewModel>();
            sc.AddTransient<QuantityViewModel>();

            return sc.BuildServiceProvider();
        }

        private static async Task MigrateAsync(ServiceProvider sp)
        {
            await using var scope = sp.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            _services.Dispose();
            base.OnExit(e);
        }
    }

}

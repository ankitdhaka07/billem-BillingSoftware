## UI – WPF Billing Application

## Solution Structure

Three projects in the solution:

* **domain/** – .NET 8.0 class library; business entities only, no UI or data deps.
* **Infrastructure/** – .NET 8.0 class library; EF Core + SQLite persistence, repository implementations.
* **UI/** – WPF (.NET 8.0-windows) application; references both domain and Infrastructure projects.

---

## Domain Project (`domain/`)

Pure entity classes, no logic beyond tax calculation:

* **Customer.cs** – Customer info (`Guid Id`, Name, GstNo, BillingAddress, ShippingAddress).
* **Item.cs** – Product/catalog item (`Guid Id`, Name, Price).
* **Bill.cs** – Invoice; holds `CustomerId` FK; computes CGST + SGST (2.5% each) on `TotalTaxableAmount`. Computed properties are not stored in DB.
* **BillItem.cs** – Line item linking an Item to a Bill. Stores `ItemName` and `UnitPrice` as price snapshots (decoupled from live Item price); also holds `Id`, `BillId`, `ItemId` FKs for EF.

---

## Infrastructure Project (`Infrastructure/`)

Data access layer; no UI dependencies:

```
Infrastructure/
├── AppDbContext.cs                  # EF Core DbContext; DbSets for Customer, Item, BillItem, Bill
├── AppDbContextFactory.cs           # Design-time factory for EF CLI migrations
├── Infrastructure.csproj            # References domain; EF Core + SQLite packages
├── Migrations/
│   ├── 20260321095944_InitialCreate.cs          # Schema migration – all four tables
│   ├── 20260321095944_InitialCreate.Designer.cs
│   └── AppDbContextModelSnapshot.cs
└── Repositories/
    ├── Interfaces/
    │   ├── ICustomerRepository.cs   # GetAllAsync, GetByIdAsync, AddAsync, UpdateAsync, DeleteAsync
    │   ├── IItemRepository.cs       # Same CRUD contract for Item catalog
    │   └── IBillRepository.cs       # GetAllAsync, GetByIdAsync, AddAsync (bills are append-only)
    └── Implementations/
        ├── CustomerRepository.cs    # EF Core implementation; uses AsNoTracking for reads
        ├── ItemRepository.cs        # EF Core implementation; sets Id = Guid.NewGuid() on AddAsync if null
        └── BillRepository.cs        # Eager-loads Customer + BillItems; marks Customer as Unchanged on AddAsync to avoid re-insert
```

**Key design decisions:**
* Fluent API configures required fields, `REAL` column types for doubles, and explicitly ignores computed properties on `Bill`.
* Repository pattern keeps EF details out of ViewModels.
* SQLite DB stored at `%AppData%\BillingApp\billing.db`; directory is auto-created on startup.

---

## UI Project Structure

```
UI/
├── App.xaml / App.xaml.cs              # App bootstrap; builds DI ServiceProvider; runs EF migrations; global DataTemplate VM→View mappings
├── MainWindow.xaml / .cs               # Root window; hosts ContentControl driven by CurrentViewModel
├── MainWindowViewModel.cs              # Holds CurrentViewModel; wires NavigationService + factory
├── AssemblyInfo.cs
├── Readme-llm.md
│
├── Core/
│   ├── Base/ViewModelBase.cs           # INotifyPropertyChanged base for all VMs
│   ├── Commands/RelayCommand.cs        # ICommand + ICommand<T> implementations
│   └── Navigation/NavigationService.cs # Decoupled navigation: Func<Type,ViewModelBase> factory + OnNavigate action
│
├── Features/
│   ├── Home/
│   │   ├── HomePage.xaml / .cs         # Landing page; buttons to navigate to Customers, Items, Billing, CheckBills
│   │   └── HomeViewModel.cs            # Commands: GenerateBill, ManageItems, ManageCustomers, CheckBills, About
│   ├── Billing/
│   │   ├── BillingSession.cs                 # Singleton shared state (SelectedItems, SelectedCustomer) across billing sub-flow
│   │   ├── ProductSelectionPage.xaml / .cs   # Customer ComboBox + product card grid (loads from DB) + selected list UI
│   │   ├── ProductSelectionViewModel.cs      # AvailableItems (from IItemRepository), Customers (from ICustomerRepository); AddItem, RemoveItem, Next
│   │   ├── QuantityPage.xaml / .cs           # Editable quantity list + Generate Bill button; binds ItemName/UnitPrice snapshots
│   │   └── QuantityViewModel.cs              # Saves Bill to DB via IBillRepository; generates PDF; navigates to Home
│   ├── Bills/
│   │   ├── CheckBillsPage.xaml / .cs         # Read-only list: Invoice#, Customer, Date, Total columns
│   │   └── CheckBillsViewModel.cs            # Loads all bills via IBillRepository (eager-loaded with Customer + BillItems)
│   ├── Customers/
│   │   ├── ManageCustomersPage.xaml / .cs    # Form (Name, GST No, Billing/Shipping Address) + list with Edit+Delete per row
│   │   └── ManageCustomersViewModel.cs       # Full CRUD backed by ICustomerRepository; commands: Add, Update, Delete, Select, Clear, Back
│   ├── Items/
│   │   ├── ManageItemsPage.xaml / .cs        # Form (Name, Price) + list with Edit+Delete per row
│   │   └── ManageItemsViewModel.cs           # Full CRUD backed by IItemRepository
│   └── About/
│       ├── AboutPage.xaml / .cs
│       └── AboutViewModel.cs
│
└── Services/
    └── BillPdfGenerator.cs             # PDF output using QuestPDF v2026.2.0; uses BillItem.UnitPrice (snapshot)
```

---

## Dependency Injection & Startup (`App.xaml.cs`)

`OnStartup` is `async` and performs the following in order:

1. `BuildServices()` — creates a `ServiceCollection` and registers:
   * `AppDbContext` with SQLite (`%AppData%\BillingApp\billing.db`)
   * `ICustomerRepository → CustomerRepository` (scoped)
   * `IItemRepository → ItemRepository` (scoped)
   * `IBillRepository → BillRepository` (scoped)
   * All ViewModels
2. `MigrateAsync()` — applies any pending EF Core migrations automatically.
3. Resolves `MainWindowViewModel` from the container and shows `MainWindow`.

---

## Navigation Flow

```
App → Home → (Customers | Items | Billing | CheckBills | About)
Billing sub-flow: ProductSelection → (Next) → Quantity → GenerateBill → saved to DB → PDF opened → Home
```

Navigation is ViewModel-first: `NavigationService.Navigate<TViewModel>()` swaps `MainWindowViewModel.CurrentViewModel`; `DataTemplate` in `App.xaml` resolves the correct View.

---

## Key Dependencies

* **Microsoft.EntityFrameworkCore.Sqlite** – persistence in `Infrastructure`.
* **Microsoft.EntityFrameworkCore.Design** – design-time migration support.
* **Microsoft.Extensions.DependencyInjection** – DI container wired in `App.xaml.cs`.
* **QuestPDF v2026.2.0** – PDF generation in `BillPdfGenerator`.
* **domain project** – shared entity types across all features.
* **Infrastructure project** – referenced by UI for repositories and `AppDbContext`.

---

## Architecture Summary

* MVVM throughout; no logic in code-behind files.
* **Three-layer architecture:** domain (entities) → Infrastructure (persistence) → UI (presentation).
* `Core/` contains only framework-level plumbing (base class, commands, navigation).
* `Features/` groups each business capability with its own VM + View pair; ViewModels depend on repository interfaces, not EF directly.
* `Services/` holds infrastructure concerns (PDF generation).
* Domain layer is UI-agnostic and has no EF dependency; Infrastructure depends on domain; UI depends on both.
* Price snapshots (`ItemName`, `UnitPrice`) on `BillItem` ensure historical invoice accuracy even if catalog prices change.

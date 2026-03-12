Chatbot Helper
UI – WPF Billing Application

## Solution Structure

Two projects in the solution:
* **domain/** – .NET 8.0 class library; business entities only, no UI deps.
* **UI/** – WPF (.NET 8.0-windows) application; references domain project.

---

## Domain Project (domain/)

Pure entity classes, no logic beyond tax calculation:

* **Customer.cs** – Customer info.
* **Item.cs** – Product/catalog item.
* **Bill.cs** – Invoice; computes CGST + SGST (2.5% each).
* **BillItem.cs** – Line item linking Item + quantity to a Bill.

---

## UI Project Structure

```
UI/
├── App.xaml / App.xaml.cs              # App bootstrap; global resources; DataTemplate VM→View mappings
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
│   │   ├── HomePage.xaml / .cs         # Landing page; buttons to navigate to Customers, Items, Billing
│   │   └── HomeViewModel.cs
│   ├── Billing/
│   │   ├── ProductSelectionPage.xaml / .cs   # Product card grid + selected list UI
│   │   ├── ProductSelectionViewModel.cs      # AvailableItems, SelectedItems; AddItem, Next commands
│   │   ├── QuantityPage.xaml / .cs           # Editable quantity list + Generate Bill button
│   │   └── QuantityViewModel.cs              # BillItems; GenerateBill() → calls BillPdfGenerator
│   ├── Customers/
│   │   ├── ManageCustomersPage.xaml / .cs
│   │   └── ManageCustomersViewModel.cs       # Customer CRUD UI
│   ├── Items/
│   │   ├── ManageItemsPage.xaml / .cs
│   │   └── ManageItemsViewModel.cs           # Item/product CRUD UI
│   └── About/
│       ├── AboutPage.xaml / .cs
│       └── AboutViewModel.cs
│
└── Services/
    └── BillPdfGenerator.cs             # PDF output using QuestPDF v2026.2.0
```

---

## Navigation Flow

App → Home → (Customers | Items | Billing | About)

Billing sub-flow: ProductSelection → (Next) → Quantity → GenerateBill → PDF saved/opened

Navigation is ViewModel-first: `NavigationService.Navigate<TViewModel>()` swaps `MainWindowViewModel.CurrentViewModel`; `DataTemplate` in `App.xaml` resolves the correct View.

---

## Key Dependencies

* **QuestPDF v2026.2.0** – PDF generation in `BillPdfGenerator`.
* **domain project** – shared entity types across all features.

---

## Architecture Summary

* MVVM throughout; no logic in code-behind files.
* `Core/` contains only framework-level plumbing (base class, commands, navigation).
* `Features/` groups each business capability with its own VM + View pair.
* `Services/` holds infrastructure concerns (PDF generation).
* Domain layer is UI-agnostic; UI layer depends on domain, not vice versa.

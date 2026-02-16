Chatbot Healper 
UI – WPF Billing POC 

Files:

* MainWindow.xaml (Window): Hosts ContentControl + DataTemplates for VM→View mapping.
* MainWindow.xaml.cs (.cs): Boilerplate; sets DataContext.
* MainWindowViewModel.cs (VM): Holds CurrentViewModel; navigates between screens.
* ProductSelectionPage.xaml (UserControl): Product cards + selected list UI.
* ProductSelectionPage.xaml.cs (.cs): Boilerplate.
* ProductSelectionViewModel.cs (VM): AvailableItems, SelectedItems; AddItem, Next commands.
* QuantityPage.xaml (UserControl): Edit quantities list + Generate Bill button.
* QuantityPage.xaml.cs (.cs): Boilerplate.
* QuantityViewModel.cs (VM): Holds BillItems; GenerateBill().
* RelayCommand.cs (Command): ICommand + ICommand<T> implementations.
* ViewModelBase.cs (Base): INotifyPropertyChanged.

Flow: App loads ProductSelection → AddItem adds to cart → Next swaps VM → Quantity edits → GenerateBill creates PDF.
Tech/Arch: WPF (.NET 8), ViewModel-first navigation via DataTemplates, ICommand-based actions, no code-behind logic.
Chatbot Healper
UI – WPF Billing POC

Files:

* MainWindow.xaml (Window): Hosts ContentControl + DataTemplates for VM→View mapping.
* MainWindow.xaml.cs (.cs): Boilerplate; sets DataContext.
* MainWindowViewModel.cs (VM): Holds CurrentViewModel; sets up NavigationService + factory; kicks off navigation.
* ProductSelectionPage.xaml (UserControl): Product cards + selected list UI.
* ProductSelectionPage.xaml.cs (.cs): Boilerplate.
* ProductSelectionViewModel.cs (VM): AvailableItems, SelectedItems; AddItem, Next commands. Uses NavigationService to navigate to QuantityViewModel.
* QuantityPage.xaml (UserControl): Edit quantities list + Generate Bill button.
* QuantityPage.xaml.cs (.cs): Boilerplate.
* QuantityViewModel.cs (VM): Holds BillItems; GenerateBill().
* RelayCommand.cs (Command): ICommand + ICommand<T> implementations.
* ViewModelBase.cs (Base): INotifyPropertyChanged.
* Services/NavigationService.cs (Service): Decoupled navigation via Func<Type,ViewModelBase> factory + OnNavigate action.

Flow: App loads ProductSelection → AddItem adds to cart → Next calls NavigationService.Navigate<QuantityViewModel>() → Quantity edits → GenerateBill creates PDF.
Tech/Arch: WPF (.NET 8), ViewModel-first navigation via DataTemplates, ICommand-based actions, NavigationService for decoupled VM transitions, no code-behind logic.
using domain;
using Infrastructure;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UI.Core.Base;
using UI.Core.Commands;
using UI.Core.Navigation;
using UI.Features.Home;

namespace UI.Features.Items;

public class ManageItemsViewModel : ViewModelBase
{
    private readonly IItemRepository _itemRepo;
    private readonly NavigationService _navigation;

    public ObservableCollection<Item> Items { get; } = new();

    private Item? _selectedItem;
    public Item? SelectedItem
    {
        get => _selectedItem;
        set { _selectedItem = value; OnPropertyChanged(); }
    }

    // ── Form fields ───────────────────────────────────────────────
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    private string _priceText = string.Empty;
    public string PriceText
    {
        get => _priceText;
        set { _priceText = value; OnPropertyChanged(); }
    }

    // ── Commands ──────────────────────────────────────────────────
    public ICommand BackCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand UpdateCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand SelectCommand { get; }
    public ICommand ClearCommand { get; }

    public ManageItemsViewModel(IItemRepository itemRepo, NavigationService navigation)
    {
        _itemRepo = itemRepo;
        _navigation = navigation;

        BackCommand = new RelayCommand(() => navigation.Navigate<HomeViewModel>());
        AddCommand = new RelayCommand(async () => await AddAsync(), () => IsValidForm());
        UpdateCommand = new RelayCommand(async () => await UpdateAsync(), () => SelectedItem?.Id != null && IsValidForm());
        DeleteCommand = new RelayCommand<Item>(async (item) =>
        {
            if (item?.Id == null) return;
            await _itemRepo.DeleteAsync(item.Id.Value);
            Items.Remove(item);
        });
        SelectCommand = new RelayCommand<Item>(OnItemSelected);
        ClearCommand = new RelayCommand(ClearForm);

        _ = LoadAsync();
    }

    private bool IsValidForm()
        => !string.IsNullOrWhiteSpace(Name) && double.TryParse(PriceText, out var p) && p >= 0;

    private async Task LoadAsync()
    {
        var all = await _itemRepo.GetAllAsync();
        Items.Clear();
        foreach (var i in all)
            Items.Add(i);
    }

    private async Task AddAsync()
    {
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = Name,
            Price = double.Parse(PriceText)
        };
        await _itemRepo.AddAsync(item);
        Items.Add(item);
        ClearForm();
    }

    private async Task UpdateAsync()
    {
        if (SelectedItem?.Id == null) return;

        SelectedItem.Name = Name;
        SelectedItem.Price = double.Parse(PriceText);

        await _itemRepo.UpdateAsync(SelectedItem);

        var index = Items.IndexOf(SelectedItem);
        if (index >= 0)
        {
            Items.RemoveAt(index);
            Items.Insert(index, SelectedItem);
        }

        ClearForm();
    }

    private void OnItemSelected(Item? item)
    {
        if (item is null) return;
        SelectedItem = item;
        Name = item.Name;
        PriceText = item.Price.ToString();
    }

    private void ClearForm()
    {
        SelectedItem = null;
        Name = string.Empty;
        PriceText = string.Empty;
    }
}

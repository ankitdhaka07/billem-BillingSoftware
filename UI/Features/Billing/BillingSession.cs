using domain;
using System.Collections.ObjectModel;

namespace UI.Features.Billing;

public class BillingSession
{
    public ObservableCollection<BillItem> SelectedItems { get; } = new();
    public Customer? SelectedCustomer { get; set; }
}

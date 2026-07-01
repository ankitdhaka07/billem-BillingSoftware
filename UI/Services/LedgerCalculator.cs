using domain;

namespace UI.Services;

/// <summary>One line in a computed ledger (a bill or a payment) with its running balance.</summary>
public class LedgerRow
{
    public DateTime Date { get; init; }
    public string Particulars { get; init; } = string.Empty;
    public double Debit { get; init; }
    public double Credit { get; init; }
    public double Balance { get; init; }

    // Display helpers shared by the UI page and the PDF.
    public string DebitText => Debit > 0 ? Debit.ToString("0.00") : "";
    public string CreditText => Credit > 0 ? Credit.ToString("0.00") : "";
    public string BalanceLabel => Balance >= 0
        ? $"{Balance:0.00} Dr"
        : $"{Math.Abs(Balance):0.00} Cr";
}

/// <summary>
/// A fully computed ledger for a customer and period. Computed on demand —
/// never persisted; the UI shows it and the PDF generator renders it.
/// </summary>
public class LedgerResult
{
    public Customer Customer { get; init; } = null!;
    public DateTime From { get; init; }
    public DateTime To { get; init; }
    public double OpeningBalance { get; init; }
    public List<LedgerRow> Rows { get; init; } = new();
    public double PeriodDebit { get; init; }
    public double PeriodCredit { get; init; }
    public double ClosingBalance { get; init; }

    public string OpeningBalanceLabel => OpeningBalance >= 0
        ? $"{OpeningBalance:0.00} Dr"
        : $"{Math.Abs(OpeningBalance):0.00} Cr";

    public string ClosingBalanceLabel => ClosingBalance >= 0
        ? $"{ClosingBalance:0.00} Dr"
        : $"{Math.Abs(ClosingBalance):0.00} Cr";

    public string ClosingSummary => ClosingBalance >= 0
        ? $"Closing Balance: Rs {Math.Abs(ClosingBalance):0.00} Dr  —  Customer owes this amount"
        : $"Closing Balance: Rs {Math.Abs(ClosingBalance):0.00} Cr  —  Amount due to customer";
}

public static class LedgerCalculator
{
    /// <summary>
    /// Computes the ledger for a customer over [from, to] from the full
    /// bill/payment lists. Opening balance is the net of everything before the period.
    /// </summary>
    public static LedgerResult Calculate(
        Customer customer,
        DateTime from,
        DateTime to,
        List<Bill> allBills,
        List<Payment> allPayments)
    {
        var fromDate = from.Date;
        var toDate = to.Date.AddDays(1).AddTicks(-1);

        var priorBills = allBills
            .Where(b => b.CustomerId == customer.Id && b.Date < fromDate)
            .Sum(b => b.TotalAmount);
        var priorPayments = allPayments
            .Where(p => p.CustomerId == customer.Id && p.PaymentDate < fromDate)
            .Sum(p => p.AmountPaid);
        var openingBalance = priorBills - priorPayments; // positive = Dr (customer owes us)

        var entries = new List<(DateTime Date, string Particulars, double Debit, double Credit)>();

        foreach (var bill in allBills.Where(b =>
                     b.CustomerId == customer.Id && b.Date >= fromDate && b.Date <= toDate))
            entries.Add((bill.Date, $"Invoice {bill.InvoiceNumber}", bill.TotalAmount, 0));

        foreach (var payment in allPayments.Where(p =>
                     p.CustomerId == customer.Id && p.PaymentDate >= fromDate && p.PaymentDate <= toDate))
            entries.Add((payment.PaymentDate, "Payment Received", 0, payment.AmountPaid));

        entries = entries.OrderBy(e => e.Date).ToList();

        double runningBalance = openingBalance;
        var rows = new List<LedgerRow>();
        foreach (var e in entries)
        {
            runningBalance += e.Debit - e.Credit;
            rows.Add(new LedgerRow
            {
                Date = e.Date,
                Particulars = e.Particulars,
                Debit = e.Debit,
                Credit = e.Credit,
                Balance = runningBalance
            });
        }

        var periodDebit = entries.Sum(e => e.Debit);
        var periodCredit = entries.Sum(e => e.Credit);

        return new LedgerResult
        {
            Customer = customer,
            From = from,
            To = to,
            OpeningBalance = openingBalance,
            Rows = rows,
            PeriodDebit = periodDebit,
            PeriodCredit = periodCredit,
            ClosingBalance = openingBalance + periodDebit - periodCredit
        };
    }
}

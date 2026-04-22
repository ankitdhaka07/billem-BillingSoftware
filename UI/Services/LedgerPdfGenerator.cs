using domain;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.IO;

namespace UI.Services;

public static class LedgerPdfGenerator
{
    private record LedgerEntry(DateTime Date, string Particulars, double Debit, double Credit);

    public static void Generate(
        Customer customer,
        DateTime from,
        DateTime to,
        List<Bill> bills,
        List<Payment> payments,
        double openingBalance,
        string filePath)
    {
        var entries = new List<LedgerEntry>();

        foreach (var bill in bills)
            entries.Add(new LedgerEntry(bill.Date, $"Invoice {bill.InvoiceNumber}", bill.TotalAmount, 0));

        foreach (var payment in payments)
            entries.Add(new LedgerEntry(payment.PaymentDate, "Payment Received", 0, payment.AmountPaid));

        entries = entries.OrderBy(e => e.Date).ToList();

        // Running balance starts from the opening balance
        double runningBalance = openingBalance;
        var rows = entries.Select(e =>
        {
            runningBalance += e.Debit - e.Credit;
            return (e, balance: runningBalance);
        }).ToList();

        double periodDebit  = entries.Sum(e => e.Debit);
        double periodCredit = entries.Sum(e => e.Credit);
        double closingBalance = openingBalance + periodDebit - periodCredit;

        byte[]? imageByte = File.Exists(Seller.LogoPath) ? File.ReadAllBytes(Seller.LogoPath) : null;

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Content()
                    .Border(1)
                    .Padding(10)
                    .Column(col =>
                    {
                        BuildHeader(col, customer, from, to, imageByte);
                        BuildTable(col, rows, openingBalance, periodDebit, periodCredit, closingBalance);
                        BuildSummary(col, closingBalance);
                    });
            });
        }).GeneratePdf(filePath);
    }

    static void BuildHeader(ColumnDescriptor col, Customer customer, DateTime from, DateTime to, byte[]? imageByte)
    {
        col.Item().PaddingHorizontal(-10).Row(row =>
        {
            row.RelativeItem().Element(x => x.PaddingHorizontal(4)).Column(c =>
            {
                c.Item().Text($"GSTIN: {Seller.GstNo}").FontSize(9);
                c.Item().Text($"PAN: {Seller.PANNo}").FontSize(9);
                c.Item().Text($"TIN: {Seller.TINNo}").FontSize(9);
            });

            row.ConstantItem(80).AlignCenter().AlignMiddle().Element(e =>
            {
                if (imageByte != null)
                    e.Image(imageByte).FitArea();
            });

            row.RelativeItem().Element(x => x.PaddingHorizontal(4)).AlignRight().Column(c =>
            {
                foreach (var p in Seller.PhoneNumbers)
                    c.Item().AlignRight().Text($"Mobile: {p}").FontSize(9);
            });
        });

        col.Item().AlignCenter().PaddingHorizontal(-10).PaddingTop(4).Text("CUSTOMER LEDGER")
            .FontSize(20).Bold();

        col.Item().PaddingHorizontal(-10).BorderTop(1).BorderBottom(1).Row(row =>
        {
            row.RelativeItem()
                .BorderRight(1)
                .Element(x => x.Padding(5))
                .Height(60)
                .Column(c =>
                {
                    c.Item().Text(Seller.Name).Bold().FontSize(13);
                    c.Item().Text("Hathnoda Chomu Jaipur, Jaipur, Rajasthan").FontSize(10);
                });

            row.RelativeItem()
                .Element(x => x.Padding(5))
                .Height(60)
                .Column(c =>
                {
                    c.Item().Text("Customer Details").Bold().FontSize(10);
                    c.Item().Text($"Name: {customer.Name}").FontSize(10);
                    c.Item().Text($"GSTIN: {customer.GstNo ?? "URP"}").FontSize(10);
                    c.Item().Text($"Period: {from:dd-MM-yyyy}  to  {to:dd-MM-yyyy}").FontSize(10);
                });
        });
    }

    static void BuildTable(
        ColumnDescriptor col,
        List<(LedgerEntry entry, double balance)> rows,
        double openingBalance,
        double periodDebit,
        double periodCredit,
        double closingBalance)
    {
        col.Item().PaddingTop(10).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(80);   // Date
                columns.RelativeColumn();     // Particulars
                columns.ConstantColumn(85);   // Debit
                columns.ConstantColumn(85);   // Credit
                columns.ConstantColumn(90);   // Balance
            });

            table.Header(header =>
            {
                header.Cell().Element(HeaderCell).Text("Date");
                header.Cell().Element(HeaderCell).Text("Particulars");
                header.Cell().Element(HeaderCell).AlignRight().Text("Debit (Rs)");
                header.Cell().Element(HeaderCell).AlignRight().Text("Credit (Rs)");
                header.Cell().Element(HeaderCell).AlignRight().Text("Balance (Rs)");
            });

            // Opening balance row
            var openingLabel = openingBalance >= 0
                ? $"{openingBalance:0.00} Dr"
                : $"{Math.Abs(openingBalance):0.00} Cr";
            table.Cell().Element(OpeningCell).Text("").FontSize(10);
            table.Cell().Element(OpeningCell).Text("Opening Balance").Bold().FontSize(10);
            table.Cell().Element(OpeningCell).Text("").FontSize(10);
            table.Cell().Element(OpeningCell).Text("").FontSize(10);
            table.Cell().Element(OpeningCell).AlignRight().Text(openingLabel).Bold().FontSize(10);

            if (rows.Count == 0)
            {
                table.Cell().ColumnSpan(5).Element(Cell)
                    .AlignCenter().Text("No transactions in this period").Italic().FontSize(10);
            }

            foreach (var (entry, balance) in rows)
            {
                var balanceLabel = balance >= 0
                    ? $"{balance:0.00} Dr"
                    : $"{Math.Abs(balance):0.00} Cr";

                table.Cell().Element(Cell).Text(entry.Date.ToString("dd-MM-yyyy")).FontSize(10);
                table.Cell().Element(Cell).Text(entry.Particulars).FontSize(10);
                table.Cell().Element(Cell).AlignRight()
                    .Text(entry.Debit > 0 ? entry.Debit.ToString("0.00") : "").FontSize(10);
                table.Cell().Element(Cell).AlignRight()
                    .Text(entry.Credit > 0 ? entry.Credit.ToString("0.00") : "").FontSize(10);
                table.Cell().Element(Cell).AlignRight().Text(balanceLabel).FontSize(10);
            }

            // Closing totals row
            var closingLabel = closingBalance >= 0
                ? $"{closingBalance:0.00} Dr"
                : $"{Math.Abs(closingBalance):0.00} Cr";

            table.Cell().Element(TotalsCell).Text("TOTAL").Bold();
            table.Cell().Element(TotalsCell).Text("");
            table.Cell().Element(TotalsCell).AlignRight().Text(periodDebit.ToString("0.00")).Bold();
            table.Cell().Element(TotalsCell).AlignRight().Text(periodCredit.ToString("0.00")).Bold();
            table.Cell().Element(TotalsCell).AlignRight().Text(closingLabel).Bold();
        });
    }

    static void BuildSummary(ColumnDescriptor col, double closingBalance)
    {
        var isDebit = closingBalance >= 0;
        var absBalance = Math.Abs(closingBalance);
        var label = isDebit
            ? $"Closing Balance: Rs {absBalance:0.00} Dr  —  Customer owes this amount"
            : $"Closing Balance: Rs {absBalance:0.00} Cr  —  Amount due to customer";

        col.Item().PaddingTop(12).PaddingHorizontal(-10)
            .Border(1).Padding(8)
            .AlignCenter()
            .Text(label)
            .FontSize(13).Bold();
    }

    static IContainer HeaderCell(IContainer c) =>
        c.Background("#D9E1F2").Border(1).Padding(5).AlignCenter().AlignMiddle();

    static IContainer Cell(IContainer c) =>
        c.BorderLeft(1).BorderRight(1).BorderBottom(1).Padding(4);

    static IContainer OpeningCell(IContainer c) =>
        c.Background("#EEF2FF").BorderLeft(1).BorderRight(1).BorderBottom(1).Padding(4);

    static IContainer TotalsCell(IContainer c) =>
        c.Background("#F2F4F7").Border(1).Padding(4);
}

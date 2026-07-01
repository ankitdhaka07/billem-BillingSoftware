using domain;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.IO;

namespace UI.Services;

public static class LedgerPdfGenerator
{
    /// <summary>Renders the PDF to the Desktop with the standard file name and returns the path.</summary>
    public static string ExportToDesktop(LedgerResult ledger)
    {
        var fileName = $"Ledger_{ledger.Customer.Name.Replace(" ", "_")}_{ledger.From:yyyyMMdd}_{ledger.To:yyyyMMdd}.pdf";
        var filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            fileName);
        Generate(ledger, filePath);
        return filePath;
    }

    public static void Generate(LedgerResult ledger, string filePath)
    {
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
                        BuildHeader(col, ledger, imageByte);
                        BuildTable(col, ledger);
                        BuildSummary(col, ledger);
                    });
            });
        }).GeneratePdf(filePath);
    }

    static void BuildHeader(ColumnDescriptor col, LedgerResult ledger, byte[]? imageByte)
    {
        col.Item().PaddingHorizontal(-10).Row(row =>
        {
            row.RelativeItem().Element(x => x.PaddingHorizontal(4)).Column(c =>
            {
                c.Item().Text($"GSTIN: {Seller.GstNo}").FontSize(9);
                c.Item().Text($"PAN: {Seller.PANNo}").FontSize(9);
                c.Item().Text($"TIN: {Seller.TINNo}").FontSize(9);
            });

            row.ConstantItem(80).MaxHeight(55).AlignCenter().AlignMiddle().Element(e =>
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
                .MinHeight(60)
                .Column(c =>
                {
                    c.Item().Text(Seller.Name).Bold().FontSize(13);
                    c.Item().Text(Seller.Address).FontSize(10);
                });

            row.RelativeItem()
                .Element(x => x.Padding(5))
                .MinHeight(60)
                .Column(c =>
                {
                    c.Item().Text("Customer Details").Bold().FontSize(10);
                    c.Item().Text($"Name: {ledger.Customer.Name}").FontSize(10);
                    c.Item().Text($"GSTIN: {ledger.Customer.GstNo ?? "URP"}").FontSize(10);
                    c.Item().Text($"Period: {ledger.From:dd-MM-yyyy}  to  {ledger.To:dd-MM-yyyy}").FontSize(10);
                });
        });
    }

    static void BuildTable(ColumnDescriptor col, LedgerResult ledger)
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
            table.Cell().Element(OpeningCell).Text("").FontSize(10);
            table.Cell().Element(OpeningCell).Text("Opening Balance").Bold().FontSize(10);
            table.Cell().Element(OpeningCell).Text("").FontSize(10);
            table.Cell().Element(OpeningCell).Text("").FontSize(10);
            table.Cell().Element(OpeningCell).AlignRight().Text(ledger.OpeningBalanceLabel).Bold().FontSize(10);

            if (ledger.Rows.Count == 0)
            {
                table.Cell().ColumnSpan(5).Element(Cell)
                    .AlignCenter().Text("No transactions in this period").Italic().FontSize(10);
            }

            foreach (var row in ledger.Rows)
            {
                table.Cell().Element(Cell).Text(row.Date.ToString("dd-MM-yyyy")).FontSize(10);
                table.Cell().Element(Cell).Text(row.Particulars).FontSize(10);
                table.Cell().Element(Cell).AlignRight().Text(row.DebitText).FontSize(10);
                table.Cell().Element(Cell).AlignRight().Text(row.CreditText).FontSize(10);
                table.Cell().Element(Cell).AlignRight().Text(row.BalanceLabel).FontSize(10);
            }

            // Closing totals row
            table.Cell().Element(TotalsCell).Text("TOTAL").Bold();
            table.Cell().Element(TotalsCell).Text("");
            table.Cell().Element(TotalsCell).AlignRight().Text(ledger.PeriodDebit.ToString("0.00")).Bold();
            table.Cell().Element(TotalsCell).AlignRight().Text(ledger.PeriodCredit.ToString("0.00")).Bold();
            table.Cell().Element(TotalsCell).AlignRight().Text(ledger.ClosingBalanceLabel).Bold();
        });
    }

    static void BuildSummary(ColumnDescriptor col, LedgerResult ledger)
    {
        col.Item().PaddingTop(12).PaddingHorizontal(-10)
            .Border(1).Padding(8)
            .AlignCenter()
            .Text(ledger.ClosingSummary)
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

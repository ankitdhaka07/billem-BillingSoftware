using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public static class BillPdfGenerator
{
    public static void Generate(Bill bill, string filePath)
    {
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    col.Item().Text("INVOICE")
                        .FontSize(20)
                        .Bold()
                        .AlignCenter();

                    col.Item().Text($"Item: {bill.BillItems[0].Item.Name}");
                    col.Item().Text($"Price: ₹{bill.BillItems[0].Item.Price}");
                    col.Item().Text($"Total Taxable: ({bill.BillItems[0].Amount}");
                    col.Item().Text($"GST: 5%");
                    col.Item().Text($"Tax Percentage: 5%");
                    col.Item().Text($"Total Tax ₹{bill.TaxApplied}");

                    col.Item().LineHorizontal(1);
                    col.Item().Text($"Total ₹{bill.TotalAmount}")
                        .FontSize(14)
                        .Bold();
                });
            });
        }).GeneratePdf(filePath);
    }
}
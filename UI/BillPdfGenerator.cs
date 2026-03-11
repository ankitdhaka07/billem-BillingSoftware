using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using System.Globalization;

public static class BillPdfGenerator
{
    public static void Generate(Bill bill, string filePath)
    {
        var companyName = "SHIV GANGA STONE CRUSHER";
        var companyAddress = "Hathnoda Chomu Jaipur, Jaipur, Rajasthan";
        var companyPhone = "8890803066";

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
                        BuildHeader(col, bill, companyName, companyAddress, companyPhone);
                        col.Item().PaddingHorizontal(-10).LineHorizontal(1);
                        BuildTable(col, bill);
                        col.Item().PaddingHorizontal(-10).LineHorizontal(1);

                        BuildTotals(col, bill);

                        BuildFooter(col, companyName);
                    });
            });
        })
        .GeneratePdf(filePath);
    }

    static void BuildHeader(ColumnDescriptor col, Bill bill, string companyName, string address, string phone)
    {
        col.Item().AlignCenter().Text("Thank-you for doing business with us");

        col.Item().AlignCenter().Text("INVOICE")
            .FontSize(22)
            .Bold();
        col.Item().PaddingHorizontal(-10).LineHorizontal(1);

        col.Item().Row(row =>
        {
            row.ConstantItem(70)
                .Height(70)
                .Border(1)
                .PaddingRight(10)
                .AlignCenter()
                .AlignMiddle()
                .Text("LOGO");

            row.RelativeItem().Column(c =>
            {
                c.Item().Text(companyName).Bold().FontSize(14);
                c.Item().Text(address);
                c.Item().Text(phone);
            });
            row.ConstantItem(1)
                .BorderLeft(1)
                .Height(60);
            row.RelativeItem().Column(c =>
            {
                c.Item().Text($"Invoice Number: {bill.InvoiceNumber}");
                c.Item().Text($"Invoice Date: {bill.Date:dd-MM-yyyy}");
                c.Item().Text($"Date Of Supply: {bill.SupplyDate:dd-MM-yyyy}");
                c.Item().Text("Reverse Charge: NO");
            });
        });
        col.Item().PaddingHorizontal(-10).LineHorizontal(1);
        col.Item().PaddingTop(10).Row(row =>
        {
            row.RelativeItem().Column(c =>
            {
                c.Item().Text("Details of Receiver | Billed to").Bold();
                c.Item().Text($"Name: {bill.Customer.Name}");
                c.Item().Text($"Address: {bill.Customer.BillingAddress}");
                c.Item().Text($"GSTIN: {bill.Customer.GstNo ?? "URP"}");
            });
            // this is the problemtic lines
            row.ConstantItem(1)
                .BorderLeft(1).PaddingVertical(-20);

            row.RelativeItem().Column(c =>
            {
                c.Item().Text("Details of Consignee | Shipped to").Bold();
                c.Item().Text($"Address: {bill.Customer.ShippingAddress}");
            });
        });
    }

    static void BuildTable(ColumnDescriptor col, Bill bill)
    {
        int desiredRows = 12;

        col.Item().PaddingTop(10).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(30); // Sr
                columns.RelativeColumn();   // Product

                columns.ConstantColumn(45); // Qty
                columns.ConstantColumn(50); // Rate
                columns.ConstantColumn(60); // Taxable

                columns.ConstantColumn(40); // CGST Rate
                columns.ConstantColumn(55); // CGST Amount

                columns.ConstantColumn(40); // SGST Rate
                columns.ConstantColumn(55); // SGST Amount

                columns.ConstantColumn(65); // Total
            });

            table.Header(header =>
            {
                // ROW 1
                header.Cell().RowSpan(2).Element(HeaderCell).Text("Sr");
                header.Cell().RowSpan(2).Element(HeaderCell).Text("Product");
                header.Cell().RowSpan(2).Element(HeaderCell).Text("QTY");
                header.Cell().RowSpan(2).Element(HeaderCell).Text("Rate");
                header.Cell().RowSpan(2).Element(HeaderCell).Text("Taxable");

                header.Cell().ColumnSpan(2).Element(HeaderCell).Text("CGST");
                header.Cell().ColumnSpan(2).Element(HeaderCell).Text("SGST");

                header.Cell().RowSpan(2).Element(HeaderCell).Text("Total");

                // ROW 2
                header.Cell().Element(HeaderCell).Text("Rate");
                header.Cell().Element(HeaderCell).Text("Amount");

                header.Cell().Element(HeaderCell).Text("Rate");
                header.Cell().Element(HeaderCell).Text("Amount");
            });

            int index = 1;

            foreach (var item in bill.BillItems)
            {
                var taxable = item.Amount;
                var cgst = taxable * bill.CgstPercentage;
                var sgst = taxable * bill.SgstPercentage;
                var total = taxable + cgst + sgst;

                table.Cell().Element(Cell).Text(index++.ToString());
                table.Cell().Element(Cell).Text(item.Item.Name);
                table.Cell().Element(Cell).Text(item.Quantity.ToString());
                table.Cell().Element(Cell).Text(item.Item.Price.ToString("0.00"));
                table.Cell().Element(Cell).Text(taxable.ToString("0.00"));

                table.Cell().Element(Cell).Text($"{bill.CgstPercentage * 100}%");
                table.Cell().Element(Cell).Text(cgst.ToString("0.00"));

                table.Cell().Element(Cell).Text($"{bill.SgstPercentage * 100}%");
                table.Cell().Element(Cell).Text(sgst.ToString("0.00"));

                table.Cell().Element(Cell).Text(total.ToString("0.00"));
            }

            for (int i = bill.BillItems.Count; i < desiredRows; i++)
            {
                for (int j = 0; j < 10; j++)
                    table.Cell().Element(Cell).Text("");
            }
        });
    }

    static void BuildTotals(ColumnDescriptor col, Bill bill)
    {
        col.Item().AlignRight().Column(c =>
        {
            c.Item().Text($"Taxable Amount ₹ {bill.TotalTaxableAmount:0.00}");
            c.Item().Text($"Add: CGST ₹ {bill.CgstAmount:0.00}");
            c.Item().Text($"Add: SGST ₹ {bill.SgstAmount:0.00}");

            c.Item().Text($"TOTAL ₹ {bill.TotalAmount:0.00}")
                .Bold()
                .FontSize(14);

            c.Item().Text($"Total Amount in words: {NumberToWords(bill.TotalAmount)} Rupees Only")
                .FontSize(10);
        });
    }

    static void BuildFooter(ColumnDescriptor col, string company)
    {
        col.Item().PaddingTop(15).Row(row =>
        {
            row.RelativeItem().Column(c =>
            {
                c.Item().Text("Bank and Payment Details").Bold();
                c.Item().Text("Bank Name / Account Number / IFSC");
            });

            row.RelativeItem().Column(c =>
            {
                c.Item().Text("Certified that the particulars given above are true and correct");
                c.Item().Text($"For, {company}").Bold();
                c.Item().Height(40);
                c.Item().Text("Authorised Signatory");
            });
        });

        col.Item().PaddingTop(10).Column(c =>
        {
            c.Item().Text("Terms And Conditions").Bold();

            c.Item().Text(
                "1. Goods once sold will not be taken back.\n" +
                "2. Materials as above are received in good condition.\n" +
                "3. Subject to Jaipur Jurisdiction. 4. Royalty Paid.");
        });

        col.Item().AlignCenter().Text("Thankyou for your business");
    }

    static IContainer HeaderCell(IContainer container)
    {
        return container
            .Background("#D9E1F2")
            .Border(1)
            .Padding(5)
            .AlignCenter()
            .AlignMiddle()
            ;
    }

    static IContainer Cell(IContainer container)
    {
        return container
            .BorderLeft(1)
            .BorderRight(1)
            .Padding(4);
    }

    static string NumberToWords(double number)
    {
        return number.ToString("N2", CultureInfo.InvariantCulture);
    }
}

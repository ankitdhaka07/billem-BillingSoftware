using domain;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.IO;
using System.Net;

namespace UI.Services;

public static class BillPdfGenerator
{
    public static void Generate(Bill bill, string filePath)
    {
        //var companyName = "SHIV GANGA STONE CRUSHER";
        //var companyAddress = "Hathnoda Chomu Jaipur, Jaipur, Rajasthan";
        //var companyPhone = "8890803066";

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
                        BuildHeader(col, bill) ;
                        BuildTable(col, bill);
                        col.Item().PaddingHorizontal(-10).LineHorizontal(1);

                        //BuildTotals(col, bill);

                        BuildFooter(col, Seller.Name);
                    });
            });
        })
        .GeneratePdf(filePath);
    }

    static void BuildHeader(ColumnDescriptor col, Bill bill)
    {
        var companyGst = Seller.GstNo;
        var companyTin = Seller.TINNo;
        var companyPan = Seller.PANNo;  
        var companyName = Seller.Name;
        var address = "Hathnoda Chomu Jaipur, Jaipur, Rajasthan";
        var phone = "8890803066";
        var image = Seller.LogoPath;
        byte[] imageByte = File.Exists(image) ? File.ReadAllBytes(image) : null;
        col.Item().PaddingHorizontal(-10).Row(row =>
        {
            row.RelativeItem().Element(x=>x.PaddingHorizontal(4)).Column(c =>
            {
                c.Item().Text($"GSTIN: {companyGst}").FontSize(9);
                c.Item().Text($"PAN: {companyPan}").FontSize(9);
                c.Item().Text($"TIN: {companyTin}").FontSize(9);
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
        col.Item().AlignCenter().PaddingHorizontal(-10).Element(x=>x.PaddingBottom(2)).Text("Thank-you for doing business with us");
        col.Item().AlignCenter().PaddingHorizontal(-10).Element(x => x.PaddingBottom(2)).Text("INVOICE")
            .FontSize(22)
            .Bold();

        col.Item().PaddingHorizontal(-10).BorderTop(1).BorderBottom(1).Row(row =>
        {

            row.RelativeItem()
                .BorderRight(1)
                .Element(x => x.Padding(5))
                .Height(70)
                .Column(c =>
                {
                    c.Item().Text(companyName).Bold().FontSize(14);
                    c.Item().Text(address);
                    c.Item().Text(phone);
                });

            row.RelativeItem()
                .Element(x => x.Padding(5))
                .Height(70)
                .Column(c =>
                {
                    c.Item().Text($"Invoice Number: {bill.InvoiceNumber}");
                    c.Item().Text($"Invoice Date: {bill.Date:dd-MM-yyyy}");
                    c.Item().Text($"Place Of Supply: {bill.SupplyDate:dd-MM-yyyy}");
                    c.Item().Text("Reverse Charge: NO");
                });
        });

        col.Item().PaddingHorizontal(-10).BorderTop(1).BorderBottom(1).Row(row =>
        {
            row.RelativeItem()
            .Height(70)
                .BorderRight(1)
                .Element(x => x.Padding(5))
                .Column(c =>
                {
                    c.Item().Text("Details of Receiver | Billed to").Bold();
                    c.Item().Text($"Name: {bill.Customer.Name}");
                    c.Item().Text($"Address: {bill.Customer.BillingAddress}");
                    c.Item().Text($"GSTIN: {bill.Customer.GstNo ?? "URP"}");
                });

            row.RelativeItem()
            .Height(70)
                .BorderRight(1)
                .Element(x => x.Padding(5))
                .Column(c =>
                {
                    c.Item().Text("Details of Consignee | Shipped to").Bold();
                    c.Item().Text($"Address: {bill.Customer.ShippingAddress}");
                });
        });
    }

    static void BuildFooter(ColumnDescriptor col, string company)
    {
        col.Item().PaddingHorizontal(-10).BorderTop(1).BorderBottom(1).Row(row =>
        {

            row.RelativeItem()
                .BorderRight(1)
                .Element(x => x.Padding(5))
                .Height(70)
                .Column(c =>
                {
                    c.Item().Text("Bank and Payment Details").Bold();
                    c.Item().Text("Bank Name / Account Number / IFSC");
                });

            row.RelativeItem()
                .Element(x => x.Padding(5))
                .Height(70)
                .Column(c =>
                {
                    c.Item().Text("Certified that the particulars given above are true and correct");
                    c.Item().Text($"For, {company}").Bold();
                    c.Item().Height(10);
                    c.Item().Text("Authorised Signatory");
                });
        });
        col.Item().PaddingTop(10).Column(c =>
        {
            c.Item().Text("Terms And Conditions").FontSize(10).Bold();

            c.Item().Text(
                "1. Goods once sold will not be taken back.\n" +
                "2. Materials as above are received in good condition.\n" +
                "3. Subject to Jaipur Jurisdiction. 4. Royalty Paid.").FontSize(10);
        });

        //col.Item().AlignCenter().Text("Thankyou for your business");

    }

    static void BuildTable(ColumnDescriptor col, Bill bill)
    {
        int emptySpaceRows = 5;

        col.Item().PaddingTop(10).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(30);   // Sr
                columns.RelativeColumn();     // Product
                columns.ConstantColumn(45);   // Qty
                columns.ConstantColumn(40);   // Rate
                columns.ConstantColumn(55);   // Taxable
                columns.ConstantColumn(40);   // CGST Rate
                columns.ConstantColumn(55);   // CGST Amount
                columns.ConstantColumn(35);   // SGST Rate
                columns.ConstantColumn(75);   // SGST Amount (wider for labels)
                columns.ConstantColumn(65);   // Total
            });

            table.Header(header =>
            {
                header.Cell().RowSpan(2).Element(HeaderCell).Text("Sr");
                header.Cell().RowSpan(2).Element(HeaderCell).Text("Product");
                header.Cell().RowSpan(2).Element(HeaderCell).Text("QTY");
                header.Cell().RowSpan(2).Element(HeaderCell).Text("Rate");
                header.Cell().RowSpan(2).Element(HeaderCell).Text("Taxable");

                header.Cell().ColumnSpan(2).Element(HeaderCell).Text("CGST");
                header.Cell().ColumnSpan(2).Element(HeaderCell).Text("SGST");

                header.Cell().RowSpan(2).Element(HeaderCell).Text("Total");

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

            // fill empty rows so table height stays constant
            for (int i = 0; i < emptySpaceRows; i++)
            {
                for (int j = 0; j < 10; j++)
                    table.Cell().Element(Cell).Text("");
            }

            // spacer row
            for (int j = 0; j < 10; j++)
                table.Cell().Element(Cell).Text("");

            void TotalsRow(string label, double value, bool bold = false, int fontSize = 8)
            {
                // columns 1–8 empty
                for (int i = 0; i < 8; i++)
                    table.Cell().Element(Cell).Text("");

                // column 9 → label
                var labelCell = table.Cell().Element(Cell).AlignRight() ;
                if (bold)
                    labelCell.Text(label).FontSize(fontSize).Bold();
                else
                    labelCell.Text(label).FontSize(fontSize);

                // column 10 → value
                var valueCell = table.Cell().Element(Cell).AlignRight();
                if (bold)
                    valueCell.Text(value.ToString("0.00")).Bold();
                else
                    valueCell.Text(value.ToString("0.00"));
            }

            TotalsRow("Taxable Amount", bill.TotalTaxableAmount,fontSize: 10);
            TotalsRow("CGST", bill.CgstAmount, fontSize: 10);
            TotalsRow("SGST", bill.SgstAmount, fontSize: 10);
            TotalsRow("Total", bill.TotalAmount, true,fontSize: 10);
        });
    }

    static void BuildTotals(ColumnDescriptor col, Bill bill)
    {
        col.Item().AlignRight().Column(c =>
        {
            c.Item().Text($"Taxable Amount ₹ {bill.TotalTaxableAmount:0.00}");
            c.Item().Text($"Add: CGST ₹ {bill.CgstAmount:0.00}");
            c.Item().Text($"Add: SGST ₹ {bill.SgstAmount:0.00}");
            c.Item().Text($"TOTAL ₹ {bill.TotalAmount:0.00}").Bold().FontSize(14);
            c.Item().Text($"Total Amount in words: {NumberToWords(bill.TotalAmount)} Rupees Only").FontSize(10);
        });
    }

    static IContainer HeaderCell(IContainer container)
    {
        return container
            .Background("#D9E1F2")
            .Border(1)
            .Padding(5)
            .AlignCenter()
            .AlignMiddle();
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

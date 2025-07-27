using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuestPDF;
using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using Shared.Enums;

namespace API.Services
{
    public class ReportRenderingService
    {
        public ReportRenderingService()
        {
            Settings.License = LicenseType.Community;
        }
        public byte[] GeneratePDFReport()
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Column(column =>
                        {
                            column.Spacing(10);

                            // First row: logo on left, company name on right
                            column.Item().Row(row =>
                            {
                                //row.RelativeItem().AlignLeft().Height(50).Image("Resources/logo.png");
                                row.RelativeItem().AlignRight().Text("شرکت کابل صرافی").FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);
                            });

                            // Centered title
                            column.Item().AlignCenter()
                                .Text("رسید تراکنش")
                                .FontSize(18)
                                .SemiBold()
                                .FontColor(Colors.Blue.Darken2);

                            // Customer info in Persian
                            column.Item().Row(row =>
                            {

                                row.RelativeItem().AlignRight().Text(text =>
                                {
                                    text.Span("شماره پیگیری تراکنش: ").Bold();
                                    text.Span("123456789"); // Replace with dynamic account number
                                });

                                row.RelativeItem().AlignRight().Text(text =>
                                {
                                    text.Span("شماره حساب: ").Bold();
                                    text.Span("123456789"); // Replace with dynamic account number
                                });
                                
                                row.RelativeItem().AlignRight().Text(text =>
                                {
                                    text.Span("نام مشتری: ").Bold();
                                    text.Span("احمد کریمی"); // Replace with dynamic name if needed
                                });
                            });
                        });

                    // Main content (Table)
                    page.Content().PaddingTop(20)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(100);     // Date
                                columns.ConstantColumn(100);     // Description
                                columns.ConstantColumn(75);     // Currency
                                columns.ConstantColumn(120);     // Amount
                                columns.ConstantColumn(65);     // Transaction Type
                                columns.ConstantColumn(65);     // Deal Type
                            });

                            // Styled header
                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCellStyle).AlignRight().Text("تاریخ");
                                header.Cell().Element(HeaderCellStyle).AlignRight().Text("شرح");
                                header.Cell().Element(HeaderCellStyle).AlignRight().Text("ارز");
                                header.Cell().Element(HeaderCellStyle).AlignRight().Text("مقدار");
                                header.Cell().Element(HeaderCellStyle).AlignRight().Text("نوع معامله");
                                header.Cell().Element(HeaderCellStyle).AlignRight().Text("نوع تراکنش");

                                static IContainer HeaderCellStyle(IContainer container)
                                {
                                    return container
                                        .Background(Colors.Blue.Darken2)
                                        .DefaultTextStyle(x => x.FontColor(Colors.White).Bold())
                                        .PaddingVertical(8);
                                        //.PaddingHorizontal(16);
                                }
                            });

                            // Transaction data
                            var transactions = new[]
                            {
                    new { Date = "1403/01/18", Description = "پرداخت نقدی بابت قرض از احمد به محمود", Currency = "افغانی", Amount = "100000000000", TransactionType = "خرید", DealType = "رسید" },
                    new { Date = "1403/01/18", Description = "پرداخت نقدی مکنشسیتبکمنتسیشبنمتک منسشتیبکمتیسکمبت ", Currency = "لیر ترکیه", Amount = "10,000 ", TransactionType = "خرید", DealType = "رسید" },
                    new { Date = "1403/01/18", Description = "پرداخت نقدی", Currency = "یوان چین", Amount = "10,000 ", TransactionType = "خرید", DealType = "رسید" }
                            };

                            for (int i = 0; i < transactions.Length; i++)
                            {
                                var t = transactions[i];
                                var backgroundColor = i % 2 == 0 ? Colors.Grey.Lighten5 : Colors.Grey.Lighten4;

                                table.Cell().Element(c => CellStyle(c, backgroundColor)).AlignRight().Text(t.Date);
                                table.Cell().Element(c => CellStyle(c, backgroundColor)).AlignRight().Text(t.Description);
                                table.Cell().Element(c => CellStyle(c, backgroundColor)).AlignRight().Text(t.Currency);
                                table.Cell().Element(c => CellStyle(c, backgroundColor)).AlignRight().Text(t.Amount);
                                table.Cell().Element(c => CellStyle(c, backgroundColor)).AlignRight().Text(t.DealType);
                                table.Cell().Element(c => CellStyle(c, backgroundColor)).AlignRight().Text(t.TransactionType);
                            }

                            static IContainer CellStyle(IContainer container, string backgroundColor)
                            {
                                return container
                                    .Background(backgroundColor)
                                    .PaddingVertical(8)
                                    .PaddingHorizontal(16);
                            }
                        });

                    // Footer
                    page.Footer().PaddingBottom(20)
                        .AlignRight()
                        .Text("مهر و امضا شرکت")
                        .FontSize(14)
                        .Bold();
                });
            });

            //document.ShowInCompanion();

            return document.GeneratePdf();
        }
    }
}

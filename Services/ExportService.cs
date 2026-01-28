using JournalApp.Interfaces;
using JournalApp.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JournalApp.Services
{
    public interface IExportService
    {
        Task ExportToPdfAsync(string filePath, IEnumerable<JournalEntry> entries);
    }

    public class ExportService : IExportService
    {
        static ExportService()
        {
            // QuestPDF License - Required for community use
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task ExportToPdfAsync(string filePath, IEnumerable<JournalEntry> entries)
        {
            await Task.Run(() =>
            {
                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(1, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(11));

                        page.Header().Text("My Journal Entries").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                        page.Content().PaddingVertical(10).Column(column =>
                        {
                            foreach (var entry in entries)
                            {
                                column.Item().PaddingBottom(10).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Column(c =>
                                {
                                    c.Item().Row(row =>
                                    {
                                        row.RelativeItem().Text(entry.Date.ToString("D")).Bold();
                                        row.RelativeItem().AlignRight().Text(entry.PrimaryMood).Italic();
                                    });
                                    c.Item().Text(entry.Title).FontSize(14).SemiBold();
                                    c.Item().Text(entry.Content);
                                    if (entry.Tags.Count > 0)
                                    {
                                        c.Item().Text("Tags: " + string.Join(", ", entry.Tags.Select(t => t.Name))).FontSize(9).FontColor(Colors.Grey.Medium);
                                    }
                                });
                            }
                        });

                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                    });
                }).GeneratePdf(filePath);
            });
        }
    }
}

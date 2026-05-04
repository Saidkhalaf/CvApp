using cvapp.BL.Domain.cvExportDocument;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CVAPP.UI.Web.MVC.Services.Export;

public class PdfCvExportService : IPdfExportService
{
  
    public byte[] GeneratePdf(CvExportDocument document, string templateName)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return templateName switch
        {
            "Professional" => GenerateProfessionalPdf(document),
            "Most used" => GenerateMostUsedPdf(document),
            _ => GenerateStandardPdf(document)
        };
    }

    // centered header , contact info and stacked sections
    private static byte[] GenerateStandardPdf(CvExportDocument document)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text(document.FullName).FontSize(28).Bold();
                       // column.Item().Text("Curriculum Vitae").FontSize(13).FontColor(Colors.Grey.Darken2);
                        AddContactLine(column, document.Email);
                        AddContactLine(column, document.Phone);
                        AddContactLine(column, document.LinkedInUrl);
                        AddContactLine(column, document.Address);
                    });

                    if (document.HasLogo)
                    {
                        row.ConstantItem(110).Height(70).Image(document.LogoBytes!).FitArea();
                    }
                });

                page.Content().PaddingTop(25).Column(column =>
                {
                    column.Spacing(20);

                    if (document.Languages.Any())
                        AddSimpleSection(column, "Languages", FormatLanguages(document.Languages));

                    if (document.Educations.Any())
                        AddSimpleSection(column, "Education", FormatEducations(document.Educations));

                    if (document.Certificates.Any())
                        AddSimpleSection(column, "Certificates", FormatCertificates(document.Certificates));

                    if (document.Trainings.Any())
                        AddSimpleSection(column, "Trainings", FormatTrainings(document.Trainings));
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Page ");
                    text.CurrentPageNumber();
                });
            });
        }).GeneratePdf();
    }

    // blue side bar with white text for contact and languages, main section with profile and other sections
    private static byte[] GenerateProfessionalPdf(CvExportDocument document)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(0);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Content().Row(row =>
                {
                    row.ConstantItem(170).Background(Colors.Blue.Darken4).Padding(20).Column(sidebar =>
                    {
                        sidebar.Spacing(14);

                        if (document.HasLogo)
                        {
                            sidebar.Item()
                                .AlignCenter()
                                .Width(100)
                                .Height(70)
                                .Image(document.LogoBytes!)
                                .FitArea();
                        }

                        sidebar.Item().PaddingTop(20).Text(document.FullName)
                            .FontSize(20)
                            .Bold()
                            .FontColor(Colors.White);

                        
                       /* sidebar.Item().Text("Curriculum Vitae")
                            .FontSize(11)
                            .FontColor(Colors.Blue.Lighten4);*/ 

                        AddSidebarContactLine(sidebar, document.Email);
                        AddSidebarContactLine(sidebar, document.Phone);
                        AddSidebarContactLine(sidebar, document.LinkedInUrl);

                        if (document.Languages.Any())
                        {
                            AddSidebarSection(sidebar, "Languages", FormatLanguages(document.Languages));
                        }
                    });

                    row.RelativeItem().Padding(35).Column(main =>
                    {
                        main.Spacing(24);

                        main.Item().Text("Professional Profile")
                            .FontSize(24)
                            .Bold()
                            .FontColor(Colors.Blue.Darken4);

                        if (document.Educations.Any())
                            AddModernSection(main, "Education", FormatEducations(document.Educations));

                        if (document.Certificates.Any())
                            AddModernSection(main, "Certificates", FormatCertificates(document.Certificates));

                        if (document.Trainings.Any())
                            AddModernSection(main, "Trainings", FormatTrainings(document.Trainings));
                    });
                });

                page.Footer().PaddingBottom(15).AlignCenter().Text(text =>
                {
                    text.Span(" Page ").FontSize(9).FontColor(Colors.Grey.Darken1);
                    text.CurrentPageNumber().FontSize(9).FontColor(Colors.Grey.Darken1);
                });
            });
        }).GeneratePdf();
    }

    //two column layout (left sidebar and right content)
    private static byte[] GenerateMostUsedPdf(CvExportDocument document)
    {
        const string Accent = "#9BAFA8";

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(0);
                page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(9.5f).FontColor("#222222"));

                page.Background().Background(Accent);

                page.Content()
                    .Padding(12)
                    .Background(Colors.White)
                    .Padding(32)
                    .Column(root =>
                    {
                        root.Spacing(24);

                        root.Item().Row(header =>
                        {
                            header.RelativeItem().Column(nameBlock =>
                            {
                                var names = document.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                var firstName = names.Length > 0 ? names[0].ToUpper() : "CV";
                                var lastName = names.Length > 1 ? string.Join(" ", names.Skip(1)).ToUpper() : "";

                                nameBlock.Item().Text(firstName).FontSize(32).FontColor(Colors.Black);
                                nameBlock.Item().PaddingTop(4).Text(lastName).FontSize(32).FontColor(Colors.Black);
                                nameBlock.Item().PaddingTop(18).LineHorizontal(2).LineColor(Accent);
                            });

                            if (document.HasLogo)
                            {
                                header.ConstantItem(115)
                                    .AlignRight()
                                    .Width(105)
                                    .Height(105)
                                    .Border(8)
                                    .BorderColor(Accent)
                                    .Image(document.LogoBytes!)
                                    .FitArea();
                            }
                        });

                        root.Item().Row(body =>
                        {
                            body.ConstantItem(175).Column(left =>
                            {
                                left.Spacing(22);

                                AddMostUsedLeftSection(left, "Contact", BuildContactItems(document), false);
                                AddMostUsedLeftSection(left, "Education", FormatEducations(document.Educations), false);
                                AddMostUsedLeftSection(left, "Languages", FormatLanguages(document.Languages), true);
                            });

                            body.ConstantItem(34)
                                .AlignCenter()
                                .LineVertical(2)
                                .LineColor(Accent);

                            body.RelativeItem().Column(right =>
                            {
                                right.Spacing(24);
                                AddMostUsedRightSection(right, "Certificates", FormatCertificates(document.Certificates));
                                AddMostUsedRightSection(right, "Trainings", FormatTrainings(document.Trainings));
                            });
                        });
                    });
            });
        }).GeneratePdf();
    }

    //adds a simple section (title and items with bulletpoint to the column )
    private static void AddSimpleSection(ColumnDescriptor column, string title, List<string> items)
    {
        column.Item().Column(section =>
        {
            section.Item().Text(title).FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
            section.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

            foreach (var item in items)
            {
                section.Item().PaddingTop(5).Text($"- {item}");
            }
        });
    }

    // gray backgorund header and formatted items to the column
    private static void AddModernSection(ColumnDescriptor column, string title, List<string> items)
    {
        column.Item().Column(section =>
        {
            section.Item().Background(Colors.Grey.Lighten3).Padding(8).Text(title)
                .FontSize(15)
                .Bold()
                .FontColor(Colors.Blue.Darken4);

            foreach (var item in items)
            {
                section.Item().PaddingTop(8).Row(row =>
                {
                    row.ConstantItem(10).Text("-").FontColor(Colors.Blue.Darken3);
                    row.RelativeItem().Text(item).FontSize(10);
                });
            }
        });
    }
    //section to the sidebar with white title and light blue items

    private static void AddSidebarSection(ColumnDescriptor column, string title, List<string> items)
    {
        column.Item().Column(section =>
        {
            section.Item().Text(title)
                .FontSize(13)
                .Bold()
                .FontColor(Colors.White);

            foreach (var item in items)
            {
                section.Item().PaddingTop(5).Text(item)
                    .FontSize(9)
                    .FontColor(Colors.Blue.Lighten5);
            }
        });
    }

    // left section in the mostused template with optional bullets 
    private static void AddMostUsedLeftSection(ColumnDescriptor column, string title, List<string> items, bool bullets)
    {
        if (!items.Any())
        {
            return;
        }

        column.Item().Column(section =>
        {
            section.Item().Text(title).Italic().Bold().FontSize(14);

            section.Item().PaddingTop(10).Column(list =>
            {
                list.Spacing(6);

                foreach (var item in items.Take(8))
                {
                    if (bullets)
                    {
                        list.Item().Row(row =>
                        {
                            row.ConstantItem(10).Text("-").FontSize(11).FontColor("#315E55");
                            row.RelativeItem().Text(item).FontSize(8.5f).LineHeight(1.25f);
                        });
                    }
                    else
                    {
                        list.Item().Text(item).FontSize(9).LineHeight(1.35f);
                    }
                }
            });

            section.Item().PaddingTop(16).LineHorizontal(2).LineColor("#9BAFA8");
        });
    }

    //right section in the mostused template with bullets and smaller font size
    private static void AddMostUsedRightSection(ColumnDescriptor column, string title, List<string> items)
    {
        if (!items.Any())
        {
            return;
        }

        column.Item().Column(section =>
        {
            section.Item().Text(title).Italic().Bold().FontSize(15);

            section.Item().PaddingTop(10).Column(list =>
            {
                list.Spacing(6);

                foreach (var item in items.Take(10))
                {
                    list.Item().Row(row =>
                    {
                        row.ConstantItem(12).Text("-").FontSize(11).FontColor("#315E55");
                        row.RelativeItem().Text(item).FontSize(9.5f).LineHeight(1.35f);
                    });
                }
            });
        });
    }

    //adds contact line to the sidebar if the value is not empty
    private static void AddContactLine(ColumnDescriptor column, string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            column.Item().Text(value).FontSize(9).FontColor(Colors.Grey.Darken2);
        }
    }

    // contact line to the sidebar if the value is not empty
    private static void AddSidebarContactLine(ColumnDescriptor column, string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            column.Item().Text(value).FontSize(8).FontColor(Colors.Blue.Lighten5);
        }
    }

    // builds a list of contact items (email, phone, linkedin, address) 
    private static List<string> BuildContactItems(CvExportDocument document)
    {
        return new[] { document.Email, document.Phone, document.LinkedInUrl, document.Address }
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToList();
    }
    //formats language into readable string with name and level

    private static List<string> FormatLanguages(IEnumerable<CvExportLanguageItem> items)
    {
        return items.Select(item => $"{item.Name} - {item.Level}").ToList();
    }

    //formats education into readable string with name, institute, level and years
    private static List<string> FormatEducations(IEnumerable<CvExportEducationItem> items)
    {
        return items.Select(item =>
        {
            var details = new List<string>
            {
                $"{item.StartYear} - {item.EndYear}",
                item.InstituteName,
                item.EducationLevel,
            };

            return $"{item.Name} ({string.Join(", ", details.Where(value => !string.IsNullOrWhiteSpace(value)))})";
        }).ToList();
    }
    
    //formats certificates into readable string with name, institute, year, code and expiration
    private static List<string> FormatCertificates(IEnumerable<CvExportCertificateItem> items)
    {
        return items.Select(item =>
        {
            var details = new List<string>
            {
                item.Year.ToString(),
                item.InstituteName,
                string.IsNullOrWhiteSpace(item.CertificateCode) ? string.Empty : $"Code {item.CertificateCode}",
                item.ExpiresOn == default ? string.Empty : $"expires {item.ExpiresOn:dd/MM/yyyy}"
            };

            return $"{item.Name} ({string.Join(", ", details.Where(value => !string.IsNullOrWhiteSpace(value)))})";
        }).ToList();
    }

    //formats trainings into readable string with name, institute and year
    private static List<string> FormatTrainings(IEnumerable<CvExportTrainingItem> items)
    {
        return items.Select(item =>
        {
            var details = new List<string>
            {
                item.Year.ToString(),
                item.InstituteName,
            };

            return $"{item.Name} ({string.Join(", ", details.Where(value => !string.IsNullOrWhiteSpace(value)))})";
        }).ToList();
    }
}

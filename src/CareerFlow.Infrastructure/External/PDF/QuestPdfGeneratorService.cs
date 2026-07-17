using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Infrastructure.External.PDF;

/// <summary>
/// Serviço de geração de PDF usando QuestPDF.
/// </summary>
public class QuestPdfGeneratorService : IPdfGeneratorService
{
    public QuestPdfGeneratorService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Task<byte[]> GenerateResumePdfAsync(ResumeData data, CancellationToken ct = default)
    {
        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                // Cabeçalho
                page.Header().Element(header => ComposeHeader(header, data));

                // Conteúdo
                page.Content().Element(content => ComposeContent(content, data));

                // Rodapé
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("CareerFlow - ");
                    text.Span("Gerado em ").FontSize(8);
                    text.Span(DateTime.Now.ToString("dd/MM/yyyy")).FontSize(8);
                });
            });
        });

        return Task.FromResult(pdf.GeneratePdf());
    }

    public Task<byte[]> GenerateAtsResumePdfAsync(ResumeData data, CancellationToken ct = default)
    {
        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                // ATS: Formato simplificado, sem gráficos, texto puro
                page.Content().Element(content =>
                {
                    content.Column(column =>
                    {
                        // Nome
                        column.Item().Text(data.PersonName).FontSize(18).Bold();
                        column.Item().Text($"{data.City}/{data.State} | {data.Phone} | {data.Email}").FontSize(10);
                        column.Item().PaddingVertical(5);

                        // Resumo
                        if (!string.IsNullOrWhiteSpace(data.ProfessionalSummary))
                        {
                            column.Item().Text("RESUMO PROFISSIONAL").FontSize(13).Bold().Underline();
                            column.Item().Text(data.ProfessionalSummary).FontSize(10);
                            column.Item().PaddingVertical(5);
                        }

                        // Experiências
                        if (data.Experiences.Count > 0)
                        {
                            column.Item().Text("EXPERIÊNCIA PROFISSIONAL").FontSize(13).Bold().Underline();
                            foreach (var exp in data.Experiences)
                            {
                                column.Item().PaddingVertical(3);
                                column.Item().Text($"{exp.Position} - {exp.CompanyName}").FontSize(11).Bold();
                                column.Item().Text(exp.IsCurrent
                                    ? $"{exp.StartDate:MMM/yyyy} - Atual"
                                    : $"{exp.StartDate:MMM/yyyy} - {exp.EndDate:MMM/yyyy}").FontSize(10);
                                if (!string.IsNullOrWhiteSpace(exp.Description))
                                    column.Item().Text(exp.Description).FontSize(10);
                            }
                            column.Item().PaddingVertical(5);
                        }

                        // Formação
                        if (data.Educations.Count > 0)
                        {
                            column.Item().Text("FORMAÇÃO ACADÊMICA").FontSize(13).Bold().Underline();
                            foreach (var edu in data.Educations)
                            {
                                column.Item().PaddingVertical(2);
                                column.Item().Text($"{edu.Course} - {edu.Institution}").FontSize(11).Bold();
                                column.Item().Text($"{edu.Level} | {edu.Status}").FontSize(10);
                            }
                            column.Item().PaddingVertical(5);
                        }

                        // Habilidades
                        if (data.Skills.Count > 0)
                        {
                            column.Item().Text("HABILIDADES").FontSize(13).Bold().Underline();
                            column.Item().Text(string.Join(" • ", data.Skills.Select(s => s.Name))).FontSize(10);
                        }
                    });
                });
            });
        });

        return Task.FromResult(pdf.GeneratePdf());
    }

    private void ComposeHeader(IContainer container, ResumeData data)
    {
        container.Column(column =>
        {
            column.Item().Text(data.PersonName).FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
            column.Item().Text(data.CurrentPosition ?? "").FontSize(13).FontColor(Colors.Grey.Darken2);
            column.Item().Text($"{data.City}/{data.State} | {data.Phone} | {data.Email}").FontSize(9);
        });
    }

    private void ComposeContent(IContainer container, ResumeData data)
    {
        container.Column(column =>
        {
            if (!string.IsNullOrWhiteSpace(data.ProfessionalSummary))
            {
                column.Item().PaddingVertical(5).Text("Resumo").FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
                column.Item().Text(data.ProfessionalSummary).FontSize(10);
            }

            if (data.Experiences.Count > 0)
            {
                column.Item().PaddingVertical(5).Text("Experiência").FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
                foreach (var exp in data.Experiences)
                {
                    column.Item().PaddingVertical(3);
                    column.Item().Text(exp.Position).FontSize(12).Bold();
                    column.Item().Text($"{exp.CompanyName} | {exp.DurationFormatted}").FontSize(10);
                    if (!string.IsNullOrWhiteSpace(exp.Description))
                        column.Item().Text(exp.Description).FontSize(10);
                }
            }

            if (data.Skills.Count > 0)
            {
                column.Item().PaddingVertical(5).Text("Habilidades").FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
                column.Item().Text(string.Join(", ", data.Skills.Select(s => s.Name))).FontSize(10);
            }
        });
    }
}

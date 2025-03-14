using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Managers;
using EntitiesLayer.Entities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextParagraph = iTextSharp.text.Paragraph;
using iTextFont = iTextSharp.text.Font;
using Entities.SharedServices;

namespace PresentationLayer
{
    public partial class UcProfileStats : UserControl
    {
        private readonly AdService adService;
        private readonly CategoryService categoryService;

        public UcProfileStats()
        {
            InitializeComponent();
            adService = new AdService();
            categoryService = new CategoryService();
            Loaded += UcProfileStats_Loaded;
        }

        private async void UcProfileStats_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadUserStatsAsync();
        }

        private async Task LoadUserStatsAsync()
        {
            var currentUser = SessionManager.GetCurrentUser();
            if (currentUser == null)
            {
                MessageBox.Show("Niste prijavljeni.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var userAds = await adService.GetUserAdsAsync(currentUser.id);
            if (userAds == null || !userAds.Any())
            {
                lblTotalViews.Content = "0";
                lblAverageViews.Content = "0";
                dgViewsPerCategory.ItemsSource = null;
                return;
            }


            var categories = await categoryService.GetCategoriesAsync();

            int totalViews = userAds.Sum(ad => ad.views ?? 0);
            double averageViews = userAds.Average(ad => ad.views ?? 0);


            var categoryViews = userAds
                .Select(ad => new
                {
                    CategoryName = LocalizationService.GetLocalizedString(categories.FirstOrDefault(c => c.id == ad.category_id)?.localizationkey)
                                   ?? $"Category {ad.category_id}",
                    Views = ad.views ?? 0
                })
                .GroupBy(item => item.CategoryName)
                .Select(group => new
                {
                    Category = group.Key,
                    Views = group.Sum(item => item.Views)
                })
                .ToList();

            lblTotalViews.Content = totalViews.ToString();
            lblAverageViews.Content = averageViews.ToString("N2");
            dgViewsPerCategory.ItemsSource = categoryViews;
        }

        private void btnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            ExportToPdf();
        }

        private void ExportToPdf()
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "PDF files (.pdf)|.pdf",
                FileName = "ProfileStats.pdf"
            };

            if (saveDialog.ShowDialog() != true)
            {
                return;
            }

            string filePath = saveDialog.FileName;
            var document = new Document(PageSize.A4, 50, 50, 25, 25);

            try
            {
                PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                iTextParagraph header = new iTextParagraph(
                    "Profile Statistics",
                    new iTextFont(Font.FontFamily.HELVETICA, 16, iTextFont.BOLD)
                );
                header.Alignment = Element.ALIGN_CENTER;
                document.Add(header);

                document.Add(new iTextParagraph(" "));
                document.Add(new iTextParagraph("Total Views: " + lblTotalViews.Content));
                document.Add(new iTextParagraph("Average Views: " + lblAverageViews.Content));
                document.Add(new iTextParagraph(" "));

                PdfPTable table = new PdfPTable(2)
                {
                    WidthPercentage = 100
                };
                table.AddCell(new PdfPCell(new Phrase("Category")));
                table.AddCell(new PdfPCell(new Phrase("Views")));

                var items = dgViewsPerCategory.ItemsSource;
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        var itemType = item.GetType();
                        var category = itemType.GetProperty("Category").GetValue(item, null)?.ToString();
                        var views = itemType.GetProperty("Views").GetValue(item, null)?.ToString();
                        table.AddCell(new PdfPCell(new Phrase(category)));
                        table.AddCell(new PdfPCell(new Phrase(views)));
                    }
                }

                document.Add(table);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting to PDF: " + ex.Message);
            }
            finally
            {
                document.Close();
            }

            MessageBox.Show("Export successful!", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
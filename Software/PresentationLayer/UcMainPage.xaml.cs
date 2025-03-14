using BusinessLogicLayer.Managers;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Threading.Tasks;

namespace PresentationLayer
{
    public partial class UcMainPage : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<RecommendedItem> RecommendedItems { get; set; }
        public ObservableCollection<RecommendedItem> DisplayedRecommendedItems { get; set; }

        private int _currentPage;
        private int itemsPerPage;

        private ICommand _previousCommand;
        private ICommand _nextCommand;


        public bool IsPreviousVisible => _currentPage > 0;
        public bool IsNextVisible => (_currentPage + 1) * itemsPerPage < RecommendedItems.Count;

        public ICommand PreviousCommand => _previousCommand ?? (_previousCommand = new CommandHandler(ShowPrevious, () => IsPreviousVisible));
        public ICommand NextCommand => _nextCommand ?? (_nextCommand = new CommandHandler(ShowNext, () => IsNextVisible));

        public UcMainPage()
        {
            InitializeComponent();
            Categories = new ObservableCollection<Category>();
            RecommendedItems = new ObservableCollection<RecommendedItem>();
            DisplayedRecommendedItems = new ObservableCollection<RecommendedItem>();
            DataContext = this;

            _currentPage = 0;
            itemsPerPage = CalculateItemsPerPage();

            InitializeAsync().ConfigureAwait(false); UpdateDisplayedRecommendedItems();
            this.SizeChanged += UcMainPage_SizeChanged;

            RecommendedItemsControl.Loaded += RecommendedItemsControl_Loaded;
        }

        private async Task InitializeAsync()
        {
            try
            {
                await FetchCategoriesAsync();
                await FetchRecommendedItemsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching recommended items: {ex.Message}");
            }
        }

        private async Task FetchCategoriesAsync()
        {
            try
            {
                var categoryService = new CategoryService();
                var categoriesFromManager = await categoryService.GetCategoriesAsync();

                await Dispatcher.InvokeAsync(() =>
                {
                    foreach (var category in categoriesFromManager)
                    {
                        category.img = $"/Resources/{category.img}.png";
                        category.localizationkey = LocaleManager.GetLocalizedString(category.localizationkey);
                        Console.WriteLine($"Localization key: {category.localizationkey}");

                        Categories.Add(category);
                    }

                    OnPropertyChanged(nameof(Categories));
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching categories: {ex.Message}");
                // Optionally, show an error message to the user
            }
        }

        private void RecommendedItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            itemsPerPage = CalculateItemsPerPage();
            UpdateDisplayedRecommendedItems();
        }

        private void UcMainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            itemsPerPage = CalculateItemsPerPage();
            UpdateDisplayedRecommendedItems();
        }

        private int CalculateItemsPerPage()
        {
            double itemWidth = 222;
            double availableWidth = Application.Current.MainWindow.ActualWidth;

            if (availableWidth > 0)
            {
                return Math.Max(1, (int)Math.Floor(availableWidth / itemWidth));
            }

            return 1;
        }
        private async Task FetchRecommendedItemsAsync()
        {
            var adService = new AdService();
            var ads = await adService.GetAllAdsAsync();

            RecommendedItems.Clear();

            foreach (var ad in ads)
            {
                var imagePath = ConvertBase64ToImageSource(ad.ImageBase64);

                Console.WriteLine($"ImagePath for Ad {ad.Id}: {imagePath}");

                RecommendedItems.Add(new RecommendedItem
                {
                    id = ad.Id,
                    ImagePath = imagePath,
                    Title = ad.Title,
                    Description = ad.Description
                });
            }

            UpdateDisplayedRecommendedItems();
        }


        private ImageSource ConvertBase64ToImageSource(string base64String)
        {
            if (string.IsNullOrEmpty(base64String) || base64String == "default_image_path")
                return null;

            try
            {
                string base64Data = base64String;
                if (base64Data.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
                {
                    var indexOfBase64 = base64Data.IndexOf("base64,", StringComparison.OrdinalIgnoreCase);
                    if (indexOfBase64 >= 0)
                    {
                        base64Data = base64Data.Substring(indexOfBase64 + 7);
                    }
                }

                byte[] bytes = Convert.FromBase64String(base64Data);
                using (var stream = new System.IO.MemoryStream(bytes))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; 
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Base64 conversion failed: " + ex.Message);
                return null;
            }
        }


        private void OnCategoryClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var clickedCategory = button?.CommandParameter as Category;

            if (clickedCategory != null)
            {
                var parentWindow = Window.GetWindow(this);
                if (parentWindow != null)
                {
                    var mainContentControl = parentWindow.FindName("MainContentControl") as ContentControl;
                    if (mainContentControl != null)
                    {
                        mainContentControl.Content = new UcSearchAds("", clickedCategory.id);
                    }
                }
            }
        }


        private void OnItemClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var clickedItem = button?.CommandParameter as RecommendedItem;

            if (clickedItem != null)
            {
                var parentWindow = Window.GetWindow(this) as MainWindow;
                parentWindow.MainContentControl.Content = ((new AdDetailsUC(clickedItem.id)));
            }
        }

        private void ShowPrevious()
        {
            if (_currentPage > 0)
            {
                _currentPage--;
                UpdateDisplayedRecommendedItems();
            }
        }

        private void ShowNext()
        {
            if ((_currentPage + 1) * itemsPerPage < RecommendedItems.Count)
            {
                _currentPage++;
                UpdateDisplayedRecommendedItems();
            }
        }

        private void UpdateDisplayedRecommendedItems()
        {
            DisplayedRecommendedItems.Clear();
            var items = RecommendedItems.Skip(_currentPage * itemsPerPage).Take(itemsPerPage);
            foreach (var item in items)
            {
                DisplayedRecommendedItems.Add(item);
            }

            OnPropertyChanged(nameof(IsPreviousVisible));
            OnPropertyChanged(nameof(IsNextVisible));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RecommendedItem
    {
        public int id { get; set; }
        public ImageSource ImagePath { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class CommandHandler : ICommand
    {
        private readonly Action _action;
        private readonly Func<bool> _canExecute;

        public CommandHandler(Action action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
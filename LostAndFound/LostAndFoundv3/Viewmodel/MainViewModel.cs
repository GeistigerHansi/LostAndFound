using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Input;
using LostAndFound.WPF.Model;
using LostAndFound.WPF.View;

namespace LostAndFound.WPF.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        // API Basis-URL – ggf. anpassen
        private const string ApiBaseUrl = "http://localhost:5000/api";

        private readonly HttpClient _httpClient = new();

        #region Properties

        private ObservableCollection<Item> _items = new();
        public ObservableCollection<Item> Items
        {
            get => _items;
            set { _items = value; OnPropertyChanged(nameof(Items)); }
        }

        private Item? _selectedItem;
        public Item? SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(nameof(SelectedItem)); }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(nameof(SearchText)); }
        }

        private string _statusMessage = "Bereit.";
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(nameof(StatusMessage)); }
        }

        #endregion

        #region Commands
        public ICommand LoadItemsCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand EditItemCommand { get; }
        public ICommand DeleteItemCommand { get; }
        public ICommand ShowClaimsCommand { get; }
        public ICommand SearchCommand { get; }
        #endregion

        public MainViewModel()
        {
            LoadItemsCommand  = new RelayCommand(_ => LoadItems());
            AddItemCommand    = new RelayCommand(_ => OpenAddItem());
            EditItemCommand   = new RelayCommand(_ => OpenEditItem(), _ => SelectedItem != null);
            DeleteItemCommand = new RelayCommand(_ => DeleteItem(), _ => SelectedItem != null);
            ShowClaimsCommand = new RelayCommand(_ => OpenClaims(), _ => SelectedItem != null);
            SearchCommand     = new RelayCommand(_ => LoadItems());

            LoadItems();
        }

        #region Command Methods

        private async void LoadItems()
        {
            try
            {
                StatusMessage = "Lade Gegenstände...";
                var url = string.IsNullOrWhiteSpace(SearchText)
                    ? $"{ApiBaseUrl}/Items"
                    : $"{ApiBaseUrl}/Items?search={SearchText}";

                var items = await _httpClient.GetFromJsonAsync<List<Item>>($"{ApiBaseUrl}/Items");
                Items = new ObservableCollection<Item>(items ?? new List<Item>());

                // Clientseitige Filterung nach SearchText
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var filtered = Items.Where(i =>
                        i.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        i.Location.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        i.Category.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                    Items = new ObservableCollection<Item>(filtered);
                }

                StatusMessage = $"{Items.Count} Gegenstand/Gegenstände geladen.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fehler beim Laden: {ex.Message}";
                MessageBox.Show($"API nicht erreichbar:\n{ex.Message}", "Verbindungsfehler",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenAddItem()
        {
            var vm = new ItemDetailViewModel(null, _httpClient, ApiBaseUrl);
            var window = new ItemDetailWindow { DataContext = vm };
            vm.CloseAction = () => { window.Close(); LoadItems(); };
            window.ShowDialog();
        }

        private void OpenEditItem()
        {
            if (SelectedItem == null) return;
            var vm = new ItemDetailViewModel(SelectedItem, _httpClient, ApiBaseUrl);
            var window = new ItemDetailWindow { DataContext = vm };
            vm.CloseAction = () => { window.Close(); LoadItems(); };
            window.ShowDialog();
        }

        private async void DeleteItem()
        {
            if (SelectedItem == null) return;

            // Sicherheitsabfrage
            var result = MessageBox.Show(
                $"Gegenstand \"{SelectedItem.Title}\" wirklich löschen?\nAlle zugehörigen Ansprüche werden ebenfalls gelöscht.",
                "Löschen bestätigen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/Items/{SelectedItem.Id}");
                if (response.IsSuccessStatusCode)
                {
                    StatusMessage = $"\"{SelectedItem.Title}\" wurde gelöscht.";
                    LoadItems();
                }
                else
                {
                    StatusMessage = "Fehler beim Löschen.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fehler: {ex.Message}";
            }
        }

        private void OpenClaims()
        {
            if (SelectedItem == null) return;
            var vm = new ClaimsViewModel(SelectedItem, _httpClient, ApiBaseUrl);
            var window = new ClaimsWindow { DataContext = vm };
            window.ShowDialog();
        }

        #endregion
    }
}

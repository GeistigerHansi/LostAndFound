using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Input;
using LostAndFound.WPF.Model;

namespace LostAndFound.WPF.ViewModel
{
    public class ItemDetailViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly bool _isEditMode;

        public Action? CloseAction { get; set; }

        #region Properties

        private int _id;
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        private string _location = string.Empty;
        public string Location
        {
            get => _location;
            set { _location = value; OnPropertyChanged(nameof(Location)); }
        }

        private DateTime _foundDate = DateTime.Now;
        public DateTime FoundDate
        {
            get => _foundDate;
            set { _foundDate = value; OnPropertyChanged(nameof(FoundDate)); }
        }

        private string _category = "Other";
        public string Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(nameof(Category)); }
        }

        private string _status = "Unclaimed";
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        public string WindowTitle => _isEditMode ? "Gegenstand bearbeiten" : "Neuen Gegenstand erfassen";

        public List<string> Categories { get; } = new()
        {
            "Electronics", "Clothing", "Keys", "Wallet", "Bag", "Documents", "Jewelry", "Other"
        };

        public List<string> Statuses { get; } = new()
        {
            "Unclaimed", "Claimed", "Archived"
        };

        #endregion

        #region Commands
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        #endregion

        public ItemDetailViewModel(Item? item, HttpClient httpClient, string apiBaseUrl)
        {
            _httpClient = httpClient;
            _apiBaseUrl = apiBaseUrl;
            _isEditMode = item != null;

            if (item != null)
            {
                Id          = item.Id;
                Title       = item.Title;
                Description = item.Description;
                Location    = item.Location;
                FoundDate   = item.FoundDate;
                Category    = item.Category;
                Status      = item.Status;
            }

            SaveCommand   = new RelayCommand(_ => Save(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => CloseAction?.Invoke());
        }

        private bool CanSave() =>
            !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Location);

        private async void Save()
        {
            var item = new Item
            {
                Id          = Id,
                Title       = Title,
                Description = Description,
                Location    = Location,
                FoundDate   = FoundDate,
                Category    = Category,
                Status      = Status
            };

            try
            {
                if (_isEditMode)
                {
                    var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/Items/{Id}", item);
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Fehler beim Speichern.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/Items", item);
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Fehler beim Erstellen.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Verbindungsfehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

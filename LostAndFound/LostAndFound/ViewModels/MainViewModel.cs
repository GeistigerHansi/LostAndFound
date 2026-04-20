using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LostAndFound.Models;
using LostAndFound.Services;

namespace LostAndFound.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _api;

        public ObservableCollection<ItemModel> Items { get; } = new();

        private ItemModel? _selectedItem;
        public ItemModel? SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
        }

        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }

        public MainViewModel()
        {
            // TODO: adjust base address if API runs on different port
            _api = new ApiService("https://localhost:7189/");

            LoadCommand = new RelayCommand(async _ => await LoadAsync());
            AddCommand = new RelayCommand(async _ => await AddAsync());
            DeleteCommand = new RelayCommand(async _ => await DeleteAsync(), _ => SelectedItem != null);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public async Task LoadAsync()
        {
            Items.Clear();
            var items = await _api.GetItemsAsync();
            foreach (var it in items) Items.Add(it);
        }

        public async Task AddAsync()
        {
            if (SelectedItem == null) return;
            if (string.IsNullOrWhiteSpace(SelectedItem.Name))
            {
                MessageBox.Show("Name ist erforderlich.");
                return;
            }
            var added = await _api.AddItemAsync(SelectedItem);
            if (added != null) Items.Add(added);
        }

        public async Task DeleteAsync()
        {
            if (SelectedItem == null) return;
            var ok = MessageBox.Show("Willst du das Item löschen?", "Bestätigung", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            if (!ok) return;
            var success = await _api.DeleteItemAsync(SelectedItem.Id);
            if (success) Items.Remove(SelectedItem);
        }
    }
}

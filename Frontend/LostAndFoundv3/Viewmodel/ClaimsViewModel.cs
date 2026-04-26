using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Input;
using LostAndFound.WPF.Model;

namespace LostAndFound.WPF.ViewModel
{
    public class ClaimsViewModel : INotifyPropertyChanged
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
        private readonly Item _item;

        public string ItemTitle => $"Ansprüche für: {_item.Title}";

        #region Properties

        private ObservableCollection<Claim> _claims = new();
        public ObservableCollection<Claim> Claims
        {
            get => _claims;
            set { _claims = value; OnPropertyChanged(nameof(Claims)); }
        }

        private Claim? _selectedClaim;
        public Claim? SelectedClaim
        {
            get => _selectedClaim;
            set { _selectedClaim = value; OnPropertyChanged(nameof(SelectedClaim)); OnPropertyChanged(nameof(IsEditing)); }
        }

        // Eingabefelder für neuen / bearbeiteten Claim
        private string _claimantName = string.Empty;
        public string ClaimantName
        {
            get => _claimantName;
            set { _claimantName = value; OnPropertyChanged(nameof(ClaimantName)); }
        }

        private string _claimantContact = string.Empty;
        public string ClaimantContact
        {
            get => _claimantContact;
            set { _claimantContact = value; OnPropertyChanged(nameof(ClaimantContact)); }
        }

        private string _claimDescription = string.Empty;
        public string ClaimDescription
        {
            get => _claimDescription;
            set { _claimDescription = value; OnPropertyChanged(nameof(ClaimDescription)); }
        }

        private bool _isApproved;
        public bool IsApproved
        {
            get => _isApproved;
            set { _isApproved = value; OnPropertyChanged(nameof(IsApproved)); }
        }

        public bool IsEditing => SelectedClaim != null;

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(nameof(StatusMessage)); }
        }

        #endregion

        #region Commands
        public ICommand LoadClaimsCommand { get; }
        public ICommand AddClaimCommand { get; }
        public ICommand SaveClaimCommand { get; }
        public ICommand DeleteClaimCommand { get; }
        public ICommand SelectClaimCommand { get; }
        public ICommand ClearFormCommand { get; }
        #endregion

        public ClaimsViewModel(Item item, HttpClient httpClient, string apiBaseUrl)
        {
            _item       = item;
            _httpClient = httpClient;
            _apiBaseUrl = apiBaseUrl;

            LoadClaimsCommand  = new RelayCommand(_ => LoadClaims());
            AddClaimCommand    = new RelayCommand(_ => AddClaim(), _ => CanSave());
            SaveClaimCommand   = new RelayCommand(_ => SaveClaim(), _ => SelectedClaim != null && CanSave());
            DeleteClaimCommand = new RelayCommand(_ => DeleteClaim(), _ => SelectedClaim != null);
            SelectClaimCommand = new RelayCommand(claim => SelectClaim(claim as Claim));
            ClearFormCommand   = new RelayCommand(_ => ClearForm());

            LoadClaims();
        }

        private async void LoadClaims()
        {
            try
            {
                var claims = await _httpClient.GetFromJsonAsync<List<Claim>>(
                    $"{_apiBaseUrl}/Claims/ByItem/{_item.Id}");
                Claims = new ObservableCollection<Claim>(claims ?? new List<Claim>());
                StatusMessage = $"{Claims.Count} Anspruch/Ansprüche gefunden.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fehler: {ex.Message}";
            }
        }

        private void SelectClaim(Claim? claim)
        {
            SelectedClaim    = claim;
            if (claim == null) return;
            ClaimantName     = claim.ClaimantName;
            ClaimantContact  = claim.ClaimantContact;
            ClaimDescription = claim.Description;
            IsApproved       = claim.IsApproved;
        }

        private bool CanSave() =>
            !string.IsNullOrWhiteSpace(ClaimantName) && !string.IsNullOrWhiteSpace(ClaimantContact);

        private async void AddClaim()
        {
            var claim = new Claim
            {
                ItemId          = _item.Id,
                ClaimantName    = ClaimantName,
                ClaimantContact = ClaimantContact,
                Description     = ClaimDescription,
                ClaimDate       = DateTime.Now,
                IsApproved      = IsApproved
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/Claims", claim);
                if (response.IsSuccessStatusCode)
                {
                    StatusMessage = "Anspruch hinzugefügt.";
                    ClearForm();
                    LoadClaims();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fehler: {ex.Message}";
            }
        }

        private async void SaveClaim()
        {
            if (SelectedClaim == null) return;

            SelectedClaim.ClaimantName    = ClaimantName;
            SelectedClaim.ClaimantContact = ClaimantContact;
            SelectedClaim.Description     = ClaimDescription;
            SelectedClaim.IsApproved      = IsApproved;

            try
            {
                var response = await _httpClient.PutAsJsonAsync(
                    $"{_apiBaseUrl}/Claims/{SelectedClaim.Id}", SelectedClaim);
                if (response.IsSuccessStatusCode)
                {
                    StatusMessage = "Anspruch gespeichert.";
                    ClearForm();
                    LoadClaims();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fehler: {ex.Message}";
            }
        }

        private async void DeleteClaim()
        {
            if (SelectedClaim == null) return;

            var result = MessageBox.Show(
                $"Anspruch von \"{SelectedClaim.ClaimantName}\" wirklich löschen?",
                "Löschen bestätigen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/Claims/{SelectedClaim.Id}");
                if (response.IsSuccessStatusCode)
                {
                    StatusMessage = "Anspruch gelöscht.";
                    ClearForm();
                    LoadClaims();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fehler: {ex.Message}";
            }
        }

        private void ClearForm()
        {
            SelectedClaim    = null;
            ClaimantName     = string.Empty;
            ClaimantContact  = string.Empty;
            ClaimDescription = string.Empty;
            IsApproved       = false;
        }
    }
}

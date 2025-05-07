
using PokemonAlbum.Helpers;
using PokemonAlbum.IO;
using PokemonAlbum.Models;
using PokemonAlbum.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PokemonAlbum.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Card> _cards;
        private List<Card> _allCards = new();
        public ObservableCollection<Card> Cards
        {
            get => _cards;
            set
            {
                _cards = value;
                OnPropertyChanged();
            }
        }
        private readonly CardService _cardService = new CardService(new JsonHelper(), new PokemonApiHelper());
        public ICommand OpenAddWindowCommand { get; }
        public ICommand OpenStatisticsWindowCommand { get; }
        public ICommand DeleteCardCommand { get; }
        public ICommand OpenDetailViewCommand { get; }
        public ICommand ImportCardCommand { get; }
        public ICommand ExportCardCommand { get; }
        public ObservableCollection<string> Rarities { get; set; } = new();
        public ObservableCollection<string> Types { get; set; } = new();
        public ObservableCollection<Set> Sets { get; set; } = new();
        public string SelectedRarity { get; set; }
        public string SelectedType { get; set; }
        public Set SelectedSetFilter { get; set; }
        public ObservableCollection<string> SortOptions { get; set; } = new() { "", "Price", "HP", "Number" };
        public string SelectedSortOption { get; set; }
        public ICommand ApplyFiltersCommand { get; }

        public MainWindowViewModel()
        {
            _ = InitializeAsync();
            OpenAddWindowCommand = new RelayCommand(_ => OpenAddWindow(), _ => true);
            OpenStatisticsWindowCommand = new RelayCommand(_ => OpenStatisticsWindow(), _ => true);
            DeleteCardCommand = new RelayCommand(card => DeleteCard((Card)card), card => card != null);
            OpenDetailViewCommand = new RelayCommand(card => OpenDetailView((Card)card), card => card != null);
            ImportCardCommand = new RelayCommand(_ => ImportCards(), _ => true);
            ExportCardCommand = new RelayCommand(_ => ExportCards(), _ => true);
            ApplyFiltersCommand = new RelayCommand(_ => ApplyFilters(), _ => true);
        }

        private async Task InitializeAsync()
        {
            await _cardService.LoadPokemonNamesAsync();
            await _cardService.LoadPokemonSetsAsync();

            var cards = await _cardService.GetCardsAsync();

            _allCards = cards.ToList();
            Cards = new ObservableCollection<Card>(_allCards);

            Rarities = new ObservableCollection<string>(_allCards
                .Select(c => c.Rarity)
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Distinct()
                .Append("")
                .OrderBy(r => r));

            Types = new ObservableCollection<string>(_allCards
                .Select(c => c.Type)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct()
                .Append("")
                .OrderBy(t => t));

            Sets = new ObservableCollection<Set>(await _cardService.GetSetsAsync());
            OnPropertyChanged(nameof(Sets));
            OnPropertyChanged(nameof(Rarities));
            OnPropertyChanged(nameof(Types));
            OnPropertyChanged(nameof(Sets));
        }

        private void ApplyFilters()
        {
            var filtered = _allCards.AsEnumerable();

            if (!string.IsNullOrEmpty(SelectedRarity))
                filtered = filtered.Where(c => c.Rarity == SelectedRarity);

            if (!string.IsNullOrEmpty(SelectedType))
                filtered = filtered.Where(c => c.Type == SelectedType);

            if (SelectedSetFilter != null && !string.IsNullOrEmpty(SelectedSetFilter.Name))
                filtered = filtered.Where(c => c.Set == SelectedSetFilter.Name);

            filtered = SelectedSortOption switch
            {
                "Price" => filtered.OrderByDescending(c => c.Price),
                "HP" => filtered.OrderByDescending(c => c.HP),
                "Number" => filtered.OrderBy(c => int.TryParse(c.Number, out var n) ? n : int.MaxValue),
                _ => filtered
            };

            Cards = new ObservableCollection<Card>(filtered);
        }

        private ImportMode? AskUserForImportMode()
        {
            var result = MessageBox.Show(
                "How would you like to import cards?\n\nYes = Replace existing\nNo = Add to existing\nCancel = Abort import.",
                "Choose Import Mode",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            return result switch
            {
                MessageBoxResult.Yes => ImportMode.Replace,
                MessageBoxResult.No => ImportMode.Add,
                _ => null
            };
        }

        private async void ExportCards()
        {
            try
            {
                bool success = await Task.Run(() => Exporter.ExportToJson(Cards.ToList()));
                if (success)
                {
                    MessageBox.Show("Export DONE!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export cards: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ImportCards()
        {
            try
            {
                var importMode = AskUserForImportMode();
                if (importMode == null)
                    return;

                var importedCards = await Task.Run(() => Importer.ImportFromJson());

                if (importedCards != null)
                {
                    if (importMode == ImportMode.Replace)
                    {
                        Cards.Clear();
                        _allCards.Clear();
                        _cardService.ClearJson();
                    }

                    foreach (var card in importedCards)
                    {
                        Cards.Add(card);
                        _allCards.Add(card);
                    }
                    _cardService.AddCardsToAlbum(importedCards);
                }

                MessageBox.Show("Import DONE!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to import cards: {ex.Message}", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenAddWindow()
        {
            var addWindow = new CardAddView(_cardService);
            addWindow.ShowDialog();
            _ = InitializeAsync();
        }

        private void OpenStatisticsWindow()
        {
            var statisticsWindow = new StatisticsView(_cardService);
            statisticsWindow.ShowDialog();
        }

        private void DeleteCard(Card card)
        {
            var detailWindow = new CardDeleteView(card, _cardService);
            detailWindow.ShowDialog();
            _ = InitializeAsync();
        }

        private void OpenDetailView(Card card)
        {
            var detailWindow = new CardDetailView(card);
            detailWindow.ShowDialog();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

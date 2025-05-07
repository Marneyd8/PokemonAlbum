using PokemonAlbum.Helpers;
using PokemonAlbum.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PokemonAlbum.ViewModels
{
    public class StatisticsViewModel : INotifyPropertyChanged
    {
        private readonly CardService _cardService;
        private List<Card> _allCards = new();
        private Set _selectedSet;
        public ObservableCollection<Card> MostExpensiveCards { get; set; } = new();
        public ObservableCollection<Set> Sets { get; set; } = new();
        public Set SelectedSet
        {
            get => _selectedSet;
            set
            {
                _selectedSet = value;
                OnPropertyChanged();
                UpdateCompletion();
                UpdateMostExpensiveCards();
            }
        }
        public int TotalCards => _allCards.Count;
        public double TotalValue => _allCards.Sum(c => c.Price);

        private double _completionPercentage;
        public double CompletionPercentage
        {
            get => _completionPercentage;
            set { _completionPercentage = value; OnPropertyChanged(); }
        }

        private string _completionText;
        public string CompletionText
        {
            get => _completionText;
            set { _completionText = value; OnPropertyChanged(); }
        }

        public StatisticsViewModel(CardService cardService)
        {
            _cardService = cardService;
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            _allCards = await _cardService.GetCardsAsync();
            var sets = await _cardService.GetSetsAsync();
            foreach (var set in sets)
                Sets.Add(set);

            var topCards = _allCards.OrderByDescending(c => c.Price).Take(3);
            MostExpensiveCards.Clear();
            foreach (var card in topCards)
                MostExpensiveCards.Add(card);

            OnPropertyChanged(nameof(TotalCards));
            OnPropertyChanged(nameof(TotalValue));
        }

        private void UpdateMostExpensiveCards()
        {
            var filteredCards = _allCards;

            if (SelectedSet != null)
                filteredCards = filteredCards.Where(c => c.Set == SelectedSet.Name).ToList();

            var top3 = filteredCards
                .OrderByDescending(c => c.Price)
                .Take(3)
                .ToList();

            MostExpensiveCards.Clear();
            foreach (var card in top3)
                MostExpensiveCards.Add(card);
        }

        private void UpdateCompletion()
        {
            if (SelectedSet == null)
            {
                CompletionPercentage = 0;
                CompletionText = "Please select a set.";
                return;
            }

            var distinctCards = _allCards
                .Where(c => c.Set == SelectedSet.Name)
                .GroupBy(c => c.Id)
                .Select(g => g.First())
                .ToList();

            var collected = distinctCards.Count();
            var total = _selectedSet.Total;

            CompletionPercentage = total > 0 ? 100.0 * collected / total : 0;
            CompletionText = total > 0
                ? $"You have {collected} / {total} cards in \"{SelectedSet.Name}\""
                : $"No known total for \"{SelectedSet.Name}\"";
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

using PokemonAlbum.Helpers;
using PokemonAlbum.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace PokemonAlbum.ViewModels
{
    public class CardAddViewModel : INotifyPropertyChanged
    {
        private readonly CardService _cardService;
        private readonly Action _closeWindow;
        private Set? _selectedSet;
        private string? _cardNumberInput;
        public ObservableCollection<string> PokemonNames { get; } = [];
        public ObservableCollection<Set> PokemonSets { get; } = [];
        public Set? SelectedSet
        {
            get => _selectedSet;
            set
            {
                if (_selectedSet != value)
                {
                    _selectedSet = value;
                    OnPropertyChanged();
                }
            }
        }
        public string? CardNumberInput
        {
            get => _cardNumberInput;
            set
            {
                if (_cardNumberInput != value)
                {
                    _cardNumberInput = value;
                    OnPropertyChanged();
                }
            }
        }
        public ICommand SaveCardCommand { get; }
        public CardAddViewModel(CardService cardService, Action closeWindow)
        {
            _cardService = cardService;
            _closeWindow = closeWindow;
            SaveCardCommand = new RelayCommandAsync(SaveCardAsync, _ => true);
            LoadDropdowns();
        }

        private void LoadDropdowns()
        {
            var (names, sets) = _cardService.LoadDropdownData();

            foreach (var name in names)
            {
                var formatedName = string.IsNullOrWhiteSpace(name) ? "" : char.ToUpper(name[0]) + name[1..].Replace("-", " ");
                PokemonNames.Add(formatedName);
            }

            foreach (var set in sets)
                PokemonSets.Add(set);
        }

        private async Task SaveCardAsync(object? obj)
        {
            await _cardService.AddCardAsync(SelectedSet?.Id ?? "", CardNumberInput ?? "");
            _closeWindow();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}

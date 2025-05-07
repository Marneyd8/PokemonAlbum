using PokemonAlbum.Helpers;
using PokemonAlbum.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PokemonAlbum.ViewModels
{
    public class CardDeleteViewModel
    {
        private Card _card;
        public string CardName => _card?.Name;
        private CardService _cardService;
        public ICommand ConfirmDeleteCommand { get; }
        public ICommand CancelDeleteCommand { get; }

        public CardDeleteViewModel(Card card, CardService cardService)
        {
            _card = card;
            _cardService = cardService;
            ConfirmDeleteCommand = new RelayCommand(ConfirmDelete, _ => true);
            CancelDeleteCommand = new RelayCommand(Cancel, _ => true);
        }

        private async void ConfirmDelete(object obj)
        {
            await _cardService.DeleteCardAsync(_card);
            CloseWindow(obj);
        }

        private void Cancel(object obj)
        {
            CloseWindow(obj);
        }

        private void CloseWindow(object obj)
        {
            if (obj is Window window)
                window.Close();
        }
    }
}

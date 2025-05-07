using PokemonAlbum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonAlbum.ViewModels
{
    public class CardDetailViewModel
    {
        public Card Card { get; }
        public CardDetailViewModel(Card card)
        {
            Card = card;
        }
    }
}

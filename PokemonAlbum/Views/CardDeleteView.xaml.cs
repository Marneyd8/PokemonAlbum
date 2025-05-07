using PokemonAlbum.Helpers;
using PokemonAlbum.Models;
using PokemonAlbum.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PokemonAlbum.Views
{
    /// <summary>
    /// Interaction logic for CardDeleteView.xaml
    /// </summary>
    public partial class CardDeleteView : Window
    {
        public CardDeleteView(Card card, CardService cardService)
        {
            InitializeComponent();
            DataContext = new CardDeleteViewModel(card, cardService);
        }
    }
}

using PokemonAlbum.Models;
using PokemonAlbum.ViewModels;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for CardDetailView.xaml
    /// </summary>
    public partial class CardDetailView : Window
    {
        public CardDetailView(Card card)
        {
            InitializeComponent();
            DataContext = new CardDetailViewModel(card);
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

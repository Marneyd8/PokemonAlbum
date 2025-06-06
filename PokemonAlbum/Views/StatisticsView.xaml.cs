﻿using PokemonAlbum.Helpers;
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
    /// Interaction logic for StatisticsView.xaml
    /// </summary>
    public partial class StatisticsView : Window
    {
        public StatisticsView(CardService cardService)
        {
            InitializeComponent();
            DataContext = new StatisticsViewModel(cardService);
        }
    }
}

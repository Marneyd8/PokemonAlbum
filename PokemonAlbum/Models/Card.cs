using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PokemonAlbum.Models
{
    public class Card
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; }
        public string Type { get; set; }
        public int HP { get; set; }
        public string Set { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double Price { get; set; }
    }
}

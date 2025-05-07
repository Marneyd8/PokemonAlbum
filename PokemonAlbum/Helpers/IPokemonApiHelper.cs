using PokemonAlbum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonAlbum.Helpers
{
    public interface IPokemonApiHelper
    {
        Task<List<string>> GetPokemonNamesAsync();
        Task<List<Set>> GetPokemonSetsAsync();
        Task<Card?> FetchCardAsync(string set, string number);
    }
}

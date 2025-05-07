using PokemonAlbum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonAlbum.Helpers
{
    public interface IJsonHelper
    {
        Task ClearJson();
        Task<List<Card>> LoadAlbumAsync();
        Task SaveAlbumAsync(List<Card> cards);
        Task LoadPokemonNamesAsync(IPokemonApiHelper pokemonApiHelper);
        Task<List<Set>> GetPokemonSetsAsync();
        Task LoadPokemonSetsAsync(IPokemonApiHelper pokemonApiHelper);
        (List<string> Names, List<Set> Sets) LoadDropdownData();
    }
}

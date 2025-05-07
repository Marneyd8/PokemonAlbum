using PokemonAlbum.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonAlbum.Helpers
{
    public class CardService
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly IPokemonApiHelper _apiHelper;

        public CardService(IJsonHelper jsonHelper, IPokemonApiHelper apiHelper)
        {
            _jsonHelper = jsonHelper;
            _apiHelper = apiHelper;
        }

        public async void ClearJson()
        {
            await _jsonHelper.ClearJson();
        }

        public async Task<Card?> AddCardAsync(string set, string number)
        {
            var card = await _apiHelper.FetchCardAsync(set, number);
            if (card == null)
                return null;

            var album = (await _jsonHelper.LoadAlbumAsync()) ?? new List<Card>();
            album.Add(card);

            await _jsonHelper.SaveAlbumAsync(album);
            return card;
        }

        public async Task AddCardsToAlbum(List<Card> cards)
        {
            var album = (await _jsonHelper.LoadAlbumAsync()) ?? new List<Card>();
            foreach(var card in cards)
            {
                    album.Add(card);
            }
            _jsonHelper.SaveAlbumAsync(album);
        }

        public async Task<List<Card>> GetCardsAsync()
        {
            return await _jsonHelper.LoadAlbumAsync() ?? new List<Card>();
        }

        public async Task<List<Set>> GetSetsAsync()
        {
            return await _jsonHelper.GetPokemonSetsAsync();
        }

        public async Task DeleteCardAsync(Card cardToDelete)
        {
            var album = await _jsonHelper.LoadAlbumAsync() ?? new List<Card>();

            var card = album.FirstOrDefault(c =>
                c.Name == cardToDelete.Name &&
                c.Set == cardToDelete.Set &&
                c.Number == cardToDelete.Number);

            if (card != null)
            {
                album.Remove(card);
                await _jsonHelper.SaveAlbumAsync(album);
            }
        }

        public (List<string> Names, List<Set> Sets) LoadDropdownData()
        {
            return _jsonHelper.LoadDropdownData();
        }

        public async Task LoadPokemonNamesAsync()
        {
            await _jsonHelper.LoadPokemonNamesAsync(_apiHelper);
        }

        public async Task LoadPokemonSetsAsync()
        {
            await _jsonHelper.LoadPokemonSetsAsync(_apiHelper);
        }
    }
}

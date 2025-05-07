using PokemonAlbum.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PokemonAlbum.Helpers
{
    public class PokemonApiHelper : IPokemonApiHelper
    {
        private static readonly HttpClient httpClient = new();

        public async Task<List<string>> GetPokemonNamesAsync()
        {
            var response = await httpClient.GetAsync("https://pokeapi.co/api/v2/pokemon?limit=100000");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var names = doc.RootElement.GetProperty("results");
            return names
                .EnumerateArray()
                .Select(p => p.GetProperty("name").GetString())
                .Where(name => name != null)
                .Cast<string>()
                .ToList();
        }

        public async Task<List<Set>> GetPokemonSetsAsync()
        {
            var response = await httpClient.GetAsync("https://api.pokemontcg.io/v2/sets");
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);
            var sets = doc.RootElement.GetProperty("data").EnumerateArray();
            return [.. sets
                .Select(set =>
                {
                    var id = set.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var name = set.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : null;
                    int total = set.TryGetProperty("printedTotal", out var printedTotalProp) && printedTotalProp.TryGetInt32(out var printedTotal)
                        ? printedTotal
                        : set.TryGetProperty("total", out var totalProp) && totalProp.TryGetInt32(out var totalF) ? totalF : 0;
                    return new Set
                    {
                        Id = id ?? "",
                        Name = name ?? "",
                        Total = total
                    };
                })
                .OrderBy(s => s.Name)];
        }

        public async Task<Card?> FetchCardAsync(string set, string number)
        {
            try
            {
                var query = $"https://api.pokemontcg.io/v2/cards?q=set.id:{set} number:{number}";
                var response = await httpClient.GetAsync(query);
                response.EnsureSuccessStatusCode();

                // {"data":[{"id":"swsh1-1","name":"Celebi V","supertype":"Pokémon","subtypes":["Basic","V"],"hp":"180","types":["Grass"],"rules":["V rule: When your Pokémon V is Knocked Out, your opponent takes 2 Prize cards."],"attacks":[{"name":"Find a Friend","cost":["Grass"],"convertedEnergyCost":1,"damage":"","text":"Search your deck for up to 2 Pokémon, reveal them, and put them into your hand. Then, shuffle your deck."},{"name":"Line Force","cost":["Grass","Colorless"],"convertedEnergyCost":2,"damage":"50+","text":"This attack does 20 more damage for each of your Benched Pokémon."}],"weaknesses":[{"type":"Fire","value":"×2"}],"retreatCost":["Colorless"],"convertedRetreatCost":1,"set":{"id":"swsh1","name":"Sword & Shield","series":"Sword & Shield","printedTotal":202,"total":216,"legalities":{"unlimited":"Legal","expanded":"Legal"},"ptcgoCode":"SSH","releaseDate":"2020/02/07","updatedAt":"2020/08/14 09:35:00","images":{"symbol":"https://images.pokemontcg.io/swsh1/symbol.png","logo":"https://images.pokemontcg.io/swsh1/logo.png"}},"number":"1","artist":"PLANETA Igarashi","rarity":"Rare Holo V","nationalPokedexNumbers":[251],"legalities":{"unlimited":"Legal","expanded":"Legal"},"regulationMark":"D","images":{"small":"https://images.pokemontcg.io/swsh1/1.png","large":"https://images.pokemontcg.io/swsh1/1_hires.png"},"tcgplayer":{"url":"https://prices.pokemontcg.io/tcgplayer/swsh1-1","updatedAt":"2025/05/05","prices":{"holofoil":{"low":0.5,"mid":1.52,"high":7.99,"market":1.63,"directLow":2.16}}},"cardmarket":{"url":"https://prices.pokemontcg.io/cardmarket/swsh1-1","updatedAt":"2025/05/05","prices":{"averageSellPrice":1.85,"lowPrice":0.5,"trendPrice":1.93,"germanProLow":0.0,"suggestedPrice":0.0,"reverseHoloSell":0.0,"reverseHoloLow":0.0,"reverseHoloTrend":4.67,"lowPriceExPlus":1.0,"avg1":0.9,"avg7":1.74,"avg30":1.9,"reverseHoloAvg1":3.45,"reverseHoloAvg7":4.69,"reverseHoloAvg30":4.55}}}],"page":1,"pageSize":250,"count":1,"totalCount":1}

                using var stream = await response.Content.ReadAsStreamAsync();
                var doc = await JsonDocument.ParseAsync(stream);
                var cardJson = doc.RootElement.GetProperty("data")[0];
                double price = GetPriceWithFallback(cardJson);
                return new Card
                {
                    Id = cardJson.GetProperty("id").GetString() ?? string.Empty,
                    Name = cardJson.GetProperty("name").GetString() ?? string.Empty,
                    Type = cardJson.GetProperty("types")[0].GetString() ?? string.Empty,
                    HP = int.TryParse(cardJson.GetProperty("hp").GetString(), out var hp) ? hp : 0,
                    Set = cardJson.GetProperty("set").GetProperty("name").GetString() ?? string.Empty,
                    Number = cardJson.GetProperty("number").GetString() ?? string.Empty,
                    Rarity = cardJson.GetProperty("rarity").GetString() ?? string.Empty,
                    ImageUrl = cardJson.GetProperty("images").GetProperty("small").GetString() ?? string.Empty,
                    Price = price
                };
            }
            catch
            {
                return null;
            }
        }
        private static double GetPriceWithFallback(JsonElement cardJson)
        {
            try
            {
                return cardJson.GetProperty("tcgplayer")
                               .GetProperty("prices")
                               .GetProperty("holofoil")
                               .GetProperty("market")
                               .GetDouble();
            }
            catch
            {
                try
                {
                    return cardJson.GetProperty("tcgplayer")
                                   .GetProperty("prices")
                                   .GetProperty("reverseHolofoil")
                                   .GetProperty("market")
                                   .GetDouble();
                }
                catch
                {
                    return 0.0;
                }
            }
        }
    }
}

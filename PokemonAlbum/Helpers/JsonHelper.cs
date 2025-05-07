using PokemonAlbum.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PokemonAlbum.Helpers
{
    public class JsonHelper : IJsonHelper
    {
        private static readonly string AlbumFilePath = "Album.json";

        private static readonly string PokemonNamesFilePath = "PokemonNames.json";
        private static readonly string PokemonSetsFilePath = "PokemonSets.json";

        public async Task ClearJson()
        {
            await File.WriteAllTextAsync(AlbumFilePath, "[]");
        }

        public virtual async Task<List<Card>> LoadAlbumAsync()
        {
            if (!File.Exists(AlbumFilePath))
                return new List<Card>();
            var json = await File.ReadAllTextAsync(AlbumFilePath);
            return JsonSerializer.Deserialize<List<Card>>(json) ?? new List<Card>();
        }

        public virtual async Task SaveAlbumAsync(List<Card> cards)
        {
            var json = JsonSerializer.Serialize(cards, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(AlbumFilePath, json);
        }

        public async Task LoadPokemonNamesAsync(IPokemonApiHelper pokemonApiHelper)
        {
            List<string> existingNames = new List<string>();
            if (File.Exists(PokemonNamesFilePath))
            {
                var json = await File.ReadAllTextAsync(PokemonNamesFilePath);
                existingNames = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }

            var newNames = await pokemonApiHelper.GetPokemonNamesAsync();

            var updatedNames = existingNames.Concat(newNames)
                                            .Distinct()
                                            .OrderBy(name => name)
                                            .ToList();

            var updatedJson = JsonSerializer.Serialize(updatedNames, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(PokemonNamesFilePath, updatedJson);
        }

        public async Task<List<Set>> GetPokemonSetsAsync()
        {
            List<Set> existingSets = new List<Set>();
            if (File.Exists(PokemonSetsFilePath))
            {
                var json = await File.ReadAllTextAsync(PokemonSetsFilePath);
                existingSets = JsonSerializer.Deserialize<List<Set>>(json) ?? new List<Set>();
            }
            var updatedSets = existingSets
                .GroupBy(s => s.Id)
                .Select(g => g.First())
                .OrderBy(s => s.Name)
                .ToList();

            var updatedJson = JsonSerializer.Serialize(updatedSets, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(PokemonSetsFilePath, updatedJson);
            return updatedSets;
        }

        public async Task LoadPokemonSetsAsync(IPokemonApiHelper pokemonApiHelper)
        {
            List<Set> existingSets = new List<Set>();
            if (File.Exists(PokemonSetsFilePath))
            {
                var json = await File.ReadAllTextAsync(PokemonSetsFilePath);
                existingSets = JsonSerializer.Deserialize<List<Set>>(json) ?? new List<Set>();
            }

            var newSets = await pokemonApiHelper.GetPokemonSetsAsync();
            var updatedSets = existingSets.Concat(newSets)
                                         .GroupBy(s => s.Id)
                                         .Select(g => g.First())
                                         .OrderBy(s => s.Name)
                                         .ToList();

            var updatedJson = JsonSerializer.Serialize(updatedSets, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(PokemonSetsFilePath, updatedJson);
        }
        public (List<string> Names, List<Set> Sets) LoadDropdownData()
        {
            List<string> names = new();
            List<Set> sets = new();

            if (File.Exists(PokemonNamesFilePath))
            {
                var namesJson = File.ReadAllTextAsync(PokemonNamesFilePath).Result;
                names = JsonSerializer.Deserialize<List<string>>(namesJson) ?? new List<string>();
            }

            if (File.Exists(PokemonSetsFilePath))
            {
                var setsJson = File.ReadAllText(PokemonSetsFilePath);
                sets = JsonSerializer.Deserialize<List<Set>>(setsJson) ?? new List<Set>();
            }

            return (names.OrderBy(n => n).ToList(), sets.OrderBy(s => s.Name).ToList());
        }
    }
}

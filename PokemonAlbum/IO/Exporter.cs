using Microsoft.Win32;
using PokemonAlbum.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PokemonAlbum.IO
{
    public static class Exporter
    {
        public static async Task<bool> ExportToJson(List<Card> cards)
        {
            var saveDialog = new SaveFileDialog
            {
                Title = "Export Pokémon Cards",
                Filter = "JSON files (*.json)|*.json",
                FileName = "Album.json",
                DefaultExt = ".json"
            };

            if (saveDialog.ShowDialog() == true)
            {
                string filePath = saveDialog.FileName;

                await Task.Run(() =>
                {
                    var json = JsonSerializer.Serialize(cards, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(filePath, json);
                });

                return true;
            }

            return false;
        }
    }
}

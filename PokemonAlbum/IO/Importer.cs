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
    public static class Importer
    {
        public static async Task<List<Card>?> ImportFromJson()
        {
            var openDialog = new OpenFileDialog
            {
                Title = "Import Pokémon Cards",
                Filter = "JSON files (*.json)|*.json",
                DefaultExt = ".json"
            };

            if (openDialog.ShowDialog() == true)
            {
                string filePath = openDialog.FileName;

                return await Task.Run(() =>
                {
                    var json = File.ReadAllText(filePath);
                    return JsonSerializer.Deserialize<List<Card>>(json);
                });
            }

            return null;
        }
    }
}

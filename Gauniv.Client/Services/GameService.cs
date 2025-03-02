using Gauniv.Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Gauniv.Client.Services
{
    public class GameService
    {
        private readonly HttpClient _httpClient;

        public GameService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public GameService()
        {
        }

        public async Task<List<GameModel>> GetGamesAsync(int page, int pageSize, string search = "")
        {
            try
            {
                string url = $"api/1.0.0/games?offset={(page - 1) * pageSize}&limit={pageSize}";
                if (!string.IsNullOrEmpty(search))
                    url += $"&search={search}";

                return await _httpClient.GetFromJsonAsync<List<GameModel>>(url) ?? new List<GameModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des jeux : {ex.Message}");
                return new List<GameModel>();
            }
        }

        public async Task<bool> BuyGameAsync(int gameId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/1.0.0/users/{UserSession.UserId}/games/{gameId}", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'achat du jeu : {ex.Message}");
                return false;
            }
        }

        public async Task DownloadGameAsync(int gameId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/1.0.0/games/download/{gameId}");
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Téléchargement réussi pour le jeu {gameId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du téléchargement du jeu : {ex.Message}");
            }
        }

        public async Task<bool> DeleteGameAsync(int gameId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/1.0.0/games/{gameId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la suppression du jeu : {ex.Message}");
                return false;
            }
        }

        public async Task<List<GameModel>> GetGamesAsync(int page, int pageSize, string search = "", bool owned = false, string category = "", decimal? minPrice = null, decimal? maxPrice = null)
        {
            try
            {
                string url = $"api/1.0.0/games?offset={(page - 1) * pageSize}&limit={pageSize}";
                if (!string.IsNullOrEmpty(search))
                    url += $"&search={search}";
                if (owned)
                    url += $"&owned=true";
                if (!string.IsNullOrEmpty(category))
                    url += $"&category={category}";
                if (minPrice.HasValue)
                    url += $"&minPrice={minPrice.Value}";
                if (maxPrice.HasValue)
                    url += $"&maxPrice={maxPrice.Value}";

                return await _httpClient.GetFromJsonAsync<List<GameModel>>(url) ?? new List<GameModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des jeux : {ex.Message}");
                return new List<GameModel>();
            }
        }

    }
}

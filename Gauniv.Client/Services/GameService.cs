using Gauniv.Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Windows.Networking.NetworkOperators;

namespace Gauniv.Client.Services
{
    public class GameService
    {
        private readonly HttpClient _httpClient;

        public GameService(HttpClient httpClient)
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            _httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost:7209/api/1.0.0/Games") };

            _httpClient = httpClient;
        }

        public GameService()
        {
        }

        public async Task<List<GameModel>> GetGamesAsync(int page, int pageSize, string search = "")
        {
            try
            {
                string url = $"?offset={(page - 1) * pageSize}&limit={pageSize}";
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
                var response = await _httpClient.PostAsync($"users/{UserSession.UserId}/games/{gameId}", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'achat du jeu : {ex.Message}");
                return false;
            }
        }
    }
}

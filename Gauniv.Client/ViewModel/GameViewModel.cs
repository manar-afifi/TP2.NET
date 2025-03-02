using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Gauniv.Client.Models;
using Gauniv.Client.Services;
using Microsoft.Maui.Controls;

namespace Gauniv.Client.ViewModel
{
    public class GameViewModel : BaseViewModel
    {
        private readonly GameService _gameService;
        private int _page = 1;
        private const int PageSize = 10;

        public ObservableCollection<GameModel> Games { get; set; } = new();
        public ObservableCollection<GameModel> OwnedGames { get; set; } = new();

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                LoadGames(true);
            }
        }

        public ICommand LoadMoreCommand { get; }
        public ICommand BuyGameCommand { get; }
        public ICommand DownloadGameCommand { get; }
        public ICommand PlayGameCommand { get; }
        public ICommand StopGameCommand { get; }
        public ICommand DeleteGameCommand { get; }
        public ICommand UpdateProfileCommand { get; }

        public GameViewModel(GameService gameService)
        {
            _gameService = gameService;
            LoadMoreCommand = new Command(async () => await LoadGames());
            BuyGameCommand = new Command<int>(async (gameId) => await BuyGame(gameId));
            DownloadGameCommand = new Command<int>(async (gameId) => await DownloadGame(gameId));
            PlayGameCommand = new Command<int>((gameId) => PlayGame(gameId));
            StopGameCommand = new Command<int>((gameId) => StopGame(gameId));
            DeleteGameCommand = new Command<int>(async (gameId) => await DeleteGame(gameId));
            UpdateProfileCommand = new Command(async () => await UpdateProfile());
            LoadGames();
        }

        private async Task LoadGames(bool reset = false)
        {
            if (reset) _page = 1;
            var newGames = await _gameService.GetGamesAsync(_page, PageSize, SearchText);
            if (reset) Games.Clear();
            foreach (var game in newGames) Games.Add(game);
            _page++;
        }

        private async Task BuyGame(int gameId)
        {
            bool success = await _gameService.BuyGameAsync(gameId);
            if (success)
            {
                await Application.Current.MainPage.DisplayAlert("Succès", "Jeu acheté !", "OK");
                LoadGames();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Achat échoué.", "OK");
            }
        }

        private async Task DownloadGame(int gameId)
        {
            await _gameService.DownloadGameAsync(gameId);
            await Application.Current.MainPage.DisplayAlert("Succès", "Jeu téléchargé !", "OK");
        }

        private void PlayGame(int gameId)
        {
            var game = Games.FirstOrDefault(g => g.Id == gameId);
            if (game != null && game.IsDownloaded)
            {
                game.Status = "InProgress";
                System.Diagnostics.Debug.WriteLine($"Lancement du jeu {gameId}");
            }
        }

        private void StopGame(int gameId)
        {
            var game = Games.FirstOrDefault(g => g.Id == gameId);
            if (game != null && game.Status == "InProgress")
            {
                game.Status = "Ready";
                System.Diagnostics.Debug.WriteLine($"Arrêt forcé du jeu {gameId}");
            }
        }

        private async Task DeleteGame(int gameId)
        {
            await _gameService.DeleteGameAsync(gameId);
            await Application.Current.MainPage.DisplayAlert("Succès", "Jeu supprimé !", "OK");
            LoadGames();
        }

        private async Task UpdateProfile()
        {
            await Application.Current.MainPage.DisplayAlert("Mise à jour", "Profil mis à jour avec succès.", "OK");
        }
    }
}

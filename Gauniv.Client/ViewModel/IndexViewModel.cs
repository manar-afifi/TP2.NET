using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Gauniv.Client.Models;
using Gauniv.Client.Services;
using Microsoft.Maui.Controls;

namespace Gauniv.Client.ViewModel
{
    public class IndexViewModel : BaseViewModel
    {
        private readonly GameService _gameService;
        private int _page = 1;
        private const int PageSize = 10;

        // Utilisation de la bonne déclaration sans ambiguïté
        private ObservableCollection<GameModel> _games;
        public ObservableCollection<GameModel> Games
        {
            get => _games;
            set
            {
                _games = value;
                OnPropertyChanged();
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    LoadGames(); // Recharge les jeux quand le texte change
                }
            }
        }

        public ICommand LoadMoreCommand { get; }
        public ICommand BuyGameCommand { get; }

        public IndexViewModel()
        {
            _gameService = new GameService();
            Games = new ObservableCollection<GameModel>();
            LoadMoreCommand = new Command(async () => await LoadGames());
            BuyGameCommand = new Command<int>(async (gameId) => await BuyGame(gameId));

            LoadGames();
        }

        private async Task LoadGames()
        {
            var newGames = await _gameService.GetGamesAsync(_page, PageSize, SearchText);
            Console.WriteLine($"Nombre de jeux récupérés : {newGames.Count}");
            if (newGames != null)
            {
                foreach (var game in newGames)

                {
                    Games.Add(game);
                }
                _page++;
            }
        }

        private async Task BuyGame(int gameId)
        {
            bool success = await _gameService.BuyGameAsync(gameId);
            if (success)
            {
                await Application.Current.MainPage.DisplayAlert("Succès", "Jeu acheté !", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "L'achat a échoué.", "OK");
            }
        }
    }
}

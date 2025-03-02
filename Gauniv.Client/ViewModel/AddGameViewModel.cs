/*using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gauniv.Client.Services;
using System;
using System.Threading.Tasks;

namespace Gauniv.Client.ViewModel
{
    public partial class AddGameViewModel : ObservableObject
    {
        private readonly GameService _gameService;

        [ObservableProperty]
        private GameDto _newGame;

        public AddGameViewModel()
        {
        }

        public AddGameViewModel(GameService gameService)
        {
            _gameService = gameService;
            _newGame = new GameDto();
            AddGameCommand = new RelayCommand(OnAddGame);
        }

        public IRelayCommand AddGameCommand { get; }

        private async void OnAddGame()
        {
            if (string.IsNullOrEmpty(_newGame.Name) || string.IsNullOrEmpty(_newGame.Description) || _newGame.Price == 0)
            {
                // Validation
                return;
            }

            // Appel API pour ajouter le jeu
            await _gameService.AddGameAsync(_newGame);

            // Redirection ou message de confirmation
            // Navigation à la page de liste des jeux ou message de succès
        }
    }
}*/

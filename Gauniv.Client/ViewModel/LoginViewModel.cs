/*using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gauniv.Client.Services;
using Gauniv.WebServer.Dtos;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Gauniv.Client.ViewModel
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly GameService _gameService;

        [ObservableProperty]
        private LoginDto login;

        public LoginViewModel(GameService gameService)
        {
            _gameService = gameService;
            Login = new LoginDto(); // Initialisation de l'objet LoginDto
        }

        [RelayCommand]
        private async Task OnLogin()
        {
            if (string.IsNullOrWhiteSpace(Login.Email) || string.IsNullOrWhiteSpace(Login.Password))
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Veuillez remplir tous les champs.", "OK");
                return;
            }

            // Log pour debug
            System.Diagnostics.Debug.WriteLine($"Tentative de connexion avec Email: {Login.Email}");

            var token = await _gameService.LoginAsync(Login);

            if (!string.IsNullOrEmpty(token))
            {
                await SecureStorage.SetAsync("authToken", token);

                var isAdmin = await _gameService.IsAdminAsync(token);

                // Log pour voir où ça bloque
                System.Diagnostics.Debug.WriteLine($"Connexion réussie - Admin : {isAdmin}");

                await Shell.Current.GoToAsync(isAdmin ? "//addGamePage" : "//IndexPage");

            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Échec de la connexion. Vérifiez vos identifiants.", "OK");
            }
        }
    }
}*/

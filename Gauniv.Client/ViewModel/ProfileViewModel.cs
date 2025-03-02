using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace Gauniv.Client.ViewModel
{
    public class ProfileViewModel : BaseViewModel
    {
        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _installationPath;
        public string InstallationPath
        {
            get => _installationPath;
            set { _installationPath = value; OnPropertyChanged(); }
        }

        public ICommand SaveProfileCommand { get; }

        public ProfileViewModel()
        {
            SaveProfileCommand = new Command(SaveProfile);
        }

        private async void SaveProfile()
        {
            await Application.Current.MainPage.DisplayAlert("Profil", "Profil mis à jour avec succès", "OK");
        }
    }
}

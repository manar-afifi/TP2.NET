using CommunityToolkit.Mvvm.ComponentModel;

namespace Gauniv.WebServer.Dtos
{
    public class LoginDto : ObservableObject
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}

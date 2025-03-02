using Gauniv.Client.ViewModel;

namespace Gauniv.Client.Pages;

public partial class IndexPage : ContentPage
{
    public IndexPage()
    {
        InitializeComponent();
        BindingContext = new IndexViewModel(); 
    }
}

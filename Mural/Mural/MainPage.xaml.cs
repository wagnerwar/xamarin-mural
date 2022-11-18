using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Mural.ViewModel;
using Mural.Model;
using Rg.Plugins.Popup.Services;
namespace Mural
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = new MainPageViewModel(Navigation);

            MessagingCenter.Subscribe<MainPage, String>(this, "Erro", (sender, a) =>
            {
                DisplayAlert("Erro", a, "OK");
            });
            MessagingCenter.Subscribe<MainPage, String>(this, "Sucesso", (sender, a) =>
            {
                DisplayAlert("Sucesso!!!", a, "OK");
            });
            MessagingCenter.Subscribe<MainPage>(this, "ShowLoading", (sender) =>
            {
                var pagina = new PopupPage();
                pagina.CloseWhenBackgroundIsClicked = false;
                PopupNavigation.Instance.PushAsync(pagina);
            });
            MessagingCenter.Subscribe<MainPage>(this, "HideLoading", (sender) =>
            {
                PopupNavigation.Instance.PopAllAsync(true);
            });
        }
    }
}

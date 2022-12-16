using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Mural.Model;
using Mural.ViewModel;
using Xamarin.CommunityToolkit.Extensions;
using Rg.Plugins.Popup.Services;

namespace Mural
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ComentarioPopup : Rg.Plugins.Popup.Pages.PopupPage
    {
        public ComentarioPopup(Postagem postagem)
        {
            this.BindingContext = new ComentarioPopupViewModel(postagem);
            InitializeComponent();
            MessagingCenter.Subscribe<ComentarioPopup, String>(this, "Erro", (sender, a) =>
            {
                this.DisplayToastAsync(a, 3000);
            });
            MessagingCenter.Subscribe<ComentarioPopup>(this, "Fechar", (sender) =>
            {
                PopupNavigation.Instance.PopAllAsync(true);
            });
            MessagingCenter.Subscribe<ComentarioPopup>(this, "Sucesso", (sender) =>
            {
                this.DisplayToastAsync("Comentário enviado com sucesso", 3000);
            });
        }       
    }
}
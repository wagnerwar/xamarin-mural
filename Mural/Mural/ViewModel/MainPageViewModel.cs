using System;
using System.Collections.Generic;
using System.Text;
using Mural.Model;
using Xamarin.Forms;
using Mural.Service;
using System.Windows.Input;
using System.Threading.Tasks;

namespace Mural.ViewModel
{
    public class MainPageViewModel : BaseViewModel
    {
        private INavigation _navigation { get; set; }
        private BancoService _service { get; set; }
        public ICommand EnviarPostagemCommand { get; set; }
        private String _conteudo;
        public String Conteudo
        {
            get { return _conteudo; }
            set
            {
                _conteudo = value;
                OnPropertyChanged();
            }
        }
        public MainPageViewModel(INavigation navigation)
        {
            _navigation = navigation;
            _service = new BancoService();
            EnviarPostagemCommand = new Command(async () => await EnviarPostagem());
        }
        public async Task EnviarPostagem()
        {
            try
            {
                if(String.IsNullOrEmpty(Conteudo))
                {
                    throw new Exception("Conteúdo deve estar preenchido");
                }
                MessagingCenter.Send<MainPage, String>(new MainPage(), "Sucesso", "Postagem enviada com sucesso");
            }
            catch(Exception ex)
            {
                MessagingCenter.Send<MainPage, String>(new MainPage(), "Erro", ex.Message);
            }
        }
    }
}

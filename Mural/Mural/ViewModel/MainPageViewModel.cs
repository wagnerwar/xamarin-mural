using System;
using System.Collections.Generic;
using System.Text;
using Mural.Model;
using Xamarin.Forms;
using Mural.Service;
using System.Windows.Input;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.Threading;
namespace Mural.ViewModel
{
    public class MainPageViewModel : BaseViewModel
    {
        private INavigation _navigation { get; set; }
        private BancoService _service { get; set; }
        public ICommand EnviarPostagemCommand { get; set; }
        public ICommand EnviarArquivoCommand { get; set; }
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
            EnviarArquivoCommand = new Command(async () => await EnviarArquivo());
        }
        public async Task EnviarPostagem()
        {
            try
            {
                IsLoading = true;                
                if (String.IsNullOrEmpty(Conteudo))
                {
                    throw new Exception("Conteúdo deve estar preenchido");
                }
                MessagingCenter.Send<MainPage>(new MainPage(), "ShowLoading");
                await Task.Delay(TimeSpan.FromSeconds(3));
                MessagingCenter.Send<MainPage>(new MainPage(), "HideLoading");
                IsLoading = false;
                await LimparCampos();
                MessagingCenter.Send<MainPage, String>(new MainPage(), "Sucesso", "Postagem enviada com sucesso");
            }
            catch(Exception ex)
            {
                MessagingCenter.Send<MainPage, String>(new MainPage(), "Erro", ex.Message);
            }
        }
        public async Task EnviarArquivo()
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync();
                await LoadPhotoAsync(photo);
            }
            catch (Exception ex)
            {
                MessagingCenter.Send<MainPage, String>(new MainPage(), "Erro", ex.Message);
            }
        }
        public async Task LimparCampos()
        {
            try
            {
                Conteudo = String.Empty;
            }
            catch (Exception ex)
            {
                MessagingCenter.Send<MainPage, String>(new MainPage(), "Erro", ex.Message);
            }
        }
        async Task LoadPhotoAsync(FileResult photo)
        {
            // canceled
            if (photo == null)
            {
                //PhotoPath = null;
                return;
            }
            // recuperar binário e salvar no banco
            
        }
    }
}

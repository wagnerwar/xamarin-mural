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
using System.IO;
using System.Collections.ObjectModel;
namespace Mural.ViewModel
{
    public class ComentarioPopupViewModel : BaseViewModel
    {
        public ICommand FecharCommand { get; set; }
        public ICommand EnviarComentarioCommand { get; set; }
        public ComentarioPopupViewModel(Postagem post)
        {
            Postagem = post;
            FecharCommand = new Command(async () => await Fechar());
            EnviarComentarioCommand = new Command(async () => await EnviarComentario());
        }
        private Postagem _postagem;
        public Postagem Postagem
        {
            get { return _postagem; }
            set
            {
                _postagem = value;
                OnPropertyChanged();
            }
        }
        private String _texto;
        public String Texto
        {
            get { return _texto; }
            set
            {
                _texto = value;
                OnPropertyChanged();
            }
        }
        private async Task Fechar()
        {
            try
            {
                MessagingCenter.Send<ComentarioPopup>(new ComentarioPopup(Postagem), "Fechar");
            }
            catch (Exception ex)
            {
                MessagingCenter.Send<ComentarioPopup, String>(new ComentarioPopup(Postagem), "Erro", ex.Message);
            }
        }
        private async Task EnviarComentario()
        {
            try
            {
                if (String.IsNullOrEmpty(Texto))
                {
                    throw new Exception("Comentário deve ser preenchido");
                }
                IsLoading = true;
                await Task.Delay(3000);
                IsLoading = false;
                Texto = String.Empty;
                MessagingCenter.Send<ComentarioPopup>(new ComentarioPopup(Postagem), "Sucesso");
            }
            catch (Exception ex)
            {
                MessagingCenter.Send<ComentarioPopup, String>(new ComentarioPopup(Postagem), "Erro", ex.Message);
            }
        }
    }
}

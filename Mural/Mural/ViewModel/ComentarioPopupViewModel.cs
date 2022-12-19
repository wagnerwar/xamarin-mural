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
        private BancoService _service { get; set; }
        public ICommand FecharCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand EnviarComentarioCommand { get; set; }
        public ComentarioPopupViewModel(Postagem post)
        {            
            _service = new BancoService();
            Postagem = post;
            FecharCommand = new Command(async () => await Fechar());
            EnviarComentarioCommand = new Command(async () => await EnviarComentario());
            RefreshCommand = new Command(async () => await RefreshItemsAsync());
            Items = new ObservableCollection<Comentario>();
            var t = Task.Run(() => this.CarregarComentarios());
            t.Wait();
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
        bool isRefreshing;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Comentario> items;
        public ObservableCollection<Comentario> Items
        {
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged();
            }
        }
        private bool _isComentariosLoading;
        public bool IsComentariosLoading
        {
            get
            {
                return this._isComentariosLoading;
            }
            set
            {
                this._isComentariosLoading = value;
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
                Comentario comentario = new Comentario();
                comentario.Conteudo = Texto;
                comentario.postagemId = Postagem.Id;
                _service.InserirComentario(comentario);
                IsLoading = false;
                Texto = String.Empty;
                MessagingCenter.Send<ComentarioPopup>(new ComentarioPopup(Postagem), "Sucesso");
                await CarregarComentarios();
            }
            catch (Exception ex)
            {
                MessagingCenter.Send<ComentarioPopup, String>(new ComentarioPopup(Postagem), "Erro", ex.Message);
            }
        }
        private async Task RefreshItemsAsync()
        {
            IsRefreshing = true;
            await CarregarComentarios();
            IsRefreshing = false;
        }
        public async Task CarregarComentarios()
        {
            try
            {
                Items.Clear();
                IsComentariosLoading = true;
                var dados = await _service.RecuperarComentarios(Postagem.Id);
                if (dados != null && dados.Count > 0)
                {
                    foreach (var d in dados)
                    {
                        Items.Add(d);
                    }
                    IsComentariosLoading = false;
                }
            }
            catch (Exception ex)
            {
                MessagingCenter.Send<ComentarioPopup, String>(new ComentarioPopup(Postagem), "Erro", ex.Message);
            }
        }
    }
}

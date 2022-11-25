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
    public class MainPageViewModel : BaseViewModel
    {
        private int limite = 2;
        private INavigation _navigation { get; set; }
        private BancoService _service { get; set; }
        public ICommand EnviarPostagemCommand { get; set; }
        public ICommand ExcluirPostagemCommand { get; set; }
        public ICommand EnviarArquivoCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand CarregarMaisItensCommand { get; set; }
        private int _page;
        public int Page
        {
            get { return _page; }
            set
            {
                _page = value;
                OnPropertyChanged();
            }
        }
        bool _isLoadingItems;
        public bool IsLoadingItems
        {
            get { return _isLoadingItems; }
            set
            {
                _isLoadingItems = value;
                OnPropertyChanged();
            }
        }
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
        private String _nomeArquivo;
        public String NomeArquivo
        {
            get { return _nomeArquivo; }
            set
            {
                _nomeArquivo = value;
                OnPropertyChanged();
            }
        }
        private byte[] _arquivo;
        public byte[] Arquivo
        {
            get { return _arquivo; }
            set
            {
                _arquivo = value;
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
        private ObservableCollection<Postagem> items;
        public ObservableCollection<Postagem> Items
        {
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged();
            }
        }
        public MainPageViewModel(INavigation navigation)
        {
            Page = 1;
            _navigation = navigation;
            _service = new BancoService();
            EnviarPostagemCommand = new Command(async () => await EnviarPostagem());
            EnviarArquivoCommand = new Command(async () => await EnviarArquivo());
            RefreshCommand = new Command(async () => await RefreshItemsAsync());
            CarregarMaisItensCommand = new Command(async () => await CarregarMaisItens());
            ExcluirPostagemCommand = new Command<Postagem>(async (p) => await ExcluirPostagem(p));
            Items = new ObservableCollection<Postagem>();
            var t = Task.Run( () => this.CarregarPostagens());
            t.Wait();
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
                Postagem postagem = new Postagem();
                postagem.Conteudo = Conteudo;
                postagem.Arquivo = Arquivo;
                // inserir no banco
                _service.InserirPostagem(postagem);
                MessagingCenter.Send<MainPage>(new MainPage(), "HideLoading");
                IsLoading = false;
                await LimparCampos();
                await CarregarPostagens();
                MessagingCenter.Send<MainPage, String>(new MainPage(), "Sucesso", "Postagem enviada com sucesso");
            }
            catch(Exception ex)
            {
                MessagingCenter.Send<MainPage, String>(new MainPage(), "Erro", ex.Message);
            }
        }
        public async Task ExcluirPostagem(Postagem postagem)
        {
            try
            {
                IsLoading = true;                
                MessagingCenter.Send<MainPage>(new MainPage(), "ShowLoading");
                await Task.Delay(TimeSpan.FromSeconds(3));
                _service.ExcluirPostagem(postagem);
                MessagingCenter.Send<MainPage>(new MainPage(), "HideLoading");
                IsLoading = false;
                await LimparCampos();
                await CarregarPostagens();
                MessagingCenter.Send<MainPage, String>(new MainPage(), "Sucesso", "Postagem excluida com sucesso");
            }
            catch (Exception ex)
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
                NomeArquivo = String.Empty;
                Arquivo = null;
            }
            catch (Exception ex)
            {
                MessagingCenter.Send<MainPage, String>(new MainPage(), "Erro", ex.Message);
            }
        }
        public async Task CarregarPostagens()
        {
            try
            {
                Page = 1;
                Items.Clear();
                var dados = await _service.RecuperarPostagens(limite, 1);
                if(dados != null && dados.Count > 0)
                {
                    foreach(var d in dados)
                    {
                        Items.Add(d);
                    }
                }
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
            Arquivo = File.ReadAllBytes(photo.FullPath);
            NomeArquivo = photo.FileName;
        }
        private async Task RefreshItemsAsync()
        {            
            IsRefreshing = true;
            await CarregarPostagens();
            IsRefreshing = false;
        }
        private async Task CarregarMaisItens()
        {
            try
            {
                if (IsLoadingItems == true)
                {
                    return;
                }

                IsLoadingItems = true;
                int TotalPaginas = 1;
                int totalRegistros = _service.RecuperarTotalPostagens();
                if (totalRegistros > limite)
                {
                    decimal divisao = Convert.ToDecimal(totalRegistros / Convert.ToDecimal(limite));
                    var parteInteira = Convert.ToInt32(divisao);
                    if (divisao > parteInteira)
                    {
                        TotalPaginas = parteInteira + 1;
                    }
                    else
                    {
                        TotalPaginas = parteInteira;
                    }
                }
                if (Page < TotalPaginas)
                {
                    Page++;
                    var dados = await _service.RecuperarPostagens(limite, Page);
                    foreach (var d in dados)
                    {
                        Items.Add(d);
                    }
                }
                IsLoadingItems = false;
            }
            catch (Exception ex)
            {
                MessagingCenter.Send<MainPage, String>(new MainPage(), "Erro", ex.Message);
                IsLoading = false;
            }
        }
    }
}

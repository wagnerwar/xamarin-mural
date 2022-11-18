using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;
namespace Mural.Model
{
    public class Postagem
    {
        public int Id { get; set; }
        public String Conteudo { get; set; }
        public byte[] Arquivo { get; set; }
        public ImageSource ArquivoSource
        {
            get
            {
                ImageSource imageaData = ImageSource.FromStream(() => new MemoryStream(Arquivo));
                return imageaData;
            }
        }
    }
}

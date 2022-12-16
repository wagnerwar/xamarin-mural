using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;
namespace Mural.Model
{
    public class Comentario
    {
        public int Id { get; set; }
        public String Conteudo { get; set; }
        public int postagemId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Mural.Model
{
    public class Postagem
    {
        public int Id { get; set; }
        public String Nome { get; set; }
        public String Conteudo { get; set; }
        public byte[] Arquivo { get; set; }
    }
}

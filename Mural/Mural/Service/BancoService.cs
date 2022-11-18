using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Mural.Model;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Threading.Tasks;

namespace Mural.Service
{
    public class BancoService
    {
        private String _banco = "mural.db";
        public string DatabasePath
        {
            get
            {
                var connectionStringBuilder = new SqliteConnectionStringBuilder();
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                String caminho = Path.Combine(basePath, _banco);
                connectionStringBuilder.DataSource = caminho;
                return connectionStringBuilder.ConnectionString;
            }
        }
        public void CriarBanco()
        {
            using (var conexao = new SqliteConnection(DatabasePath))
            {
                conexao.Open();
                try
                {
                    var command = conexao.CreateCommand();
                    command.CommandText =
                @"
                    CREATE TABLE IF NOT EXISTS postagem (
                        id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        conteudo text NOT NULL,
                        arquivo blob null
                    );
                ";
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conexao.Close();
                }
            }
        }
        public void InserirPostagem(Postagem postagem)
        {
            try
            {
                this.CriarBanco();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            using (var conexao = new SqliteConnection(DatabasePath))
            {
                conexao.Open();
                try
                {
                    using (var transaction = conexao.BeginTransaction())
                    {
                        try
                        {

                            var command = conexao.CreateCommand();
                            if (postagem.Arquivo != null)
                            {
                                command.CommandText =
                            String.Format(@"
                            insert into postagem (conteudo, arquivo ) values('{0}', @pic)
                            ", postagem.Conteudo);
                                command.Parameters.Add("@pic", SqliteType.Blob);
                                command.Parameters[0].Value = postagem.Arquivo;
                            }
                            else
                            {
                                command.CommandText =
                            String.Format(@"
                            insert into postagem (conteudo ) values('{0}')
                            ", postagem.Conteudo);
                            }
                            command.ExecuteNonQuery();
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conexao.Close();
                }
            }
        }
        public async Task<List<Postagem>> RecuperarPostagens(int limite, int page = 1)
        {
            try
            {
                this.CriarBanco();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            int offset = 0;
            if (page > 1)
            {
                offset = (page - 1) * limite;
            }
            List<Postagem> lista = new List<Postagem>();
            using (var conexao = new SqliteConnection(DatabasePath))
            {
                conexao.Open();
                try
                {
                    var command = conexao.CreateCommand();
                    command.CommandText =
                    String.Format(@"
                        select id, conteudo, arquivo from postagem limit {0}, {1}
                    ", offset, limite);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Postagem item = new Postagem();
                            item.Id = reader.GetInt32(0);
                            item.Conteudo = reader.GetString(1);
                            MemoryStream outputStream = new MemoryStream();
                            try
                            {
                                if (reader.GetStream(2) != null)
                                {
                                    using (var readStream = reader.GetStream(2))
                                    {
                                        await readStream.CopyToAsync(outputStream);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                            if (outputStream.Length > 0)
                            {
                                item.Arquivo = outputStream.ToArray();
                            }
                            lista.Add(item);
                        }
                    }
                    return lista;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conexao.Close();
                }
            }
        }
        public int RecuperarTotalPostagens()
        {
            int retorno = 0;
            using (var conexao = new SqliteConnection(DatabasePath))
            {
                conexao.Open();
                try
                {
                    var command = conexao.CreateCommand();
                    command.CommandText =
                    String.Format(@"
                        select count(*) from postagem
                    ");
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            retorno = reader.GetInt32(0);
                        }
                    }
                    return retorno;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conexao.Close();
                }
            }
        }
        public void ExcluirPostagem(Postagem postagem)
        {
            try
            {
                this.CriarBanco();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            using (var conexao = new SqliteConnection(DatabasePath))
            {
                conexao.Open();
                try
                {
                    using (var transaction = conexao.BeginTransaction())
                    {
                        try
                        {
                            var command = conexao.CreateCommand();
                            command.CommandText =
                            String.Format(@"
                            delete from postagem where id= '{0}'
                            ", postagem.Id);
                            command.ExecuteNonQuery();
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conexao.Close();
                }
            }
        }
    }
}

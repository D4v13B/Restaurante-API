using MySql.Data.MySqlClient;
using Restaurante.Models;
using System.Data;

namespace Restaurante.Datos
{
    public class Db
    {
        MySqlConnection con;
        MySqlCommand cmd;
        MySqlDataAdapter adapter;
        DataSet ds;
        public Db()
        {
            string cadenaConexion = "Server=193.203.166.22;Database=u949375132_restaurante;User Id=u949375132_gs121;Password=ElielAngelica121;";
            con = new MySqlConnection();
            con.ConnectionString = cadenaConexion;
            cmd = new MySqlCommand();
            cmd.Connection = con;
        }


        public List<Usuario> ObtenerUsuarios()
        {
            List<Usuario> usuarios = new List<Usuario>();
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM usuarios a INNER JOIN usuarios_tipos b ON a.usuarioTipoid = b.id";

                cmd.Connection.Open();
                ds = new DataSet();

                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(ds);

                foreach (DataTable table in ds.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var usuario = new Usuario()
                        {
                            Id = Convert.ToInt32(row["id"]),
                            Nombre = row["nombre"].ToString(),
                            Password = row["password"].ToString(),
                            Email = row["email"].ToString(),
                            UsuarioTipoId = Convert.ToInt32(row["usuarioTipoid"]),
                            UsuarioTipo = new UsuarioTipo()
                            { 
                                Id = Convert.ToInt32(row["usuarioTipoid"]),
                                Tipo = row["tipo"].ToString()
                            }

                        };
                        usuarios.Add(usuario);
                    }
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }

            return usuarios;
        }

        public int SaveUser(UsuarioRequest usuario)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO usuarios(nombre, password, email, telefono, estado, usuarioTipoId)values(@n,@p,@e,@t,1,@st)";
                cmd.Parameters.Add(new MySqlParameter("@n", usuario.Nombre));
                cmd.Parameters.Add(new MySqlParameter("@e", usuario.Email));
                cmd.Parameters.Add(new MySqlParameter("@p", usuario.Password));
                cmd.Parameters.Add(new MySqlParameter("@t", usuario.Telefono));
                cmd.Parameters.Add(new MySqlParameter("@st", usuario.UsuarioTipoId));

                cmd.Connection.Open();
                int insertedId = Convert.ToInt32(cmd.ExecuteNonQuery());
                if (insertedId > 0)
                    return insertedId;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally { cmd.Connection.Close(); }
            return 0;
        }
    }
}

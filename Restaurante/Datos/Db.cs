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
            //string cadenaConexion = "Server=193.203.166.22;Database=u949375132_restaurante;User Id=u949375132_gs121;Password=ElielAngelica121;";
            string cadenaConexion = "Server=localhost;Database=apprestaurante;User Id=root;Password=;";

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
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "ObtenerUsuarios";

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

        // Obtener todos los productos
        public List<Producto> ObtenerProductos()
        {
            List<Producto> productos = new List<Producto>();
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM productos p INNER JOIN producto_tipo pt ON p.productoTipoId = pt.id";

                cmd.Connection.Open();
                ds = new DataSet();

                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(ds);

                foreach (DataTable table in ds.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var producto = new Producto()
                        {
                            Id = Convert.ToInt32(row["id"]),
                            Nombre = row["nombre"].ToString(),
                            Precio = Convert.ToDecimal(row["precio"]),
                            Descripcion = row["descripcion"].ToString(),
                            UnidadMedida = row["unidad_medida"].ToString(),
                            ProductoTipoId = Convert.ToInt32(row["productoTipoId"]),
                            Foto = row["foto"].ToString(),
                            ProductoTipo = new ProductoTipo()
                            {
                                Id = Convert.ToInt32(row["productoTipoId"]),
                                Tipo = row["tipo"].ToString()
                            }
                        };
                        productos.Add(producto);
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

            return productos;
        }

        // Obtener producto por ID
        public Producto ObtenerProductoPorId(int id)
        {
            Producto producto = null;
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM productos p INNER JOIN producto_tipo pt ON p.productoTipoId = pt.id WHERE p.id = @id";
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                ds = new DataSet();

                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    var row = ds.Tables[0].Rows[0];
                    producto = new Producto()
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Nombre = row["nombre"].ToString(),
                        Precio = Convert.ToDecimal(row["precio"]),
                        Descripcion = row["descripcion"].ToString(),
                        UnidadMedida = row["unidad_medida"].ToString(),
                        ProductoTipoId = Convert.ToInt32(row["productoTipoId"]),
                        Foto = row["foto"].ToString(),
                        ProductoTipo = new ProductoTipo()
                        {
                            Id = Convert.ToInt32(row["productoTipoId"]),
                            Tipo = row["tipo"].ToString()
                        }
                    };
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

            return producto;
        }

        // Guardar producto
        public int SaveProduct(ProductoRequest productoRequest)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO productos(nombre, precio, descripcion, unidad_medida, productoTipoId, foto) VALUES(@n, @p, @d, @u, @pt, @f)";
                cmd.Parameters.Add(new MySqlParameter("@n", productoRequest.Nombre));
                cmd.Parameters.Add(new MySqlParameter("@p", productoRequest.Precio));
                cmd.Parameters.Add(new MySqlParameter("@d", productoRequest.Descripcion));
                cmd.Parameters.Add(new MySqlParameter("@u", productoRequest.UnidadMedida));
                cmd.Parameters.Add(new MySqlParameter("@pt", productoRequest.ProductoTipoId));
                cmd.Parameters.Add(new MySqlParameter("@f", productoRequest.Foto));

                cmd.Connection.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        // Actualizar producto
        public int UpdateProduct(int id, ProductoRequest productoRequest)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE productos SET nombre = @n, precio = @p, descripcion = @d, unidad_medida = @u, productoTipoId = @pt, foto = @f WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@id", id));
                cmd.Parameters.Add(new MySqlParameter("@n", productoRequest.Nombre));
                cmd.Parameters.Add(new MySqlParameter("@p", productoRequest.Precio));
                cmd.Parameters.Add(new MySqlParameter("@d", productoRequest.Descripcion));
                cmd.Parameters.Add(new MySqlParameter("@u", productoRequest.UnidadMedida));
                cmd.Parameters.Add(new MySqlParameter("@pt", productoRequest.ProductoTipoId));
                cmd.Parameters.Add(new MySqlParameter("@f", productoRequest.Foto));

                cmd.Connection.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        // Eliminar producto
        public int DeleteProduct(int id)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM productos WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public Usuario? Login(string email, string password)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM usuarios a INNER JOIN usuarios_tipos b ON a.usuarioTipoId = b.id  WHERE email =  @p_email AND password = SHA2(@p_password, 256) LIMIT 1 ";

                cmd.Parameters.AddWithValue("@p_email", email);
                cmd.Parameters.AddWithValue ("@p_password", password);
                cmd.Connection.Open();
                ds = new DataSet();

                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(ds);

                foreach (DataTable table in ds.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        return new Usuario()
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

            return null;
        }
    }
}

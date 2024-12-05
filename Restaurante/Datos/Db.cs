using Heiwa.Models;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using MySqlX.XDevAPI.CRUD;
using Restaurante.Controllers;
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
        private readonly string? cadenaConexion;
        public Db()
        {
            cadenaConexion = AppSettings.Configuration["ConnectionStrings:DefaultConnection"];

            con = new MySqlConnection();
            con.ConnectionString = cadenaConexion;
            cmd = new MySqlCommand();
            cmd.Connection = con;
        }

        public int ActualizarUsuario(int id, UsuarioRequest usuarioRequest)
        {
            try
            {
                // Limpiar parámetros de la consulta anterior (si los hay)
                cmd.Parameters.Clear();

                // Establecer el tipo de comando y la consulta SQL
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
            UPDATE usuarios 
            SET 
                nombre = @nombre, 
                password = @password, 
                email = @email, 
                telefono = @telefono, 
                usuarioTipoId = @usuarioTipoId
            WHERE id = @id";

                cmd.Parameters.Add(new MySqlParameter("@nombre", usuarioRequest.Nombre));
                cmd.Parameters.Add(new MySqlParameter("@password", usuarioRequest.Password));
                cmd.Parameters.Add(new MySqlParameter("@email", usuarioRequest.Email));
                cmd.Parameters.Add(new MySqlParameter("@telefono", usuarioRequest.Telefono ?? (object)DBNull.Value));
                cmd.Parameters.Add(new MySqlParameter("@usuarioTipoId", usuarioRequest.UsuarioTipoId));
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public int EliminarUsuario(int id)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM usuarios WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                return cmd.ExecuteNonQuery(); // Devuelve el número de filas afectadas
            }
            finally
            {
                cmd.Connection.Close();
            }
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
                cmd.Parameters.AddWithValue("@p_password", password);
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

        // Métodos para Ingrediente
        public List<Ingrediente> ObtenerIngredientes()
        {
            var ingredientes = new List<Ingrediente>();

            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM ingredientes";

                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ingredientes.Add(new Ingrediente
                        {
                            Id = reader.GetInt32("id"),
                            Nombre = reader.GetString("nombre"),
                            Stock = reader.GetInt32("stock"),
                            UnidadMedida = reader.GetString("unidad_medida")
                        });
                    }
                }
            }
            finally
            {
                cmd.Connection.Close();
            }

            return ingredientes;
        }

        public List<Ingrediente> ObtenerIngredientesXPlatillo()
        {
            var ingredientes = new List<Ingrediente>();

            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM ingredientes INNER JOIN platillosIngredientes";

                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ingredientes.Add(new Ingrediente
                        {
                            Id = reader.GetInt32("id"),
                            Nombre = reader.GetString("nombre"),
                            Stock = reader.GetInt32("stock"),
                            UnidadMedida = reader.GetString("unidad_medida")
                        });
                    }
                }
            }
            finally
            {
                cmd.Connection.Close();
            }

            return ingredientes;
        }

        public Ingrediente ObtenerIngredientePorId(int id)
        {
            Ingrediente ingrediente = null;

            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM ingredientes WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ingrediente = new Ingrediente
                        {
                            Id = reader.GetInt32("id"),
                            Nombre = reader.GetString("nombre"),
                            Stock = reader.GetInt32("stock"),
                            UnidadMedida = reader.GetString("unidad_medida")
                        };
                    }
                }
            }
            finally
            {
                cmd.Connection.Close();
            }

            return ingrediente;
        }

        public int SaveIngrediente(IngredienteRequest ingredienteRequest)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO ingredientes(nombre, stock, unidad_medida) VALUES(@n, @s, @u)";
                cmd.Parameters.Add(new MySqlParameter("@n", ingredienteRequest.Nombre));
                cmd.Parameters.Add(new MySqlParameter("@s", ingredienteRequest.Stock));
                cmd.Parameters.Add(new MySqlParameter("@u", ingredienteRequest.UnidadMedida));

                cmd.Connection.Open();
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public int UpdateIngrediente(int id, IngredienteRequest ingredienteRequest)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE ingredientes SET nombre = @n, stock = @s, unidad_medida = @u WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@id", id));
                cmd.Parameters.Add(new MySqlParameter("@n", ingredienteRequest.Nombre));
                cmd.Parameters.Add(new MySqlParameter("@s", ingredienteRequest.Stock));
                cmd.Parameters.Add(new MySqlParameter("@u", ingredienteRequest.UnidadMedida));

                cmd.Connection.Open();
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public int DeleteIngrediente(int id)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM ingredientes WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        // Obtener todos los métodos de pago
        public List<MetodoPago> ObtenerMetodosPago()
        {
            var metodosPago = new List<MetodoPago>();

            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM metodo_pago";

                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        metodosPago.Add(new MetodoPago
                        {
                            Id = reader.GetInt32("id"),
                            Nombre = reader.GetString("nombre")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los métodos de pago", ex);
            }
            finally
            {
                cmd.Connection.Close();
            }

            return metodosPago;
        }

        // Obtener un método de pago por su ID
        public MetodoPago ObtenerMetodoPagoPorId(int id)
        {
            MetodoPago metodoPago = null;

            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM metodo_pago WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        metodoPago = new MetodoPago
                        {
                            Id = reader.GetInt32("id"),
                            Nombre = reader.GetString("nombre")
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el método de pago", ex);
            }
            finally
            {
                cmd.Connection.Close();
            }

            return metodoPago;
        }

        // Guardar un nuevo método de pago
        public int SaveMetodoPago(MetodoPago metodoPago)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO metodo_pago(nombre) VALUES(@nombre)";
                cmd.Parameters.Add(new MySqlParameter("@nombre", metodoPago.Nombre));

                cmd.Connection.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar el método de pago", ex);
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        // Actualizar un método de pago
        public int UpdateMetodoPago(int id, MetodoPago metodoPago)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE metodo_pago SET nombre = @nombre WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@nombre", metodoPago.Nombre));
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el método de pago", ex);
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        // Eliminar un método de pago
        public int DeleteMetodoPago(int id)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM metodo_pago WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el método de pago", ex);
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public int SaveUsuarioTipo(UsuarioTipoRequest usuarioTipoRequest)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO usuario_tipos(tipo) VALUES(@t)";
                cmd.Parameters.Add(new MySqlParameter("@t", usuarioTipoRequest.Tipo));

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

        public int UpdateUsuarioTipo(int id, UsuarioTipoRequest usuarioTipoRequest)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE usuario_tipos SET tipo = @t WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@t", usuarioTipoRequest.Tipo));
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

        public int DeleteUsuarioTipo(int id)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM usuario_tipos WHERE id = @id";
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

        public List<UsuarioTipo> ObtenerUsuarioTipos()
        {
            List<UsuarioTipo> usuarioTipos = new List<UsuarioTipo>();

            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT id, tipo FROM usuario_tipos";

                cmd.Connection.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    usuarioTipos.Add(new UsuarioTipo
                    {
                        Id = reader.GetInt32("id"),
                        Tipo = reader.GetString("tipo")
                    });
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

            return usuarioTipos;
        }

        public UsuarioTipo ObtenerUsuarioTipoPorId(int id)
        {
            UsuarioTipo usuarioTipo = null;

            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT id, tipo FROM usuario_tipos WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    usuarioTipo = new UsuarioTipo
                    {
                        Id = reader.GetInt32("id"),
                        Tipo = reader.GetString("tipo")
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

            return usuarioTipo;
        }

        public int SavePromocion(PromocionRequest promocionRequest)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO promociones(nombre, fecha_validez_inicio, fecha_validez_final, precio) VALUES(@n, @fvi, @fvf, @p)";
                cmd.Parameters.Add(new MySqlParameter("@n", promocionRequest.Nombre));
                cmd.Parameters.Add(new MySqlParameter("@fvi", promocionRequest.FechaValidezInicio));
                cmd.Parameters.Add(new MySqlParameter("@fvf", promocionRequest.FechaValidezFinal));
                cmd.Parameters.Add(new MySqlParameter("@p", promocionRequest.Precio));
                //cmd.Parameters.Add(new MySqlParameter("@f", promocionRequest.Foto));

                cmd.Connection.Open();
                int result = cmd.ExecuteNonQuery();

                int promocionId = 0;
                if (result > 0)
                {
                    cmd.CommandText = "SELECT LAST_INSERT_ID()";
                    promocionId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                if (promocionId > 0)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT INTO promociones_detalles(productoId, promocionId) VALUES(@productoId, @promocionId)";

                    List<ProductPromocion> products = promocionRequest.Productos;

                    foreach (var producto in products)
                    {
                        cmd.Parameters.Add(new MySqlParameter("@productoId", producto.productId));
                        cmd.Parameters.Add(new MySqlParameter("@promocionId", promocionId)); // Usamos el ID de la promoción

                        result = cmd.ExecuteNonQuery(); // Ejecuta la inserción para cada producto

                        // Limpiar los parámetros para la siguiente iteración
                        cmd.Parameters.Clear();
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public int UpdatePromocion(int id, PromocionRequest promocionRequest)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE promociones SET nombre = @n, fecha_validez_inicio = @fvi, fecha_validez_final = @fvf, precio = @p, foto = @f WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@n", promocionRequest.Nombre));
                cmd.Parameters.Add(new MySqlParameter("@fvi", promocionRequest.FechaValidezInicio));
                cmd.Parameters.Add(new MySqlParameter("@fvf", promocionRequest.FechaValidezFinal));
                cmd.Parameters.Add(new MySqlParameter("@p", promocionRequest.Precio));
                cmd.Parameters.Add(new MySqlParameter("@f", promocionRequest.Foto));
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

        public int DeletePromocion(int id)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM promociones WHERE id = @id";
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

        public List<Promocion> ObtenerPromociones()
        {
            List<Promocion> promociones = new List<Promocion>();

            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;

                // Consulta SQL para traer todas las promociones
                cmd.CommandText = @"
            SELECT id, nombre, fecha_validez_inicio, fecha_validez_final, precio, foto
            FROM promociones";

                cmd.Connection.Open();
                var reader = cmd.ExecuteReader();

                // Primero, obtenemos todas las promociones
                while (reader.Read())
                {
                    var promocion = new Promocion
                    {
                        Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32("id"),
                        Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? string.Empty : reader.GetString("nombre"),
                        FechaValidezInicio = reader.IsDBNull(reader.GetOrdinal("fecha_validez_inicio")) ? DateTime.MinValue : reader.GetDateTime("fecha_validez_inicio"),
                        FechaValidezFinal = reader.IsDBNull(reader.GetOrdinal("fecha_validez_final")) ? DateTime.MinValue : reader.GetDateTime("fecha_validez_final"),
                        Precio = reader.IsDBNull(reader.GetOrdinal("precio")) ? 0 : reader.GetDecimal("precio"),
                        Foto = reader.IsDBNull(reader.GetOrdinal("foto")) ? string.Empty : reader.GetString("foto"),
                        Detalles = new List<PromocionDetalle>() // Inicializamos la lista de detalles
                    };

                    promociones.Add(promocion);
                }

                reader.Close();  // Cerrar el reader de promociones

                // Ahora, para cada promoción, hacemos una consulta adicional para obtener sus detalles
                foreach (var promocion in promociones)
                {
                    // Consulta SQL para obtener los detalles de una promoción específica
                    cmd.CommandText = @"
                SELECT pd.id AS promocion_detalle_id, pd.productoId, pd.promocionId
                FROM promociones_detalles pd
                WHERE pd.promocionId = @PromocionId";

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@PromocionId", promocion.Id);

                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        promocion.Detalles.Add(new PromocionDetalle
                        {
                            Id = reader.IsDBNull(reader.GetOrdinal("promocion_detalle_id")) ? 0 : reader.GetInt32("promocion_detalle_id"),
                            ProductoId = reader.IsDBNull(reader.GetOrdinal("productoId")) ? 0 : reader.GetInt32("productoId"),
                            PromocionId = reader.IsDBNull(reader.GetOrdinal("promocionId")) ? 0 : reader.GetInt32("promocionId")
                        });
                    }

                    reader.Close(); // Cerrar el reader de detalles
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }

            return promociones;
        }



        public Promocion ObtenerPromocionPorId(int id)
        {
            Promocion promocion = null;

            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT id, nombre, fecha_validez_inicio, fecha_validez_final, precio, foto FROM promociones WHERE promocionId = @id";
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    promocion = new Promocion
                    {
                        Id = reader.GetInt32("id"),
                        Nombre = reader.GetString("nombre"),
                        FechaValidezInicio = reader.GetDateTime("fecha_validez_inicio"),
                        FechaValidezFinal = reader.GetDateTime("fecha_validez_final"),
                        Precio = reader.GetDecimal("precio"),
                        Foto = reader.GetString("foto")
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

            return promocion;
        }

        public int SavePromocionDetalle(PromocionDetalleRequest promocionDetalleRequest)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO promociones_detalles(producto_id, promocion_id) VALUES(@productoId, @promocionId)";
                cmd.Parameters.Add(new MySqlParameter("@productoId", promocionDetalleRequest.ProductoId));
                cmd.Parameters.Add(new MySqlParameter("@promocionId", promocionDetalleRequest.PromocionId));

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

        public int DeletePromocionDetalle(int id)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM promociones_detalles WHERE id = @id";
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

        public List<PromocionDetalle> ObtenerPromocionDetalles()
        {
            List<PromocionDetalle> promocionDetalles = new List<PromocionDetalle>();

            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT id, producto_id, promocion_id FROM promociones_detalles";

                cmd.Connection.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    promocionDetalles.Add(new PromocionDetalle
                    {
                        Id = reader.GetInt32("id"),
                        ProductoId = reader.GetInt32("producto_id"),
                        PromocionId = reader.GetInt32("promocion_id")
                    });
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

            return promocionDetalles;
        }

        public PromocionDetalle ObtenerPromocionDetallePorId(int id)
        {
            PromocionDetalle promocionDetalle = null;

            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT id, producto_id, promocion_id FROM promociones_detalles WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    promocionDetalle = new PromocionDetalle
                    {
                        Id = reader.GetInt32("id"),
                        ProductoId = reader.GetInt32("producto_id"),
                        PromocionId = reader.GetInt32("promocion_id")
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

            return promocionDetalle;
        }

        public int SavePlatilloIngrediente(PlatilloIngredienteRequest platilloIngredienteRequest)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO platillo_ingredientes(platillo_id, ingredienteId, cantidad_uso) VALUES(@platilloId, @ingredienteId, @cantidadUso)";
                cmd.Parameters.Add(new MySqlParameter("@platilloId", platilloIngredienteRequest.PlatilloId));
                cmd.Parameters.Add(new MySqlParameter("@ingredienteId", platilloIngredienteRequest.IngredienteId));
                cmd.Parameters.Add(new MySqlParameter("@cantidadUso", platilloIngredienteRequest.CantidadUso));

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

        public int DeletePlatilloIngrediente(int id)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM platillo_ingredientes WHERE platillo_id = @id";
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

        public List<PlatilloIngrediente> ObtenerPlatilloIngredientes()
        {
            List<PlatilloIngrediente> platilloIngredientes = new List<PlatilloIngrediente>();

            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT platillo_id, ingredienteId, cantidad_uso FROM platillo_ingredientes";

                cmd.Connection.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    platilloIngredientes.Add(new PlatilloIngrediente
                    {
                        PlatilloId = reader.GetInt32("platillo_id"),
                        IngredienteId = reader.GetInt32("ingredienteId"),
                        CantidadUso = reader.GetDecimal("cantidad_uso")
                    });
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

            return platilloIngredientes;
        }

        public PlatilloIngrediente ObtenerPlatilloIngredientePorId(int id)
        {
            PlatilloIngrediente platilloIngrediente = null;

            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT platillo_id, ingredienteId, cantidad_uso FROM platillo_ingredientes WHERE platillo_id = @id";
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    platilloIngrediente = new PlatilloIngrediente
                    {
                        PlatilloId = reader.GetInt32("platillo_id"),
                        IngredienteId = reader.GetInt32("ingredienteId"),
                        CantidadUso = reader.GetDecimal("cantidad_uso")
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

            return platilloIngrediente;
        }

        public int SaveProductoTipo(ProductoTipoRequest productoTipoRequest)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO producto_tipo(tipo) VALUES(@tipo)";
                cmd.Parameters.Add(new MySqlParameter("@tipo", productoTipoRequest.Tipo));

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

        public int DeleteProductoTipo(int id)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM producto_tipo WHERE id = @id";
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

        public List<ProductoTipo> ObtenerProductoTipos()
        {
            List<ProductoTipo> productoTipos = new List<ProductoTipo>();

            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT id, tipo FROM producto_tipo";

                cmd.Connection.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    productoTipos.Add(new ProductoTipo
                    {
                        Id = reader.GetInt32("id"),
                        Tipo = reader.GetString("tipo")
                    });
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

            return productoTipos;
        }

        public ProductoTipo ObtenerProductoTipoPorId(int id)
        {
            ProductoTipo productoTipo = null;

            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT id, tipo FROM producto_tipo WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    productoTipo = new ProductoTipo
                    {
                        Id = reader.GetInt32("id"),
                        Tipo = reader.GetString("tipo")
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

            return productoTipo;
        }

        public int SaveOrdenEstado(OrdenEstadoRequest ordenEstadoRequest)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO orden_estado(estado) VALUES(@estado)";
                cmd.Parameters.Add(new MySqlParameter("@estado", ordenEstadoRequest.Estado));

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

        public int DeleteOrdenEstado(int id)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM orden_estado WHERE id = @id";
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

        public List<OrdenEstado> ObtenerOrdenEstados()
        {
            List<OrdenEstado> ordenEstados = new List<OrdenEstado>();

            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT id, estado FROM orden_estado";

                cmd.Connection.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ordenEstados.Add(new OrdenEstado
                    {
                        Id = reader.GetInt32("id"),
                        Estado = reader.GetString("estado")
                    });
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

            return ordenEstados;
        }

        public OrdenEstado ObtenerOrdenEstadoPorId(int id)
        {
            OrdenEstado ordenEstado = null;

            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT id, estado FROM orden_estado WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    ordenEstado = new OrdenEstado
                    {
                        Id = reader.GetInt32("id"),
                        Estado = reader.GetString("estado")
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

            return ordenEstado;
        }

        public int UpdateEmpresa(int id, EmpresaRequest empresaRequest)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE empresa SET nombre = @nombre, direccion = @direccion WHERE id = @id";

                cmd.Parameters.Add(new MySqlParameter("@nombre", empresaRequest.Nombre));
                cmd.Parameters.Add(new MySqlParameter("@direccion", empresaRequest.Direccion));
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

        //CRUD ORDEN
        public int CrearOrden(OrdenRequest ordenRequest)
        {
            int ordenId = 0;

            try
            {
                cmd.Parameters.Clear();

                // Configuración para el procedimiento almacenado
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "CrearOrden";
                cmd.Parameters.Add(new MySqlParameter("p_Fecha", ordenRequest.Fecha));
                cmd.Parameters.Add(new MySqlParameter("p_ObservacionCliente", ordenRequest.ObservacionCliente));
                cmd.Parameters.Add(new MySqlParameter("p_MetodoPagoId", ordenRequest.MetodoPagoId));
                cmd.Parameters.Add(new MySqlParameter("p_OrdenEstadoId", ordenRequest.OrdenEstadoId));
                cmd.Parameters.Add(new MySqlParameter("p_UsuarioId", ordenRequest.UsuarioId));

                cmd.Connection.Open();

                // Ejecuta la creación de la orden
                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    // Obtiene el ID de la orden recién creada
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT LAST_INSERT_ID()";
                    ordenId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Inserta los detalles si la orden fue creada
                if (ordenId > 0 && ordenRequest.DetalleOrden != null)
                {
                    foreach (var producto in ordenRequest.DetalleOrden)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "INSERT INTO orden_detalles (productoId, cantidad, precio, ordenId) " +
                                          "VALUES (@productoId, @cantidad, @precio, @ordenId)";
                        cmd.Parameters.Add(new MySqlParameter("@productoId", producto.ProductoId));
                        cmd.Parameters.Add(new MySqlParameter("@cantidad", producto.Cantidad));
                        cmd.Parameters.Add(new MySqlParameter("@precio", producto.Precio));
                        cmd.Parameters.Add(new MySqlParameter("@ordenId", ordenId));

                        cmd.ExecuteNonQuery();
                    }
                }

                return ordenId; // Devuelve el ID de la orden creada
            }
            catch (Exception ex)
            {
                // Maneja el error adecuadamente (puedes agregar logs)
                throw new Exception("Error al crear la orden", ex);
            }
            finally
            {
                cmd.Connection.Close();
            }
        }



        public List<Orden> ObtenerOrdenes()

        {
            List<Orden> orden = new List<Orden>();
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "LeerOrden";

                cmd.Connection.Open();
                ds = new DataSet();

                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(ds);

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    orden.Add(new Orden()
                    {
                        Id = reader.GetInt32("id"),
                        Fecha = reader.GetDateTime("Fecha"),
                        ObservacionCliente = reader.GetString("observacion_cliente"),
                        MetodoPagoId = reader.GetInt32("metodoPagoId"),
                        OrdenEstadoId = reader.GetInt32("ordenEstadoId"),
                        UsuarioId = reader.GetInt32("usuarioId")
                    });
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

            return orden;
        }
        //Por ID    
        public Orden ObtenerOrdenexId(int id)

        {
            Orden orden = new Orden();
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "LeerOrdenPorId";
                cmd.Parameters.Add(new MySqlParameter("p_Id", id));

                cmd.Connection.Open();
                ds = new DataSet();

                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(ds);

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    orden = new Orden()
                    {
                        Id = reader.GetInt32("id"),
                        Fecha = reader.GetDateTime("Fecha"),
                        ObservacionCliente = reader.GetString("observacion_cliente"),
                        MetodoPagoId = reader.GetInt32("metodoPagoId"),
                        OrdenEstadoId = reader.GetInt32("ordenEstadoId"),
                        UsuarioId = reader.GetInt32("usuarioId")
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

            return orden;
        }

        public int UpdateOrden(int idOrden, OrdenRequest ordenRequest)
        {
            try
            {

                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "ActualizarOrden";
                cmd.Parameters.Add(new MySqlParameter("p_Fecha", ordenRequest.Fecha));
                cmd.Parameters.Add(new MySqlParameter("p_ObservacionCliente", ordenRequest.ObservacionCliente));
                cmd.Parameters.Add(new MySqlParameter("p_MetodoPagoId", ordenRequest.MetodoPagoId));
                cmd.Parameters.Add(new MySqlParameter("p_OrdenEstadoId", ordenRequest.OrdenEstadoId));
                cmd.Parameters.Add(new MySqlParameter("p_UsuarioId", ordenRequest.UsuarioId));
                cmd.Parameters.Add(new MySqlParameter("p_Id", idOrden));

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

        public int DeleteOrden(int id)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "EliminarOrden";
                cmd.Parameters.Add(new MySqlParameter("p_Id", id));

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

        //CRUD ORDENDETALLE
        public int CrearOrdenDetalle(OrdenDetalleRequest ordenDetalle)
        {
            try
            {

                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "CrearOrdenDetalle";
                cmd.Parameters.Add(new MySqlParameter("p_ProductoId", ordenDetalle.ProductoId));
                cmd.Parameters.Add(new MySqlParameter("p_Cantidad", ordenDetalle.Cantidad));
                cmd.Parameters.Add(new MySqlParameter("p_Precio", ordenDetalle.Precio));
                cmd.Parameters.Add(new MySqlParameter("p_Descuento", ordenDetalle.Descuento));
                cmd.Parameters.Add(new MySqlParameter("p_OrdenId", ordenDetalle.OrdenId));

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

        public List<OrdenDetalle> ObtenerOrdenDetalle(int idOrden)

        {
            List<OrdenDetalle> ordenDetalles = new List<OrdenDetalle>();
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "LeerOrdenDetalle";
                cmd.Parameters.Add(new MySqlParameter("p_OrdenId", idOrden));

                cmd.Connection.Open();
                ds = new DataSet();

                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(ds);

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    ordenDetalles.Add(new OrdenDetalle()
                    {
                        Id = reader.GetInt32("id"),
                        ProductoId = reader.GetInt32("productoId"),
                        Cantidad = reader.GetInt32("cantidad"),
                        Precio = reader.GetDecimal("precio"),
                        Descuento = reader.GetInt32("descuento"),
                        OrdenId = reader.GetInt32("ordenId"),
                    });
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

            return ordenDetalles;
        }

        public int UpdateOrdenDetalle(int idOrdenDetalle, OrdenDetalleRequest ordenDetalleRequest)
        {
            try
            {

                cmd.Parameters.Clear();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "ActualizarOrdenDetalle";
                cmd.Parameters.Add(new MySqlParameter("p_ProductoId", ordenDetalleRequest.ProductoId));
                cmd.Parameters.Add(new MySqlParameter("p_Cantidad", ordenDetalleRequest.Cantidad));
                cmd.Parameters.Add(new MySqlParameter("p_Precio", ordenDetalleRequest.Precio));
                cmd.Parameters.Add(new MySqlParameter("p_Descuento", ordenDetalleRequest.Descuento));
                cmd.Parameters.Add(new MySqlParameter("p_OrdenId", ordenDetalleRequest.OrdenId));
                cmd.Parameters.Add(new MySqlParameter("p_Id", idOrdenDetalle));

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

        public int DeleteOrdenDetalle(int id)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "EliminarOrdenDetalle";
                cmd.Parameters.Add(new MySqlParameter("p_Id", id));
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

        public Orden ObtenerOrdenYDetallePorId(int id)
        {

            Orden orden = new Orden();
            List<OrdenDetalle> ordenDetalles = new List<OrdenDetalle>();
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "ObtenerOrdenxId";
                cmd.Parameters.Add(new MySqlParameter("p_Id", id));
                cmd.Connection.Open();

                // Obtener Orden

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        orden = new Orden()
                        {
                            Id = (int)reader["Id"],
                            Fecha = (DateTime)reader["Fecha"],
                            ObservacionCliente = (string)reader["observacion_cliente"],
                            MetodoPagoId = (int)reader["Cliente"],
                            OrdenEstadoId = (int)reader["Orden"],
                            UsuarioId = (int)reader["Orden"]

                        };
                    }
                }

                // Obtener Detalles
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "LeerOrdenDetalle";
                cmd.Parameters.Add(new MySqlParameter("p_OrdenId", id));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ordenDetalles.Add(new OrdenDetalle
                        {
                            Id = (int)reader["Id"],
                            ProductoId = (int)reader["ProductoId"],
                            Cantidad = (int)reader["Cantidad"],
                            Precio = (decimal)reader["Precio"],
                            Descuento = (decimal)reader["Descuento"],
                            OrdenId = (int)reader["OrdenId"]
                        });
                    }
                }
            }
            catch (Exception ex) { }
            finally
            {

            }
            return orden;
        }

        public List<TopProductosVendidos> GetTopProductosMasVendidos(DateTime fechaInicio, DateTime fechaFin)
        {
            var result = new List<TopProductosVendidos>();

            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "TopProductosVendidosPorFechas"; // Nombre del procedimiento almacenado

                // Agregar parámetros
                cmd.Parameters.Add(new MySqlParameter("fechaInicio", fechaInicio));
                cmd.Parameters.Add(new MySqlParameter("fechaFin", fechaFin));

                // Abrir conexión
                cmd.Connection.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new TopProductosVendidos
                        {
                            Producto = reader["Producto"].ToString(),
                            TotalVendido = Convert.ToInt32(reader["TotalVendido"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar el procedimiento almacenado", ex);
            }
            finally
            {
                cmd.Connection.Close(); // Asegurar el cierre de la conexión
            }

            return result; // Devolver los datos
        }

        public List<CantXMetodoPago> GetCantidadVendidaPorMetodoPago()
        {
            var result = new List<CantXMetodoPago>();

            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "TotalesFacturadosPorProducto";
                cmd.Connection.Open();



                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new CantXMetodoPago
                        {
                            MetodoPago = reader["MetodoPago"].ToString(),
                            CantidadOrdenes = Convert.ToInt32(reader["CantidadOrdenes"])
                        });
                    }
                }


            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                throw new Exception("Error al obtener los datos desde la base de datos", ex);
            }

            return result;
        }

        public List<TotalFacturadoXProducto> GetTotalesFacturadosPorProducto()
        {
            var result = new List<TotalFacturadoXProducto>();

            try
            {
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "TotalesFacturadosPorProducto";
                    cmd.Connection.Open();
                    
                        

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new TotalFacturadoXProducto
                                {
                                    Producto = reader["Producto"].ToString(),
                                    TotalFacturado = Convert.ToDecimal(reader["TotalFacturado"])
                                });
                            }
                        }
                    
                
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                throw new Exception("Error al obtener los datos desde la base de datos", ex);
            }

            return result;
        }

        public List<Sucursal> ObtenerSucursales()
        {
            List<Sucursal> sucursales = new List<Sucursal>();
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT id, nombre, direccion FROM sucursales";

                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sucursales.Add(new Sucursal
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Nombre = reader["nombre"].ToString(),
                            Direccion = reader["direccion"].ToString()
                        });
                    }
                }
            }
            finally
            {
                cmd.Connection.Close();
            }
            return sucursales;
        }

        public Sucursal ObtenerSucursalPorId(int id)
        {
            Sucursal sucursal = null;
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT id, nombre, direccion FROM sucursales WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        sucursal = new Sucursal
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Nombre = reader["nombre"].ToString(),
                            Direccion = reader["direccion"].ToString()
                        };
                    }
                }
            }
            finally
            {
                cmd.Connection.Close();
            }
            return sucursal;
        }

        public int GuardarSucursal(SucursalRequest sucursalRequest)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO sucursales (nombre, direccion) VALUES (@nombre, @direccion)";
                cmd.Parameters.Add(new MySqlParameter("@nombre", sucursalRequest.Nombre));
                cmd.Parameters.Add(new MySqlParameter("@direccion", sucursalRequest.Direccion));

                cmd.Connection.Open();
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public int ActualizarSucursal(int id, SucursalRequest sucursalRequest)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE sucursales SET nombre = @nombre, direccion = @direccion WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@nombre", sucursalRequest.Nombre));
                cmd.Parameters.Add(new MySqlParameter("@direccion", sucursalRequest.Direccion));
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public int EliminarSucursal(int id)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM sucursales WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        // Obtener todas las reservas
        public List<Reserva> ObtenerReservas()
        {
            var reservas = new List<Reserva>();
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT Id, Nombre, Telefono, FechaHoraReserva FROM reservas";

                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reservas.Add(new Reserva
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString(),
                            Telefono = reader["Telefono"].ToString(),
                            FechaHoraReserva = Convert.ToDateTime(reader["FechaHoraReserva"])
                        });
                    }
                }
            }
            finally
            {
                cmd.Connection.Close();
            }
            return reservas;
        }

        // Obtener una reserva por ID
        public Reserva ObtenerReservaPorId(int id)
        {
            Reserva reserva = null;
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT Id, Nombre, Telefono, FechaHoraReserva FROM reservas WHERE Id = @id";
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        reserva = new Reserva
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString(),
                            Telefono = reader["Telefono"].ToString(),
                            FechaHoraReserva = Convert.ToDateTime(reader["FechaHoraReserva"])
                        };
                    }
                }
            }
            finally
            {
                cmd.Connection.Close();
            }
            return reserva;
        }

        // Crear una nueva reserva
        public int CrearReserva(ReservaRequest reserva)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO reservas (Nombre, Telefono, FechaHoraReserva) VALUES (@Nombre, @Telefono, @FechaHoraReserva)";
                cmd.Parameters.Add(new MySqlParameter("@Nombre", reserva.Nombre));
                cmd.Parameters.Add(new MySqlParameter("@Telefono", reserva.Telefono));
                cmd.Parameters.Add(new MySqlParameter("@FechaHoraReserva", reserva.FechaHoraReserva));

                cmd.Connection.Open();
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        // Actualizar una reserva existente
        public int ActualizarReserva(int id, ReservaRequest reserva)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE reservas SET Nombre = @Nombre, Telefono = @Telefono, FechaHoraReserva = @FechaHoraReserva WHERE Id = @Id";
                cmd.Parameters.Add(new MySqlParameter("@Nombre", reserva.Nombre));
                cmd.Parameters.Add(new MySqlParameter("@Telefono", reserva.Telefono));
                cmd.Parameters.Add(new MySqlParameter("@FechaHoraReserva", reserva.FechaHoraReserva));
                cmd.Parameters.Add(new MySqlParameter("@Id", id));

                cmd.Connection.Open();
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        // Eliminar una reserva
        public int EliminarReserva(int id)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM reservas WHERE Id = @Id";
                cmd.Parameters.Add(new MySqlParameter("@Id", id));

                cmd.Connection.Open();
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public List<Pedido> ObtenerPedidos()
        {
            List<Pedido> pedidos = new List<Pedido>();
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT id, cliente, producto, producto_img, quantity, value FROM pedidos";

                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pedidos.Add(new Pedido
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Cliente = reader["cliente"].ToString(),
                            Producto = reader["producto"].ToString(),
                            ProductoImg = reader["producto_img"].ToString(),
                            Value = Convert.ToDouble(reader["value"]),
                            Quantity = Convert.ToInt32(reader["quantity"])
                        });
                    }
                }
            }
            finally
            {
                cmd.Connection.Close();
            }
            return pedidos;
        }

        public Pedido ObtenerPedidoPorId(int id)
        {
            Pedido pedido = null;
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT id, cliente, producto, producto_img FROM pedidos WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        pedido = new Pedido
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Cliente = reader["cliente"].ToString(),
                            Producto = reader["producto"].ToString(),
                            ProductoImg = reader["producto_img"].ToString(),
                            Value = Convert.ToDouble(reader["value"]),
                            Quantity = Convert.ToInt32(reader["quantity"])
                        };
                    }
                }
            }
            finally
            {
                cmd.Connection.Close();
            }
            return pedido;
        }

        public int GuardarPedido(PedidoRequest pedidoRequest)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO pedidos (cliente, producto, value, quantity) VALUES (@cliente, @producto, @value, @quantity)";
                cmd.Parameters.Add(new MySqlParameter("@cliente", pedidoRequest.Cliente));
                cmd.Parameters.Add(new MySqlParameter("@producto", pedidoRequest.Producto));
                // cmd.Parameters.Add(new MySqlParameter("@producto_img", pedidoRequest.ProductoImg));
                cmd.Parameters.Add(new MySqlParameter("@value", pedidoRequest.Value));
                cmd.Parameters.Add(new MySqlParameter("@quantity", pedidoRequest.Quantity));

                cmd.Connection.Open();
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public int ActualizarPedido(int id, PedidoRequest pedidoRequest)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE pedidos SET cliente = @cliente, producto = @producto, producto_img = @producto_img WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@cliente", pedidoRequest.Cliente));
                cmd.Parameters.Add(new MySqlParameter("@producto", pedidoRequest.Producto));
                cmd.Parameters.Add(new MySqlParameter("@producto_img", pedidoRequest.ProductoImg));
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public int EliminarPedido(int id)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM pedidos WHERE id = @id";
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                cmd.Connection.Open();
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        //Reportes
        public List<Orden> ObtenerOrdenesReporte()
        {
            List<Orden> ordenes = new List<Orden>();
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM orden";

                cmd.Connection.Open();
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Orden orden = new Orden
                    {
                        Id = reader.GetInt32("id"),
                        Fecha = reader.GetDateTime("fecha"),
                        ObservacionCliente = reader.IsDBNull(reader.GetOrdinal("observacion_cliente")) ? null : reader.GetString("observacion_cliente"),
                        MetodoPagoId = reader.GetInt32("metodoPagoId"),
                        OrdenEstadoId = reader.GetInt32("ordenEstadoId"),
                        UsuarioId = reader.GetInt32("usuarioId")
                    };
                    ordenes.Add(orden);
                }
                return ordenes;
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


    }
}
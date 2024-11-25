using MySql.Data.MySqlClient;
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
                cmd.CommandText = "INSERT INTO promociones(nombre, fecha_validez_inicio, fecha_validez_final, precio, foto) VALUES(@n, @fvi, @fvf, @p, @f)";
                cmd.Parameters.Add(new MySqlParameter("@n", promocionRequest.Nombre));
                cmd.Parameters.Add(new MySqlParameter("@fvi", promocionRequest.FechaValidezInicio));
                cmd.Parameters.Add(new MySqlParameter("@fvf", promocionRequest.FechaValidezFinal));
                cmd.Parameters.Add(new MySqlParameter("@p", promocionRequest.Precio));
                cmd.Parameters.Add(new MySqlParameter("@f", promocionRequest.Foto));

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
                cmd.CommandText = "SELECT id, nombre, fecha_validez_inicio, fecha_validez_final, precio, foto FROM promociones";

                cmd.Connection.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    promociones.Add(new Promocion
                    {
                        Id = reader.GetInt32("id"),
                        Nombre = reader.GetString("nombre"),
                        FechaValidezInicio = reader.GetDateTime("fecha_validez_inicio"),
                        FechaValidezFinal = reader.GetDateTime("fecha_validez_final"),
                        Precio = reader.GetDecimal("precio"),
                        Foto = reader.GetString("foto")
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

            return promociones;
        }

        public Promocion ObtenerPromocionPorId(int id)
        {
            Promocion promocion = null;

            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT id, nombre, fecha_validez_inicio, fecha_validez_final, precio, foto FROM promociones WHERE id = @id";
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
            try
            {

                    cmd.Parameters.Clear();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "CrearOrden";
                    cmd.Parameters.Add(new MySqlParameter("p_Fecha", ordenRequest.Fecha));
                    cmd.Parameters.Add(new MySqlParameter("p_ObservacionCliente", ordenRequest.ObservacionCliente));
                    cmd.Parameters.Add(new MySqlParameter("p_MetodoPagoId", ordenRequest.MetodoPagoId));
                    cmd.Parameters.Add(new MySqlParameter("p_OrdenEstadoId", ordenRequest.OrdenEstadoId));
                    cmd.Parameters.Add(new MySqlParameter("p_UsuarioId", ordenRequest.UsuarioId));

                    cmd.Connection.Open();
                    return cmd.ExecuteNonQuery();
                
            }catch (Exception ex) 
            { 
                throw; 
            }
            finally {
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
            OrdenDetalle ordenDetalles = new OrdenDetalle();
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
                cmd.CommandText = "ObtenerOrdenDetalle";
                cmd.Parameters.Add(new MySqlParameter("p_OrdenId", id));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ordenDetalles = new OrdenDetalle
                        {
                            Id = (int)reader["Id"],
                            ProductoId = (int)reader["ProductoId"],
                            Cantidad = (int)reader["Cantidad"],
                            Precio = (decimal)reader["Precio"],
                            Descuento = (decimal)reader["Descuento"],
                            OrdenId = (int)reader["OrdenId"]
                        };
                    }
                }
            }
            catch (Exception ex) { }
            finally { 
                    
            }
            return orden;
        }


    }
}
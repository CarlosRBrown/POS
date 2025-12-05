using Data;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient; // Contiene las clases de conexión a SQL Server
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace LoginC
{
    public partial class Products : Form
    {
        private string imagePath = null; // Variable global para almacenar la ruta

        private string connectionString;

        public int ID;
        public string name;

        public Products(int userId, string username)
        {
            InitializeComponent();

            this.Load += Products_Load;
            dataGridView1.CellFormatting += dataGridView1_CellFormatting_1;
            dataGridView1.DataError += dataGridView1_DataError;
            // 2. Captura el evento KeyPress en el TextBox.
            txtCodigo.KeyPress += txtCodigo_KeyPress;
            ID = userId; name=username;

        }

        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 1. Detección de la tecla ENTER enviada por el escáner.
            if (e.KeyChar == (char)Keys.Return)
            {
                // 2. Evita que el sistema haga un sonido de error.
                e.Handled = true;

                // 3. Obtiene el código escaneado para procesarlo.
                string codigoEscaneado = txtCodigo.Text;

                // 4. Lógica de Procesamiento del Código (ejemplo: mostrar un mensaje)
                MessageBox.Show($"Código procesado: {codigoEscaneado}", "Escaneo Completado");

                // 5. ¡ACCIÓN CLAVE! Borra el contenido del TextBox.
                txtCodigo.Clear();

                // 6. Mantiene el foco en el TextBox, preparándolo para el siguiente escaneo.
                txtCodigo.Focus();
            }
        }


        private void Products_Load(object sender, EventArgs e)
        {
            // Asegura que el campo de texto esté listo para recibir la entrada del escáner al cargar el formulario.
            txtCodigo.Focus();

            // 1. Instanciar la clase Connection de la librería Data
            Connection conexionDB = new Connection();

            // 2. Llamar al método público para obtener la cadena de conexión
            connectionString = conexionDB.cadena_conexion();

            CargarDatosEnGrid();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            // Consulta SQL con placeholders para los parámetros (@Nombre, @Edad)
            string query = "INSERT INTO Products (Code_Product,Description,Observation,Zone,Price,Quantity,Image,Status,Created_By,Created_At,Updated_By,Updated_At) VALUES(@Code_Product,@Description,@Observation,@Zone,@Price,@Quantity,@Image,@Status,@Created_By,@Created_At,@Updated_By,@Updated_At)";

            if (imagePath == null)
            {
                MessageBox.Show("Por favor, selecciona una imagen primero.");
                return;
            }

            // --- Variables de Configuración ---
            string nombreProducto = txtCodigo.Text; // Obtener de un TextBox
            string carpetaDestino = @"C:\"; // Asegúrate de que esta carpeta exista

            // 1. Generar un nombre de archivo único para evitar colisiones
            string fileName = Path.GetFileName(imagePath);
            string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(imagePath);

            // 2. Definir la ruta completa donde se guardará el archivo
            string fullPathDestino = Path.Combine(carpetaDestino, newFileName);

            // 3. Ruta que se guardará en la base de datos (puede ser relativa o absoluta)
            string rutaADb = newFileName; // Guardamos solo el nombre del archivo si la carpeta es conocida
            // Uso de la sentencia 'using' para asegurar que los objetos de conexión se cierren y se liberen
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    // 1. Agregar Parámetros (¡Esto es vital para la seguridad!)
                    // Mapea las variables de C# a los placeholders SQL
                    command.Parameters.AddWithValue("@Code_Product", txtCode.Text);
                    command.Parameters.AddWithValue("@Description", txtDescription.Text);
                    command.Parameters.AddWithValue("@Observation", txtObservations.Text);
                    command.Parameters.AddWithValue("@Zone", txtZone.Text);
                    command.Parameters.AddWithValue("@Price", txtPrice.Text);
                    command.Parameters.AddWithValue("@Quantity", txtQuantity.Text);
                    command.Parameters.AddWithValue("@Image", fileName);
                    command.Parameters.AddWithValue("@Status", 1);
                    command.Parameters.AddWithValue("@Created_By", 1);
                    command.Parameters.AddWithValue("@Created_At", DateTime.Now);
                    command.Parameters.AddWithValue("@Updated_By", 1);
                    command.Parameters.AddWithValue("@Updated_At", DateTime.Now);

                    try
                    {
                        // 2. Abrir la conexión
                        connection.Open();

                        // 3. Ejecutar la consulta (ExecuteNonQuery se usa para INSERT, UPDATE o DELETE)
                        int rowsAffected = command.ExecuteNonQuery();
                        // Mostrar un mensaje de éxito
                        MessageBox.Show($"Datos guardados correctamente. Filas afectadas: {rowsAffected}");
                    }
                    catch (Exception ex)
                    {
                        // Manejo de errores de conexión o de consulta
                        MessageBox.Show("Error al guardar los datos: " + ex.Message);
                    }
                } // El SqlCommand se destruye aquí
            } // El SqlConnection se cierra y se destruye aquí
        }


        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // Filtros para asegurar que solo se seleccionen archivos de imagen
            openFileDialog.Filter = "Archivos de Imagen|*.jpg;*.jpeg;*.png;*.gif|Todos los archivos|*.*";
            openFileDialog.Title = "Seleccionar Imagen";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Almacena la ruta del archivo seleccionado
                imagePath = openFileDialog.FileName;
                //Opcional: Mostrar la imagen en un PictureBox (supuesto: pbImagen)
                pictureBox1.Image = Image.FromFile(imagePath);

            }

        }

        private void button3_Click(object sender, EventArgs e)
        {

            // la carpeta base donde están guardadas todas tus imágenes
            string carpetabaseimagenes = @"C:\";
            string rutadesdedb = null;

            string query = "SELECT Code_Product, Image, Description, Observation, Zone FROM Products";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // parámetro para buscar el producto específico
                    command.Parameters.AddWithValue("@productoid", txtCodigo.Text);
                    connection.Open();

                    // ejecuta la consulta y lee el resultado
                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        rutadesdedb = result.ToString();
                    }
                }

                // 2. cargar la imagen si se encontró la ruta
                if (!string.IsNullOrEmpty(rutadesdedb))
                {
                    // combina la carpeta base con el nombre del archivo guardado en la bd
                    string rutacompleta = Path.Combine(carpetabaseimagenes, rutadesdedb);

                    if (File.Exists(rutacompleta))
                    {
                        // carga la imagen desde la ruta del disco y la asigna al picturebox
                        // (asumiendo que tu picturebox se llama 'pbimagen')
                        pictureBox2.Image = Image.FromFile(rutacompleta);
                    }
                    else
                    {
                        MessageBox.Show($"error: archivo de imagen no encontrado en {rutacompleta}");
                        pictureBox2.Image = null;
                    }
                }
                else
                {
                    MessageBox.Show("no se encontró la ruta de la imagen para el id proporcionado.");
                    pictureBox2.Image = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("error al consultar o cargar la imagen: " + ex.Message);
            }
        }

        private void txtCodigo_TextChanged(object sender, EventArgs e)
        {

        }

        public DataTable ObtenerProductosConRutas()
        {
            DataTable dt = new DataTable();
            string query = "SELECT Id_Product, Code_Product, Description, Observation, Zone, Quantity, Image FROM Products";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dt);
                    }
                    catch (Exception ex)
                    {
                        // Manejar errores de conexión o consulta
                        MessageBox.Show("Error al conectar o consultar la base de datos: " + ex.Message);
                    }
                }
            }
            return dt;
        }

        private void CargarDatosEnGrid()
        {
            // 1. Obtener la tabla de productos
            DataTable productos = ObtenerProductosConRutas();
            dataGridView1.Columns.Clear();
            // 2. Configurar el DataGridView
            dataGridView1.AutoGenerateColumns = false;

            // --- AÑADIR ESTE CÓDIGO PARA AUMENTAR EL TAMAÑO DE LAS FILAS ---
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None; // Desactivar autoajuste
            dataGridView1.RowTemplate.Height = 200; // <-- Define la altura de cada fila (Ejemplo: 200 píxeles)


            dataGridView1.Columns.Add("Id_Product", "ID");
            dataGridView1.Columns.Add("Code", "Código");
            dataGridView1.Columns.Add("Description", "Descripcción");
            dataGridView1.Columns.Add("Observation", "Observación");
            dataGridView1.Columns.Add("Zone", "Zona");
            dataGridView1.Columns.Add("Quantity", "Cantidad");


            dataGridView1.Columns["Id_Product"].DataPropertyName = "Id_Product";
            dataGridView1.Columns["Code"].DataPropertyName = "Code_Product";
            dataGridView1.Columns["Description"].DataPropertyName = "Description";
            dataGridView1.Columns["Observation"].DataPropertyName = "Observation";
            dataGridView1.Columns["Zone"].DataPropertyName = "Zone";
            dataGridView1.Columns["Quantity"].DataPropertyName = "Quantity";

            ////Renombrar nombre de las columnas
            //dataGridView1.Columns[0].HeaderText = "ID";
            //dataGridView1.Columns[1].HeaderText = "ID";

            // Agregar la columna de imagen
            DataGridViewImageColumn imgColumna = new DataGridViewImageColumn();
            imgColumna.Name = "ProductImage";
            imgColumna.HeaderText = "Imagen";
            imgColumna.ImageLayout = DataGridViewImageCellLayout.Zoom; // Para que se ajuste
            dataGridView1.Columns.Add(imgColumna);
            dataGridView1.Columns["ProductImage"].Width = 250; // <-- Define el ancho de la columna de imagen

            // 3. Llenar el DataGridView y procesar las imágenes
            dataGridView1.DataSource = productos;

            // Recorrer las filas del DataTable y asignar la imagen procesada
            foreach (DataRow fila in productos.Rows)
            {
                Image img = ObtenerImagenParaFila(fila);

                // Buscar la fila correspondiente en el DataGridView
                // Asumiendo que el índice es el mismo después de asignar el DataSource
                int rowIndex = productos.Rows.IndexOf(fila);

                if (rowIndex >= 0 && rowIndex < dataGridView1.Rows.Count)
                {
                    // Asignar la imagen a la celda de la columna "ProductImage"
                    dataGridView1.Rows[rowIndex].Cells["ProductImage"].Value = img;
                }
            }
            // --- LÍNEA CLAVE PARA QUITAR LA FILA VACÍA ---
            dataGridView1.AllowUserToAddRows = false; // <-- Esto quita la última fila vacía
            dataGridView1.AutoGenerateColumns = false;
        }

        // Esta función procesa una fila y devuelve la imagen lista para mostrar
        public Image ObtenerImagenParaFila(DataRow fila)
        {
           string RutaBaseImagenes = @"C:\"; // Tu ruta base
           string nombreImagen = fila["Image"]?.ToString();

            // 1. Verifica si el campo 'Image' está vacío o nulo
            if (string.IsNullOrWhiteSpace(nombreImagen))
            {
                // Muestra "No imagen" si el campo está vacío
                return CrearImagenDeTexto("NO IMAGEN");
            }

            // 2. Construye la ruta completa del archivo
            string rutaCompleta = Path.Combine(RutaBaseImagenes, nombreImagen);

            // 3. Verifica si el archivo realmente existe en el disco
            if (File.Exists(rutaCompleta))
            {
                try
                {
                    // Carga la imagen de forma segura (copiándola a un MemoryStream)
                    // Esto es crucial para liberar el archivo original inmediatamente.
                    using (var stream = new MemoryStream(File.ReadAllBytes(rutaCompleta)))
                    {
                        return Image.FromStream(stream);
                    }
                }
                catch (Exception)
                {
                    // Muestra "Error al cargar" si el archivo existe pero no es válido (corrupto, formato incorrecto, etc.)
                    return CrearImagenDeTexto("ERROR AL CARGAR");
                }
            }
            else
            {
                // Muestra "No imagen" si el archivo no se encontró en la ruta especificada
                return CrearImagenDeTexto("NO IMAGEN");
            }
        }

        private Image CrearImagenDeTexto(string texto)
        {
            // Define un tamaño más grande para la imagen de sustitución (Ejemplo: 250x200 píxeles)
            Bitmap bmp = new Bitmap(250, 200); // <-- ¡CAMBIA ESTOS VALORES!
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightGray);
                Font font = new Font("Arial", 14, FontStyle.Bold); // También puedes hacer la fuente más grande
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                Rectangle rect = new Rectangle(0, 0, 250, 200); // Coincidir con el tamaño del Bitmap
                g.DrawString(texto, font, Brushes.Black, rect, sf);
            }
            return bmp;
        }


        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Revisamos que sea un error de formato en la columna 'Image'
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Image")
            {
                // Silenciamos la excepción, ya que el CellFormatting se encargará de mostrarla correctamente
                e.ThrowException = false;
            }
            e.ThrowException = true;
        
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.BackColor = Color.FromArgb(0, 30, 80);
            button2.ForeColor = Color.White;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.BackColor = Color.White;
            button2.ForeColor = Color.Black;

        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(0, 30, 80);
            button1.ForeColor = Color.White;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = Color.White;
            button1.ForeColor = Color.Black;
        }

        private void dataGridView1_CellFormatting_1(object sender, DataGridViewCellFormattingEventArgs e)
        {
            const string COLUMNA_VISUAL_FOTO = "ColumnaFoto";
            const string COLUMNA_DATOS_RUTA = "Image";

            // 1. Verificar si estamos en la columna de imagen y no en el encabezado
            if (dataGridView1.Columns[e.ColumnIndex].Name == COLUMNA_VISUAL_FOTO && e.RowIndex >= 0)
            {
                // 2. Obtener el valor de la celda de ruta (la columna oculta "Image" de SQL)
                object valorCeldaRuta = dataGridView1.Rows[e.RowIndex].Cells[COLUMNA_DATOS_RUTA].Value;

                // 3. Manejar campos NULOS o VACÍOS
                if (valorCeldaRuta != null && valorCeldaRuta != DBNull.Value && !string.IsNullOrWhiteSpace(valorCeldaRuta.ToString()))
                {
                    string nombreArchivo = valorCeldaRuta.ToString().Trim();
                    // 4. Crear la ruta completa combinando la base y el nombre del archivo
                    string rutaCompleta = Path.Combine(@"C:\", nombreArchivo);

                    // 5. Intentar cargar la imagen si el archivo existe
                    if (File.Exists(rutaCompleta))
                    {
                        try
                        {
                            // Usar Image.FromFile y luego crear un Bitmap para liberar el archivo inmediatamente
                            using (Image imgFromFile = Image.FromFile(rutaCompleta))
                            {
                                e.Value = new Bitmap(imgFromFile); // Asignar la imagen a la celda
                            }
                            e.FormattingApplied = true;
                        }
                        catch (Exception)
                        {
                            // En caso de error de lectura
                            e.Value = null;
                            e.FormattingApplied = true;
                        }
                    }
                    else
                    {
                        // El archivo no se encontró en C:\
                        e.Value = null;
                        e.FormattingApplied = true;
                    }
                }
                else
                {
                    // El campo de la base de datos está vacío (NULL o "")
                    e.Value = null;
                    e.FormattingApplied = true;
                }
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Dashboard F3 = new Dashboard(ID, name);
            F3.Show();
            this.Hide();
            // Mostrar el siguiente formulario
        }
    }
}

using Prueba.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prueba.Forms
{
    public partial class ProductoManager : Form
    {
        public static ProductoManager productoManager;
        private TextBox[] textBoxes;
        public string productoSeleccionado;
        public DateTime datetime;
        public ProductoManager()
        {
            InitializeComponent();
            textBoxes = new TextBox[] { textBox1, textBox2, textBox3};
            productoManager = this;
            productoSeleccionado = string.Empty;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                int.TryParse(textBox1.Text, out int _stock);
                float.TryParse(textBox3.Text, out float _precio);
                if (_stock <= 0) throw new Exception("La unidad de stock no puede ser menor o igual a 0");
                if (_precio <= 0) throw new Exception("El precio no puede ser menor o igual a 0");
                Producto producto = new Producto
                {
                    stock = _stock,
                    nombre = textBox2.Text,
                    precio = _precio
                };
                Form1.form1.database.Insertar(producto);
            }
            catch (Exception x) { MessageBox.Show("ERROR: " + x, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally
            {
                Form1.form1.UpdateTable(Form1.form1.database.getData());

                foreach (TextBox textBox in textBoxes)
                {
                    textBox.Text = string.Empty;
                }
            }



        }

        public void UpdateTable(List<ProductoVenta> listProductos)
        {


                    dataGridView1.Rows.Clear();
                    foreach (ProductoVenta productoventa in listProductos)
                    {
                        DataGridViewRow fila = new DataGridViewRow();
                        fila.Cells.Add(new DataGridViewTextBoxCell() { Value = productoventa.ventas });
                        fila.Cells.Add(new DataGridViewTextBoxCell() { Value = productoventa.fecha });
                        fila.Cells.Add(new DataGridViewTextBoxCell() { Value = productoventa.nombre });
                        fila.Cells.Add(new DataGridViewTextBoxCell() { Value = productoventa.precio + "$"});
                        
                        dataGridView1.Rows.Add(fila);
                   
                    }

                    float preciototal = 0;
                    foreach (ProductoVenta productoventa in listProductos)
                    {
                        preciototal += productoventa.precio * productoventa.ventas;
                    }
                    label5.Text = String.Format("Ganancia Total: {0}$", preciototal);
                
        }

        private void ProductoManager_Load(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(wtf));
            thread.Start();
        }

        private void wtf()
        {
            UpdateTable(Form1.form1.databaseVentas.getData());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var opcion = MessageBox.Show(String.Format("¿Deseas Eliminar {0}?", productoSeleccionado), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (opcion.ToString() == "Yes")
                {
                    Form1.form1.databaseVentas.Eliminar(x => x.fecha == datetime);

                }
            }
            catch
            {
                MessageBox.Show("No Se Ha Seleccionado Ningun Producto");
            }
            
            Form1.form1.UpdateTable(Form1.form1.database.getData());
            this.UpdateTable(Form1.form1.databaseVentas.getData());
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            productoSeleccionado = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            datetime = (DateTime)dataGridView1.CurrentRow.Cells[1].Value;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                var opcion = MessageBox.Show(String.Format("¿Deseas Eliminar Todos Los Productos: {0}?", productoSeleccionado), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (opcion.ToString() == "Yes")
                {
                    var datos = Form1.form1.databaseVentas.Buscar(x => x.nombre == productoSeleccionado);
                    foreach (var item in datos) Form1.form1.databaseVentas.Eliminar(x => x.nombre == productoSeleccionado);
                }
            }
            catch
            {
                MessageBox.Show("No Se Ha Seleccionado Ningun Producto");
            }

            Form1.form1.UpdateTable(Form1.form1.database.getData());
            this.UpdateTable(Form1.form1.databaseVentas.getData());
        }
    }
}
 
using Prueba.Database;
using Prueba.Forms;
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

namespace Prueba
{
    public partial class Form1 : Form
    {
        public static Form1 form1;
        private ProductoManager productoManager;
        public DatabaseManager<Producto> database = new DatabaseManager<Producto>($"{Environment.CurrentDirectory}/database.json");
        public DatabaseManager<ProductoVenta> databaseVentas = new DatabaseManager<ProductoVenta>($"{Environment.CurrentDirectory}/databaseVentas.json");
        private String nombretemp;
        public Form1()
        {
            InitializeComponent();
            form1 = this;
            nombretemp = string.Empty;
            
        }

        public void UpdateTable(List<Producto> listProductos)
        {
            ThreadPool.QueueUserWorkItem(delegate (object item)
            {
                dataGridView1.Rows.Clear();
                foreach (Producto producto in listProductos)
                {
                    DataGridViewRow fila = new DataGridViewRow();
                    fila.Cells.Add(new DataGridViewTextBoxCell() { Value = producto.stock });
                    fila.Cells.Add(new DataGridViewTextBoxCell() { Value = producto.nombre });
                    fila.Cells.Add(new DataGridViewTextBoxCell() { Value = producto.precio });
                    dataGridView1.Rows.Add(fila);
                }
            });


            ThreadPool.QueueUserWorkItem(delegate (object item2)
            {

                float preciototal = 0;
                foreach (Producto producto in listProductos)
                {
                    preciototal += producto.precio * producto.stock;
                }
                label1.Text = $"En Total: {preciototal}$";
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            database.Cargar();
            databaseVentas.Cargar();
            UpdateTable(database.getData());
            productoManager = new ProductoManager();
            productoManager.Show();

        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Producto producto = database.Buscar(x => x.nombre == nombretemp)[0];
                producto.stock++;
                database.Actualizar(x => x.nombre == nombretemp, producto);
                UpdateTable(database.getData());
            }
            catch (Exception)
            {

                MessageBox.Show("No Has Seleccionado Ningun Producto");
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                Producto producto = database.Buscar(x => x.nombre == nombretemp)[0];
                producto.stock--;
                database.Actualizar(x => x.nombre == nombretemp, producto);
                


                // VENTA

                try
                {
                    ProductoVenta productoVenta = databaseVentas.Buscar(x => x.nombre == producto.nombre)[0];

                    productoVenta.ventas++;
                    databaseVentas.Actualizar(x => x.nombre == producto.nombre, productoVenta);
                }
                catch
                {
                    ProductoVenta productoVenta = new ProductoVenta
                    {
                        nombre = producto.nombre,
                        precio = producto.precio,
                        ventas = 1
                    };
                    databaseVentas.Insertar(productoVenta);
                }
                UpdateTable(database.getData());
            }
            catch (Exception)
            {

                MessageBox.Show("No Has Seleccionado Ningun Producto");
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var lista = database.Buscar(x => x.nombre.Contains(textBox1.Text)); UpdateTable(lista);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Producto producto = database.Buscar(x => x.nombre == dataGridView1.CurrentRow.Cells[1].Value.ToString())[0];
                nombretemp = producto.nombre;
                label2.Text = string.Format("Producto Seleccionado: {0}", producto.nombre);
            }
            catch { }
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
            productoManager.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            database.Guardar();
            databaseVentas.Guardar();
            Application.Exit();
        }
    }
}

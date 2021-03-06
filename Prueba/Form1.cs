using ProductosManager.Exceptions;
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
        private Boolean stop;

        private Thread updateTableThread;



        public Form1()
        {
            InitializeComponent();
            form1 = this;
            nombretemp = string.Empty;
            stop = false;

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

            if (stop) { MessageBox.Show("Espera Unos Momentos"); return; }

            
                try
                {
                    stop = true;
                    this.Text = stop.ToString();
                    Producto producto = database.Buscar(x => x.nombre == nombretemp)[0];

                    if (producto.stock <= 0) Custom.TestThrow();
                    producto.stock--;
                    database.Actualizar(x => x.nombre == nombretemp, producto);



                // VENTA

                    try
                    {
                        ProductoVenta productoVenta = new ProductoVenta
                        {
                            nombre = producto.nombre,
                            precio = producto.precio,
                            ventas = 1,
                            fecha = DateTime.Now
                        };
                        databaseVentas.Insertar(productoVenta);
                    }
                    catch
                    {
                        MessageBox.Show("Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        ProductoManager.productoManager.UpdateTable(databaseVentas.getData());
                        stop = false;
                    }
                    UpdateTable(database.getData());
                    stop = false;
                    this.Text = stop.ToString();
                }
                catch (ArgumentException)
                {
                    
                    MessageBox.Show("No Has Seleccionado Ningun Producto", "No Has Seleccionado Producto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    stop = false;
                }
                catch (Custom)
                {
                    
                    MessageBox.Show("Ya te quedaste sin Stock. No Puedes Vender Mas :(", "Sin Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    stop = false;
                }
            

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(delegate (object item)
            {
                var lista = database.Buscar(x => x.nombre.Contains(textBox1.Text)); UpdateTable(lista);
            });
            
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
            MessageBox.Show("Este Boton No Tiene Ninguna Funcionalidad :(");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            database.Guardar();
            databaseVentas.Guardar();
            Application.Exit();
        }
    }
}

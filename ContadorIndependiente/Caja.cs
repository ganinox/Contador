
using ContadorSinErrores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Contador
{
    public partial class Caja : Form
    {
        private string tipoTransaccion;

        public Caja()
        {
            InitializeComponent();
            ArchivoDeCaja();
            CrearDT();
            Limpiar();
        }
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        private void ArchivoDeCaja()
        {
            string filePath = Path.Combine(Application.StartupPath, "Datos.txt");

            if (!File.Exists(filePath))
            {
                // Crear archivo de texto
                StreamWriter sw = new StreamWriter(filePath);
                sw.Close();
            }
        }
        private void CrearDT()
        {
            string pathDatos = Path.Combine(Application.StartupPath, "Datos.txt");
            DataTable dt = new DataTable();
            dt.Columns.Add("Transaccion", typeof(string));
            dt.Columns.Add("Metodo de Pago", typeof(string));
            dt.Columns.Add("Monto", typeof(decimal));
            using (StreamReader sr = new StreamReader(pathDatos))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    string transaccion = parts[1];
                    string metodoPago = parts[2];
                    decimal monto = decimal.Parse(parts[0]);

                    dt.Rows.Add(transaccion, metodoPago, monto);
                }
            }
            dataGridView1.DataSource = dt;
            MostrarDatos();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Guardar();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void Guardar()
        {
            string filePath = Path.Combine(Application.StartupPath, "Datos.txt");

            // Obtener valores de los controles
            decimal monto = Convert.ToDecimal(TxtMonto.Text);


            
            string tipoTransaccion = CmbTransaccion.SelectedItem.ToString();
            
            string metodoPago = CmbMetodo.SelectedItem.ToString();
            // Calcular la diferencia según el tipo de transacción
            decimal diferencia = 0;

            if (tipoTransaccion == "Ingreso")
            {
                diferencia = monto;
            }
            else if (tipoTransaccion == "Egreso")
            {
                diferencia = -monto;
            }

            // Escribir los datos en el archivo
            if(monto >=0)
            {
                try
                {
                    EscribirLinea(filePath, monto, tipoTransaccion, metodoPago);
                }
                catch ( Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }

            }
            else
            {
                MessageBox.Show("Debe introducir un monto valido para guardar la transaccion.");
            }
            // Limpiar los controles
            Limpiar();

            // Actualizar el total según el método de pago
           
            CrearDT();
        }
        private void CmbTransaccion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmbTransaccion.SelectedIndex == 0)
            {
                tipoTransaccion = "Ingreso";
            }
            else if (CmbTransaccion.SelectedIndex == 1)
            {
                tipoTransaccion = "Egreso";
            }
        }

        private void Limpiar()
        {
            TxtMonto.Text = "0.00";
            CmbTransaccion.SelectedIndex = 0;
            CmbMetodo.SelectedIndex = 0;
        }
        private void EscribirLinea(string filePath, decimal monto, string tipoTransaccion, string metodoPago)
        {
            StreamWriter sw = new StreamWriter(filePath, true);
            sw.WriteLine($"{monto},{tipoTransaccion},{metodoPago}");
            sw.Close();
            MessageBox.Show("Datos guardados exitosamente");
        }
        private void MostrarDatos()
        {
            string pathDatos = Path.Combine(Application.StartupPath, "Datos.txt");

            DataTable dt = new DataTable();
            dt.Columns.Add("Transaccion", typeof(string));
            dt.Columns.Add("Metodo de Pago", typeof(string));
            dt.Columns.Add("Monto", typeof(decimal));

            Dictionary<string, decimal> ingresosPorMetodoPago = new Dictionary<string, decimal>();
            Dictionary<string, decimal> egresosPorMetodoPago = new Dictionary<string, decimal>();

            using (StreamReader sr = new StreamReader(pathDatos))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    string transaccion = parts[1];
                    string metodoPago = parts[2];
                    decimal monto = decimal.Parse(parts[0]);

                    dt.Rows.Add(transaccion, metodoPago, monto);

                    if (transaccion == "Ingreso")
                    {
                        if (ingresosPorMetodoPago.ContainsKey(metodoPago))
                        {
                            ingresosPorMetodoPago[metodoPago] += monto;
                        }
                        else
                        {
                            ingresosPorMetodoPago[metodoPago] = monto;
                        }
                    }
                    else if (transaccion == "Egreso")
                    {
                        if (egresosPorMetodoPago.ContainsKey(metodoPago))
                        {
                            egresosPorMetodoPago[metodoPago] += monto;
                        }
                        else
                        {
                            egresosPorMetodoPago[metodoPago] = monto;
                        }
                    }
                }
            }

            dataGridView1.DataSource = dt;
            dataGridView1.AutoResizeColumns();

            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Metodo de Pago", typeof(string));
            dt2.Columns.Add("Ingresos", typeof(decimal));
            dt2.Columns.Add("Egresos", typeof(decimal));
            dt2.Columns.Add("Total", typeof(decimal));

            foreach (string metodoPago in ingresosPorMetodoPago.Keys)
            {
                decimal ingresos = ingresosPorMetodoPago[metodoPago];
                decimal egresos = egresosPorMetodoPago.ContainsKey(metodoPago) ? egresosPorMetodoPago[metodoPago] : 0;
                decimal total = ingresos - egresos;
                dt2.Rows.Add(metodoPago, ingresos, egresos, total);
            }

            dataGridView2.DataSource = dt2;
            dataGridView2.AutoResizeColumns();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CrearDT();
        }

        private void TxtMonto_KeyPress(object sender, KeyPressEventArgs e)
        {
            Validaciones.SoloDecimales(e, TxtMonto.Text);
            
        }

        private void PanelNombre_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Caja_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void PanelSuperior_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Minimized;
                label1.Left = (ClientSize.Width - label1.Width) / 2;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }
        }
    }
}

using ContadorSinErrores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Contador
{
    public partial class Giftcards : Form
    {
        public Giftcards()
        {
            InitializeComponent();
            ArchivoDeGiftCards();
            ArchivoDeSaldos();
            ListarGiftcards();
            CrearDT();
            Limpiar();
        }
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);
        private void ArchivoDeGiftCards()
        {
            string filePath = Path.Combine(Application.StartupPath, "GiftCards.txt");

            if (!File.Exists(filePath))
            {
                // Crear archivo de texto
                StreamWriter sw = new StreamWriter(filePath);
                sw.Close();
            }
        }
        private void ArchivoDeSaldos()
        {
            string filePath = Path.Combine(Application.StartupPath, "Saldos.txt");

            if (!File.Exists(filePath))
            {
                // Crear archivo de texto
                StreamWriter sw = new StreamWriter(filePath);
                sw.Close();
            }
        }
        private void ListarGiftcards()
        {
            string filePath = Path.Combine(Application.StartupPath, "GiftCards.txt");
            string[] lines = File.ReadAllLines(filePath);

            // Agregar la primera palabra de cada línea al ComboBox
            foreach (string line in lines)
            {
                string[] words = line.Split(',');
                if (words.Length > 0)
                {
                    CmbTarjetas.Items.Add(words[0]);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Guardar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Guardar()
        {
            string filePath = Path.Combine(Application.StartupPath, "Saldos.txt");

            // Obtener valores de los controles
            decimal monto = Convert.ToDecimal(TxtMonto.Text);
            string tipoTransaccion = CmbTransaccion.SelectedItem.ToString();
            string Tarjeta = CmbTarjetas.SelectedItem.ToString();



            // Escribir los datos en el archivo
            StreamWriter sw = new StreamWriter(filePath, true);
            sw.WriteLine($"{monto},{tipoTransaccion},{Tarjeta}");
            sw.Close();

            // Mostrar mensaje de éxito
            MessageBox.Show("Datos guardados exitosamente");

            // Limpiar los controles
            TxtMonto.Text = "";
            CmbTransaccion.SelectedIndex = -1;
            CmbTarjetas.SelectedIndex = -1;
            CrearDT();
            Limpiar();
        }
        private void CrearDT()
        {
            string pathDatos = Path.Combine(Application.StartupPath, "Saldos.txt");
            DataTable dt = new DataTable();
            dt.Columns.Add("Transaccion", typeof(string));
            dt.Columns.Add("Tarjeta", typeof(string));
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
        private void MostrarDatos()
        {
            string pathDatos = Path.Combine(Application.StartupPath, "Saldos.txt");

            DataTable dt = new DataTable();
            dt.Columns.Add("Transaccion", typeof(string));
            dt.Columns.Add("Tarjeta", typeof(string));
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
                    string Tarjeta = parts[2];
                    decimal monto = decimal.Parse(parts[0]);

                    dt.Rows.Add(transaccion, Tarjeta, monto);

                    if (transaccion == "Ingreso")
                    {
                        if (ingresosPorMetodoPago.ContainsKey(Tarjeta))
                        {
                            ingresosPorMetodoPago[Tarjeta] += monto;
                        }
                        else
                        {
                            ingresosPorMetodoPago[Tarjeta] = monto;
                        }
                    }
                    else if (transaccion == "Egreso")
                    {
                        if (egresosPorMetodoPago.ContainsKey(Tarjeta))
                        {
                            egresosPorMetodoPago[Tarjeta] += monto;
                        }
                        else
                        {
                            egresosPorMetodoPago[Tarjeta] = monto;
                        }
                    }
                }
            }
            dataGridView1.DataSource = dt;
            dataGridView1.AutoResizeColumns();

            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Tarjeta", typeof(string));
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

        private void TxtMonto_KeyPress(object sender, KeyPressEventArgs e)
        {
            Validaciones.SoloDecimales(e, TxtMonto.Text);
        }

        private void MoverFormulario(object sender, MouseEventArgs e)
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

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Limpiar()
        {
            TxtMonto.Text = "0.00";
            CmbTransaccion.SelectedIndex = 0;
            CmbTarjetas.SelectedIndex = 0;
        }
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Contador
{

    public partial class Contador : Form
    {
        //Lista de consolas
        public BindingList<Temporizador> consolaList = new BindingList<Temporizador>();
        public Contador()
        {
            InitializeComponent();
            AdicionDeConsolas();
            AdicionDeBotones();
            DgvContador.CellContentClick += DgvContador_CellContentClick;
            DgvContador.RowPostPaint += DgvContador_RowPostPaint; ;
            label1.Left = (ClientSize.Width - label1.Width) / 2;
            

        }



        //Creamos los Combo box en metodo extra
        private DataGridViewComboBoxColumn CrearSeleccionadorDeHoras(string nombre)
        {
            DataGridViewComboBoxColumn combo = new DataGridViewComboBoxColumn
            {
                Name = nombre,
                HeaderText = nombre
            };

            for (int i = 0; i <= 12; i += 1)
            {
                combo.Items.Add(i.ToString());
            }
            combo.DefaultCellStyle.NullValue = "0";
            combo.ValueMember = "0";
            return combo;

        }
        private DataGridViewComboBoxColumn CrearSeleccionadorDeMinutos(string nombre)
        {
            DataGridViewComboBoxColumn combo = new DataGridViewComboBoxColumn
            {
                Name = nombre,
                HeaderText = nombre
            };
            for (int i = 0; i <= 55; i += 5)
            {
                combo.Items.Add(i.ToString());
            }
            combo.DefaultCellStyle.NullValue = "0";
            combo.ValueMember = "0";


            return combo;

        }
        private DataGridViewComboBoxColumn CrearSeleccionadorDeMandos(string nombre)
        {
            DataGridViewComboBoxColumn combo = new DataGridViewComboBoxColumn
            {
                Name = nombre,
                HeaderText = nombre
            };
            for (int i = 1; i <= 4; i += 1)
            {
                combo.Items.Add(i.ToString());
            }
            combo.DefaultCellStyle.NullValue = "1";
            combo.ValueMember = "1";
            return combo;
        }
        //Función que crea un botón en la grilla
        private DataGridViewButtonColumn CrearBotones(string nombre)
        {
            DataGridViewButtonColumn button = new DataGridViewButtonColumn
            {
                Name = nombre,
                HeaderText = nombre,
                Text = nombre,
                UseColumnTextForButtonValue = true
            };
            return button;
        }
        private void AdicionDeBotones()
        {
            //añadimos botones para ejecutar acciones
            DgvContador.Columns.Add(CrearBotones("Iniciar"));
            DgvContador.Columns.Add(CrearBotones("Parar"));
            DgvContador.Columns.Add(CrearSeleccionadorDeHoras("Horas"));
            DgvContador.Columns.Add(CrearSeleccionadorDeMinutos("Minutos"));
            DgvContador.Columns.Add(CrearSeleccionadorDeMandos("Mandos"));
            DgvContador.Columns.Add(CrearBotones("Asignar"));

        }

        private void AdicionDeConsolas()
        {
            //asignamos consolas a la lista
            consolaList.Add(new Temporizador("XBOX 360", DgvContador));
            consolaList.Add(new Temporizador("PC 1", DgvContador));
            consolaList.Add(new Temporizador("PC 2", DgvContador));
            consolaList.Add(new Temporizador("Nintendo Switch 1", DgvContador));
            consolaList.Add(new Temporizador("Nintendo Switch 2", DgvContador));
            consolaList.Add(new Temporizador("Playstation 3 - 1", DgvContador));
            consolaList.Add(new Temporizador("Playstation 4 - 1", DgvContador));
            consolaList.Add(new Temporizador("Playstation 4 - 2", DgvContador));
            consolaList.Add(new Temporizador("Playstation 5 - 1", DgvContador));
            consolaList.Add(new Temporizador("Playstation 5 - 2", DgvContador));
            consolaList.Add(new Temporizador("Playstation 5 - 3", DgvContador));
            consolaList.Add(new Temporizador("Playstation 5 - 4", DgvContador));
            consolaList.Add(new Temporizador("Oculust Rift 1", DgvContador));
            consolaList.Add(new Temporizador("Oculust Rift 2", DgvContador));
            consolaList.Add(new Temporizador("Guitar Hero", DgvContador));
            consolaList.Add(new Temporizador("Simulador de Carreras 1", DgvContador));
            consolaList.Add(new Temporizador("Simulador de Carreras 2", DgvContador));

            //Binding de la lista al datagridview
            DgvContador.DataSource = consolaList;
        }
        //Para cambiar el color de una fila cuando el tiempo haya terminado
        private void DgvContador_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            var consola = senderGrid.Rows[e.RowIndex].DataBoundItem as Temporizador;
            if (consola is null)
                return;

            DataGridViewCellStyle style = senderGrid.Rows[e.RowIndex].DefaultCellStyle;
            if (consola.Disponible)
            {
                style.BackColor = Color.Red;
            }
            if (!consola.Disponible)
            {
                style.BackColor = Color.White;
            }
        }

        //detección de todos los botones
        private void DgvContador_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                {

                }
                else
                {
                    var senderGrid = (DataGridView)sender;
                    var consola = senderGrid.Rows[e.RowIndex].DataBoundItem as Temporizador;
                    var button = senderGrid.Columns[e.ColumnIndex] as DataGridViewButtonColumn;
                    if (button is null || consola is null)
                        return;
                    AccionesDeBotones(consola, button.Name, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //Accionar de los botones
        private void AccionesDeBotones(Temporizador consola, string accion, DataGridViewCellEventArgs e)
        {
            if (accion == "Iniciar")
            {

                
                AsignarMandoConsola(e);
                AsignarClienteConsola(e);

                consola.Play();
                DgvContador.Refresh();

            }

            if (accion == "Parar")
            {
                if (consola.EnUso())
                {
                    consola.Pausa();
                }
            }
            if (accion == "Asignar")
            {
                AsignarTiempoConsola(e);
                AsignarMandoConsola(e);
                AsignarClienteConsola(e);

                DgvContador.Refresh();
            }

        }

        private void AsignarTiempoConsola(DataGridViewCellEventArgs e)
        {
            //Obtener la fila y la columna de la celda seleccionada
            int filaSeleccionada = e.RowIndex;
            int columnaSeleccionada = e.ColumnIndex;
            //Obtener la consola seleccionada de la lista de consolas
            try
            {
                Temporizador consolaSeleccionada = (Temporizador)DgvContador.Rows[filaSeleccionada].DataBoundItem;
                if (consolaSeleccionada.EnUso() == true && columnaSeleccionada == 5)
                {
                    MessageBox.Show("La consola seleccionada esta en uso, por favor seleccione una disponible o termine la funcion de esta antes de asignarle mas tiempo.");
                }
                if (consolaSeleccionada.EnUso() == false && columnaSeleccionada == 5)

                {
                    try
                    {
                        //Verificar si la columna seleccionada es la columna del botón "Asignar"
                        if (columnaSeleccionada == 5)
                        {
                            //Obtener el valor seleccionado en los ComboBox de horas y minutos
                            int horasSeleccionadas;
                            int minutosSeleccionados;

                            horasSeleccionadas = Convert.ToInt32(DgvContador.Rows[filaSeleccionada].Cells["Horas"].Value ?? 0);


                            minutosSeleccionados = Convert.ToInt32(DgvContador.Rows[filaSeleccionada].Cells["Minutos"].Value ?? 0);

                            //Asignar el tiempo seleccionado a la consola seleccionada
                            if (horasSeleccionadas == 0 && minutosSeleccionados == 0)
                            {
                                MessageBox.Show("Seleccione un lapso de tiempo valido para iniciar la asignacion");
                            }
                            else
                            {

                                consolaSeleccionada.AsignarTiempo(horasSeleccionadas, minutosSeleccionados, 0);
                            }

                            //Iniciar el temporizador de la consola seleccionada

                        }

                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);

                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        }
        private void AsignarMandoConsola(DataGridViewCellEventArgs e)
        {
            int filaSeleccionada = e.RowIndex;
            Temporizador consolaSeleccionada = (Temporizador)DgvContador.Rows[filaSeleccionada].DataBoundItem;
            consolaSeleccionada.AsignarMando(Convert.ToInt32(DgvContador.Rows[filaSeleccionada].Cells["Mandos"].Value ?? 1));
            DgvContador.Rows[0].Cells[1].Value = Convert.ToString(Convert.ToInt32(DgvContador.Rows[filaSeleccionada].Cells["Mandos"].Value ?? 1));
        }
        private void AsignarClienteConsola(DataGridViewCellEventArgs e)
        {
            int filaseleccionada = e.RowIndex;
            Temporizador consolaSeelccionada = (Temporizador)DgvContador.Rows[filaseleccionada].DataBoundItem;
            consolaSeelccionada.AsignarCliente(Convert.ToString(DgvContador.Rows[filaseleccionada].Cells["Cliente"].Value ?? "Anonimo"));
        }


        #region Botones de interfaz
        private void Btnclose_Click(object sender, EventArgs e)
        {
            
           if( MessageBox.Show("Seguro que desea salir","Alerta",MessageBoxButtons.YesNo)== DialogResult.Yes)
            {
                Application.Exit();
            }
            
        }

        private void PictureBox2_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                label1.Left = (ClientSize.Width - label1.Width) / 2;
                DgvContador.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                DgvContador.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                
            }
        }

        private void PictureBox1_Click(object sender, EventArgs e)
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
        #endregion

        #region Eventos visuales
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);


        private void PanelNombre_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void PanelSuperior_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void Label1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        private void Contador_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }



        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            Form Caja = new Caja();

            Caja.Show();
                
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form Caja = new Giftcards();

            Caja.Show();
        }
    }
}


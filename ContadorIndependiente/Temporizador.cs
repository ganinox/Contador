using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Drawing;
using Contador;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Contador.Properties;
using System.Globalization;

namespace Contador
{
    public class Temporizador : INotifyPropertyChanged
    {

        public string Consola { get; set; }
        public int Mandos { get; set; }
        private readonly System.Timers.Timer timer;
        private TimeSpan Transcurso = TimeSpan.FromSeconds(0);
        private readonly DataGridView _dataGridView;
        public string Cliente { get; set; }
        public string HoraEntrada { get; set; }
        public string HoraSalida { get; set; }

        public bool Disponible { get; set; } = false;
        public string Tiempo


        {
            get => string.Format("{0:00}:{1:00}:{2:00}", Transcurso.Hours, Transcurso.Minutes, Transcurso.Seconds);
        }

        public Temporizador(string Name, DataGridView dataGridView)
        {
            _dataGridView = dataGridView;
            this.Consola = Name;

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += TranscursoDelTimer;
            AsignarTiempo(0, 0, 0);
            AsignarMando(Mandos);
            AsignarCliente(Cliente);
        }
        private void Notificaciones(string consoleName, int Mando,string Cliente)
        {
            NotifyIcon notifyIcon = new NotifyIcon
            {
                Icon = new Icon(SystemIcons.Application, 40, 40),
                Visible = true,
                Text = "Notificacion",
                BalloonTipTitle = "El temporizador ha finalizado"
                
            };
            if (Mando == 1)
            {
                notifyIcon.BalloonTipText = "El Tiempo del cliente"+Cliente+"En la consola" + consoleName + " Con " + Mando + " mando" + " ha finalizado.";
            }
            else
            {
                notifyIcon.BalloonTipText = "El Tiempo del cliente" + Cliente + "En la consola" + consoleName + " Con " + Mando + " mandos" + " ha finalizado.";
            }
            notifyIcon.ShowBalloonTip(5000);

            Timer closeNotification = new Timer
            {
                Interval = 10000 //5 minutes in milliseconds
            };
            closeNotification.Start();
        }

        public void AsignarTiempo(int hora, int min, int seg = 0)
        {
            Disponible = false;
            Transcurso = TimeSpan.FromSeconds(hora * 3600 + min * 60 + seg);
            DateTime fechaHora = DateTime.Now;
            DateTime horaEntradaDT = fechaHora.Date.Add(fechaHora.TimeOfDay);
            DateTime horaSalidaDT = horaEntradaDT.AddSeconds(Transcurso.TotalSeconds);
            HoraEntrada = horaEntradaDT.ToString(@"hh\:mm\:ss");
            HoraSalida = horaSalidaDT.ToString(@"hh\:mm\:ss");
        }
        public void AsignarMando(int mando)
        {
            Mandos = mando;
        }
        public void AsignarCliente(String cliente)
        {
            Cliente = cliente;
        }
        public void Play() => timer.Start();
        public void Pausa() => timer.Stop();
        public bool EnUso() => timer.Enabled;

        public void Mando(int mando) => Mandos = mando;
        private void TranscursoDelTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            Transcurso = Transcurso.Add(TimeSpan.FromSeconds(-1));
            OnPropertyChanged("Tiempo"); //Avisamos que la propiedad Tiempo ha cambiado

            if (Transcurso.TotalSeconds <= 0)
            {

                Disponible = true;
                OnPropertyChanged("Disponible");
                Notificaciones(Consola, Mandos,Cliente);
                ReproducirSonido();
                Pausa();
                Disponible = true;
                
            }

        }
        public void ReproducirSonido()
        {
            System.Media.SoundPlayer sonido = new System.Media.SoundPlayer
            {
                Stream = Resources.Alerta_Ping
            };
            sonido.Play();
        }



        //Implementación de la Interfaz INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly object _lock = new object();
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            lock (_lock)
            {
                _dataGridView.Invoke((MethodInvoker)delegate
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                });
            }
        }


    }
    public static class ControlExtensions
    {
        public static void InvokeIfRequired(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}

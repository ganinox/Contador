using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace ContadorSinErrores
{
    public class Validaciones
    {
        public static bool SoloNumero(KeyPressEventArgs e)
        {
            if (char.IsNumber(e.KeyChar))
            {
                e.Handled = false;
                return true;
            }
            else if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
                return true;
            }
            else
            {
                e.Handled = true;
                return false;
            }
        }
        public static bool SoloDecimales(KeyPressEventArgs e, string cadena)
        {
            int contador = 0;
            string caracter;
            for (int n = 0; n < cadena.Length; n++)
            {
                caracter = cadena.Substring(n, 1);
                if (caracter == ".")
                {
                    contador++;
                }
            }
            bool bandera;
            if (contador == 0)
            {
                bandera = true;
                if (e.KeyChar == '.' && bandera)
                {
                    bandera = false;
                    e.Handled = false;
                }
                else if (char.IsDigit(e.KeyChar))
                {
                    e.Handled = false;
                }
                else if (char.IsControl(e.KeyChar))
                {
                    e.Handled = false;
                }
                else { e.Handled = true; }

            }
            else
            {
                bandera = false;
                e.Handled = true;
                if (char.IsDigit(e.KeyChar))
                {
                    e.Handled = false;
                }
                else if (char.IsControl(e.KeyChar))
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            }
            return true;
        }
        public static bool Vacios(TextBox ptxt)
        {
            if (ptxt.Text == string.Empty)
            {
                return true;
            }
            else return false;
        }

        public static bool ValidarCorreo
(string pCorreo)
        {
            return pCorreo != null && Regex.IsMatch(pCorreo,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}
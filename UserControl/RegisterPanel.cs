using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarSellApp
{
    public partial class RegisterPanel : UserControl
    {
        public RegisterPanel()
        {
            InitializeComponent();
        }

        public bool CheckEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                email = Regex.Replace(email, 
                    @"(@)(.+)$", DomainMapper, 
                    RegexOptions.None, TimeSpan.FromMilliseconds(200));

                string DomainMapper(Match match)
                {
                    var idn = new IdnMapping();
                    string domainName = idn.GetAscii(match.Groups[2].Value);
                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException error)
            {
                Console.WriteLine(error);
                return false;
            }
            catch (ArgumentException error)
            {
                Console.WriteLine(error);
                return false;
            }

            try
            {
                return Regex.IsMatch(email, 
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$", 
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private void labelZmianaPanelu_Click_1(object sender, EventArgs e)
        {
            Form1.instance.ChangePanelLogin(new LoginPanel()); 
        }

        private void btnRejestruj_Click_1(object sender, EventArgs e)
        {
            if (textBoxEmail.Text != "" && textBoxHaslo1.Text != "" && textBoxHaslo2.Text != "" && textBoxTel.Text != "")
            {
                if (!CheckEmailValid(textBoxEmail.Text))
                    MessageBox.Show("Twój email jest błędny!");
                else if (textBoxHaslo1.Text != textBoxHaslo2.Text)
                    MessageBox.Show("Hasła nie są zgodne!");
                else if (checkBoxZgoda.CheckState == CheckState.Unchecked)
                    MessageBox.Show("Bez zgody na przetwarzanie danych nie możesz kontynuować!");
                else
                {
                    DbOperation polaczenie = new DbOperation();
                    if (polaczenie.RejestracjaDoSerwisu(textBoxEmail.Text, textBoxHaslo1.Text, textBoxTel.Text))
                    {
                        MessageBox.Show("Udało się zarejestrować, teraz możesz się już zalogować");
                        Form1.instance.ChangePanelLogin(new LoginPanel());
                    }
                    else
                        MessageBox.Show("Konto o podanych wartosciach już istnieje");
                }
            }
            else
                MessageBox.Show("Nie podałeś wszystkich potrzebnych danych do rejestracji!");

        }
    }
}

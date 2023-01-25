using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarSellApp
{
    public partial class LoginPanel : UserControl
    {

        public LoginPanel()
        {
            InitializeComponent();
        }

        private void labelZmianaPanelu_Click_1(object sender, EventArgs e)
        {
            Form1.instance.ChangePanelLogin(new RegisterPanel());
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            DbOperation polaczenie = new DbOperation();
            string login = textBoxLogin.Text;
            string haslo = textBoxHaslo.Text;

            if (polaczenie.SprawdzLogowanie(login, haslo))
            {
                List<string> daneUzytkownika = polaczenie.LogujDoSerwisu(login, haslo);
                MainApp mainApp = new MainApp();
                mainApp.idKonta = int.Parse(daneUzytkownika[0]);
                mainApp.emailKonta = daneUzytkownika[1];
                mainApp.telKonta = daneUzytkownika[2];
                mainApp.telKonta = mainApp.telKonta.Insert(3, "-");
                mainApp.telKonta = mainApp.telKonta.Insert(7, "-");
                Form1.ActiveForm.Hide();
                mainApp.Show();
            }
            else
                MessageBox.Show("Logowanie nie powiodło się");

        }
    }
}

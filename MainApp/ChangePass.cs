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
    public partial class ChangePass : Form
    {
        public ChangePass()
        {
            InitializeComponent();
        }

        private void buttonChange_Click(object sender, EventArgs e)
        {
            DbOperation operacje = new DbOperation();
            if(textBoxPassNew1.Text != textBoxPassNew2.Text)
            {
                MessageBox.Show("Hasła nie są identyczne!");
                return;
            }
            if(operacje.ZmienHaslo(textBoxPassOld.Text, textBoxPassNew1.Text, MainApp.instance.idKonta))
            {
                MessageBox.Show("Udało się zmienić hasło!");
                MainApp.instance.changePass = false;
                ChangePass.ActiveForm.Close();
            }
            else
            {
                MessageBox.Show("Nie udało się zmienić hasła, spróbuj jeszcze raz!");
            }
                
        }

        private void ChangePass_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainApp.instance.changePass = false;
        }
    }
}

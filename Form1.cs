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
    public partial class Form1 : Form
    {
        public static Form1 instance;

        public Form1()
        {
            InitializeComponent();
            instance = this;
        }

        public void ChangePanelLogin(Control nestPnl)
        {
            panelContainer.Controls.Clear();
            panelContainer.Controls.Add(nestPnl);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panelContainer.Controls.Add(new LoginPanel());
        }
    }
}

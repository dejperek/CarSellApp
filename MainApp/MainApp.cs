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
    public partial class MainApp : Form
    {
        public static MainApp instance;

        public int idKonta;
        public string emailKonta;
        public string telKonta;
        public bool changePass = false;

        private int idSprzedajacego;

        public Panel testowyPanel = new Panel();
        
        List<Ogloszenia> ogloszenia = new List<Ogloszenia>();
        int nrOgloszenia = 0;

        public MainApp()
        {
            InitializeComponent();
            instance = this;
            testowyPanel = panel2;
        }

        private Filtrowanie FiltrowanieO()
        {
            Filtrowanie filtr = new Filtrowanie();
            //Marka

            if (listBoxMarka.SelectedItem is null)
                filtr.filtrMarka = "";
            else
                filtr.filtrMarka = $"'{listBoxMarka.SelectedItem}'";

            //Model

            if (listBoxModel.SelectedItem is null)
                filtr.filtrModel = "";
            else
                filtr.filtrModel = $"'{listBoxModel.SelectedItem}'";

            //Typ auta

            if (listBoxTyp.SelectedItem is null)
                filtr.filtrTyp = "";
            else
                filtr.filtrTyp = $"'{listBoxTyp.SelectedItem}'";

            //Rodzaj paliwa

            if (listBoxRodzajPaliwa.SelectedItem is null)
                filtr.filtrPaliwo = "";
            else
                filtr.filtrPaliwo = $"'{listBoxRodzajPaliwa.SelectedItem}'";

            ///Cena
                filtr.filtrCenaOd = textBoxCenaOd.Text.Replace(",", ".");
                filtr.filtrCenaDo = textBoxCenaDo.Text.Replace(",", ".");

            ///Przebieg
                filtr.filtrPrzebiegOd = textBoxPrzeOd.Text;
                filtr.filtrPrzebiegDo = textBoxPrzeDo.Text;

            ///Rok produkcji
                filtr.filtrRokProdOd = textBoxRokOd.Text;
                filtr.filtrRokProdDo = textBoxRokDo.Text;

            return filtr;
        }

        private void ktorytBtn(Button btn)
        {
            buttonObsOgloszenia.BackColor = Color.White;
            buttonObsOgloszenia.ForeColor = Color.Black;
            buttonMyOgl.BackColor = Color.White;
            buttonMyOgl.ForeColor = Color.Black;
            buttonOgloszenia.BackColor = Color.White;
            buttonOgloszenia.ForeColor = Color.Black;
            btn.BackColor = Color.DarkGray;
            btn.ForeColor = Color.MidnightBlue;
        }
        private void UploadOgloszenia(List<Ogloszenia> ogloszeniaF, int ktore)
        {
            if(ogloszeniaF.Count == 0)
            {
                buttonBack.Enabled = false;
                buttonNext.Enabled = false;
                buttonObs.Enabled = false;
                MessageBox.Show("Nie ma żadnych ogłoszeń z podanymi parametrami!");
                labelTytul.Text = "Brak ogłoszeń";
                labelEmailSprzedajacego.Text = "";
                labelTelefon.Text = "";
                labelCena.Text = "";
                labelOpis.Text = "";
                labelMarka.Text = "";
                labelModel.Text = "";
                labelPrzebieg.Text = "";
                labelRocznik.Text = "";
                labelNadwozie.Text = "";
                labelPaliwo.Text = "";
                labelStan.Text = "";
                labelKolor.Text = "";
                labelBezW.Text = "";
            }
            else
            {
                buttonBack.Enabled = true;
                buttonNext.Enabled = true;
                buttonObs.Enabled = true;
                idSprzedajacego = ogloszeniaF[ktore].idOgloszenia;
                labelEmailSprzedajacego.Text = ogloszenia[ktore].emailSprzedajacego;
                string tel = ogloszenia[ktore].numerTelefonu;
                tel = tel.Insert(3, "-");
                tel = tel.Insert(7, "-");
                labelTelefon.Text = tel;
                labelCena.Text = Math.Round(float.Parse(ogloszeniaF[ktore].cena), 2) + " zł";
                labelTytul.Text = ogloszeniaF[ktore].tytul;
                labelOpis.Text = ogloszeniaF[ktore].opis;
                labelMarka.Text = ogloszeniaF[ktore].marka;
                labelModel.Text = ogloszeniaF[ktore].model;
                labelPrzebieg.Text = ogloszeniaF[ktore].przebieg.ToString();
                labelRocznik.Text = ogloszeniaF[ktore].dataProdukcji.ToString();
                labelNadwozie.Text = ogloszeniaF[ktore].typ;
                labelPaliwo.Text = ogloszeniaF[ktore].rodzajPaliwa;
                labelStan.Text = ogloszeniaF[ktore].stan;
                labelKolor.Text = ogloszeniaF[ktore].kolor;
                labelBezW.Text = ogloszeniaF[ktore].bezWypadkowy.ToString() == "True" ? "Tak" : "Nie";
            }
            labelLiczbaOgloszen.Text = ogloszeniaF.Count.ToString();
        }

        public void MainApp_Load(object sender, EventArgs e)
        {
            ktorytBtn(buttonOgloszenia);
            labelEmail.Text = emailKonta;
            labelTel.Text = telKonta;
            checkedListBoxSort.SetItemCheckState(0, CheckState.Checked);
            DbOperation operacje = new DbOperation();
            listBoxMarka.Items.Clear();
            listBoxTyp.Items.Clear();
            listBoxRodzajPaliwa.Items.Clear();
            for (int i = 0; i < operacje.PobierzDaneDoFiltrowania("Marki").Count; i++)
                listBoxMarka.Items.Add(operacje.PobierzDaneDoFiltrowania("Marki")[i]);
            for (int i = 0; i < operacje.PobierzDaneDoFiltrowania("TypAuta").Count; i++)
                listBoxTyp.Items.Add(operacje.PobierzDaneDoFiltrowania("TypAuta")[i]);
            for (int i = 0; i < operacje.PobierzDaneDoFiltrowania("TypPaliwa").Count; i++)
                listBoxRodzajPaliwa.Items.Add(operacje.PobierzDaneDoFiltrowania("TypPaliwa")[i]);
            ogloszenia.Clear();
            ogloszenia = operacje.PobranieOgloszenSort(checkedListBoxSort.CheckedItems[0].ToString(), FiltrowanieO());
            UploadOgloszenia(ogloszenia, nrOgloszenia);
        }

        private void checkedListBoxSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            int sort = int.Parse(checkedListBoxSort.SelectedIndex.ToString());
            for (int i = 0; i < checkedListBoxSort.Items.Count; i++)
            {
                if (i != sort)
                    checkedListBoxSort.SetItemCheckState(i, CheckState.Unchecked);
            }
            if (checkedListBoxSort.CheckedItems.Count == 0)
                checkedListBoxSort.SetItemCheckState(0, CheckState.Checked);

            DbOperation operacje = new DbOperation();
            ogloszenia.Clear();
            ogloszenia = operacje.PobranieOgloszenSort(checkedListBoxSort.CheckedItems[0].ToString(), FiltrowanieO());
            nrOgloszenia = 0;
            UploadOgloszenia(ogloszenia, nrOgloszenia);
        }

        private void buttonWyloguj_Click(object sender, EventArgs e)
        {
            this.Close();
            new Form1().Show();
        }

        private void listBoxMarka_SelectedValueChanged(object sender, EventArgs e)
        {
            listBoxModel.Items.Clear();
            DbOperation operacje = new DbOperation();
            for (int i = 0; i < operacje.PobierzModele(listBoxMarka.SelectedIndex + 1).Count; i++)
                listBoxModel.Items.Add(operacje.PobierzModele(listBoxMarka.SelectedIndex + 1)[i]);
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (ogloszenia.Count == (nrOgloszenia + 1))
                MessageBox.Show("Jest to ostatnie ogłoszenie");
            else
            {
                nrOgloszenia++;
                UploadOgloszenia(ogloszenia, nrOgloszenia);
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (nrOgloszenia == 0)
                MessageBox.Show("Jest to pierwsze ogłoszenie");
            else
            {
                nrOgloszenia--;
                UploadOgloszenia(ogloszenia, nrOgloszenia);
            }
        }

        private void buttonResetFiltr_Click(object sender, EventArgs e)
        {
            listBoxMarka.SelectedItems.Clear();
            listBoxModel.SelectedItems.Clear();
            listBoxTyp.SelectedItems.Clear();
            listBoxRodzajPaliwa.SelectedItems.Clear();
            textBoxCenaDo.Text = "";
            textBoxCenaOd.Text = "";
            textBoxPrzeDo.Text = "";
            textBoxPrzeOd.Text = "";
            textBoxRokDo.Text = "";
            textBoxRokOd.Text = "";

            DbOperation operacje = new DbOperation();
            ogloszenia.Clear();
            ogloszenia = operacje.PobranieOgloszenSort(checkedListBoxSort.CheckedItems[0].ToString(), FiltrowanieO());
            UploadOgloszenia(ogloszenia, nrOgloszenia);
        }

        private void buttonFiltruj_Click(object sender, EventArgs e)
        {
            DbOperation operacje = new DbOperation();
            ogloszenia.Clear();
            ogloszenia = operacje.PobranieOgloszenSort(checkedListBoxSort.CheckedItems[0].ToString(), FiltrowanieO());
            UploadOgloszenia(ogloszenia, nrOgloszenia);
        }

        private void buttonAddOfer_Click(object sender, EventArgs e)
        {
            MainApp.ActiveForm.Hide();
            listBoxMarka.SelectedItems.Clear();
            listBoxModel.SelectedItems.Clear();
            listBoxTyp.SelectedItems.Clear();
            listBoxRodzajPaliwa.SelectedItems.Clear();
            textBoxCenaDo.Text = "";
            textBoxCenaOd.Text = "";
            textBoxPrzeDo.Text = "";
            textBoxPrzeOd.Text = "";
            textBoxRokDo.Text = "";
            textBoxRokOd.Text = "";
            new AddNewOfer().Show();
        }

        private void buttonObs_Click(object sender, EventArgs e)
        {
            DbOperation operacje = new DbOperation();
            //MessageBox.Show(operacje.DodajDoObserwowanych(idSprzedajacego, idKonta));
            if (operacje.DodajDoObserwowanych(idSprzedajacego, idKonta))
                MessageBox.Show("Dodane do obserwowanych ogłoszeń");
            else
                MessageBox.Show("Coś poszło nie tak, nie udało się dodać");
        }

        private void buttonObsOgloszenia_Click(object sender, EventArgs e)
        {
            DbOperation operacje = new DbOperation();
            if (operacje.SprawdzObserwowaneOgloszenia(idKonta) != 0)
            {
                ktorytBtn(buttonObsOgloszenia);
                testowyPanel.Enabled = false;
                //MainApp.ActiveForm.Controls.Remove(testowyPanel);
                buttonDelObserowane.Visible = true;
                buttonObs.Visible = false;
                nrOgloszenia = 0;
                ogloszenia = operacje.PobierzObserwowaneOgloszenia(idKonta);
                UploadOgloszenia(ogloszenia, nrOgloszenia);
                //MessageBox.Show(operacje.PobierzObserwowaneOgloszenia(idKonta));
            }
            else
                MessageBox.Show("Nie masz żadnych zaobserwowanych ogłoszeń");

        }

        private void buttonOgloszenia_Click(object sender, EventArgs e)
        {
            //MainApp.ActiveForm.Controls.Add(testowyPanel);
            testowyPanel.Enabled = true;
            ktorytBtn(buttonOgloszenia);
            labelEmail.Text = emailKonta;
            labelTel.Text = telKonta;
            checkedListBoxSort.SetItemCheckState(0, CheckState.Checked);
            DbOperation operacje = new DbOperation();
            listBoxMarka.Items.Clear();
            listBoxTyp.Items.Clear();
            listBoxRodzajPaliwa.Items.Clear();
            for (int i = 0; i < operacje.PobierzDaneDoFiltrowania("Marki").Count; i++)
                listBoxMarka.Items.Add(operacje.PobierzDaneDoFiltrowania("Marki")[i]);
            for (int i = 0; i < operacje.PobierzDaneDoFiltrowania("TypAuta").Count; i++)
                listBoxTyp.Items.Add(operacje.PobierzDaneDoFiltrowania("TypAuta")[i]);
            for (int i = 0; i < operacje.PobierzDaneDoFiltrowania("TypPaliwa").Count; i++)
                listBoxRodzajPaliwa.Items.Add(operacje.PobierzDaneDoFiltrowania("TypPaliwa")[i]);
            ogloszenia.Clear();
            ogloszenia = operacje.PobranieOgloszenSort(checkedListBoxSort.CheckedItems[0].ToString(), FiltrowanieO());
            UploadOgloszenia(ogloszenia, nrOgloszenia);
            buttonDelObserowane.Visible = false;
            buttonObs.Visible = true;
        }

        private void buttonMyOgl_Click(object sender, EventArgs e)
        {
            DbOperation operacje = new DbOperation();
            if (operacje.SprawdzMojeOgloszenia(idKonta) != 0)
            {
                ktorytBtn(buttonMyOgl);
                testowyPanel.Enabled = false;
                //MainApp.ActiveForm.Controls.Remove(testowyPanel);
                buttonDelObserowane.Visible = true;
                buttonObs.Visible = false;
                nrOgloszenia = 0;
                ogloszenia = operacje.PobierzMojeOgloszenia(idKonta);
                UploadOgloszenia(ogloszenia, nrOgloszenia);
                //MessageBox.Show(operacje.PobierzObserwowaneOgloszenia(idKonta));
            }
            else
                MessageBox.Show("Nie masz żadnych ogłoszeń");
        }

        private void buttonDelObserowane_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Czy napewno chcesz usunąć to ogłoszenie", "Uwaga", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                if (buttonObsOgloszenia.BackColor == Color.DarkGray)
                {
                    DbOperation operacje = new DbOperation();
                    if (operacje.UsunObserwowaneOgloszenie(idKonta, ogloszenia[nrOgloszenia].idOgloszenia))
                    {
                        if (operacje.SprawdzObserwowaneOgloszenia(idKonta) != 0)
                        {
                            //MainApp.ActiveForm.Controls.Remove(testowyPanel);
                            buttonDelObserowane.Visible = true;
                            buttonObs.Visible = false;
                            nrOgloszenia = 0;
                            ogloszenia = operacje.PobierzObserwowaneOgloszenia(idKonta);
                            UploadOgloszenia(ogloszenia, nrOgloszenia);
                            //MessageBox.Show(operacje.PobierzObserwowaneOgloszenia(idKonta));
                        }
                        else
                            MessageBox.Show("Nie masz żadnych zaobserwowanych ogłoszeń");
                    }
                    else
                        MessageBox.Show("Nie udało się usunąć obserwowanego ogłoszenia");
                }
                else if (buttonMyOgl.BackColor == Color.DarkGray)
                {
                    DbOperation operacje = new DbOperation();
                    if (operacje.UsunMojeOgloszenie(ogloszenia[nrOgloszenia].idOgloszenia))
                    {
                        if (operacje.SprawdzMojeOgloszenia(idKonta) != 0)
                        {
                            //MainApp.ActiveForm.Controls.Remove(testowyPanel);
                            buttonDelObserowane.Visible = true;
                            buttonObs.Visible = false;
                            nrOgloszenia = 0;
                            ogloszenia = operacje.PobierzMojeOgloszenia(idKonta);
                            UploadOgloszenia(ogloszenia, nrOgloszenia);
                            //MessageBox.Show(operacje.PobierzObserwowaneOgloszenia(idKonta));
                        }
                        else
                            MessageBox.Show("Nie masz żadnych ogłoszeń");
                    }
                    else
                        MessageBox.Show("Nie udało się usunąć ogłoszenia");
                }
            }
            else
                return;     
        }

        private void MainApp_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void buttonChangePass_Click(object sender, EventArgs e)
        {
            if (!changePass)
            {
                new ChangePass().Show();
                changePass = true;
            }
                
        }
    }
}

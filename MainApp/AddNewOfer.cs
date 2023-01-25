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
    public partial class AddNewOfer : Form
    {
        public AddNewOfer()
        {
            InitializeComponent();
        }

        bool checkLenght(int ile, TextBox text)
        {
            if (text.Text.Length <= ile)
                return true;
            else 
                return false;
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            AddNewOfer.ActiveForm.Close();
            MainApp.instance.Show();
        }

        private void AddNewOfer_Load(object sender, EventArgs e)
        {
            DbOperation operacje = new DbOperation();
            for (int i = 0; i < operacje.PobierzDaneDoFiltrowania("Marki").Count; i++)
                listBoxMarka.Items.Add(operacje.PobierzDaneDoFiltrowania("Marki")[i]);
            for (int i = 0; i < operacje.PobierzDaneDoFiltrowania("TypAuta").Count; i++)
                listBoxTyp.Items.Add(operacje.PobierzDaneDoFiltrowania("TypAuta")[i]);
            for (int i = 0; i < operacje.PobierzDaneDoFiltrowania("TypPaliwa").Count; i++)
                listBoxRodzajPaliwa.Items.Add(operacje.PobierzDaneDoFiltrowania("TypPaliwa")[i]);
            for (int i = 0; i < operacje.PobierzDaneDoFiltrowania("StanAuta").Count; i++)
                listBoxStan.Items.Add(operacje.PobierzDaneDoFiltrowania("StanAuta")[i]);
        }

        private void listBoxMarka_SelectedValueChanged(object sender, EventArgs e)
        {
            listBoxModel.Items.Clear();
            DbOperation operacje = new DbOperation();
            for (int i = 0; i < operacje.PobierzModele(listBoxMarka.SelectedIndex + 1).Count; i++)
                listBoxModel.Items.Add(operacje.PobierzModele(listBoxMarka.SelectedIndex + 1)[i]);
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            textBoxCena.Text = textBoxCena.Text.Replace(",", ".");
            Ogloszenia ogloszenie = new Ogloszenia();
            if (textBoxTytul.Text == "")
            {
                MessageBox.Show("Sprawdź wpisany tytuł i podaj poprawny!");
                return;
            }
            else if (textBoxOpis.Text == "")
            {
                MessageBox.Show("Sprawdź wpisany opis i podaj poprawny!");
                return;
            }
            else if (!int.TryParse(textBoxPrzebieg.Text, out ogloszenie.przebieg))
            {
                MessageBox.Show("Przebieg jest nieprawidłowy, popraw wpisaną wartość!");
                return;
            }
            else if(listBoxModel.SelectedItem is null)
            {
                MessageBox.Show("Nie wybrałeś modelu, wybierz model auta!");
                return;
            }
            else if (listBoxMarka.SelectedItem is null)
            {
                MessageBox.Show("Nie wybrałeś marki, wybierz markę auta!");
                return;
            }
            else if (listBoxTyp.SelectedItem is null)
            {
                MessageBox.Show("Nie wybrałeś typu samochodu, wybierz typ auta!");
                return;
            }
            else if (!int.TryParse(textBoxRokP.Text, out ogloszenie.dataProdukcji))
            {
                MessageBox.Show("Rok produkcji jest nieprawidłowy, popraw wpisaną wartość!");
                return;
            }
            else if (textBoxKolor.Text == "")
            {
                MessageBox.Show("Sprawdź wpisany kolor i podaj poprawny!");
                return;
            }
            else if (listBoxRodzajPaliwa.SelectedItem is null)
            {
                MessageBox.Show("Nie wybrałeś typu paliwa, wybierz typ paliwa!");
                return;
            }
            else if (listBoxStan.SelectedItem is null)
            {
                MessageBox.Show("Nie wybrałeś stanu auta, wybierz stan auta!");
                return;
            }
            else if (float.TryParse(textBoxCena.Text, out float cena))
            {
                MessageBox.Show("Podaj poprawną cenę auta!");
                return;
            }

            DbOperation operacje = new DbOperation();
            
            ogloszenie.idSprzedajacego = MainApp.instance.idKonta;
            ogloszenie.tytul = textBoxTytul.Text;
            ogloszenie.opis = textBoxOpis.Text;
            ogloszenie.model = (operacje.PobierzIdDoFiltracji("Modele", $"'{listBoxModel.SelectedItem}'")).ToString();
            ogloszenie.marka = (operacje.PobierzIdDoFiltracji("Marki", $"'{listBoxMarka.SelectedItem}'")).ToString();
            ogloszenie.typ = (operacje.PobierzIdDoFiltracji("TypAuta", $"'{listBoxTyp.SelectedItem}'")).ToString();
            ogloszenie.kolor = textBoxKolor.Text;
            ogloszenie.rodzajPaliwa = (operacje.PobierzIdDoFiltracji("TypPaliwa", $"'{listBoxRodzajPaliwa.SelectedItem}'")).ToString();
            ogloszenie.bezWypadkowy = checkBoxBezwypadkowy.CheckState == CheckState.Checked ? 1 : 0;
            ogloszenie.stan = (operacje.PobierzIdDoFiltracji("StanAuta", $"'{listBoxStan.SelectedItem}'")).ToString();
            ogloszenie.cena = textBoxCena.Text.Replace(",", ".");
            //MessageBox.Show(operacje.DodajOgloszenie(ogloszenie));
            
            if (operacje.DodajOgloszenie(ogloszenie))
            {
                MessageBox.Show("Pomyślnie dodane ogłoszenie");
                AddNewOfer.ActiveForm.Close();
                MainApp.instance.MainApp_Load(new object(), e);
                MainApp.instance.Show();
            }
            else
                MessageBox.Show("Błąd, nie udało się dodać twojego ogłoszenia");
        }
    }
}

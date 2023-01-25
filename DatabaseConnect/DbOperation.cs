using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace CarSellApp
{ 
    class DbOperation
    {
        static string sourceData;
        SqlConnection connection;
        SqlCommand sqlCommand;
        DataTable dataTable;
        SqlDataAdapter dataAdapter;

        public DbOperation()
        {
            sourceData = "Data Source=QWENT1661;Initial Catalog=CarSellApp;Integrated Security=True";
            connection = new SqlConnection(sourceData);
        }

        public bool UsunMojeOgloszenie(int idOgloszenia)
        {
            bool delete = false;
            try
            {
                connection.Open();
                sqlCommand = new SqlCommand($"delete ObserwowaneOgloszenia where idOgloszenia = {idOgloszenia}", connection);
                sqlCommand.ExecuteNonQuery();
                sqlCommand = new SqlCommand($"delete Ogloszenia where id = {idOgloszenia}", connection);
                sqlCommand.ExecuteNonQuery();
                delete = true;
            }
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
                delete = false;
            }
            finally
            {
                connection.Close();
            }
            return delete;
        }

        public bool UsunObserwowaneOgloszenie(int idKonta, int idObserwowanegoOgloszenia)
        {
            bool delete = false;
            try
            {
                connection.Open();
                sqlCommand = new SqlCommand($"delete ObserwowaneOgloszenia where idKonta = {idKonta} AND idOgloszenia = {idObserwowanegoOgloszenia}", connection);
                sqlCommand.ExecuteNonQuery();
                delete = true;
            }
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
                delete = false;
            }
            finally
            {
                connection.Close();
            }

            return delete;
        }
        
        public List<Ogloszenia> PobierzMojeOgloszenia(int idKonta)
        {
            List<Ogloszenia> ogloszenia = new List<Ogloszenia>();
            try
            {
                connection.Open();
                string sqlZap = $"SELECT Ogloszenia.id, idKonta, tytul, opis, przebieg, dataProdukcji, kolor, bezWypadkowy, cena, Marki.nazwa AS Marka, Modele.nazwa AS Model, TypAuta.nazwa AS Typ, TypPaliwa.nazwa AS Paliwo, StanAuta.nazwa AS Stan, Konta.email, Konta.telefon FROM Ogloszenia JOIN Marki ON Ogloszenia.idMarki = Marki.id JOIN Modele ON Ogloszenia.idModelu = Modele.id JOIN TypAuta ON Ogloszenia.idTypu = TypAuta.id JOIN TypPaliwa ON Ogloszenia.idPaliwa = TypPaliwa.id JOIN StanAuta ON Ogloszenia.idStanu = StanAuta.id JOIN Konta ON Ogloszenia.idKonta = Konta.id where idKonta = {idKonta}";
                sqlCommand = new SqlCommand(sqlZap, connection);
                sqlCommand.ExecuteNonQuery();
                dataTable = new DataTable();
                dataAdapter = new SqlDataAdapter(sqlCommand);
                dataAdapter.Fill(dataTable);
                DataRow[] rowsMojeOgloszenia = dataTable.Select();

                for (int i = 0; i < rowsMojeOgloszenia.Length; i++)
                {
                    Ogloszenia ogloszenie = new Ogloszenia();
                    ogloszenie.idOgloszenia = int.Parse(rowsMojeOgloszenia[i]["id"].ToString());
                    ogloszenie.idSprzedajacego = int.Parse(rowsMojeOgloszenia[i]["idKonta"].ToString());
                    ogloszenie.tytul = rowsMojeOgloszenia[i]["tytul"].ToString();
                    ogloszenie.opis = rowsMojeOgloszenia[i]["opis"].ToString();
                    ogloszenie.przebieg = int.Parse(rowsMojeOgloszenia[i]["przebieg"].ToString());
                    ogloszenie.typ = rowsMojeOgloszenia[i]["Typ"].ToString();
                    ogloszenie.dataProdukcji = int.Parse(rowsMojeOgloszenia[i]["dataProdukcji"].ToString());
                    ogloszenie.kolor = rowsMojeOgloszenia[i]["kolor"].ToString();
                    ogloszenie.rodzajPaliwa = rowsMojeOgloszenia[i]["Paliwo"].ToString();
                    ogloszenie.bezWypadkowy = (rowsMojeOgloszenia[i]["bezWypadkowy"].ToString() == "true" ? 1 : 0);
                    ogloszenie.stan = rowsMojeOgloszenia[i]["Stan"].ToString();
                    ogloszenie.cena = rowsMojeOgloszenia[i]["cena"].ToString();
                    ogloszenie.marka = rowsMojeOgloszenia[i]["Marka"].ToString();
                    ogloszenie.model = rowsMojeOgloszenia[i]["Model"].ToString();
                    ogloszenie.emailSprzedajacego = rowsMojeOgloszenia[i]["email"].ToString();
                    ogloszenie.numerTelefonu = rowsMojeOgloszenia[i]["telefon"].ToString();
                    ogloszenia.Add(ogloszenie);
                }
                
            }
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
            }
            finally
            {
                connection.Close();
            }
            return ogloszenia;
        }

        public int SprawdzMojeOgloszenia(int idKonta)
        {
            int iloscOgloszenObserwowanych = 0;
            try
            {
                connection.Open();
                sqlCommand = new SqlCommand($"select count(*) as ilosc from Ogloszenia where idKonta = {idKonta}", connection);
                sqlCommand.ExecuteNonQuery();
                dataTable = new DataTable();
                dataAdapter = new SqlDataAdapter(sqlCommand);
                dataAdapter.Fill(dataTable);
                DataRow[] iloscOgloszen = dataTable.Select();
                iloscOgloszenObserwowanych = int.Parse(iloscOgloszen[0]["ilosc"].ToString());
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
            finally
            {
                connection.Close();
            }
            return iloscOgloszenObserwowanych;
        }

        public List<Ogloszenia> PobierzObserwowaneOgloszenia(int idKonta)
        {
            //string test = "";
            List<Ogloszenia> ogloszenia = new List<Ogloszenia>();
            try
            {
                connection.Open();
                sqlCommand = new SqlCommand($"Select idOgloszenia from ObserwowaneOgloszenia where idKonta = {idKonta}", connection);
                sqlCommand.ExecuteNonQuery();
                dataTable = new DataTable();
                dataAdapter = new SqlDataAdapter(sqlCommand);
                dataAdapter.Fill(dataTable);
                DataRow[] rowsKonta = dataTable.Select();
                string sqlZap = "SELECT Ogloszenia.id, idKonta, tytul, opis, przebieg, dataProdukcji, kolor, bezWypadkowy, cena, Marki.nazwa AS Marka, Modele.nazwa AS Model, TypAuta.nazwa AS Typ, TypPaliwa.nazwa AS Paliwo, StanAuta.nazwa AS Stan, Konta.email, Konta.telefon FROM Ogloszenia JOIN Marki ON Ogloszenia.idMarki = Marki.id JOIN Modele ON Ogloszenia.idModelu = Modele.id JOIN TypAuta ON Ogloszenia.idTypu = TypAuta.id JOIN TypPaliwa ON Ogloszenia.idPaliwa = TypPaliwa.id JOIN StanAuta ON Ogloszenia.idStanu = StanAuta.id JOIN Konta ON Ogloszenia.idKonta = Konta.id where Ogloszenia.id =";

                for (int i = 0; i < rowsKonta.Length; i++)
                {
                    
                    //Console.WriteLine($"{sqlZap} {rowsKonta[i]["idOgloszenia"]}");
                    sqlCommand = new SqlCommand($"{sqlZap} {rowsKonta[i]["idOgloszenia"]}", connection);
                    sqlCommand.ExecuteNonQuery();
                    dataTable = new DataTable();
                    dataAdapter = new SqlDataAdapter(sqlCommand);
                    dataAdapter.Fill(dataTable);
                    DataRow[] rowsOgloszeniaObserwowane = dataTable.Select();
                    /*
                    Console.WriteLine("Ktory wiersz: " + i + "\n");
                    Console.WriteLine("Ktore id ogloszenia: " + rowsKonta[i]["idOgloszenia"] + "\n");
                    Console.WriteLine($"{sqlZap} {rowsKonta[i]["idOgloszenia"]}");
                    */
                    Ogloszenia ogloszenie = new Ogloszenia();
                    ogloszenie.idOgloszenia = int.Parse(rowsOgloszeniaObserwowane[0]["id"].ToString());
                    ogloszenie.idSprzedajacego = int.Parse(rowsOgloszeniaObserwowane[0]["idKonta"].ToString());
                    ogloszenie.tytul = rowsOgloszeniaObserwowane[0]["tytul"].ToString();
                    ogloszenie.opis = rowsOgloszeniaObserwowane[0]["opis"].ToString();
                    ogloszenie.przebieg = int.Parse(rowsOgloszeniaObserwowane[0]["przebieg"].ToString());
                    ogloszenie.typ = rowsOgloszeniaObserwowane[0]["Typ"].ToString();
                    ogloszenie.dataProdukcji = int.Parse(rowsOgloszeniaObserwowane[0]["dataProdukcji"].ToString());
                    ogloszenie.kolor = rowsOgloszeniaObserwowane[0]["kolor"].ToString();
                    ogloszenie.rodzajPaliwa = rowsOgloszeniaObserwowane[0]["Paliwo"].ToString();
                    ogloszenie.bezWypadkowy = (rowsOgloszeniaObserwowane[0]["bezWypadkowy"].ToString() == "true" ? 1 : 0);
                    ogloszenie.stan = rowsOgloszeniaObserwowane[0]["Stan"].ToString();
                    ogloszenie.cena = rowsOgloszeniaObserwowane[0]["cena"].ToString();
                    ogloszenie.marka = rowsOgloszeniaObserwowane[0]["Marka"].ToString();
                    ogloszenie.model = rowsOgloszeniaObserwowane[0]["Model"].ToString();
                    ogloszenie.emailSprzedajacego = rowsOgloszeniaObserwowane[0]["email"].ToString();
                    ogloszenie.numerTelefonu = rowsOgloszeniaObserwowane[0]["telefon"].ToString();
                    ogloszenia.Add(ogloszenie);
                }
            }
            catch(Exception error)
            {
                Console.WriteLine(error.ToString());
            }
            finally
            {
                connection.Close();
            }
            return ogloszenia;
        }

        public int SprawdzObserwowaneOgloszenia(int idKonta)
        {
            int iloscOgloszenObserwowanych = 0;
            try
            {
                connection.Open();
                sqlCommand = new SqlCommand($"select count(idOgloszenia) as ilosc from ObserwowaneOgloszenia where idKonta = {idKonta}", connection);
                sqlCommand.ExecuteNonQuery();
                dataTable = new DataTable();
                dataAdapter = new SqlDataAdapter(sqlCommand);
                dataAdapter.Fill(dataTable);
                DataRow[] iloscOgloszen = dataTable.Select();
                iloscOgloszenObserwowanych = int.Parse(iloscOgloszen[0]["ilosc"].ToString());
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
            finally
            {
                connection.Close();
            }
            return iloscOgloszenObserwowanych;
        }

        public bool DodajDoObserwowanych(int idOgloszenia, int idKonta)
        {
            //return $"INSERT INTO ObserwowaneOgloszenia VALUES ({idKonta}, {idOgloszenia})";
            bool dodanie = false;
            try
            {
                connection.Open();
                sqlCommand = new SqlCommand($"INSERT INTO ObserwowaneOgloszenia VALUES ({idKonta}, {idOgloszenia})", connection);
                sqlCommand.ExecuteNonQuery();
                dodanie = true;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                dodanie = false;
            }
            finally
            {
                connection.Close();
            }
            return dodanie;
        }

        public bool DodajOgloszenie(Ogloszenia ogloszenie)
        {
            //return $"INSERT INTO Ogloszenia VALUES ({ogloszenie.idSprzedajacego}, '{ogloszenie.tytul}', '{ogloszenie.opis}', {ogloszenie.przebieg}, {ogloszenie.marka}, {ogloszenie.model}, {ogloszenie.typ}, {ogloszenie.dataProdukcji}, '{ogloszenie.kolor}', {ogloszenie.rodzajPaliwa}, {ogloszenie.bezWypadkowy}, {ogloszenie.stan}, {ogloszenie.cena})";
            bool dodanie = false;
            try
            {
                connection.Open();
                sqlCommand = new SqlCommand($"INSERT INTO Ogloszenia VALUES ({ogloszenie.idSprzedajacego}, '{ogloszenie.tytul}', '{ogloszenie.opis}', {ogloszenie.przebieg}, {ogloszenie.marka}, {ogloszenie.model}, {ogloszenie.typ}, {ogloszenie.dataProdukcji}, '{ogloszenie.kolor}', {ogloszenie.rodzajPaliwa}, {ogloszenie.bezWypadkowy}, {ogloszenie.stan}, {ogloszenie.cena})", connection);
                sqlCommand.ExecuteNonQuery();
                dodanie = true;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                dodanie = false;
            }
            finally
            {
                connection.Close();
            }
            return dodanie;
        }

        public List<string> PobierzModele(int idMarki)
        {
            List<string> modele = new List<string>();
            try
            {
                connection.Open();
                sqlCommand = new SqlCommand($"SELECT nazwa FROM Modele where idMarki = '{idMarki}'", connection);
                sqlCommand.ExecuteNonQuery();
                dataTable = new DataTable();
                dataAdapter = new SqlDataAdapter(sqlCommand);
                dataAdapter.Fill(dataTable);
                DataRow[] rowsModele = dataTable.Select();

                for (int i = 0; i < rowsModele.Length; i++)
                {
                    string model = "";
                    model = rowsModele[i][0].ToString();
                    modele.Add(model);
                }

            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
            finally
            {
                connection.Close();
            }
            return modele;
        }

        public List<string> PobierzDaneDoFiltrowania(string dane)
        {
            List<string> daneDoFiltracji = new List<string>();
            try
            {
                connection.Open();
                sqlCommand = new SqlCommand($"SELECT nazwa FROM {dane}", connection);
                sqlCommand.ExecuteNonQuery();
                dataTable = new DataTable();
                dataAdapter = new SqlDataAdapter(sqlCommand);
                dataAdapter.Fill(dataTable);
                DataRow[] rowsDane = dataTable.Select();

                for (int i = 0; i < rowsDane.Length; i++)
                {
                    string dana = "";
                    dana = rowsDane[i][0].ToString();
                    daneDoFiltracji.Add(dana);
                }

            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
            finally
            {
                connection.Close();
            }
            return daneDoFiltracji;
        }

        public int PobierzIdDoFiltracji(string nazwaTabeli, string element)
        {
            int id;
            try
            {
                connection.Open();
                sqlCommand = new SqlCommand($"SELECT id FROM {nazwaTabeli} where nazwa = {element}", connection);
                sqlCommand.ExecuteNonQuery();
                dataTable = new DataTable();
                dataAdapter = new SqlDataAdapter(sqlCommand);
                dataAdapter.Fill(dataTable);
                DataRow[] rows = dataTable.Select();
                id = int.Parse(rows[0]["id"].ToString());
            }
            finally
            {
                connection.Close();
            }
            return id;
        }

        public List<Ogloszenia> PobranieOgloszenSort(string sortowanie, Filtrowanie filtr)
        {
                int firstFiltr = 1;
                string zapytanieFiltrujące = "";

                if(filtr.filtrMarka == "") //filtr marki
                    zapytanieFiltrujące += "";
                else
                    if (firstFiltr == 1)
                    {
                        zapytanieFiltrujące += $" Ogloszenia.idMarki = {PobierzIdDoFiltracji("Marki", filtr.filtrMarka)}";
                        firstFiltr++;
                    }
                    else
                        zapytanieFiltrujące += $" and Ogloszenia.idMarki = {PobierzIdDoFiltracji("Marki", filtr.filtrMarka)}";

                if (filtr.filtrModel == "") //filtr modelu
                    zapytanieFiltrujące += "";
                else
                    if (firstFiltr == 1)
                    {
                        zapytanieFiltrujące += $" Ogloszenia.idModelu = {PobierzIdDoFiltracji("Modele", filtr.filtrModel)}";
                        firstFiltr++;
                    }
                    else
                        zapytanieFiltrujące += $" and Ogloszenia.idModelu = {PobierzIdDoFiltracji("Modele", filtr.filtrModel)}";

                if (filtr.filtrTyp == "") //filtr typu samochodu
                        zapytanieFiltrujące += "";
                else
                    if (firstFiltr == 1)
                    {
                        zapytanieFiltrujące += $" Ogloszenia.idTypu = {PobierzIdDoFiltracji("TypAuta", filtr.filtrTyp)}";
                        firstFiltr++;
                    }
                    else
                        zapytanieFiltrujące += $" and Ogloszenia.idTypu = {PobierzIdDoFiltracji("TypAuta", filtr.filtrTyp)}";

                if (filtr.filtrPaliwo == "") //filtr paliwa
                    zapytanieFiltrujące += "";
                else
                    if (firstFiltr == 1)
                    {
                        zapytanieFiltrujące += $" Ogloszenia.idPaliwa = {PobierzIdDoFiltracji("TypPaliwa", filtr.filtrPaliwo)}";
                        firstFiltr++;
                    }
                    else
                        zapytanieFiltrujące += $" and Ogloszenia.idPaliwa = {PobierzIdDoFiltracji("TypPaliwa", filtr.filtrPaliwo)}";

                if (filtr.filtrCenaOd == "") //filtr cena od
                    zapytanieFiltrujące += "";
                else
                    if (firstFiltr == 1)
                {
                    zapytanieFiltrujące += $" Ogloszenia.cena >= {filtr.filtrCenaOd}";
                    firstFiltr++;
                }
                else
                    zapytanieFiltrujące += $" and Ogloszenia.cena >= {filtr.filtrCenaOd}";

                if (filtr.filtrCenaDo == "") //filtr cena do
                    zapytanieFiltrujące += "";
                else
                    if (firstFiltr == 1)
                {
                    zapytanieFiltrujące += $" Ogloszenia.cena <= {filtr.filtrCenaDo}";
                    firstFiltr++;
                }
                else
                    zapytanieFiltrujące += $" and Ogloszenia.cena <= {filtr.filtrCenaDo}";

                if (filtr.filtrPrzebiegOd == "") //filtr przebieg od
                    zapytanieFiltrujące += "";
                else
                    if (firstFiltr == 1)
                {
                    zapytanieFiltrujące += $" Ogloszenia.przebieg >= {filtr.filtrPrzebiegOd}";
                    firstFiltr++;
                }
                else
                    zapytanieFiltrujące += $" and Ogloszenia.przebieg >= {filtr.filtrPrzebiegOd}";

                if (filtr.filtrPrzebiegDo == "") //filtr przebieg do
                    zapytanieFiltrujące += "";
                else
                    if (firstFiltr == 1)
                {
                    zapytanieFiltrujące += $" Ogloszenia.przebieg <= {filtr.filtrPrzebiegDo}";
                    firstFiltr++;
                }
                else
                    zapytanieFiltrujące += $" and Ogloszenia.przebieg <= {filtr.filtrPrzebiegDo}";

                if (filtr.filtrRokProdOd == "") //filtr rok produkcji od
                    zapytanieFiltrujące += "";
                else
                    if (firstFiltr == 1)
                {
                    zapytanieFiltrujące += $" Ogloszenia.dataProdukcji >= {filtr.filtrRokProdOd}";
                    firstFiltr++;
                }
                else
                    zapytanieFiltrujące += $" and Ogloszenia.dataProdukcji >= {filtr.filtrRokProdOd}";

                if (filtr.filtrRokProdDo == "") //filtr rok produkcji do
                zapytanieFiltrujące += "";
                else
                    if (firstFiltr == 1)
                {
                    zapytanieFiltrujące += $" Ogloszenia.dataProdukcji <= {filtr.filtrRokProdDo}";
                    firstFiltr++;
                }
                else
                    zapytanieFiltrujące += $" and Ogloszenia.dataProdukcji <= {filtr.filtrRokProdDo}";

            if (zapytanieFiltrujące == "")
                    zapytanieFiltrujące = "1 = 1";

                List<Ogloszenia> ogloszenia = new List<Ogloszenia>();
            try
            {
                connection.Open();
                string sqlZap = "SELECT Ogloszenia.id, idKonta, tytul, opis, przebieg, dataProdukcji, kolor, bezWypadkowy, cena, Marki.nazwa AS Marka, Modele.nazwa AS Model, TypAuta.nazwa AS Typ, TypPaliwa.nazwa AS Paliwo, StanAuta.nazwa AS Stan, Konta.email, Konta.telefon FROM Ogloszenia JOIN Marki ON Ogloszenia.idMarki = Marki.id JOIN Modele ON Ogloszenia.idModelu = Modele.id JOIN TypAuta ON Ogloszenia.idTypu = TypAuta.id JOIN TypPaliwa ON Ogloszenia.idPaliwa = TypPaliwa.id JOIN StanAuta ON Ogloszenia.idStanu = StanAuta.id JOIN Konta ON Ogloszenia.idKonta = Konta.id";
                if (sortowanie == "Domyślnie")
                    sqlCommand = new SqlCommand(sqlZap + $" where {zapytanieFiltrujące}", connection);
                else if (sortowanie == "Cena najwyższa")
                    sqlCommand = new SqlCommand($"{sqlZap} where {zapytanieFiltrujące} order by cena desc", connection);
                else if (sortowanie == "Cena najniższa")
                    sqlCommand = new SqlCommand($"{sqlZap} where {zapytanieFiltrujące} order by cena asc", connection);
                else if (sortowanie == "Przebieg najwyższy")
                    sqlCommand = new SqlCommand($"{sqlZap} where {zapytanieFiltrujące} order by przebieg desc", connection);
                else if (sortowanie == "Przebieg najniższy")
                    sqlCommand = new SqlCommand($"{sqlZap} where {zapytanieFiltrujące} order by przebieg asc", connection);
                else if (sortowanie == "Rocznik najnowszy")
                    sqlCommand = new SqlCommand($"{sqlZap} where {zapytanieFiltrujące} order by dataProdukcji desc", connection);
                else if (sortowanie == "Rocznik najstarszy")
                    sqlCommand = new SqlCommand($"{sqlZap} where {zapytanieFiltrujące} order by dataProdukcji asc", connection);

                sqlCommand.ExecuteNonQuery();
                dataTable = new DataTable();
                dataAdapter = new SqlDataAdapter(sqlCommand);
                dataAdapter.Fill(dataTable);
                DataRow[] rowsKonta = dataTable.Select();

                for (int i = 0; i < rowsKonta.Length; i++)
                {
                    Ogloszenia ogloszenie = new Ogloszenia();
                    ogloszenie.idOgloszenia = int.Parse(rowsKonta[i]["id"].ToString());
                    ogloszenie.idSprzedajacego = int.Parse(rowsKonta[i]["idKonta"].ToString());
                    ogloszenie.tytul = rowsKonta[i]["tytul"].ToString();
                    ogloszenie.opis = rowsKonta[i]["opis"].ToString();
                    ogloszenie.przebieg = int.Parse(rowsKonta[i]["przebieg"].ToString());
                    ogloszenie.typ = rowsKonta[i]["Typ"].ToString();
                    ogloszenie.dataProdukcji = int.Parse(rowsKonta[i]["dataProdukcji"].ToString());
                    ogloszenie.kolor = rowsKonta[i]["kolor"].ToString();
                    ogloszenie.rodzajPaliwa = rowsKonta[i]["Paliwo"].ToString();
                    ogloszenie.bezWypadkowy = (rowsKonta[i]["bezWypadkowy"].ToString() == "true" ? 1 : 0);
                    ogloszenie.stan = rowsKonta[i]["Stan"].ToString();
                    ogloszenie.cena = rowsKonta[i]["cena"].ToString();
                    ogloszenie.marka = rowsKonta[i]["Marka"].ToString();
                    ogloszenie.model = rowsKonta[i]["Model"].ToString();
                    ogloszenie.emailSprzedajacego = rowsKonta[i]["email"].ToString();
                    ogloszenie.numerTelefonu = rowsKonta[i]["telefon"].ToString();
                    ogloszenia.Add(ogloszenie);
                }
            }
            finally
            {
                connection.Close();
            }
            return ogloszenia;
        }

        public bool RejestracjaDoSerwisu(string email, string haslo, string telefon)
        {
            bool returnValue = true;
            try
            {
                connection.Open();
                sqlCommand = new SqlCommand($"SELECT email, telefon FROM Konta", connection);
                sqlCommand.ExecuteNonQuery();
                dataTable = new DataTable();
                dataAdapter = new SqlDataAdapter(sqlCommand);
                dataAdapter.Fill(dataTable);
                DataRow[] rowsKonta = dataTable.Select();
                for (int i = 0; i < rowsKonta.Length; i++)
                {
                    if (rowsKonta[i]["email"].ToString() == email)
                        returnValue = false;
                    else if (rowsKonta[i]["telefon"].ToString() == telefon)
                        returnValue = false;
                }
                if (returnValue) 
                {
                    sqlCommand = new SqlCommand($"INSERT INTO Konta VALUES('{email}', '{HashujHaslo(haslo)}', '{telefon}')", connection);
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                returnValue = false;
            }
            finally
            {
                connection.Close();
            }
            return returnValue;
        }

        public string HashujHaslo(string haslo)
        {
            string hasloHash = "";
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(haslo);
            byte[] hash = sha256.ComputeHash(bytes);
            foreach (byte x in hash)
                hasloHash += String.Format("{0:x2}", x);

            return hasloHash;
        }

        public bool ZmienHaslo(string stareHaslo, string noweHaslo, int idKonta)
        {
            bool returnValue;

            try
            {
                connection.Open();
                sqlCommand = new SqlCommand($"SELECT count(*) FROM Konta WHERE id = {idKonta} AND haslo = '{HashujHaslo(stareHaslo)}'", connection);
                sqlCommand.ExecuteNonQuery();
                dataTable = new DataTable();
                dataAdapter = new SqlDataAdapter(sqlCommand);
                dataAdapter.Fill(dataTable);
                if (dataTable.Rows[0][0].ToString() == "1") //Sprawdza poprawnosc wpisanego hasła
                {
                    sqlCommand = new SqlCommand($"update Konta set haslo = '{HashujHaslo(noweHaslo)}' where id = {idKonta}", connection);
                    sqlCommand.ExecuteNonQuery();
                    returnValue = true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                returnValue = false;
            }
            finally
            {
                connection.Close();
            }


            return returnValue;
        }

        public bool SprawdzLogowanie(string email, string haslo)
        {
            bool returnValue;
            try
            {
                connection.Open();
                sqlCommand = new SqlCommand($"SELECT count(*) FROM Konta WHERE email = '{email}' AND haslo = '{HashujHaslo(haslo)}'", connection);
                sqlCommand.ExecuteNonQuery();
                dataTable = new DataTable();
                dataAdapter = new SqlDataAdapter(sqlCommand);
                dataAdapter.Fill(dataTable);                
                if(dataTable.Rows[0][0].ToString() == "1") //Sprawdza poprawnosc wpisanego hasła
                    returnValue = true;
                else
                    returnValue = false;
               
            }
            catch (Exception)
            {
                returnValue = false;
            }
            finally
            {
                connection.Close();
            }
            return returnValue;
        }


        public List<string> LogujDoSerwisu(string email, string haslo)
        {
            List<string> daneUzytkownika = new List<string>();
            try
            {
                connection.Open();
                sqlCommand = new SqlCommand($"SELECT id, email, telefon FROM Konta WHERE email = '{email}' AND haslo = '{HashujHaslo(haslo)}'", connection);
                sqlCommand.ExecuteNonQuery();
                dataTable = new DataTable();
                dataAdapter = new SqlDataAdapter(sqlCommand);
                dataAdapter.Fill(dataTable);
                daneUzytkownika.Add(dataTable.Rows[0][0].ToString()); //Przypisanie id
                daneUzytkownika.Add(dataTable.Rows[0][1].ToString()); //Przypisanie emaila
                daneUzytkownika.Add(dataTable.Rows[0][2].ToString()); //Przypisanie numeru telefonu
            }
            finally
            {
                connection.Close();
            }
            return daneUzytkownika;
        }

    }
}

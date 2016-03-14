using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syntax_analyzer_generator {
	class SintaksniAnalizator {


		LrTablica _tablice;
		string[] _sinkronizacijskiZnakovi;
        string[] _ulazniZnakovi;
        CvorStabla _pocetniCvor;
        List<CvorStabla> _stog;
        int _rbRedka = 0;

        bool _pogreska = false;
        bool _prazanStog = false;
        bool _prihvatio = false;
        bool _ulazniNizPrazan = false;

        int _trenutnoStanje;

		public SintaksniAnalizator(LrTablica tablice, string[] sinkronizacijskiZnakovi, string[] ulazniZnakovi) {
			_tablice = tablice;
			_sinkronizacijskiZnakovi = sinkronizacijskiZnakovi;
            _ulazniZnakovi = ulazniZnakovi;

            _stog = new List<CvorStabla>();
		}


		private string[] consoleToString() {
			string line;
			List<string> linijeUlaza = new List<string>();

			do {
				line = System.Console.ReadLine();
				if (line != null)
					linijeUlaza.Add(line);
				else
					break;
			} while (true);

			return linijeUlaza.ToArray();
		}


		public void Analiziraj(string lokacijaDatoteke) {

            // Postavljanje pocetnog stanja

            // Citanje ulaza
            string[] linijeUlaza = System.IO.File.ReadAllLines(lokacijaDatoteke);
			//string[] linijeUlaza = consoleToString();

            // Dodavanje oznake kraja stoga 
            _trenutnoStanje = 0;
            CvorStabla oznakaKrajaStoga = new CvorStabla("##KRAJSTOGA##", false, false, 0, "",_trenutnoStanje);
            _stog.Add(oznakaKrajaStoga);

            // Citamo sve ulazne linije i analiziramo
            foreach (string linijaUlaza in linijeUlaza) {

                // Parsiranje ulaznog uniformong znaka

                // Index prvog raznmaka
                int prviRazmak = linijaUlaza.IndexOf(" ");

                // Izdvajanje naziva znaka
                string znak = linijaUlaza.Substring(0, prviRazmak);

                // Index drugog razmaka
                int drugiRazmak = linijaUlaza.Substring(prviRazmak + 1).IndexOf(" ") + prviRazmak + 1;

                // String pa int rbRedka
                string rbRedkaString = linijaUlaza.Substring(prviRazmak + 1, drugiRazmak - prviRazmak - 1);

                try
                {
                    _rbRedka = Convert.ToInt32(rbRedkaString);
                }
                catch (Exception e)
                {
                    // Sto cemo ako ne uspije convertat?
                }

                // RegEx
                string regEx = linijaUlaza.Substring(drugiRazmak + 1);

                // Oporavak od pogreske
                if (_pogreska)
                {
                    pogreskaObradi(znak);
                    if (_pogreska) {
                        continue;
                    }
                }

                _pogreska = false;
                _prazanStog = false;
                _prihvatio = false;

                // Analiza akcije za ulazni znak
                analizirajAkcije(znak,regEx);

            }

            // Provjeri ima li akcija s zavrsnim znakom niza
            _rbRedka = 0;
            _ulazniNizPrazan = true;

            analizirajAkcije("\0", "");

            if (_pogreska)
            {
                dojavaPogreske("\0", "");
            }

            // Ispis cijelog stabla ako je u stanju prihvaceno
            ispisStablo(_pocetniCvor, 0);
  
		}

        public void analizirajAkcije(String znak, String regEx){
            bool zavrsniDodan = false;

            do
            {
                // Dohvat akcije
                string akcija = _tablice.GetAkcija(_trenutnoStanje, znak);

                if (akcija.Substring(0, 7).Equals("Pomakni"))
                {

                    string trenutnoStanjeString = akcija.Substring(8, akcija.IndexOf(')') - 8);

                    try
                    {
                        _trenutnoStanje = Convert.ToInt32(trenutnoStanjeString);
                    }
                    catch (Exception e)
                    {
                        // Sto cemo ako ne uspije convertat?
                    }

                    // Dodavanje novog cvora na stog
                    CvorStabla noviCvor = new CvorStabla(znak, true, false, _rbRedka, regEx, _trenutnoStanje);
                    _stog.Add(noviCvor);
                    zavrsniDodan = true;
                }
                else if (akcija.Substring(0, 7).Equals("Reducir"))
                {
                    // Parsiranje nezavrsnog znaka lijeva strana produkcije
                    string nezavrsniZnak = akcija.Substring(akcija.IndexOf('(') + 1, akcija.IndexOf(' ') - akcija.IndexOf('(') - 1);

                    // Parsiranje cijele desne strane produkcije
                    string[] desnaProdukcija = akcija.Substring(akcija.IndexOf(' ') + 4, akcija.Length - 1 - akcija.IndexOf(' ') - 4).Split(' ');

                    CvorStabla noviCvor = new CvorStabla(nezavrsniZnak, false, false, 0, "", 0);


                    if (desnaProdukcija[0].Equals("$"))
                    {
                        CvorStabla epsilonCvor = new CvorStabla("$", false, true, 0, "", 0);

                        noviCvor.DodajDjete(epsilonCvor);
                    }
                    else
                    {
                        // S stoga dodajemo djecu nezavrsnog znaka
                        for (int i = 0; i < desnaProdukcija.Length; i++)
                        {
                            noviCvor.DodajDjete(_stog[_stog.Count - (desnaProdukcija.Length - i)]);
                        }

                        // Makivamo znakove sa stoga
                        _stog.RemoveRange(_stog.Count() - desnaProdukcija.Length, desnaProdukcija.Length);

                    }
         
                         // Uzimanje trenutnog stanja s zadnjeg elementa 
                       _trenutnoStanje = _stog.ElementAt(_stog.Count() - 1).GetStanje();

                        // Postavljamo stanje novog cvora stabla
                        string akcija2 = _tablice.GetNovoStanje(_trenutnoStanje, nezavrsniZnak);
                        if (akcija2.Substring(0, 5).Equals("Stavi"))
                        {
                            string stanjeStr = akcija2.Substring(akcija2.IndexOf('(') + 1, akcija2.IndexOf(')') - akcija2.IndexOf('(') - 1);

                            int novoStanje = Convert.ToInt32(stanjeStr);

                            noviCvor.SetStanje(novoStanje);
                            _trenutnoStanje = novoStanje;
                        }

                    // Dodavanje nezavrsnog znaka na stog
                    _stog.Add(noviCvor);

                    // Postavljanje pocetnog cvora sto je potrebno za pocetak ispisa
                    _pocetniCvor = noviCvor;

                }
                else if (akcija.Substring(0, 8).Equals("Prihvati"))
                {
                    _prihvatio = true;
                    break;
                }
                else if (akcija.Substring(0, 6).Equals("Odbaci"))
                {

                    _pogreska = true;

                    dojavaPogreske(znak, regEx);

                    pogreskaObradi(znak);

                    if (_pogreska)
                    {
                        break;
                    }
                }


            } while (!zavrsniDodan);   
        
        }

         public void ispisStablo(CvorStabla cvor,int dubina){
               
                for(int i = 0; i < dubina; i++){
                    Console.Out.Write(" ");
                }

                if (cvor.ZavrsniZnak())
                {
                    Console.Out.WriteLine("{0} {1} {2}", cvor.GetZnak(), cvor.GetRbRedka(), cvor.GetRegEx());
                }
                else {
                    Console.Out.WriteLine("{0}", cvor.GetZnak());
                }

                if (cvor.GetDjeca().Count() > 0) {
                    foreach (CvorStabla cvor2 in cvor.GetDjeca()) {
                        ispisStablo(cvor2, dubina + 1);
                    }
                }
         }

         public void dojavaPogreske(String znak, String regEx)
         {
             if (!_ulazniNizPrazan)
             {
                 Console.Error.WriteLine("Procitan znak za koji se desila greska {0} {1} {2}", znak, _rbRedka, regEx);
                 Console.Error.WriteLine("Pogreska u redku {0}", _rbRedka);
             }
             else {
                 Console.Error.WriteLine("Greska se desila prilikom analize s oznakom kraja ulaznog niza");
             }

             foreach (string znakovi in _ulazniZnakovi)
             {
                 string akcija2 = _tablice.GetAkcija(_trenutnoStanje, znakovi);
                 if (!akcija2.Substring(0, 8).Equals("Odbaci()"))
                 {
                     Console.Error.WriteLine("Pogreska se ne bi desila za ulazni znak {0}", znakovi);
                 }
             }
           
         }

         public void pogreskaObradi(String znak) {
             if (_sinkronizacijskiZnakovi.Contains(znak))
             {
                 // Provjerava je li za sikronizacijski znak i element na stogu definirana akcija odbaci
                 // Ako je odbacije znak s stoga i nastavlja pretreazivanje

                 bool nijeOdbaci = false;
                 do
                 {
                     string akcija2 = _tablice.GetAkcija(_stog.ElementAt(_stog.Count - 1).GetStanje(), znak);
                     if (akcija2.Substring(0, 8).Equals("Odbaci()"))
                     {
                         _stog.RemoveAt(_stog.Count - 1);
                     }
                     else
                     {
                         nijeOdbaci = true;
                         _pogreska = false;
                     }
                 } while (!nijeOdbaci);

             }

             // Postavljanje stanja na stanje vrha stoga
             _trenutnoStanje = _stog.ElementAt(_stog.Count - 1).GetStanje();
         }

        
    }
}

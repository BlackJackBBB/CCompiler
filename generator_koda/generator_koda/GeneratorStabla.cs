using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generetor_koda
{
    class GeneratorStabla
    {

        string[] _linijeUlaza;
        List<CvorStabla> _pomocnaLista = new List<CvorStabla>();
        CvorStabla _korjenStabla;

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


        public GeneratorStabla(string ulaznaDatoteka)
        {

            // Citanje ulaza
            //_linijeUlaza = System.IO.File.ReadAllLines(ulaznaDatoteka);
			_linijeUlaza = consoleToString();

            stvoriStablo();

        }


        void stvoriStablo()
        {

            foreach (string linija in _linijeUlaza)
            {

                int brojPraznina = 0;

                for (int i = 0; i < linija.Length; i++)
                {
                    if (linija[i] != ' ')
                    {
                        brojPraznina = i;
                        break;
                    }
                }


                // Ako linija ima praznina nakon prvog znaka znaci da se radi o zavrsnom znaku
                if (linija.Substring(brojPraznina).IndexOf(' ') != -1)
                {

                    int drugaPraznina = linija.Substring(brojPraznina).IndexOf(' ');
                    int trecaPraznina = linija.Substring(brojPraznina + drugaPraznina + 1).IndexOf(' ');

                    string nazivZnaka = linija.Substring(brojPraznina, drugaPraznina);

                    string redniBrojRetka = linija.Substring(brojPraznina + drugaPraznina + 1,trecaPraznina);
                    int redniBroj = Convert.ToInt32(redniBrojRetka);

                    string izrazPrograma = linija.Substring(brojPraznina + drugaPraznina + trecaPraznina + 2);

                    CvorStabla noviCvor2 = new CvorStabla(nazivZnaka, true, false, redniBroj, izrazPrograma,0);

                    if (brojPraznina > 0)
                    {
                        _pomocnaLista[brojPraznina - 1].DodajDjete(noviCvor2);
                    }
                    else
                    {
                        _korjenStabla = noviCvor2;
                    }

                    _pomocnaLista.Insert(brojPraznina, noviCvor2);
                }
                else {
                    string nazivZnaka = linija.Substring(brojPraznina);

                    CvorStabla noviCvor;

                    if (nazivZnaka.Equals("$"))
                    {
                        noviCvor = new CvorStabla(nazivZnaka, false, true, 0, "",0);
                    }
                    else {
                        noviCvor = new CvorStabla(nazivZnaka, true, false, 0, "",0);
                    }

                    if (brojPraznina > 0)
                    {
                        _pomocnaLista[brojPraznina - 1].DodajDjete(noviCvor);
                    }
                    else
                    {
                        _korjenStabla = noviCvor;
                    }

                    _pomocnaLista.Insert(brojPraznina,noviCvor);
                }


            }

         }


		public CvorStabla Korijen {
			get { return _korjenStabla; }
		}


    }
}

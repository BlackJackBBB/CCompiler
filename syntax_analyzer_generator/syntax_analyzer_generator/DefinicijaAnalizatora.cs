using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syntax_analyzer_generator {
	class DefinicijaAnalizatora {


		private string[] _nezavrsniZnakovi;
		private string[] _zavrsniZnakovi;
		private string[] _sinkronizacijskiZnakovi;
		private List<Produkcija> _produkcije;
		private List<string> _prazniNezavrsniZnakovi;
		private Dictionary<String, List<String>> _zapocinje = new Dictionary<string, List<string>>();


		public DefinicijaAnalizatora(string lokacijaDatoteke) {

			string[] linijeUlaza = System.IO.File.ReadAllLines(lokacijaDatoteke);
			//string[] linijeUlaza = consoleToString();

			ucitajPodatke(linijeUlaza);
			prosiriListuPraznihNezavrsnih();
			racunajZapocinjeSkup();

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
		

		/// <summary>
		/// Metoda ucitava nezavršne, završne, sinkronizacijske i prazne nezavršne znakove, te produkcije.
		/// </summary>
		/// <param name="linijeUlaza"></param>
		private void ucitajPodatke(string[] linijeUlaza) {

		

			_nezavrsniZnakovi = linijeUlaza[0].Substring(3).Split(' ');
			_zavrsniZnakovi = linijeUlaza[1].Substring(3).Split(' ');
			_sinkronizacijskiZnakovi = linijeUlaza[2].Substring(5).Split(' ');

			_produkcije = new List<Produkcija>();
			_prazniNezavrsniZnakovi = new List<string>();
			String lijevaStrana = "";
			int redniBroj = 2;
			for (int i = 3; i < linijeUlaza.Length; i++) {
				if (!linijeUlaza[i].StartsWith(" ")) {
					lijevaStrana = linijeUlaza[i].Trim();

				} 
				else {
					String desnaStrana = linijeUlaza[i].Trim();
					//_produkcije.Add(new Produkcija(lijevaStrana, desnaStrana.Split(' '), redniBroj++));
					_produkcije.Add(new Produkcija(lijevaStrana, desnaStrana.Split(' ').ToList(), redniBroj++));
					
					if (desnaStrana.Equals("$")) { //ako je desna strana $ znači da je znak s lijeve strane produkcije prazan
						_prazniNezavrsniZnakovi.Add(lijevaStrana);
					}
				}
			}

			// dodavanje novog pocetnog nezavrsnog znaka 
			String noviNezavrsni = _nezavrsniZnakovi[0].Insert(_nezavrsniZnakovi[0].Length - 1, "'");
			List<string> temp = _nezavrsniZnakovi.ToList();
			temp.Add(noviNezavrsni);
			_nezavrsniZnakovi = temp.ToArray();
			//dodavanje nove produkcije koja omogućuje početak rada automata
			//_produkcije.Insert(0, new Produkcija(noviNezavrsni, new String[] { _nezavrsniZnakovi[0] }));// produkcija pocetni
			_produkcije.Insert(0, new Produkcija(noviNezavrsni, new List<string> { _nezavrsniZnakovi[0] }));

		}


		/// <summary>
		/// Metodsa proširuje listu praznih nezavršnih znakova.
		/// Metoda u listu dodaje lijeve strane onih produkcija koje na desnoj strani imaju isključivo prazne znakvove.
		/// </summary>
		private void prosiriListuPraznihNezavrsnih() {
			List<String> staraLista = new List<string>();
			List<String> novaLista = new List<string>(_prazniNezavrsniZnakovi);
			do {

				staraLista.Clear(); staraLista.AddRange(novaLista);//nova=stara;
				foreach (Produkcija p in _produkcije) {
					if (!staraLista.Contains(p.LijevaStrana)) { //ako s lijeve strane već nije prazan znak

						;
						bool noviPrazan = true;
						foreach (String znak in p.DesnaStrana) {
							if (!staraLista.Contains(znak)) {
								noviPrazan = false;
								break;

							}
						}
						if (noviPrazan == true) {
							novaLista.Add(p.LijevaStrana);
							break;
						}

					}

				}
			} while (!novaLista.SequenceEqual(staraLista));

			_prazniNezavrsniZnakovi = novaLista;
		}


		/// <summary>
		/// Metoda računa započinje skup za nezavršne znakove.
		/// Skup se čuva u instannci dictionarya čiji je ključ nezavršni znak a vrijednost lista završnih i nezavršnih znakva
		/// koji su u njegovom započinje skupu.
		/// </summary>
		private void racunajZapocinjeSkup() {

			int rows = _nezavrsniZnakovi.Length;
			int cols = _nezavrsniZnakovi.Length + _zavrsniZnakovi.Length;

			String[,] tablicaRelacijeZapocinje = new String[rows, cols];
			for (int i = 0; i < rows; i++) {
				for (int j = 0; j < _nezavrsniZnakovi.Length; j++) {
					if (zapocinjeIzravno(_nezavrsniZnakovi[i], _nezavrsniZnakovi[j])) { tablicaRelacijeZapocinje[i, j] = "1"; } else tablicaRelacijeZapocinje[i, j] = " ";
				}
				for (int j = _nezavrsniZnakovi.Length; j < cols; j++) {
					if (zapocinjeIzravno(_nezavrsniZnakovi[i], _zavrsniZnakovi[j - _nezavrsniZnakovi.Length])) { tablicaRelacijeZapocinje[i, j] = "1"; } else tablicaRelacijeZapocinje[i, j] = " ";
				}
			}


			for (int i = 0; i < rows; i++) {
				List<string> nova = new List<string>();
				for (int j = 0; j < _nezavrsniZnakovi.Length; j++) {
					if (zapocinjeIzravno(_nezavrsniZnakovi[i], _nezavrsniZnakovi[j])) nova.Add(_nezavrsniZnakovi[j]);

				}
				for (int j = _nezavrsniZnakovi.Length; j < cols; j++) {
					if (zapocinjeIzravno(_nezavrsniZnakovi[i], _zavrsniZnakovi[j - _nezavrsniZnakovi.Length]))
						nova.Add(_zavrsniZnakovi[j - _nezavrsniZnakovi.Length]);
				}
				_zapocinje.Add(_nezavrsniZnakovi[i], nova);


			}

			//u ovom trenutku zapocinje je zapravo IZRAVNO Zapocinje tablica

			foreach (KeyValuePair<string, List<string>> entry in _zapocinje) {
				_zapocinje[entry.Key].Add(entry.Key);
			}




			Boolean imaPromjene = false;
			do {
				imaPromjene = false;
				foreach (KeyValuePair<string, List<string>> entry in _zapocinje) {
					List<String> lista = new List<string>(entry.Value);
					foreach (String s in lista) {
						if (!s.Equals(entry.Key) && s.StartsWith("<")) {
							foreach (String njegov in _zapocinje[s]) {
								if (!_zapocinje[entry.Key].Contains(njegov)) {
									_zapocinje[entry.Key].Add(njegov);
									imaPromjene = true;
								}
							}
						}
					}
				}


			} while (imaPromjene == true);
		}


		/// <summary>
		/// Metoda vraća skup ZAPOCINJE za zadani skup znakova.
		/// </summary>
		/// <param name="skupZnakova"></param>
		/// <returns></returns>
		public List<string> getSkupZapocinje(List<string> skupZnakova) {
			List<string> lista = new List<String>();
			
			
			foreach (String znak in skupZnakova) {
				if (!znak.StartsWith("<")) {//ako je znak završni, njegov započinje skup je on sam
					lista.Add(znak);
				} else {
					foreach (string zn in _zapocinje[znak]) {
						if (!zn.StartsWith("<")) lista.Add(zn);
					}
			
				}
				if (!PrazniNezavrsniZnakovi.Contains(znak)) break; //ovo je nedostajalo
				//ako znak koji smo prethodno obradili nije prazan znak dodavanje u 
				//skup mora stati prema pravilu na str 103.
			}
			HashSet<String> set = new HashSet<string>(lista);//makni duplikate

			return new List<string>(set);
		}


		/// <summary>
		/// Metoda provjerava dali je moguće generirati prazni niz iz zadanog slupa znakova.
		/// </summary>
		/// <param name="skupZankova"></param>
		/// <returns></returns>
		public bool jeMoguceGeneriratPrazniNiz(List<string> skupZankova) {
			foreach (String znak in skupZankova) {
				if (!_prazniNezavrsniZnakovi.Contains(znak)) return false;
			}
			return true;
		}


		public string[] NezavrsniZnakovi {
			get { return _nezavrsniZnakovi; }
		}


		public string[] ZavrsniZnakovi {
			get { return _zavrsniZnakovi; }
		}


		public string[] SinkronizacijskiZnakovi {
			get { return _sinkronizacijskiZnakovi; }
		}


		public List<Produkcija> Produkcije {
			get { return _produkcije; }
		}


		public List<string> PrazniNezavrsniZnakovi {
			get { return _prazniNezavrsniZnakovi; }
		}


        private Boolean zapocinjeIzravno(String prvi, String drugi)
        {
            foreach (Produkcija p in _produkcije)
            {
                if (!p.LijevaStrana.Equals(prvi)) continue;//ne treba gledati te produkcije jer prvi nije s lijeve strane
                if (!p.DesnaStrana.Contains(drugi)) continue; //desna strana uopće nema drugo znaka ne gleda se
                if (p.DesnaStrana[0].Equals(drugi)) return true; //slucaj kad desna strana pocinje drugim znakom
                //slucaj_ lijeva strana je prvi, desna sadrzi trazeni ali njime ne zapocinje
                //provjeri jesu li ovi ispred prazni znakovi ako jesu vrati true
              
                
                bool je = true;
                foreach (String d in p.DesnaStrana)
                {
                    if (d.Equals(drugi)) break;
                    if (!d.StartsWith("<") || !_prazniNezavrsniZnakovi.Contains(d))
                    {
                        je = false;
                        break;
                    }

                }
                if (je == true) return true;
            }
            return false;
        }
	}


}

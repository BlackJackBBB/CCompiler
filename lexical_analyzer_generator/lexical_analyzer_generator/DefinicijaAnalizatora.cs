using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using lexical_analyzer;

namespace lexical_analyzer_generator {

	class DefinicijaLeksAnalizatora {
		
		private Dictionary<string, string> _regDef = new Dictionary<string, string>();
		private string[] _stanjaAnalizatora;
		private string[] _imenaLeksJedinki;
		private Dictionary<string, List<LeksJedinka>> _pravilaAnalizatora = new Dictionary<string,List<LeksJedinka>>();


		/// <summary>
		/// Konstruktor iscitava pravila analizatora iz zadane datoteke
		/// </summary>
		/// <param name="lokacijaDatoteke"></param>
		public DefinicijaLeksAnalizatora(string lokacijaDatoteke) {

			/*string line;
			List<string> linijeUlaza = new List<string>();
			do {
				line = System.Console.ReadLine();
				if (line != null)
					linijeUlaza.Add(line);
				else
					break;
			} while (true);
			*/

			// citam sve linije
            string[] linijeUlaza = System.IO.File.ReadAllLines(lokacijaDatoteke);


            // odredjujem redak definicije stanja i identifikatora
            int posStanja = 0;
            int posUnif = 0;
            for (int i = 0; i < linijeUlaza.Length; i++)
            {
                if (linijeUlaza[i].StartsWith("%X"))
                {
                    posStanja = i;
                }
                if (linijeUlaza[i].StartsWith("%L"))
                {
                    posUnif = i;
                    break;
                }
            }

            // regularne definicije
            for (int i = 0; i < posStanja; i++)
            {
                string linija = linijeUlaza[i];
                string sljedecaLinija = (i < (posStanja - 1) ? linijeUlaza[i + 1] : "");
                if (!sljedecaLinija.StartsWith("{"))
                {
                    linija += sljedecaLinija;
                }
                int drugaZag = linija.IndexOf("}");
                string imeRegDef = linija.Substring(1, drugaZag - 1);
                string vrijednostRegDef = linija.Substring(drugaZag + 2);
                // zamjeni ime regularne definicije u reg. izrazu
                if (i > 0)
                {
                    foreach (KeyValuePair<string, string> entry in _regDef)
                    {
                        if (vrijednostRegDef.Contains("{" + entry.Key + "}"))
                        {
                            vrijednostRegDef = vrijednostRegDef.Replace("{" + entry.Key + "}", "(" + entry.Value + ")");
                        }
                    }
                }
                _regDef.Add(imeRegDef, vrijednostRegDef);
            }


            //parsiram stanja
			String[] _stanjaAnalizatora = linijeUlaza[posStanja].Substring(3).Split(' ');

			String[] _imenaLeksJedinki = linijeUlaza[posUnif].Substring(3).Split(' ');


            //List<LeksJedinka> jedinka = new List<LeksJedinka>();
            // Ovdje pocinju pravila leksickog analizatora
            int ii = posUnif;
			ENKAGenerator generator = new ENKAGenerator();
            // Tu se parsiraju pravila leksickog analizatora i stavljaju u odgovarajucu listu.
            do {
                ii++;
                if (linijeUlaza[ii][0] == '<') {
                    string regIzraz, nazivUniformongZnaka, nazivStanja = "", nazivTrenutnogStanja;
                    int brojZnakovaJedinke = 0;
                    bool noviRed = false, vratiSe = false;

                    int a = linijeUlaza[ii].IndexOf('>');
                    // Stanje
                    nazivTrenutnogStanja = linijeUlaza[ii].Substring(1, a - 1);

                    regIzraz = linijeUlaza[ii].Substring(a + 1);

                    string kopijaRegIzraza = "" + regIzraz;                       
                    
                    for (int j = 0; j < kopijaRegIzraza.Length; j++) {
						if (kopijaRegIzraza[j] == '{') {
                            if (!ENKAGenerator.jeOperator(kopijaRegIzraza, j))
                            {
                                continue;
                            }
							int drugaZag = kopijaRegIzraza.IndexOf('}', j);

							string oznaka = kopijaRegIzraza.Substring(j + 1, drugaZag - j - 1);
							regIzraz = regIzraz.Replace("{" + oznaka + "}", '(' + _regDef[oznaka] + ')');
						}
                        
                    }  
                        
                    ii += 2;
                    nazivUniformongZnaka = linijeUlaza[ii];

                    do
                    {
                        ii++;
                        if (linijeUlaza[ii][0] == '}')
                        {
                            break;
                        }
                        else if (linijeUlaza[ii].Substring(0, 10).Equals("NOVI_REDAK"))
                        {
                            noviRed = true;
                        }
                        else if (linijeUlaza[ii].Substring(0, 8).Equals("VRATI_SE"))
                        {
                            vratiSe = true;
                            brojZnakovaJedinke = Convert.ToInt32(linijeUlaza[ii].Substring(9));
                        }
                        else if (linijeUlaza[ii].Substring(0, 13).Equals("UDJI_U_STANJE"))
                        {
                            nazivStanja = linijeUlaza[ii].Substring(14);
                        }
                    } while (true);

                    // dodavanje nove jedinke u listu stanja
					List<LeksJedinka> jedinka;
					ENKA automat = new ENKA();
					generator.GenerirajAutomat(regIzraz, automat);

					if (_pravilaAnalizatora.TryGetValue(nazivTrenutnogStanja, out jedinka)) {
						jedinka.Add(new LeksJedinka(automat, regIzraz, nazivUniformongZnaka, vratiSe, brojZnakovaJedinke, noviRed, nazivStanja));
					}
					else {
						jedinka = new List<LeksJedinka>();
						jedinka.Add(new LeksJedinka(automat, regIzraz, nazivUniformongZnaka, vratiSe, brojZnakovaJedinke, noviRed, nazivStanja));
						_pravilaAnalizatora.Add(nazivTrenutnogStanja, jedinka);
					}
                    
                }

            } while (ii < linijeUlaza.Length - 1);
        }


		/// <summary>
		/// Pravila leksičkog analizatora organiziranih tako da su leksičke grupirane sa pojedinim stanjima leksičkog anlaizatora.
		/// Ključ je ime stanja leksičkog analizatora.
		/// </summary>
		public Dictionary<string, List<LeksJedinka>> PravilaAnalizatora {
			get {
				return _pravilaAnalizatora;
			}
		}


		/// <summary>
		/// Popis imena svih stanja leksičkog analizatora.
		/// </summary>
		public string[] StanjaAnalizatora {
			get {
				return StanjaAnalizatora;
			}
		}


		/// <summary>
		/// Popis imena leksičkih jedniki.
		/// </summary>
		public string[] ImenaLeksJedinki {
			get {
				return _imenaLeksJedinki;
			}
		}


		/// <summary>
		///  Popis leksičkih definicija organiziranh tako da je ime definicije ključ.
		/// </summary>
		public Dictionary<string, string> RegDef {
			get {
				return _regDef;
			}
		}

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lexical_analyzer;

namespace lexical_analyzer_generator {
    class ENKAGenerator {

        private struct ParStanja {

			public int lijevoStanje;
			public int desnoStanje;

			/// <summary>
			/// Konstrukor stvara par stanja lijevo-desno;
			/// </summary>
			/// <param name="lijevo"></param>
			/// <param name="desno"></param>
			ParStanja(int lijevo, int desno) {
				lijevoStanje = lijevo;
				desnoStanje = desno;
			}

            
        } 

		/// <summary>
		/// Generator generira stanja za automat iz zadanog regularnog izraza.
		/// </summary>
		/// <param name="regularniIzraz"></param>
		/// <param name="automat"></param>
        public void GenerirajAutomat(string regularniIzraz, ENKA automat) {
            ParStanja rezulatat = pretvori(regularniIzraz, automat);
			automat.PocetnoStanje = rezulatat.lijevoStanje;
			automat.PrihvatljivoStanje = rezulatat.desnoStanje;
        }

		/// <summary>
		/// Metoda provjerava dali je zadani znak u regularnom izrazu operator ili je samo znak 
		/// tako što broji dali ima parani broj znakova '\' ispred zadanog znaka operatora.
		/// </summary>
		/// <param name="izraz"></param>
		/// <param name="i"></param>
		/// <returns></returns>
        public static bool jeOperator(string izraz, int i) {
            int brojac = 0;
            
            while (i - 1 >= 0 && izraz[i - 1] == '\\') {
                brojac++;
                i--;
            }

            return brojac % 2 == 0;
        }

		/// <summary>
		///  Metoda pretvara regularni izraz u automat.
		/// </summary>
		/// <param name="izraz"></param>
		/// <param name="automat"></param>
		/// <returns></returns>
        private ParStanja pretvori(string izraz, ENKA automat) {
            List<string> izbori = new List<string>();
            bool imaIzbora = false;
            int zadnjiZnak = 0;

            for (int i = 0, brojacZagrada = 0; i < izraz.Length; i++) {
                if (izraz[i] == '(' && jeOperator(izraz, i))
                    brojacZagrada++;
                else if (izraz[i] == ')' && jeOperator(izraz, i))
                    brojacZagrada--;
                else if (brojacZagrada == 0 && izraz[i] == '|' && jeOperator(izraz, i)) {
                    imaIzbora = true;
                    izbori.Add(izraz.Substring(zadnjiZnak, i - zadnjiZnak));               
                    zadnjiZnak = i + 1;
                }
            }

            if (imaIzbora) 
                izbori.Add(izraz.Substring(zadnjiZnak, izraz.Length - zadnjiZnak));

			int lijevoStanje = automat.DodajStanje();
			int desnoStanje = automat.DodajStanje();
			if (imaIzbora) {
		       foreach (dynamic izbor in izbori) {
	                ParStanja privremeno = pretvori(izbor, automat);
                    automat.DodajEpsilonPrijelaz(lijevoStanje, privremeno.lijevoStanje);
				    automat.DodajEpsilonPrijelaz(privremeno.desnoStanje, desnoStanje);
				}
			}
		    else {
	            bool prefiksirano = false;
                int zadnjeStanje = lijevoStanje;
				for (int i = 0; i < izraz.Length; i++) {
			        int a, b;
		            if (prefiksirano == true) {
						// slučaj 1
					    prefiksirano = false;
					    char prijelazniZnak;
							
						if (izraz[i] == 't')
					        prijelazniZnak = '\t';
				        else if (izraz[i] == 'n')
			                prijelazniZnak = '\n';
		                else if (izraz[i] == '_')
	                        prijelazniZnak = ' ';
                        else
							prijelazniZnak = izraz[i];

						a = automat.DodajStanje();
					    b = automat.DodajStanje();
					    automat.DodajPrijelaz(a, b, prijelazniZnak);
					}
					else {
						// slučaj 2
						if (izraz[i] == '\\') {
				            prefiksirano = true;
			                continue;
		                }

						if (izraz[i] == '(') {
					        //slučaj 2a

							int brojacZagrada = 0;
							a = b = 0;

							for (int j = i; j < izraz.Length; j++) {
								if (izraz[j] == '(' && jeOperator(izraz, j))
									brojacZagrada++;
								else if (izraz[j] == ')' && jeOperator(izraz, j))
									brojacZagrada--;
								if (brojacZagrada == 0) {
									string temp = izraz.Substring(i + 1, j - i - 1);
									ParStanja privremeno = pretvori(temp, automat);
									a = privremeno.lijevoStanje;
									b = privremeno.desnoStanje;
									i = j;
									break;
								}
							}                                     
						}	
						else {
							// slučaj 2b;

							a = automat.DodajStanje();
							b = automat.DodajStanje();

							if (izraz[i] == '$')
								automat.DodajEpsilonPrijelaz(a, b);
							else
								automat.DodajPrijelaz(a, b, izraz[i]);                                        
					    }  
					}
                        
					// provjera ponavljanja
					if (i + 1 < izraz.Length && izraz[i + 1] == '*') {
						int x = a;
						int y = b;
					    a = automat.DodajStanje();
				        b = automat.DodajStanje();
						automat.DodajEpsilonPrijelaz(a, x);
						automat.DodajEpsilonPrijelaz(y, b);
						automat.DodajEpsilonPrijelaz(a, b);
						automat.DodajEpsilonPrijelaz(y, x);
				        i++;
			        }

					//povezivanje s ostatkom automata
					automat.DodajEpsilonPrijelaz(zadnjeStanje, a);
					zadnjeStanje = b;                        
				}
				automat.DodajEpsilonPrijelaz(zadnjeStanje, desnoStanje);
			}

			ParStanja tempStanje;
			tempStanje.lijevoStanje = lijevoStanje;
			tempStanje.desnoStanje = desnoStanje;

			return tempStanje;
		}

       
    }

        
}


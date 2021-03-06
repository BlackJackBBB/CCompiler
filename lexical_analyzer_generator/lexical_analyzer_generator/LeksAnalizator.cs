﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lexical_analyzer {
    class LeksAnalizator {

        private Dictionary<string, List<LeksJedinka>> _pravilaAnalizatora;
        private string _trenutnoStanje;
        private char _trenutniZnak;
        private int _pocetak;
        private int _zavrsetak;
        private int _redak;


		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="pravilaAnalizatora"></param>
        public LeksAnalizator(Dictionary<string, List<LeksJedinka>> pravilaAnalizatora) {
            _pravilaAnalizatora = pravilaAnalizatora;
            _trenutnoStanje = _pravilaAnalizatora.Keys.ElementAt(0);
            _pocetak = 0;
            _zavrsetak = 0;
			_redak = 1;
        }


		public static string fileToString(String filepath) {
			string s = "";
			string[] lines = System.IO.File.ReadAllLines(filepath);
			foreach (String line in lines) {
				s += line + "\n";
			}
			return s.Substring(0, s.Length - 1);

		}


		private string consoleToString() {
			string fullString = "";
			string line;

			do {
				line = System.Console.ReadLine();
				if (line != null)
					fullString += line + "\n";
				else
					break;
			} while (true);

			return fullString;
		}


        public void Analiziraj(string lokacijaPrograma) {

			//string ulazniProgram = consoleToString();

			string ulazniProgram = fileToString(lokacijaPrograma);

            List<LeksJedinka> trenutnaLista = new List<LeksJedinka>();
			bool kraj = false;

            do {
                trenutnaLista = _pravilaAnalizatora[_trenutnoStanje];

                // Postavljam sve leksicke jedinke na pocetnu vrijednost
                foreach (LeksJedinka jedinka in trenutnaLista) {
                    jedinka.Aktivan = true;
                    jedinka.DuljinaProcitanogNiza = 0;
                    foreach (Stanje stanje1 in jedinka.Automat.ListaStanja) {
                        stanje1.JeAktivno = false;
                    }
                    jedinka.Automat.ListaStanja[jedinka.Automat.PocetnoStanje].JeAktivno = true;
                }

                // Svi epsilon prijelazi
                foreach (LeksJedinka jedinka in trenutnaLista) {
                    bool promjena;
					List<Prijelaz> iskoristeniPrijelazi = new List<Prijelaz>();
                    do {
                        promjena = false;
                        foreach (Prijelaz jedanPrijelaz in jedinka.Automat.ListaEpsilonPrijelaza) {
							if (jedinka.Automat.ListaStanja[jedanPrijelaz.TrenutnoStanje].JeAktivno && !iskoristeniPrijelazi.Contains<Prijelaz>(jedanPrijelaz)) {
								jedinka.Automat.ListaStanja[jedanPrijelaz.SljedeceStanje].JeAktivno = true;
								iskoristeniPrijelazi.Add(jedanPrijelaz);
						        promjena = true;
                            }
                        }
                    } while (promjena);
                }

                bool postojiAktivan;
                List<int> dodanaStanja = new List<int>();
                do {
					
					try {
						_trenutniZnak = ulazniProgram.ElementAt<char>(_zavrsetak);
					}
					catch (System.ArgumentOutOfRangeException) {
						_trenutniZnak = '\0';
						kraj = true;
					}
					
                    // Provjerava postoji li aktivan automat
                    postojiAktivan = false;

                    foreach (LeksJedinka jedinka in trenutnaLista) {

                        // Provjerava sadrzi li jedinka aktivna stanja
						if (jedinka.Aktivan) {
							dodanaStanja = new List<int>();

							// Pokupimo sve automate u koje je moguce proci
							foreach (Prijelaz jedanPrijelaz in jedinka.Automat.ListaPrijelaza) {
								if (jedinka.Automat.ListaStanja[jedanPrijelaz.TrenutnoStanje].JeAktivno) {
									if (jedanPrijelaz.Znak == _trenutniZnak) {
										dodanaStanja.Add(jedanPrijelaz.SljedeceStanje);
									}
								}
							}


							// Ako nema novih stanja jedinka se zaustavlja
							if (dodanaStanja.Count == 0) {
								jedinka.Aktivan = false;
							}
							// Ako ima novih stanja
							else {
								// Oznacimo nova stanja
								foreach (Stanje jednoStanje in jedinka.Automat.ListaStanja) {
									jednoStanje.JeAktivno = false;
								}
								foreach (int i in dodanaStanja) {
									jedinka.Automat.ListaStanja[i].JeAktivno = true;
								}

								// Provjeri je li koje od potvrdenih stanja prihvatljivo
								if (dodanaStanja.Contains(jedinka.Automat.PrihvatljivoStanje)) {
									jedinka.DuljinaProcitanogNiza = _zavrsetak - _pocetak + 1;
								}

								// Dodavanje epsilon prijelaza
								bool promjena;
								List<Prijelaz> iskoristeniPrijelazi = new List<Prijelaz>();
								do {
									promjena = false;
									foreach (Prijelaz jedanPrijelaz in jedinka.Automat.ListaEpsilonPrijelaza) {
										if (jedinka.Automat.ListaStanja[jedanPrijelaz.TrenutnoStanje].JeAktivno && !iskoristeniPrijelazi.Contains<Prijelaz>(jedanPrijelaz)) {
											jedinka.Automat.ListaStanja[jedanPrijelaz.SljedeceStanje].JeAktivno = true;
											iskoristeniPrijelazi.Add(jedanPrijelaz);
											promjena = true;
											if (jedanPrijelaz.SljedeceStanje == jedinka.Automat.PrihvatljivoStanje) {
												jedinka.DuljinaProcitanogNiza = _zavrsetak - _pocetak + 1;
											}

										}
									}
								} while (promjena);
								postojiAktivan = true;
							}
						}
                    }

					_zavrsetak++;

                } while (postojiAktivan);

                // Nadi najdulji pronadeni niz
                int najdulji = 0, brojLeksJedinke = -1, ii = 0;
                foreach (LeksJedinka jedinka in trenutnaLista) {
                    if (jedinka.DuljinaProcitanogNiza > najdulji) {
                        najdulji = jedinka.DuljinaProcitanogNiza;
                        brojLeksJedinke = ii;
                    }
                    ii++;
                }

                // Ako niti jedan automat nije prihvatio
                if (brojLeksJedinke == -1) {
                    _pocetak++;
                    _zavrsetak = _pocetak;
                }
                // Ako je barem jedan automat prihvatio
                else {
                    // Treba li procitati samo dio
                    if (trenutnaLista[brojLeksJedinke].VratiSe) {
                        trenutnaLista[brojLeksJedinke].DuljinaProcitanogNiza = trenutnaLista[brojLeksJedinke].BrojZnakovaJedinke;
                    }

                    // Provjerava redak
                    if (trenutnaLista[brojLeksJedinke].NoviRedak) {
                        _redak++;
                    }

                    // Provjerava prelazak u stanje
                    if (!trenutnaLista[brojLeksJedinke].NazivStanja.Equals("")) {
                        _trenutnoStanje = trenutnaLista[brojLeksJedinke].NazivStanja;
                    }

                    // Ispis leksicke jedinke
                    if (trenutnaLista[brojLeksJedinke].NazivUniformnogZnaka != "-") {
                        Console.WriteLine(trenutnaLista[brojLeksJedinke].NazivUniformnogZnaka + " " + _redak + " " + ulazniProgram.Substring(_pocetak, trenutnaLista[brojLeksJedinke].DuljinaProcitanogNiza));
                    }

                    // Postavljanje parametara
                    _pocetak += trenutnaLista[brojLeksJedinke].DuljinaProcitanogNiza;
                    _zavrsetak = _pocetak;
                }

            } while (!kraj);


        }

    }
}
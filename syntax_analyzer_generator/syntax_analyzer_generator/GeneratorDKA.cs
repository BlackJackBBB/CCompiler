using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syntax_analyzer_generator {
	class GeneratorDKA {

		private static int _brojac = 0;

		public static DKA generiraj(ENKA enka) {
			DKA dka = new DKA();

			dodajPocetnoStanje(enka, dka);

			for (int i = 0; i < dka.SkupStanja.Count; i++) {
				dodajNovaStanja(enka, dka, dka.SkupStanja[i]);
			}

			dka.PrihvatljivoStanje = dka.SkupStanja[1];

			return dka;
		}


		private static void dodajNovaStanja(ENKA enka, DKA dka, StanjeDKA stanje) {

			foreach (LrStavka stavka in stanje.Stavke) {
				if (!stavka.Aktivna) continue;

				string znak;
				try {
					znak = stavka.DesnaStrana.ElementAt(stavka.PozicijaTocke + 1);
				}
				catch (ArgumentOutOfRangeException) {
					continue;
				}

				HashSet<LrStavka> stavkeNovoStanje = new HashSet<LrStavka>();
				// Traze se sve stavke koje desno od tocke imaju trazeni znak
				var stavkeZaZnak = stanje.Stavke.Where(s => s.DesnaStrana.Count - 1 > s.PozicijaTocke && s.DesnaStrana[s.PozicijaTocke + 1].Equals(znak));
				foreach (LrStavka podStavka in stavkeZaZnak) {
					podStavka.Aktivna = false;

					List<PrijelazENKA> prijelazi = enka.GetListaPrijelaza(podStavka);
					foreach (PrijelazENKA prijelaz in prijelazi) 
						stavkeNovoStanje.Add(prijelaz.SljedeceStanje);
					
				}

				prosiriStanje(enka, stavkeNovoStanje);

				StanjeDKA novoStanje = new StanjeDKA(_brojac++, stavkeNovoStanje);
				StanjeDKA staroStanje = dka.SkupStanja.Find(s => s.Equals(novoStanje));
				if (staroStanje != null) {
					_brojac--;
					dka.DodajPrijelaz(stanje, staroStanje, znak);
				}
				else {
					dka.DodajStanje(novoStanje);
					dka.DodajPrijelaz(stanje, novoStanje, znak);
				}
			}

			// Vrati stavke na aktivno.
			foreach (LrStavka stavka in stanje.Stavke)
				stavka.Aktivna = true;

		}


		private static void dodajPocetnoStanje(ENKA enka, DKA dka) {
			HashSet<LrStavka> listaStavki = new HashSet<LrStavka>();

			listaStavki.Add(enka.PocetnoStanje);
			prosiriStanje(enka, listaStavki);
			dka.DodajStanje(new StanjeDKA(_brojac++, listaStavki));

		}


		private static void prosiriStanje(ENKA enka, HashSet<LrStavka> listaStavki) {
			List<LrStavka> tempLista = new List<LrStavka>(listaStavki);

			for (int i = 0; i < tempLista.Count; i++) {
				List<PrijelazENKA> epsilonPrijelazi = enka.GetListaEpsilonPrijelaza(tempLista[i]);
				foreach (PrijelazENKA prijelaz in epsilonPrijelazi) {
					if (listaStavki.Add(prijelaz.SljedeceStanje)) 
						tempLista.Add(prijelaz.SljedeceStanje);
				}
			}

		}


		//////////////////////////////////////////////DENIS///////////////////////////////////////////////////////////////////////////////

		/*
		public static DKA generiraj(ENKA enka) {
			DKA dka = new DKA();

			StanjeDKA pocetnostanje = izracunajpocetnostanje(enka, dka);
			dka.DodajStanje(pocetnostanje);

			napraviDKA(pocetnostanje, enka, dka);
			oznaciPrihvatljivoStanje(enka, dka);
			return dka;

		}


		private static StanjeDKA izracunajpocetnostanje(ENKA enka, DKA dka) {
			List<LrStavka> LrStavkePocetnogStanja = new List<LrStavka>();
			LrStavkePocetnogStanja.Add(enka.ListaStanja[0]);
			StanjeDKA pocetnostanje = napraviStanje(LrStavkePocetnogStanja, enka, dka.SkupStanja.Count);
			return pocetnostanje;
		}


		private static void napraviDKA(StanjeDKA pocetnostanje, ENKA enka, DKA dka) {
			int pozicijaulisti = 0;
			bool listasepovecala = true;

			while (listasepovecala) {
				//ova duljina je broj stanja u DKA
				int duljina = dka.SkupStanja.Count;
				List<StanjeDKA> novastanja = new List<StanjeDKA>();
				//salje se stanje iz dka, enka, broj stanja u dka i dka
				DodajNovaStanja(dka.SkupStanja[pozicijaulisti], enka, duljina, dka);
				int novaduljina = dka.SkupStanja.Count;
				if ((duljina == novaduljina) && (pozicijaulisti == novaduljina - 1) ) {
					listasepovecala = false;
				}
				pozicijaulisti += 1;
			}

		}


		/// <summary>
		/// Dodaje sva stanja u dka
		/// </summary>
		/// <param name="pocetnostanje"></param>
		/// <param name="enka"></param>
		/// <param name="brojStanjauDKA"></param>
		/// <param name="dka"></param>
		private static void DodajNovaStanja(StanjeDKA pocetnostanje, ENKA enka, int brojStanjauDKA, DKA dka) {
			//List<StanjeDKA> novastanja = new List<StanjeDKA>();
			List<string> znakovi = new List<string>();
			string znakk;
			for (int i=0; i < pocetnostanje.Stavke.Count; i++){
				if (pocetnostanje.Stavke[i].PozicijaTocke != pocetnostanje.Stavke[i].DesnaStrana.Count - 1) {
					znakk = pocetnostanje.Stavke[i].DesnaStrana[pocetnostanje.Stavke[i].PozicijaTocke + 1];
					if (!znakovi.Contains(znakk)) {
						znakovi.Add(znakk);
					}
				}	
			}

			//za svaki znak u znakovi
			foreach (string znak in znakovi) {
				List<LrStavka> LrStavkeStanja = new List<LrStavka>();
				// pogledaj prijelaze za svaku LRstavku
				for (int i = 0; i < pocetnostanje.Stavke.Count; i++) {
					List<PrijelazENKA> prijelazi = enka.GetListaPrijelaza(pocetnostanje.Stavke[i]);

					//i medu prijelazima potrazi one LRstavke koje se dobiju prijelazom za taj znak
					if (prijelazi != null) {
						for (int j = 0; j < prijelazi.Count; j++) {
							if (prijelazi[j].Znak == znak) {
								LrStavkeStanja.Add(prijelazi[j].SljedeceStanje);
							}
						}
					}
				}

				if (LrStavkeStanja.Count != 0) {
					StanjeDKA stanje = napraviStanje(LrStavkeStanja, enka, brojStanjauDKA);

					//try {
					//	StanjeDKA stanjeZaPrijelaz = dka.SkupStanja.Find(a => a.Equals(stanje));
					//	dka.DodajPrijelaz(pocetnostanje, stanjeZaPrijelaz, znak);
					//}
					//catch (NullReferenceException) {
					//	dka.DodajStanje(stanje);
					//	dka.DodajPrijelaz(pocetnostanje, stanje, znak);
					//	novastanja.Add(stanje);
					//	brojStanjauDKA += 1;
					//}

					if (!dka.SkupStanja.Contains(stanje)) {
						dka.DodajStanje(stanje);
						dka.DodajPrijelaz(pocetnostanje, stanje, znak);
						//novastanja.Add(stanje);
						brojStanjauDKA += 1;
					} else {
						StanjeDKA stanjeZaPrijelaz = dka.SkupStanja.Find(a => a.Equals(stanje));
						dka.DodajPrijelaz(pocetnostanje, stanjeZaPrijelaz, znak);
					}
					
				}
			
			}
			return;
		}


		/// <summary>
		/// U ovu metodu se posalju LRstavke koje ce biti u stanju
		/// Vraca stanje sa svojim LR stavkama
		/// </summary>
		/// <param name="LrStavkeStanja"></param>
		/// <param name="enka"></param>
		/// <param name="brojStanjauDKA"></param>
		/// <returns></returns>
		private static StanjeDKA napraviStanje(List<LrStavka> LrStavkeStanja, ENKA enka, int brojStanjauDKA) {
			int pozicija = 0;
			bool listasepovecala = true;

			while (listasepovecala) {
				int duljinaliste = LrStavkeStanja.Count;

				//nadi epsilon prijelaze
				List<PrijelazENKA> epsilonprijelazi = enka.GetListaEpsilonPrijelaza(LrStavkeStanja[pozicija]);

				if (epsilonprijelazi != null) {
					for (int i = 0; i < epsilonprijelazi.Count; i++) {
						if (!LrStavkeStanja.Contains(epsilonprijelazi[i].SljedeceStanje))
							LrStavkeStanja.Add(epsilonprijelazi[i].SljedeceStanje);
					}
				}


				int novaduljinaliste = LrStavkeStanja.Count;
				if ((duljinaliste == novaduljinaliste) && (pozicija == novaduljinaliste - 1)) {
					listasepovecala = false;
				}
				pozicija += 1;
			}

			//napravi stanje i u njega dodaj LRstavke
			StanjeDKA stanje = new StanjeDKA(brojStanjauDKA);
			for (int i = 0; i < LrStavkeStanja.Count; i++) {
				stanje.DodajStavku(LrStavkeStanja[i]);
			}

			return stanje;
		}


		private static void oznaciPrihvatljivoStanje(ENKA enka, DKA dka) {
			int redniBrojStavke = 0;
			
			dka.PrihvatljivoStanje = dka.SkupStanja[1];
			
		}*/


	}
}

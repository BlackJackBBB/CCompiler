using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syntax_analyzer_generator {
	class GeneratorENKA {


		private static int _brojac = 1;


		public static ENKA generiraj(DefinicijaAnalizatora definicija) {
			ENKA enka = new ENKA();

			enka.DodajLrStavku(new LrStavka(definicija.NezavrsniZnakovi[0].Insert(definicija.NezavrsniZnakovi[0].Count() - 1, "'"), new List<string> {".", definicija.NezavrsniZnakovi[0]}, 0, _brojac++, new List<string> {"\0"}, 1));

			for (int i = 0; i < enka.ListaStanja.Count; i++) {

				// Ako je točka na zadnjem mjestu tj. stavka oblika A -> bB...*
				if (enka.ListaStanja[i].PozicijaTocke == enka.ListaStanja[i].DesnaStrana.Count - 1)
					continue;

				DodajPomaknutuStavke(definicija, enka, enka.ListaStanja[i]);

				// Ako je produkcija oblika <A> -> ... * <znak>... tj. ako tocka nije na kraju.
				if (enka.ListaStanja[i].DesnaStrana[enka.ListaStanja[i].PozicijaTocke + 1].StartsWith("<") &&
					enka.ListaStanja[i].DesnaStrana[enka.ListaStanja[i].PozicijaTocke + 1].EndsWith(">")) 
						DodajEpsilonStavke(definicija, enka, enka.ListaStanja[i]);
			}

			enka.PocetnoStanje = enka.ListaStanja[0];

			return enka;
		}


		private static void DodajEpsilonStavke(DefinicijaAnalizatora definicija, ENKA enka, LrStavka stavka) {
			List<Produkcija> produkcije = definicija.Produkcije.FindAll(p => p.LijevaStrana == stavka.DesnaStrana[stavka.PozicijaTocke + 1]);

			foreach (var produkcija in produkcije) {
				// Stvori stavku iz produkcije dodajuci tocku na pocetak.
				List<string> novaDesnaStrana = new List<string>(produkcija.DesnaStrana);

				// Ako je epslion produkcija.
				if (novaDesnaStrana[0] == "$")
					novaDesnaStrana.RemoveAt(0);

				
				novaDesnaStrana.Insert(0, ".");

				// U stavci obika A -> ... * B betaSkup gdje je * točka izračunava se, a betaSkup skup završnih i nezavrsnih znakova ili prazan skup...
				List<string> betaSkup;
				if (stavka.PozicijaTocke + 2 <= stavka.DesnaStrana.Count - 1)
					betaSkup = stavka.DesnaStrana.GetRange(stavka.PozicijaTocke + 2, stavka.DesnaStrana.Count - stavka.PozicijaTocke - 2);
				else betaSkup = new List<string>();

				List<string> noviPodskupSlijedi = new List<string>(definicija.getSkupZapocinje(betaSkup));
				if (definicija.jeMoguceGeneriratPrazniNiz(betaSkup))
					foreach (string znak in stavka.PodskupSlijedi) 
						if (!noviPodskupSlijedi.Contains(znak))
							noviPodskupSlijedi.Add(znak);

				LrStavka nova = new LrStavka(produkcija.LijevaStrana, novaDesnaStrana, 0, _brojac++, noviPodskupSlijedi, produkcija.Rbr);
				if (!enka.ListaStanja.Contains(nova)) {
					enka.DodajLrStavku(nova);
					enka.DodajEpsilonPrijelaz(stavka, nova);
				}
				else {
					_brojac--;
					enka.DodajEpsilonPrijelaz(stavka, enka.ListaStanja.Find(s => s.Equals(nova)));
				}
				
			}

		}


		private static void DodajPomaknutuStavke(DefinicijaAnalizatora definicija, ENKA enka, LrStavka stavka) {
			List<string> novaDesnaStrana = new List<string>(stavka.DesnaStrana);

			//for (int i = stavka.PozicijaTocke + 1; i < stavka.DesnaStrana.Count; i++) {
			int i = stavka.PozicijaTocke + 1;
			//if (i < stavka.DesnaStrana.Count) {
				novaDesnaStrana = new List<string>(novaDesnaStrana);

				// Pomakni tocku.
				string temp = novaDesnaStrana[i];
				novaDesnaStrana[i] = novaDesnaStrana[i - 1];
				novaDesnaStrana[i - 1] = temp;

				LrStavka novaStavka = new LrStavka(stavka.LijevaStrana, novaDesnaStrana, i, _brojac++, stavka.PodskupSlijedi, stavka.Prioritet);

				enka.DodajLrStavku(novaStavka);

				// Povezi novu sa starom stavkom.
				enka.DodajPrijelaz(stavka, novaStavka, stavka.DesnaStrana[i]);

				//stavka = enka.ListaStanja[enka.ListaStanja.Count - 1];
			//}

		}


		/*
		public static ENKA generirajENKA(DefinicijaAnalizatora definicija) {
			ENKA enka = new ENKA();

			dodajSveStavke(definicija.Produkcije, enka);
			enka.PocetnoStanje = enka.ListaStanja[0];
			dodajPrijelaze(enka.ListaStanja, enka);
			dodajEpsilonPrijelaze(enka.ListaStanja, enka);
			

			//denis
			//definirajSveZnakove(definicija.NezavrsniZnakovi, definicija.ZavrsniZnakovi, enka);

			return enka;
		}


		/// <summary>
		/// Vraća listu LrStavki koje se dobiju iz predane produkcije.
		/// Za produkciju A->aC dobije se A->.aC A->a.C A->aC.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public static List<LrStavka> getLrStavkeizProdukcije(Produkcija p) {
			List<LrStavka> lista = new List<LrStavka>();


			if (p.DesnaStrana.Length==1 && p.DesnaStrana[0].Equals("$")) {
		
				String jedinaLijevaStrana = p.LijevaStrana;
				List<String> jedinaDesnaStrana = new List<String> { "." };
				int jedinaPozicijaTocke = 0;
				int jedinaRedniBroj = _brojac++;
				lista.Add(new LrStavka(jedinaLijevaStrana, jedinaDesnaStrana, jedinaPozicijaTocke, jedinaRedniBroj));
				return lista;
			}
			
			int brStavki = p.DesnaStrana.Length + 1;
			for (int i = 0; i < brStavki; i++) {
				
				String novaLijevaStrana = p.LijevaStrana;
				int novaPozicijaTocke = 0;
				List<string> novaDesnaStrana = new List<String>(p.DesnaStrana.Length+1);
				for (int j = 0; j < brStavki; j++) {
					if (j < i) novaDesnaStrana.Add( p.DesnaStrana[j]);
					if (j == i) { novaDesnaStrana.Add("."); novaPozicijaTocke = i; }
					if (j > i) novaDesnaStrana.Add(p.DesnaStrana[j - 1]);
					
					
				}
				int novaRedniBroj = _brojac++;
				lista.Add(new LrStavka(novaLijevaStrana, novaDesnaStrana, novaPozicijaTocke, novaRedniBroj));

			}

			return lista;
		}


		/// <summary>
		/// Metoda vraća sve LrStavke koje se dobiju iz predanih produkcija.
		/// </summary>
		/// <returns></returns>
		private static void dodajSveStavke(List<Produkcija> produkcije, ENKA enka) {
			List<LrStavka> lista = new List<LrStavka>();
			
			foreach (Produkcija p in produkcije) {
				foreach (LrStavka l in getLrStavkeizProdukcije(p)) {
				
					enka.DodajLrStavku(l);
				}
			}
			
		}


		/// <summary>
		/// Za predanu listu lrStavki računa listu Prijelaza tipa: A->.AB,A---->A->A.B (pomicanje točke u desno)
		/// </summary>
		/// <param name="listLr">lista Lrstavki za koje treba naći "prijelaze pomične točke"</param>
		private static void dodajPrijelaze(List<LrStavka> listLr, ENKA enka) {
			
			foreach (LrStavka lr in listLr) {
				if (lr.PozicijaTocke == lr.DesnaStrana.Count - 1) continue;
				
				String novaLijevaStrana = lr.LijevaStrana;
				int novaPozicijaTocke = lr.PozicijaTocke + 1;
				String znak = lr.DesnaStrana[lr.PozicijaTocke + 1];
				List<String> novaDesnaStrana = new List<String>(lr.DesnaStrana);
				
				novaDesnaStrana[lr.PozicijaTocke] = znak;
				novaDesnaStrana[novaPozicijaTocke] = ".";

				int noviRedniBroj = 0;

				// Trazi sve stavke koje odgovaraju stvorenoj i za njih se stvara prijelaz
				enka.ListaStanja.FindAll(s =>
					s.LijevaStrana == novaLijevaStrana &&
					s.DesnaStrana.SequenceEqual(novaDesnaStrana) &&
					s.PozicijaTocke == novaPozicijaTocke).ForEach(e => {
						noviRedniBroj = e.RedniBroj;
						LrStavka nova = new LrStavka(novaLijevaStrana, novaDesnaStrana, novaPozicijaTocke, noviRedniBroj);
						enka.DodajPrijelaz(lr, nova, znak);
					});
			}
			
		}


		/// <summary>
		/// Vraća epsilon prijelaze koji se dobiju iz liste LrStavki
		/// Npr A->c.B ide u sve produkcije oblika B-> .* gdje je * bilo koji skup završnih
		/// i nezavršnih znakova uključujući $.
		/// </summary>
		/// <param name="listLr"></param>
		/// <returns></returns>
		private static void dodajEpsilonPrijelaze(List<LrStavka> listLr, ENKA enka) {
			
				foreach (LrStavka lr in listLr) {
					if (lr.PozicijaTocke == lr.DesnaStrana.Count - 1)
						continue;
					if (lr.DesnaStrana[lr.PozicijaTocke + 1].StartsWith("<")) {//ako se tocka nalazi ispred nezavrsnog znaka
						String eps = lr.DesnaStrana[lr.PozicijaTocke + 1];//znak ispred kojeg je tocka
						foreach (LrStavka lrr in listLr) {
							if (lrr.LijevaStrana.Equals(eps) && lrr.PozicijaTocke == 0) {
								enka.DodajEpsilonPrijelaz(lr, lrr);
							}
						}
					}
				}
				
		}
		*/


	}
}

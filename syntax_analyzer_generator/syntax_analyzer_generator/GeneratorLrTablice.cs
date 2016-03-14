using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syntax_analyzer_generator {
	class GeneratorLrTablice {


		public static LrTablica generiraj(DKA dka) {
			LrTablica tablica = new LrTablica();

			foreach (StanjeDKA stanje in dka.SkupStanja) {
				HashSet<string> obradjeniZavrsniZnakovi = new HashSet<string>();

				if (stanje.RedniBroj == dka.PrihvatljivoStanje.RedniBroj) {
					tablica.DodajElementUTablicuAkcija(stanje.RedniBroj, "\0", "Prihvati()");
					obradjeniZavrsniZnakovi.Add("\0");
				}

				foreach (var prijelaz in dka.GetListaPrijlaza(stanje)) {
					if (prijelaz.Znak.StartsWith("<") && prijelaz.Znak.EndsWith(">"))
						tablica.DodajElementUTablicuNovoStanje(stanje.RedniBroj, prijelaz.Znak, "Stavi(" + prijelaz.SljedeceStanje.RedniBroj.ToString() + ")");
					else {
						obradjeniZavrsniZnakovi.Add(prijelaz.Znak);
						tablica.DodajElementUTablicuAkcija(stanje.RedniBroj, prijelaz.Znak, "Pomakni(" + prijelaz.SljedeceStanje.RedniBroj.ToString() + ")");
					}
				}

				var potpuneStavke = stanje.Stavke.Where(s => s.DesnaStrana.Count - 1 == s.PozicijaTocke).OrderBy(s => s.Prioritet);
				foreach (LrStavka stavka in potpuneStavke) 
					foreach (string znak in stavka.PodskupSlijedi) 
						if (!obradjeniZavrsniZnakovi.Contains(znak)) {
							tablica.DodajElementUTablicuAkcija(stanje.RedniBroj, znak, "Reduciraj(" + stavka.ToProdukcijaString() + ")");
							obradjeniZavrsniZnakovi.Add(znak);
						}
					
				

			}

			return tablica;
		}

		
		/*public static LrTablica generiraj(DKA dka, string[] zavrsniZnakovi) {
			LrTablica tablica = new LrTablica();

			foreach (var stanje in dka.SkupStanja) {

				List<string> obradjeniZnakovi = new List<string>();

				bool provjera = true;
				if (stanje.RedniBroj == dka.PrihvatljivoStanje.RedniBroj) {
					tablica.DodajElementUTablicuAkcija(stanje.RedniBroj, "\0", "Prihvati()");
					provjera = false;
				}

				// Traze se svi prijelzi iz trenutnog stanja i dodaju se akcije Pomakni u podTablicu Akcija LR tablice.
	
				
				foreach (var prijelaz in dka.GetListaPrijlaza(stanje)) {
					if (prijelaz.Znak.StartsWith("<") && prijelaz.Znak.EndsWith(">"))
						tablica.DodajElementUTablicuNovoStanje(stanje.RedniBroj, prijelaz.Znak, "Stavi(" + prijelaz.SljedeceStanje.RedniBroj.ToString() + ")");
					else {
						obradjeniZnakovi.Add(prijelaz.Znak);
						tablica.DodajElementUTablicuAkcija(stanje.RedniBroj, prijelaz.Znak, "Pomakni(" + prijelaz.SljedeceStanje.RedniBroj.ToString() + ")");
					}
				}

				// Ako nema prijalza onda stanje DKA ima potpune stavke.
				//if (!imaPrijlaza) {

					// Trazi se index potpune stavke koja ima najveci prioritet.

				var stavke = stanje.Stavke.Where(y => y.DesnaStrana.Count - 1 == y.PozicijaTocke);

				if (stavke.Count() != 0 && provjera) {
					var stavka = stavke.Where(y => y.RedniBroj == stavke.Min(x => x.RedniBroj)).First();

					foreach (var znak in zavrsniZnakovi)
						if (!obradjeniZnakovi.Contains(znak))
							tablica.DodajElementUTablicuAkcija(stanje.RedniBroj, znak, "Reduciraj(" + stavka.ToProdukcijaString() + ")");

					//tablica.DodajElementUTablicuAkcija(stanje.RedniBroj, "\0", "Reduciraj(" + stavka.ToProdukcija() + ")");
				}


				//}

			}

			return tablica;
		}*/


	}
}

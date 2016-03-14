using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace syntax_analyzer_generator {

	class ENKA {

		
		private List<LrStavka> _skupStanja = new List<LrStavka>();
		private LrStavka _pocetnoStanje;
		private Dictionary<int, List<PrijelazENKA>> _prijelazi = new Dictionary<int, List<PrijelazENKA>>();
		private Dictionary<int, List<PrijelazENKA>> _epsilonPrijelaz = new Dictionary<int, List<PrijelazENKA>>();


		//denis
		//private string[] _sviZnakovi;
		

		public ENKA() {	}


		/// <summary>
		/// Pocetno stanje automata.
		/// </summary>
		public LrStavka PocetnoStanje {
			get { return _pocetnoStanje; }
			set { _pocetnoStanje = value; }
		}


		/// <summary>
		/// Metoda dodaje stanje u automat.
		/// </summary>
		/// <returns></returns>
		public void DodajLrStavku(LrStavka stavka) {
			_skupStanja.Add(stavka);
		}


		//denis
		/*public void dodajSveZnakove(string[] znakovi) {
			_sviZnakovi = znakovi;
		}*/


		/// <summary>
		/// Metoda dodaje novi prijelaz.
		/// </summary>
		/// <param name="lijevoLrStavka"></param>
		/// <param name="desnoLrStavka"></param>
		/// <param name="znak"></param>
		public void DodajPrijelaz(LrStavka lijevaLrStavka, LrStavka desnaLrStavka, string znak) {
			try {
				PrijelazENKA prijelaz = new PrijelazENKA(lijevaLrStavka, desnaLrStavka, znak);
				if (!_prijelazi[lijevaLrStavka.RedniBroj].Contains(prijelaz))
					_prijelazi[lijevaLrStavka.RedniBroj].Add(prijelaz);
			}
			catch (KeyNotFoundException) {
				_prijelazi.Add(lijevaLrStavka.RedniBroj, new List<PrijelazENKA> { new PrijelazENKA(lijevaLrStavka, desnaLrStavka, znak) });
			}
			
		}


		public void DodajEpsilonPrijelaz(LrStavka lijevaLrStavka, LrStavka desnaLrStavka) {
			try {
				PrijelazENKA prijelaz = new PrijelazENKA(lijevaLrStavka, desnaLrStavka, "$");
				if (!_epsilonPrijelaz[lijevaLrStavka.RedniBroj].Contains(prijelaz))
					_epsilonPrijelaz[lijevaLrStavka.RedniBroj].Add(prijelaz);
			}
			catch (KeyNotFoundException) {
				_epsilonPrijelaz.Add(lijevaLrStavka.RedniBroj, new List<PrijelazENKA> { new PrijelazENKA(lijevaLrStavka, desnaLrStavka, "$") });
			}
		}


		//denis
		//vraca sve znakove, zavrsne i nezavrsne
		/*public string[] SviZnakovi {
			get { return _sviZnakovi; }
		}*/


		/// <summary>
		/// Metoda vraća listu svih prijlaza za zadanu stavku.
		/// Ukoliko ne postoji niti jedan prijelaz za zadanu stavku metoda vraća null
		/// </summary>
		/// <param name="stavka"></param>
		/// <returns></returns>
		public List<PrijelazENKA> GetListaPrijelaza(LrStavka stavka) {
			try {
				return _prijelazi[stavka.RedniBroj];
			}
			catch (KeyNotFoundException) {
				return new List<PrijelazENKA>();
			}
		}

	
		/// <summary>
		/// Metoda vraća listu svih epsilon prijlaza za zadanu stavku.
		/// Ukoliko ne postoji niti jedan epsilon prijelaz za zadanu stavku metoda vraća null
		/// </summary>
		/// <param name="stavka"></param>
		/// <returns></returns>
		public List<PrijelazENKA> GetListaEpsilonPrijelaza(LrStavka stavka) {
			try {
				return _epsilonPrijelaz[stavka.RedniBroj];
			}
			catch (KeyNotFoundException) {
				return new List<PrijelazENKA>();
			}
		}


		/// <summary>
		/// Lista svih stanja automata.
		/// </summary>
		public List<LrStavka> ListaStanja {
			get { return _skupStanja; }
		}


		public override string ToString() {
			String s = "STANJA\n";
			foreach(LrStavka l in _skupStanja){
				s += l.ToString() + "\n";
			}
	
			s += "PRIJELAZI\n";
			foreach (KeyValuePair<int, List<PrijelazENKA>> entry in _prijelazi) {
				s += "KLJUC: " + entry.Key + "   VRIJEDNOSTI:\n" ;
				foreach(PrijelazENKA pri in entry.Value){
					s += pri + "\n";
				}
			}
			s += " EPSILON PRIJELAZI\n";
			foreach (KeyValuePair<int, List<PrijelazENKA>> entry in _epsilonPrijelaz) {
				s += "KLJUC: " + entry.Key + "   VRIJEDNOSTI:\n";
				foreach (PrijelazENKA pri in entry.Value) {
					s += pri + "\n";
				}
			}
			return s;

		}

	
	}

}

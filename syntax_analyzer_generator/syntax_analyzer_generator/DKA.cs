using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syntax_analyzer_generator {
	class DKA {


		private List<StanjeDKA> _skupStanja = new List<StanjeDKA>();
		private Dictionary<int, List<PrijelazDKA>> _prijelazi = new Dictionary<int, List<PrijelazDKA>>();
		private StanjeDKA _prihvatljivoStanje;


		public DKA() { 	}


		public void DodajStanje(StanjeDKA stanje) {
			_skupStanja.Add(stanje);
		}


		public void DodajPrihvatljivoStanje(StanjeDKA stanje) {
			_prihvatljivoStanje = stanje;
		}


		public void DodajPrijelaz(StanjeDKA lijevoStanje, StanjeDKA desnoStanje, string znak) {
			try {
				_prijelazi[lijevoStanje.RedniBroj].Add(new PrijelazDKA(lijevoStanje, desnoStanje, znak));
			}
			catch (KeyNotFoundException) {
				_prijelazi.Add(lijevoStanje.RedniBroj, new List<PrijelazDKA> { new PrijelazDKA(lijevoStanje, desnoStanje, znak) });
			}
		}


		/// <summary>
		/// Metoda vraća sve prijleze za zadano stanje.
		/// Ako ne postoji niti jedan prijelaz za zadanu stavku metoda vraća null;
		/// </summary>
		/// <param name="stanje"></param>
		/// <returns></returns>
		public List<PrijelazDKA> GetListaPrijlaza(StanjeDKA stanje) {
			try {
				return _prijelazi[stanje.RedniBroj];
			}
			catch (KeyNotFoundException) {
				return new List<PrijelazDKA>();
			}
		}


		public List<StanjeDKA> SkupStanja {
			get { return _skupStanja; }
		}


		public StanjeDKA PrihvatljivoStanje {
			get { return _prihvatljivoStanje; }
			set { _prihvatljivoStanje = value; }
		}


	}
}

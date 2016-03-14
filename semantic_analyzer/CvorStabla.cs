using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semantic_analyzer
{
    class CvorStabla
    {
        
		private string _znak;
		private bool _zavrsni;
		private bool _epsilon;
		private int _rbRedka;
		private string _regEx;
		private int _stanje;
		private List<CvorStabla> _listaCvorovaDjece;

		// Konstruktor CvorStabla 
		// Parametri: znak - puni naziv zavrsnog ili nezavrsnog znaka
		// 		 	  zavrsni - ako je znak zavrsni znak onda true, suprotno false
		//		      epsilon - je li znak epsilon prijelaz
		//			 	 ako je znak zavrsni i nije epsilon potrebno je unijeti jos 3 parametra
		//						rbRedka - broj retka u kom se nalazi nezavrsni znak
		//						regEx - regularni izraz
		//						stanje - broj stanja
		public CvorStabla(string znak, bool zavrsni, bool epsilon, int rbRedka, string regEx, int stanje) {
			_znak = znak;
			_zavrsni = zavrsni;
			_epsilon = epsilon;
			
			if (zavrsni && !epsilon) {
				_rbRedka = rbRedka;
				_regEx = regEx;
				_stanje = stanje;
			}
			else {
				_rbRedka = 0;
				_regEx = "";
				_stanje = 0;
			}

            _listaCvorovaDjece = new List<CvorStabla>();
		}

		public CvorStabla(string znak, bool zavrsni, bool epsilon, int rbRedka, string regEx) {
			_znak = znak;
			_zavrsni = zavrsni;
			_epsilon = epsilon;

			if (zavrsni && !epsilon) {
				_rbRedka = rbRedka;
				_regEx = regEx;
			}
			else {
				_rbRedka = 0;
				_regEx = "";
			}

			_listaCvorovaDjece = new List<CvorStabla>();
		}

        // Dodatan seter za stanje. Potreban za stavi akciju.
        public void SetStanje(int stanje) {
            _stanje = stanje;
        }

		// Metoda dodaje novo djete trenutnom cvoru		
		public void DodajDjete(CvorStabla cvor) {
			_listaCvorovaDjece.Add(cvor);
		}
		
		// Getter naziva znaka	
		public string GetZnak() {
			return _znak;
		}
		
		// Metoda vraca ima li znak redni broj i regEx
		public bool ZavrsniZnak() {
			if(_zavrsni && !_epsilon){
				return true;
			}
			
			return false;
		}
		
		// Getter rednog broja znaka	
		public int GetRbRedka() {
			return _rbRedka;
		}
		
		// Getter regularnog izraza znaka	
		public string GetRegEx() {
			return _regEx;
		}	
		
		
		// Getter liste djece	
		public List<CvorStabla> GetDjeca() {
			return _listaCvorovaDjece;
		}
		
		// Getter stanja znaka	
		public int GetStanje() {
			return _stanje;
		}


		public string Znak {
			get { return _znak; }
		}


		public List<CvorStabla> Djeca {
			get { return _listaCvorovaDjece; }
		}


		public string LeksJedinka {
			get { return _regEx; }
		}


		public int RbRedka {
			get { return _rbRedka; }
		}

		public bool IsZavrsniZnak() {
			if (_znak.StartsWith("<") && _znak.EndsWith(">"))
				return false;
			else
				return true;
		}
		
    }
}

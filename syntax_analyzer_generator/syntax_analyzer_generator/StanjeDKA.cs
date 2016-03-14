using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syntax_analyzer_generator {
	class StanjeDKA : IEquatable<StanjeDKA> {

		private HashSet<LrStavka> _stavke = new HashSet<LrStavka>();
		//private List<LrStavka> _stavke = new List<LrStavka>();
		private bool _aktivno;
		private int _redniBroj;


		public bool Equals(StanjeDKA other) {
			//return _stavke.SequenceEqual(other.Stavke);
			return _stavke.SetEquals(other.Stavke);
		}


		public StanjeDKA(int redniBroj, HashSet<LrStavka> stavke) {
			_stavke = stavke;
			_redniBroj = redniBroj;
			_aktivno = false;
		}


		public bool Aktivno {
			get { return _aktivno; }
			set { _aktivno = value; }
		}

		/// <summary>
		/// Geter za redni broj stanja.
		/// </summary>
		public int RedniBroj {
			get { return _redniBroj; }
		}

		
		/*public List<LrStavka> Stavke {
			get { return _stavke; }
		}*/


		public HashSet<LrStavka> Stavke {
			get { return _stavke; }
		}


		public void DodajStavku(LrStavka stavka) {
			_stavke.Add(stavka);
		}

		
		
	}
}

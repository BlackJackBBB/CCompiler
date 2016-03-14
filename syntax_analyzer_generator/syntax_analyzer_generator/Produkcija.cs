using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syntax_analyzer_generator {
	class Produkcija : IEquatable<Produkcija> {


		private string _lijevaStrana;
		private List<string> _desnaStrana;
		private int _rbr;


		public bool Equals(Produkcija other) {
			return _lijevaStrana.Equals(other.LijevaStrana) && _desnaStrana.SequenceEqual(other.DesnaStrana);
		}


		/// <summary>
		/// Stvori produkciju od lijeve i desne strane.
		/// </summary>
		/// <param name="lijevaStrana"></param>
		/// <param name="desnaStrana"></param>
		public Produkcija(string lijevaStrana = "", List<string> desnaStrana = null, int rbr = 1) {
			_lijevaStrana = lijevaStrana;
			_desnaStrana = desnaStrana;
			_rbr = rbr;
		}


		public override string ToString() {
			return _lijevaStrana + "  ->  " + _desnaStrana;
		}


		public List<string> DesnaStrana {
			get { return _desnaStrana; }
			set { _desnaStrana = value; }
		}


		public string LijevaStrana {
			get { return _lijevaStrana; }
			set { _lijevaStrana = value; }
		}

		public int Rbr {
			get { return _rbr; }
			set { _rbr = value; }
		}




	}
}

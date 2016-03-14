using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace syntax_analyzer_generator {

	class PrijelazENKA : IEquatable<PrijelazENKA> {


		private LrStavka _trenutnoStanje;
		private LrStavka _sljedeceStanje;
		private string _znak;


		public bool Equals(PrijelazENKA other) {
			return _trenutnoStanje.Equals(other.TrenutnoStanje) && _sljedeceStanje.Equals(other.SljedeceStanje) && _znak.Equals(other.Znak);
		}


		/// <summary>
		/// Konstruktor stvara prijelaz automata iz trenutnog u sljedece stanje za zadani znak.
		/// </summary>
		/// <param name="lijevoStanje"></param>
		/// <param name="desnoStanje"></param>
		/// <param name="znak"></param>
		public PrijelazENKA(LrStavka trenutnoStanje, LrStavka sljedeceStanje, string znak) {
			_trenutnoStanje = trenutnoStanje;
			_sljedeceStanje = sljedeceStanje;
			_znak = znak;
		}


		/// <summary>
		/// Index trenutnoog stanje automata.
		/// </summary>
		public LrStavka TrenutnoStanje {
			get {
				return _trenutnoStanje;
			}
		}


		/// <summary>
		/// Index sljedeceg stanje automata.
		/// </summary>
		public LrStavka SljedeceStanje {
			get { return _sljedeceStanje; }
		}


		/// <summary>
		/// Znak za koji automat prijlazi iz trenutnog u sljedece stanje.
		/// </summary>
		public String Znak {
			get { return _znak; }
		}


		public override string ToString() {
			return "( "+ _trenutnoStanje + " , " + Znak + " )  ===>  " + _sljedeceStanje;
		}


	}
}

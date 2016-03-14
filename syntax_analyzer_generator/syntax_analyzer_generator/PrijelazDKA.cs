using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syntax_analyzer_generator {
	class PrijelazDKA {


		private StanjeDKA _trenutnoStanje;
		private StanjeDKA _sljedeceStanje;
		private string _znak;


		public PrijelazDKA(StanjeDKA trenutnoStanje, StanjeDKA sljedeceStanje, string znak) {
			_trenutnoStanje = trenutnoStanje;
			_sljedeceStanje = sljedeceStanje;
			_znak = znak;
		}


		public StanjeDKA TrenutnoStanje {
			get { return _trenutnoStanje; }
		}


		public StanjeDKA SljedeceStanje {
			get { return _sljedeceStanje; }
		}


		public string Znak {
			get { return _znak; }
		}

	}
}

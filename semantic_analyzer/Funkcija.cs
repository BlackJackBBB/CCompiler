using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semantic_analyzer {
	class Funkcija : IEquatable<Funkcija> {
		private string ime;
		private string povratniTip;
		private List<string> tipoviArgumenata;
		private List<string> imenaArgumenata;
		bool definicija;
		bool imaArgumenata;


		public Funkcija(string imeFje,string povratniTipFje,List<string>tipoviArg,List<string>imenaArg, bool definicija){
			ime = imeFje;
			povratniTip = povratniTipFje;
			tipoviArgumenata = tipoviArg;
			imenaArgumenata = imenaArg;
			this.definicija = definicija;
			imaArgumenata = tipoviArgumenata.Count != 0;

		}

		public Funkcija(string imeFje, string povratniTipFje, bool definicija) {
			ime = imeFje;
			povratniTip = povratniTipFje;
			tipoviArgumenata = new List<string>();
			imenaArgumenata = new List<string>();
			this.definicija = definicija;
			imaArgumenata = tipoviArgumenata.Count != 0;
		}

		public bool Equals(Funkcija other) {
			if (other == null)
				return false;
			if(this.Ime!=other.Ime) return false;
			if (this.PovratniTip != other.PovratniTip) return false;
			if(!this.TipoviArgumenata.SequenceEqual(other.TipoviArgumenata)) return false;
				
				return false;
		}


		public string Ime{
			get { return ime; }
		}

		public string PovratniTip {
			get { return povratniTip; }
		}

		public bool Definicija {
			get { return definicija; }
			set { definicija = value; }
		}

		public List<string> TipoviArgumenata {
			get { return tipoviArgumenata; }
		}

		public List<string> ImenaArgumenata {
			get { return imenaArgumenata; }
		}


	}
}

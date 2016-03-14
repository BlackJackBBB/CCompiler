using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syntax_analyzer_generator {
	class LrStavka : IEquatable<LrStavka> {


		private int _redniBroj;
		private int _prioritet;
		private string _lijevaStrana;
		private List<string> _desnaStrana;
		private List<string> _podskupSlijedi;
		private int _pozicijaTocke;
		private bool _aktivna;


		public bool Equals(LrStavka other) {
			return //_redniBroj == other.RedniBroj &&
				_lijevaStrana.Equals(other.LijevaStrana) &&
				_desnaStrana.SequenceEqual(other.DesnaStrana) &&
				_pozicijaTocke == other.PozicijaTocke &&
				_podskupSlijedi.SequenceEqual(other.PodskupSlijedi);
		}


		public override int GetHashCode() {
			return _redniBroj;
		}


		public override bool Equals(object obj) {
			if (obj == null)
				return false;

			if (this.GetType() != obj.GetType())
				return false;

			var a = (LrStavka) obj;
			return a.RedniBroj == this.RedniBroj;
		}


		public LrStavka(string lijevaStrana, List<string> desnaStrana, int pozicijaTocke, int redniBroj, List<string> podskupSlijedi, int prioritet) {
			_lijevaStrana = lijevaStrana;
			_desnaStrana = desnaStrana;
			_pozicijaTocke = pozicijaTocke;
			_redniBroj = redniBroj;
			_podskupSlijedi = podskupSlijedi;
			_prioritet = prioritet;
			_aktivna = true;
		}


		public string LijevaStrana {
			get { return _lijevaStrana; }
			set { _lijevaStrana = value; }
		}


		public int Prioritet {
			get { return _prioritet; }
		}


		public List<string> DesnaStrana {
			get { return _desnaStrana; }
		}


		public int RedniBroj {
			get { return _redniBroj; }
		}


		public int PozicijaTocke {
			get { return _pozicijaTocke; }
			set { _pozicijaTocke = value; }
		}


		public bool Aktivna {
			get { return _aktivna; }
			set { _aktivna = value; }
		}


		public List<string> PodskupSlijedi {
			get { return _podskupSlijedi; }
		}


		public void DodajUPodskupSlijedi(string zavrsniZnak) {
			_podskupSlijedi.Add(zavrsniZnak);
		}


		public override string ToString() {
			string s = RedniBroj+") "+_lijevaStrana + "  ->  ";
			foreach (string sa in _desnaStrana) {
				s += sa + " ";
			}
			return s;
		}


		public string ToProdukcijaString() {
			string novaDesnaStrana = "";

			for (int i = 0; i < _desnaStrana.Count; i++) {
				if (i != _pozicijaTocke) novaDesnaStrana += " " + _desnaStrana[i];
			}

			if (novaDesnaStrana.Length == 0)
				return _lijevaStrana + " ->" + " $";
			else
				return _lijevaStrana + " ->" + novaDesnaStrana;
		}


		public Produkcija ToProdukcija(LrStavka stavka) {
			List<string> desnaStranaProdukcije = new List<string>(stavka.DesnaStrana);
			desnaStranaProdukcije.RemoveAt(stavka.PozicijaTocke);
			return new Produkcija(stavka.LijevaStrana, desnaStranaProdukcije);
		}
	
	}
}

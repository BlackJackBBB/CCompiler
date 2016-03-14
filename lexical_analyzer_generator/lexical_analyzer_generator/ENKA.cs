using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace lexical_analyzer {

    [Serializable()]
    class ENKA : ISerializable {

        private List<Stanje> _skupStanja = new List<Stanje>();
        private int _pocetnoStanje;
        private int _prihvatljivoStanje;
        private List<Prijelaz> _prijelazi = new List<Prijelaz>();
		private List<Prijelaz> _epsilonPrijelaz = new List<Prijelaz>();


        public ENKA() { }


        public ENKA(SerializationInfo info, StreamingContext ctxt)
        {

            this._skupStanja = (List<Stanje>)info.GetValue("skupStanja", typeof(List<Stanje>));
            this._pocetnoStanje = (int)info.GetValue("pocetnoStanje", typeof(int));
            this._prihvatljivoStanje = (int)info.GetValue("prihvatljivoStanje", typeof(int));
            this._prijelazi = (List<Prijelaz>)info.GetValue("prijelazi", typeof(List<Prijelaz>));
            this._epsilonPrijelaz = (List<Prijelaz>)info.GetValue("epsilonPrijelaz", typeof(List<Prijelaz>));
        }


        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("skupStanja", _skupStanja);
            info.AddValue("pocetnoStanje", _pocetnoStanje);
            info.AddValue("prihvatljivoStanje", _prihvatljivoStanje);
            info.AddValue("prijelazi", _prijelazi);
            info.AddValue("epsilonPrijelaz", _epsilonPrijelaz);

        }


		/*
		/// <summary>
		/// Konstrukcija automata za zadani regularni izraz.
		/// </summary>
		/// <param name="regIzraz"></param>
		public ENKA(string regIzraz) {
			ENKAGenerator generator = new ENKAGenerator();
			generator.GenerirajAutomat(regIzraz, this);
		}
		*/


		/// <summary>
		/// Pocetno stanje automata.
		/// </summary>
        public int PocetnoStanje {
            get { return _pocetnoStanje; }
            set { _pocetnoStanje = value; }
        }


		/// <summary>
		/// Prihvatljivo stanje automata.
		/// </summary>
        public int PrihvatljivoStanje {
            get { return _prihvatljivoStanje; }
            set { _prihvatljivoStanje = value;}
        }


        /// <summary>
		/// Metoda dodaje stanje u automat i vraća njegovu oznaku.
        /// </summary>
        /// <returns></returns>
        public int DodajStanje() {
            _skupStanja.Add(new Stanje(false));
            return _skupStanja.Count - 1;
        }


		/// <summary>
		/// Metoda dodaje novi prijelaz koji spaja lijevo i desno stanje za zadani znak.
		/// Epsilon prijelaz se označava s '$'.
		/// </summary>
		/// <param name="lijevoStanje"></param>
		/// <param name="desnoStanje"></param>
		/// <param name="znak"></param>
        public void DodajPrijelaz(int lijevoStanje, int desnoStanje, char znak) {
            _prijelazi.Add(new Prijelaz(lijevoStanje, desnoStanje, znak));
        }


		public void DodajEpsilonPrijelaz(int lijevoStanje, int desnoStanje) {
			_epsilonPrijelaz.Add(new Prijelaz(lijevoStanje, desnoStanje, '$'));
		}


		/// <summary>
		/// Lista prijelaza automata sve znakove.
		/// </summary>
		public List<Prijelaz> ListaPrijelaza { 
			get { return _prijelazi; }
		}


		/// <summary>
		/// Lista epsilon prijelaza automata.
		/// </summary>
		public List<Prijelaz> ListaEpsilonPrijelaza {
			get { return _epsilonPrijelaz; }
		}


		/// <summary>
		/// Lista svih stanja automata.
		/// </summary>
		public List<Stanje> ListaStanja {
			get { return _skupStanja; }
		}

    }
}

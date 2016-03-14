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
    class Prijelaz : ISerializable {

        private int _trenutnoStanje;
        private int _sljedeceStanje;
        private char _znak;


		/// <summary>
		/// Konstruktor stvara prijelaz automata iz trenutnog u sljedece stanje za zadani znak.
		/// </summary>
		/// <param name="lijevoStanje"></param>
		/// <param name="desnoStanje"></param>
		/// <param name="znak"></param>
		public Prijelaz(int trenutnoStanje, int sljedeceStanje, char znak) {
			_trenutnoStanje = trenutnoStanje;
			_sljedeceStanje = sljedeceStanje;
			_znak = znak;
		}


        public Prijelaz(SerializationInfo info, StreamingContext ctxt){
            _trenutnoStanje = (int)info.GetValue("trenutnoStanje", typeof(int));
            _sljedeceStanje = (int)info.GetValue("sljedeceStanje", typeof(int));
            _znak = (char)info.GetValue("znak", typeof(char));
        }


        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("trenutnoStanje", _trenutnoStanje);
            info.AddValue("sljedeceStanje", _sljedeceStanje);
            info.AddValue("znak", _znak);
        }


		/// <summary>
		/// Index trenutnoog stanje automata.
		/// </summary>
        public int TrenutnoStanje {
            get {
                return _trenutnoStanje;
            }
        }


		/// <summary>
		/// Index sljedeceg stanje automata.
		/// </summary>
        public int SljedeceStanje {
            get { return _sljedeceStanje; }
        }


		/// <summary>
		/// Znak za koji automat prijlazi iz trenutnog u sljedece stanje.
		/// </summary>
        public char Znak {
			get { return _znak; }
        }

    }
}

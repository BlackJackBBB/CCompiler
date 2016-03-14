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
    class LeksJedinka : ISerializable
    {
		
		private ENKA _automat;
		private string _regIzraz;
		private string _nazivUniformnogZnaka;
		private bool _vratiSe;
		private int _brojZnakovaJedinke;
		private bool _noviRedak;
		private string _nazivStanja;
		private bool _aktivan;
		private int _duljinaProcitanogNiza;


		public LeksJedinka(ENKA automat, string reg, string nazivUniformnog, bool vrati, int brojZnakova, bool redak, string nazivStanj) {
			_automat = automat;
			_regIzraz = reg;
			_nazivUniformnogZnaka = nazivUniformnog;
			_brojZnakovaJedinke = brojZnakova;
			_noviRedak = redak;
			_vratiSe = vrati;
			_nazivStanja = nazivStanj;
			_aktivan = false;
			_duljinaProcitanogNiza = 0;
		}


        public LeksJedinka(SerializationInfo info, StreamingContext ctxt)
        {
            _automat = (ENKA)info.GetValue("automat", typeof(ENKA));
            _regIzraz = (string)info.GetValue("regIzraz", typeof(string));
            _nazivUniformnogZnaka = (string)info.GetValue("nazivUniformnogZnaka", typeof(string));
            _vratiSe = (bool)info.GetValue("vratiSe", typeof(bool));
            _brojZnakovaJedinke = (int)info.GetValue("brojZnakovaJedinke", typeof(int));
            _noviRedak = (bool)info.GetValue("noviRedak", typeof(bool));
            _nazivStanja = (string)info.GetValue("nazivStanja", typeof(string));
            _aktivan = (bool)info.GetValue("aktivan", typeof(bool));
            _duljinaProcitanogNiza = (int)info.GetValue("duljinaProcitanogNiza", typeof(int));
        }


        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("automat", _automat);
            info.AddValue("regIzraz", _regIzraz);
            info.AddValue("nazivUniformnogZnaka",  _nazivUniformnogZnaka);
            info.AddValue("vratiSe",  _vratiSe);
            info.AddValue("brojZnakovaJedinke",  _brojZnakovaJedinke);
            info.AddValue("noviRedak",  _noviRedak);
            info.AddValue("nazivStanja",  _nazivStanja);
            info.AddValue("aktivan",  _aktivan);
            info.AddValue("duljinaProcitanogNiza",  _duljinaProcitanogNiza);
        }

		 
        public ENKA Automat {
            get { return _automat; }
            set { _automat = value; }
        }


        public string RegIzraz {
            get { return _regIzraz; }
            set { _regIzraz = value; }
        }


        public string NazivUniformnogZnaka {
            get { return _nazivUniformnogZnaka; }
            set { _nazivUniformnogZnaka = value; }
        }


        public bool VratiSe {
            get { return _vratiSe; }
            set { _vratiSe = value; }
        }


        public int BrojZnakovaJedinke {
            get { return _brojZnakovaJedinke; }
            set { _brojZnakovaJedinke = value; }
        }


        public bool NoviRedak {
            get { return _noviRedak; }
            set { _noviRedak = value; }
        }


        public string NazivStanja {
            get { return _nazivStanja; }
            set { _nazivStanja = value; }
        }


        public bool Aktivan {
            get { return _aktivan; }
            set { _aktivan = value; }
        }


        public int DuljinaProcitanogNiza {
            get { return _duljinaProcitanogNiza; }
            set { _duljinaProcitanogNiza = value; }
        }

	}

}

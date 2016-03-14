using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace syntax_analyzer
{
    [Serializable()]
    class Prijelaz : ISerializable
    {

        private LrStavka _trenutnoStanje;
        private  LrStavka _sljedeceStanje;
        private String _znak;


        /// <summary>
        /// Konstruktor stvara prijelaz automata iz trenutnog u sljedece stanje za zadani znak.
        /// </summary>
        /// <param name="lijevoStanje"></param>
        /// <param name="desnoStanje"></param>
        /// <param name="znak"></param>
        public Prijelaz(LrStavka trenutnoStanje, LrStavka sljedeceStanje, String znak)
        {
            _trenutnoStanje = trenutnoStanje;
            _sljedeceStanje = sljedeceStanje;
            _znak = znak;
        }


        public Prijelaz(SerializationInfo info, StreamingContext ctxt)
        {
            _trenutnoStanje = (LrStavka)info.GetValue("trenutnoStanje", typeof(LrStavka));
            _sljedeceStanje = (LrStavka)info.GetValue("sljedeceStanje", typeof(LrStavka));
            _znak = (String)info.GetValue("znak", typeof(char));
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
        public LrStavka TrenutnoStanje
        {
            get
            {
                return _trenutnoStanje;
            }
        }


        /// <summary>
        /// Index sljedeceg stanje automata.
        /// </summary>
        public LrStavka SljedeceStanje
        {
            get { return _sljedeceStanje; }
        }


        /// <summary>
        /// Znak za koji automat prijlazi iz trenutnog u sljedece stanje.
        /// </summary>
        public String Znak
        {
            get { return _znak; }
        }


        public override string ToString()
        {
            return _trenutnoStanje + " , " + Znak + "  ->  " + _sljedeceStanje;
        }


    }
}

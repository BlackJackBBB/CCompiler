using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace lexical_analyzer {

    [Serializable()]
    class Stanje : ISerializable {


        private bool _jeAktivno;


        /// <summary>
        /// Konstruktor stvara stanje i označava ga za aktivno ili pasivno.
        /// </summary>
        public Stanje(bool aktivno) {
            _jeAktivno = aktivno;
        }


        public Stanje(SerializationInfo info, StreamingContext ctxt)
        {
            _jeAktivno = (bool)info.GetValue("aktivno", typeof(bool));

        }


        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("aktivno", _jeAktivno);
        }


        public bool JeAktivno {
            get { return _jeAktivno; }
            set { _jeAktivno = value; }
        } 

    }
}

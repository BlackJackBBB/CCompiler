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
    class ENKA : ISerializable
    {

        private List<LrStavka> _skupStanja = new List<LrStavka>();
        private LrStavka _pocetnoStanje;
        private List<Prijelaz> _prijelazi = new List<Prijelaz>();
        private List<Prijelaz> _epsilonPrijelaz = new List<Prijelaz>();


        public ENKA() { }


        public ENKA(SerializationInfo info, StreamingContext ctxt)
        {

            this._skupStanja = (List<LrStavka>)info.GetValue("skupStanja", typeof(List<LrStavka>));
            this._pocetnoStanje = (LrStavka)info.GetValue("pocetnoLrStavka", typeof(LrStavka));
            
            this._prijelazi = (List<Prijelaz>)info.GetValue("prijelazi", typeof(List<Prijelaz>));
            this._epsilonPrijelaz = (List<Prijelaz>)info.GetValue("epsilonPrijelaz", typeof(List<Prijelaz>));
        }


        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("skupStanja", _skupStanja);
            info.AddValue("pocetnoLrStavka", _pocetnoStanje);
           
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
        /// Pocetno LrStavka automata.
        /// </summary>
        public LrStavka PocetnoStanje
        {
            get { return _pocetnoStanje; }
            set { _pocetnoStanje = value; }
        }


       

        /// <summary>
        /// Metoda dodaje LrStavka u automat i vraća njegovu oznaku.
        /// </summary>
        /// <returns></returns>
        public void DodajLrStavka(LrStavka stavka)
        {
            _skupStanja.Add(stavka);
        }

      

        /// <summary>
        /// Metoda dodaje novi prijelaz koji spaja lijevo i desno LrStavka za zadani znak.
        /// Epsilon prijelaz se označava s '$'.
        /// </summary>
        /// <param name="lijevoLrStavka"></param>
        /// <param name="desnoLrStavka"></param>
        /// <param name="znak"></param>
        public void DodajPrijelaz(LrStavka lijevoLrStavka, LrStavka desnoLrStavka, string znak)
        {
            _prijelazi.Add(new Prijelaz(lijevoLrStavka, desnoLrStavka, znak));
        }


        public void DodajEpsilonPrijelaz(LrStavka lijevoLrStavka, LrStavka desnoLrStavka)
        {
            _epsilonPrijelaz.Add(new Prijelaz(lijevoLrStavka, desnoLrStavka, "$"));
        }


        /// <summary>
        /// Lista prijelaza automata sve znakove.
        /// </summary>
        public List<Prijelaz> ListaPrijelaza
        {
            get { return _prijelazi; }
        }


        /// <summary>
        /// Lista epsilon prijelaza automata.
        /// </summary>
        public List<Prijelaz> ListaEpsilonPrijelaza
        {
            get { return _epsilonPrijelaz; }
        }


        /// <summary>
        /// Lista svih stanja automata.
        /// </summary>
        public List<LrStavka> ListaStanja
        {
            get { return _skupStanja; }
            set { _skupStanja=value;}
        }

    }
}

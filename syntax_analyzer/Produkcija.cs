using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syntax_analyzer
{
    class Produkcija
    {
        public string lijevaStrana;
        public string desnaStrana;
        
        public Produkcija(string l, string d)
        {
            lijevaStrana = l;
            desnaStrana = d;//znakovi desne strane ostaju odvojeni razmacima
        }

        public Produkcija()
        {

        }

        public override string ToString()
        {
            return lijevaStrana + "  ->  " + desnaStrana;
        }

       
    }
}

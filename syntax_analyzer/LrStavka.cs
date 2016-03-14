using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syntax_analyzer
{
    class LrStavka
    {
        public String lijevaStrana;
        public String[] desnaStrana;
        public int posTocka;

        /// <summary>
        /// Iz zadane produkcije vraća listu LrStavki
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static List<LrStavka> izProdukcije(Produkcija p)
        {
            List<LrStavka> lista = new List<LrStavka>();
           
            if (p.desnaStrana.Equals("$"))
            {
                LrStavka jedina = new LrStavka();
                jedina.lijevaStrana = p.lijevaStrana;
                jedina.desnaStrana = new String[]{"."};
                jedina.posTocka = 0;
                lista.Add(jedina);
                return lista;
            }
            String[] des = p.desnaStrana.Split(' ');

           


            int brStavki = des.Length + 1;
            for (int i = 0; i < brStavki; i++)
            {
                LrStavka nova = new LrStavka();
                nova.lijevaStrana = p.lijevaStrana;
                
                nova.desnaStrana = new String[brStavki];
                for (int j = 0; j < brStavki; j++)
                {
                    if (j < i) nova.desnaStrana[j] = des[j];
                    if (j == i) { nova.desnaStrana[j] = "."; nova.posTocka = i; }
                    if (j > i) nova.desnaStrana[j] = des[j - 1];
                }
                lista.Add(nova);
                
            }

            return lista;
        }


        public override string ToString()
        {
            String s = lijevaStrana + "  ->  ";
            foreach (string sa in desnaStrana)
            {
                s += sa;
            }
            return s;
        }
    }
}

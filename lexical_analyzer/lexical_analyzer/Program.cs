using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lexical_analyzer
{

    public struct stanja
    {
        // ovdje treba ici niz stanja u koje se prijelazi za pojedine znakove

        public bool aktivan;
    }

    // Struktura svakog NKA
    public struct NKA
    {
        List<stanja> svaStanja;
    }

    // Ovo je struktura svake jedinke
    public struct leksJedinka
    {
        //tu jos treba ici referenca na NKA

        // samo privremeno dok automat ne radi
        public string regIzraz;

        public string nazivUniformnogZnaka;

        public bool vratiSe;
        public int brojZnakovaJedinke;

        public bool noviRedak;

        public string nazivStanja;

        public bool aktivan;
        public int duljinaProcitanogNiza;

        public leksJedinka(string reg, string nazivUniformnog, bool vrati, int brojZnakova, bool redak, string nazivStanj)
        {
            regIzraz = reg;
            nazivUniformnogZnaka = nazivUniformnog;
            brojZnakovaJedinke = brojZnakova;
            noviRedak = redak;
            vratiSe = vrati;
            nazivStanja = nazivStanj;
            aktivan = true;
            duljinaProcitanogNiza = 0;
        }
    }

    class Program
    {



        public static List<String> splitWithoutEscaped(String line, char regex)
        {
            int start = 0, end = line.IndexOf(regex);
            if (line[0] == regex)
            {
                start = 1;

            }
            List<String> lista = new List<string>();
            while (end > 0 && end > start)
            {
                if (line[end - 1] == '\\')
                {
                    end = line.IndexOf(regex, end + 1);
                }
                else
                {
                    lista.Add(line.Substring(start, end - start));
                    start = end + 1;
                    end = line.IndexOf(regex, start);
                }

            }
            if (start < line.Length) lista.Add(line.Substring(start));
            return lista;
        }


        static void Main(string[] args)
        {
            Dictionary<string, List<String>> regeksi = new Dictionary<string, List<string>>();
            // Ovo je dictionary svih stanja na koje su spojene liste s pripadajucim leksJedinkama
            Dictionary<string, List<leksJedinka>> svaStanja = new Dictionary<string, List<leksJedinka>>();


            //citam sve linije
            string[] linijeUlaza = System.IO.File.ReadAllLines("D:/lab1_minusLang.txt");
            //odredjujem redak definicije stanja i identifikatora
            int posStanja = 0;
            int posUnif = 0;
            for (int i = 0; i < linijeUlaza.Length; i++)
            {
                if (linijeUlaza[i].StartsWith("%X"))
                {
                    posStanja = i;
                }
                if (linijeUlaza[i].StartsWith("%L"))
                {
                    posUnif = i;
                    break;
                }
            }

            //parsiram regekse
            for (int i = 0; i < posStanja; i++)
            {
                String linija = linijeUlaza[i];
                String sljedecaLinija = (i < (posStanja - 1) ? linijeUlaza[i + 1] : "");
                if (!sljedecaLinija.StartsWith("{"))
                {
                    linija += sljedecaLinija;
                }
                int drugaZag = linija.IndexOf("}");
                String imeRegexa = linija.Substring(1, drugaZag - 1);
                String vrijednostRegexa = linija.Substring(drugaZag + 2);
                regeksi.Add(imeRegexa, splitWithoutEscaped(vrijednostRegexa, '|'));

            }
            //ispis regeksa (kontrolni)
            foreach (KeyValuePair<string, List<string>> entry in regeksi)
            {
                Console.Write("\n" + entry.Key + ":");
                foreach (string v in entry.Value)
                {
                    Console.Write(v);
                }
            }

            //parsiram stanja

            String[] stanja = linijeUlaza[posStanja].Substring(3).Split(' ');


            //ispis stanja
            Console.Write("\nSTANJA:");
            foreach (string st in stanja)
            {
                Console.Write("\t" + st);
            }

            String[] uniformni = linijeUlaza[posUnif].Substring(3).Split(' ');


            //ispis stanja
            Console.Write("\nUNIFORMNI ZNAKOVI:");
            foreach (string st in uniformni)
            {
                Console.Write(" " + st);
            }
            Console.ReadKey();



            List<leksJedinka> jedinka;
            // Ovdje pocinju leksicke jedinke
            int ii = posUnif;
            // Tu se parsiraju leksicke jedinke i stavljaju u odgovarajucu listu.
            do
            {
                ii++;
                if (linijeUlaza[ii][0] == '<')
                {

                    string regIzraz, nazivUniformongZnaka, nazivStanja = "", nazivTrenutnogStanja;
                    int brojZnakovaJedinke = 0;
                    bool noviRed = false, vratiSe = false;

                    int a = linijeUlaza[ii].IndexOf('>');
                    // Stanje
                    nazivTrenutnogStanja = linijeUlaza[ii].Substring(1, a - 1);

                    // Marceeee :) ovo ti je regularni izraz u jedinki jer mozes kod njega napravit zamjenu s svojim izrazima
                    regIzraz = linijeUlaza[ii].Substring(a + 1);

                    ii += 2;
                    nazivUniformongZnaka = linijeUlaza[ii];

                    do
                    {
                        ii++;
                        if (linijeUlaza[ii][0] == '}')
                        {
                            break;
                        }
                        else if (linijeUlaza[ii].Substring(0, 10).Equals("NOVI_REDAK"))
                        {
                            noviRed = true;
                        }
                        else if (linijeUlaza[ii].Substring(0, 8).Equals("VRATI_SE"))
                        {
                            vratiSe = true;
                            brojZnakovaJedinke = Convert.ToInt32(linijeUlaza[ii].Substring(9));
                        }
                        else if (linijeUlaza[ii].Substring(0, 13).Equals("UDJI_U_STANJE"))
                        {
                            nazivStanja = linijeUlaza[ii].Substring(14);
                        }
                    } while (true);

                    // dodavanje nove jedinke u listu stanja
                    jedinka = svaStanja[nazivTrenutnogStanja];
                    jedinka.Add(new leksJedinka(regIzraz, nazivUniformongZnaka, vratiSe, brojZnakovaJedinke, noviRed, nazivStanja));
                }

            } while (ii < linijeUlaza.Length - 1);
        }
    }

    //treba parsirati Pravila leksickog aanalizatora
    //modelirati razredom Pravilo?
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syntax_analyzer
{
    class DefinicijaSintaksnogAnalizatora
    {
        string[] nezavrsniZnakovi;
        string[] zavrsniZnakovi;
        string[] sinkronizacijskiZnakovi;
        List<Produkcija> produkcije;
        List<String> prazniNezavrsniZnakovi;
        Dictionary<String, List<String>> zapocinje = new Dictionary<string,List<string>>();


        public DefinicijaSintaksnogAnalizatora(String lokacijaDatoteke)
        {
            string[] linijeUlaza = System.IO.File.ReadAllLines(lokacijaDatoteke);

            foreach (String line in linijeUlaza)
            {
                Console.WriteLine(line);
            }

            nezavrsniZnakovi = linijeUlaza[0].Substring(3).Split(' ');
            zavrsniZnakovi = linijeUlaza[1].Substring(3).Split(' ');
            sinkronizacijskiZnakovi = linijeUlaza[2].Substring(5).Split(' ');

            produkcije = new List<Produkcija>();
            prazniNezavrsniZnakovi = new List<string>();
            String lijevaStrana = "";

            for (int i = 3; i < linijeUlaza.Length; i++)
            {
                if (!linijeUlaza[i].StartsWith(" "))
                {
                    lijevaStrana = linijeUlaza[i].Trim();
                }
                else
                {
                    String desnaStrana = linijeUlaza[i].Trim();
                    produkcije.Add(new Produkcija(lijevaStrana, desnaStrana));

                    if (desnaStrana.Equals("$"))
                    { //ako je desna strana $ znači da je znak s lijeve strane produkcije prazan
                        prazniNezavrsniZnakovi.Add(lijevaStrana);
                    }
                }
            }

            String noviNezavrsni = nezavrsniZnakovi[0].Insert(nezavrsniZnakovi[0].Length - 1, "'");
            List<string> temp = nezavrsniZnakovi.ToList();
            temp.Add(noviNezavrsni);
            nezavrsniZnakovi = temp.ToArray();
            //dodan novi početni znak oblika <pocetni'> di je pocetni prvi element u polju nezavrsni znakovi
            //ako je prvi element <A> onda je dodani znak <A'>

            produkcije.Add(new Produkcija(noviNezavrsni, nezavrsniZnakovi[0]));// produkcija pocetni'->pocetni
            foreach (Produkcija p in produkcije)
            {
                Console.WriteLine(p);
            }

            //treba proširiti listu praznih znakova
            //prazan je i onaj koji s desne strane produkcije ima sve prazne znakove;

            List<String> staraLista = new List<string>();
            List<String> novaLista = new List<string>(prazniNezavrsniZnakovi);
            do
            {

                staraLista.Clear(); staraLista.AddRange(novaLista);//nova=stara;
                foreach (Produkcija p in produkcije)
                {
                    if (!staraLista.Contains(p.lijevaStrana))
                    { //ako s lijeve strane već nije prazan znak

                        string[] znakoviDesneStrane = p.desnaStrana.Split(' ');
                        bool noviPrazan = true;
                        foreach (String znak in znakoviDesneStrane)
                        {
                            if (!staraLista.Contains(znak))
                            {
                                noviPrazan = false;
                                break;

                            }
                        }
                        if (noviPrazan == true)
                        {
                            novaLista.Add(p.lijevaStrana);
                            break;
                        }

                    }

                }
            } while (!novaLista.SequenceEqual(staraLista));

            prazniNezavrsniZnakovi = novaLista;

            foreach (Produkcija p in produkcije)
            {
                Console.WriteLine(p);
            }
            foreach (String p in prazniNezavrsniZnakovi)
            {
                Console.WriteLine(p);
            }

            int rows = nezavrsniZnakovi.Length;
            int cols = nezavrsniZnakovi.Length + zavrsniZnakovi.Length;

            String[,] tablicaRelacijeZapocinje = new String[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < nezavrsniZnakovi.Length; j++)
                {
                    if (zapocinjeIzravno(nezavrsniZnakovi[i], nezavrsniZnakovi[j])) { tablicaRelacijeZapocinje[i, j] = "1"; }
                    else tablicaRelacijeZapocinje[i, j] = " ";
                }
                for (int j = nezavrsniZnakovi.Length; j < cols; j++)
                {
                    if (zapocinjeIzravno(nezavrsniZnakovi[i], zavrsniZnakovi[j - nezavrsniZnakovi.Length])) { tablicaRelacijeZapocinje[i, j] = "1"; }
                    else tablicaRelacijeZapocinje[i, j] = " ";
                }
            }


            for (int i = 0; i < rows; i++)
            {
                List<string> nova = new List<string>();
                for (int j = 0; j < nezavrsniZnakovi.Length; j++)
                {
                    if (zapocinjeIzravno(nezavrsniZnakovi[i], nezavrsniZnakovi[j])) nova.Add(nezavrsniZnakovi[j]);

                }
                for (int j = nezavrsniZnakovi.Length; j < cols; j++)
                {
                    if (zapocinjeIzravno(nezavrsniZnakovi[i], zavrsniZnakovi[j - nezavrsniZnakovi.Length]))
                        nova.Add(zavrsniZnakovi[j - nezavrsniZnakovi.Length]);
                }
                zapocinje.Add(nezavrsniZnakovi[i], nova);

              
            }
          
            //u ovom trenutku zapocinje je zapravo IZRAVNO Zapocinje tablica

            foreach (KeyValuePair<string, List<string>> entry in zapocinje)
            {
                zapocinje[entry.Key].Add(entry.Key);
            }

          


            Boolean imaPromjene = false;
            do
            {
                imaPromjene = false;
                foreach (KeyValuePair<string, List<string>> entry in zapocinje)
                {
                    List<String> lista = new List<string>(entry.Value);
                    foreach (String s in lista)
                    {
                        if (!s.Equals(entry.Key) && s.StartsWith("<"))
                        {
                            foreach (String njegov in zapocinje[s])
                            {
                                if (!zapocinje[entry.Key].Contains(njegov))
                                {
                                    zapocinje[entry.Key].Add(njegov);
                                    imaPromjene = true;
                                }
                            }
                        }
                    }
                }


            } while (imaPromjene == true);
          

            //U ovom trenutku zapocinje je ZapocinjeZnakom

            Console.WriteLine();
            foreach (KeyValuePair<string, List<string>> entry in zapocinje)
            {
                Console.Write(entry.Key + " : ");
                foreach (string s in entry.Value)
                {
                    Console.Write(s);
                }
                Console.WriteLine();
              
            }



            foreach (Prijelaz l in DefinicijaSintaksnogAnalizatora.izLrStavke(sveStavke()))
            {
                Console.WriteLine(l);
            }



        }


        /// <summary>
        /// Vraća true ako prvi nezavršni znak počinje izrvano drugim znakom
        /// Vraća true ako postoji produkcija oblika prvi-> drugi nesto nesto
        /// ili prvi-> treci drugi && treci->$
        /// 
        /// </summary>
        /// <param name="prvi"></param>
        /// <param name="drugi"></param>
        /// <returns></returns>
        private Boolean zapocinjeIzravno(String prvi, String drugi)
        {
            foreach (Produkcija p in produkcije)
            {
                if (!p.lijevaStrana.Equals(prvi)) continue;//ne treba gledati te produkcije jer prvi nije s lijeve strane
                if (!p.desnaStrana.Contains(drugi)) continue; //desna strana uopće nema drugo znaka ne gleda se
                if (p.desnaStrana.StartsWith(drugi)) return true; //slucaj kad desna strana pocinje drugim znakom
                //slucaj_ lijeva strana je prvi, desna sadrzi trazeni ali njime ne zapocinje
                //provjeri jesu li ovi ispred prazni znakovi ako jesu vrati true
               int ind= p.desnaStrana.IndexOf(drugi);
               String[] moguciPrazni = p.desnaStrana.Substring(0, ind-1).Split(' ');
               bool je = true;
               foreach (String znak in moguciPrazni)
               {
                   if (!znak.StartsWith("<") || !prazniNezavrsniZnakovi.Contains(znak))
                   {
                       je = false;
                       break;
                   }
               }
                    if(je==true) return true;
                
            }
            return false;
        }


        /// <summary>
        /// Metoda vraća sve LrStavke koje se dobiju iz produkcija.
        /// </summary>
        /// <returns></returns>
        private List<LrStavka> sveStavke()
        {
            List<LrStavka> lista = new List<LrStavka>();
            foreach (Produkcija p in produkcije)
            {
                lista.AddRange(LrStavka.izProdukcije(p));
            }
            return lista;
        }


        public ENKA generirajENKA()
        {
            ENKA enka = new ENKA();
            enka.PocetnoStanje = LrStavka.izProdukcije(produkcije[produkcije.Count - 1])[0];
            enka.ListaStanja = sveStavke();
            //ode triba dodat prijelaze i epsilon prijelaze
            return enka;
        }


        /// <summary>
        /// Za predanu listu lrStavki računa listu Prijelaza tipa: A->.AB,A---->A->A.B (pomicanje točke u desno)
        /// </summary>
        /// <param name="listLr">lista Lrstavki za koje treba naći "prijelaze pomične točke"</param>
        /// <returns>lista jedne vrste prijelaza dobivene iz liste LrStavki</returns>
        private static List<Prijelaz> izLrStavke(List<LrStavka> listLr)
        {
            List<Prijelaz> pri = new List<Prijelaz>();
            foreach (LrStavka lr in listLr)
            {
                if (lr.posTocka == lr.desnaStrana.Length - 1) continue;
                LrStavka nova = new LrStavka();
                nova.lijevaStrana = lr.lijevaStrana;
                nova.posTocka = lr.posTocka + 1;
                String znak = lr.desnaStrana[lr.posTocka + 1];
                nova.desnaStrana = new String[lr.desnaStrana.Length];
                lr.desnaStrana.CopyTo(nova.desnaStrana, 0);
                nova.desnaStrana[lr.posTocka] = znak;
                nova.desnaStrana[nova.posTocka] = ".";
                pri.Add( new Prijelaz(lr, nova, znak));
            }
            return pri;
            }

      
    }
}

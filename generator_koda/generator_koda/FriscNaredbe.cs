using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generator_koda
{
    class FriscNaredbe
    {
        System.IO.StreamWriter _file = new System.IO.StreamWriter("a.frisc");
        string _naredbeNakonReturn = "\r\n";
        int privremenaVarijabla = 0;
        bool uMain = false;
        List<Tuple<string,List<Tuple<string,string>>>> lokalneVarijable = new List<Tuple<string,List<Tuple<string,string>>>>();
        bool r5Zauzet;
		int brojacUvjeta = 0;
		public bool unutarIfa = false;
		public bool invertiraj = false;
	

        public FriscNaredbe()
        {
            string pocetak = " MOVE 40000, R7\r\n CALL F_main\r\n HALT\r\n\r\n";

            _file.Write(pocetak);

        }

        public void definicijaFunkcije(string ime, List<Tuple<string,string>> parametri) {
            _file.Write("F_" + ime);
            lokalneVarijable.RemoveRange(0, lokalneVarijable.Count);

            List<Tuple<string, string>> pomocnaLista = new List<Tuple<string, string>>();
            if (parametri != null)
            {
                foreach (Tuple<string, string> tuple in parametri)
                {
                    pomocnaLista.Add(tuple);
                }
            }
            lokalneVarijable.Add(Tuple.Create(ime, pomocnaLista));

            if (ime.Equals("main")) {
                uMain = true;
            }

        }

        public void pozoviReturn(Varijabla varijabla)
        {
            string redak;

            if (!varijabla.ime.Equals(""))
            {
                if (varijabla.tip.Substring(0, 1).Equals("f"))
                {
         
                   
                }
                else if (varijabla.imeFunkcije.Equals(""))
                {
                    _file.Write(" LOAD R6, (" + varijabla.ime + "_)\r\n");
                }
                else
                {
                    {
                        int i,j=-1;
            
                        foreach (Tuple<string, List<Tuple<string, string>>> pomocni in lokalneVarijable)
                        {
                            if (pomocni.Item1.Equals(varijabla.imeFunkcije))
                            {
                                for (i = 0; i < pomocni.Item2.Count; i++)
                                {
                                    if (pomocni.Item2[i].Item1.Equals(varijabla.ime))
                                    {
                                        j = i;
                                    }
                                }

                                if (j == -1 || pomocni.Item2.Count == 0)
                                {
                                    
                                    _file.Write(" LOAD R6, (" + varijabla.ime + "_)\r\n");
                                }
                                else
                                {
                                    _file.Write(" LOAD R6, (R7 + " + (pomocni.Item2.Count - j - 1) * 4 + ")\r\n");
                                }
                            }
                        }
                    }
                }
 
            }else if(!varijabla.tip.Equals("")){

                if (varijabla.vrijednost > 1000000)
                {
                    _naredbeNakonReturn = _naredbeNakonReturn + "PRIVREMENA" + privremenaVarijabla + " DW %D " + varijabla.vrijednost + "\r\n";
                    redak = " LOAD R6, (PRIVREMENA" + privremenaVarijabla + ")\r\n";
                    _file.Write(redak);
                    privremenaVarijabla++;
                }
                else
                {
                    redak = " MOVE %D " + varijabla.vrijednost + " , R6\r\n";
                    _file.Write(redak);
                }
            }
            else {
                redak = " MOVE R3, R6\r\n";
                _file.Write(redak);
            }

            int b;
            foreach (Tuple<string, List<Tuple<string, string>>> pomocni in lokalneVarijable)
            {
                if (pomocni.Item1.Equals(varijabla.imeFunkcije) || (pomocni.Item1.Equals("main") && varijabla.imeFunkcije.Equals("")))
                {
                    for (b = 0; b < pomocni.Item2.Count; b++)
                    {
                        _file.Write(" POP R1\r\n");
                    }
                }
            }

            redak = " RET\r\n\r\n";
            _file.Write(redak);

         
            if (uMain && unutarIfa==false) {
                upisiNakonReturn();
            }           
            

        }

        public void pozivFunkcije(string ime, List<Varijabla> parametri) {

            if (parametri != null)
            {
                foreach (Varijabla pomocna in parametri)
                {

                    if (!pomocna.ime.Equals(""))
                    {
                        if (pomocna.imeFunkcije.Equals(""))
                        {
                            _file.Write(" LOAD R1, (" + pomocna.ime + "_" + pomocna.imeFunkcije + ")\r\n");
                            _file.Write(" PUSH R1\r\n");
                        }
                        else
                        {
                            int i;

                            foreach (Tuple<string, List<Tuple<string, string>>> pomocni in lokalneVarijable)
                            {
                                if (pomocni.Item1.Equals(pomocna.imeFunkcije))
                                {
                                    for (i = 0; i < pomocni.Item2.Count; i++)
                                    {
                                        if (pomocni.Item2[i].Equals(pomocna.ime))
                                        {
                                            break;
                                        }
                                    }

                                    _file.Write(" LOAD R1, (R7" + (pomocni.Item2.Count - i ) * 4 + ")\r\n");
                                    _file.Write(" PUSH R1\r\n");
                                }
                            }

                           
                        }
                    }

                    else
                    {
                        if (pomocna.vrijednost > 1000000)
                        {
                            _naredbeNakonReturn = _naredbeNakonReturn + "PRIVREMENA" + privremenaVarijabla + " DW %D " + pomocna.vrijednost + "\r\n";
                            _file.Write(" LOAD R1, (PRIVREMENA" + privremenaVarijabla + ")\r\n");
                            _file.Write(" PUSH R1\r\n");
                            privremenaVarijabla++;
                        }
                        else
                        {
                            _file.Write(" MOVE %D " + pomocna.vrijednost + " , R1\r\n");
                            _file.Write(" PUSH R1\r\n");
                        }
                    }
                }
            }
            _file.Write(" MOVE R6,R5\r\n");
            _file.Write(" CALL F_" + ime + "\r\n");


        }

        public void operacija(Varijabla prvi, Varijabla drugi, string operacija)
        {
           
            
            if (!prvi.ime.Equals(""))
            {
                int i;
                bool pronasao = false;
                if (prvi.tip.Length > 3)
                {
                    if (prvi.tip.Substring(0, 3).Equals("fun"))
                    {
                        pronasao = true;
                        if (drugi.tip.Substring(0, 3).Equals("fun"))
                        {
                            _file.Write(" MOVE R5,R1\r\n");
                        }
                        else
                        {
                            _file.Write(" MOVE R6,R1\r\n");
                        }

                    }
                }
                
                if(!pronasao)
                {
                    foreach (Tuple<string, List<Tuple<string, string>>> pomocni in lokalneVarijable)
                    {
                        if (pomocni.Item1.Equals(prvi.imeFunkcije))
                        {
                            for (i = 0; i < pomocni.Item2.Count; i++)
                            {

                                if (pomocni.Item2[i].Item1.Equals(prvi.ime))
                                {
                                    break;
                                }
                            }

                            if (i == pomocni.Item2.Count || pomocni.Item2.Count == 0)
                            {
                                _file.Write(" LOAD R1, (" + prvi.ime + "_)\r\n");
                            }
                            else
                            {
                                _file.Write(" LOAD R1, (R7 + " + (pomocni.Item2.Count - i-1) * 4 + ")\r\n");
                            }
                            break;
                        }
                    }
                }
            }else
            {
                if (prvi.vrijednost > 1000000)
                {
                    _naredbeNakonReturn = _naredbeNakonReturn + "PRIVREMENA" + privremenaVarijabla + " DW %D " + prvi.vrijednost + "\r\n";
                    _file.Write(" LOAD R1, (PRIVREMENA" + privremenaVarijabla + ")\r\n");
                    privremenaVarijabla++;
                }
                else
                {
                    _file.Write(" MOVE %D " + prvi.vrijednost + " , R1\r\n");
                }
            }

            if (!drugi.ime.Equals(""))
            {

                bool pronasao = false;
                if (drugi.tip.Length > 3)
                {
                    if (drugi.tip.Substring(0, 3).Equals("fun"))
                    {
                        pronasao = true;
                        _file.Write(" MOVE R6,R2\r\n");
                    }
                }
                
                if(!pronasao)
                {
                    int i;

                    foreach (Tuple<string, List<Tuple<string, string>>> pomocni in lokalneVarijable)
                    {
                        if (pomocni.Item1.Equals(drugi.imeFunkcije))
                        {
                            for (i = 0; i < pomocni.Item2.Count; i++)
                            {

                                if (pomocni.Item2[i].Item1.Equals(drugi.ime))
                                {
                                    break;
                                }
                            }

                            if (i == pomocni.Item2.Count || pomocni.Item2.Count == 0)
                            {
                                _file.Write(" LOAD R2, (" + drugi.ime + "_)\r\n");
                            }
                            else
                            {
                                _file.Write(" LOAD R2, (R7 + " + (pomocni.Item2.Count - i-1) * 4 + ")\r\n");
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                if (drugi.vrijednost > 1000000)
                {
                    _naredbeNakonReturn = _naredbeNakonReturn + "PRIVREMENA" + privremenaVarijabla + " DW %D " + drugi.vrijednost + "\r\n";
                    _file.Write(" LOAD R2, (PRIVREMENA" + privremenaVarijabla + ")\r\n");
                    privremenaVarijabla++;
                }
                else
                {
                    _file.Write(" MOVE %D " + drugi.vrijednost + " , R2\r\n");
                }
            }

            if (operacija.Equals("MINUS")) {
                _file.Write(" SUB R1, R2, R3\r\n");
            }

            if (operacija.Equals("PLUS"))
            {
                _file.Write(" ADD R1, R2, R3\r\n");
            }

            if (operacija.Equals("OP_BIN_ILI"))
            {
                _file.Write(" OR R1, R2, R3\r\n");
            }

            if (operacija.Equals("OP_BIN_I"))
            {
                _file.Write(" AND R1, R2, R3\r\n");
            }

            if (operacija.Equals("OP_BIN_XILI"))
            {
                _file.Write(" XOR R1, R2, R3\r\n");
            } 
         	if(operacija.Equals("OP_LT")){
				_file.Write(" SUB R1, R2, R3\r\n");
			}
			if (operacija.Equals("OP_GTE")) {
				_file.Write(" SUB R1, R2, R3\r\n");
				invertiraj = true;
			}

        }

        public void deklalirajVarijablu(Varijabla prvi, Varijabla drugi)
        {
            if (prvi.imeFunkcije.Equals(""))
            {
                _naredbeNakonReturn = _naredbeNakonReturn + prvi.ime + "_" + " DW %D " + drugi.vrijednost + "\r\n";
            }
            else
            {

                if (drugi.imeFunkcije.Equals("") && drugi.ime.Equals("") && drugi.vrijednost == 0)
                {
                    _file.Write(" MOVE R3,R1\r\n");
                }
                else if (drugi.imeFunkcije.Equals("") && !drugi.ime.Equals(""))
                {
                    _file.Write(" LOAD R1,(" + drugi.ime + "_)\r\n");
                }
                else if (!drugi.ime.Equals(""))
                {
                    foreach (Tuple<string, List<Tuple<string, string>>> pomocni in lokalneVarijable)
                    {
                        int i;
                        if (pomocni.Item1.Equals(drugi.imeFunkcije))
                        {
                            for (i = 0; i < pomocni.Item2.Count; i++)
                            {

                                if (pomocni.Item2[i].Item1.Equals(drugi.ime))
                                {
                                    break;
                                }
                            }

                            if (i == pomocni.Item2.Count || pomocni.Item2.Count == 0)
                            {
                                _file.Write(" LOAD R1, (" + drugi.ime + "_)\r\n");
                            }
                            else
                            {
                                _file.Write(" LOAD R1, (R7 + " + (pomocni.Item2.Count - i-1) * 4 + ")\r\n");
                            }
                            break;
                        }
                    }
                }
                else
                {

                    if (drugi.vrijednost > 1000000)
                    {
                        _naredbeNakonReturn = _naredbeNakonReturn + "PRIVREMENA" + privremenaVarijabla + " DW %D " + drugi.vrijednost + "\r\n";
                        _file.Write(" LOAD R1, (PRIVREMENA" + privremenaVarijabla + ")\r\n");
                        privremenaVarijabla++;
                    }
                    else
                    {
                        _file.Write(" MOVE %D " + drugi.vrijednost + " , R1\r\n");
                    }
                }



                _file.Write(" PUSH R1\r\n");

                foreach (Tuple<string, List<Tuple<string, string>>> pomocni in lokalneVarijable)
                    if (pomocni.Item1.Equals(prvi.imeFunkcije))
                    {
                        pomocni.Item2.Add(Tuple.Create(prvi.ime, prvi.tip));

                    }

            }
        }

        private void upisiNakonReturn()
        {

            _file.Write(_naredbeNakonReturn);
         _file.Close();

        }

	

		public void uvjetnoGrananjePocima(String rezultatUvjeta,bool invertirajUvjet) {
			unutarIfa = true;
			_naredbeNakonReturn = _naredbeNakonReturn + "BROJACUVJETA" + brojacUvjeta + " DW %D " + rezultatUvjeta + "\r\n";
			_file.Write(" LOAD R1, (BROJACUVJETA" + brojacUvjeta + ")\r\n");

			
				_file.Write(" CMP R1,0\r\n");
				if (!invertirajUvjet) {
					_file.Write(" JP_EQ SKOCI_" + brojacUvjeta + "\r\n");
				} else {
					_file.Write(" JP_NE SKOCI_" + brojacUvjeta + "\r\n");
				}
			
			brojacUvjeta++;
	
		
		}

		public void uvjetnoGrananjePocima(bool invertirajUvjet) {
			unutarIfa = true;

			if (!invertirajUvjet ) {
				if (invertiraj) {
					_file.Write(" JP_N SKOCI_" + brojacUvjeta + "\r\n");
				} else {
					_file.Write(" JP_NN SKOCI_" + brojacUvjeta + "\r\n");
				}
				
			} else {
				if (invertiraj) {
					_file.Write(" JP_N SKOCI_" + brojacUvjeta + "\r\n");
				} else {
					_file.Write(" JP_NN SKOCI_" + brojacUvjeta + "\r\n");
				}
				
			}

			
			brojacUvjeta++;
		}

		public void uvjetnoGrananjeZavrseno() {
			unutarIfa = false;
			brojacUvjeta--;
			_file.Write("SKOCI_" + brojacUvjeta + "\r\n");
			brojacUvjeta++;


		}

       // public int aditivniIzraz(primIzraz prvi, primIzraz drugi) {

            
    }
}


using generator_koda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generetor_koda {
	class SmantickaPravila {


		Okruzenje _okruzenje;
        List<Varijabla> varijable = new List<Varijabla>();
        List<string> zadnjaNaredba = new List<string>();
		FriscNaredbe naredbe = new FriscNaredbe();


        string funkcija = "";
        bool unarni_minus = false;

		bool obradujemoPolje = false;

		bool racunaSeUvjet = false;

		List<String> rezultatUvjeta = new List<string>();


		public SmantickaPravila(Okruzenje okruzenje) {
			_okruzenje = okruzenje;
		}

		/// <summary>
		/// Metoda pronalazi i sprema u listu zadani broj sljedecih cvorova u preoreder obilasku.
		/// </summary>
		/// <param name="cvor"></param>
		/// <param name="ispis"></param>
		/// <param name="brojSljedecihCvorova"></param>
		private void preOrder(CvorStabla cvor, List<CvorStabla> ispis, int brojSljedecihCvorova) {
			if (brojSljedecihCvorova == 0)
				return;

			for (int i = 0; i < cvor.Djeca.Count; i++) {
				ispis.Add(cvor.Djeca[i]);
				preOrder(cvor.Djeca[i], ispis, --brojSljedecihCvorova);

				if (brojSljedecihCvorova == 0)
					return;
			}
		}


		private string zavrsniZnakToIspisString(CvorStabla cvor) {
			return cvor.Znak + "(" + cvor.RbRedka + "," + cvor.LeksJedinka + ")";
		}


		private void ispisGresku(CvorStabla cvor) {
			Console.Write("{0} ::=", cvor.Znak);
			foreach (CvorStabla djete in cvor.Djeca)
				if (djete.IsZavrsniZnak())
					Console.Write(" {0}", zavrsniZnakToIspisString(djete));
				else
					Console.Write(" {0}", djete.Znak);
			Console.WriteLine();
			System.Environment.Exit(0);
		}


		/// <summary>
		/// Pronalazi dali se generira kroz nisz jedničnih produkcija =>* NIZ_ZNAKOVA.
		/// Ako se generira vraca se duljina od NIZ_ZNAKOVA.ime inace se vraca -1.
		/// </summary>
		/// <param name="cvor"></param>
		/// <returns></returns>
		private int seJedinicnoGeneriraNizZnakova(CvorStabla cvor) {
			if (cvor.Djeca.Count == 1)
				if (cvor.Djeca[0].Znak == "NIZ_ZNAKOVA")
					return cvor.Djeca[0].LeksJedinka.Length - 2;
				else
					return seJedinicnoGeneriraNizZnakova(cvor.Djeca[0]);
			else 
				return -1;
		}


		///////////////////////////////////////////////////////////// Izrazi ///////////////////////////////////////////////////////////////


		public void PrimarniIzraz(CvorStabla korijen, ref string tip, ref bool lIzraz) {

			if (korijen.Djeca[0].Znak == "IDN") {
				
			

				if (!_okruzenje.jeDeklarirano(korijen.Djeca[0].LeksJedinka))
					ispisGresku(korijen);

				tip = _okruzenje.getTip(korijen.Djeca[0].LeksJedinka);


				if (Provjere.isT(tip))
					lIzraz = true;
				else
					lIzraz = false;
				//spremam ime identifikatora
                //Console.WriteLine(funkcija + " as " + korijen.Djeca[0].LeksJedinka);
                varijable.Add(new Varijabla(tip, korijen.Djeca[0].LeksJedinka, 0, funkcija));
				

			}
			else if (korijen.Djeca[0].Znak == "BROJ") {
				
				tip = "int";
				lIzraz = false;

				

				try {
					int i = Convert.ToInt32(korijen.Djeca[0].LeksJedinka);

                    varijable.Add(new Varijabla("int", "",(unarni_minus)?(-i):(i), funkcija));
                    unarni_minus = false;
					if (racunaSeUvjet) {
						rezultatUvjeta.Add(korijen.Djeca[0].LeksJedinka);
					}
				
		
				}
				catch (FormatException) {
					ispisGresku(korijen);
				}
				catch (OverflowException) {
					ispisGresku(korijen);
				}
				

			}
			else if (korijen.Djeca[0].Znak == "ZNAK") {
				tip = "char";
				lIzraz = false;

				string s = korijen.Djeca[0].LeksJedinka;


				if (s != "'\n'" && s != "'\t'" && s != "'\0'" && s != "'\'" && s != "'\"'" && s != "'\\'") {
					string ss = s.Substring(1, s.Length - 2);
					if (ss.Length >= 2)
						Console.WriteLine("{0} ::= {1}({2},{3})", korijen.Znak, korijen.Djeca[0].Znak, korijen.Djeca[0].RbRedka, korijen.Djeca[0].LeksJedinka);
				    varijable.Add(new Varijabla("char", "",ss[0] , funkcija));
                }

			}
			else if (korijen.Djeca[0].Znak == "NIZ_ZNAKOVA") {
				tip = "niz(const(char))";
				lIzraz = false;

				string s = korijen.Djeca[0].LeksJedinka.Substring(1, korijen.Djeca[0].LeksJedinka.Length - 2);
				for (int i = 0; i < s.Length - 1; i++) {
					if (s[i] == '\\') {
						int brojac = 1;
						for (int j = i + 1; j < s.Length; j++) {
							if (s[j] == '\\')
								brojac++;
							else
								break;
						}

						if (brojac % 2 != 0 &&
							s[i + brojac] != 'n' &&
							s[i + brojac] != 't' &&
							s[i + brojac] != '0' &&
							s[i + brojac] != '\'' &&
							s[i + brojac] != '"' &&
							s[i + brojac] != '\\')
							ispisGresku(korijen);

						i += brojac;
					}
				}
			}
			else if (korijen.Djeca[0].Znak == "L_ZAGRADA" && 
				korijen.Djeca[1].Znak == "<izraz>" && 
				korijen.Djeca[2].Znak == "D_ZAGRADA") {
				Izraz(korijen.Djeca[1], ref tip, ref lIzraz);
			}
			else {
				ispisGresku(korijen);
			}

		}


		public void PostfiksIzraz(CvorStabla korijen, ref string tip, ref bool lIzraz) {

			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<primarni_izraz>") {
				PrimarniIzraz(korijen.Djeca[0], ref tip, ref lIzraz);
			} 
			else if (korijen.Djeca.Count == 4 && 
				korijen.Djeca[0].Znak == "<postfiks_izraz>" && 
				korijen.Djeca[1].Znak == "L_UGL_ZAGRADA" &&
				korijen.Djeca[2].Znak == "<izraz>" && 
				korijen.Djeca[3].Znak == "D_UGL_ZAGRADA") {

				

				//1.
				PostfiksIzraz(korijen.Djeca[0], ref tip, ref lIzraz);

				string tip2 = "";
				//2. Ako tip nije niz(X)
				if (!Provjere.isNiz(tip))
					ispisGresku(korijen);
				else {

					//tip <-- X
					switch (tip) {
						case "niz(int)":
							tip2 = "int";
							break;
						case "niz(char)":
							tip2 = "char";
							break;
						case "niz(const(int))":
							tip2 = "const(int)";
							break;
						case "niz(const(char))":
							tip2 = "const(char)";
							break;
						default:
							break;
					}

					//3.
					Izraz(korijen.Djeca[2], ref tip, ref lIzraz);

					//4. Provjerava se Izraz.tip ~ int
					if (!Provjere.isImplictCastable(tip, "int")) 
						ispisGresku(korijen);


					//lIzraz <-- X =/= const(T)
					if (tip != "const(int)" && tip != "const(char)")
						lIzraz = true;
					else
						lIzraz = false;

					tip = tip2;

				}
			} 
			else if (korijen.Djeca.Count == 3 && 
				korijen.Djeca[0].Znak == "<postfiks_izraz>" && 
				korijen.Djeca[1].Znak == "L_ZAGRADA" && 
				korijen.Djeca[2].Znak == "D_ZAGRADA") {

				PostfiksIzraz(korijen.Djeca[0], ref tip, ref lIzraz);

				if (tip.StartsWith("funkcija")) {
					List<string> parametri =new List<string>();
					string pov = "";
					Provjere.rastaviFunkciju(tip, ref parametri, ref pov);

					if (parametri.Count != 0)
 						ispisGresku(korijen);

					tip = pov;
				}

                naredbe.pozivFunkcije(varijable[varijable.Count - 1].ime, null);
                funkcija = varijable[varijable.Count - 1].ime;


				lIzraz = false;
			} 
			else if (korijen.Djeca.Count == 4 && 
				korijen.Djeca[0].Znak == "<postfiks_izraz>" &&
				korijen.Djeca[1].Znak == "L_ZAGRADA" && 
				korijen.Djeca[2].Znak == "<lista_argumenata>" &&
				korijen.Djeca[3].Znak == "D_ZAGRADA") {

				List<string> tipovi = new List<string>();	
				PostfiksIzraz(korijen.Djeca[0], ref tip, ref lIzraz);
				ListaArgumenata(korijen.Djeca[2], ref tipovi);

				if (tip.StartsWith("funkcija")) {
					List<string> parametri = new List<string>();
					string pov = "";
					Provjere.rastaviFunkciju(tip, ref parametri, ref pov);

					if (parametri.Count == 0 || parametri.Count != tipovi.Count)
						ispisGresku(korijen);

					for (int i = 0; i < parametri.Count; i++) 
						if (!Provjere.isImplictCastable(tipovi[i], parametri[i]))
							ispisGresku(korijen);

                   
					tip = pov;
				}
               

				lIzraz = false;
                List<Varijabla> pomocnaLista = new List<Varijabla>();
                for (int i = 0; i < tipovi.Count; i++) {
                    varijable[varijable.Count - tipovi.Count + i].imeFunkcije = varijable[varijable.Count - 1 - tipovi.Count].ime;
          
                    pomocnaLista.Add(varijable[varijable.Count - tipovi.Count + i]);
                }

                naredbe.pozivFunkcije(varijable[varijable.Count - 1 - tipovi.Count].ime,pomocnaLista);
                for (int i = 0; i < tipovi.Count; i++)
                {
                    varijable.RemoveAt(varijable.Count - 1);
                }
                funkcija = varijable[varijable.Count - 1].ime;

			} 
			else if (korijen.Djeca.Count == 2 && 
				korijen.Djeca[0].Znak == "<postfiks_izraz>" && 
				(korijen.Djeca[1].Znak == "OP_INC" || korijen.Djeca[1].Znak == "OP_DEC")) {

				PostfiksIzraz(korijen.Djeca[0], ref tip, ref lIzraz);
				//2. ako je izraz razlicit od 1 i ako se tip ne moze implicitno promijenit u int
				if (lIzraz == false || !Provjere.isImplictCastable(tip, "int")) 
					ispisGresku(korijen);
				
				tip = "int";
				lIzraz = false;
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void ListaArgumenata(CvorStabla korijen, ref List<string> tipovi) {
			if (korijen.Djeca.Count == 1 && 
				korijen.Djeca[0].Znak == "<izraz_pridruzivanja>") {

				string tip = "";
				bool lIzraz = false;
				IzrazPridruzivanja(korijen.Djeca[0], ref tip, ref lIzraz);
				tipovi.Add(tip);
			}
			else if (korijen.Djeca.Count == 3 &&
				korijen.Djeca[0].Znak == "<lista_argumenata>" && 
				korijen.Djeca[1].Znak == "ZAREZ" && 
				korijen.Djeca[2].Znak == "<izraz_pridruzivanja>") {

				string tip = "";
				bool lIzraz = false;
				ListaArgumenata(korijen.Djeca[0], ref tipovi);
				IzrazPridruzivanja(korijen.Djeca[2], ref tip, ref lIzraz);

				tipovi.Add(tip);
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void UnarniIzraz(CvorStabla korijen, ref string tip, ref bool lIzraz) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<postfiks_izraz>") {
				PostfiksIzraz(korijen.Djeca[0], ref tip, ref lIzraz);
			}
			else if (korijen.Djeca.Count == 2 &&
				(korijen.Djeca[0].Znak == "OP_INC" || korijen.Djeca[0].Znak == "OP_DEC") && 
				korijen.Djeca[1].Znak == "<unarni_izraz>") {

				UnarniIzraz(korijen.Djeca[1], ref tip, ref lIzraz);

				if (lIzraz == false || !Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);

				tip = "int";
				lIzraz = false;

			}
			else if (korijen.Djeca.Count == 2 && 
				korijen.Djeca[0].Znak == "<unarni_operator>" && 
				korijen.Djeca[1].Znak == "<cast_izraz>") {

                unarni_minus = true;
				CastIzraz(korijen.Djeca[1], ref tip, ref lIzraz);

				if (!Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);

				tip = "int";
				lIzraz = false;
            }
			else {
				ispisGresku(korijen);
			}
		}


		public void CastIzraz(CvorStabla korijen, ref string tip, ref bool lIzraz) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<unarni_izraz>") {
				UnarniIzraz(korijen.Djeca[0], ref tip, ref lIzraz);
			} 
			else if (korijen.Djeca.Count == 4 && 
				korijen.Djeca[0].Znak == "L_ZAGRADA" && 
				korijen.Djeca[1].Znak == "<ime_tipa>" && 
				korijen.Djeca[2].Znak == "D_ZAGRADA" && 
				korijen.Djeca[3].Znak == "<cast_izraz>") {

				ImeTipa(korijen.Djeca[1], ref tip);
				string imeTipa_tip = tip;

				CastIzraz(korijen.Djeca[3], ref tip, ref lIzraz);

				if (!(Provjere.isImplictCastable(tip, imeTipa_tip) || Provjere.isExplicitCastable(tip, imeTipa_tip)) || Provjere.isNiz(tip)) 
					ispisGresku(korijen);
				
				lIzraz = false;
				tip = imeTipa_tip;

			}
			else {
				ispisGresku(korijen);
			}
		}


		public void ImeTipa(CvorStabla korijen, ref string tip) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<specifikator_tipa>") {
				SpecifikatorTipa(korijen.Djeca[0], ref tip);
			}
			else if (korijen.Djeca.Count == 2 && 
				korijen.Djeca[0].Znak == "KR_CONST" && 
				korijen.Djeca[1].Znak == "<specifikator_tipa>") {

				SpecifikatorTipa(korijen.Djeca[1], ref tip);
				if (tip == "void")
					ispisGresku(korijen);
				
				tip = "const(" + tip + ")";
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void SpecifikatorTipa(CvorStabla korijen, ref String tip) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "KR_VOID") 
				tip = "void";
			else if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "KR_CHAR") 
				tip = "char";
			else if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "KR_INT") 
				tip = "int";
			else
				ispisGresku(korijen);
		}


		public void MultiplikativniIzraz(CvorStabla korijen, ref String tip, ref bool lIzraz) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<cast_izraz>") {
				CastIzraz(korijen.Djeca[0], ref tip, ref lIzraz);
			}
			else if (korijen.Djeca.Count == 3 &&
				korijen.Djeca[0].Znak == "<multiplikativni_izraz>" && 
				(korijen.Djeca[1].Znak == "OP_PUTA" || korijen.Djeca[1].Znak == "OP_DIJELI" || korijen.Djeca[1].Znak == "OP_MOD") &&
				korijen.Djeca[2].Znak == "<cast_izraz>" ) {

				MultiplikativniIzraz(korijen.Djeca[0], ref tip, ref lIzraz);

				if (!Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);
				
				CastIzraz(korijen.Djeca[2], ref tip, ref lIzraz);

				if (!Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);

				tip = "int";
				lIzraz = false;
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void AditivniIzraz(CvorStabla korijen, ref string tip, ref bool lIzraz) {
			zadnjaNaredba.Add("aditivni_izraz");
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<multiplikativni_izraz>") {
				MultiplikativniIzraz(korijen.Djeca[0], ref tip, ref lIzraz);
			}
			else if (korijen.Djeca.Count == 3 && 
				korijen.Djeca[0].Znak == "<aditivni_izraz>" && 
				(korijen.Djeca[1].Znak == "PLUS" || korijen.Djeca[1].Znak == "MINUS") && 
				korijen.Djeca[2].Znak == "<multiplikativni_izraz>") {

				AditivniIzraz(korijen.Djeca[0], ref tip, ref lIzraz);

				if (!Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);

				MultiplikativniIzraz(korijen.Djeca[2], ref tip, ref lIzraz);

				if (!Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);

				tip = "int";
				lIzraz = false;

                naredbe.operacija(varijable[varijable.Count - 2], varijable[varijable.Count - 1], korijen.Djeca[1].Znak);
                varijable.RemoveAt(varijable.Count - 1);
                varijable.RemoveAt(varijable.Count - 1);
                varijable.Add(new Varijabla("", "", 0, ""));
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void OdnosniIzraz(CvorStabla korijen, ref string tip, ref bool lIzraz) {
	
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<aditivni_izraz>") {
				AditivniIzraz(korijen.Djeca[0], ref tip, ref lIzraz);
			}
			else if (korijen.Djeca.Count == 3 && 
				korijen.Djeca[0].Znak == "<odnosni_izraz>" && 
				(korijen.Djeca[1].Znak == "OP_LT" || korijen.Djeca[1].Znak == "OP_GT" || korijen.Djeca[2].Znak == "OP_LTE" ||
				korijen.Djeca[1].Znak == "OP_GTE") && 
				korijen.Djeca[2].Znak == "<aditivni_izraz>") {

				OdnosniIzraz(korijen.Djeca[0], ref tip, ref lIzraz);

				if (!Provjere.isImplictCastable(tip, "int")) 
					ispisGresku(korijen);
				
				AditivniIzraz(korijen.Djeca[2], ref tip, ref lIzraz);

				if (!Provjere.isImplictCastable(tip, "int")) 
					ispisGresku(korijen);
				
				tip = "int";
				lIzraz = false;

				naredbe.operacija(varijable[varijable.Count - 2], varijable[varijable.Count - 1], korijen.Djeca[1].Znak);
				varijable.RemoveAt(varijable.Count - 1);
				varijable.RemoveAt(varijable.Count - 1);
				varijable.Add(new Varijabla("", "", 0, ""));
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void JednakosniIzraz(CvorStabla korijen, ref string tip, ref bool lIzraz) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<odnosni_izraz>") {
				OdnosniIzraz(korijen.Djeca[0], ref tip, ref lIzraz);
			}
			else if (korijen.Djeca.Count == 3 &&
				korijen.Djeca[0].Znak == "<jednakosni_izraz>" && 
				(korijen.Djeca[1].Znak == "OP_EQ" || korijen.Djeca[1].Znak == "OP_NEQ") && 
				korijen.Djeca[2].Znak == "<odnosni_izraz>") {
				
				JednakosniIzraz(korijen.Djeca[0], ref tip, ref lIzraz);

				if (!Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);

				OdnosniIzraz(korijen.Djeca[2], ref tip, ref lIzraz);

				if (!Provjere.isImplictCastable(tip, "int")) 
					ispisGresku(korijen);

				tip = "int";
				lIzraz = false;
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void BinIIzraz(CvorStabla korijen, ref string tip, ref bool lIzraz) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<jednakosni_izraz>") {
				JednakosniIzraz(korijen.Djeca[0], ref tip, ref lIzraz);
			}
			else if (korijen.Djeca.Count == 3 &&
				korijen.Djeca[0].Znak == "<bin_i_izraz>" && 
				korijen.Djeca[1].Znak == "OP_BIN_I" && 
				korijen.Djeca[2].Znak == "<jednakosni_izraz>") {

				BinIIzraz(korijen.Djeca[0], ref tip, ref lIzraz);

				if (!Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);

				JednakosniIzraz(korijen.Djeca[2], ref tip, ref lIzraz);
				
				if (!Provjere.isImplictCastable(tip, "int")) 
					ispisGresku(korijen);
				
				tip = "int";
				lIzraz = false;

                naredbe.operacija(varijable[varijable.Count - 2], varijable[varijable.Count - 1], korijen.Djeca[1].Znak);
                varijable.RemoveAt(varijable.Count - 1);
                varijable.RemoveAt(varijable.Count - 1);
                varijable.Add(new Varijabla("", "", 0, ""));
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void BinXiliIzraz(CvorStabla korijen, ref string tip, ref bool lIzraz) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<bin_i_izraz>") {
				BinIIzraz(korijen.Djeca[0], ref tip, ref lIzraz);
			}
			else if (korijen.Djeca.Count == 3 && 
				korijen.Djeca[0].Znak == "<bin_xili_izraz>" && 
				korijen.Djeca[1].Znak == "OP_BIN_XILI" && 
				korijen.Djeca[2].Znak == "<bin_i_izraz>") {
				
				BinXiliIzraz(korijen.Djeca[0], ref tip, ref lIzraz);

				if (!Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);
				
				BinIIzraz(korijen.Djeca[2], ref tip, ref lIzraz);

				if (!Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);

				tip = "int";
				lIzraz = false;

                naredbe.operacija(varijable[varijable.Count - 2], varijable[varijable.Count - 1], korijen.Djeca[1].Znak);
                varijable.RemoveAt(varijable.Count - 1);
                varijable.RemoveAt(varijable.Count - 1);
                varijable.Add(new Varijabla("", "", 0, ""));
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void BinIliIzraz(CvorStabla korijen, ref string tip, ref bool lIzraz) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<bin_xili_izraz>") {
				BinXiliIzraz(korijen.Djeca[0], ref tip, ref lIzraz);
			}
			else if (korijen.Djeca.Count == 3 &&
				korijen.Djeca[0].Znak == "<bin_ili_izraz>" && 
				korijen.Djeca[1].Znak == "OP_BIN_ILI" && 
				korijen.Djeca[2].Znak == "<bin_xili_izraz>") {

				BinIliIzraz(korijen.Djeca[0], ref tip, ref lIzraz);

				if (!Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);

				BinXiliIzraz(korijen.Djeca[2], ref tip, ref lIzraz);

				if (!Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);

                naredbe.operacija(varijable[varijable.Count - 2], varijable[varijable.Count - 1], korijen.Djeca[1].Znak);
                varijable.RemoveAt(varijable.Count - 1);
                varijable.RemoveAt(varijable.Count - 1);
                varijable.Add(new Varijabla("", "", 0, ""));

				tip = "int";
				lIzraz = false;
			}
			else {
				ispisGresku(korijen);
			}

		}


		public void LogIIzraz(CvorStabla korijen, ref string tip, ref bool lIzraz) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<bin_ili_izraz>") {
				BinIliIzraz(korijen.Djeca[0], ref tip, ref lIzraz);
			}
			else if (korijen.Djeca.Count == 3 &&
				korijen.Djeca[0].Znak == "<log_i_izraz>" && 
				korijen.Djeca[1].Znak == "OP_I" && 
				korijen.Djeca[2].Znak == "<bin_ili_izraz>") {

				LogIIzraz(korijen.Djeca[0], ref tip, ref lIzraz);

				if (!Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);

				BinIliIzraz(korijen.Djeca[2], ref tip, ref lIzraz);

				if (!Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);

				tip = "int";
				lIzraz = false;
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void LogIliIzraz(CvorStabla korijen, ref string tip, ref bool lIzraz) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<log_i_izraz>") {
				LogIIzraz(korijen.Djeca[0], ref tip, ref lIzraz);
			}
			else if (korijen.Djeca.Count == 3 && 
				korijen.Djeca[0].Znak == "<log_ili_izraz>" && 
				korijen.Djeca[1].Znak == "OP_ILI" &&
				korijen.Djeca[2].Znak == "<log_i_izraz>") {

				LogIliIzraz(korijen.Djeca[0], ref tip, ref lIzraz);

				if (!Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);

				LogIIzraz(korijen.Djeca[2], ref tip, ref lIzraz);

				if (!Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);

				tip = "int";
				lIzraz = false;
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void IzrazPridruzivanja(CvorStabla korijen, ref string tip, ref bool lIzraz) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<log_ili_izraz>") {
				LogIliIzraz(korijen.Djeca[0], ref tip, ref lIzraz);
			}
			else if (korijen.Djeca.Count == 3 && 
				korijen.Djeca[0].Znak == "<postfiks_izraz>" && 
				korijen.Djeca[1].Znak == "OP_PRIDRUZI" &&
				korijen.Djeca[2].Znak == "<izraz_pridruzivanja>") {

				PostfiksIzraz(korijen.Djeca[0], ref tip, ref lIzraz);
				
				string postfiks_izraz_tip = tip;

				if (lIzraz == false)
					ispisGresku(korijen);

				IzrazPridruzivanja(korijen.Djeca[2], ref tip, ref lIzraz);

				if (!Provjere.isImplictCastable(tip, postfiks_izraz_tip))
					ispisGresku(korijen);

				lIzraz = false;
				tip = postfiks_izraz_tip;
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void Izraz(CvorStabla korijen, ref string tip, ref bool lIzraz) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<izraz_pridruzivanja>") {
				IzrazPridruzivanja(korijen.Djeca[0], ref tip, ref lIzraz);
			}
			else if (korijen.Djeca.Count == 3 && 
				korijen.Djeca[0].Znak == "<izraz>" && 
				korijen.Djeca[1].Znak == "ZAREZ" && 
				korijen.Djeca[2].Znak == "<izraz_pridruzivanja>") {

				Izraz(korijen.Djeca[0], ref tip, ref lIzraz);
				IzrazPridruzivanja(korijen.Djeca[2], ref tip, ref lIzraz);

				lIzraz = false;
			}
			else {
				ispisGresku(korijen);
			}
		}

		/////////////////////////////////////////////////////////////// Naredbe Strukture //////////////////////////////////////////////////
	

		public void SlozenaNaredba(CvorStabla korijen, bool uNaredbiPetlje = false) {
			if (korijen.Djeca.Count == 3 &&
				korijen.Djeca[0].Znak == "L_VIT_ZAGRADA" &&
				korijen.Djeca[1].Znak == "<lista_naredbi>" &&
				korijen.Djeca[2].Znak == "D_VIT_ZAGRADA") {

				_okruzenje.udjiUBlok();
				ListaNaredbi(korijen.Djeca[1], uNaredbiPetlje);
				_okruzenje.izadjiIzBloka();

			}
			else if (korijen.Djeca.Count == 4 &&
				korijen.Djeca[0].Znak == "L_VIT_ZAGRADA" &&
				korijen.Djeca[1].Znak == "<lista_deklaracija>" &&
				korijen.Djeca[2].Znak == "<lista_naredbi>" &&
				korijen.Djeca[3].Znak == "D_VIT_ZAGRADA") {

				_okruzenje.udjiUBlok();
				ListaDeklaracija(korijen.Djeca[1]);
				ListaNaredbi(korijen.Djeca[2], uNaredbiPetlje);
				_okruzenje.izadjiIzBloka();
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void ListaNaredbi(CvorStabla korijen, bool uNaredbiPetlje = false) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<naredba>") {
				Naredba(korijen.Djeca[0], uNaredbiPetlje);
			}
			else if (korijen.Djeca.Count == 2 && 
				korijen.Djeca[0].Znak == "<lista_naredbi>" && 
				korijen.Djeca[1].Znak == "<naredba>") {

				ListaNaredbi(korijen.Djeca[0], uNaredbiPetlje);
				Naredba(korijen.Djeca[1], uNaredbiPetlje);
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void Naredba(CvorStabla korijen, bool uNaredbiPetlje = false) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<slozena_naredba>") 
				SlozenaNaredba(korijen.Djeca[0], uNaredbiPetlje);
			else if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<izraz_naredba>") {
				string tip = "";
				IzrazNaredba(korijen.Djeca[0], ref tip, uNaredbiPetlje);
			}
			else if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<naredba_grananja>")
				NaredbaGrananja(korijen.Djeca[0], uNaredbiPetlje);
			else if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<naredba_petlje>")
				NaredbaPetlje(korijen.Djeca[0]);
			else if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<naredba_skoka>")
				NaredbaSkoka(korijen.Djeca[0], uNaredbiPetlje);
			else {
				ispisGresku(korijen);
			}
		}


		public void IzrazNaredba(CvorStabla korijen, ref string tip, bool uNaredbiPetlje = false) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "TOCKAZAREZ")
				tip = "int";
			else if (korijen.Djeca.Count == 2 && korijen.Djeca[0].Znak == "<izraz>" && korijen.Djeca[1].Znak == "TOCKAZAREZ") {
				bool lIzraz = false;
				Izraz(korijen.Djeca[0], ref tip, ref lIzraz);
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void NaredbaGrananja(CvorStabla korijen, bool uNaredbiPetlje = false) {
			if (korijen.Djeca.Count == 5 &&
				korijen.Djeca[0].Znak == "KR_IF" &&
				korijen.Djeca[1].Znak == "L_ZAGRADA" &&
				korijen.Djeca[2].Znak == "<izraz>" &&
				korijen.Djeca[3].Znak == "D_ZAGRADA" &&
				korijen.Djeca[4].Znak == "<naredba>") {

				string tip = "";
				bool lIzraz = false;

				racunaSeUvjet = true;
			
				Izraz(korijen.Djeca[2], ref tip, ref lIzraz);
				if (rezultatUvjeta.Count==0) { naredbe.uvjetnoGrananjePocima(false); }
				else{
				naredbe.uvjetnoGrananjePocima(rezultatUvjeta.Count == 0 ? "0" : rezultatUvjeta[rezultatUvjeta.Count - 1],false);
				}
				racunaSeUvjet = false;
				

				if (!Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);

				Naredba(korijen.Djeca[4], uNaredbiPetlje);
				naredbe.uvjetnoGrananjeZavrseno();
			
				naredbe.invertiraj = false;
			}
			else if (korijen.Djeca.Count == 7 &&
				korijen.Djeca[0].Znak == "KR_IF" &&
				korijen.Djeca[1].Znak == "L_ZAGRADA" &&
				korijen.Djeca[2].Znak == "<izraz>" &&
				korijen.Djeca[3].Znak == "D_ZAGRADA" &&
				korijen.Djeca[4].Znak == "<naredba>" &&
				korijen.Djeca[5].Znak == "KR_ELSE" &&
				korijen.Djeca[6].Znak == "<naredba>") {

				string tip = "";
				bool lIzraz = false;
				racunaSeUvjet = true;
				Izraz(korijen.Djeca[2], ref tip, ref lIzraz);
				if (rezultatUvjeta.Count == 0) {
					naredbe.uvjetnoGrananjePocima(false);
				} else {
					naredbe.uvjetnoGrananjePocima(rezultatUvjeta.Count == 0 ?"0": rezultatUvjeta[rezultatUvjeta.Count - 1], false);
				}
				racunaSeUvjet = false;

				if (!Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);

				Naredba(korijen.Djeca[4], uNaredbiPetlje);
				naredbe.uvjetnoGrananjeZavrseno();
				if (rezultatUvjeta.Count == 0) { naredbe.uvjetnoGrananjePocima(true); } else { 
				naredbe.uvjetnoGrananjePocima(rezultatUvjeta.Count == 0 ? "0" : rezultatUvjeta[rezultatUvjeta.Count - 1], true);}
				Naredba(korijen.Djeca[6], uNaredbiPetlje);
				naredbe.uvjetnoGrananjeZavrseno();
				
				naredbe.invertiraj = false;
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void NaredbaPetlje(CvorStabla korijen) {
			if (korijen.Djeca.Count == 5 &&
				korijen.Djeca[0].Znak == "KR_WHILE" &&
				korijen.Djeca[1].Znak == "L_ZAGRADA" &&
				korijen.Djeca[2].Znak == "<izraz>" &&
				korijen.Djeca[3].Znak == "D_ZAGRADA" &&
				korijen.Djeca[4].Znak == "<naredba>") {

				string tip = "";
				bool lIzraz = false;
				Izraz(korijen.Djeca[2], ref tip, ref lIzraz);

				if (!Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);

				Naredba(korijen.Djeca[4], true);
			}
			else if (korijen.Djeca.Count == 6 &&
				korijen.Djeca[0].Znak == "KR_FOR" &&
				korijen.Djeca[1].Znak == "L_ZAGRADA" &&
				korijen.Djeca[2].Znak == "<izraz_naredba>" &&
				korijen.Djeca[3].Znak == "<izraz_naredba>" &&
				korijen.Djeca[4].Znak == "D_ZAGRADA" &&
				korijen.Djeca[5].Znak == "<naredba>") {

				string tip = "";
				IzrazNaredba(korijen.Djeca[2], ref tip);
				IzrazNaredba(korijen.Djeca[3], ref tip);

				if (!Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);

				Naredba(korijen.Djeca[5], true);
			}
			else if (korijen.Djeca.Count == 7 &&
				korijen.Djeca[0].Znak == "KR_FOR" &&
				korijen.Djeca[1].Znak == "L_ZAGRADA" &&
				korijen.Djeca[2].Znak == "<izraz_naredba>" &&
				korijen.Djeca[3].Znak == "<izraz_naredba>" &&
				korijen.Djeca[4].Znak == "<izraz>" &&
				korijen.Djeca[5].Znak == "D_ZAGRADA" &&
				korijen.Djeca[6].Znak == "<naredba>") {

				string tip = "";
				IzrazNaredba(korijen.Djeca[2], ref tip);
				IzrazNaredba(korijen.Djeca[3], ref tip);

				if (!Provjere.isImplictCastable(tip, "int"))
					ispisGresku(korijen);

				bool lIzraz = false;
				Izraz(korijen.Djeca[4], ref tip, ref lIzraz);
				Naredba(korijen.Djeca[6], true);
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void NaredbaSkoka(CvorStabla korijen, bool uNaredbiPetlje) {
			if (korijen.Djeca.Count == 2 &&
				(korijen.Djeca[0].Znak == "KR_CONTINUE" || korijen.Djeca[0].Znak == "KR_BREAK") &&
				korijen.Djeca[1].Znak == "TOCKAZAREZ") {

				if (uNaredbiPetlje == false)
					ispisGresku(korijen);

			}
			else if (korijen.Djeca.Count == 2 &&
				korijen.Djeca[0].Znak == "KR_RETURN" &&
				korijen.Djeca[1].Znak == "TOCKAZAREZ") {

				Funkcija f = _okruzenje.getZadnjaDefiniranaFunkcija();
				if (f.PovratniTip != "void")
					ispisGresku(korijen);
				
			}
			else if (korijen.Djeca.Count == 3 &&
				korijen.Djeca[0].Znak == "KR_RETURN" &&
				korijen.Djeca[1].Znak == "<izraz>" &&
				korijen.Djeca[2].Znak == "TOCKAZAREZ") {
					
				string tip = "";
				bool lIzraz = false;
				Izraz(korijen.Djeca[1], ref tip, ref lIzraz);

				Funkcija f = _okruzenje.getZadnjaDefiniranaFunkcija();
				if (!Provjere.isImplictCastable(tip, f.PovratniTip))
					ispisGresku(korijen);

                naredbe.pozoviReturn(((varijable.ElementAt(varijable.Count - 1)) == null) ? (null) : (varijable.ElementAt(varijable.Count - 1)));
                varijable.RemoveAt(varijable.Count - 1);
          
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void PrijevodnaJedinica(CvorStabla korijen) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<vanjska_deklaracija>") {
				VanjskaDeklaracija(korijen.Djeca[0]);
			}
			else if (korijen.Djeca.Count == 2 && korijen.Djeca[0].Znak == "<prijevodna_jedinica>" && korijen.Djeca[1].Znak == "<vanjska_deklaracija>") {
				PrijevodnaJedinica(korijen.Djeca[0]);
				VanjskaDeklaracija(korijen.Djeca[1]);
			}
			else {
				ispisGresku(korijen);
			}

		}


		public void VanjskaDeklaracija(CvorStabla korijen) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<definicija_funkcije>")
				DefinicijaFunkcije(korijen.Djeca[0]);
			else if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<deklaracija>")
				Deklaracija(korijen.Djeca[0]);
				//naredbe.globalnaVarijabla();
			else {
				ispisGresku(korijen);
			}
		}


		////////////////////////////////////////////////////////// Deklaracije ///////////////////////////////////////////////////////////////////


		public void DefinicijaFunkcije(CvorStabla korijen) {

			if (korijen.Djeca.Count == 6 &&
				korijen.Djeca[0].Znak == "<ime_tipa>" &&
				korijen.Djeca[1].Znak == "IDN" &&
				korijen.Djeca[2].Znak == "L_ZAGRADA" &&
				korijen.Djeca[3].Znak == "KR_VOID" &&
				korijen.Djeca[4].Znak == "D_ZAGRADA" &&
				korijen.Djeca[5].Znak == "<slozena_naredba>") {

				string ime_tipa_Tip = "";
                funkcija = korijen.Djeca[1].LeksJedinka;

				ImeTipa(korijen.Djeca[0], ref ime_tipa_Tip);

                

				if (Provjere.isConstT(ime_tipa_Tip))
					ispisGresku(korijen);
				
				// Provjeri dali postoji definicja funkcije IDN.ime
				if (_okruzenje.jeFunkcijaDefinirana(korijen.Djeca[1].LeksJedinka))
					ispisGresku(korijen);

				if (!_okruzenje.suTipoviUskladjeni(korijen.Djeca[1].LeksJedinka, ime_tipa_Tip, new List<string>()))
					ispisGresku(korijen);
				
				// 5. zabiljeˇzi deﬁniciju i deklaraciju funkcije
				_okruzenje.definirajDeklarirajFunkciju(ime_tipa_Tip, korijen.Djeca[1].LeksJedinka, true);

                naredbe.definicijaFunkcije(korijen.Djeca[1].LeksJedinka,null);

				SlozenaNaredba(korijen.Djeca[5]);


			}
			else if (korijen.Djeca.Count == 6 &&
				korijen.Djeca[0].Znak == "<ime_tipa>" &&
				korijen.Djeca[1].Znak == "IDN" &&
				korijen.Djeca[2].Znak == "L_ZAGRADA" &&
				korijen.Djeca[3].Znak == "<lista_parametara>" &&
				korijen.Djeca[4].Znak == "D_ZAGRADA" &&
				korijen.Djeca[5].Znak == "<slozena_naredba>") {
                    funkcija = korijen.Djeca[1].LeksJedinka;

				string ime_tipa_Tip = "";
				ImeTipa(korijen.Djeca[0], ref ime_tipa_Tip);

				// ako tip nije int, char ili void javi gresku
				if (Provjere.isConstT(ime_tipa_Tip))
					ispisGresku(korijen);

				// Provjeri dali postoji definicja funkcije IDN.ime
				if (_okruzenje.jeFunkcijaDefinirana(korijen.Djeca[1].LeksJedinka))
					ispisGresku(korijen);
				
				List<string> tipovi = new List<string>();
				List<string> imena = new List<string>();
				ListaParametara(korijen.Djeca[3], ref tipovi, ref imena);

                List<Tuple<string, string>> lista = new List<Tuple<string, string>>();
                for (int i = 0; i < tipovi.Count; i++) {
                    lista.Add(Tuple.Create(imena[i], tipovi[i]));
                }
                

				if (!_okruzenje.suTipoviUskladjeni(korijen.Djeca[1].LeksJedinka, ime_tipa_Tip, tipovi))
					ispisGresku(korijen);

				_okruzenje.definirajDeklarirajFunkciju(ime_tipa_Tip, korijen.Djeca[1].LeksJedinka, imena, tipovi, true);

				_okruzenje.napuniBuffer(tipovi, imena);
                naredbe.definicijaFunkcije(korijen.Djeca[1].LeksJedinka, lista);

				SlozenaNaredba(korijen.Djeca[5]);

			}
			else {
				ispisGresku(korijen);
			}

		}


		public void ListaParametara(CvorStabla korijen, ref List<string> tipovi, ref List<string> imena) {

			if (korijen.Djeca.Count == 1 &&
				korijen.Djeca[0].Znak == "<deklaracija_parametra>") {

				string tip = "";
				string ime = "";

				DeklaracijaParametra(korijen.Djeca[0], ref tip, ref ime);

				tipovi.Add(tip);
				imena.Add(ime);

			}
			else if (korijen.Djeca.Count == 3 &&
				korijen.Djeca[0].Znak == "<lista_parametara>" &&
				korijen.Djeca[1].Znak == "ZAREZ" &&
				korijen.Djeca[2].Znak == "<deklaracija_parametra>") {

				string tip = "";
				string ime = "";

				// inicijaliziranje liste
				// Berger popravio
				//List<string> novi_tipovi = new List<string>();
				//List<string> nova_imena = new List<string>();

				ListaParametara(korijen.Djeca[0], ref tipovi, ref imena);

				DeklaracijaParametra(korijen.Djeca[2], ref tip, ref ime);

				// Provjerava sadrzi li definicija vec postojece ime
				if (imena.Contains(ime))
					ispisGresku(korijen);

				tipovi.Add(tip);
				imena.Add(ime);

			}
			else {
				ispisGresku(korijen);
			}

		}


		public void DeklaracijaParametra(CvorStabla korijen, ref string tip, ref string ime) {

			if (korijen.Djeca.Count == 2 && 
				korijen.Djeca[0].Znak == "<ime_tipa>" && 
				korijen.Djeca[1].Znak == "IDN") {

				ImeTipa(korijen.Djeca[0], ref tip);
				ime = korijen.Djeca[1].LeksJedinka;

				if (tip == "void") {
					ispisGresku(korijen);

				}
			}
			else if (korijen.Djeca.Count == 4 &&
				korijen.Djeca[0].Znak == "<ime_tipa>" && 
				korijen.Djeca[1].Znak == "IDN" && 
				korijen.Djeca[2].Znak == "L_UGL_ZAGRADA" && 
				korijen.Djeca[3].Znak == "D_UGL_ZAGRADA") {
				
				ImeTipa(korijen.Djeca[0], ref tip);
				ime = korijen.Djeca[1].LeksJedinka;

				if (tip == "void")
					ispisGresku(korijen);

				tip = "niz(" + tip + ")";
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void ListaDeklaracija(CvorStabla korijen) {

			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<deklaracija>") 
				Deklaracija(korijen.Djeca[0]);
			else if (korijen.Djeca.Count == 2 && korijen.Djeca[0].Znak == "<lista_deklaracija>" && korijen.Djeca[1].Znak == "<deklaracija>") {
				ListaDeklaracija(korijen.Djeca[0]);
				Deklaracija(korijen.Djeca[1]);
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void Deklaracija(CvorStabla korijen) {

			if (korijen.Djeca.Count == 3 && 
				korijen.Djeca[0].Znak == "<ime_tipa>" &&
				korijen.Djeca[1].Znak == "<lista_init_deklaratora>" &&
				korijen.Djeca[2].Znak == "TOCKAZAREZ") {

				string tip = "";
				ImeTipa(korijen.Djeca[0], ref tip);

				ListaInitDeklaratora(korijen.Djeca[1], tip);

				
              
                naredbe.deklalirajVarijablu(varijable.ElementAt(varijable.Count - 2),varijable.ElementAt(varijable.Count - 1));
                varijable.RemoveAt(varijable.Count - 1);
                varijable.RemoveAt(varijable.Count - 1);
				
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void ListaInitDeklaratora(CvorStabla korijen, string ntip) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<init_deklarator>") {
				InitDeklarator(korijen.Djeca[0], ntip);
			//naredbe.globalnaVarijabla()
			}
			else if (korijen.Djeca.Count == 3 &&
				korijen.Djeca[0].Znak == "<lista_init_deklaratora>" && 
				korijen.Djeca[1].Znak == "ZAREZ" && 
				korijen.Djeca[2].Znak == "<init_deklarator>") {

				ListaInitDeklaratora(korijen.Djeca[0], ntip);
				InitDeklarator(korijen.Djeca[2], ntip);
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void InitDeklarator(CvorStabla korijen, string ntip) {

			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<izravni_deklarator>") {
				string tip = "";
				// mozda bolje da overlodamo izravni_deklarator
				int br_elem = 0;
				IzravniDeklarator(korijen.Djeca[0], ntip, ref tip, ref br_elem);

				if (Provjere.isConstT(tip) || Provjere.isNizConstT(tip)) {
					ispisGresku(korijen);

				}
				
				
			}
			else if (korijen.Djeca.Count == 3 && 
				korijen.Djeca[0].Znak == "<izravni_deklarator>" && 
				korijen.Djeca[1].Znak == "OP_PRIDRUZI" && 
				korijen.Djeca[2].Znak == "<inicijalizator>") {

				string tip = "";
				int br_elemenata = 0;
				IzravniDeklarator(korijen.Djeca[0], ntip, ref tip, ref br_elemenata);

				int br_elemenata2 = 0;
				string tip2 = "";
				List<string> tipovi = new List<string>();
				Inicijalizator(korijen.Djeca[2], ref tip2, ref br_elemenata2, ref tipovi);

				if (Provjere.isT(tip) || Provjere.isConstT(tip)) {
					if (!Provjere.isImplictCastable(tip2, Provjere.getT(tip)))
						ispisGresku(korijen);
				}
				else if (Provjere.isNiz(tip)) {

					if (br_elemenata2 > br_elemenata)
						ispisGresku(korijen);

					foreach (var t in tipovi)
						if (!Provjere.isImplictCastable(t, Provjere.getT(tip)))
							ispisGresku(korijen);

				}
				else {
					ispisGresku(korijen);
				}
			}
			else {
				ispisGresku(korijen);
			}

		}


		public void IzravniDeklarator(CvorStabla korijen, string ntip, ref string tip, ref int br_elemenata) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "IDN") {
				//Console.WriteLine(korijen.Djeca[0].LeksJedinka);
				if (ntip == "void" || _okruzenje.jeVarijablaDeklariranaLokalno(korijen.Djeca[0].LeksJedinka)) {
					ispisGresku(korijen);
			
					
				}
					
				_okruzenje.deklarirajVarijablu(korijen.Djeca[0].LeksJedinka, ntip);

                zadnjaNaredba.Add("deklaracija");			

				tip = ntip;

                varijable.Add(new Varijabla(ntip, korijen.Djeca[0].LeksJedinka, 0, funkcija));

			}
			else if (korijen.Djeca.Count == 4 && 
				korijen.Djeca[0].Znak == "IDN" && 
				korijen.Djeca[1].Znak == "L_UGL_ZAGRADA" && 
				korijen.Djeca[2].Znak == "BROJ" && 
				korijen.Djeca[3].Znak == "D_UGL_ZAGRADA") {


				if (ntip == "void" || _okruzenje.jeVarijablaDeklarirana(korijen.Djeca[0].LeksJedinka))
					ispisGresku(korijen);

				try {
					br_elemenata = Convert.ToInt32(korijen.Djeca[2].LeksJedinka);
				}
				catch (FormatException) {
					ispisGresku(korijen);
				}
				catch (OverflowException) {
					ispisGresku(korijen);
				}

				if (br_elemenata < 1 || br_elemenata > 1024)
					ispisGresku(korijen);

				_okruzenje.deklarirajVarijablu(korijen.Djeca[0].LeksJedinka, "niz(" + ntip + ")");
				

				tip = "niz(" + ntip + ")";

			}
			else if (korijen.Djeca.Count == 4 && 
				korijen.Djeca[0].Znak == "IDN" && 
				korijen.Djeca[1].Znak == "L_ZAGRADA" && 
				korijen.Djeca[2].Znak == "KR_VOID" && 
				korijen.Djeca[3].Znak == "D_ZAGRADA") {

				if (_okruzenje.jeFunkcijaDeklariranaLokalno(korijen.Djeca[0].LeksJedinka)) {
					if (!_okruzenje.suTipoviUskladjeniLokalno(korijen.Djeca[0].LeksJedinka, ntip, new List<string>()))
						ispisGresku(korijen);
				}
				else 
					//netraba slati u funkciju imena argumenata funkcije. To treba maknit
					_okruzenje.definirajDeklarirajFunkciju(ntip, korijen.Djeca[0].LeksJedinka, new List<string>(), new List<string>(), false);



			}
			else if (korijen.Djeca.Count == 4 && 
				korijen.Djeca[0].Znak == "IDN" && 
				korijen.Djeca[1].Znak == "L_ZAGRADA" && 
				korijen.Djeca[2].Znak == "<lista_parametara>" && 
				korijen.Djeca[3].Znak == "D_ZAGRADA") {

					//Console.WriteLine("funkcija");
				List<string> tipovi = new List<string>();
				List<string> imena = new List<string>();

				ListaParametara(korijen.Djeca[2], ref tipovi, ref imena);

				if (_okruzenje.jeFunkcijaDeklariranaLokalno(korijen.Djeca[0].LeksJedinka)) {
					if (!_okruzenje.suTipoviUskladjeniLokalno(korijen.Djeca[0].LeksJedinka, ntip, tipovi))
						ispisGresku(korijen);
				}
				else
					//netraba slati u funkciju imena argumenata funkcije. To treba maknit
					_okruzenje.definirajDeklarirajFunkciju(ntip, korijen.Djeca[0].LeksJedinka, imena, tipovi, false);

			}
			else {
				ispisGresku(korijen);
			}
		}


		public void Inicijalizator(CvorStabla korijen, ref string tip, ref int br_elem, ref List<string> tipovi) {
			if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<izraz_pridruzivanja>") {

				bool lIzraz = false;
				string tip2 = "";
				IzrazPridruzivanja(korijen.Djeca[0], ref tip2, ref lIzraz);

				int duljinaNiza = seJedinicnoGeneriraNizZnakova(korijen.Djeca[0]);
				if (duljinaNiza != -1) {
					br_elem = duljinaNiza + 1;
					for (int i = 0; i < br_elem; i++)
						tipovi.Add("char");
				}
				else {
					tip = tip2;
				}
			}
			else if (korijen.Djeca.Count == 3 && 
				korijen.Djeca[0].Znak == "L_VIT_ZAGRADA" && 
				korijen.Djeca[1].Znak == "<lista_izraza_pridruzivanja>" && 
				korijen.Djeca[2].Znak == "D_VIT_ZAGRADA") {

				ListaIzrazaPridruzivanja(korijen.Djeca[1], ref tipovi, ref br_elem);
			}
			else {
				ispisGresku(korijen);
			}
		}


		public void ListaIzrazaPridruzivanja(CvorStabla korijen, ref List<string> tipovi, ref int br_elem) {

            if (korijen.Djeca.Count == 1 && korijen.Djeca[0].Znak == "<izraz_pridruzivanja>") {
                string tip = "";
				bool lIzraz = false;
				IzrazPridruzivanja(korijen.Djeca[0], ref tip, ref lIzraz);

                tipovi.Add(tip);
                br_elem = 1;

            }
            else if (korijen.Djeca.Count == 3 && 
				korijen.Djeca[0].Znak == "<lista_izraza_pridruzivanja>" && 
				korijen.Djeca[1].Znak == "ZAREZ" && 
				korijen.Djeca[2].Znak == "<izraz_pridruzivanja>") {

				ListaIzrazaPridruzivanja(korijen.Djeca[0], ref tipovi, ref br_elem);
               
				string tip = "";
				bool lIzraz = false;
				IzrazPridruzivanja(korijen.Djeca[2], ref tip, ref lIzraz);

				tipovi.Add(tip);
				br_elem++;
            }
            else
            {
				ispisGresku(korijen);
            }
        }


		////////////////////////////////////////////////////////////////////Djelokrug///////////////////////////////////////////////////////////////



	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generetor_koda {
	class Provjere {

		static public bool isNiz(string tip) {
			if (tip == "niz(char)" || tip == "niz(int)" || tip == "niz(const(int))" || tip == "niz(const(char))")
				return true;
			else
				return false;
		}


		static public bool isNizConstT(string tip) {
			if (tip == "niz(const(int))" || tip == "niz(const(char))")
				return true;
			else
				return false;
		}


		static public bool isImplictCastable(string tip1, string tip2) {
			switch (tip1) {
				case "char" : case "const(char)":
					if (isNiz(tip2))
						return false;
					else
						return true;
				case "int": case "const(int)":
					if (tip2 == "int" || tip2 == "const(int)")
						return true;
					else
						return false;
				case "niz(int)":
					if (tip2 == "niz(const(int))" || tip2 == "niz(int)")
						return true;
					else
						return false;
				case "niz(char)":
					if (tip2 == "niz(const(char))" || tip2 == "niz(char)")
						return true;
					else
						return false;
				default:
					return false;
			}
		}


		static public bool isExplicitCastable(string tip1, string tip2) {
			switch (tip1) {
				// mozda treba izbacit const(int) && const(char)
				case "int": case "const(int)":
					if (tip2 == "const(char)" || tip2 == "char")
						return true;
					else
						return false;
				default:
					return false;
			}
		}


		static public bool isConstT(string tip) {
			if (tip == "const(int)" || tip == "const(char)")
				return true;
			else
				return false;
		}


		static public bool isT(string tip) {
			if (tip == "char" || tip == "int")
				return true;
			else
				return false;
		}


		static public string getT(string tip) {
			switch (tip) {
				case "int":
				case "const(int)":
				case "niz(int)":
				case "niz(const(int))":
					return "int";
				case "char":
				case "const(char)":
				case "niz(char)":
				case "niz(const(char))":
					return "char";
				default:
					return tip;
			}
		}



		static public void rastaviFunkciju(string tip, ref List<string> parametri, ref string povratniTip) {
			tip = tip.Substring(9, tip.Count() - 10);
			String[] ld = tip.Split('=');
			parametri = new List<string>();
			if (ld[0] != "void") {
				foreach (string s in ld[0].Split(',')) {
					parametri.Add(s);
				}
			}
			povratniTip = ld[1];
		}

	}
}

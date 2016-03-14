using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semantic_analyzer {
	class Okruzenje {
		//kljuc: ime varijable, tuple: tip, vrijednost, jeDefinirana
		List<Dictionary<string, Tuple<string, string,bool>>> varijable;
		List<Dictionary<string, Funkcija>> funkcije;
		Dictionary<string, List<Tuple<Funkcija,bool>>> definirane;

		//ime, tip
		List<Tuple<string, string>> buffer;

		Funkcija zadnjaDefiniranaFunkcija;
		Tuple<string, string, bool> zadnjaDefiniranaVarijabla;

		public Okruzenje() {
			varijable = new List<Dictionary<string, Tuple<string, string,bool>>>();
			funkcije=new List<Dictionary<string,Funkcija>>();
			buffer = new List<Tuple<string, string>>();
			definirane = new Dictionary<string, List<Tuple<Funkcija, bool>>>();

			udjiUBlok();//"ulazimo" u globalni blok
		}


		public bool suSveDeklariraneIDefinirane() {
			foreach (KeyValuePair<string, List<Tuple<Funkcija, bool>>> p in definirane) {
				foreach(Tuple<Funkcija,bool> t in p.Value){
					if (t.Item2 == false) return false;

				}
				
			}
			return true;
		}

		/// <summary>
		/// Svaki put kad udjemo u novi blok dodajemo novo okruzenje
		/// </summary>
		public void udjiUBlok() {
			varijable.Add(new Dictionary<string, Tuple<string, string, bool>>());
			funkcije.Add(new Dictionary<string, Funkcija>());
			if (buffer.Count != 0) {
				foreach (Tuple<string, string> t in buffer) {
					deklarirajVarijablu(t.Item1, t.Item2);
				}
				buffer.Clear();
			}
		}


		/// <summary>
		/// Svaki put kad izlazimo iz bloka micemo zadnje okruzenje iz liste
		/// </summary>
		public void izadjiIzBloka() {
			if (varijable.Count > 0) {
				varijable.Remove(varijable[varijable.Count - 1]);
			}
			if (funkcije.Count > 0) {
				funkcije.Remove(funkcije[funkcije.Count - 1]);
			}
		}

		public void napuniBuffer(List<string> tipoviParametara,List<string> imenaParametara){
			for(int i=0;i<imenaParametara.Count;i++){
				buffer.Add(new Tuple<string,string>(imenaParametara[i],tipoviParametara[i]));
			}
		}
		
		/// <summary>
		/// Vraća true ako je varijabla deklarirana u nekom od okruzenja, false inace.
		/// Varijabla se smatra deklariranom ako je zadana ovako int a=5; ili ovako: int a;
		/// </summary>
		/// <param name="imeVarijable"></param>
		/// <returns></returns>
		public bool jeVarijablaDeklarirana(string imeVarijable) {

			for (int i = varijable.Count - 1; i >= 0; i--) {
				if (varijable[i].ContainsKey(imeVarijable)) return true;
			}
			return false;
		}

		public bool jeVarijablaDeklariranaLokalno(string imeVarijable) {
			return getTrenutnoOkruzenjeVarijabli().ContainsKey(imeVarijable);
		}

		public bool jeFunkcijaDeklarirana(string imeFunkcije) {

			for (int i = funkcije.Count - 1; i >= 0; i--) {
				if (funkcije[i].ContainsKey(imeFunkcije)) return true;
			}
			return false;
		}

		public bool jeFunkcijaDeklariranaLokalno(string imeFunkcije) {
				if (funkcije[funkcije.Count-1].ContainsKey(imeFunkcije)) return true;
			return false;
		}
		/// <summary>
		/// Vraca true ako je varijabla definirana false inace.
		/// Ako varijabla nije deklarirana ne moze biti ni definirana.
		/// Definiranom se smatra varijabla koja je zadana ovako int a; i onda negdje u kodu a=5;
		/// ili zadana int a=5;
		/// </summary>
		/// <param name="imeVarijable"></param>
		/// <returns></returns>
		public bool jeVarijablaDefinirana(string imeVarijable) {
			for (int i = varijable.Count - 1; i >= 0; i--) {
				if (varijable[i].ContainsKey(imeVarijable)) return varijable[i][imeVarijable].Item3;
			}
			return false;
		}

	
		/// <summary>
		/// Vraca true ako je funkcija vec definirana.
		/// </summary>
		/// <param name="imeFunkcije"></param>
		/// <returns></returns>
		public bool jeFunkcijaDefinirana(string imeFunkcije) {

			
				if (funkcije[0].ContainsKey(imeFunkcije)) return funkcije[0][imeFunkcije].Definicija;
				return false;
			
		}
		/// <summary>
		/// Metoda koja obavlja operaciju pridruzivanja vec deklariranoj varijabli.
		/// Vraca true u slucaju uspjesnog pridruzivanja false inace.
		/// Metoda ce proci neuspjesno ako pokusavamo napisati b=5; a prije toga nemamo int b,
		/// znaci ako pridruzujemo broj nedeklariranoj vrijednosti.
		/// Za naredbe oblika int b=5; koristit metodu deklarirajVarijablu()
		/// </summary>
		/// <param name="imeVarijable"></param>
		/// <param name="novaVrijednost"></param>
		/// <returns></returns>
		public bool pridruziVrijednost(string imeVarijable, string novaVrijednost) {
			//ne moze se pridruziti vrijednost ako var nije deklarirana
			if (!jeVarijablaDeklarirana(imeVarijable)) return false;
			Tuple<string, string,bool> nova = new Tuple<string,string,bool>(getTipVarijable(imeVarijable), novaVrijednost,true);
			var okruzenje = getOkruzenjeVarijable(imeVarijable);
			okruzenje.Remove(imeVarijable);
			okruzenje.Add(imeVarijable, nova);
			return true;
		}
		/// <summary>
		/// Vraca tip varijable iz NAJBLIZEG okruzenja
		/// </summary>
		/// <param name="imeVarijable"></param>
		/// <returns></returns>
		public string getTipVarijable(string imeVarijable) {
			Tuple<string, string,bool> t = getTipImeVarijable(imeVarijable);
			if (t == null) return null;
			return t.Item1;

		}
		/// <summary>
		/// Vraca vrijednost varijable iz NAJBLIZEG okruzenja. (najugnjezdenijeg)
		/// </summary>
		/// <param name="imeVarijable"></param>
		/// <returns></returns>
		public string getVrijednostVarijable(string imeVarijable) {
			Tuple<string, string,bool> t = getTipImeVarijable(imeVarijable);
			if (t == null) return null;
			return t.Item2;
		}
		/// <summary>
		/// pomocna, vraca kompletnu ntorku za predano ime varijable iz najblizeg okruzenja
		/// </summary>
		/// <param name="imeVarijable"></param>
		/// <returns></returns>
		private Tuple<string, string,bool> getTipImeVarijable(string imeVarijable) {
			for (int i = varijable.Count - 1; i >= 0; i--) {
				if (varijable[i].ContainsKey(imeVarijable)) return varijable[i][imeVarijable];
			}
			return null;
		}
		/// <summary>
		/// Metoda vraca povratni tip funkcije s predanim imenom.
		/// </summary>
		/// <param name="imeFunkcije"></param>
		/// <returns></returns>
		public string getPovratniTipFunkcije(string imeFunkcije) {
			for (int i = funkcije.Count - 1; i >= 0; i--) {
				if (funkcije[i].ContainsKey(imeFunkcije)) return funkcije[i][imeFunkcije].PovratniTip;
			}
			return null;
		}
		/// <summary>
		/// Pomocna metoda koja vraca instancu razreda Funkcija koja predstavlja funkciju s predanim imenom.
		/// </summary>
		/// <param name="imeFunkcije"></param>
		/// <returns></returns>
		private Funkcija getFunkcija(string imeFunkcije) {
			for (int i = funkcije.Count - 1; i >= 0; i--) {
				if (funkcije[i].ContainsKey(imeFunkcije)) return funkcije[i][imeFunkcije];
			}
			return null;
		}
		/// <summary>
		/// Vraca trenutno okruzenje
		/// </summary>
		/// <returns></returns>
		private Dictionary<string, Tuple<string, string,bool>> getTrenutnoOkruzenjeVarijabli() {
			return varijable[varijable.Count - 1];
		}

		private Dictionary<string, Funkcija> getTrenutnoOkruzenjeFunkcija() {
			return funkcije[funkcije.Count - 1];
		}
		/// <summary>
		/// Pomocna, vraca NAJBLIZE okruzenje u kojem se nalazi varijabla s predanim imenom.
		/// </summary>
		/// <param name="imeVarijable"></param>
		/// <returns></returns>
		private Dictionary<string, Tuple<string, string,bool>> getOkruzenjeVarijable(string imeVarijable) {
			for (int i = varijable.Count - 1; i >= 0; i--) {
				if (varijable[i].ContainsKey(imeVarijable)) return varijable[i];
			}
			return null;
		}

		/// <summary>
		/// Dodavanje nove varijable kada je zadana inicijalna vrijednost vraca true ako je
		/// dodavanje uspjelo a false inace.
		/// Dodavanje nece uspjeti ako varijabla s istim imenom postoji u trenutnom okruzenju.
		/// </summary>
		/// <param name="imeVarijable"></param>
		/// <param name="tipVarijable"></param>
		/// <returns></returns>
		public bool deklarirajVarijablu(string imeVarijable, string tipVarijable, string vrijednostVarijable) {
			if (getTrenutnoOkruzenjeVarijabli().ContainsKey(imeVarijable)) return false;
			getTrenutnoOkruzenjeVarijabli().Add(imeVarijable, new Tuple<string, string,bool>(tipVarijable, vrijednostVarijable,true));
			zadnjaDefiniranaVarijabla = new Tuple<string, string, bool>(tipVarijable, vrijednostVarijable,true);
			return true;
		}
		/// <summary>
		/// Dodavanje nove varijable kada NIJE zadana inicijalna vrijednost vraca true ako je
		/// dodavanje uspjelo a false inace.
		/// Dodavanje nece uspjeti ako varijabla s istim imenom postoji u trenutnom okruzenju.
		/// </summary>
		/// <param name="imeVarijable"></param>
		/// <param name="tipVarijable"></param>
		/// <returns></returns>
		public bool deklarirajVarijablu(string imeVarijable, string tipVarijable) {
			if (getTrenutnoOkruzenjeVarijabli().ContainsKey(imeVarijable)) return false;
			getTrenutnoOkruzenjeVarijabli().Add(imeVarijable, new Tuple<string, string, bool>(tipVarijable, "", false));
			return true;
		}
		/// <summary>
		/// Metoda odredjuje li tip vec deklarirane fje s predanim imenom ako takva postoji 
		/// isti kao predani tipovi.
		/// </summary>
		/// <param name="imeFje"></param>
		/// <param name="povratniTip"></param>
		/// <param name="tipoviArgumenata"></param>
		/// <returns></returns>
		public bool suTipoviUskladjeni(string imeFje,string povratniTip, List<string> tipoviArgumenata) {
		
	if (funkcije[0].ContainsKey(imeFje)) {
		Funkcija vecDeklarirna;
		 funkcije[0].TryGetValue(imeFje,out vecDeklarirna);
				//povratni tip sadasnje deklaracije ne odgovara prijasnjoj
				if (!povratniTip.Equals(vecDeklarirna.PovratniTip)) return false;
				//tipvi argumenata sadasnje deklaracije ne odgovaraju prijasnjoj
				if (!tipoviArgumenata.SequenceEqual(vecDeklarirna.TipoviArgumenata)) return false;
				
				
			}
			return true;
		}

		public bool suTipoviUskladjeniLokalno(string imeFje, string povratniTip, List<string> tipoviArgumenata) {

			if (getTrenutnoOkruzenjeFunkcija().ContainsKey(imeFje)) {
				Funkcija vecDeklarirna;
				getTrenutnoOkruzenjeFunkcija().TryGetValue(imeFje, out vecDeklarirna);
				//povratni tip sadasnje deklaracije ne odgovara prijasnjoj
				if (!povratniTip.Equals(vecDeklarirna.PovratniTip)) return false;
				//tipvi argumenata sadasnje deklaracije ne odgovaraju prijasnjoj
				if (!tipoviArgumenata.SequenceEqual(vecDeklarirna.TipoviArgumenata)) return false;


			}
			return true;
		}
		/// <summary>
		/// Pomocna metoda za deklaraciju/definiciju funkcije
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		private bool deklarirajFunkciju(Funkcija f) {
			if (jeFunkcijaDeklarirana(f.Ime)) {
				if (!suTipoviUskladjeni(f.Ime, f.PovratniTip, f.TipoviArgumenata)) return false;
				if (jeFunkcijaDefinirana(f.Ime) && f.Definicija) return false;
			}
			getTrenutnoOkruzenjeFunkcija().Remove(f.Ime);
			getTrenutnoOkruzenjeFunkcija().Add(f.Ime,f);
			if (f.Definicija) {
				zadnjaDefiniranaFunkcija = f;
				dodajUDeklarirane(f, true);
			} else {
				dodajUDeklarirane(f, false);
			}
			return true;

		}
		private void dodajUDeklarirane(Funkcija f, bool def) {
			if (definirane.ContainsKey(f.Ime)) {
				List<Tuple<Funkcija, bool>> ta = definirane[f.Ime];
				foreach(Tuple<Funkcija,bool> t in ta){
					if (t.Item1.Equals(f) && t.Item2==false) {
						ta.Remove(t);
						ta.Add(new Tuple<Funkcija,bool>(f,def));
						return;
					}
				}
				ta.Add(new Tuple<Funkcija, bool>(f, def));
				return;
			} else {
				var nova=new List<Tuple<Funkcija,bool>>();
				nova.Add(new Tuple<Funkcija,bool>(f,def));
				definirane.Add(f.Ime, nova);
			}
		}
		/// <summary>
		/// Metoda koja ovisno o zastavici radi definiciju/deklaraciju funkcije bez argumenata.
		/// Ako je vrijednost zastavice definicija=true obavit ce se definicija, deklaracija inace.
		/// Svaka definicija ukljucuje deklaraciju.
		/// </summary>
		/// <param name="povratniTip"></param>
		/// <param name="ime"></param>
		/// <param name="definicija"></param>
		/// <returns></returns>
		public bool definirajDeklarirajFunkciju(String povratniTip, String ime, bool definicija) {
			Funkcija nova = new Funkcija(ime, povratniTip, new List<string>(), new List<string>(), definicija);
			return deklarirajFunkciju(nova);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="povratniTip"></param>
		/// <param name="ime"></param>
		/// <param name="imenaArg"></param>
		/// <param name="tipoviArg"></param>
		/// <param name="definicija"></param>
		/// <returns></returns>
		public bool definirajDeklarirajFunkciju(String povratniTip, String ime, List<string> imenaArg, List<string> tipoviArg, bool definicija) {
			Funkcija nova = new Funkcija(ime, povratniTip, tipoviArg, imenaArg, definicija);
			return deklarirajFunkciju(nova);
		}

		public Funkcija getZadnjaDefiniranaFunkcija() {
			return zadnjaDefiniranaFunkcija;
		}

		public string getTip(string imeIdentifikatora){
			for (int i = funkcije.Count - 1; i >= 0; i--) {
				if (varijable[i].ContainsKey(imeIdentifikatora)) return varijable[i][imeIdentifikatora].Item1;
				if(funkcije[i].ContainsKey(imeIdentifikatora)){
					Funkcija f=funkcije[i][imeIdentifikatora];
					String ret = "funkcija(";
					List<string> arg = f.TipoviArgumenata;
					if (arg.Count == 0) {
						ret += "void";
					} 
					else {

						for (int j = 0; j < arg.Count - 1; j++) ret += arg[j] + ",";
						ret += arg[arg.Count - 1];
					}
					ret += "=";
					ret += f.PovratniTip;
					ret += ")";
					return ret;
				}
				
			}
			return null; 
		}

		public static void rastaviFunkciju(string tip, ref List<string> parametri, ref string povratniTip) {
			tip = tip.Substring(9, tip.Count() - 10);
			String[] ld=tip.Split('=');
			parametri = new List<string>();
			if (ld[0] != "void") {
				foreach (string s in ld[0].Split(',')) {
					parametri.Add(s);
				}
			}
			povratniTip = ld[1];
		}

		public bool jeDeklarirano(string imeIdentifikatora) {
			if (jeVarijablaDeklarirana(imeIdentifikatora)) return true;
			if (jeFunkcijaDeklarirana(imeIdentifikatora)) return true;
			return false;
		}

		public bool jeDeklariranoLokalno(string imeIdentifikatora) {
			if (jeVarijablaDeklariranaLokalno(imeIdentifikatora)) return true;
			if (jeFunkcijaDeklariranaLokalno(imeIdentifikatora)) return true;
			return false;
		}

		
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace syntax_analyzer_generator {

	[Serializable()]
	class LrTablica : ISerializable {


		private Dictionary<Tuple<int, string>, string> _tablicaAkcija = new Dictionary<Tuple<int, string>, string>();
		private Dictionary<Tuple<int, string>, string> _tablicaNovoStanje = new Dictionary<Tuple<int, string>, string>();


		public LrTablica() { }


		public LrTablica(SerializationInfo info, StreamingContext ctxt) {
			_tablicaAkcija = (Dictionary<Tuple<int, string>, string>) info.GetValue("tablicaAkcija", typeof(Dictionary<Tuple<int, string>, string>));
			_tablicaNovoStanje = (Dictionary<Tuple<int, string>, string>) info.GetValue("tablicaNovoStanje", typeof(Dictionary<Tuple<int, string>, string>));
		}


		public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
			info.AddValue("tablicaAkcija", _tablicaAkcija);
			info.AddValue("tablicaNovoStanje", _tablicaNovoStanje);
		}


		/// <summary>
		/// Metoda vraća akciju iz tablice LR parsera za zadano stanje i zavrsni znak.
		/// Ako nepostoji par (stanje, zavrsni znak) u tablici metoda vraća "Odbaci()".
		/// Akcija je kodirana na jedan od sljedecih nacina:
		///		Pomakni(t) gdje t oznacava stanje automata.
		///		Reduciraj(p) gdje p ozacava produkciju oblika "A -> B b C c ..."
		///		Prihvati()
		///		Odbaci()
		/// </summary>
		/// <param name="stanje"></param>
		/// <param name="zavrsniZnak"></param>
		/// <returns></returns>
		public string GetAkcija(int stanje, string zavrsniZnak) {
			try {
				return _tablicaAkcija[Tuple.Create(stanje, zavrsniZnak)];
			}
			catch (KeyNotFoundException) {
				return "Odbaci()";
			}
		}


		/// <summary>
		/// Metoda vraća naredbu iz tablice novo stanje LR parsera za zadano stanje i nezavrsni znak.
		/// Ako nepostoji par (stanje, nezavrsni znak) u tablici metoda vraća "Odbaci()".
		/// Akcija je kodirana na jedan od sljedecih nacina:
		///		Stavi(t) gdje t oznacava stanje automata.
		/// </summary>
		/// <param name="stanje"></param>
		/// <param name="nezavrsniZnak"></param>
		/// <returns></returns>
		public string GetNovoStanje(int stanje, string nezavrsniZnak) {
			try {
				return _tablicaNovoStanje[Tuple.Create(stanje, nezavrsniZnak)];
			}
			catch (KeyNotFoundException) {
				return "Odbaci()";
			}
		}


		/// <summary>
		/// Dodaj akciju u tablicu akcija za zadano stanje automata i zadani zavrsni nezavrsni znak.
		/// Akcija se kodira u obliku stringa.
		/// </summary>
		/// <param name="stanje"></param>
		/// <param name="znak"></param>
		/// <param name="akcija"></param>
		public void DodajElementUTablicuAkcija(int stanje, string zavrsniZnak, string akcija) {
			try {
				_tablicaAkcija[Tuple.Create(stanje, zavrsniZnak)] = akcija;
			}
			catch (KeyNotFoundException) {
				_tablicaAkcija.Add(Tuple.Create(stanje, zavrsniZnak), akcija);
			}
		}


		/// <summary>
		/// Dodaj akciju u tablicu novoStanje za zadano stanje automata i zadani nezavrsni znak.
		/// Akcija se kodira u obliku stringa.
		/// </summary>
		/// <param name="stanje"></param>
		/// <param name="znak"></param>
		/// <param name="akcija"></param>
		public void DodajElementUTablicuNovoStanje(int stanje, string nezavrsniZnak, string akcija) {
			try {
				_tablicaNovoStanje[Tuple.Create(stanje, nezavrsniZnak)] = akcija;
			}
			catch (KeyNotFoundException) {
				_tablicaNovoStanje.Add(Tuple.Create(stanje, nezavrsniZnak), akcija);
			}
		}

	}

}

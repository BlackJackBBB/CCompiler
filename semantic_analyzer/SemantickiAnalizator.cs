using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semantic_analyzer {
	class SemantickiAnalizator {

		private CvorStabla _korijenGenStabla;
		private Okruzenje _okruzenje;


		public SemantickiAnalizator(CvorStabla korijenStabla, Okruzenje okruzenje) {
			_korijenGenStabla = korijenStabla;
			_okruzenje = okruzenje;
		}


		private string[] consoleToString() {
			string line;
			List<string> linijeUlaza = new List<string>();

			do {
				line = System.Console.ReadLine();
				if (line != null)
					linijeUlaza.Add(line);
				else
					break;
			} while (true);

			return linijeUlaza.ToArray();
		}


		public void analiziraj() {

			SmantickaPravila pravila = new SmantickaPravila(_okruzenje);

			pravila.PrijevodnaJedinica(_korijenGenStabla);

			if (!_okruzenje.suTipoviUskladjeni("main", "int", new List<string>()) || !_okruzenje.jeFunkcijaDefinirana("main"))
				Console.WriteLine("main");

			if (!_okruzenje.suSveDeklariraneIDefinirane())
				Console.WriteLine("funkcija");


		}

		
		
	}
}

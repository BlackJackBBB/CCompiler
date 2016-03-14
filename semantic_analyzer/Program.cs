using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semantic_analyzer {
	class Program {

		static void Main(string[] args) {

			GeneratorStabla stablo = new GeneratorStabla("D:\\Downloads\\New folder\\30_const_init\\test.in");

			SemantickiAnalizator analizator = new SemantickiAnalizator(stablo.Korijen, new Okruzenje());

			analizator.analiziraj();

		}
	}
}

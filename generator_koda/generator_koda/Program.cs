using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generetor_koda {
	class Program {

		static void Main(string[] args) {

			GeneratorStabla stablo = new GeneratorStabla("D:\\Desktop\\New folder\\12_fun2\\test.in");

			SemantickiAnalizator analizator = new SemantickiAnalizator(stablo.Korijen, new Okruzenje());

			analizator.analiziraj();

		}
	}
}

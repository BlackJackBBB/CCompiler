using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lexical_analyzer;

namespace lexical_analyzer_generator {
    class Program {

		static void Main(string[] args) {
			/*GeneratorKoda.KopirajTekst("Stanje.txt", "analizator/Stanje.cs");
			GeneratorKoda.KopirajTekst("Prijelaz.txt", "analizator/Prijelaz.cs");
			GeneratorKoda.KopirajTekst("LeksJedinka.txt", "analizator/LeksJedinka.cs");
			GeneratorKoda.KopirajTekst("ENKA.txt", "analizator/ENKA.cs");
			GeneratorKoda.KopirajTekst("LeksAnalizator.txt", "analizator/LeksAnalizator.cs");
			GeneratorKoda.KopirajTekst("Serializer.txt", "analizator/Serializer.cs");
			GeneratorKoda.KopirajTekst("MainAnalizator.txt", "analizator/Program.cs");*/

			DefinicijaLeksAnalizatora definicijaAnalizatora = new DefinicijaLeksAnalizatora("D:/Downloads/04_regex_laksi/test.lan");

			Dictionary<string, List<LeksJedinka>> pravilaAnalizatora = definicijaAnalizatora.PravilaAnalizatora;

			//Serializer.SerializeObject("analizator/tablica.bin", pravilaAnalizatora);

			//Dictionary<string, List<LeksJedinka>> prav = s.DeSerializeObject<Dictionary<string, List<LeksJedinka>>>("D:/Downloads/01_nadji_x/test.out");


			LeksAnalizator analizator = new LeksAnalizator(pravilaAnalizatora);
			analizator.Analiziraj("D:/Downloads/04_regex_laksi/test.in");

		}
    }
}

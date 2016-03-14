using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syntax_analyzer_generator {
	class Program {
		static void Main(string[] args) {


			/*
			GeneratorKoda.KopirajTekst("SintaksniAnalizator.txt", "analizator/SintaksniAnalizator.cs");
			GeneratorKoda.KopirajTekst("Serializer.txt", "analizator/Serializer.cs");
			GeneratorKoda.KopirajTekst("LrTablica.txt", "analizator/LrTablica.cs");
			GeneratorKoda.KopirajTekst("CvorStabla.txt", "analizator/CvorStabla.cs");
			GeneratorKoda.KopirajTekst("MainAnalizator.txt", "analizator/Program.cs");

			DefinicijaAnalizatora definicija = new DefinicijaAnalizatora();

			ENKA enka = GeneratorENKA.generirajENKA(definicija);

			DKA dka = GeneratorDKA.generiraj(enka);

			LrTablica tablica = GeneratorLrTablice.generiraj(dka, definicija.ZavrsniZnakovi);

			Serializer.SerializeObject("analizator/tablica.bin", tablica);

			Serializer.SerializeObject("analizator/zavrsni_znakovi.bin", definicija.ZavrsniZnakovi);

			Serializer.SerializeObject("analizator/sinkronizacijski_znakovi.bin", definicija.SinkronizacijskiZnakovi);
			*/


			///////////////////////////////////////////////////////////////////////////////////////////////////////////


			/*
			LrTablica lrTablica = Serializer.DeSerializeObject<LrTablica>("analizator/tablica.bin");

			string[] zavrsniZnakovi = Serializer.DeSerializeObject<string[]>("analizator/zavrsni_znakovi.bin");

			string[] sinkronizacijskiZnakovi = Serializer.DeSerializeObject<string[]>("analizator/sinkronizacijski_znakovi.bin");

			SintaksniAnalizator analizator = new SintaksniAnalizator(lrTablica, sinkronizacijskiZnakovi, zavrsniZnakovi);
			analizator.Analiziraj("D:/Downloads/New folder/00aab_1/test.in");
			*/


			/////////////////////////////////////////////////////////////////////////////////////////////////////////


			string lokacijaUlazneDatoteke = ("D:/Downloads/New folder/14simplePpjLang/test.san");

			DefinicijaAnalizatora definicija = new DefinicijaAnalizatora(lokacijaUlazneDatoteke);


			ENKA enka = GeneratorENKA.generiraj(definicija);


			DKA dka = GeneratorDKA.generiraj(enka);


			LrTablica tablica = GeneratorLrTablice.generiraj(dka);

			SintaksniAnalizator analizator = new SintaksniAnalizator(tablica, definicija.SinkronizacijskiZnakovi, definicija.ZavrsniZnakovi);

			string lokacijaPrograma = "D:/Downloads/New folder/14simplePpjLang/test.in";

			analizator.Analiziraj(lokacijaPrograma);
			

		}
	}
}



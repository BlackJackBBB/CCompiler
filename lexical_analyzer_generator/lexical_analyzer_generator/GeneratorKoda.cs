﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lexical_analyzer_generator {
	class GeneratorKoda {

		/// <summary>
		/// Kontruktor...
		/// </summary>
		public GeneratorKoda() {}

		
        public static void KopirajTekst(String srcPath, String destPath) {
            System.IO.File.WriteAllText(destPath, System.IO.File.ReadAllText(srcPath, Encoding.UTF8));
        }


	}
}

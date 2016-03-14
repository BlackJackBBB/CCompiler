using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace lexical_analyzer {

    class Serializer {

        public Serializer() { }


        public static void SerializeObject<T>(string filename, T objectToSerialize) {
            Stream stream = File.Open(filename, FileMode.Create);
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, objectToSerialize);
            stream.Close();
        }


        public static T DeSerializeObject<T>(string filename) {
			T objectToSerialize;
			Stream stream = File.Open(filename, FileMode.Open);
			BinaryFormatter bFormatter = new BinaryFormatter();
			objectToSerialize = (T) bFormatter.Deserialize(stream);
			stream.Close();
			return objectToSerialize;
        }

    }
}

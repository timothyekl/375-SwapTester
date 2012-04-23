using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace AssignmentTests
{
	public class TestHelper
	{
		public static string ExtractResourceToTempFile (string resourceName) {
			Assembly assembly = Assembly.GetExecutingAssembly();
			Stream resourceStream = assembly.GetManifestResourceStream(resourceName);
			StreamReader reader = new StreamReader(resourceStream);
			
			string tempFileName = Path.GetTempFileName ();
			using(FileStream tempWriteStream = File.Open(tempFileName, FileMode.Open)) {
				Byte[] text = new UTF8Encoding(true).GetBytes(reader.ReadToEnd());
				tempWriteStream.Write(text, 0, text.Length);
			}
			
			return tempFileName;
		}
	}
}


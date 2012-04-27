using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace AssignmentTests
{
	public class TestHelper
	{
		public static TempFileWrapper ExtractResourceToTempFile (string resourceName) {
			Assembly assembly = Assembly.GetExecutingAssembly();
			Stream resourceStream = assembly.GetManifestResourceStream(resourceName);
			StreamReader reader = new StreamReader(resourceStream);
			
			string tempFileName = Path.GetTempFileName ();
			using(FileStream tempWriteStream = File.Open(tempFileName, FileMode.Open)) {
				Byte[] text = new UTF8Encoding(true).GetBytes(reader.ReadToEnd());
				tempWriteStream.Write(text, 0, text.Length);
			}
			
			return new TempFileWrapper(tempFileName);
		}
		
		public static List<string> ExtractResourceToLineArray (string resourceName) {
			Assembly assembly = Assembly.GetExecutingAssembly();
			Stream resourceStream = assembly.GetManifestResourceStream(resourceName);
			StreamReader reader = new StreamReader(resourceStream);
			
			List<string> result = new List<string>();
			string line;
			while((line = reader.ReadLine ()) != null) {
				result.Add(line);
			}
			return result;
		}
	}
	
	public class TempFileWrapper : IDisposable
	{
		public string Name {get; private set;}
		
		public TempFileWrapper(string name) {
			this.Name = name;
		}
		
		void IDisposable.Dispose() {
			File.Delete(this.Name);
		}
		
		public override string ToString() {
			return this.Name;
		}
		
		public static implicit operator string(TempFileWrapper t) {
			return t.ToString ();
		}
	}
}


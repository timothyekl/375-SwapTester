using System;
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


using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace AssignmentTests
{
	public class TestHelper
	{
		public static TempFileWrapper ExtractResourceToTempFile (string resourceName)
		{
			return ExtractResourceToTempFileWithPath (resourceName, Path.GetTempFileName ());
		}
		
		public static TempFileWrapper ExtractResourceToTempFileWithName (string resourceName, string targetName)
		{
			return ExtractResourceToTempFileWithPath (resourceName, Path.GetTempPath () + targetName);
		}
		
		public static TempFileWrapper ExtractResourceToTempFileWithPath (string resourceName, string targetPath)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			Stream resourceStream = assembly.GetManifestResourceStream(resourceName);
			StreamReader reader = new StreamReader(resourceStream);
			
			using(FileStream tempWriteStream = File.Open(targetPath, FileMode.OpenOrCreate)) {
				Byte[] text = new UTF8Encoding(true).GetBytes(reader.ReadToEnd());
				tempWriteStream.Write(text, 0, text.Length);
			}
			
			return new TempFileWrapper(targetPath);
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
		
		public static string JoinQuoted (string[] args)
		{
			string argStr = "";
			if(args != null) {
				foreach(string arg in args) {
					if(arg.IndexOf(' ') != -1) {
						argStr += '"' + arg + '"';
					} else {
						argStr += arg;
					}
					argStr += " ";
				}
			}
			return argStr;
		}
	}
	
	public class TempFileWrapper : IDisposable
	{
		public string Path {get; private set;}
		
		public TempFileWrapper(string path) {
			this.Path = path;
		}
		
		void IDisposable.Dispose() {
			File.Delete(this.Path);
		}
		
		public override string ToString() {
			return this.Path;
		}
		
		public static implicit operator string(TempFileWrapper t) {
			return t.ToString ();
		}
	}
}


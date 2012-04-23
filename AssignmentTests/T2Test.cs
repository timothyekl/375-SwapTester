using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AssignmentTests
{
	[TestFixture(), Timeout(2000)]
	public class T2Test : ATest
	{
		[Test()]
		public void TestBatchLoad ()
		{
			// Use the T1 input file multiple times
			// The temp file wrapper semantics will give different range names
			using(TempFileWrapper tempFile1 = TestHelper.ExtractResourceToTempFile ("AssignmentTests.Resources.t1.in"),
			      tempFile2 = TestHelper.ExtractResourceToTempFile ("AssignmentTests.Resources.t1.in"),
			      tempFile3 = TestHelper.ExtractResourceToTempFile ("AssignmentTests.Resources.t1.in"))
			{
				this.Runner.StartApp (new string[] {tempFile1, tempFile2, tempFile3});
				this.Runner.WriteInputLine ("");
				
				List<String> lines = this.Runner.GetOutputLines ();
				lines.RemoveAll ((string s) => (new Regex("^\\s*$")).IsMatch (s));
				
				Assert.AreEqual (7 * 9, lines.Count, "Unexpected number of lines in output");
			}
		}
		
		[Test()]
		public void TestFileOutput ()
		{
			using(TempFileWrapper outFile = new TempFileWrapper(Path.GetTempFileName ()),
			      inFile = TestHelper.ExtractResourceToTempFile ("AssignmentTests.Resources.t1.in"))
			{
				this.Runner.StartApp (new string[] {"-output", outFile, inFile});
				
				List<string> lines = this.Runner.GetOutputLines ();
				lines.RemoveAll ((string line) => (new Regex("^\\s*$")).IsMatch (line));
				
				Assert.AreEqual (0, lines.Count, "Unexpected output on console");
			
				using(FileStream outputReadStream = File.Open(outFile, FileMode.Open))
				{
					StreamReader reader = new StreamReader(outputReadStream);
					List<string> fileLines = new List<string>();
					String line;
					while((line = reader.ReadLine ()) != null) {
						fileLines.Add (line);
					}
					
					fileLines.RemoveAll ((string s) => (new Regex("^\\s*$")).IsMatch (s));
					
					Assert.AreEqual (3 * 7, fileLines.Count, "Unexpected number of lines in output file");
				}
			}
		}
		
		[Test()]
		public void TestFileOutputWithBatchLoad ()
		{
			// Use the T1 input file multiple times
			// The temp file wrapper semantics will give different range names
			using(TempFileWrapper outFile = new TempFileWrapper(Path.GetTempFileName ()),
			      inFile = TestHelper.ExtractResourceToTempFile ("AssignmentTests.Resources.t1.in"),
			      inFile2 = TestHelper.ExtractResourceToTempFile ("AssignmentTests.Resources.t1.in"))
			{
				this.Runner.StartApp (new string[] {"-output", outFile, inFile, inFile2});
				this.Runner.WriteInputLine ("");
				
				List<String> lines = this.Runner.GetOutputLines ();
				lines.RemoveAll ((string s) => (new Regex("^\\s*$")).IsMatch (s));
				
				Assert.AreEqual (0, lines.Count, "Unexpected output on console");
				
				using(FileStream outputReadStream = File.Open(outFile, FileMode.Open))
				{
					StreamReader reader = new StreamReader(outputReadStream);
					List<string> fileLines = new List<string>();
					String line;
					while((line = reader.ReadLine ()) != null) {
						fileLines.Add (line);
					}
					
					fileLines.RemoveAll ((string s) => (new Regex("^\\s*$")).IsMatch (s));
					
					Assert.AreEqual (6 * 7, fileLines.Count, "Unexpected number of lines in output file");
				}
			}
		}
		
		[Test()]
		public void TestARMSupport ()
		{
			using(TempFileWrapper inFile = TestHelper.ExtractResourceToTempFile ("AssignmentTests.Resources.t2-frmarm.in")) {
				this.Runner.StartApp (new string[] {inFile});
				this.Runner.WriteInputLine ("");
				
				List<string> lines = this.Runner.GetOutputLines ();
				lines.RemoveAll ((string s) => (new Regex("^\\s*$")).IsMatch (s));
				
				Assert.AreEqual (15, lines.Count, "Unexpected number of output lines");
				
				Assert.AreEqual (1, lines.FindAll ((string s) => (new Regex("ARM$")).IsMatch (s)).Count, "Unexpected number of ARM loans displayed");
				Assert.AreEqual (1, lines.FindAll ((string s) => (new Regex("^Margin.*\\.5$")).IsMatch (s)).Count, "Margin rate not given properly for ARM loan");
			}
		}
		
		[Test()]
		public void TestErrors ()
		{
			using(TempFileWrapper inFile = TestHelper.ExtractResourceToTempFile ("AssignmentTests.Resources.t2-errors.in")) {
				this.Runner.StartApp (new string[] {inFile});
				this.Runner.WriteInputLine ("");
				
				List<string> lines = this.Runner.GetOutputLines ();
				lines.RemoveAll ((string s) => (new Regex("^\\s*$")).IsMatch (s));
				
				Assert.AreEqual (11, lines.Count, "Unexpected number of output lines");
				
				List<string> errorLines = lines.FindAll ((string s) => (new Regex("^Error.*ABC123")).IsMatch (s));
				Assert.AreEqual (2, errorLines.Count, "Wrong number of errors detected");
				foreach (string errorLine in errorLines) {
					Int32 idx = lines.IndexOf(errorLine) + 1;
					if(!(new Regex("^Unknown.*XXX")).IsMatch (lines[idx]) && !(new Regex("[Ff]ace.*[Vv]alue.*0")).IsMatch (lines[idx])) {
						Assert.Fail ("Wrong error detected (on text line " + (idx + 1) + ")");
					}
				}
			}
		}
	}
}


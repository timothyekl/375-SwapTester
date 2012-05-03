using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AssignmentTests
{
	public enum R2OutputLineType {
		Whitespace,
		Header,
		Range,
		Error,
		Other
	};
	
	[TestFixture(), Timeout(2000)]
	public class R2Test : RTest
	{
		[Test()]
		public void TestBatchLoad ()
		{
			string resource = "AssignmentTests.Resources.r1-full.in";
			string targetName = "test range.txt";
			// Use the R1 input file multiple times
			// The temp file wrapper semantics will give different range names
			using(TempFileWrapper tempFile1 = TestHelper.ExtractResourceToTempFileWithName (resource, targetName),
			      tempFile2 = TestHelper.ExtractResourceToTempFileWithName (resource, targetName),
			      tempFile3 = TestHelper.ExtractResourceToTempFileWithName (resource, targetName))
			{
				this.Runner.StartApp (new string[] {tempFile1, tempFile2, tempFile3});
				this.Runner.WriteInputLine ("");
				
				List<string> lines = this.Runner.GetOutputLines ();
				Dictionary<R2OutputLineType,List<string>> categorizedLines = this.CategorizeLines (lines);
				
				Assert.AreEqual (3, categorizedLines[R2OutputLineType.Header].Count, 
				                 this.Runner.ExtendedMessage ().WithMessage ("Unexpected number of header lines printed"));
				Assert.AreEqual (12, categorizedLines[R2OutputLineType.Range].Count,
				                 this.Runner.ExtendedMessage ().WithMessage ("Unexpected number of range lines printed"));
			}
		}
		
		[Test()]
		public void TestFileOutput ()
		{
			using(TempFileWrapper outFile = new TempFileWrapper(Path.GetTempFileName ()),
			      inFile = TestHelper.ExtractResourceToTempFileWithName ("AssignmentTests.Resources.r1-full.in", "test range.txt"))
			{
				this.Runner.StartApp (new string[] {"-output", outFile, inFile});
				
				List<string> lines = this.Runner.GetOutputLines ();
				Dictionary<R2OutputLineType,List<string>> categorizedLines = this.CategorizeLines (lines);
				
				int total = 0;
				foreach (R2OutputLineType type in Enum.GetValues (typeof(R2OutputLineType))) {
					total += categorizedLines[type].Count;
				}
				
				Assert.AreEqual (0, total, this.Runner.ExtendedMessage ().WithMessages ("Program provided unexpected output", "All output should be to file"));
				
				using(FileStream outputReadStream = File.Open(outFile, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					StreamReader reader = new StreamReader(outputReadStream);
					List<string> fileLines = new List<string>();
					String line;
					while((line = reader.ReadLine ()) != null) {
						fileLines.Add (line);
					}
					categorizedLines = this.CategorizeLines(fileLines);
					
					Assert.AreEqual (1, categorizedLines[R2OutputLineType.Header].Count, 
					                 this.Runner.ExtendedMessage ().WithMessage ("Program had unexpected number of header lines in output file"));
					Assert.AreEqual (4, categorizedLines[R2OutputLineType.Range].Count, 
					                 this.Runner.ExtendedMessage ().WithMessage ("Program had unexpected number of range lines in output file"));
				}
			}
		}
		
		[Test()]
		public void TestFileOutputWithBatch ()
		{
			string resource = "AssignmentTests.Resources.r1-full.in";
			string targetName = "test range.txt";
			using(TempFileWrapper outFile = new TempFileWrapper(Path.GetTempFileName ()),
			      inFile = TestHelper.ExtractResourceToTempFileWithName (resource, targetName),
			      inFile2 = TestHelper.ExtractResourceToTempFileWithName (resource, targetName))
			{
				this.Runner.StartApp (new string[] {"-output", outFile, inFile, inFile2});
				
				List<string> lines = this.Runner.GetOutputLines ();
				Dictionary<R2OutputLineType,List<string>> categorizedLines = this.CategorizeLines (lines);
				
				int total = 0;
				foreach (R2OutputLineType type in Enum.GetValues (typeof(R2OutputLineType))) {
					total += categorizedLines[type].Count;
				}
				
				Assert.AreEqual (0, total, this.Runner.ExtendedMessage ().WithMessages ("Program provided unexpected output", "All output should be to file"));
				
				using(FileStream outputReadStream = File.Open(outFile, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					StreamReader reader = new StreamReader(outputReadStream);
					List<string> fileLines = new List<string>();
					String line;
					while((line = reader.ReadLine ()) != null) {
						fileLines.Add (line);
					}
					categorizedLines = this.CategorizeLines(fileLines);
					
					Assert.AreEqual (2, categorizedLines[R2OutputLineType.Header].Count, 
					                 this.Runner.ExtendedMessage ().WithMessage ("Program had unexpected number of header lines in output file"));
					Assert.AreEqual (8, categorizedLines[R2OutputLineType.Range].Count, 
					                 this.Runner.ExtendedMessage ().WithMessage ("Program had unexpected number of range lines in output file"));
				}
			}
		}
		
		[Test()]
		public void TestNegativeRanges ()
		{
			using(TempFileWrapper inFile = TestHelper.ExtractResourceToTempFileWithName ("AssignmentTests.Resources.r2-negative.in", "negative range.txt"))
			{
				this.Runner.StartApp (new string[] {inFile});
				this.Runner.WriteInputLine ("");
				
				List<String> lines = this.Runner.GetOutputLines ();
				Dictionary<R2OutputLineType,List<string>> categorizedLines = this.CategorizeLines (lines);
				
				Assert.AreEqual (1, categorizedLines[R2OutputLineType.Header].Count, this.Runner.ExtendedMessage ().WithMessage ("Unexpected number of header lines printed"));
				Assert.AreEqual (3, categorizedLines[R2OutputLineType.Range].Count, this.Runner.ExtendedMessage ().WithMessage ("Unexpected number of range lines printed"));
				
				int[] occurrences = new int[] {1, 2, 1};
				for(int i = 0; i < 3; i++) {
					Assert.AreEqual (occurrences[i], (new Regex("100")).Matches(categorizedLines[R2OutputLineType.Range][i]).Count, 
					                 this.Runner.ExtendedMessage ().WithMessage ("Bad output in range entry " + (i + 1)));
				}
			}
		}
		
		[Test()]
		public void TestExtraValues ()
		{
			using(TempFileWrapper inFile = TestHelper.ExtractResourceToTempFileWithName ("AssignmentTests.Resources.r2-extra.in", "extra value range.txt"))
			{
				this.Runner.StartApp (new string[] {inFile});
				this.Runner.WriteInputLine ("");
				
				List<String> lines = this.Runner.GetOutputLines ();
				Dictionary<R2OutputLineType,List<string>> categorizedLines = this.CategorizeLines (lines);
				
				Assert.AreEqual (1, categorizedLines[R2OutputLineType.Header].Count, this.Runner.ExtendedMessage ().WithMessage ("Unexpected number of header lines printed"));
				Assert.AreEqual (4, categorizedLines[R2OutputLineType.Range].Count, this.Runner.ExtendedMessage ().WithMessage ("Unexpected number of range lines printed"));
				
				Regex extraValueRegex = new Regex("\\s*\\.[0-9]+$");
				for(int i = 0; i < 4; i++) {
					Assert.AreEqual (1, extraValueRegex.Matches (categorizedLines[R2OutputLineType.Range][i]).Count, 
					                 this.Runner.ExtendedMessage ().WithMessage ("Extra value not found in output line " + (i + 1)));
				}
			}
		}
		
		[Test()]
		public void TestErrors ()
		{
			using(TempFileWrapper inFile = TestHelper.ExtractResourceToTempFileWithName ("AssignmentTests.Resources.r2-error.in", "error range.txt"))
			{
				this.Runner.StartApp (new string[] {inFile});
				this.Runner.WriteInputLine ("");
				
				List<String> lines = this.Runner.GetOutputLines ();
				Dictionary<R2OutputLineType,List<string>> categorizedLines = this.CategorizeLines (lines);
				
				Assert.AreEqual (1, categorizedLines[R2OutputLineType.Header].Count, this.Runner.ExtendedMessage ().WithMessage ("Unexpected number of header lines printed"));
				Assert.AreEqual (0, categorizedLines[R2OutputLineType.Range].Count, this.Runner.ExtendedMessage ().WithMessage ("Unexpected number of range lines printed"));
				Assert.AreEqual (3, categorizedLines[R2OutputLineType.Error].Count + categorizedLines[R2OutputLineType.Other].Count, 
				                 this.Runner.ExtendedMessage ().WithMessage ("Unexpected number of error indicators printed"));
				
				// TODO able to test for more specific errors?
			}
		}
	}
}


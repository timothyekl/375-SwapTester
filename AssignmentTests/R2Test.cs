using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AssignmentTests
{
	public enum R2OutputLineType {
		R2OutputLineTypeWhitespace,
		R2OutputLineTypeHeader,
		R2OutputLineTypeRange
	};
	
	[TestFixture()]
	public class R2Test : ATest
	{
		[Test()]
		public void TestBatchLoad ()
		{
			// Use the R1 input file multiple times
			// The temp file wrapper semantics will give different range names
			using(TempFileWrapper tempFile1 = TestHelper.ExtractResourceToTempFile ("AssignmentTests.Resources.r1-full.in"),
			      tempFile2 = TestHelper.ExtractResourceToTempFile ("AssignmentTests.Resources.r1-full.in"),
			      tempFile3 = TestHelper.ExtractResourceToTempFile ("AssignmentTests.Resources.r1-full.in"))
			{
				this.Runner.StartApp (new string[] {tempFile1, tempFile2, tempFile3});
				this.Runner.WriteInputLine ("");
				
				List<String> lines = this.Runner.GetOutputLines ();
				Dictionary<R2OutputLineType,List<string>> categorizedLines = this.CategorizeLines (lines);
				
				Assert.AreEqual (3, categorizedLines[R2OutputLineType.R2OutputLineTypeHeader].Count, "Unexpected number of header lines printed");
				Assert.AreEqual (12, categorizedLines[R2OutputLineType.R2OutputLineTypeRange].Count, "Unexpected number of range lines printed");
			}
		}
		
		[Test()]
		public void TestFileOutput ()
		{
			using(TempFileWrapper outFile = new TempFileWrapper(Path.GetTempFileName ()),
			      inFile = TestHelper.ExtractResourceToTempFile ("AssignmentTests.Resources.r1-full.in"))
			{
				this.Runner.StartApp (new string[] {"-output", outFile, inFile});
				
				List<string> lines = this.Runner.GetOutputLines ();
				Dictionary<R2OutputLineType,List<string>> categorizedLines = this.CategorizeLines (lines);
				
				Assert.AreEqual (0, categorizedLines[R2OutputLineType.R2OutputLineTypeRange].Count, "Program provided unexpected output");
				Assert.AreEqual (0, categorizedLines[R2OutputLineType.R2OutputLineTypeHeader].Count, "Program provided unexpected output");
				
				using(FileStream outputReadStream = File.Open(outFile, FileMode.Open))
				{
					
				}
			}
		}
		
		public Dictionary<R2OutputLineType,List<string>> CategorizeLines (List<string> lines)
		{
			Regex headerRegex = new Regex("^Range");
			Regex rangeRegex = new Regex("^\\[(-?[0-9]*)-(-?[0-9]*)\\)");
			Regex whitespaceRegex = new Regex("^\\s*$");
			
			Dictionary<R2OutputLineType,List<string>> result = new Dictionary<R2OutputLineType, List<string>>();
			foreach (R2OutputLineType type in Enum.GetValues (typeof(R2OutputLineType))) {
				result.Add (type, new List<string>());
			}
			
			foreach (string line in lines)
			{
				if (headerRegex.Matches (line).Count > 0) result[R2OutputLineType.R2OutputLineTypeHeader].Add (line);
				if (rangeRegex.Matches (line).Count > 0) result[R2OutputLineType.R2OutputLineTypeRange].Add (line);
				if (whitespaceRegex.Matches (line).Count > 0) result[R2OutputLineType.R2OutputLineTypeWhitespace].Add (line);
			}
			
			return result;
		}
	}
}


using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AssignmentTests
{
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
				
				int headers = 0;
				int ranges = 0;
				Regex headerRegex = new Regex("^Range");
				Regex rangeRegex = new Regex("^\\[(-?[0-9]*)-(-?[0-9]*)\\)");
				
				foreach (string line in lines)
				{
					if (headerRegex.Matches (line).Count > 0) headers ++;
					if (rangeRegex.Matches (line).Count > 0) ranges ++;
				}
				
				Assert.AreEqual (3, headers, "Unexpected number of header lines printed");
				Assert.AreEqual (12, ranges, "Unexpected number of range lines printed");
			}
		}
		
		[Test()]
		public void TestFileOutput ()
		{
			
		}
	}
}


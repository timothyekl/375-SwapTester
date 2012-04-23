using System;
using System.Collections.Generic;
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
				lines.RemoveAll ((string line) => (new Regex("^\\s*$")).IsMatch(line));
				
				Assert.AreEqual (21 * 3, lines.Count, "Unexpected number of lines in output");
			}
		}
	}
}


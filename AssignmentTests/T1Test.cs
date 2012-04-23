using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AssignmentTests
{
	[TestFixture(), Timeout(2000)]
	public class T1Test : ATest
	{
		[Test()]
		public void TestParsing()
		{
			using(TempFileWrapper inFile = TestHelper.ExtractResourceToTempFile ("AssignmentTests.Resources.t1.in"))
			{
				this.Runner.StartApp (new string[] {inFile});
				this.Runner.WriteInputLine ("");
				
				List<string> lines = this.Runner.GetOutputLines ();
				lines.RemoveAll (IsWhitespace);
				
				Assert.AreEqual (21, lines.Count, "Unexpected number of output lines");
			}
		}
		
		private bool IsWhitespace(string line) {
			return (new Regex("^\\s*$")).IsMatch (line);
		}
	}
}


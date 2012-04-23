using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AssignmentTests
{
	[TestFixture()]
	public class R1Test : ATest
	{
		[Test()]
		public void TestEmpty ()
		{
			using(TempFileWrapper tempFile = TestHelper.ExtractResourceToTempFile ("AssignmentTests.Resources.empty.txt")) {
				this.Runner.StartApp (new string[] {tempFile});
				
				this.Runner.WriteInputLine ("");
				
				List<String> lines = this.Runner.GetOutputLines ();
				Assert.AreEqual (1, lines.Count, "Unexpected number of lines in output");
				
				Regex firstLineRegex = new Regex("^Range Name: .*$");
				Assert.IsTrue (firstLineRegex.Matches(lines[0]).Count > 0, "First line of text did not match");
			}
		}
		
		[Test()]
		public void TestFull ()
		{
			using(TempFileWrapper tempFile = TestHelper.ExtractResourceToTempFile ("AssignmentTests.Resources.r1-full.in")) {
				this.Runner.StartApp (new string[] {tempFile});
				
				this.Runner.WriteInputLine ("");
				
				List<String> lines = this.Runner.GetOutputLines ();
				Assert.AreEqual (5, lines.Count, "Unexpected number of lines in output");
				
				// TODO match the output more closely
			}
		}
	}
}


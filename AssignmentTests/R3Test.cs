using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AssignmentTests
{
	[TestFixture()]
	public class R3Test : ATest
	{
		[Test()]
		public void TestUniqueNames ()
		{
			string resource = "AssignmentTests.Resources.r3-samename.in";
			using (TempFileWrapper inFile = TestHelper.ExtractResourceToTempFile(resource))
			{
				this.Runner.StartApp (new string[] {inFile});
				this.Runner.WriteInputLine("");
				
				List<string> lines = this.Runner.GetOutputLines ();
				
				List<string> errorLines = lines.FindAll ((string s) => (new Regex("^[Ee]rror")).IsMatch (s));
				Assert.AreEqual (1, errorLines.Count, this.Runner.ExtendedMessage ().WithMessages ("Unexpected number of errors detected in file", "This test should detect exactly one error"));
				
				List<string> nameLines = lines.FindAll ((string s) => (new Regex("Same Name")).IsMatch (s));
				Assert.AreEqual (1, nameLines.Count, this.Runner.ExtendedMessage ().WithMessages ("No indication of duplicate name", "Program should identify name of duplicate record in file"));
			}
		}
	}
}


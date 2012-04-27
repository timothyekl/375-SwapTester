using System;
using System.Collections.Generic;
using System.Linq;
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
			string resource = "AssignmentTests.Resources.t1.in";
			using(TempFileWrapper inFile = TestHelper.ExtractResourceToTempFile (resource))
			{
				this.Runner.StartApp (new string[] {inFile});
				this.Runner.WriteInputLine ("");
				
				List<string> lines = this.Runner.GetOutputLines ();
				lines.RemoveAll ((string line) => {
					return (new Regex("^\\s*$")).IsMatch (line);
				});
				
				// Check total output count
				Assert.AreEqual (21, lines.Count, this.Runner.ExtendedMessage ());
				
				// Check ordering of lines
				List<string> currentFaceLines = lines.FindAll((string line) => (new Regex("^Current [Ff]ace:")).IsMatch (line));
				IEnumerable<Int32> currentFaceValues = Enumerable.Select(currentFaceLines, (string line) => Int32.Parse ((new Regex("([0-9]+)$")).Match (line).Captures[0].Value));
				Int32 last = 999999;
				foreach(Int32 face in currentFaceValues) {
					Assert.LessOrEqual(face, last, "Entries are not sorted in descending order by current face value");
					last = face;
				}
				
				// TODO much more testing could be done here
			}
		}
	}
}


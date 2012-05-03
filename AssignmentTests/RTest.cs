using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AssignmentTests
{
	[TestFixture()]
	public class RTest : ATest
	{
		public Dictionary<R2OutputLineType,List<string>> CategorizeLines (List<string> lines)
		{
			Regex headerRegex = new Regex("^Range");
			Regex rangeRegex = new Regex("^[\\[\\(](-?[0-9]*)-(-?[0-9]*)[\\]\\)]");
			Regex whitespaceRegex = new Regex("^\\s*$");
			Regex errorRegex = new Regex("^Error in file$");
			
			Dictionary<R2OutputLineType,List<string>> result = new Dictionary<R2OutputLineType, List<string>>();
			foreach (R2OutputLineType type in Enum.GetValues (typeof(R2OutputLineType))) {
				result.Add (type, new List<string>());
			}
			
			foreach (string line in lines)
			{
				if (headerRegex.Matches (line).Count > 0) result[R2OutputLineType.Header].Add (line);
				else if (rangeRegex.Matches (line).Count > 0) result[R2OutputLineType.Range].Add (line);
				else if (whitespaceRegex.Matches (line).Count > 0) result[R2OutputLineType.Whitespace].Add (line);
				else if (errorRegex.Matches (line).Count > 0) result[R2OutputLineType.Error].Add (line);
				else result[R2OutputLineType.Other].Add (line);
			}
			
			return result;
		}
	}
}


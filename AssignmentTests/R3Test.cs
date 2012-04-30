using System;
using System.Collections.Generic;
using System.Reflection;
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
			using (TempFileWrapper inFile = TestHelper.ExtractResourceToTempFileWithName ("AssignmentTests.Resources.r3-samename.in", "same name range.txt"))
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
		
		[Test()]
		public void TestInterfaceExistence ()
		{
			Assembly appAssembly = this.Runner.LoadApplicationAssembly ();
			
			List<Type> appTypes = new List<Type> (appAssembly.GetTypes ());
			List<string> typeNames = appTypes.ConvertAll<string> ((Type t) => t.Name);
			
			Dictionary<string, List<string>> expectedInterfaces = new Dictionary<string, List<string>> ();
			expectedInterfaces.Add ("IRange", new List<string> () {"Name", "Top", "Bottom", "ExtraValue", "ThisNumberFitsInThisRange"});
			expectedInterfaces.Add ("IRangeSet", new List<string> () {"Name", "Ranges", "Errors", "WhichRangeDoesThisNumberFitIn"});
			expectedInterfaces.Add ("IRangeLoader", new List<string> () {"GetRangeSetsFromFiles", "GetRangeSetFromFile", "GetRangeSetFromText"});
			foreach(KeyValuePair<string, List<string>> kvp in expectedInterfaces)
			{
				string expectedInterfaceName = kvp.Key;
				List<string> expectedInterfaceMethods = kvp.Value;
				
				Assert.IsTrue (typeNames.Contains (expectedInterfaceName), "Program does not contain the type " + expectedInterfaceName);
				
				Type type = appAssembly.GetType (expectedInterfaceName);
				Assert.IsTrue (type.IsInterface, "Type " + type.Name + " is not an interface type");
				
				MethodInfo[] typeMethods = type.GetMethods ();
				List<string> typeMethodNames = (new List<MethodInfo> (typeMethods)).ConvertAll<string> ((MethodInfo mi) => mi.Name);
				
				foreach(string expectedMethodName in expectedInterfaceMethods)
				{
					Assert.IsTrue (typeMethodNames.Contains (expectedMethodName), "Type " + type.Name + " does not have method with name " + expectedMethodName);
				}
			}
		}
	}
}


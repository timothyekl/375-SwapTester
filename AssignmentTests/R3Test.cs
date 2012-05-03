using Microsoft.CSharp;
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
			
			Dictionary<string, List<string>> expectedInterfaces = new Dictionary<string, List<string>> ();
			expectedInterfaces.Add ("IRange", new List<string> () {"Name", "Top", "Bottom", "ExtraValue", "ThisNumberFitsInThisRange"});
			expectedInterfaces.Add ("IRangeSet", new List<string> () {"Name", "Ranges", "Errors", "WhichRangeDoesThisNumberFitIn"});
			expectedInterfaces.Add ("IRangeLoader", new List<string> () {"GetRangeSetsFromFiles", "GetRangeSetFromFile", "GetRangeSetFromText"});
			foreach(KeyValuePair<string, List<string>> kvp in expectedInterfaces)
			{
				string expectedInterfaceName = kvp.Key;
				List<string> expectedInterfaceMethods = kvp.Value;
				
				Type type = TestHelper.LoadTypeFromAssembly (expectedInterfaceName, appAssembly);
				Assert.IsNotNull (type, "Test loaded null type for name " + expectedInterfaceName);
				
				Assert.IsTrue (type.IsInterface, "Type " + type.Name + " is not an interface type");
				
				List<string> propertyNames = new List<string> ();
				propertyNames.AddRange ((new List<PropertyInfo> (type.GetProperties ())).ConvertAll<string> ((PropertyInfo pi) => pi.Name));
				propertyNames.AddRange ((new List<MethodInfo> (type.GetMethods ())).ConvertAll<string> ((MethodInfo mi) => mi.Name));
				
				foreach(string expectedMethodName in expectedInterfaceMethods)
				{
					Assert.IsTrue (propertyNames.Contains (expectedMethodName), "Type " + type.Name + " does not have method with name " + expectedMethodName);
				}
			}
		}
		
		[Test()]
		public void TestLoadingViaInterface ()
		{
			Assembly appAssembly = this.Runner.LoadApplicationAssembly ();
			
			dynamic loader = null;
			try {
				loader = TestHelper.CreateInstanceOfImplementationOfTypeFromAssembly ("IRangeLoader", appAssembly);
			} catch (Exception e) {
				Assert.Ignore ("Failed to find implementation of IRangeLoader:\n" + e.Message);
			}
			
			/*
			System.Console.WriteLine ("TEST: loader is " + loader.ToString ());
			FieldInfo fi = loader.GetType ().GetField("_fileProcessor", BindingFlags.Instance | BindingFlags.NonPublic);
			dynamic processor = fi.GetValue(loader);
			dynamic storage = processor.GetStorage ();
			System.Console.WriteLine ("TEST: loader's processor's storage is " + storage.ToString ());
			*/
			
			using (TempFileWrapper inFile = TestHelper.ExtractResourceToTempFileWithName ("AssignmentTests.Resources.r1-full.in", "sample range.txt")) {
				dynamic rangeSet = loader.GetRangeSetFromFile (inFile.ToString ());
				
				Assert.IsNotNull (rangeSet, "IRangeLoader implementation did not load range set properly");
				Assert.AreEqual ("sample", rangeSet.Name, "Loaded range set name is inaccurate");
				Assert.AreEqual (4, (new List<Object> (rangeSet.Ranges)).Count, "Loaded range set has wrong number of ranges");
				Assert.AreEqual (0, rangeSet.Errors.Count, "Loaded range set wrongly detected an error");
				Assert.AreEqual ("Stud Credit", rangeSet.WhichRangeDoesThisNumberFitIn (840).Name, "Loaded range set provided wrong range name for value 840");
				
				System.Console.WriteLine ("success");
			}
		}
	}
}


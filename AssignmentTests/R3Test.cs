using Microsoft.CSharp;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AssignmentTests
{
	[TestFixture()]
	public class R3Test : RTest
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
			
			using (TempFileWrapper inFile = TestHelper.ExtractResourceToTempFileWithName ("AssignmentTests.Resources.r1-full.in", "sample range.txt")) {
				dynamic rangeSet = TestHelper.InvokeDynamicMethod (loader, "GetRangeSetFromFile", inFile.ToString ());
				Assert.IsNotNull (rangeSet, "IRangeLoader implementation did not load range set properly");
				
				dynamic rangeSetName = TestHelper.InvokeDynamicMethod (rangeSet, "Name");
				Assert.IsTrue (rangeSetName.Contains ("sample"), "Loaded range set name is inaccurate");
				
				dynamic rangeSetRanges = TestHelper.InvokeDynamicMethod (rangeSet, "Ranges");
				Assert.AreEqual (4, (new List<Object> (rangeSetRanges)).Count, "Loaded range set has wrong number of ranges");
				
				dynamic rangeSetErrors = TestHelper.InvokeDynamicMethod (rangeSet, "Errors");
				Assert.AreEqual (0, rangeSetErrors.Count, "Loaded range set wrongly detected an error");
				
				dynamic resultingSet = TestHelper.InvokeDynamicMethod (rangeSet, "WhichRangeDoesThisNumberFitIn", 840);
				dynamic resultingSetName = TestHelper.InvokeDynamicMethod (resultingSet, "Name");
				Assert.AreEqual ("Stud Credit", resultingSetName, "Loaded range set provided wrong range name for value 840");
			}
		}
		
		[Test()]
		public void TestLoadingFromCSV ()
		{
			using (TempFileWrapper inFile = TestHelper.ExtractResourceToTempFileWithName ("AssignmentTests.Resources.r3-csv.in", "sample range.csv"))
			{
				this.Runner.StartApp (new string[] {inFile});
				this.Runner.WriteInputLine("");
				
				List<string> lines = this.Runner.GetOutputLines ();
				Dictionary<R2OutputLineType,List<string>> categorizedLines = this.CategorizeLines (lines);
				
				Assert.AreEqual (1, categorizedLines[R2OutputLineType.Header].Count, 
				                 this.Runner.ExtendedMessage ().WithMessage ("Unexpected number of header lines printed"));
				Assert.AreEqual (4, categorizedLines[R2OutputLineType.Range].Count,
				                 this.Runner.ExtendedMessage ().WithMessage ("Unexpected number of range lines printed"));
			}
		}
		
		[Test()]
		public void TestLoadingInclusiveExclusive ()
		{
			using (TempFileWrapper inFile = TestHelper.ExtractResourceToTempFileWithName ("AssignmentTests.Resources.r3-incexc.in", "sample range.csv"))
			{
				this.Runner.StartApp (new string[] {inFile});
				this.Runner.WriteInputLine("");
				
				List<string> lines = this.Runner.GetOutputLines ();
				Dictionary<R2OutputLineType,List<string>> categorizedLines = this.CategorizeLines (lines);
				
				Assert.AreEqual (1, categorizedLines[R2OutputLineType.Header].Count, 
				                 this.Runner.ExtendedMessage ().WithMessage ("Unexpected number of header lines printed"));
				Assert.AreEqual (4, categorizedLines[R2OutputLineType.Range].Count,
				                 this.Runner.ExtendedMessage ().WithMessage ("Unexpected number of range lines printed"));
			}
		}
		
		[Test()]
		public void TestInclusiveExclusiveMembership ()
		{
			Assembly appAssembly = this.Runner.LoadApplicationAssembly ();
			
			dynamic loader = null;
			try {
				loader = TestHelper.CreateInstanceOfImplementationOfTypeFromAssembly ("IRangeLoader", appAssembly);
			} catch (Exception e) {
				Assert.Ignore ("Failed to find implementation of IRangeLoader:\n" + e.Message);
			}
			
			using (TempFileWrapper inFile = TestHelper.ExtractResourceToTempFileWithName ("AssignmentTests.Resources.r3-incexc.in", "sample range.csv")) {
				dynamic rangeSet = TestHelper.InvokeDynamicMethod (loader, "GetRangeSetFromFile", inFile.ToString ());
				Assert.IsNotNull (rangeSet, "IRangeLoader implementation did not load range set properly");
				
				dynamic rangeSetName = TestHelper.InvokeDynamicMethod (rangeSet, "Name");
				Assert.IsTrue (rangeSetName.Contains ("sample"), "Loaded range set name is inaccurate");
				
				dynamic rangeSetRanges = TestHelper.InvokeDynamicMethod (rangeSet, "Ranges");
				Assert.AreEqual (4, (new List<Object> (rangeSetRanges)).Count, "Loaded range set has wrong number of ranges");
				
				dynamic rangeSetErrors = TestHelper.InvokeDynamicMethod (rangeSet, "Errors");
				Assert.AreEqual (0, rangeSetErrors.Count, "Loaded range set wrongly detected an error");
				
				Dictionary<double, string> expectedRanges = new Dictionary<double, string>() {
					{850, null},
					{849, "Stud Credit"},
					{826, "Stud Credit"},
					{825, "Good Credit"},
					{800, "Good Credit"},
					{751, "Good Credit"},
					{750, "OK credit"},
					{600, "OK credit"},
					{599, "Bad credit"},
					{300, "Bad credit"},
					{299, "Bad credit"},
					{1, "Bad credit"},
					{0, "Bad credit"},
					{-1, "Bad credit"},
					{-300, "Bad credit"},
					{-301, null}
				};
				
				foreach(KeyValuePair<double, string> kvp in expectedRanges) {
					double score = kvp.Key;
					string creditName = kvp.Value;
					
					dynamic range = TestHelper.InvokeDynamicMethod (rangeSet, "WhichRangeDoesThisNumberFitIn", score);
					if(creditName == null) {
						Assert.IsNull (range, "Range set returned non-null range for out-of-set score " + score);
					} else {
						Assert.IsNotNull (range, "Range set returned null range for in-set score " + score);
						Assert.IsTrue (creditName.Equals(TestHelper.InvokeDynamicMethod (range, "Name"), StringComparison.OrdinalIgnoreCase), 
						               "Returned range had name " + TestHelper.InvokeDynamicMethod (range, "Name") + "; expected name " + creditName);
						Assert.IsTrue (TestHelper.InvokeDynamicMethod (range, "ThisNumberFitsInThisRange", score), 
						               "Range was returned properly but gives false for containment test");
					}
				}
			}
		}
		
		[Test()]
		public void TestInclusiveExclusiveErrors ()
		{
			using (TempFileWrapper inFile = TestHelper.ExtractResourceToTempFileWithName ("AssignmentTests.Resources.r3-incexcerror.in", "sample error range.csv"))
			{
				this.Runner.StartApp (new string[] {inFile});
				this.Runner.WriteInputLine("");
				
				List<string> lines = this.Runner.GetOutputLines ();
				Dictionary<R2OutputLineType,List<string>> categorizedLines = this.CategorizeLines (lines);
				
				Assert.GreaterOrEqual (categorizedLines[R2OutputLineType.Header].Count, 1,
				                 this.Runner.ExtendedMessage ().WithMessage ("Unexpected number of header lines printed"));
				Assert.AreEqual (1, categorizedLines[R2OutputLineType.Error].Count,
				                 this.Runner.ExtendedMessage ().WithMessage ("Unexpected number of range lines printed"));
			}
		}
		
		[Test()]
		public void TestInterfaceLoadingErrors ()
		{
			Assembly appAssembly = this.Runner.LoadApplicationAssembly ();
			
			dynamic loader = null;
			try {
				loader = TestHelper.CreateInstanceOfImplementationOfTypeFromAssembly ("IRangeLoader", appAssembly);
			} catch (Exception e) {
				Assert.Ignore ("Failed to find implementation of IRangeLoader:\n" + e.Message);
			}
			
			using (TempFileWrapper inFile = TestHelper.ExtractResourceToTempFileWithName ("AssignmentTests.Resources.r3-incexcerror.in", "sample error range.csv"))
			{
				dynamic rangeSet = TestHelper.InvokeDynamicMethod (loader, "GetRangeSetFromFile", inFile.ToString ());
				
				dynamic rangeSetErrors = TestHelper.InvokeDynamicMethod (rangeSet, "Errors");
				Assert.AreEqual (1, rangeSetErrors.Count, "Range set detected wrong number of errors");
				
				dynamic rangeSetRanges = TestHelper.InvokeDynamicMethod (rangeSet, "Ranges");
				Assert.AreEqual (0, new List<Object> (rangeSetRanges).Count, "Range set included ranges despite file error");
			}
		}
	}
}


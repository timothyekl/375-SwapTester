using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AssignmentTests
{
	[TestFixture()]
	public class T3Test : ATest
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
		
		[Test()]
		public void TestInterfaceExistence ()
		{
			Assembly appAssembly = this.Runner.LoadApplicationAssembly ();
			
			List<Type> appTypes = new List<Type> (appAssembly.GetTypes ());
			List<string> typeNames = appTypes.ConvertAll<string> ((Type t) => t.Name);
			
			Dictionary<string, List<string>> expectedInterfaces = new Dictionary<string, List<string>> ();
			expectedInterfaces.Add ("LoanType", new List<string> () {"FRM", "ARM"});
			expectedInterfaces.Add ("ITransaction", new List<string> () {"Name", "Type", "FaceValue", "CurrentFace", "IssueDate", "MaturityDate", "CouponRate", "MarginRate", "PrepayRate", "CreditRating"});
			expectedInterfaces.Add ("ITransactionLoader", new List<string> () {"GetTransactionsFromFiles", "GetTransactionsFromText"});
			foreach(KeyValuePair<string, List<string>> kvp in expectedInterfaces)
			{
				string expectedInterfaceName = kvp.Key;
				List<string> expectedInterfaceMethods = kvp.Value;
				
				Assert.IsTrue (typeNames.Contains (expectedInterfaceName), "Program does not contain the type " + expectedInterfaceName);
				
				Type type = appAssembly.GetType (expectedInterfaceName);
				List<string> propertyNames;
				if(expectedInterfaceName.Equals ("LoanType"))
				{
					Assert.IsTrue (type.IsEnum, "Type " + type.Name + " is not an enum type");
					
					MethodInfo[] typeMethods = type.GetMethods ();
					propertyNames = (new List<MethodInfo> (typeMethods)).ConvertAll<string> ((MethodInfo mi) => mi.Name);
				}
				else
				{
					Assert.IsTrue (type.IsInterface, "Type " + type.Name + " is not an interface type");
					
					propertyNames = new List<string> (Enum.GetNames (type));
				}
				
				foreach(string expectedMethodName in expectedInterfaceMethods)
				{
					Assert.IsTrue (propertyNames.Contains (expectedMethodName), "Type " + type.Name + " does not have method with name " + expectedMethodName);
				}
			}
		}
	}
}


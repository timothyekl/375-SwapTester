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
			string resource = "AssignmentTests.Resources.t3-samename.in";
			using (TempFileWrapper inFile = TestHelper.ExtractResourceToTempFile(resource))
			{
				this.Runner.StartApp (new string[] {inFile});
				this.Runner.WriteInputLine("");
				
				List<string> lines = this.Runner.GetOutputLines ();
				
				List<string> errorLines = lines.FindAll ((string s) => (new Regex("^[Ee]rror")).IsMatch (s));
				Assert.AreEqual (1, errorLines.Count, this.Runner.ExtendedMessage ().WithMessages ("Unexpected number of errors detected in file", "This test should detect exactly one error"));
			}
		}
		
		[Test()]
		public void TestInterfaceExistence ()
		{
			Assembly appAssembly = this.Runner.LoadApplicationAssembly ();
			if(appAssembly == null)
			{
				Assert.Inconclusive ("Could not load application assembly\nPlease verify interfaces manually");
			}
			
			Dictionary<string, List<string>> expectedInterfaces = new Dictionary<string, List<string>> ();
			expectedInterfaces.Add ("LoanType", new List<string> () {"NONE", "FRM", "ARM"});
			expectedInterfaces.Add ("ITransaction", new List<string> () {"Name", "Type", "FaceValue", "CurrentFace", "IssueDate", "MaturityDate", "CouponRate", "MarginRate", "PrepayRate", "CreditRating"});
			expectedInterfaces.Add ("ITransactionLoader", new List<string> () {"GetTransactionsFromFiles", "GetTransactionsFromText"});
			foreach(KeyValuePair<string, List<string>> kvp in expectedInterfaces)
			{
				string expectedInterfaceName = kvp.Key;
				List<string> expectedInterfaceMethods = kvp.Value;
				
				Type type = TestHelper.LoadTypeFromAssembly (expectedInterfaceName, appAssembly);
				Assert.IsNotNull (type, "Test loaded null type for name " + expectedInterfaceName);
				
				List<string> propertyNames;
				if(!expectedInterfaceName.Equals ("LoanType"))
				{
					Assert.IsTrue (type.IsInterface, "Type " + type.Name + " is not an interface type");
					
					propertyNames = new List<string> ();
					propertyNames.AddRange ((new List<PropertyInfo> (type.GetProperties ())).ConvertAll<string> ((PropertyInfo pi) => pi.Name));
					propertyNames.AddRange ((new List<MethodInfo> (type.GetMethods ())).ConvertAll<string> ((MethodInfo mi) => mi.Name));
				}
				else
				{
					Assert.IsTrue (type.IsEnum, "Type " + type.Name + " is not an enum type");
					
					propertyNames = new List<string> (Enum.GetNames (type));
				}
				
				foreach(string expectedMethodName in expectedInterfaceMethods)
				{
					Assert.IsTrue (propertyNames.Contains (expectedMethodName), "Type " + type.Name + " does not have method with name " + expectedMethodName);
				}
			}
		}
		
		[Test()]
		public void TestUsesShortDates ()
		{
			using(TempFileWrapper inFile = TestHelper.ExtractResourceToTempFile("AssignmentTests.Resources.t1.in")) {
				this.Runner.StartApp(new string[] {inFile});
				this.Runner.WriteInputLine("");
				
				List<string> lines = this.Runner.GetOutputLines ();
				
				Regex dateCandidateRegex = new Regex("^.*: (.*[0-9]+\\/[0-9]+\\/[0-9]+.*)$");
				foreach (string line in lines) {
					if (dateCandidateRegex.IsMatch (line)) {
						string dateCandidate = dateCandidateRegex.Match (line).Groups[1].Value;
						DateTime dt = DateTime.Parse (dateCandidate);
						
						Assert.AreNotEqual (0, dt.Day, this.Runner.ExtendedMessage ().WithMessage ("Expected value for date"));
						Assert.AreNotEqual (0, dt.Month, this.Runner.ExtendedMessage ().WithMessage ("Expected value for month"));
						Assert.AreNotEqual (0, dt.Year, this.Runner.ExtendedMessage ().WithMessage ("Expected value for year"));
						
						Assert.AreEqual (0, dt.Hour, this.Runner.ExtendedMessage ().WithMessage ("Expected zero value for hour"));
						Assert.AreEqual (0, dt.Minute, this.Runner.ExtendedMessage ().WithMessage ("Expected zero value for minute"));
						Assert.AreEqual (0, dt.Second, this.Runner.ExtendedMessage ().WithMessage ("Expected zero value for second"));
					}
				}
			}
		}
		
		[Test()]
		public void TestZeroErrors ()
		{
			using(TempFileWrapper inFile = TestHelper.ExtractResourceToTempFile("AssignmentTests.Resources.t3-zeroerror.in")) {
				this.Runner.StartApp(new string[] {inFile});
				this.Runner.WriteInputLine("");
				
				List<string> lines = this.Runner.GetOutputLines ();
				
				Assert.AreEqual (1, lines.FindAll ((string s) => (new Regex("[Ee]rror")).IsMatch (s)).Count,
				                 this.Runner.ExtendedMessage ().WithMessages ("Incorrect number of error reports", "This test should report exactly one error"));
				Assert.AreEqual (1, lines.FindAll ((string s) => (new Regex("ABC123")).IsMatch (s)).Count,
				                 this.Runner.ExtendedMessage ().WithMessages ("Incomplete error report", "Error report should include failing loan's name"));
				Assert.AreEqual (1, lines.FindAll ((string s) => (new Regex("(?:[Ff]ace )?[Vv]alue")).IsMatch (s)).Count,
				                 this.Runner.ExtendedMessage ().WithMessages ("Incomplete error report", "Error report should include the phrase 'face value'"));
			}
		}
		
		[Test()]
		public void TestCreditRiskParsing ()
		{
			using(TempFileWrapper inFile = TestHelper.ExtractResourceToTempFile("AssignmentTests.Resources.t3-creditrisk.in")) {
				this.Runner.StartApp(new string[] {inFile});
				this.Runner.WriteInputLine("");
				
				List<string> lines = this.Runner.GetOutputLines ();
				
				foreach(KeyValuePair<string,string> kvp in 
				       new Dictionary<string,string> {
							{"margin", "[Mm]argin"}, 
							{"credit rating", "[Cc]redit [Rr]at(?:ing|e)"}, 
							{"prepay rate", "[Pp]repay [Rr]ate"}
				}) {
					string phrase = kvp.Key;
					Regex regex = new Regex(kvp.Value);
					
					Assert.AreEqual (2, lines.FindAll ((string s) => regex.IsMatch (s)).Count, 
					                 this.Runner.ExtendedMessage ().WithMessages ("Failed to parse " + phrase, "Expected exactly two transactions with " + phrase + "field"));
				}
			}
		}
		
		[Test()]
		public void TestValidatesCreditRisk ()
		{
			using(TempFileWrapper inFile = TestHelper.ExtractResourceToTempFile("AssignmentTests.Resources.t3-creditvalueerror.in")) {
				this.Runner.StartApp(new string[] {inFile});
				this.Runner.WriteInputLine("");
				
				List<string> lines = this.Runner.GetOutputLines ();
				
				Assert.AreEqual (2, lines.FindAll ((string s) => (new Regex("[Ee]rror")).IsMatch (s)).Count,
				                 this.Runner.ExtendedMessage ().WithMessages ("Incorrect number of error reports", "This test should report exactly two errors"));
				Assert.AreEqual (2, lines.FindAll ((string s) => (new Regex("(:?ABC|XYZ)123")).IsMatch (s)).Count,
				                 this.Runner.ExtendedMessage ().WithMessages ("Incorrect number of loan names printed", "Each error should include the relevant loan name"));
				Assert.AreEqual (1, lines.FindAll ((string s) => (new Regex("[Pp]repay [Rr]ate")).IsMatch (s)).Count,
				                 this.Runner.ExtendedMessage ().WithMessage ("Expected field name for validation failure"));
				Assert.AreEqual (1, lines.FindAll ((string s) => (new Regex("[Cc]redit [Rr]at(:?ing|e)")).IsMatch (s)).Count,
				                 this.Runner.ExtendedMessage ().WithMessage ("Expected field name for validation failure"));
				
			}
		}
		
		[Test()]
		public void TestLoanTypeParseFlexibility ()
		{
			using(TempFileWrapper inFile = TestHelper.ExtractResourceToTempFile("AssignmentTests.Resources.t3-loantype.in")) {
				this.Runner.StartApp(new string[] {inFile});
				this.Runner.WriteInputLine("");
				
				List<string> lines = this.Runner.GetOutputLines ();
				
				foreach(KeyValuePair<string,string> kvp in 
				       new Dictionary<string,string> {
							{"margin", "[Mm]argin"}, 
							{"credit rating", "[Cc]redit [Rr]at(?:ing|e)"}, 
							{"prepay rate", "[Pp]repay [Rr]ate"},
							{"ARM", "ARM"},
							{"FRM", "FRM"}
				}) {
					string phrase = kvp.Key;
					Regex regex = new Regex(kvp.Value);
					
					Assert.AreEqual (2, lines.FindAll ((string s) => regex.IsMatch (s)).Count, 
					                 this.Runner.ExtendedMessage ().WithMessages ("Failed to parse " + phrase, "Expected exactly two transactions with " + phrase + " field"));
				}
			}
		}
		
		[Test()]
		public void TestNewOutputFormat ()
		{
			using(TempFileWrapper inFile = TestHelper.ExtractResourceToTempFile("AssignmentTests.Resources.t3-loantype.in")) {
				this.Runner.StartApp(new string[] {"-csv", inFile});
				this.Runner.WriteInputLine("");
				
				List<string> lines = this.Runner.GetOutputLines ();
				lines.RemoveAll((string s) => new Regex ("^\\s*$").IsMatch (s));
				
				List<string> expectedLines = TestHelper.ExtractResourceToLineArray("AssignmentTests.Resources.t3-loantype-csv.out");
				
				Assert.AreEqual (expectedLines.Count, lines.Count, this.Runner.ExtendedMessage ().WithMessage ("Unexpected number of lines in output"));
				for(int i = 0; i < lines.Count; i++) {
					Assert.AreEqual (expectedLines[i], lines[i], this.Runner.ExtendedMessage ().WithMessage ("Improper line in output"));
				}
			}
		}
		
		[Test()]
		public void TestNewOutputErrorFormat ()
		{
			using(TempFileWrapper inFile = TestHelper.ExtractResourceToTempFile("AssignmentTests.Resources.t3-zeroerror.in")) {
				this.Runner.StartApp(new string[] {"-csv", inFile});
				this.Runner.WriteInputLine("");
				
				List<string> lines = this.Runner.GetOutputLines ();
				lines.RemoveAll((string s) => new Regex ("^\\s*$").IsMatch (s));
				
				Assert.AreEqual (1, lines.Count, this.Runner.ExtendedMessage ().WithMessage ("Unexpected number of lines in output"));
				Assert.IsTrue ((new Regex("^[Ee]rror in loan[: ]+ABC123,(.*)$")).IsMatch (lines[0]), this.Runner.ExtendedMessage ().WithMessage ("Improper line in output"));
			}
		}
		
		[Test()]
		public void TestLoadingViaInterface ()
		{
			Assembly appAssembly = this.Runner.LoadApplicationAssembly ();
			
			dynamic loader = null;
			try {
				loader = TestHelper.CreateInstanceOfImplementationOfTypeFromAssembly ("ITransactionLoader", appAssembly);
			} catch (Exception e) {
				Assert.Ignore ("Failed to find implementation of IRangeLoader:\n" + e.Message);
			}
			
			using (TempFileWrapper inFile = TestHelper.ExtractResourceToTempFileWithName ("AssignmentTests.Resources.t1.in", "sample range.txt")) {
				dynamic transactions = TestHelper.InvokeDynamicMethod(loader, "GetTransactionsFromFiles", new List<string> () {inFile.ToString()});
				
				Assert.IsNotNull (transactions, "Expected non-null transaction set");
				Assert.AreEqual (3, transactions.Count, "Expected three transactions to load");
				
				foreach(dynamic transaction in transactions) {
					dynamic transactionName = TestHelper.InvokeDynamicMethod (transaction, "Name");
					Assert.IsTrue ((new Regex("^[ABC]$")).IsMatch (transactionName), "Expected transaction name to be A, B, or C");
				}
			}
		}
	}
}


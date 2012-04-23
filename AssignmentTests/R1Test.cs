using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AssignmentTests
{
	[TestFixture()]
	public class R1Test
	{
		public static int PROCESS_WAIT_TIME = 1000;
		
		private AppRunner Runner {get; set;}
		
		[TestFixtureSetUp()]
		public void TestFixtureSetUp ()
		{
			// Get app name under test
			Console.Write ("R1 executable under test: ");
			string appName = Console.ReadLine ();
			Console.WriteLine ();
			
			this.Runner = new AppRunner(appName);
		}
		
		[TearDown()]
		public void TearDown ()
		{
			this.Runner.KillApp ();
		}
		
		[Test()]
		public void TestTests ()
		{
			Assert.IsTrue (true, "testing framework isn't feeling well today");
		}
		
		[Test()]
		public void TestProcessLaunches ()
		{
			this.Runner.StartApp (null);
			Assert.IsTrue (this.Runner.IsRunning(), "Application did not launch properly.");
		}
		
		[Test()]
		public void TestEmpty ()
		{
			string tempFile = TestHelper.ExtractResourceToTempFile ("AssignmentTests.Resources.empty.txt");
			
			this.Runner.StartApp (new string[] {tempFile});
			
			this.Runner.WriteInputLine ("");
			
			List<String> lines = this.Runner.GetOutputLines ();
			Assert.AreEqual (1, lines.Count, "Unexpected number of lines in output");
			
			Regex firstLineRegex = new Regex("^Range Name: .*$");
			Assert.IsTrue (firstLineRegex.Matches(lines[0]).Count > 0, "First line of text did not match");
			
			File.Delete (tempFile);
			
			Assert.IsTrue (this.Runner.StopApp(PROCESS_WAIT_TIME), "Application did not stop in time");
		}
		
		[Test()]
		public void TestFull ()
		{
			string tempFile = TestHelper.ExtractResourceToTempFile ("AssignmentTests.Resources.r1-full.in");
			
			this.Runner.StartApp (new string[] {tempFile});
			
			this.Runner.WriteInputLine ("");
			
			List<String> lines = this.Runner.GetOutputLines ();
			Assert.AreEqual (5, lines.Count, "Unexpected number of lines in output");
			
			// TODO match the output more closely
			
			File.Delete (tempFile);
			
			Assert.IsTrue (this.Runner.StopApp(PROCESS_WAIT_TIME), "Application did not stop in time");
		}
	}
}


using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AssignmentTests
{
	[TestFixture()]
	public class ATest
	{
		public static int PROCESS_WAIT_TIME = 1000;
		
		public static string AppName {get; set;}
		
		protected AppRunner Runner {get; set;}
		
		[TestFixtureSetUp()]
		public void TestFixtureSetUp ()
		{
			// Get app name under test
			if(AppName == null) {
				Console.Write ("Executable under test: ");
				AppName = Console.ReadLine ();
				Console.WriteLine ();
			}
			
			this.Runner = new AppRunner(AppName);
		}
		
		[TearDown()]
		public void TearDown ()
		{
			Assert.IsTrue (this.Runner.StopApp(PROCESS_WAIT_TIME), "Application did not stop in time");
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
			this.Runner.WriteInputLine ("");
		}
	}
}


using System;
using NUnit.Framework;

namespace AssignmentTests
{
	[TestFixture()]
	public class R1Test
	{
		private AppRunner Runner {get; set;}
		
		[TestFixtureSetUp()]
		public void TestFixtureSetUp ()
		{
			// Get app name under test
			Console.Write ("Name of executable under test: ");
			string appName = Console.ReadLine ();
			Console.WriteLine ();
			
			this.Runner = new AppRunner(appName);
		}
		
		[TearDown()]
		public void TearDown ()
		{
			this.Runner.StopApp ();
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
	}
}


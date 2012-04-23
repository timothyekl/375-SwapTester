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
			this.Runner = new AppRunner("SwapAssignment1.exe");
		}
		
		[SetUp()]
		public void SetUp ()
		{
			this.Runner.StartApp (null);
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
	}
}


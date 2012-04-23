using System;
using System.Reflection;
using NUnit;

namespace AssignmentTests
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			NUnit.ConsoleRunner.Runner.Main(new string[] {
		       Assembly.GetExecutingAssembly().Location 
		    });
		}
	}
}

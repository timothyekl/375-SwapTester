using System;

namespace Assignment2
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			// Get app name under test
			Console.Write ("Name of executable under test: ");
			string exeName = Console.ReadLine ();
			Console.WriteLine ();
			
			// Build app runner
			AppRunner runner = new AppRunner(exeName);
			
			// test code
			runner.StartApp (null);
		}
	}
}

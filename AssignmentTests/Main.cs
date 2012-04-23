using System;
using System.Reflection;
using NUnit;

namespace AssignmentTests
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			// gah! ugly
			char type = 'x';
			int number = 0;
			
			while(type == 'x' || number == 0) {
				System.Console.WriteLine ("Enter code of test suite to run");
				System.Console.Write ("(Rx:range, Tx:trans; x = assignment #): ");
				string code = System.Console.ReadLine ();
				
				if(code.Length != 2) continue;
				
				type = code[0];
				if(type != 'R' && type != 'A') {type = 'x'; continue;}
				
				number = Int32.Parse (code.Substring(1));
				if(number < 1 || number > 9) {number = 0; continue;}
			}
			
			NUnit.ConsoleRunner.Runner.Main(new string[] {
				"-run:AssignmentTests." + type + number + "Test",
		        Assembly.GetExecutingAssembly().Location
		    });
		}
	}
}

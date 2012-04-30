using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit;

namespace AssignmentTests
{
	class MainClass
	{
		public static readonly string VERSION = "Swap3-pre1"; 
		
		public static void Main (string[] args)
		{
			System.Console.WriteLine ("Launching swap tester version: " + VERSION);
			
			// gah! ugly
			char type = 'x';
			int number = -1;
			
			while(type == 'x' || number == -1) {
				System.Console.WriteLine ("Enter code of test suite to run");
				System.Console.Write ("(Rx:range, Tx:trans; x = optional asst. #): ");
				string code = System.Console.ReadLine ();
				
				if(code.Length != 1 && code.Length != 2) continue;
				
				type = code[0];
				if(type != 'R' && type != 'T') {type = 'x'; continue;}
				
				if(code.Length == 1) {
					number = 0;
				} else {
					number = Int32.Parse (code.Substring(1));
					if(number < 1 || number > 9) {number = -1; continue;}
				}
			}
			
			string runString = "";
			if(number == 0) {
				List<string> packageStrings = new List<string>();
				for(int i = 1; i <= 2; i++) {
					packageStrings.Add("AssignmentTests." + type + i + "Test");
				}
				runString = String.Join(",", packageStrings.ToArray());
			} else {
				runString = "AssignmentTests." + type + number + "Test";
			}
			NUnit.ConsoleRunner.Runner.Main(new string[] {
				"-run:" + runString,
		        Assembly.GetExecutingAssembly().Location
		    });
		}
	}
}

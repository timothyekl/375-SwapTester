using System;
using System.Collections.Generic;

namespace AssignmentTests
{
	public class ExtendedMessage
	{
		public List<string> Input {get; set;}
		public List<string> Output {get; set;}
		public string Arguments {get; set;}
		
		private List<string> Messages {get; set;}
		
		public ExtendedMessage (params string[] args)
			: this(new string[] {}, new List<string> (), new List<string> (), args) { }
		
		public ExtendedMessage (List<string> input, List<string> output, params string[] args)
			: this(new string[] {}, input, output, args) { }
		
		public ExtendedMessage (string[] cliArgs, List<string> input, List<string> output, params string[] args)
			: this(string.Join (" ", cliArgs), input, output, args) { }
		
		public ExtendedMessage (string cliArgs, List<string> input, List<string> output, params string[] args)
		{
			this.Arguments = cliArgs;
			this.Input = input;
			this.Output = output;
			this.Messages = new List<string>(args);
		}
		
		public override string ToString ()
		{
			string result = "";
			foreach(string message in this.Messages)
			{
				result += message + "\n";
			}
			result += "\n";
			
			if(this.Input != null)
			{
				result += "\n\nInput:\n";
				foreach(string message in this.Input)
				{
					result += message + "\n";
				}
				result += "\n";
			}
			
			if(this.Output != null)
			{
				result += "\n\nOutput:\n";
				foreach(string message in this.Output)
				{
					result += message + "\n";
				}
				result += "\n";
			}
			
			return result;
		}
		
		public static implicit operator string(ExtendedMessage em) {
			return em.ToString ();
		}
	}
}


using System;
using System.Collections.Generic;
using System.IO;

namespace AssignmentTests
{
	public class ExtendedMessage : ICloneable
	{
		public List<string> Input {get; set;}
		public List<string> Output {get; set;}
		public string[] Arguments {get; set;}
		
		private List<string> Messages {get; set;}
		
		// Constructors
		
		public ExtendedMessage (params string[] args)
			: this(new string[] {}, new List<string> (), new List<string> (), args) { }
		
		public ExtendedMessage (List<string> input, List<string> output, params string[] args)
			: this(new string[] {}, input, output, args) { }
		
		public ExtendedMessage (string[] cliArgs, List<string> input, List<string> output, params string[] args)
		{
			this.Arguments = cliArgs;
			this.Input = input;
			this.Output = output;
			this.Messages = new List<string>(args);
		}
		
		// Fluent methods
		
		public ExtendedMessage WithMessage(string msg)
		{
			this.Messages.Add (msg);
			return this;
		}
		
		public ExtendedMessage WithMessages(params string[] msgs)
		{
			this.Messages.AddRange (msgs);
			return this;
		}
		
		// Stringifying
		
		public override string ToString ()
		{
			string result = "";
			foreach(string message in this.Messages)
			{
				result += message + "\n";
			}
			result += "\n";
			
			result += "CLI Arguments: " + TestHelper.JoinQuoted (this.Arguments) + "\n\n";
			
			foreach(string cliArg in this.Arguments)
			{
				if(File.Exists (cliArg))
				{
					result += "File contents: " + cliArg + "\n";
					
					string[] fileContents = File.ReadAllLines (cliArg);
					foreach(string fileLine in fileContents)
					{
						result += "> " + fileLine + "\n";
					}
					
					result += "\n";
				}
			}
			
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
		
		public object Clone ()
		{
			return new ExtendedMessage (this.Arguments, this.Input, this.Output, this.Messages.ToArray ());
		}
	}
}


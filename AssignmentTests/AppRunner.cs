using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Timers;

namespace AssignmentTests
{
	/**
	 * Class responsible for encapsulating process launches, I/O, and exits.
	 */
	public class AppRunner
	{
		string ApplicationName {get; set;}
		Process _process;
		ExtendedMessage _extendedMessage;
		
		public static Int32 PROCESS_TIMEOUT = 2000;
		
		/**
		 * Build a new application launcher with the given app path
		 */
		public AppRunner (string appName)
		{
			this.ApplicationName = appName;
			
			if(!File.Exists(this.ApplicationName)) {
				throw new Exception("App file '" + this.ApplicationName + "' does not exist");
			}
			
			this.ConfigureProcess ();
		}
		
		/**
		 * Whether or not the process managed by this runner is running.
		 */
		public bool IsRunning() {
			if(_process == null) return false;
			try {
				if(_process.HasExited) return false;
				return true;
			} catch(Exception) {
				return false;
			}
		}
		
		/**
		 * Launch this runner's process.
		 * 
		 * @param args Arguments for process to run; joined & quoted as necessary. Pass null for no args
		 * @param timeout Maximum length process can run. Pass 0 for no timeout
		 */
		public void StartApp(string[] args, Int32 timeout) {
			if(this.IsRunning ()) return;
			this.ConfigureProcess ();
			
			// Concatenate arguments
			string argStr = TestHelper.JoinQuoted (args);
			_process.StartInfo.Arguments = argStr;
			_extendedMessage.Arguments = args;
			
			_process.Start ();
			
			if(timeout > 0) {
				Timer timer = new Timer();
				timer.Elapsed += this.AppTimeoutHandler;
				timer.Interval = timeout;
				timer.AutoReset = false; // only run once
				timer.Start();
			}
		}
		
		public void StartApp(string[] args) {
			this.StartApp(args, PROCESS_TIMEOUT);
		}
		
		public bool StopApp(Int32 waitTime) {
			if(!this.IsRunning()) return true;
			
			_process.StandardOutput.ReadToEnd ();
			_process.StandardError.ReadToEnd ();
			return _process.WaitForExit (waitTime);
		}
		
		/**
		 * Kill this runner's process.
		 */
		public void KillApp() {
			if(!this.IsRunning ()) return;
			
			_process.Kill ();
			_process = null;
		}
		
		public void WriteInputLine (string line) {
			if(!this.IsRunning ()) return;
			
			_process.StandardInput.WriteLine (line);
			_process.StandardInput.Flush ();
			
			_extendedMessage.Input.Add (line);
		}
		
		public List<String> GetOutputLines () {
			if(!this.IsRunning ()) return null;
			
			string line;
			List<String> lines = new List<String>();
			while((line = _process.StandardOutput.ReadLine ()) != null) {
				lines.Add (line);
			}
			
			_extendedMessage.Output.AddRange (lines);
			
			return lines;
		}
		
		public ExtendedMessage ExtendedMessage () {
			return (ExtendedMessage) (_extendedMessage.Clone ());
		}
		
		// Configuration for system process execution
		private void ConfigureProcess() {
			_process = new Process();
			_process.StartInfo.FileName = this.ApplicationName;
			_process.StartInfo.UseShellExecute = false;
			_process.StartInfo.RedirectStandardError = true;
			_process.StartInfo.RedirectStandardInput = true;
			_process.StartInfo.RedirectStandardOutput = true;
			_process.Exited += this.AppStopHandler;
			
			_extendedMessage = new ExtendedMessage();
		}
		
		// Event handler for process exit
		private void AppStopHandler(object sender, EventArgs e) {
			_process = null;
		}
		
		private void AppTimeoutHandler(object sender, EventArgs e) {
			this.KillApp();
		}
		
		// Reflection helpers
		public Assembly LoadApplicationAssembly ()
		{
			try
			{
				return Assembly.LoadFrom (this.ApplicationName);
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}


using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace AssignmentTests
{
	/**
	 * Class responsible for encapsulating process launches, I/O, and exits.
	 */
	public class AppRunner
	{
		string ApplicationName {get; set;}
		Process _process;
		
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
		 * Launch this runner's process. Pass list of arguments to pass through to command; pass null for empty arg string.
		 */
		public void StartApp(string[] args) {
			if(this.IsRunning ()) return;
			this.ConfigureProcess ();
			
			// Concatenate arguments
			string argStr = "";
			if(args != null) {
				foreach(string arg in args) {
					if(arg.IndexOf(' ') != -1) {
						argStr += '"' + arg + '"';
					} else {
						argStr += arg;
					}
					argStr += " ";
				}
			}
			_process.StartInfo.Arguments = argStr;
			
			_process.Start ();
		}
		
		public bool StopApp(Int32 waitTime) {
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
		}
		
		public List<String> GetOutputLines () {
			if(!this.IsRunning ()) return null;
			
			string line;
			List<String> lines = new List<String>();
			while((line = _process.StandardOutput.ReadLine ()) != null) {
				lines.Add (line);
			}
			return lines;
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
		}
		
		// Event handler for process exit
		private void AppStopHandler(object sender, EventArgs e) {
			_process = null;
		}
	}
}


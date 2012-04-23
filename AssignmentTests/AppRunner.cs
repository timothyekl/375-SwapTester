using System;
using System.IO;
using System.Diagnostics;
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
			
			_process = new Process();
			_process.StartInfo.FileName = this.ApplicationName;
			_process.StartInfo.UseShellExecute = false;
			_process.StartInfo.RedirectStandardError = true;
			_process.StartInfo.RedirectStandardInput = true;
			_process.StartInfo.RedirectStandardOutput = true;
			_process.Exited += this.AppStopHandler;
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
		public void StartApp(List<string> args) {
			if(this.IsRunning ()) return;
			
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
		
		/**
		 * Kill this runner's process.
		 */
		public void StopApp() {
			if(!this.IsRunning ()) return;
			
			_process.Kill ();
			_process = null;
		}
		
		// Event handler for process exit
		private void AppStopHandler(object sender, EventArgs e) {
			_process = null;
		}
	}
}


#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#endregion

namespace LifegameGame
{
	/// <summary>
	/// The main class.
	/// </summary>
	public static class Program
	{
		private static Game1 game;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			using (game = new Game1(new LaunchArgment()))
			{
				game.Run();
			}
		}

		public static void SetTrace()
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			Trace.AutoFlush = true;
			Trace.Listeners.Add(new ConsoleTraceListener());
			Trace.Listeners.Add(new TextWriterTraceListener(System.IO.File.Open("log.txt", System.IO.FileMode.Create, System.IO.FileAccess.Write)));
			Trace.WriteLine("Launch");
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Trace.WriteLine(e.ExceptionObject);
		}
	}
}

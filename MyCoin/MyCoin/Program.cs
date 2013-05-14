using System;
using System.Net.Sockets;
using System.Diagnostics;

namespace MyCoin
{
	class MainClass
	{

		public static void Main (string[] args)
		{
			_Log("test");
			Process.GetCurrentProcess().WaitForExit();
		}

		private static void _Log(string message)
		{
			string myDate;
			myDate = DateTime.Now.ToString("MM/dd/yyyy");
			Console.WriteLine("{0} {1} | {2} | {3}" + Environment.NewLine,"[Server]",DateTime.Now.ToShortTimeString(),myDate,message);
		}
		
	}
}

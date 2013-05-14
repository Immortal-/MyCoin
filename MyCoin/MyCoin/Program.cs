using System;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace MyCoin
{
	class MainClass
	{
		private static int _port = 2565;
		private static IPAddress _ip = Dns.GetHostEntry (Dns.GetHostName ()).AddressList[0];
		private static IPEndPoint _ipend = new IPEndPoint(_ip, 0);

		public static void Main (string[] args)
		{
			TcpListener server = null;
			try{

				server = new TcpListener(_ip,_port);
				server.Start();
				Byte[] bytes = new Byte[256];
				String data = null;

				while(true)
				{
					_Log ("Server initilized on port: {0}", _port);
					TcpClient client = server.AcceptTcpClient();
					_Log ("Client connected!");
					data = null;
					NetworkStream stream = client.GetStream();
					int i;

					while((i=stream.Read(bytes, 0, bytes.Length))!=0)
					{
						data = Encoding.ASCII.GetString(bytes, 0, i);
						_Log("Recived: {0}",data);
						data = data.ToUpper();
						byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
						stream.Write(msg, 0, msg.Length);
						_Log("Sent: {0}",data);
					}
					client.Close();
				}

			}catch(SocketException e)
			{
				_Log(e);
			} 
			finally
			{
				server.Stop();
			}

			Process.GetCurrentProcess().WaitForExit();
		}

		//OVERLOAD: used for displaying socket exceptions to the console
		private static void _Log(SocketException e)
		{
			string myDate;
			myDate = DateTime.Now.ToString("MM/dd/yyyy");
			Console.WriteLine("{0} {1} | {2} | {3}" + Environment.NewLine,"[Error]",DateTime.Now.ToShortTimeString(),myDate,e);
		}
		//Used to display text to the console
		private static void _Log(string message)
		{
			string myDate;
			myDate = DateTime.Now.ToString("MM/dd/yyyy");
			Console.WriteLine("{0} {1} | {2} | {3}" + Environment.NewLine,"[Server]",DateTime.Now.ToShortTimeString(),myDate,message);
		}
		//For displaying to the console like the console would but with time and date
		private static void _Log(string message , object arg1)
		{
			string myDate;
			myDate = DateTime.Now.ToString("MM/dd/yyyy");
			string _message = message.Replace("{0}",arg1.ToString());
			Console.WriteLine("{0} {1} | {2} | {3}" + Environment.NewLine,"[Server]",DateTime.Now.ToShortTimeString(),myDate,_message);
		}
		
	}
}

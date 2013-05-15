using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MyCoin_Client
{
	class MainClass
	{
		private static int _port = 2565;
		private static IPAddress _ip = IPAddress.Parse("127.0.0.1");	

		public static void Main (string[] args)
		{
		Begin:
			string message = Console.ReadLine();
			Send (message);
			switch(message){
			case "diff":
				string diffaculty = Recive();
				_Log(diffaculty);
				break;
			}

			goto Begin;
			//_Log(Recive());
		}
		private static string Recive()
		{
			Byte[] _bytes = new Byte[256];
			try{
				TcpClient client = new TcpClient();
				client.Connect(_ip,_port);
				
				NetworkStream stream = client.GetStream();
				String responseData = String.Empty;
				Int32 bytes = stream.Read(_bytes, 0, 256);
				responseData = Encoding.ASCII.GetString(_bytes, 0, bytes);
				stream.Dispose();         
				client.Close();  

				return responseData;     
			}catch(SocketException e){
				_Log (e);
			}
			return "Error!";
		}
		private static void Send(string message)
		{
			Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);     
			try{
				TcpClient client = new TcpClient();
				client.Connect(_ip,_port);

				NetworkStream stream = client.GetStream();
				stream.Write(data,0,data.Length);
				_Log ("Sent: {0}",message);
				stream.Dispose();
				client.Close();

			}catch(SocketException e){
				_Log (e);
			}
		}

		//OVERLOAD: used for displaying socket exceptions to the console
		private static void _Log(Exception e)
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
			Console.WriteLine("{0} {1} | {2} | {3}" + Environment.NewLine,"[Client]",DateTime.Now.ToShortTimeString(),myDate,message);
		}
		//For displaying to the console like the console would but with time and date
		private static void _Log(string message , object arg1)
		{
			string myDate;
			myDate = DateTime.Now.ToString("MM/dd/yyyy");
			string _message = message.Replace("{0}",arg1.ToString());
			Console.WriteLine("{0} {1} | {2} | {3}" + Environment.NewLine,"[Client]",DateTime.Now.ToShortTimeString(),myDate,_message);
		}
	}
}

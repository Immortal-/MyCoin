using System;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Text;

namespace MyCoin
{
	class MainClass
	{
		private static int _port = 2565;
		private static IPAddress _ip = IPAddress.Any;
		private static int DIFF = 5;

		public static void Main (string[] args)
		{
			TcpListener server = null;
			TcpClient client = null;

			try{

				server = new TcpListener(_ip,_port);
				server.Start();
				Byte[] bytes = new Byte[256];
				String data = null;

				while(true)
				{
					_Log ("Server waiting on port: {0}", _port);
					 client = server.AcceptTcpClient();
					_Log ("Client connected!");
					data = null;
					NetworkStream stream = client.GetStream();
					int i;

					while (client != null && ((i=stream.Read(bytes, 0, bytes.Length))!=0))
					{
						data = Encoding.ASCII.GetString(bytes, 0, i);
						_Log("Recived: {0}",data);
						data = data.ToUpper();

						switch(data){
						 case "DISC":
							try{
							_Log ("Client Disconnected!");
							client.Close();
							client = null;
							}catch(Exception e){
								_Log (e);
							}
							break;
						case "RBOOT":
							server.Stop();
							System.Threading.Thread.Sleep(2000);
							server.Start();
							break;
						case "DIFF":
							stream.Flush();
							byte _dif = Convert.ToByte(DIFF);
							stream.WriteByte(_dif);
							_Log("Diff: {0}",DIFF);
							break;
						case "SMT":
							string key = data.Split(':')[1];

							break;
						}

					//	byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
						//stream.Write(msg, 0, msg.Length);
						//_Log("Sent: {0}",data);
					}
				}

			}catch(Exception e)
			{
				_Log(e);
			}

			Process.GetCurrentProcess().WaitForExit();
		}
		
		//OVERLOAD: used for displaying socket exceptions to the console
		private static void _Log(Exception e)
		{
			string myDate;
			myDate = DateTime.Now.ToString("MM/dd/yyyy");
			Console.WriteLine("{0} {1} | {2} | Message: {3} Inner Exception: {4}" + Environment.NewLine,"[Error]",DateTime.Now.ToShortTimeString(),myDate,e.Message,e.InnerException);
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

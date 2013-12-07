using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LifegameGame
{
	public struct ConnectionInfo
	{
		public bool IsHost;
		public System.Net.IPAddress TargetIP;
		public int Port;
	}

	public class NetworkPlayer : Player
	{
		TcpClient socket;
		NetworkStream stream;
		ConnectionInfo Info;
		TcpListener listener;


		public NetworkPlayer(GameBoard board, GridState side, ConnectionInfo info)
			: base(board, side)
		{
			Info = info;
		}

		public void Connect()
		{
			if (Info.IsHost)
			{
				Trace.WriteLine("I am host");
				listener = new TcpListener(System.Net.IPAddress.IPv6Any, Info.Port);
				listener.Server.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, 0);
				listener.Start();
				Trace.WriteLine("accepting...");
				//Trace.WriteLine(listener.LocalEndpoint.ToString());
				socket = listener.AcceptTcpClient();
				Trace.WriteLine("accepted!");
				Trace.WriteLine(socket.Client.RemoteEndPoint.ToString());
				//listener.Stop();
			}
			else
			{
				Trace.WriteLine("I am client");
				socket = new TcpClient(AddressFamily.InterNetwork);
				
				socket.Connect(Info.TargetIP, Info.Port);
				Trace.WriteLine("connected!");
			}
			stream = socket.GetStream();

		}

		bool reading;
		Task<Point> task;

		public override bool Update()
		{
			if (!reading)
			{
				reading = true;
				task = Task.Factory.StartNew<Point>(() =>
					{
						int x, y;
						var reader = new System.IO.BinaryReader(stream);
						x = reader.ReadInt32();
						y = reader.ReadInt32();
						return new Point(x, y);
					});
			}
			if (task.Status == TaskStatus.RanToCompletion)
			{
				reading = false;
				Play(task.Result);
				return true;
			}
			else if (task.Status == TaskStatus.Faulted)
			{
				throw task.Exception;
			}

			return false;
		}

		public void Send(Point p)
		{
			var writer = new System.IO.BinaryWriter(stream);
			writer.Write(p.X);
			writer.Write(p.Y);
			writer.Flush();
		}

		public override void Dispose()
		{
			base.Dispose();
			stream.Close();
			socket.Close();
			if(listener != null)
			{ 
				listener.Stop();
			}
		}
	}
}

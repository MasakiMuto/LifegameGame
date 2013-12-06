using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

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
		

		Socket socket;

		public NetworkPlayer(GameBoard board, GridState side, ConnectionInfo info)
			: base(board, side)
		{
			if (info.IsHost)
			{
				var s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket = s.Accept();
			}
			else
			{
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.Connect(info.TargetIP, info.Port);
			}
		}

		public override bool Update()
		{
			throw new Exception();
		}
	}
}

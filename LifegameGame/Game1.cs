using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LifegameGame
{
	public enum PlayerType
	{
		Human,
		MinMaxAI,
		AlphaBetaAI,
		//MonteCarloAI,
	}

	[Serializable]
	public class LaunchArgment
	{
		public PlayerType Player1 { get; set; }
		public PlayerType Player2 { get; set; }
		public int BoardSize { get; set; }
		public int ThinkDepth1 { get; set; }
		public int ThinkDepth2 { get; set; }

		public LaunchArgment()
		{
			Player1 = PlayerType.Human;
			Player2 = PlayerType.AlphaBetaAI;
			BoardSize = 8;
			ThinkDepth1 = 3;
			ThinkDepth2 = 3;
		}
	}

	public class Game1 : IDisposable
	{
		GameBoard board;
		Player[] players;
		int current;

		bool isExit;
		CellState winner;
		LaunchArgment Argment;

		public Game1(LaunchArgment arg)
		{
			Argment = arg;
			//SaveArgment();
			LoadArgment();
			Init();
			while (!isExit)
			{
				Update();
			}
		}

		void LoadArgment()
		{
			if (!File.Exists("config.xml"))
			{
				return;
			}
			try
			{
				var s = new XmlSerializer(typeof(LaunchArgment));
				using (var str = File.OpenRead("config.xml"))
				{
					Argment = s.Deserialize(str) as LaunchArgment;
				}
			}
			catch { }
		}

		void SaveArgment()
		{
			var s = new XmlSerializer(typeof(LaunchArgment));
			FileStream str = null;
			try
			{
				str = File.OpenWrite("config.xml");
				s.Serialize(str, Argment);
				str.Close();
				str.Dispose();
				str = null;
			}
			catch { }
			finally
			{
				if (str != null)
				{
					str.Dispose();
				}
			}
		}

		
		void Init()
		{
			Trace.WriteLine("Start Game");
			board = new LifegameBoard(Argment.BoardSize);
			players = new Player[2];
			
			var a = new[]
				{
					Tuple.Create(Argment.Player1, Argment.ThinkDepth1, CellState.White),
					Tuple.Create(Argment.Player2, Argment.ThinkDepth2, CellState.Black)
				};
			for (int i = 0; i < 2; i++)
			{
				switch (a[i].Item1)
				{
					case PlayerType.Human:
						players[i] = new HumanPlayer(board, a[i].Item3);
						break;
					case PlayerType.MinMaxAI:
						players[i] = new MinMaxPlayer(board, a[i].Item3, a[i].Item2);
						break;
					case PlayerType.AlphaBetaAI:
						players[i] = new AlphaBetaPlayer(board, a[i].Item3, a[i].Item2);
						break;
					//case PlayerType.MonteCarloAI:
					//	players[i] = new MonteCarloPlayer(board, a[i].Item3);
					//	break;
					default:
						break;
				}
			}

			isExit = false;
			winner = CellState.None;
			current = 0;
		}


		protected void Update()
		{
			if (isExit)
			{
				return;
			}
			Console.Write(board.ToString());
			Console.WriteLine(players[current].Side.ToString() + (players[current].Side == CellState.White ? "(o)" : "(x)") + " Turn");
			if (players[current].Update())
			{
				current = 1 - current;
				if (board.IsExit())
				{
					isExit = true;
					winner = board.GetWinner();
					Console.WriteLine(winner + " Win.");
				}
			}
			
		}


		public void Dispose()
		{
			board.Dispose();
			board = null;
			players[0].Dispose();
			players[1].Dispose();
		}

	}
}

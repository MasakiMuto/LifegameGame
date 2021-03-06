﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LifegameGame
{
	public enum PlayerType
	{
		Human,
		MinMaxAI,
		AlphaBetaAI,
		//MonteCarloAI,
	}

	public class LaunchArgment
	{
		public bool IsOnline { get; set; }
		public LifegameGame.ConnectionInfo Connection;
		public PlayerType Player1 { get; set; }
		public PlayerType Player2 { get; set; }
		public int BoardSize { get; set; }
		public int ThinkDepth1 { get; set; }
		public int ThinkDepth2 { get; set; }

		public LaunchArgment()
		{
			IsOnline = false;
			Player1 = PlayerType.Human;
			Player2 = PlayerType.Human;
			BoardSize = 8;
			ThinkDepth1 = 3;
			ThinkDepth2 = 3;
		}
	}

	public class Game1 : Game, IDisposable
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		GameBoard board;
		Player[] players;
		int current;

		bool isExit;
		CellState winner;
		readonly LaunchArgment Argment;

		public Game1(LaunchArgment arg)
			: base()
		{
			graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferHeight = 600,
				PreferredBackBufferWidth = 800,
				SynchronizeWithVerticalRetrace = true,
			};
			Content.RootDirectory = "Content";
			Argment = arg;
			this.IsMouseVisible = true;

		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			Init();
		}
		Task connectTask;

		void Init()
		{
			Trace.WriteLine("Start Game");
			Window.Title = "Lifegame Game";
			board = new LifegameBoard(spriteBatch, Argment.BoardSize);
			players = new Player[2];
			if (Argment.IsOnline)
			{
				if (Argment.Connection.IsHost)
				{
					players[0] = new HumanPlayer(board, CellState.White);
					players[1] = new NetworkPlayer(board, CellState.Black, Argment.Connection);
					players[0].OnPlay += (players[1] as NetworkPlayer).Send;
				}
				else
				{
					players[0] = new NetworkPlayer(board, CellState.White, Argment.Connection);
					players[1] = new HumanPlayer(board, CellState.Black);
					players[1].OnPlay += (players[0] as NetworkPlayer).Send;
				}
			}
			else
			{
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
			}
			var net = players.OfType<NetworkPlayer>().FirstOrDefault();
			if (net != null)
			{
				connectTask = Task.Factory.StartNew(net.Connect);
				Window.Title = "Now Connecting...";
			}

			isExit = false;
			winner = CellState.None;
			current = 0;
		}


		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				if (Argment.IsOnline)
				{
					Exit();
					return;
				}
				Init();
			}
			if (connectTask != null && !connectTask.IsCompleted)
			{
				return;
			}
			if (isExit)
			{
				return;
			}
			try
			{
				if (players[current].Update())
				{
					current = 1 - current;
					if (board.IsExit())
					{
						isExit = true;
						winner = board.GetWinner();
						Window.Title = "Winner:" + winner.ToString();
						foreach (var item in players.OfType<AIPlayerBase>())
						{
							Trace.WriteLine(string.Format("AI Player {0} Evaled Total {1} Nodes", item.Side, item.TotalEvalCount));
						}
					}
				}
			}
			catch (Exception e)
			{
				Trace.WriteLine(e);
				Exit();
			}


			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			board.Draw();
			base.Draw(gameTime);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			board.Dispose();
			board = null;
			players[0].Dispose();
			players[1].Dispose();
		}

	}
}

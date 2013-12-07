using System;
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
		AI,
	}

	public class LaunchArgment
	{
		public bool IsOnline;
		public LifegameGame.ConnectionInfo Connection;
		public PlayerType Player1, Player2;

		public LaunchArgment()
		{
			IsOnline = false;
			Player1 = PlayerType.Human;
			Player2 = PlayerType.Human;
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
		GridState winner;
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
			Window.Title = "Lifegame Game";
			board = new LifegameBoard(spriteBatch);
			players = new Player[2];
			if (Argment.IsOnline)
			{
				if (Argment.Connection.IsHost)
				{
					players[0] = new HumanPlayer(board, GridState.White);
					players[1] = new NetworkPlayer(board, GridState.Black, Argment.Connection);
					players[0].OnPlay += (players[1] as NetworkPlayer).Send;
				}
				else
				{
					players[0] = new NetworkPlayer(board, GridState.White, Argment.Connection);
					players[1] = new HumanPlayer(board, GridState.Black);
					players[1].OnPlay += (players[0] as NetworkPlayer).Send;
				}
			}
			else
			{
				players[0] = new HumanPlayer(board, GridState.White);
				players[1] = new HumanPlayer(board, GridState.Black);
			}
			var net = players.OfType<NetworkPlayer>().FirstOrDefault();
			if (net != null)
			{
				connectTask = Task.Factory.StartNew(net.Connect);
				Window.Title = "Now Connecting...";
			}

			isExit = false;
			winner = GridState.None;
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
		}

	}
}

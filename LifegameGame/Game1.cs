#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace LifegameGame
{
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		GameBoard board;
		Player[] players;
		int current;

		bool isExit;
		GridState winner;

		public Game1()
			: base()
		{
			graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferHeight = 600,
				PreferredBackBufferWidth = 800,
				SynchronizeWithVerticalRetrace = true,
			};
			Content.RootDirectory = "Content";
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

		void Init()
		{
			Window.Title = "Lifegame Game";
			board = new LifegameBoard(spriteBatch);
			players = new Player[]
			{
				new HumanPlayer(board, GridState.White),
				new HumanPlayer(board, GridState.Black),
			};
			isExit = false;
			winner = GridState.None;
			current = 0;
		}


		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Init();
			}
			if (isExit)
			{
				return;
			}
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

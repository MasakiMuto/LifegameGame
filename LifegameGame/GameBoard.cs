using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LifegameGame
{

	public enum GridState
	{
		None,
		Black,
		White
	}

	public struct Point
	{
		public int X;
		public int Y;

		public Point(int x, int y)
		{
			X = x;
			Y = y;
		}
	}

	public abstract class GameBoard : IDisposable
	{
		public readonly int Size;
		GridState[][,] grids;
		int current;

		protected GridState[,] CurrentState { get { return grids[current]; } }
		protected GridState[,] NextState { get { return grids[1 - current]; } }

		Texture2D point;
		const int DisplaySize = 600;
		readonly float GridSize;

		protected readonly SpriteBatch Sprite;

		public Point Cursor { get; set; }
		public GridState CursorSide { get; set; }

		public GameBoard(int size, SpriteBatch sprite)
		{
			Size = size;
			grids = new GridState[2][,];
			grids[0] = new GridState[Size, Size];
			grids[1] = new GridState[Size, Size];
			current = 0;
			Sprite = sprite;
			point = new Texture2D(sprite.GraphicsDevice, 1, 1);
			point.SetData(new[] { Color.White });
			GridSize = DisplaySize / Size;
			Init();
		}

		protected abstract void Init();

		protected void InitialPut(GridState state, Point p)
		{
			CurrentState[p.X, p.Y] = state;
		}

		/// <summary>
		/// 次状態ボードの更新
		/// </summary>
		/// <param name="state"></param>
		/// <param name="p"></param>
		protected void Put(GridState state, Point p)
		{
			NextState[p.X, p.Y] = state;
		}

		/// <summary>
		/// 状態を更新
		/// </summary>
		/// <param name="played"></param>
		protected void Update(Point played)
		{
			CopyBoard();
			UpdateByRule(played);
			current = 1 - current;
		}

		void CopyBoard()
		{
			for (int i = 0; i < Size; i++)
			{
				for (int j = 0; j < Size; j++)
				{
					NextState[i, j] = CurrentState[i, j];	
				}
			}
		}

		public abstract bool IsExit();
		public abstract GridState GetWinner(); 

		protected abstract void UpdateByRule(Point played);

		/// <summary>
		/// 現状態ボードの更新
		/// </summary>
		/// <param name="state"></param>
		/// <param name="p"></param>
		public void Play(GridState state, Point p)
		{
			CurrentState[p.X, p.Y] = state;
			Update(p);
		}

		public bool CanPlay(Point p)
		{
			return CurrentState[p.X, p.Y] == GridState.None;
		}

		/// <summary>
		/// 座標からグリッド位置に変換する。不正位置ならPoint(-1, -1)
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public Point GetGrid(Vector2 pos)
		{
			pos = Vector2.Clamp(pos, Vector2.Zero, new Vector2(DisplaySize - 1, DisplaySize - 1));
			return new Point((int)(pos.X / GridSize), (int)(pos.Y / GridSize));
		}

		public void Draw()
		{
			Sprite.Begin();
			Sprite.Draw(point, new Rectangle(0, 0, DisplaySize, DisplaySize), Color.Black);
			for (int i = 0; i < Size; i++)
			{
				for (int j = 0; j < Size; j++)
				{
					var lt = GetPosition(new Point(i, j));
					var rb = GetPosition(new Point(i + 1, j + 1));
					Sprite.Draw(point, new Rectangle((int)lt.X, (int)lt.Y, (int)GridSize - 1, (int)GridSize - 1), Color.DarkGreen);
					var state = CurrentState[i, j];
					if (state != GridState.None)
					{
						DrawStone(new Point(i, j), state == GridState.Black ? Color.Black : Color.White);
					}
				}
			}
			if (CursorSide != GridState.None && CanPlay(Cursor))
			{
				DrawStone(Cursor, (CursorSide == GridState.Black ? Color.Black : Color.White) * .5f);
			}
			Sprite.End();
		}

		void DrawStone(Point p, Color color)
		{
			var lt = GetPosition(p);
			Sprite.Draw(point, new Rectangle((int)lt.X + 8, (int)lt.Y + 8, (int)GridSize - 16, (int)GridSize - 16), color);
		}

		Vector2 GetPosition(Point p)
		{
			return new Vector2(GridSize * p.X, GridSize * p.Y);
		}

		~GameBoard()
		{
			Dispose();
		}


		public void Dispose()
		{
			if (point != null)
			{
				point.Dispose();
				point = null;
				GC.SuppressFinalize(this);
			}
		}
	}

}

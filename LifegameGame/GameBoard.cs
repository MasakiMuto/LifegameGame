using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LifegameGame
{

	public enum CellState
	{
		None = 0,
		Black = -1,
		White = 1
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
		CellState[][,] cells;
		int current;

		public CellState[,] CurrentState { get { return cells[current]; } }
		protected CellState[,] NextState { get { return cells[1 - current]; } }

		Texture2D point;
		const int DisplaySize = 600;
		readonly float CellSize;

		protected readonly SpriteBatch Sprite;

		public Point Cursor { get; set; }
		public CellState CursorSide { get; set; }

		public GameBoard(int size, SpriteBatch sprite)
		{
			Size = size;
			cells = new CellState[2][,];
			cells[0] = new CellState[Size, Size];
			cells[1] = new CellState[Size, Size];
			current = 0;
			Sprite = sprite;
			point = new Texture2D(sprite.GraphicsDevice, 1, 1);
			point.SetData(new[] { Color.White });
			CellSize = DisplaySize / Size;
			Init();
		}

		protected abstract void Init();

		protected void InitialPut(CellState state, Point p)
		{
			CurrentState[p.X, p.Y] = state;
		}

		/// <summary>
		/// 次状態ボードの更新
		/// </summary>
		/// <param name="state"></param>
		/// <param name="p"></param>
		protected void Put(CellState state, Point p)
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
		public abstract CellState GetWinner(); 

		protected abstract void UpdateByRule(Point played);

		/// <summary>
		/// 現状態ボードの更新
		/// </summary>
		/// <param name="state"></param>
		/// <param name="p"></param>
		public void Play(CellState state, Point p)
		{
			CurrentState[p.X, p.Y] = state;
			Update(p);
		}

		public bool CanPlay(Point p)
		{
			return CurrentState[p.X, p.Y] == CellState.None;
		}

		/// <summary>
		/// 座標からグリッド位置に変換する。不正位置ならPoint(-1, -1)
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public Point GetCellPosition(Vector2 pos)
		{
			pos = Vector2.Clamp(pos, Vector2.Zero, new Vector2(DisplaySize - 1, DisplaySize - 1));
			return new Point((int)(pos.X / CellSize), (int)(pos.Y / CellSize));
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
					Sprite.Draw(point, new Rectangle((int)lt.X, (int)lt.Y, (int)CellSize - 1, (int)CellSize - 1), Color.DarkGreen);
					var state = CurrentState[i, j];
					if (state != CellState.None)
					{
						DrawStone(new Point(i, j), state == CellState.Black ? Color.Black : Color.White);
					}
				}
			}
			if (CursorSide != CellState.None && CanPlay(Cursor))
			{
				DrawStone(Cursor, (CursorSide == CellState.Black ? Color.Black : Color.White) * .5f);
			}
			Sprite.End();
		}

		void DrawStone(Point p, Color color)
		{
			var lt = GetPosition(p);
			Sprite.Draw(point, new Rectangle((int)lt.X + 8, (int)lt.Y + 8, (int)CellSize - 16, (int)CellSize - 16), color);
		}

		Vector2 GetPosition(Point p)
		{
			return new Vector2(CellSize * p.X, CellSize * p.Y);
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

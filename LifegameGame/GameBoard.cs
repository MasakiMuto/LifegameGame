using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LifegameGame
{
	public enum CellState : sbyte
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

		public override bool Equals(object obj)
		{
			if (obj is Point)
			{
				Point p = (Point)obj;
				return X == p.X && Y == p.Y;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return X + Y * 256;
		}

		public override string ToString()
		{
			return String.Format("({0},{1})", X, Y);
		}
	}

	public class BoardInstance
	{
		public CellState[,] Cells;
		readonly int Size;
		
		public BoardInstance(BoardInstance original)
		{
			Cells = original.Cells.Clone() as CellState[,];
			
			//Cells = original.Cells.Clone() as CellState[][,];
			Size = Cells.GetLength(0);
		}

		public BoardInstance(int size)
		{
			Cells = new CellState[size, size];
			Size = size;
		}

		public override string ToString()
		{
			var s = new StringBuilder(Size * Size);
			for (int i = 0; i < Size; i++)
			{
				for (int j = 0; j < Size; j++)
				{
					var a = Cells[i, j];
					s.Append(a == CellState.None ? '.' : (a == CellState.White ? 'o' : 'x'));
					//s.Append('|');
				}
				s.AppendLine();
				//s.AppendLine(new String('-', Size * 2));
			}
			s.AppendLine("#####");
			return s.ToString();
		}

		
	}

	public abstract class GameBoard : IDisposable
	{
		public readonly int Size;
		BoardInstance originalBoard;

		public BoardInstance PlayingBoard { get; set; }

		CellState[,] nextCells;

		public CellState[,] CurrentState { get { return PlayingBoard.Cells; } }
		protected CellState[,] NextState { get { return nextCells; } }

		Texture2D pixel;
		const int DisplaySize = 600;
		readonly float CellSize;

		protected readonly SpriteBatch Sprite;

		public Point Cursor { get; set; }
		public CellState CursorSide { get; set; }

		public const float WinnerBonus = 1000;

		public GameBoard(int size, SpriteBatch sprite)
		{
			Size = size;
			nextCells = new CellState[Size, Size];
			originalBoard = new BoardInstance(size);
			PlayingBoard = originalBoard;
			Sprite = sprite;
			pixel = new Texture2D(sprite.GraphicsDevice, 1, 1);
			pixel.SetData(new[] { Color.White });
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
			var c = PlayingBoard.Cells;
			PlayingBoard.Cells = nextCells;
			nextCells = c;
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


		public BoardInstance VirtualPlay(CellState state, Point p)
		{
			var board = new BoardInstance(PlayingBoard);
			PlayingBoard = board;
			Play(state, p);
			return board;
		}

		public void SetBoardOrigin()
		{
			PlayingBoard = originalBoard;
		}


		/// <summary>
		/// 座標からグリッド位置に変換する。不正位置ならPoint(-1, -1)
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public Point GetCellPosition(Vector2 pos)
		{
			pos = Vector2.Clamp(pos, Vector2.Zero, new Vector2(CellSize * Size - 1));
			return new Point((int)(pos.X / CellSize), (int)(pos.Y / CellSize));
		}

		public void Draw()
		{
			Sprite.Begin();
			Sprite.Draw(pixel, new Rectangle(0, 0, DisplaySize, DisplaySize), Color.Black);
			for (int i = 0; i < Size; i++)
			{
				for (int j = 0; j < Size; j++)
				{
					var lt = GetPosition(new Point(i, j));
					var rb = GetPosition(new Point(i + 1, j + 1));
					Sprite.Draw(pixel, new Rectangle((int)lt.X, (int)lt.Y, (int)CellSize - 1, (int)CellSize - 1), Color.DarkGreen);
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
			Sprite.Draw(pixel, new Rectangle((int)lt.X + 8, (int)lt.Y + 8, (int)CellSize - 16, (int)CellSize - 16), color);
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
			if (pixel != null)
			{
				pixel.Dispose();
				pixel = null;
				GC.SuppressFinalize(this);
			}
		}

		public override string ToString()
		{
			return PlayingBoard.ToString();
		}

		public abstract float EvalScore();
	}

}

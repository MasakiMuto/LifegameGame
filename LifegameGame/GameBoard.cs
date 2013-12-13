using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
			Console.WriteLine(" " + new String(Enumerable.Range(0, Size).Select(x=>(char)('a'+x)).ToArray()));
			for (int i = 0; i < Size; i++)
			{
				s.Append(i.ToString("X"));
					
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

		public const float WinnerBonus = 1000;

		public GameBoard(int size)
		{
			Size = size;
			nextCells = new CellState[Size, Size];
			originalBoard = new BoardInstance(size);
			PlayingBoard = originalBoard;
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

			return p.X >= 0 && p.Y >= 0 && p.X < Size && p.Y < Size && CurrentState[p.X, p.Y] == CellState.None;
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

	
		~GameBoard()
		{
			Dispose();
		}


		public void Dispose()
		{
		
		}

		public override string ToString()
		{
			return PlayingBoard.ToString();
		}

		public abstract float EvalScore();
	}

}

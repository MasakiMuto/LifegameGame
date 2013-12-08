using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace LifegameGame
{
	public struct CellCount
	{
		public readonly int White;
		public readonly int Black;

		public CellCount(int w, int b)
		{
			White = w;
			Black = b;
		}

		public int Same(CellState side)
		{
			if (side == CellState.White) return White;
			else return Black;
		}

		public int Anti(CellState side)
		{
			if (side == CellState.White) return Black;
			else return White;
		}
	}


	public class LifegameBoard : GameBoard
	{

		public LifegameBoard(SpriteBatch sprite, int size)
			: base(size, sprite)
		{

		}

		protected override void Init()
		{
			for (int i = 0; i < Size - 1; i++)
			{
				Func<int, CellState> s = x => i % 2 == x ? CellState.Black : CellState.White;
				InitialPut(s(0), new Point(i, 0));
				InitialPut(s(1), new Point(Size - 1, i));
				InitialPut(s(0), new Point(Size - i - 1, Size - 1));
				InitialPut(s(1), new Point(0, Size - i - 1));
			}
			InitialPut(CellState.White, new Point(Size / 2, Size / 2));
			InitialPut(CellState.White, new Point(Size / 2 - 1, Size / 2 - 1));
			InitialPut(CellState.Black, new Point(Size / 2 - 1, Size / 2));
			InitialPut(CellState.Black, new Point(Size / 2, Size / 2 - 1));

		}

		bool IsFixCell(Point p)
		{
			return false;
			//return p.X == 0 || p.X == Size - 1 || p.Y == 0 || p.Y == Size - 1;
		}

		protected override void UpdateByRule(Point played)
		{
			var pos = new[]
			{
				played,
				new Point(played.X - 1, played.Y),
				new Point(played.X + 1, played.Y),
				new Point(played.X, played.Y - 1),
				new Point(played.X, played.Y + 1),

				new Point(played.X - 1, played.Y - 1),
				new Point(played.X - 1, played.Y + 1),
				new Point(played.X + 1, played.Y - 1),
				new Point(played.X + 1, played.Y + 1),
			};
			foreach (var item in pos)
			{
				if (!IsFixCell(item))
				{
					var s = Eval(item);
					if (s != CellState.None)
					{
						Put(s, item);
					}
				}
			}
		}

		
		public CellCount CountAround(Point p)
		{
			int b = 0, w = 0;
			var pos = new[]
			{
				new Point(p.X + 1, p.Y),
				new Point(p.X - 1, p.Y),
				new Point(p.X, p.Y + 1),
				new Point(p.X, p.Y - 1)
			};
			foreach (var item in pos)
			{
				if (item.X < 0 || item.Y < 0 || item.X >= Size || item.Y >= Size)
				{
					continue;
				}
				switch (CurrentState[item.X, item.Y])
				{
					case CellState.Black:
						b++;
						break;
					case CellState.White:
						w++;
						break;
					default:
						break;
				}
			}
			return new CellCount(w, b);
		}

		CellState Eval(Point p)
		{
			var count = CountAround(p);
			if (count.Black == 3 || (count.Black == 2 && count.White < 2) || count.White == 4)
			{
				return CellState.Black;
			}
			if (count.White == 3 || (count.White == 2 && count.Black < 2) || count.Black == 4)
			{
				return CellState.White;
			}
			return CellState.None;
		}


		public override bool IsExit()
		{
			return CurrentState.OfType<CellState>().All(x => x != CellState.None);
		}

		public override CellState GetWinner()
		{
			var s = CurrentState.OfType<CellState>();
			int b = s.Count(x => x == CellState.Black);
			int w = s.Count(x => x == CellState.White);
			if (b > w)
			{
				return CellState.Black;
			}
			else if (w > b)
			{
				return CellState.White;
			}
			else
			{
				return CellState.None;
			}
		}
	}
}

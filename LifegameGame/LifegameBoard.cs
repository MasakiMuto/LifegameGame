using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace LifegameGame
{
	public class LifegameBoard : GameBoard
	{

		public LifegameBoard(SpriteBatch sprite)
			: base(10, sprite)
		{

		}

		protected override void Init()
		{
			for (int i = 0; i < Size - 1; i++)
			{
				Func<int, GridState> s = x => i % 2 == x ? GridState.Black : GridState.White;
				InitialPut(s(0), new Point(i, 0));
				InitialPut(s(1), new Point(Size - 1, i));
				InitialPut(s(0), new Point(Size - i - 1, Size - 1));
				InitialPut(s(1), new Point(0, Size - i - 1));
			}
			InitialPut(GridState.White, new Point(Size / 2, Size / 2));
			InitialPut(GridState.White, new Point(Size / 2 - 1, Size / 2 - 1));
			InitialPut(GridState.Black, new Point(Size / 2 - 1, Size / 2));
			InitialPut(GridState.Black, new Point(Size / 2, Size / 2 - 1));

		}

		bool IsFixGrid(Point p)
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
				if (!IsFixGrid(item))
				{
					var s = Eval(item);
					if (s != GridState.None)
					{
						Put(s, item);
					}
				}
			}
		}

		GridState Eval(Point p)
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
					case GridState.Black:
						b++;
						break;
					case GridState.White:
						w++;
						break;
					default:
						break;
				} 
			}
			if (b == 3 || (b == 2 && w < 2))
			{
				return GridState.Black;
			}
			if (w == 3 || (w == 2 && b < 2))
			{
				return GridState.White;
			}
			if (b == 4)
			{
				return GridState.White;
			}
			if (w == 4)
			{
				return GridState.Black;
			}
			return GridState.None;
		}


		public override bool IsExit()
		{
			return CurrentState.OfType<GridState>().All(x => x != GridState.None);
		}

		public override GridState GetWinner()
		{
			var s = CurrentState.OfType<GridState>();
			int b = s.Count(x => x == GridState.Black);
			int w = s.Count(x => x == GridState.White);
			if (b > w)
			{
				return GridState.Black;
			}
			else if (w > b)
			{
				return GridState.White;
			}
			else
			{
				return GridState.None;
			}
		}
	}
}

﻿using System;
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

		/// <summary>
		/// 白が優位なら+、黒が優位なら-
		/// </summary>
		/// <returns></returns>
		public override float EvalScore()
		{
			float p = 0;

			for (int x = 0; x < Size; x++)
			{
				for (int y = 0; y < Size; y++)
				{
					var state = CurrentState[x, y];
					var count = CountAround(new Point(x, y));
					p += EvalSide(state, count, CellState.White) - EvalSide(state, count, CellState.Black);
				}
			}
			if (IsExit())
			{
				switch (GetWinner())
				{
					case CellState.None:
						break;
					case CellState.Black:
						p -= WinnerBonus;
						break;
					case CellState.White:
						p += WinnerBonus;
						break;
					default:
						break;
				}
			}
			return p;
		}

		/// <summary>
		/// セルをあるサイドから見た時の有利値
		/// </summary>
		/// <param name="state"></param>
		/// <param name="count"></param>
		/// <param name="side"></param>
		/// <returns></returns>
		float EvalSide(CellState state, CellCount count, CellState side)
		{
			float p = 0;
			const float CanGet = .5f;
			const float CanKill = .8f;
			const float PlayGet = .3f;
			const float PlayKill = .6f;
			int same = count.Same(side);
			int anti = count.Anti(side);
			if (state == side)
			{
				p++;
			}
			else
			{
				if (anti == 4 || (anti <= 1 && (same == 2 || same == 3)))
				{
					p += (state == CellState.None) ? CanGet : CanKill;
				}
				else if (anti <= 1 && same == 1)
				{
					p += (state == CellState.None) ? PlayGet : PlayKill;
				}
			}
			return p;
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

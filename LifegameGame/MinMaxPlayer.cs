using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LifegameGame
{
	public class MinMaxPlayer : Player
	{


		public MinMaxPlayer(GameBoard board, CellState side)
			: base(board, side)
		{

		}

		public override bool Update()
		{
			Play(Think(1));
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="depth">ツリーを展開する最大深さ</param>
		/// <returns></returns>
		Point Think(int depth)
		{
			var watch = new Stopwatch();
			watch.Start();

			watch.Stop();
			Trace.WriteLine("MinMax Thinking Time=" + watch.Elapsed.ToString());
			throw new Exception();
		}

		/// <summary>
		/// 自分優位なら+、敵優位なら-
		/// </summary>
		/// <param name="board"></param>
		/// <returns></returns>
		float EvalBoard(LifegameBoard board)
		{
			float p = 0;
			
			for (int x = 0; x < board.Size; x++)
			{
				for (int y = 0; y < board.Size; y++)
				{
					var state = board.CurrentState[x, y];
					var count = board.CountAround(new Point(x, y));
					p += EvalSide(state, count, CellState.White) - EvalSide(state, count, CellState.Black);
				}
			}
			if (this.Side == CellState.Black)
			{
				p *= -1;
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
	}
}

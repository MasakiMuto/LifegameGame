using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LifegameGame
{
	using PointScoreDictionary = Dictionary<Point, float>;
	public class AlphaBetaPlayer : AIPlayerBase
	{
		readonly int ThinkDepth;

		public AlphaBetaPlayer(GameBoard board, CellState side, int thinkDepth)
			: base(board, side)
		{
			ThinkDepth = thinkDepth;
		}

		protected override Point Think()
		{
			var list = new PointScoreDictionary();

			float alpha = float.MinValue;
			foreach (var p in GetPlayablePoints())
			{
				float score = EvalPosition(Board.PlayingBoard, p, ThinkDepth, this.Side, alpha);
				list[p] = score;
				if (score > alpha)
				{
					alpha = score;
				}
				Board.SetBoardOrigin();
			}

			var res = GetMaxPoint(list);

			return res.Key;
		}


		protected virtual float EvalPosition(BoardInstance board, Point p, int depth, CellState side, float alphaBeta)
		{
			EvalCount++;
			Board.PlayingBoard = board;
			Debug.Assert(Board.CanPlay(p));
			var next = Board.VirtualPlay(side, p);
			if (Board.IsExit())
			{
				if (Board.GetWinner() == this.Side)
				{
					Trace.WriteLine("I Win");
					return GameBoard.WinnerBonus;
				}
				else
				{
					return -GameBoard.WinnerBonus;
				}
			}
			if (depth == 1)
			{
				return Board.EvalScore() * (int)(this.Side);
			}
			else
			{
				bool isMax = this.Side == side;
				float current = (isMax ? float.MinValue : float.MaxValue);
				foreach (var item in GetPlayablePoints())
				{
					float score = EvalPosition(next, item, depth - 1, side == CellState.White ? CellState.Black : CellState.White, current);

					if ((isMax && score > current) || (!isMax && score < current))
					{
						current = score;
					}
					if (!isMax && current > alphaBeta)
					{
						return current;
					}
					if (isMax && current < alphaBeta)
					{
						return current;
					}
				}
				return current;
			}
		}

	}
}

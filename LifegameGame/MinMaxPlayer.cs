using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LifegameGame
{
	using PointScoreDictionary = Dictionary<Point, float>;
	public class MinMaxPlayer : AIPlayerBase
	{
		protected readonly int ThinkDepth;

		public MinMaxPlayer(GameBoard board, CellState side, int thinkDepth)
			: base(board, side)
		{
			ThinkDepth = thinkDepth;
		}

		

		/// <summary>
		/// 最善手を考える
		/// </summary>
		/// <param name="depth">ツリーを展開する最大深さ</param>
		/// <returns></returns>
		protected override Point Think()
		{
			var currentScore = Board.EvalScore();
			var list = new PointScoreDictionary();

			foreach (var p in GetPlayablePoints())
			{
				list[p] = EvalPosition(Board.PlayingBoard, p, ThinkDepth, this.Side);
				Board.SetBoardOrigin();
			}

			var res = (this.Side == CellState.White) ? GetMaxPoint(list) : GetMinPoint(list);

			return res.Key;
		}

		

		/// <summary>
		/// そこに打った時の評価値の計算
		/// </summary>
		/// <param name="p"></param>
		/// <param name="isMin">minを求めるならtrue</param>
		protected virtual float EvalPosition(BoardInstance board, Point p, int depth, CellState side)
		{
			EvalCount++;
			Board.PlayingBoard = board;
			Debug.Assert(Board.CanPlay(p));
			var next = Board.VirtualPlay(side, p);
			if (Board.IsExit())
			{
				if (Board.GetWinner() == CellState.White)
				{
					return GameBoard.WinnerBonus;
				}
				else
				{
					return -GameBoard.WinnerBonus;
				}
			}
			if (depth == 1)
			{
				return Board.EvalScore();
			}
			else
			{
				return GetBestChild(next, depth, side);
			}
		}

		protected virtual float GetBestChild(BoardInstance next, int depth, CellState side)
		{
			List<float> scores = new List<float>();
			float current = (this.Side == side ? float.MinValue : float.MaxValue);
			foreach (var item in GetPlayablePoints())
			{
				//var b = Board.VirtualPlay(Side, item);
				//float score = Board.EvalScore();
				float score = (EvalPosition(next, item, depth - 1, side == CellState.White ? CellState.Black : CellState.White));
				if ((this.Side == side && score > current) || (this.Side != side && score < current))
				{
					current = score;
				}
			}
			return current;
		}

		

	}
}

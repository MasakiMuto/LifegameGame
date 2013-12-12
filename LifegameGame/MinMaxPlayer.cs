using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LifegameGame
{
	using PointScoreDictionary = Dictionary<Point, float>;
	public class MinMaxPlayer : Player
	{
		readonly int ThinkDepth;

		public MinMaxPlayer(GameBoard board, CellState side, int thinkDepth)
			: base(board, side)
		{
			ThinkDepth = thinkDepth;
		}

		public override bool Update()
		{
			Play(Think(ThinkDepth));
			return true;
		}

		/// <summary>
		/// 最善手を考える
		/// </summary>
		/// <param name="depth">ツリーを展開する最大深さ</param>
		/// <returns></returns>
		Point Think(int depth)
		{
			var currentScore = Board.EvalScore();
			var watch = new Stopwatch();
			watch.Start();

			var list = new PointScoreDictionary();

			foreach (var p in GetPlayablePoints())
			{
				if (Board.CanPlay(p))
				{
					list[p] = EvalPosition(Board.PlayingBoard, p, depth, this.Side);
					Board.SetBoardOrigin();
				}
			}

			var res = this.Side == CellState.White ? GetMaxPoint(list) : GetMinPoint(list);

			watch.Stop();
			Trace.WriteLine("MinMax Thinking Time=" + watch.Elapsed.ToString());
			return res.Key;
		}

		IEnumerable<Point> GetPlayablePoints()
		{
			return Enumerable.Range(0, Board.Size).SelectMany(x =>
				Enumerable.Range(0, Board.Size)
				.Select(y => new Point(x, y))
				.Where(p => Board.CanPlay(p))
				);
		}

		/// <summary>
		/// そこに打った時の評価値の計算
		/// </summary>
		/// <param name="p"></param>
		/// <param name="isMin">minを求めるならtrue</param>
		float EvalPosition(BoardInstance board, Point p, int depth, CellState side)
		{
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
				List<float> scores = new List<float>();
				foreach (var item in GetPlayablePoints())
				{
					//var b = Board.VirtualPlay(Side, item);
					//float score = Board.EvalScore();
					scores.Add(EvalPosition(next, item, depth - 1, side == CellState.White ? CellState.Black : CellState.White));
				}
				if (this.Side != side)
				{
					return scores.Min();
				}
				else
				{
					return scores.Max();
				}
			}
		}

		KeyValuePair<Point, float> GetMaxPoint(PointScoreDictionary dict)
		{
			KeyValuePair<Point, float> p = dict.First();
			foreach (var item in dict)
			{
				if (item.Value > p.Value)
				{
					p = item;
				}
			}
			return p;
		}

		KeyValuePair<Point, float> GetMinPoint(PointScoreDictionary dict)
		{
			KeyValuePair<Point, float> p = dict.First();
			foreach (var item in dict)
			{
				if (item.Value < p.Value)
				{
					p = item;
				}
			}
			return p;
		}

	}
}

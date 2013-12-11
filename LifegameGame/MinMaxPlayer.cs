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
					var b = Board.VirtualPlay(Side, p);
					float score = Board.EvalScore();
					//if (this.Side == CellState.Black)
					//{
					//	score *= -1;
					//}
					list[p] = score;
					Board.SetBoardOrigin();
				}
			}
			
			var res = this.Side == CellState.White ? GetMaxPoint(list) : GetMinPoint(list);

			watch.Stop();
			Trace.WriteLine("MinMax Thinking Time=" + watch.Elapsed.ToString());
			//Trace.WriteLine("I play " + res.Key.ToString());
			//Trace.WriteLine(String.Format("Score {0}→{1}", currentScore, res.Value));
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
		float EvalPosition(BoardInstance board, Point p, int depth)
		{
			Board.PlayingBoard = board;
			Debug.Assert(Board.CanPlay(p));
			if (depth == 1)
			{
				return Board.EvalScore();
			}
			else
			{

			}
			throw new NotImplementedException();
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

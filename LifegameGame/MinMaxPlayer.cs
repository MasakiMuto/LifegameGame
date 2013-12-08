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
		/// 
		/// </summary>
		/// <param name="depth">ツリーを展開する最大深さ</param>
		/// <returns></returns>
		Point Think(int depth)
		{
			var currentScore = Board.EvalScore();
			var watch = new Stopwatch();
			watch.Start();

			var list = new PointScoreDictionary();
			for (int i = 0; i < Board.Size; i++)
			{
				for (int j = 0; j < Board.Size; j++)
				{
					var p = new Point(i, j);
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
			}
			var res = this.Side == CellState.White ? GetMaxPoint(list) : GetMinPoint(list);

			watch.Stop();
			Trace.WriteLine("MinMax Thinking Time=" + watch.Elapsed.ToString());
			//Trace.WriteLine("I play " + res.Key.ToString());
			//Trace.WriteLine(String.Format("Score {0}→{1}", currentScore, res.Value));
			return res.Key;
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

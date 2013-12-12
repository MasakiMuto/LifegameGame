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

		TreeNode<float> tree;

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
			var list = new PointScoreDictionary();
			tree = new TreeNode<float>(0, null);
			foreach (var p in GetPlayablePoints())
			{
				list[p] = EvalPosition(Board.PlayingBoard, p, ThinkDepth, this.Side, tree);
				Board.SetBoardOrigin();
			}

			var res = GetMaxPoint(list);
			tree.Value = res.Value;
			System.IO.File.WriteAllText("tree.txt", tree.ToString());
			return res.Key;
		}



		/// <summary>
		/// そこに打った時の評価値の計算
		/// </summary>
		/// <param name="p"></param>
		/// <returns>自分にとって有利なほど+</returns>
		protected virtual float EvalPosition(BoardInstance board, Point p, int depth, CellState side, TreeNode<float> t)
		{
			var tr = t.AddChild(0);
			EvalCount++;
			Board.PlayingBoard = board;
			Debug.Assert(Board.CanPlay(p));
			var next = Board.VirtualPlay(side, p);
			if (Board.IsExit())
			{
				if (Board.GetWinner() == this.Side)
				{
					//Trace.WriteLine("I Win");
					return GameBoard.WinnerBonus;
				}
				else
				{
					return -GameBoard.WinnerBonus;
				}
			}
			if (depth == 1)
			{
				var s = Board.EvalScore() * (int)(this.Side);
				tr.Value = s;
				return s;
			}
			else
			{
				bool isMax = this.Side != side;
				float current = (isMax ? float.MinValue : float.MaxValue);
				foreach (var item in GetPlayablePoints())
				{
					float score = EvalPosition(next, item, depth - 1, side == CellState.White ? CellState.Black : CellState.White, tr);
					if ((isMax && score > current) || (!isMax && score < current))
					{
						current = score;
					}
				}
				tr.Value = current;
				return current;
			}
		}

	}
}

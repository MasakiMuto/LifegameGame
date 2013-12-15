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
			var tree = TreeNode.Create(null, 0f);
			tree = null;
			foreach (var p in GetPlayablePoints())
			{
				float score = EvalPosition(Board.PlayingBoard, p, ThinkDepth, this.Side, alpha, tree);
				list[p] = score;
				if (score > alpha)
				{
					alpha = score;
				}
				Board.SetBoardOrigin();
			}
			var res = GetMaxPoint(list);
			if (tree != null)
			{
				TreeNode.SetValue(tree, res.Value);
				System.IO.File.WriteAllText("tree.txt", tree.ToString());
			}

			return res.Key;
		}


		protected virtual float EvalPosition(BoardInstance board, Point p, int depth, CellState side, float alphaBeta, TreeNode<float> t)
		{
			EvalCount++;
			TotalEvalCount++;
			Board.PlayingBoard = board;
			Debug.Assert(Board.CanPlay(p));
			var next = Board.VirtualPlay(side, p);
			if (Board.IsExit())
			{
				if (Board.GetWinner() == this.Side)
				{
					TreeNode.AddChild(t, GameBoard.WinnerBonus);
					return GameBoard.WinnerBonus;
				}
				else
				{
					TreeNode.AddChild(t, -GameBoard.WinnerBonus);
					return -GameBoard.WinnerBonus;
				}
			}
			if (depth == 1)
			{
				var s = Board.EvalScore() * (int)(this.Side);
				TreeNode.AddChild(t, s);
				return s;
			}
			else
			{
				bool isMax = this.Side != side;
				float current = (isMax ? float.MinValue : float.MaxValue);
				var tr = TreeNode.AddChild(t, 0);
				foreach (var item in GetPlayablePoints())
				{
					float score = EvalPosition(next, item, depth - 1, GetAntiPlayer(side), current, tr);
					if ((isMax && score > current) || (!isMax && score < current))
					{
						current = score;
					}
					if (isMax && current >= alphaBeta)
					{
						break;
					}
					if (!isMax && current <= alphaBeta)
					{
						break;
					}
				}
				TreeNode.SetValue(tr, current);
				return current;
			}
		}

	}
}

﻿using System;
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
			var list = new PointScoreDictionary();
			foreach (var p in GetPlayablePoints().ToArray())
			{
				var score = EvalPosition(Board.PlayingBoard, p, ThinkDepth, this.Side);
				list[p] = score;
				Board.SetBoardOrigin();
			}

			var res = GetMaxPoint(list);
			return res.Key;
		}



		/// <summary>
		/// そこに打った時の評価値の計算
		/// </summary>
		/// <param name="p"></param>
		/// <returns>自分にとって有利なほど+</returns>
		protected virtual float EvalPosition(BoardInstance board, Point p, int depth, CellState side)
		{
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
				return s;
			}
			else
			{
				bool isMax = this.Side != side;
				float current = (isMax ? float.MinValue : float.MaxValue);
				
				foreach (var item in GetPlayablePoints().ToArray())
				{
					float score = EvalPosition(next, item, depth - 1, side == CellState.White ? CellState.Black : CellState.White);
					if ((isMax && score > current) || (!isMax && score < current))
					{
						current = score;
					}
				}
				return current;
			}
		}

	}
}

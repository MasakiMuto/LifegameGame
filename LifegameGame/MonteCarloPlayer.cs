using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LifegameGame
{
	public class MonteCarloPlayer : AIPlayerBase
	{
		Random random;

		struct NodeInfo
		{
			public int PlayOutCount;
			public int WinCount;

			public void Update(bool win)
			{
				PlayOutCount++;
				if (win)
				{
					WinCount++;
				}
			}
		}

		readonly int Limit = 10000;

		public MonteCarloPlayer(GameBoard board, CellState side)
			: base(board, side)
		{
			random = new Random();
		}

		protected override Point Think()
		{
			throw new NotImplementedException();
			int total = 0;
			Point[] playablePoints = GetPlayablePoints().ToArray();
			var dict = new Dictionary<Point, NodeInfo>();
			foreach (var item in playablePoints)
			{
				dict.Add(item, new NodeInfo());
				dict[item].Update(Playout(item));
				total++;
			}
			while (total < Limit)
			{
				
			}
		}

		bool Playout(Point p)
		{
			var origin = Board.PlayingBoard;
			var side = this.Side;
			Board.Play(side, p);
			while (!Board.IsExit())
			{
				side = GetAntiPlayer(side);
				var pt = GetRandomPlayablePoint();
				Board.Play(side, pt);
			}
			Board.PlayingBoard = origin;
			return Board.GetWinner() == this.Side;
		}

		Point GetRandomPlayablePoint()
		{
			Debug.Assert(!Board.IsExit());
			int x, y;
			do
			{
				x = random.Next(0, Board.Size);
				y = random.Next(0, Board.Size);
			} while (!Board.CanPlay(new Point(x, y)));
			return new Point(x, y);
		}

	}
}

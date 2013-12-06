using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LifegameGame
{
	public abstract class Player
	{
		protected readonly GameBoard Board;
		public readonly GridState Side;

		public Player(GameBoard board, GridState side)
		{
			if (side == GridState.None)
			{
				throw new Exception("Invalid Player Side");
			}
			Board = board;
			Side = side;
		}

		/// <summary>
		/// プレイしたらtrue
		/// </summary>
		/// <returns></returns>
		public abstract bool Update();

		protected void Play(Point p)
		{
			Board.Play(Side, p);
		}

	}
}

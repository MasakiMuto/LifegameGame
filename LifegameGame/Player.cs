﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LifegameGame
{
	public abstract class Player : IDisposable
	{
		protected readonly GameBoard Board;
		public readonly GridState Side;
		public event Action<Point> OnPlay;

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
			if(!Board.CanPlay(p))
			{
				throw new Exception(String.Format( "Cannot play on ({0},{1})", p.X, p.Y));
			}
			Board.Play(Side, p);
			if (OnPlay != null)
			{
				OnPlay(p);
			}
		}




		public virtual void Dispose()
		{
		}
	}
}

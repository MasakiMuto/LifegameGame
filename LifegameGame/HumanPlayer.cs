using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LifegameGame
{
	public class HumanPlayer : Player
	{

		bool lastPush;

		public HumanPlayer(GameBoard board, CellState side)
			: base(board, side)
		{

		}

		public override bool Update()
		{
			var input = Mouse.GetState();
			var p = Board.GetCellPosition(new Vector2(input.X, input.Y));
			Board.CursorSide = Side;
			Board.Cursor = p;
			if (input.LeftButton == ButtonState.Pressed)
			{
				lastPush = true;
			}
			if(lastPush && input.LeftButton == ButtonState.Released)
			{
				lastPush = false;
				if (Board.CanPlay(p))
				{
					Play(p);
					return true;
				}
			}
			return false;
		}


	}
}

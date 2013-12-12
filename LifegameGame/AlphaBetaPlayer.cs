using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LifegameGame
{
	public class AlphaBetaPlayer : MinMaxPlayer
	{
		public AlphaBetaPlayer(GameBoard board, CellState side, int thinkDepth)
			: base(board, side, thinkDepth)
		{

		}

	}
}

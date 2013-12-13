using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LifegameGame
{
	public class HumanPlayer : Player
	{

		public HumanPlayer(GameBoard board, CellState side)
			: base(board, side)
		{

		}

		public override bool Update()
		{
			Console.WriteLine("Where?");
			try
			{
				char c1, c2;
				c1 = Char.ToLower(Console.ReadKey().KeyChar);
				c2 = Console.ReadKey().KeyChar;
				int x = c1 - 'a';
				int y = int.Parse(c2.ToString(), System.Globalization.NumberStyles.HexNumber);
				Console.WriteLine();
				var p = new Point(x, y);
				if (Board.CanPlay(p))
				{
					Play(p);
					return true;
				}
			}
			catch { }
			Console.WriteLine("Cannot Play");
			return false;
		}


	}
}

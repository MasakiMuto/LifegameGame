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
				int x = Char.ToLower(Console.ReadKey().KeyChar) - 'a';
				int y = int.Parse(Console.ReadKey().KeyChar.ToString(), System.Globalization.NumberStyles.HexNumber);
				Console.ReadLine();
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

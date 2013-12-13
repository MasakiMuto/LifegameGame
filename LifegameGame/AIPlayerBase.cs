using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LifegameGame
{
	using PointScoreDictionary = Dictionary<Point, float>;
	public abstract class AIPlayerBase : Player
	{
		Task<Point> thinkTask;
		Stopwatch watch;
		protected int EvalCount;

		public AIPlayerBase(GameBoard board, CellState state)
			: base(board, state)
		{
			watch = new Stopwatch();
		}

		public override bool Update()
		{
			if (thinkTask == null)
			{
				EvalCount = 0;
				thinkTask = Task.Factory.StartNew<Point>(Think);
				watch.Start();
			}
			if (thinkTask.IsCompleted)
			{
				watch.Stop();
				Trace.WriteLine("ThinkingTime= " + watch.Elapsed.ToString());
				Trace.WriteLine(EvalCount.ToString() + " Nodes Evaled.");
				watch.Reset();
				Play(thinkTask.Result);
				thinkTask.Dispose();
				thinkTask = null;
				return true;
			}
			return false;
		}

		protected abstract Point Think();

		protected IEnumerable<Point> GetPlayablePoints()
		{
			return Enumerable.Range(0, Board.Size).SelectMany(x =>
				Enumerable.Range(0, Board.Size)
				.Select(y => new Point(x, y))
				.Where(p => Board.CanPlay(p))
				).ToArray();
		}

		protected KeyValuePair<Point, float> GetMaxPoint(PointScoreDictionary dict)
		{
			KeyValuePair<Point, float> p = dict.First();
			foreach (var item in dict)
			{
				if (item.Value > p.Value)
				{
					p = item;
				}
			}
			return p;
		}

		protected KeyValuePair<Point, float> GetMinPoint(PointScoreDictionary dict)
		{
			KeyValuePair<Point, float> p = dict.First();
			foreach (var item in dict)
			{
				if (item.Value < p.Value)
				{
					p = item;
				}
			}
			return p;
		}

		public CellState GetAntiPlayer(CellState state)
		{
			return state == CellState.White ? CellState.Black : CellState.White;
		}
	}


}

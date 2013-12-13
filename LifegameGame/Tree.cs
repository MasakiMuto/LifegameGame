using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LifegameGame
{
	public class TreeNode<T>
	{
		public T Value;
		public readonly List<TreeNode<T>> Children;
		public readonly TreeNode<T> Parent;
		public readonly int Level;
		public readonly TreeNode<T> Root;

		public TreeNode(T value, TreeNode<T> parent)
		{
			Parent = parent;
			Value = value;
			Children = new List<TreeNode<T>>();
			if (parent == null)
			{
				Level = 0;
				Root = this;
			}
			else
			{
				Level = parent.Level + 1;
				Root = parent.Root;
			}
		}

		public TreeNode<T> AddChild(T value)
		{
			var a = new TreeNode<T>(value, this);
			Children.Add(a);
			return a;
		}

		public override string ToString()
		{
			var s = new StringBuilder();
			s.Append(new string('\t', Level));
			s.AppendLine(Value.ToString());
			foreach (var item in Children)
			{
				s.Append(item.ToString());
			}
			return s.ToString();
		}

		
	}

	public static class TreeNode
	{
		public static TreeNode<T> AddChild<T>(TreeNode<T> parent, T value)
		{
			if (parent == null)
			{
				return null;
			}
			else
			{
				return parent.AddChild(value);
			}
		}

		public static void SetValue<T>(TreeNode<T> node, T value)
		{
			if (node != null)
			{
				node.Value = value;
			}
		}

		public static TreeNode<T> Create<T>(TreeNode<T> parent, T value)
		{
			return new TreeNode<T>(value, parent);
		}
	}
}

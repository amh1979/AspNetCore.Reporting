using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal struct PointInfo
	{
		public PointF Point;

		public int Index;

		public PointF[] Points;

		public GraphicsPath Path;

		public Direction Direction;

		public int GetNextIndex(Direction direction)
		{
			if (direction == Direction.Forward)
			{
				if (this.Index == this.Points.Length - 1)
				{
					return 0;
				}
				return this.Index + 1;
			}
			if (this.Index == 0)
			{
				return this.Points.Length - 1;
			}
			return this.Index - 1;
		}

		public PointInfo GetNextPoint(Direction direction)
		{
			PointInfo result = default(PointInfo);
			result.Index = this.GetNextIndex(direction);
			result.Points = this.Points;
			result.Point = result.Points[result.Index];
			result.Path = this.Path;
			result.Direction = direction;
			return result;
		}
	}
}

using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal sealed class AnchorModel
	{
		private Anchor _interface;

		private readonly int _row;

		private readonly int _column;

		private readonly double _offsetX;

		private readonly double _offsetY;

		public Anchor Interface
		{
			get
			{
				if (this._interface == null)
				{
					this._interface = new Anchor(this);
				}
				return this._interface;
			}
		}

		public int Row
		{
			get
			{
				return this._row;
			}
		}

		public int Column
		{
			get
			{
				return this._column;
			}
		}

		public double OffsetY
		{
			get
			{
				return this._offsetY;
			}
		}

		public double OffsetX
		{
			get
			{
				return this._offsetX;
			}
		}

		public AnchorModel(int row, int column, double offsetX, double offsetY)
		{
			if (row >= 0 && column >= 0)
			{
				AnchorModel.ValidateOffsetParam(offsetX);
				AnchorModel.ValidateOffsetParam(offsetY);
				this._row = row;
				this._column = column;
				this._offsetX = Math.Min(offsetX, 100.0);
				this._offsetX = Math.Max(this._offsetX, 0.0);
				this._offsetY = Math.Min(offsetY, 100.0);
				this._offsetY = Math.Max(this._offsetY, 0.0);
				return;
			}
			throw new FatalException();
		}

		private static void ValidateOffsetParam(double param)
		{
			if (!(param < 0.0) && !(param > 100.0))
			{
				return;
			}
			throw new FatalException();
		}
	}
}

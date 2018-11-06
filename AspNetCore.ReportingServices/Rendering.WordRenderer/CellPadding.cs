using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class CellPadding
	{
		private CellSpacingStruct m_topPadding;

		private CellSpacingStruct m_leftPadding;

		private CellSpacingStruct m_bottomPadding;

		private CellSpacingStruct m_rightPadding;

		private List<byte[]> m_sprms;

		private int m_runningSize;

		private int m_numColumns;

		private int m_adjustmentTop;

		private int m_adjustmentBottom;

		internal int HeightAdjustment
		{
			get
			{
				return this.m_adjustmentBottom + this.m_adjustmentTop;
			}
		}

		internal int SprmSize
		{
			get
			{
				return this.m_runningSize;
			}
		}

		internal CellPadding(int numColumns)
		{
			this.m_sprms = new List<byte[]>();
			this.m_runningSize = 0;
			this.m_adjustmentTop = 0;
			this.m_adjustmentBottom = 0;
			this.m_numColumns = numColumns;
		}

		internal void SetPaddingTop(int cellIndex, int twips)
		{
			if (this.m_topPadding == null)
			{
				this.m_topPadding = new CellSpacingStruct(CellSpacingStruct.Location.Top);
				this.m_topPadding.ItcFirst = cellIndex;
				this.m_topPadding.Width = twips;
				this.m_adjustmentTop = Math.Max(twips, this.m_adjustmentTop);
			}
			else if (this.m_topPadding.Width != twips)
			{
				this.m_topPadding.ItcLim = cellIndex;
				this.Commit(this.m_topPadding);
				this.m_topPadding.ItcFirst = cellIndex;
				this.m_topPadding.Width = twips;
				this.m_adjustmentTop = Math.Max(twips, this.m_adjustmentTop);
			}
		}

		internal void SetPaddingLeft(int cellIndex, int twips)
		{
			if (this.m_leftPadding == null)
			{
				this.m_leftPadding = new CellSpacingStruct(CellSpacingStruct.Location.Left);
				this.m_leftPadding.ItcFirst = cellIndex;
				this.m_leftPadding.Width = twips;
			}
			else if (this.m_leftPadding.Width != twips)
			{
				this.m_leftPadding.ItcLim = cellIndex;
				this.Commit(this.m_leftPadding);
				this.m_leftPadding.ItcFirst = cellIndex;
				this.m_leftPadding.Width = twips;
			}
		}

		internal void SetPaddingBottom(int cellIndex, int twips)
		{
			if (this.m_bottomPadding == null)
			{
				this.m_bottomPadding = new CellSpacingStruct(CellSpacingStruct.Location.Bottom);
				this.m_bottomPadding.ItcFirst = cellIndex;
				this.m_bottomPadding.Width = twips;
				this.m_adjustmentBottom = Math.Max(twips, this.m_adjustmentBottom);
			}
			else if (this.m_bottomPadding.Width != twips)
			{
				this.m_bottomPadding.ItcLim = cellIndex;
				this.Commit(this.m_bottomPadding);
				this.m_bottomPadding.ItcFirst = cellIndex;
				this.m_bottomPadding.Width = twips;
				this.m_adjustmentBottom = Math.Max(twips, this.m_adjustmentBottom);
			}
		}

		internal void SetPaddingRight(int cellIndex, int twips)
		{
			if (this.m_rightPadding == null)
			{
				this.m_rightPadding = new CellSpacingStruct(CellSpacingStruct.Location.Right);
				this.m_rightPadding.ItcFirst = cellIndex;
				this.m_rightPadding.Width = twips;
			}
			else if (this.m_rightPadding.Width != twips)
			{
				this.m_rightPadding.ItcLim = cellIndex;
				this.Commit(this.m_rightPadding);
				this.m_rightPadding.ItcFirst = cellIndex;
				this.m_rightPadding.Width = twips;
			}
		}

		internal byte[] ToByteArray()
		{
			byte[] array = new byte[this.m_runningSize];
			int num = 0;
			for (int i = 0; i < this.m_sprms.Count; i++)
			{
				byte[] array2 = this.m_sprms[i];
				Array.Copy(array2, 0, array, num, array2.Length);
				num += array2.Length;
			}
			return array;
		}

		private void Commit(CellSpacingStruct spacing)
		{
			byte[] array = spacing.ToByteArray();
			this.m_sprms.Add(array);
			this.m_runningSize += array.Length;
		}

		internal void Finish()
		{
			if (this.m_topPadding != null)
			{
				this.m_topPadding.ItcLim = this.m_numColumns;
				this.Commit(this.m_topPadding);
			}
			if (this.m_leftPadding != null)
			{
				this.m_leftPadding.ItcLim = this.m_numColumns;
				this.Commit(this.m_leftPadding);
			}
			if (this.m_bottomPadding != null)
			{
				this.m_bottomPadding.ItcLim = this.m_numColumns;
				this.Commit(this.m_bottomPadding);
			}
			if (this.m_rightPadding != null)
			{
				this.m_rightPadding.ItcLim = this.m_numColumns;
				this.Commit(this.m_rightPadding);
			}
		}

		internal void Reset()
		{
			this.m_rightPadding = null;
			this.m_leftPadding = null;
			this.m_topPadding = null;
			this.m_bottomPadding = null;
			this.m_sprms = new List<byte[]>();
			this.m_runningSize = 0;
			this.m_adjustmentTop = 0;
			this.m_adjustmentBottom = 0;
		}
	}
}

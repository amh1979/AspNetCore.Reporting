using AspNetCore.ReportingServices.Rendering.Utilities;
using System;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class CellBorderColor
	{
		private byte[] m_borderColorsTop;

		private byte[] m_borderColorsLeft;

		private byte[] m_borderColorsBottom;

		private byte[] m_borderColorsRight;

		internal int SprmSize
		{
			get
			{
				return (3 + this.m_borderColorsTop.Length) * 4;
			}
		}

		internal CellBorderColor(int numColumns)
		{
			this.m_borderColorsTop = new byte[4 * numColumns];
			this.m_borderColorsLeft = new byte[4 * numColumns];
			this.m_borderColorsBottom = new byte[4 * numColumns];
			this.m_borderColorsRight = new byte[4 * numColumns];
			this.InitColors(this.m_borderColorsTop);
			this.InitColors(this.m_borderColorsLeft);
			this.InitColors(this.m_borderColorsBottom);
			this.InitColors(this.m_borderColorsRight);
		}

		private void InitColors(byte[] borderColors)
		{
			for (int i = 3; i < borderColors.Length; i += 4)
			{
				borderColors[i] = 255;
			}
		}

		internal void Reset()
		{
			Array.Clear(this.m_borderColorsTop, 0, this.m_borderColorsTop.Length);
			Array.Clear(this.m_borderColorsLeft, 0, this.m_borderColorsLeft.Length);
			Array.Clear(this.m_borderColorsBottom, 0, this.m_borderColorsBottom.Length);
			Array.Clear(this.m_borderColorsRight, 0, this.m_borderColorsRight.Length);
			this.InitColors(this.m_borderColorsTop);
			this.InitColors(this.m_borderColorsLeft);
			this.InitColors(this.m_borderColorsBottom);
			this.InitColors(this.m_borderColorsRight);
		}

		internal void SetColor(TableData.Positions position, int cellIndex, int ico24)
		{
			int offset = cellIndex * 4;
			switch (position)
			{
			case TableData.Positions.Bottom:
				LittleEndian.PutInt(this.m_borderColorsBottom, offset, ico24);
				break;
			case TableData.Positions.Left:
				LittleEndian.PutInt(this.m_borderColorsLeft, offset, ico24);
				break;
			case TableData.Positions.Right:
				LittleEndian.PutInt(this.m_borderColorsRight, offset, ico24);
				break;
			case TableData.Positions.Top:
				LittleEndian.PutInt(this.m_borderColorsTop, offset, ico24);
				break;
			}
		}

		internal byte[] ToByteArray()
		{
			int num = 0;
			byte[] array = new byte[this.SprmSize];
			num += Word97Writer.AddSprm(array, num, 54810, 0, this.m_borderColorsTop);
			num += Word97Writer.AddSprm(array, num, 54811, 0, this.m_borderColorsLeft);
			num += Word97Writer.AddSprm(array, num, 54812, 0, this.m_borderColorsBottom);
			num += Word97Writer.AddSprm(array, num, 54813, 0, this.m_borderColorsRight);
			return array;
		}
	}
}

using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Drawing;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal class Underline
	{
		internal const double UnderlineScale = 0.085;

		private Point m_startPoint;

		private Point m_endPoint;

		private Underline()
		{
		}

		internal Underline(TextRun run, Win32DCSafeHandle hdc, FontCache fontCache, Rectangle layoutRectangle, int x, int baselineY, RPLFormat.WritingModes writingMode)
		{
			int width = run.GetWidth(hdc, fontCache);
			int num = (int)((double)(int)((double)run.UnderlineHeight * 0.085) * 1.5);
			switch (writingMode)
			{
			case RPLFormat.WritingModes.Horizontal:
				this.m_startPoint = new Point(layoutRectangle.X + x, layoutRectangle.Y + baselineY + num);
				this.m_endPoint = new Point(Math.Min(this.m_startPoint.X + width, layoutRectangle.Right), this.m_startPoint.Y);
				break;
			case RPLFormat.WritingModes.Vertical:
				this.m_startPoint = new Point(layoutRectangle.Right - baselineY - num - 1, layoutRectangle.Y + x);
				this.m_endPoint = new Point(this.m_startPoint.X, Math.Min(this.m_startPoint.Y + width, layoutRectangle.Bottom));
				break;
			case RPLFormat.WritingModes.Rotate270:
				this.m_startPoint = new Point(layoutRectangle.X + baselineY + num, layoutRectangle.Bottom - x);
				this.m_endPoint = new Point(this.m_startPoint.X, Math.Max(this.m_startPoint.Y - width, layoutRectangle.Top));
				break;
			}
		}

		internal void Draw(Win32DCSafeHandle hdc, int lineThickness, uint rgbColor)
		{
			if (lineThickness < 1)
			{
				lineThickness = 1;
			}
			Win32.LOGBRUSH lOGBRUSH = default(Win32.LOGBRUSH);
			lOGBRUSH.lbColor = rgbColor;
			lOGBRUSH.lbHatch = 0;
			lOGBRUSH.lbStyle = 0u;
			Win32ObjectSafeHandle win32ObjectSafeHandle = Win32.ExtCreatePen(66048u, (uint)lineThickness, ref lOGBRUSH, 0u, null);
			Win32ObjectSafeHandle win32ObjectSafeHandle2 = Win32ObjectSafeHandle.Zero;
			try
			{
				win32ObjectSafeHandle2 = Win32.SelectObject(hdc, win32ObjectSafeHandle);
				Win32.MoveToEx(hdc, this.m_startPoint.X, this.m_startPoint.Y, IntPtr.Zero);
				Win32.LineTo(hdc, this.m_endPoint.X, this.m_endPoint.Y);
			}
			finally
			{
				if (!win32ObjectSafeHandle2.IsInvalid)
				{
					Win32ObjectSafeHandle win32ObjectSafeHandle3 = Win32.SelectObject(hdc, win32ObjectSafeHandle2);
					win32ObjectSafeHandle3.SetHandleAsInvalid();
					win32ObjectSafeHandle2.SetHandleAsInvalid();
				}
				if (!win32ObjectSafeHandle.IsInvalid)
				{
					win32ObjectSafeHandle.Close();
					win32ObjectSafeHandle = null;
				}
			}
		}
	}
}

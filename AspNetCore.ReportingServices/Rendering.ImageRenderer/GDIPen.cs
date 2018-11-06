using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class GDIPen
	{
		private GDIPen()
		{
		}

		private static string GetKey(Color color, float size, RPLFormat.BorderStyles style)
		{
			string text = color.ToString() + size;
			if ((int)(style & RPLFormat.BorderStyles.Dashed) > 0)
			{
				text += "s";
			}
			if ((int)(style & RPLFormat.BorderStyles.Dotted) > 0)
			{
				text += "t";
			}
			if ((int)(style & RPLFormat.BorderStyles.Solid) > 0)
			{
				text += "d";
			}
			return text;
		}

		internal static Pen GetPen(Dictionary<string, Pen> pens, Color color, float size, RPLFormat.BorderStyles style)
		{
			string key = GDIPen.GetKey(color, size, style);
			Pen pen = default(Pen);
			if (pens.TryGetValue(key, out pen))
			{
				return pen;
			}
			pen = new Pen(color, size);
			switch (style)
			{
			case RPLFormat.BorderStyles.Dashed:
				pen.DashStyle = DashStyle.Dash;
				break;
			case RPLFormat.BorderStyles.Dotted:
				pen.DashStyle = DashStyle.Dot;
				break;
			}
			pens.Add(key, pen);
			return pen;
		}
	}
}

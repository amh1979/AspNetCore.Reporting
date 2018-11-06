using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class GradientEditor : UITypeEditor
	{
		private MapGraphics mapGraph;

		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override void PaintValue(PaintValueEventArgs e)
		{
			if (e.Value is GradientType)
			{
				if (this.mapGraph == null)
				{
					this.mapGraph = new MapGraphics(null);
				}
				this.mapGraph.Graphics = e.Graphics;
				Color color = Color.Black;
				Color color2 = Color.White;
				if (e.Context != null && e.Context.Instance != null)
				{
					PropertyInfo property = e.Context.Instance.GetType().GetProperty("Color");
					if (property != null)
					{
						color = (Color)property.GetValue(e.Context.Instance, null);
					}
					else
					{
						property = e.Context.Instance.GetType().GetProperty("FillColor");
						if (property != null)
						{
							color = (Color)property.GetValue(e.Context.Instance, null);
						}
						else
						{
							property = e.Context.Instance.GetType().GetProperty("BackColor");
							if (property != null)
							{
								color = (Color)property.GetValue(e.Context.Instance, null);
							}
						}
					}
					property = e.Context.Instance.GetType().GetProperty("SecondaryColor");
					if (property != null)
					{
						color2 = (Color)property.GetValue(e.Context.Instance, null);
					}
					else
					{
						property = e.Context.Instance.GetType().GetProperty("FillSecondaryColor");
						if (property != null)
						{
							color2 = (Color)property.GetValue(e.Context.Instance, null);
						}
						else
						{
							property = e.Context.Instance.GetType().GetProperty("BackSecondaryColor");
							if (property != null)
							{
								color2 = (Color)property.GetValue(e.Context.Instance, null);
							}
						}
					}
				}
				if (color == Color.Empty)
				{
					color = Color.Black;
				}
				if (color2 == Color.Empty)
				{
					color2 = Color.White;
				}
				if (color == color2)
				{
					color2 = Color.FromArgb(color.B, color.R, color.G);
				}
				if ((GradientType)e.Value != 0)
				{
					Brush gradientBrush = this.mapGraph.GetGradientBrush(e.Bounds, color, color2, (GradientType)e.Value);
					e.Graphics.FillRectangle(gradientBrush, e.Bounds);
					gradientBrush.Dispose();
				}
			}
		}
	}
}

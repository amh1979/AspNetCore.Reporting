using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeMapArea_MapArea")]
	[DefaultProperty("ToolTip")]
	internal class MapArea : IMapAreaAttributes
	{
		private string toolTip = "";

		private string href = "";

		private string attributes = "";

		private string name = "Map Area";

		private bool custom = true;

		private MapAreaShape shape;

		private float[] coordinates = new float[4];

		private object mapAreaTag;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue("")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeMapArea_Custom")]
		public bool Custom
		{
			get
			{
				return this.custom;
			}
			set
			{
				this.custom = value;
			}
		}

		[SRDescription("DescriptionAttributeMapArea_Coordinates")]
		[DefaultValue("")]
		[TypeConverter(typeof(MapAreaCoordinatesConverter))]
		[Bindable(true)]
		[SRCategory("CategoryAttributeShape")]
		public float[] Coordinates
		{
			get
			{
				return this.coordinates;
			}
			set
			{
				this.coordinates = value;
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeShape")]
		[SRDescription("DescriptionAttributeMapArea_Shape")]
		[DefaultValue(typeof(MapAreaShape), "Rectangle")]
		public MapAreaShape Shape
		{
			get
			{
				return this.shape;
			}
			set
			{
				this.shape = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeData")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeMapArea_Name")]
		[DefaultValue("Map Area")]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMapArea_ToolTip")]
		[DefaultValue("")]
		[Browsable(false)]
		[SRCategory("CategoryAttributeMapArea")]
		public string ToolTip
		{
			get
			{
				return this.toolTip;
			}
			set
			{
				this.toolTip = value;
			}
		}

		[Bindable(true)]
		[DefaultValue("")]
		[SRCategory("CategoryAttributeMapArea")]
		[SRDescription("DescriptionAttributeMapArea_Href")]
		public string Href
		{
			get
			{
				return this.href;
			}
			set
			{
				this.href = value;
			}
		}

		[SRCategory("CategoryAttributeMapArea")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DefaultValue("")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMapArea_MapAreaAttributes")]
		public string MapAreaAttributes
		{
			get
			{
				return this.attributes;
			}
			set
			{
				this.attributes = value;
			}
		}

		object IMapAreaAttributes.Tag
		{
			get
			{
				return this.mapAreaTag;
			}
			set
			{
				this.mapAreaTag = value;
			}
		}

		internal string GetTag(ChartGraphics graph)
		{
			StringBuilder stringBuilder = new StringBuilder("\r\n<area shape=\"", 120);
			if (this.shape == MapAreaShape.Circle)
			{
				stringBuilder.Append("circle\"");
			}
			else if (this.shape == MapAreaShape.Rectangle)
			{
				stringBuilder.Append("rect\"");
			}
			else if (this.shape == MapAreaShape.Polygon)
			{
				stringBuilder.Append("poly\"");
			}
			if (this.Href.Length > 0)
			{
				stringBuilder.Append(" href=\"");
				if (this.Href.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
				{
					stringBuilder.Append("http://");
				}
				stringBuilder.Append(this.Href);
				stringBuilder.Append("\"");
			}
			if (this.ToolTip.Length > 0)
			{
				stringBuilder.Append(" title=\"");
				stringBuilder.Append(this.ToolTip);
				stringBuilder.Append("\"");
			}
			stringBuilder.Append(" coords=\"");
			float[] array = new float[this.Coordinates.Length];
			if (this.Shape == MapAreaShape.Circle)
			{
				PointF absolutePoint = graph.GetAbsolutePoint(new PointF(this.Coordinates[0], this.Coordinates[1]));
				array[0] = absolutePoint.X;
				array[1] = absolutePoint.Y;
				absolutePoint = graph.GetAbsolutePoint(new PointF(this.Coordinates[2], this.Coordinates[1]));
				array[2] = absolutePoint.X;
			}
			else if (this.Shape == MapAreaShape.Rectangle)
			{
				PointF absolutePoint2 = graph.GetAbsolutePoint(new PointF(this.Coordinates[0], this.Coordinates[1]));
				array[0] = absolutePoint2.X;
				array[1] = absolutePoint2.Y;
				absolutePoint2 = graph.GetAbsolutePoint(new PointF(this.Coordinates[2], this.Coordinates[3]));
				array[2] = absolutePoint2.X;
				array[3] = absolutePoint2.Y;
				if ((int)Math.Round((double)array[0]) == (int)Math.Round((double)array[2]))
				{
					array[2] = (float)((float)Math.Round((double)array[2]) + 1.0);
				}
				if ((int)Math.Round((double)array[1]) == (int)Math.Round((double)array[3]))
				{
					array[3] = (float)((float)Math.Round((double)array[3]) + 1.0);
				}
			}
			else
			{
				PointF pointF = Point.Empty;
				PointF relative = Point.Empty;
				for (int i = 0; i < this.Coordinates.Length - 1; i += 2)
				{
					relative.X = this.Coordinates[i];
					relative.Y = this.Coordinates[i + 1];
					pointF = graph.GetAbsolutePoint(relative);
					array[i] = pointF.X;
					array[i + 1] = pointF.Y;
				}
			}
			bool flag = true;
			float[] array2 = array;
			foreach (float num in array2)
			{
				if (!flag)
				{
					stringBuilder.Append(",");
				}
				flag = false;
				stringBuilder.Append((int)Math.Round((double)num));
			}
			stringBuilder.Append("\"");
			if (this.MapAreaAttributes.Length > 0)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(this.MapAreaAttributes);
			}
			if (stringBuilder.ToString().IndexOf("alt=", StringComparison.Ordinal) == -1)
			{
				stringBuilder.Append(" alt=\"\"");
			}
			stringBuilder.Append("/>");
			return stringBuilder.ToString();
		}
	}
}

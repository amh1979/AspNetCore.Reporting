using System;
using System.ComponentModel;
using System.Text;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[DefaultProperty("ToolTip")]
	[SRDescription("DescriptionAttributeMapArea_MapArea")]
	internal class MapArea : IMapAreaAttributes
	{
		private string toolTip = "";

		private string href = "";

		private string attributes = "";

		private float[] coordinates = new float[4];

		private string name = "Map Area";

		private bool custom = true;

		private MapAreaShape shape;

		private object imagMapProviderTag;

		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeMapArea_Custom")]
		[DefaultValue("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal bool Custom
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
		[TypeConverter(typeof(MapAreaCoordinatesConverter))]
		[Category("Shape")]
		[Bindable(true)]
		[DefaultValue("")]
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
		[Category("Shape")]
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

		[Category("Data")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeMapArea_Name")]
		[DefaultValue("Map Area")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

		[Bindable(true)]
		[DefaultValue("")]
		[Category("MapArea")]
		[SRDescription("DescriptionAttributeMapArea_ToolTip")]
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

		[DefaultValue("")]
		[Category("MapArea")]
		[Bindable(true)]
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue("")]
		[Bindable(true)]
		[Browsable(false)]
		[Category("MapArea")]
		[SRDescription("DescriptionAttributeMapAreaAttributes4")]
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

		internal object Tag
		{
			get
			{
				return this.imagMapProviderTag;
			}
			set
			{
				this.imagMapProviderTag = value;
			}
		}

		internal string GetTag()
		{
			StringBuilder stringBuilder = new StringBuilder("\r\n<AREA SHAPE=\"", 120);
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
				stringBuilder.Append(" HREF=\"");
				if (this.Href.StartsWith("WWW.", StringComparison.OrdinalIgnoreCase))
				{
					stringBuilder.Append("http://");
				}
				stringBuilder.Append(this.Href);
				stringBuilder.Append("\"");
			}
			if (this.ToolTip.Length > 0)
			{
				stringBuilder.Append(" Title=\"");
				stringBuilder.Append(this.ToolTip);
				stringBuilder.Append("\"");
			}
			stringBuilder.Append(" COORDS=\"");
			float[] array = new float[this.Coordinates.Length];
			this.Coordinates.CopyTo(array, 0);
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
			stringBuilder.Append(">");
			return stringBuilder.ToString();
		}
	}
}

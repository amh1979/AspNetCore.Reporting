using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace AspNetCore.Reporting.Map.WebForms
{
	[Description("Represent a custom map area element, that has a user-defined shape and dimensions.")]
	[DefaultProperty("ToolTip")]
	internal class MapArea : IMapAreaAttributes
	{
		private string toolTip = "";

		private string href = "";

		private string attributes = "";

		private int[] coordinates = new int[4];

		private string name = "Map Area";

		private bool custom = true;

		private MapAreaShape shape;

		private object mapAreaTag;

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[Description("Indicates that the map area is custom.")]
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

		[Bindable(true)]
		[SRCategory("CategoryAttribute_Shape")]
		[SRDescription("DescriptionAttributeMapArea_Coordinates")]
		[DefaultValue("")]
		[TypeConverter(typeof(MapAreaCoordinatesConverter))]
		public int[] Coordinates
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

		[DefaultValue(typeof(MapAreaShape), "Rectangle")]
		[SRCategory("CategoryAttribute_Shape")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMapArea_Shape")]
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

		[DefaultValue("Map Area")]
		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeMapArea_Name")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
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

		[DefaultValue("")]
		[SRCategory("CategoryAttribute_MapArea")]
		[Bindable(true)]
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

		[SRDescription("DescriptionAttributeMapArea_Href")]
		[DefaultValue("")]
		[SRCategory("CategoryAttribute_MapArea")]
		[Bindable(true)]
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

		[SRCategory("CategoryAttribute_MapArea")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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
				if (this.Href.ToUpper(CultureInfo.InvariantCulture).StartsWith("WWW.", StringComparison.Ordinal))
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
			bool flag = true;
			int[] array = this.Coordinates;
			foreach (int value in array)
			{
				if (!flag)
				{
					stringBuilder.Append(",");
				}
				flag = false;
				stringBuilder.Append(value);
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

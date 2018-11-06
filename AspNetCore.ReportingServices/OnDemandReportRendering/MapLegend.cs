using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLegend : MapDockableSubItem
	{
		private ReportEnumProperty<MapLegendLayout> m_layout;

		private MapLegendTitle m_mapLegendTitle;

		private ReportBoolProperty m_autoFitTextDisabled;

		private ReportSizeProperty m_minFontSize;

		private ReportBoolProperty m_interlacedRows;

		private ReportColorProperty m_interlacedRowsColor;

		private ReportBoolProperty m_equallySpacedItems;

		private ReportIntProperty m_textWrapThreshold;

		public string Name
		{
			get
			{
				return this.MapLegendDef.Name;
			}
		}

		public ReportEnumProperty<MapLegendLayout> Layout
		{
			get
			{
				if (this.m_layout == null && this.MapLegendDef.Layout != null)
				{
					this.m_layout = new ReportEnumProperty<MapLegendLayout>(this.MapLegendDef.Layout.IsExpression, this.MapLegendDef.Layout.OriginalText, EnumTranslator.TranslateMapLegendLayout(this.MapLegendDef.Layout.StringValue, null));
				}
				return this.m_layout;
			}
		}

		public MapLegendTitle MapLegendTitle
		{
			get
			{
				if (this.m_mapLegendTitle == null && this.MapLegendDef.MapLegendTitle != null)
				{
					this.m_mapLegendTitle = new MapLegendTitle(this.MapLegendDef.MapLegendTitle, base.m_map);
				}
				return this.m_mapLegendTitle;
			}
		}

		public ReportBoolProperty AutoFitTextDisabled
		{
			get
			{
				if (this.m_autoFitTextDisabled == null && this.MapLegendDef.AutoFitTextDisabled != null)
				{
					this.m_autoFitTextDisabled = new ReportBoolProperty(this.MapLegendDef.AutoFitTextDisabled);
				}
				return this.m_autoFitTextDisabled;
			}
		}

		public ReportSizeProperty MinFontSize
		{
			get
			{
				if (this.m_minFontSize == null && this.MapLegendDef.MinFontSize != null)
				{
					this.m_minFontSize = new ReportSizeProperty(this.MapLegendDef.MinFontSize);
				}
				return this.m_minFontSize;
			}
		}

		public ReportBoolProperty InterlacedRows
		{
			get
			{
				if (this.m_interlacedRows == null && this.MapLegendDef.InterlacedRows != null)
				{
					this.m_interlacedRows = new ReportBoolProperty(this.MapLegendDef.InterlacedRows);
				}
				return this.m_interlacedRows;
			}
		}

		public ReportColorProperty InterlacedRowsColor
		{
			get
			{
				if (this.m_interlacedRowsColor == null && this.MapLegendDef.InterlacedRowsColor != null)
				{
					ExpressionInfo interlacedRowsColor = this.MapLegendDef.InterlacedRowsColor;
					if (interlacedRowsColor != null)
					{
						this.m_interlacedRowsColor = new ReportColorProperty(interlacedRowsColor.IsExpression, this.MapLegendDef.InterlacedRowsColor.OriginalText, interlacedRowsColor.IsExpression ? null : new ReportColor(interlacedRowsColor.StringValue.Trim(), true), interlacedRowsColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
					}
				}
				return this.m_interlacedRowsColor;
			}
		}

		public ReportBoolProperty EquallySpacedItems
		{
			get
			{
				if (this.m_equallySpacedItems == null && this.MapLegendDef.EquallySpacedItems != null)
				{
					this.m_equallySpacedItems = new ReportBoolProperty(this.MapLegendDef.EquallySpacedItems);
				}
				return this.m_equallySpacedItems;
			}
		}

		public ReportIntProperty TextWrapThreshold
		{
			get
			{
				if (this.m_textWrapThreshold == null && this.MapLegendDef.TextWrapThreshold != null)
				{
					this.m_textWrapThreshold = new ReportIntProperty(this.MapLegendDef.TextWrapThreshold.IsExpression, this.MapLegendDef.TextWrapThreshold.OriginalText, this.MapLegendDef.TextWrapThreshold.IntValue, 0);
				}
				return this.m_textWrapThreshold;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend MapLegendDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend)base.m_defObject;
			}
		}

		public new MapLegendInstance Instance
		{
			get
			{
				return (MapLegendInstance)this.GetInstance();
			}
		}

		internal MapLegend(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend defObject, Map map)
			: base(defObject, map)
		{
		}

		internal override MapSubItemInstance GetInstance()
		{
			if (base.m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new MapLegendInstance(this);
			}
			return (MapSubItemInstance)base.m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_mapLegendTitle != null)
			{
				this.m_mapLegendTitle.SetNewContext();
			}
		}
	}
}

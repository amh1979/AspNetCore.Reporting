using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class TablixHeader : IDOwner, IPersistable
	{
		private string m_size;

		private double m_sizeValue;

		private ReportItem m_cellContents;

		private ReportItem m_altCellContents;

		[NonSerialized]
		private static readonly string[] m_StylesForEmptyRectangleInSubtotals = new string[23]
		{
			"BackgroundColor",
			"BackgroundGradientType",
			"BackgroundGradientEndColor",
			"BackgroundImageMIMEType",
			"BackgroundImageSource",
			"BackgroundImageValue",
			"BackgroundImage",
			"BackgroundRepeat",
			"BorderColor",
			"BorderColorTop",
			"BorderColorBottom",
			"BorderColorRight",
			"BorderColorLeft",
			"BorderStyle",
			"BorderStyleTop",
			"BorderStyleBottom",
			"BorderStyleLeft",
			"BorderStyleRight",
			"BorderWidth",
			"BorderWidthTop",
			"BorderWidthBottom",
			"BorderWidthLeft",
			"BorderWidthRight"
		};

		[NonSerialized]
		private static readonly Declaration m_Declaration = TablixHeader.GetDeclaration();

		[NonSerialized]
		private ReportSize m_sizeForRendering;

		[NonSerialized]
		private List<ReportItem> m_cellContentCollection;

		internal string Size
		{
			get
			{
				return this.m_size;
			}
			set
			{
				this.m_size = value;
			}
		}

		internal double SizeValue
		{
			get
			{
				return this.m_sizeValue;
			}
			set
			{
				this.m_sizeValue = value;
			}
		}

		internal ReportSize SizeForRendering
		{
			get
			{
				return this.m_sizeForRendering;
			}
			set
			{
				this.m_sizeForRendering = value;
			}
		}

		internal ReportItem CellContents
		{
			get
			{
				return this.m_cellContents;
			}
			set
			{
				this.m_cellContents = value;
			}
		}

		internal ReportItem AltCellContents
		{
			get
			{
				return this.m_altCellContents;
			}
			set
			{
				this.m_altCellContents = value;
			}
		}

		internal List<ReportItem> CellContentCollection
		{
			get
			{
				if (this.m_cellContentCollection == null && this.m_cellContents != null)
				{
					this.m_cellContentCollection = new List<ReportItem>((this.m_altCellContents == null) ? 1 : 2);
					if (this.m_cellContents != null)
					{
						this.m_cellContentCollection.Add(this.m_cellContents);
					}
					if (this.m_altCellContents != null)
					{
						this.m_cellContentCollection.Add(this.m_altCellContents);
					}
				}
				return this.m_cellContentCollection;
			}
		}

		internal TablixHeader()
		{
		}

		internal TablixHeader(int id)
			: base(id)
		{
		}

		internal void Initialize(InitializationContext context, bool isColumn, bool ignoreSize)
		{
			if (this.m_cellContents != null)
			{
				this.m_cellContents.Initialize(context);
				if (this.m_altCellContents != null)
				{
					this.m_altCellContents.Initialize(context);
				}
			}
		}

		internal object PublishClone(AutomaticSubtotalContext context, bool isClonedDynamic)
		{
			TablixHeader tablixHeader = (TablixHeader)base.PublishClone(context);
			if (this.m_size != null)
			{
				tablixHeader.m_size = (string)this.m_size.Clone();
			}
			if (this.m_cellContents != null)
			{
				if (isClonedDynamic)
				{
					ExpressionInfo meDotValueExpression = null;
					Rectangle rectangle = new Rectangle(context.GenerateID(), context.GenerateID(), context.CurrentDataRegion);
					rectangle.Name = context.CreateUniqueReportItemName(this.m_cellContents.Name, true, false);
					Style styleClass = this.m_cellContents.StyleClass;
					if (styleClass != null)
					{
						Style style = new Style(ConstructionPhase.Publishing);
						string[] stylesForEmptyRectangleInSubtotals = TablixHeader.m_StylesForEmptyRectangleInSubtotals;
						foreach (string name in stylesForEmptyRectangleInSubtotals)
						{
							this.AddAttribute(context, styleClass, style, name, meDotValueExpression);
						}
						rectangle.StyleClass = style;
					}
					tablixHeader.m_cellContents = rectangle;
				}
				else
				{
					tablixHeader.m_cellContents = (ReportItem)this.m_cellContents.PublishClone(context);
					if (this.m_altCellContents != null)
					{
						Global.Tracer.Assert(tablixHeader.m_cellContents is CustomReportItem);
						tablixHeader.m_altCellContents = (ReportItem)this.m_altCellContents.PublishClone(context);
						((CustomReportItem)tablixHeader.m_cellContents).AltReportItem = tablixHeader.m_altCellContents;
					}
				}
			}
			return tablixHeader;
		}

		private void AddAttribute(AutomaticSubtotalContext context, Style originalStyle, Style newStyle, string name, ExpressionInfo meDotValueExpression)
		{
			AttributeInfo attributeInfo = default(AttributeInfo);
			if (originalStyle.GetAttributeInfo(name, out attributeInfo))
			{
				if (attributeInfo.IsExpression)
				{
					newStyle.AddAttribute(name, (ExpressionInfo)originalStyle.ExpressionList[attributeInfo.IntValue].PublishClone(context));
				}
				else
				{
					newStyle.StyleAttributes.Add(name, attributeInfo.PublishClone(context));
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Size, Token.String));
			list.Add(new MemberInfo(MemberName.SizeValue, Token.Double));
			list.Add(new MemberInfo(MemberName.CellContents, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem));
			list.Add(new MemberInfo(MemberName.AltCellContents, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixHeader, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(TablixHeader.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Size:
					writer.Write(this.m_size);
					break;
				case MemberName.SizeValue:
					writer.Write(this.m_sizeValue);
					break;
				case MemberName.CellContents:
					writer.Write(this.m_cellContents);
					break;
				case MemberName.AltCellContents:
					writer.Write(this.m_altCellContents);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(TablixHeader.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Size:
					this.m_size = reader.ReadString();
					break;
				case MemberName.SizeValue:
					this.m_sizeValue = reader.ReadDouble();
					break;
				case MemberName.CellContents:
					this.m_cellContents = (ReportItem)reader.ReadRIFObject();
					break;
				case MemberName.AltCellContents:
					this.m_altCellContents = (ReportItem)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixHeader;
		}
	}
}

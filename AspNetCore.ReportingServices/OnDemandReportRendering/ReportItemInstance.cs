using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class ReportItemInstance : ReportElementInstance
	{
		[NonSerialized]
		protected string m_uniqueName;

		private string m_toolTip;

		private string m_bookmark;

		private string m_documentMapLabel;

		[NonSerialized]
		private bool m_toolTipEvaluated;

		[NonSerialized]
		private bool m_bookmarkEvaluated;

		[NonSerialized]
		private bool m_documentMapLabelEvaluated;

		[NonSerialized]
		protected VisibilityInstance m_visibility;

		private static readonly Declaration m_Declaration = ReportItemInstance.GetDeclaration();

		public virtual string UniqueName
		{
			get
			{
				if (base.m_reportElementDef.IsOldSnapshot)
				{
					return base.m_reportElementDef.RenderReportItem.UniqueName;
				}
				if (this.m_uniqueName == null)
				{
					this.m_uniqueName = base.m_reportElementDef.ReportItemDef.UniqueName;
				}
				return this.m_uniqueName;
			}
		}

		public string ToolTip
		{
			get
			{
				if (!this.m_toolTipEvaluated)
				{
					this.m_toolTipEvaluated = true;
					if (base.m_reportElementDef.IsOldSnapshot)
					{
						this.m_toolTip = base.m_reportElementDef.RenderReportItem.ToolTip;
					}
					else
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItemDef = base.m_reportElementDef.ReportItemDef;
						if (reportItemDef.ToolTip != null)
						{
							if (!reportItemDef.ToolTip.IsExpression)
							{
								this.m_toolTip = reportItemDef.ToolTip.StringValue;
							}
							else if (base.m_reportElementDef.CriOwner == null)
							{
								this.m_toolTip = reportItemDef.EvaluateToolTip(this.ReportScopeInstance, this.RenderingContext.OdpContext);
							}
						}
					}
				}
				return this.m_toolTip;
			}
			set
			{
				if (base.m_reportElementDef.CriGenerationPhase != 0 && (base.m_reportElementDef.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || ((ReportItem)base.m_reportElementDef).ToolTip.IsExpression))
				{
					this.m_toolTipEvaluated = true;
					this.m_toolTip = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		public string Bookmark
		{
			get
			{
				if (!this.m_bookmarkEvaluated)
				{
					this.m_bookmarkEvaluated = true;
					if (base.m_reportElementDef.IsOldSnapshot)
					{
						this.m_bookmark = base.m_reportElementDef.RenderReportItem.Bookmark;
					}
					else
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItemDef = base.m_reportElementDef.ReportItemDef;
						if (reportItemDef.Bookmark != null)
						{
							if (!reportItemDef.Bookmark.IsExpression)
							{
								this.m_bookmark = reportItemDef.Bookmark.StringValue;
							}
							else if (base.m_reportElementDef.CriOwner == null)
							{
								this.m_bookmark = reportItemDef.EvaluateBookmark(this.ReportScopeInstance, this.RenderingContext.OdpContext);
							}
						}
					}
				}
				return this.m_bookmark;
			}
			set
			{
				if (base.m_reportElementDef.CriGenerationPhase != 0 && (base.m_reportElementDef.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || ((ReportItem)base.m_reportElementDef).Bookmark.IsExpression))
				{
					this.m_bookmarkEvaluated = true;
					this.m_bookmark = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		public string DocumentMapLabel
		{
			get
			{
				if (!this.m_documentMapLabelEvaluated)
				{
					this.m_documentMapLabelEvaluated = true;
					if (base.m_reportElementDef.IsOldSnapshot)
					{
						this.m_documentMapLabel = base.m_reportElementDef.RenderReportItem.Label;
					}
					else
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItemDef = base.m_reportElementDef.ReportItemDef;
						if (reportItemDef.DocumentMapLabel != null)
						{
							if (!reportItemDef.DocumentMapLabel.IsExpression)
							{
								this.m_documentMapLabel = reportItemDef.DocumentMapLabel.StringValue;
							}
							else if (base.m_reportElementDef.CriOwner == null)
							{
								this.m_documentMapLabel = reportItemDef.EvaluateDocumentMapLabel(this.ReportScopeInstance, this.RenderingContext.OdpContext);
							}
						}
					}
				}
				return this.m_documentMapLabel;
			}
			set
			{
				if (base.m_reportElementDef.CriGenerationPhase != 0 && (base.m_reportElementDef.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || ((ReportItem)base.m_reportElementDef).DocumentMapLabel.IsExpression))
				{
					this.m_documentMapLabelEvaluated = true;
					this.m_documentMapLabel = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		public virtual VisibilityInstance Visibility
		{
			get
			{
				if (this.m_visibility == null && ((ReportItem)base.m_reportElementDef).Visibility != null)
				{
					this.m_visibility = new ReportItemVisibilityInstance(base.m_reportElementDef as ReportItem);
				}
				return this.m_visibility;
			}
		}

		internal RenderingContext RenderingContext
		{
			get
			{
				return base.m_reportElementDef.RenderingContext;
			}
		}

		internal ReportItemInstance(ReportItem reportItemDef)
			: base(reportItemDef)
		{
		}

		protected string GetDefaultFontFamily()
		{
			if (this.RenderingContext.OdpContext == null)
			{
				return null;
			}
			return this.RenderingContext.OdpContext.ReportDefinition.DefaultFontFamily;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_uniqueName = null;
			this.m_toolTipEvaluated = false;
			this.m_toolTip = null;
			this.m_bookmarkEvaluated = false;
			this.m_bookmark = null;
			this.m_documentMapLabelEvaluated = false;
			this.m_documentMapLabel = null;
			if (this.m_visibility != null)
			{
				this.m_visibility.SetNewContext();
			}
		}

		internal override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ReportItemInstance.m_Declaration);
			ReportItem reportItem = (ReportItem)base.ReportElementDef;
			List<string> list = default(List<string>);
			List<object> list2 = default(List<object>);
			reportItem.CustomProperties.GetDynamicValues(out list, out list2);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ToolTip:
				{
					string value2 = null;
					if (reportItem.ToolTip.IsExpression)
					{
						value2 = this.m_toolTip;
					}
					writer.Write(value2);
					break;
				}
				case MemberName.Bookmark:
				{
					string value3 = null;
					if (reportItem.Bookmark.IsExpression)
					{
						value3 = this.m_bookmark;
					}
					writer.Write(value3);
					break;
				}
				case MemberName.Label:
				{
					string value = null;
					if (reportItem.DocumentMapLabel.IsExpression)
					{
						value = this.m_documentMapLabel;
					}
					writer.Write(value);
					break;
				}
				case MemberName.CustomPropertyNames:
					writer.WriteListOfPrimitives(list);
					break;
				case MemberName.CustomPropertyValues:
					writer.WriteListOfPrimitives(list2);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		internal override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(ReportItemInstance.m_Declaration);
			ReportItem reportItem = (ReportItem)base.ReportElementDef;
			List<string> customPropertyNames = null;
			List<object> customPropertyValues = null;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ToolTip:
				{
					string text2 = reader.ReadString();
					if (reportItem.ToolTip.IsExpression)
					{
						this.m_toolTip = text2;
					}
					else
					{
						Global.Tracer.Assert(text2 == null, "(toolTip == null)");
					}
					break;
				}
				case MemberName.Bookmark:
				{
					string text3 = reader.ReadString();
					if (reportItem.Bookmark.IsExpression)
					{
						this.m_bookmark = text3;
					}
					else
					{
						Global.Tracer.Assert(text3 == null, "(bookmark == null)");
					}
					break;
				}
				case MemberName.Label:
				{
					string text = reader.ReadString();
					if (reportItem.DocumentMapLabel.IsExpression)
					{
						this.m_documentMapLabel = text;
					}
					else
					{
						Global.Tracer.Assert(text == null, "(documentMapLabel == null)");
					}
					break;
				}
				case MemberName.CustomPropertyNames:
					customPropertyNames = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.CustomPropertyValues:
					customPropertyValues = reader.ReadListOfPrimitives<object>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
			reportItem.CustomProperties.SetDynamicValues(customPropertyNames, customPropertyValues);
		}

		internal override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemInstance;
		}

		[SkipMemberStaticValidation(MemberName.CustomPropertyValues)]
		[SkipMemberStaticValidation(MemberName.CustomPropertyNames)]
		private static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ToolTip, Token.String));
			list.Add(new MemberInfo(MemberName.Bookmark, Token.String));
			list.Add(new MemberInfo(MemberName.Label, Token.String));
			list.Add(new MemberInfo(MemberName.CustomPropertyNames, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
			list.Add(new MemberInfo(MemberName.CustomPropertyValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Object));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportElementInstance, list);
		}
	}
}

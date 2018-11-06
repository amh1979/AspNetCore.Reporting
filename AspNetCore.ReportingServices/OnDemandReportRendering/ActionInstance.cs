using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ActionInstance : BaseInstance, IPersistable, IActionInstance
	{
		[NonSerialized]
		private bool m_isOldSnapshot;

		[NonSerialized]
		private AspNetCore.ReportingServices.ReportRendering.Action m_renderAction;

		[NonSerialized]
		private Action m_actionDef;

		[NonSerialized]
		private ReportUrl m_hyperlink;

		private string m_label;

		private string m_bookmark;

		private string m_hyperlinkText;

		private static readonly Declaration m_Declaration = ActionInstance.GetDeclaration();

		public string Label
		{
			get
			{
				if (this.m_label == null)
				{
					if (this.m_isOldSnapshot)
					{
						if (this.m_renderAction != null)
						{
							this.m_label = this.m_renderAction.Label;
						}
					}
					else if (this.m_actionDef.Label != null)
					{
						if (!this.m_actionDef.Label.IsExpression)
						{
							this.m_label = this.m_actionDef.Label.Value;
						}
						else if (this.m_actionDef.Owner.ReportElementOwner == null || this.m_actionDef.Owner.ReportElementOwner.CriOwner == null)
						{
							ActionInfo owner = this.m_actionDef.Owner;
							this.m_label = this.m_actionDef.ActionItemDef.EvaluateLabel(this.ReportScopeInstance, owner.RenderingContext.OdpContext, owner.InstancePath, owner.ObjectType, owner.ObjectName);
						}
					}
				}
				return this.m_label;
			}
			set
			{
				ReportElement reportElementOwner = this.m_actionDef.Owner.ReportElementOwner;
				if (reportElementOwner.CriGenerationPhase != 0 && (reportElementOwner.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_actionDef.Label == null || this.m_actionDef.Label.IsExpression))
				{
					this.m_label = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		public string BookmarkLink
		{
			get
			{
				if (this.m_bookmark == null)
				{
					if (this.m_isOldSnapshot)
					{
						if (this.m_renderAction != null)
						{
							this.m_bookmark = this.m_renderAction.BookmarkLink;
						}
					}
					else if (this.m_actionDef.BookmarkLink != null)
					{
						if (!this.m_actionDef.BookmarkLink.IsExpression)
						{
							this.m_bookmark = this.m_actionDef.BookmarkLink.Value;
						}
						else if (this.m_actionDef.Owner.ReportElementOwner == null || this.m_actionDef.Owner.ReportElementOwner.CriOwner == null)
						{
							ActionInfo owner = this.m_actionDef.Owner;
							this.m_bookmark = this.m_actionDef.ActionItemDef.EvaluateBookmarkLink(this.ReportScopeInstance, owner.RenderingContext.OdpContext, owner.InstancePath, owner.ObjectType, owner.ObjectName);
						}
					}
				}
				return this.m_bookmark;
			}
			set
			{
				ReportElement reportElementOwner = this.m_actionDef.Owner.ReportElementOwner;
				if (!this.m_actionDef.Owner.IsChartConstruction && (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Definition && this.m_actionDef.BookmarkLink == null) || (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && (this.m_actionDef.BookmarkLink == null || !this.m_actionDef.BookmarkLink.IsExpression))))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				this.m_bookmark = value;
			}
		}

		public ReportUrl Hyperlink
		{
			get
			{
				if (this.m_hyperlink == null)
				{
					if (this.m_isOldSnapshot)
					{
						if (this.m_renderAction != null && this.m_renderAction.HyperLinkURL != null)
						{
							this.m_hyperlink = new ReportUrl(this.m_renderAction.HyperLinkURL);
						}
					}
					else if (this.m_actionDef.Hyperlink != null)
					{
						if (!this.m_actionDef.Hyperlink.IsExpression)
						{
							this.m_hyperlink = this.m_actionDef.Hyperlink.Value;
						}
						else if (this.m_actionDef.Owner.ReportElementOwner == null || this.m_actionDef.Owner.ReportElementOwner.CriOwner == null)
						{
							ActionInfo owner = this.m_actionDef.Owner;
							string hyperlinkText = this.m_actionDef.ActionItemDef.EvaluateHyperLinkURL(this.ReportScopeInstance, owner.RenderingContext.OdpContext, owner.InstancePath, owner.ObjectType, owner.ObjectName);
							((IActionInstance)this).SetHyperlinkText(hyperlinkText);
						}
					}
				}
				return this.m_hyperlink;
			}
		}

		public string HyperlinkText
		{
			get
			{
				return this.m_hyperlinkText;
			}
			set
			{
				ReportElement reportElementOwner = this.m_actionDef.Owner.ReportElementOwner;
				if (!this.m_actionDef.Owner.IsChartConstruction && (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Definition && this.m_actionDef.Hyperlink == null) || (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && (this.m_actionDef.Hyperlink == null || !this.m_actionDef.Hyperlink.IsExpression))))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				((IActionInstance)this).SetHyperlinkText(value);
			}
		}

		internal ActionInstance(IReportScope reportScope, Action actionDef)
			: base(reportScope)
		{
			this.m_isOldSnapshot = false;
			this.m_actionDef = actionDef;
		}

		internal ActionInstance(AspNetCore.ReportingServices.ReportRendering.Action renderAction)
			: base(null)
		{
			this.m_isOldSnapshot = true;
			this.m_renderAction = renderAction;
		}

		void IActionInstance.SetHyperlinkText(string hyperlinkText)
		{
			this.m_hyperlinkText = hyperlinkText;
			if (this.m_hyperlinkText != null)
			{
				ActionInfo owner = this.m_actionDef.Owner;
				this.m_hyperlink = ReportUrl.BuildHyperlinkUrl(owner.RenderingContext, owner.ObjectType, owner.ObjectName, "Hyperlink", owner.RenderingContext.OdpContext.ReportContext, this.m_hyperlinkText);
				if (this.m_hyperlink == null)
				{
					this.m_hyperlinkText = null;
				}
			}
			else
			{
				this.m_hyperlink = null;
			}
		}

		internal void Update(AspNetCore.ReportingServices.ReportRendering.Action newAction)
		{
			this.m_renderAction = newAction;
			this.m_label = null;
			this.m_bookmark = null;
			this.m_hyperlink = null;
		}

		protected override void ResetInstanceCache()
		{
			this.m_label = null;
			this.m_bookmark = null;
			this.m_hyperlink = null;
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ActionInstance.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Label:
				{
					string value2 = null;
					if (this.m_actionDef.Label.IsExpression)
					{
						value2 = this.m_label;
					}
					writer.Write(value2);
					break;
				}
				case MemberName.BookmarkLink:
				{
					string value = null;
					if (this.m_actionDef.BookmarkLink != null && this.m_actionDef.BookmarkLink.IsExpression)
					{
						value = this.m_bookmark;
					}
					writer.Write(value);
					break;
				}
				case MemberName.HyperLinkURL:
				{
					string value3 = null;
					if (this.m_actionDef.Hyperlink != null && this.m_actionDef.Hyperlink.IsExpression)
					{
						value3 = this.m_hyperlinkText;
					}
					writer.Write(value3);
					break;
				}
				case MemberName.DrillthroughReportName:
				{
					string value4 = null;
					if (this.m_actionDef.Drillthrough != null && this.m_actionDef.Drillthrough.ReportName.IsExpression)
					{
						value4 = this.m_actionDef.Drillthrough.Instance.ReportName;
					}
					writer.Write(value4);
					break;
				}
				case MemberName.DrillthroughParameters:
				{
					ParameterInstance[] array = null;
					if (this.m_actionDef.Drillthrough != null && this.m_actionDef.Drillthrough.Parameters != null)
					{
						array = new ParameterInstance[this.m_actionDef.Drillthrough.Parameters.Count];
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = ((ReportElementCollectionBase<Parameter>)this.m_actionDef.Drillthrough.Parameters)[i].Instance;
						}
					}
					writer.Write((IPersistable[])array);
					break;
				}
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ActionInstance.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Label:
				{
					string text2 = reader.ReadString();
					if (this.m_actionDef.Label.IsExpression)
					{
						this.m_label = text2;
					}
					else
					{
						Global.Tracer.Assert(text2 == null, "(label == null)");
					}
					break;
				}
				case MemberName.BookmarkLink:
				{
					string text4 = reader.ReadString();
					if (this.m_actionDef.BookmarkLink != null && this.m_actionDef.BookmarkLink.IsExpression)
					{
						this.m_bookmark = text4;
					}
					else
					{
						Global.Tracer.Assert(text4 == null, "(bookmarkLink == null)");
					}
					break;
				}
				case MemberName.HyperLinkURL:
				{
					string text = reader.ReadString();
					if (this.m_actionDef.Hyperlink != null && this.m_actionDef.Hyperlink.IsExpression)
					{
						this.m_hyperlinkText = text;
					}
					else
					{
						Global.Tracer.Assert(text == null, "(hyperlink == null)");
					}
					break;
				}
				case MemberName.DrillthroughReportName:
				{
					string text3 = reader.ReadString();
					if (this.m_actionDef.Drillthrough != null && this.m_actionDef.Drillthrough.ReportName.IsExpression)
					{
						this.m_actionDef.Drillthrough.Instance.ReportName = text3;
					}
					else
					{
						Global.Tracer.Assert(text3 == null, "(reportName == null)");
					}
					break;
				}
				case MemberName.DrillthroughParameters:
				{
					ParameterCollection paramCollection = null;
					if (this.m_actionDef.Drillthrough != null)
					{
						paramCollection = this.m_actionDef.Drillthrough.Parameters;
					}
					((ROMInstanceObjectCreator)reader.PersistenceHelper).StartParameterInstancesDeserialization(paramCollection);
					reader.ReadArrayOfRIFObjects<ParameterInstance>();
					((ROMInstanceObjectCreator)reader.PersistenceHelper).CompleteParameterInstancesDeserialization();
					break;
				}
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionInstance;
		}

		[SkipMemberStaticValidation(MemberName.DrillthroughReportName)]
		[SkipMemberStaticValidation(MemberName.DrillthroughParameters)]
		private static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Label, Token.String));
			list.Add(new MemberInfo(MemberName.BookmarkLink, Token.String));
			list.Add(new MemberInfo(MemberName.HyperLinkURL, Token.String));
			list.Add(new MemberInfo(MemberName.DrillthroughReportName, Token.String));
			list.Add(new MemberInfo(MemberName.DrillthroughParameters, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInstance));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}
	}
}

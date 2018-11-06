using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal abstract class GaugePanelStyleContainer : IStyleContainer, IPersistable
	{
		[Reference]
		protected GaugePanel m_gaugePanel;

		protected Style m_styleClass;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GaugePanelStyleContainer.GetDeclaration();

		public Style StyleClass
		{
			get
			{
				return this.m_styleClass;
			}
			set
			{
				this.m_styleClass = value;
			}
		}

		IInstancePath IStyleContainer.InstancePath
		{
			get
			{
				return this.m_gaugePanel;
			}
		}

		AspNetCore.ReportingServices.ReportProcessing.ObjectType IStyleContainer.ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel;
			}
		}

		string IStyleContainer.Name
		{
			get
			{
				return this.m_gaugePanel.Name;
			}
		}

		internal GaugePanelStyleContainer()
		{
		}

		internal GaugePanelStyleContainer(GaugePanel gaugePanel)
		{
			this.m_gaugePanel = gaugePanel;
		}

		internal virtual void Initialize(InitializationContext context)
		{
			if (this.m_styleClass != null)
			{
				this.m_styleClass.Initialize(context);
			}
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context)
		{
			GaugePanelStyleContainer gaugePanelStyleContainer = (GaugePanelStyleContainer)base.MemberwiseClone();
			gaugePanelStyleContainer.m_gaugePanel = (GaugePanel)context.CurrentDataRegionClone;
			if (this.m_styleClass != null)
			{
				gaugePanelStyleContainer.m_styleClass = (Style)this.m_styleClass.PublishClone(context);
			}
			return gaugePanelStyleContainer;
		}

		internal virtual void SetExprHost(StyleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && null != reportObjectModel);
			exprHost.SetReportObjectModel(reportObjectModel);
			if (this.m_styleClass != null)
			{
				this.m_styleClass.SetStyleExprHost(exprHost);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.GaugePanel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanel, Token.Reference));
			list.Add(new MemberInfo(MemberName.StyleClass, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(GaugePanelStyleContainer.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.GaugePanel:
					writer.WriteReference(this.m_gaugePanel);
					break;
				case MemberName.StyleClass:
					writer.Write(this.m_styleClass);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(GaugePanelStyleContainer.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.GaugePanel:
					this.m_gaugePanel = reader.ReadReference<GaugePanel>(this);
					break;
				case MemberName.StyleClass:
					this.m_styleClass = (Style)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(GaugePanelStyleContainer.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.GaugePanel)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_gaugePanel = (GaugePanel)referenceableItems[item.RefID];
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}
		}

		public virtual AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer;
		}
	}
}

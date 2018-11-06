using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal abstract class ChartStyleContainer : IStyleContainer, IPersistable
	{
		[Reference]
		protected Chart m_chart;

		protected Style m_styleClass;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartStyleContainer.GetDeclaration();

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

		public virtual IInstancePath InstancePath
		{
			get
			{
				return this.m_chart;
			}
		}

		public AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart;
			}
		}

		public string Name
		{
			get
			{
				return this.m_chart.Name;
			}
		}

		internal ChartStyleContainer()
		{
		}

		internal ChartStyleContainer(Chart chart)
		{
			this.m_chart = chart;
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
			ChartStyleContainer chartStyleContainer = (ChartStyleContainer)base.MemberwiseClone();
			chartStyleContainer.m_chart = (Chart)context.CurrentDataRegionClone;
			if (this.m_styleClass != null)
			{
				chartStyleContainer.m_styleClass = (Style)this.m_styleClass.PublishClone(context);
			}
			return chartStyleContainer;
		}

		internal virtual void SetExprHost(StyleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && null != reportObjectModel, "(null != exprHost && null != reportObjectModel)");
			exprHost.SetReportObjectModel(reportObjectModel);
			if (this.m_styleClass != null)
			{
				this.m_styleClass.SetStyleExprHost(exprHost);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Chart, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			list.Add(new MemberInfo(MemberName.StyleClass, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ChartStyleContainer.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Chart:
					writer.WriteReference(this.m_chart);
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
			reader.RegisterDeclaration(ChartStyleContainer.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Chart:
					this.m_chart = reader.ReadReference<Chart>(this);
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
			if (memberReferencesCollection.TryGetValue(ChartStyleContainer.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.Chart)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_chart = (Chart)referenceableItems[item.RefID];
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer;
		}
	}
}

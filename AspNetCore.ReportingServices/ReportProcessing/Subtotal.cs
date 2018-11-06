using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using AspNetCore.ReportingServices.ReportRendering;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Subtotal : IDOwner
	{
		internal enum PositionType
		{
			After,
			Before
		}

		private bool m_autoDerived;

		private ReportItemCollection m_reportItems;

		private Style m_styleClass;

		private PositionType m_position;

		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput = DataElementOutputTypes.NoOutput;

		[NonSerialized]
		private bool m_firstInstance = true;

		[NonSerialized]
		private string m_renderingModelID;

		[NonSerialized]
		private bool m_computed;

		internal bool AutoDerived
		{
			get
			{
				return this.m_autoDerived;
			}
			set
			{
				this.m_autoDerived = value;
			}
		}

		internal ReportItemCollection ReportItems
		{
			get
			{
				return this.m_reportItems;
			}
			set
			{
				this.m_reportItems = value;
			}
		}

		internal ReportItem ReportItem
		{
			get
			{
				if (this.m_reportItems != null && 0 < this.m_reportItems.Count)
				{
					return this.m_reportItems[0];
				}
				return null;
			}
		}

		internal Style StyleClass
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

		internal PositionType Position
		{
			get
			{
				return this.m_position;
			}
			set
			{
				this.m_position = value;
			}
		}

		internal bool FirstInstance
		{
			get
			{
				return this.m_firstInstance;
			}
			set
			{
				this.m_firstInstance = value;
			}
		}

		internal string RenderingModelID
		{
			get
			{
				return this.m_renderingModelID;
			}
			set
			{
				this.m_renderingModelID = value;
			}
		}

		internal bool Computed
		{
			get
			{
				return this.m_computed;
			}
			set
			{
				this.m_computed = value;
			}
		}

		internal string DataElementName
		{
			get
			{
				return this.m_dataElementName;
			}
			set
			{
				this.m_dataElementName = value;
			}
		}

		internal DataElementOutputTypes DataElementOutput
		{
			get
			{
				return this.m_dataElementOutput;
			}
			set
			{
				this.m_dataElementOutput = value;
			}
		}

		internal Subtotal()
		{
		}

		internal Subtotal(int id, int idForReportItems, bool autoDerived)
			: base(id)
		{
			this.m_autoDerived = autoDerived;
			this.m_reportItems = new ReportItemCollection(idForReportItems, false);
		}

		internal void RegisterReportItems(InitializationContext context)
		{
			context.RegisterReportItems(this.m_reportItems);
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.SubtotalStart();
			this.DataRendererInitialize(context);
			context.RegisterRunningValues(this.m_reportItems.RunningValues);
			if (this.m_styleClass != null)
			{
				this.m_styleClass.Initialize(context);
			}
			this.m_reportItems.Initialize(context, false);
			context.UnRegisterRunningValues(this.m_reportItems.RunningValues);
			context.ExprHostBuilder.SubtotalEnd();
		}

		internal void UnregisterReportItems(InitializationContext context)
		{
			context.UnRegisterReportItems(this.m_reportItems);
		}

		internal void RegisterReceiver(InitializationContext context)
		{
			context.RegisterReportItems(this.m_reportItems);
			this.m_reportItems.RegisterReceiver(context);
			context.UnRegisterReportItems(this.m_reportItems);
		}

		private void DataRendererInitialize(InitializationContext context)
		{
			CLSNameValidator.ValidateDataElementName(ref this.m_dataElementName, "Total", context.ObjectType, context.ObjectName, "DataElementName", context.ErrorContext);
		}

		internal void SetExprHost(StyleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null && this.m_styleClass != null);
			exprHost.SetReportObjectModel(reportObjectModel);
			this.m_styleClass.SetStyleExprHost(exprHost);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.AutoDerived, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.ReportItems, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemCollection));
			memberInfoList.Add(new MemberInfo(MemberName.StyleClass, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Style));
			memberInfoList.Add(new MemberInfo(MemberName.Position, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.IDOwner, memberInfoList);
		}
	}
}

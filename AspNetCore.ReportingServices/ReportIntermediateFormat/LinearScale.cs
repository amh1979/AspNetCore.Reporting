using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class LinearScale : GaugeScale, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = LinearScale.GetDeclaration();

		private List<LinearPointer> m_gaugePointers;

		private ExpressionInfo m_startMargin;

		private ExpressionInfo m_endMargin;

		private ExpressionInfo m_position;

		internal List<LinearPointer> GaugePointers
		{
			get
			{
				return this.m_gaugePointers;
			}
			set
			{
				this.m_gaugePointers = value;
			}
		}

		internal ExpressionInfo StartMargin
		{
			get
			{
				return this.m_startMargin;
			}
			set
			{
				this.m_startMargin = value;
			}
		}

		internal ExpressionInfo EndMargin
		{
			get
			{
				return this.m_endMargin;
			}
			set
			{
				this.m_endMargin = value;
			}
		}

		internal ExpressionInfo Position
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

		internal LinearScale()
		{
		}

		internal LinearScale(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.LinearScaleStart(base.m_name);
			base.Initialize(context);
			if (this.m_gaugePointers != null)
			{
				for (int i = 0; i < this.m_gaugePointers.Count; i++)
				{
					this.m_gaugePointers[i].Initialize(context);
				}
			}
			if (this.m_startMargin != null)
			{
				this.m_startMargin.Initialize("StartMargin", context);
				context.ExprHostBuilder.LinearScaleStartMargin(this.m_startMargin);
			}
			if (this.m_endMargin != null)
			{
				this.m_endMargin.Initialize("EndMargin", context);
				context.ExprHostBuilder.LinearScaleEndMargin(this.m_endMargin);
			}
			if (this.m_position != null)
			{
				this.m_position.Initialize("Position", context);
				context.ExprHostBuilder.LinearScalePosition(this.m_position);
			}
			base.m_exprHostID = context.ExprHostBuilder.LinearScaleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			LinearScale linearScale = (LinearScale)base.PublishClone(context);
			if (this.m_gaugePointers != null)
			{
				linearScale.m_gaugePointers = new List<LinearPointer>(this.m_gaugePointers.Count);
				foreach (LinearPointer gaugePointer in this.m_gaugePointers)
				{
					linearScale.m_gaugePointers.Add((LinearPointer)gaugePointer.PublishClone(context));
				}
			}
			if (this.m_startMargin != null)
			{
				linearScale.m_startMargin = (ExpressionInfo)this.m_startMargin.PublishClone(context);
			}
			if (this.m_endMargin != null)
			{
				linearScale.m_endMargin = (ExpressionInfo)this.m_endMargin.PublishClone(context);
			}
			if (this.m_position != null)
			{
				linearScale.m_position = (ExpressionInfo)this.m_position.PublishClone(context);
			}
			return linearScale;
		}

		internal void SetExprHost(LinearScaleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			base.m_exprHost = exprHost;
			IList<LinearPointerExprHost> linearPointersHostsRemotable = ((LinearScaleExprHost)base.m_exprHost).LinearPointersHostsRemotable;
			if (this.m_gaugePointers != null && linearPointersHostsRemotable != null)
			{
				for (int i = 0; i < this.m_gaugePointers.Count; i++)
				{
					LinearPointer linearPointer = this.m_gaugePointers[i];
					if (linearPointer != null && linearPointer.ExpressionHostID > -1)
					{
						linearPointer.SetExprHost(linearPointersHostsRemotable[linearPointer.ExpressionHostID], reportObjectModel);
					}
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.GaugePointers, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearPointer));
			list.Add(new MemberInfo(MemberName.StartMargin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EndMargin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Position, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearScale, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeScale, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(LinearScale.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.GaugePointers:
					writer.Write(this.m_gaugePointers);
					break;
				case MemberName.StartMargin:
					writer.Write(this.m_startMargin);
					break;
				case MemberName.EndMargin:
					writer.Write(this.m_endMargin);
					break;
				case MemberName.Position:
					writer.Write(this.m_position);
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
			reader.RegisterDeclaration(LinearScale.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.GaugePointers:
					this.m_gaugePointers = reader.ReadGenericListOfRIFObjects<LinearPointer>();
					break;
				case MemberName.StartMargin:
					this.m_startMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EndMargin:
					this.m_endMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Position:
					this.m_position = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearScale;
		}

		internal double EvaluateStartMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateLinearScaleStartMarginExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateEndMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateLinearScaleEndMarginExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluatePosition(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateLinearScalePositionExpression(this, base.m_gaugePanel.Name);
		}
	}
}

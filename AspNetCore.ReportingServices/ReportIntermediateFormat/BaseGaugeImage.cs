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
	internal class BaseGaugeImage : IPersistable
	{
		[NonSerialized]
		protected BaseGaugeImageExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = BaseGaugeImage.GetDeclaration();

		[Reference]
		protected GaugePanel m_gaugePanel;

		private ExpressionInfo m_source;

		private ExpressionInfo m_value;

		private ExpressionInfo m_MIMEType;

		private ExpressionInfo m_transparentColor;

		internal ExpressionInfo Source
		{
			get
			{
				return this.m_source;
			}
			set
			{
				this.m_source = value;
			}
		}

		internal ExpressionInfo Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
			}
		}

		internal ExpressionInfo MIMEType
		{
			get
			{
				return this.m_MIMEType;
			}
			set
			{
				this.m_MIMEType = value;
			}
		}

		internal ExpressionInfo TransparentColor
		{
			get
			{
				return this.m_transparentColor;
			}
			set
			{
				this.m_transparentColor = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return this.m_gaugePanel.Name;
			}
		}

		internal BaseGaugeImageExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal BaseGaugeImage()
		{
		}

		internal BaseGaugeImage(GaugePanel gaugePanel)
		{
			this.m_gaugePanel = gaugePanel;
		}

		internal virtual void Initialize(InitializationContext context)
		{
			if (this.m_source != null)
			{
				this.m_source.Initialize("Source", context);
				context.ExprHostBuilder.BaseGaugeImageSource(this.m_source);
			}
			if (this.m_value != null)
			{
				this.m_value.Initialize("Value", context);
				context.ExprHostBuilder.BaseGaugeImageValue(this.m_value);
			}
			if (this.m_MIMEType != null)
			{
				this.m_MIMEType.Initialize("MIMEType", context);
				context.ExprHostBuilder.BaseGaugeImageMIMEType(this.m_MIMEType);
			}
			if (this.m_transparentColor != null)
			{
				this.m_transparentColor.Initialize("TransparentColor", context);
				context.ExprHostBuilder.BaseGaugeImageTransparentColor(this.m_transparentColor);
			}
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context)
		{
			BaseGaugeImage baseGaugeImage = (BaseGaugeImage)base.MemberwiseClone();
			baseGaugeImage.m_gaugePanel = (GaugePanel)context.CurrentDataRegionClone;
			if (this.m_source != null)
			{
				baseGaugeImage.m_source = (ExpressionInfo)this.m_source.PublishClone(context);
			}
			if (this.m_value != null)
			{
				baseGaugeImage.m_value = (ExpressionInfo)this.m_value.PublishClone(context);
			}
			if (this.m_MIMEType != null)
			{
				baseGaugeImage.m_MIMEType = (ExpressionInfo)this.m_MIMEType.PublishClone(context);
			}
			if (this.m_transparentColor != null)
			{
				baseGaugeImage.m_transparentColor = (ExpressionInfo)this.m_transparentColor.PublishClone(context);
			}
			return baseGaugeImage;
		}

		internal void SetExprHost(BaseGaugeImageExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Source, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TransparentColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MIMEType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.GaugePanel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanel, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BaseGaugeImage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(BaseGaugeImage.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.GaugePanel:
					writer.WriteReference(this.m_gaugePanel);
					break;
				case MemberName.Source:
					writer.Write(this.m_source);
					break;
				case MemberName.Value:
					writer.Write(this.m_value);
					break;
				case MemberName.MIMEType:
					writer.Write(this.m_MIMEType);
					break;
				case MemberName.TransparentColor:
					writer.Write(this.m_transparentColor);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(BaseGaugeImage.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.GaugePanel:
					this.m_gaugePanel = reader.ReadReference<GaugePanel>(this);
					break;
				case MemberName.Source:
					this.m_source = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Value:
					this.m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MIMEType:
					this.m_MIMEType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TransparentColor:
					this.m_transparentColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(BaseGaugeImage.m_Declaration.ObjectType, out list))
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BaseGaugeImage;
		}

		internal AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType EvaluateSource(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateImageSourceType(context.ReportRuntime.EvaluateBaseGaugeImageSourceExpression(this, this.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal string EvaluateStringValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context, out bool errorOccurred)
		{
			context.SetupContext(this.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateBaseGaugeImageStringValueExpression(this, this.m_gaugePanel.Name, out errorOccurred);
		}

		internal byte[] EvaluateBinaryValue(IReportScopeInstance romInstance, OnDemandProcessingContext context, out bool errOccurred)
		{
			context.SetupContext(this.m_gaugePanel, romInstance);
			return context.ReportRuntime.EvaluateBaseGaugeImageBinaryValueExpression(this, this.m_gaugePanel.Name, out errOccurred);
		}

		internal string EvaluateMIMEType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateBaseGaugeImageMIMETypeExpression(this, this.m_gaugePanel.Name);
		}

		internal string EvaluateTransparentColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateBaseGaugeImageTransparentColorExpression(this, this.m_gaugePanel.Name);
		}
	}
}

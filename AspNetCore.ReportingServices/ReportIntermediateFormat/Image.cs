using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions;
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
	internal sealed class Image : ReportItem, IActionOwner, IPersistable
	{
		private Action m_action;

		private AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType m_source;

		private ExpressionInfo m_value;

		private ExpressionInfo m_MIMEType;

		private List<ExpressionInfo> m_tags;

		private AspNetCore.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes m_embeddingMode;

		private AspNetCore.ReportingServices.OnDemandReportRendering.Image.Sizings m_sizing;

		[NonSerialized]
		private ImageExprHost m_exprHost;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		private static readonly Declaration m_Declaration = Image.GetDeclaration();

		[NonSerialized]
		internal static readonly byte[] TransparentImageBytes = new byte[43]
		{
			71,
			73,
			70,
			56,
			57,
			97,
			1,
			0,
			1,
			0,
			240,
			0,
			0,
			219,
			223,
			239,
			0,
			0,
			0,
			33,
			249,
			4,
			1,
			0,
			0,
			0,
			0,
			44,
			0,
			0,
			0,
			0,
			1,
			0,
			1,
			0,
			0,
			2,
			2,
			68,
			1,
			0,
			59
		};

		internal override AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Image;
			}
		}

		internal Action Action
		{
			get
			{
				return this.m_action;
			}
			set
			{
				this.m_action = value;
			}
		}

		internal AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType Source
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

		internal AspNetCore.ReportingServices.OnDemandReportRendering.Image.Sizings Sizing
		{
			get
			{
				return this.m_sizing;
			}
			set
			{
				this.m_sizing = value;
			}
		}

		internal List<ExpressionInfo> Tags
		{
			get
			{
				return this.m_tags;
			}
			set
			{
				this.m_tags = value;
			}
		}

		internal AspNetCore.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes EmbeddingMode
		{
			get
			{
				return this.m_embeddingMode;
			}
			set
			{
				this.m_embeddingMode = value;
			}
		}

		internal ImageExprHost ImageExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		Action IActionOwner.Action
		{
			get
			{
				return this.m_action;
			}
		}

		List<string> IActionOwner.FieldsUsedInValueExpression
		{
			get
			{
				return this.m_fieldsUsedInValueExpression;
			}
			set
			{
				this.m_fieldsUsedInValueExpression = value;
			}
		}

		internal Image(ReportItem parent)
			: base(parent)
		{
		}

		internal Image(int id, ReportItem parent)
			: base(id, parent)
		{
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = this.ObjectType;
			context.ObjectName = base.m_name;
			context.ExprHostBuilder.ImageStart(base.m_name);
			base.Initialize(context);
			if (base.m_visibility != null)
			{
				base.m_visibility.Initialize(context);
			}
			if (this.m_action != null)
			{
				this.m_action.Initialize(context);
			}
			if (this.m_value != null)
			{
				this.m_value.Initialize("Value", context);
				context.ExprHostBuilder.GenericValue(this.m_value);
				if (ExpressionInfo.Types.Constant == this.m_value.Type && this.m_source == AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.External && !context.ReportContext.IsSupportedProtocol(this.m_value.StringValue, true))
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsUnsupportedProtocol, Severity.Error, this.ObjectType, base.m_name, "Value", this.m_value.StringValue, "http://, https://, ftp://, file:, mailto:, or news:");
				}
			}
			if (this.m_MIMEType != null)
			{
				this.m_MIMEType.Initialize("MIMEType", context);
				context.ExprHostBuilder.ImageMIMEType(this.m_MIMEType);
			}
			if (this.m_tags != null)
			{
				for (int i = 0; i < this.m_tags.Count; i++)
				{
					ExpressionInfo expressionInfo = this.m_tags[i];
					expressionInfo.Initialize("Tag", context);
					ExpressionInfo.Types type = expressionInfo.Type;
				}
			}
			if (AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.Embedded == this.m_source && this.m_embeddingMode == AspNetCore.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes.Inline)
			{
				Global.Tracer.Assert(null != this.m_value, "(null != m_value)");
				AspNetCore.ReportingServices.ReportPublishing.PublishingValidator.ValidateEmbeddedImageName(this.m_value, context.EmbeddedImages, this.ObjectType, base.m_name, "Value", context.ErrorContext);
			}
			base.ExprHostID = context.ExprHostBuilder.ImageEnd();
			return true;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			Image image = (Image)base.PublishClone(context);
			if (this.m_action != null)
			{
				image.m_action = (Action)this.m_action.PublishClone(context);
			}
			if (this.m_value != null)
			{
				image.m_value = (ExpressionInfo)this.m_value.PublishClone(context);
			}
			if (this.m_MIMEType != null)
			{
				image.m_MIMEType = (ExpressionInfo)this.m_MIMEType.PublishClone(context);
			}
			if (this.m_tags != null)
			{
				image.m_tags = new List<ExpressionInfo>(this.m_tags.Count);
				foreach (ExpressionInfo tag in this.m_tags)
				{
					image.m_tags.Add((ExpressionInfo)tag.PublishClone(context));
				}
			}
			if (this.m_fieldsUsedInValueExpression != null)
			{
				image.m_fieldsUsedInValueExpression = new List<string>(this.m_fieldsUsedInValueExpression.Count);
				{
					foreach (string item in this.m_fieldsUsedInValueExpression)
					{
						image.m_fieldsUsedInValueExpression.Add((string)item.Clone());
					}
					return image;
				}
			}
			return image;
		}

		[SkipMemberStaticValidation(MemberName.Tag)]
		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Source, Token.Enum));
			list.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MIMEType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Sizing, Token.Enum));
			list.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.Tag, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo, Lifetime.Spanning(100, 200)));
			list.Add(new MemberInfo(MemberName.Tags, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo, Lifetime.AddedIn(200)));
			list.Add(new MemberInfo(MemberName.EmbeddingMode, Token.Enum, Lifetime.AddedIn(200)));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Image, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(Image.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Source:
					writer.WriteEnum((int)this.m_source);
					break;
				case MemberName.Value:
					writer.Write(this.m_value);
					break;
				case MemberName.MIMEType:
					writer.Write(this.m_MIMEType);
					break;
				case MemberName.Sizing:
					writer.WriteEnum((int)this.m_sizing);
					break;
				case MemberName.Action:
					writer.Write(this.m_action);
					break;
				case MemberName.Tag:
				{
					ExpressionInfo persistableObj = null;
					if (this.m_tags != null && this.m_tags.Count > 0)
					{
						persistableObj = this.m_tags[0];
					}
					writer.Write(persistableObj);
					break;
				}
				case MemberName.Tags:
					writer.Write(this.m_tags);
					break;
				case MemberName.EmbeddingMode:
					writer.WriteEnum((int)this.m_embeddingMode);
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
			reader.RegisterDeclaration(Image.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Source:
					this.m_source = (AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType)reader.ReadEnum();
					break;
				case MemberName.Value:
					this.m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MIMEType:
					this.m_MIMEType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Sizing:
					this.m_sizing = (AspNetCore.ReportingServices.OnDemandReportRendering.Image.Sizings)reader.ReadEnum();
					break;
				case MemberName.Action:
					this.m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.Tag:
				{
					ExpressionInfo expressionInfo = (ExpressionInfo)reader.ReadRIFObject();
					if (expressionInfo != null)
					{
						this.m_tags = new List<ExpressionInfo>(1)
						{
							expressionInfo
						};
					}
					break;
				}
				case MemberName.Tags:
					this.m_tags = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.EmbeddingMode:
					this.m_embeddingMode = (AspNetCore.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes)reader.ReadEnum();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Image;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				this.m_exprHost = reportExprHost.ImageHostsRemotable[base.ExprHostID];
				base.ReportItemSetExprHost(this.m_exprHost, reportObjectModel);
				if (this.m_action != null && this.m_exprHost.ActionInfoHost != null)
				{
					this.m_action.SetExprHost(this.m_exprHost.ActionInfoHost, reportObjectModel);
				}
			}
		}

		internal bool ShouldTrackFieldsUsedInValue()
		{
			if (this.Action != null)
			{
				return this.Action.TrackFieldsUsedInValueExpression;
			}
			return false;
		}

		internal string EvaluateMimeTypeExpression(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateImageMIMETypeExpression(this);
		}

		internal byte[] EvaluateBinaryValueExpression(IReportScopeInstance romInstance, OnDemandProcessingContext context, out bool errOccurred)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateImageBinaryValueExpression(this, out errOccurred);
		}

		internal string EvaluateStringValueExpression(IReportScopeInstance romInstance, OnDemandProcessingContext context, out bool errOccurred)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateImageStringValueExpression(this, out errOccurred);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateTagExpression(ExpressionInfo tag, IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateImageTagExpression(this, tag);
		}
	}
}

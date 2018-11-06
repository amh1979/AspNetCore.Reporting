using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Image : ReportItem, IActionOwner
	{
		internal enum SourceType
		{
			External,
			Embedded,
			Database
		}

		internal enum Sizings
		{
			AutoSize,
			Fit,
			FitProportional,
			Clip
		}

		private Action m_action;

		private SourceType m_source;

		private ExpressionInfo m_value;

		private ExpressionInfo m_MIMEType;

		private Sizings m_sizing;

		[NonSerialized]
		private ImageExprHost m_exprHost;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		internal override ObjectType ObjectType
		{
			get
			{
				return ObjectType.Image;
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

		internal SourceType Source
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

		internal Sizings Sizing
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
				base.m_visibility.Initialize(context, false, false);
			}
			if (this.m_action != null)
			{
				this.m_action.Initialize(context);
			}
			if (this.m_value != null)
			{
				this.m_value.Initialize("Value", context);
				context.ExprHostBuilder.GenericValue(this.m_value);
				if (ExpressionInfo.Types.Constant == this.m_value.Type && this.m_source == SourceType.External && !context.ReportContext.IsSupportedProtocol(this.m_value.Value, true))
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsUnsupportedProtocol, Severity.Error, this.ObjectType, base.m_name, "Value", this.m_value.Value, "http://, https://, ftp://, file:, mailto:, or news:");
				}
			}
			if (this.m_MIMEType != null)
			{
				this.m_MIMEType.Initialize("MIMEType", context);
				context.ExprHostBuilder.ImageMIMEType(this.m_MIMEType);
			}
			if (SourceType.Embedded == this.m_source)
			{
				Global.Tracer.Assert(null != this.m_value);
				PublishingValidator.ValidateEmbeddedImageName(this.m_value, context.EmbeddedImages, this.ObjectType, base.m_name, "Value", context.ErrorContext);
			}
			base.ExprHostID = context.ExprHostBuilder.ImageEnd();
			return true;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				this.m_exprHost = reportExprHost.ImageHostsRemotable[base.ExprHostID];
				base.ReportItemSetExprHost(this.m_exprHost, reportObjectModel);
				if (this.m_action != null)
				{
					if (this.m_exprHost.ActionInfoHost != null)
					{
						this.m_action.SetExprHost(this.m_exprHost.ActionInfoHost, reportObjectModel);
					}
					else if (this.m_exprHost.ActionHost != null)
					{
						this.m_action.SetExprHost(this.m_exprHost.ActionHost, reportObjectModel);
					}
				}
			}
		}

		internal override void ProcessDrillthroughAction(ReportProcessing.ProcessingContext processingContext, NonComputedUniqueNames nonCompNames)
		{
			if (this.m_action != null && nonCompNames != null)
			{
				this.m_action.ProcessDrillthroughAction(processingContext, nonCompNames.UniqueName);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Action));
			memberInfoList.Add(new MemberInfo(MemberName.Source, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.MIMEType, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.Sizing, Token.Enum));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItem, memberInfoList);
		}
	}
}

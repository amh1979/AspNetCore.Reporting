using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Line : ReportItem, IPersistable
	{
		private const string ZeroSize = "0mm";

		private bool m_slanted;

		[NonSerialized]
		private ReportItemExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = Line.GetDeclaration();

		internal override AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Line;
			}
		}

		internal bool LineSlant
		{
			get
			{
				return this.m_slanted;
			}
			set
			{
				this.m_slanted = value;
			}
		}

		internal Line(ReportItem parent)
			: base(parent)
		{
		}

		internal Line(int id, ReportItem parent)
			: base(id, parent)
		{
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = this.ObjectType;
			context.ObjectName = base.m_name;
			context.ExprHostBuilder.LineStart(base.m_name);
			base.Initialize(context);
			double heightValue = base.m_heightValue;
			double widthValue = base.m_widthValue;
			double topValue = base.m_topValue;
			double leftValue = base.m_leftValue;
			if (0.0 > ReportItem.RoundSize(heightValue) && 0.0 <= ReportItem.RoundSize(widthValue))
			{
				goto IL_0094;
			}
			if (0.0 > ReportItem.RoundSize(widthValue) && 0.0 <= ReportItem.RoundSize(heightValue))
			{
				goto IL_0094;
			}
			goto IL_009b;
			IL_0094:
			this.m_slanted = true;
			goto IL_009b;
			IL_009b:
			base.m_heightValue = Math.Abs(heightValue);
			base.m_widthValue = Math.Abs(widthValue);
			if (0.0 <= heightValue)
			{
				base.m_topValue = topValue;
			}
			else
			{
				base.m_topValue = topValue + heightValue;
				if (0.0 > base.m_topValue)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsNegativeTopHeight, Severity.Error, context.ObjectType, context.ObjectName, null);
				}
			}
			if (0.0 <= widthValue)
			{
				base.m_leftValue = leftValue;
			}
			else
			{
				base.m_leftValue = leftValue + widthValue;
				if (0.0 > base.m_leftValue)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsNegativeLeftWidth, Severity.Error, context.ObjectType, context.ObjectName, null);
				}
			}
			if (base.m_visibility != null)
			{
				base.m_visibility.Initialize(context);
			}
			base.ExprHostID = context.ExprHostBuilder.LineEnd();
			return true;
		}

		internal override void CalculateSizes(double width, double height, InitializationContext context, bool overwrite)
		{
			if (overwrite)
			{
				base.m_top = "0mm";
				base.m_topValue = 0.0;
				base.m_left = "0mm";
				base.m_leftValue = 0.0;
			}
			if (base.m_width == null || (overwrite && base.m_widthValue > 0.0 && base.m_widthValue != width))
			{
				base.m_width = width.ToString("f5", CultureInfo.InvariantCulture) + "mm";
				base.m_widthValue = context.ValidateSize(ref base.m_width, "Width");
			}
			if (base.m_height != null)
			{
				if (!overwrite)
				{
					return;
				}
				if (!(base.m_heightValue > 0.0))
				{
					return;
				}
				if (base.m_heightValue == height)
				{
					return;
				}
			}
			base.m_height = height.ToString("f5", CultureInfo.InvariantCulture) + "mm";
			base.m_heightValue = context.ValidateSize(ref base.m_height, "Height");
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			return base.PublishClone(context);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Slanted, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Line, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(Line.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.Slanted)
				{
					writer.Write(this.m_slanted);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(Line.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.Slanted)
				{
					this.m_slanted = reader.ReadBoolean();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Line;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				this.m_exprHost = reportExprHost.LineHostsRemotable[base.ExprHostID];
				base.ReportItemSetExprHost(this.m_exprHost, reportObjectModel);
			}
		}
	}
}

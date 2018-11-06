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
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class TextRun : IDOwner, IPersistable, IStyleContainer, IActionOwner
	{
		private ExpressionInfo m_value;

		private ExpressionInfo m_toolTip;

		private Style m_styleClass;

		private Action m_action;

		private string m_label;

		private ExpressionInfo m_markupType;

		private DataType m_constantDataType = DataType.String;

		private int m_indexInCollection;

		private int m_exprHostID = -1;

		private bool m_valueReferenced;

		[Reference]
		private Paragraph m_paragraph;

		[NonSerialized]
		private string m_idString;

		[NonSerialized]
		private string m_name;

		[NonSerialized]
		private static readonly Declaration m_Declaration = TextRun.GetDeclaration();

		[NonSerialized]
		private TextRunImpl m_textRunImpl;

		[NonSerialized]
		private TextRunExprHost m_exprHost;

		[NonSerialized]
		private TypeCode m_valueType = TypeCode.String;

		[NonSerialized]
		private bool m_valueTypeSet;

		[NonSerialized]
		private Formatter m_formatter;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		internal string IDString
		{
			get
			{
				if (this.m_idString == null)
				{
					this.m_idString = this.m_paragraph.IDString + 'x' + this.m_indexInCollection.ToString(CultureInfo.InvariantCulture);
				}
				return this.m_idString;
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

		internal string Label
		{
			get
			{
				return this.m_label;
			}
			set
			{
				this.m_label = value;
			}
		}

		internal ExpressionInfo MarkupType
		{
			get
			{
				return this.m_markupType;
			}
			set
			{
				this.m_markupType = value;
			}
		}

		internal ExpressionInfo ToolTip
		{
			get
			{
				return this.m_toolTip;
			}
			set
			{
				this.m_toolTip = value;
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

		internal Paragraph Paragraph
		{
			get
			{
				return this.m_paragraph;
			}
			set
			{
				this.m_paragraph = value;
			}
		}

		internal int IndexInCollection
		{
			get
			{
				return this.m_indexInCollection;
			}
			set
			{
				this.m_indexInCollection = value;
			}
		}

		internal DataType DataType
		{
			get
			{
				return this.m_constantDataType;
			}
			set
			{
				this.m_constantDataType = value;
			}
		}

		internal bool ValueReferenced
		{
			get
			{
				return this.m_valueReferenced;
			}
			set
			{
				this.m_valueReferenced = value;
			}
		}

		IInstancePath IStyleContainer.InstancePath
		{
			get
			{
				return this.m_paragraph.TextBox;
			}
		}

		Style IStyleContainer.StyleClass
		{
			get
			{
				return this.m_styleClass;
			}
		}

		public AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.TextRun;
			}
		}

		public string Name
		{
			get
			{
				if (this.m_name == null)
				{
					this.m_name = this.m_paragraph.Name + ".TextRuns[" + this.m_indexInCollection.ToString(CultureInfo.InvariantCulture) + "]";
				}
				return this.m_name;
			}
		}

		internal TypeCode ValueTypeCode
		{
			get
			{
				if (!this.m_valueTypeSet)
				{
					this.m_valueTypeSet = true;
					if (this.m_value == null)
					{
						this.m_valueType = TypeCode.String;
					}
					else if (!this.m_value.IsExpression)
					{
						this.m_valueType = this.m_value.ConstantTypeCode;
					}
					else
					{
						this.m_valueType = TypeCode.Object;
					}
				}
				return this.m_valueType;
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

		internal TextRunExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal TextRun(Paragraph paragraph, int index, int id)
			: base(id)
		{
			this.m_indexInCollection = index;
			this.m_paragraph = paragraph;
		}

		internal TextRun()
		{
		}

		internal bool Initialize(InitializationContext context, out bool hasExpressionBasedValue)
		{
			bool result = false;
			hasExpressionBasedValue = false;
			context.ExprHostBuilder.TextRunStart(this.m_indexInCollection);
			if (this.m_value != null)
			{
				result = true;
				hasExpressionBasedValue = this.m_value.IsExpression;
				this.m_value.Initialize("Value", context);
				context.ExprHostBuilder.TextRunValue(this.m_value);
			}
			if (this.m_toolTip != null)
			{
				this.m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.TextRunToolTip(this.m_toolTip);
			}
			if (this.m_markupType != null)
			{
				this.m_markupType.Initialize("MarkupType", context);
				context.ExprHostBuilder.TextRunMarkupType(this.m_markupType);
			}
			if (this.m_action != null)
			{
				this.m_action.Initialize(context);
			}
			if (this.m_styleClass != null)
			{
				this.m_styleClass.Initialize(context);
			}
			this.m_exprHostID = context.ExprHostBuilder.TextRunEnd();
			return result;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			TextRun textRun = (TextRun)base.PublishClone(context);
			if (this.m_value != null)
			{
				textRun.m_value = (ExpressionInfo)this.m_value.PublishClone(context);
			}
			if (this.m_toolTip != null)
			{
				textRun.m_toolTip = (ExpressionInfo)this.m_toolTip.PublishClone(context);
			}
			if (this.m_styleClass != null)
			{
				textRun.m_styleClass = (Style)this.m_styleClass.PublishClone(context);
			}
			if (this.m_markupType != null)
			{
				textRun.m_markupType = (ExpressionInfo)this.m_markupType.PublishClone(context);
			}
			if (this.m_action != null)
			{
				textRun.m_action = (Action)this.m_action.PublishClone(context);
			}
			return textRun;
		}

		internal bool DetermineSimplicity()
		{
			if (this.m_markupType != null && (this.m_markupType.IsExpression || !string.Equals(this.m_markupType.StringValue, "None", StringComparison.Ordinal)))
			{
				goto IL_0091;
			}
			TextBox textBox = this.m_paragraph.TextBox;
			if (this.m_action != null && textBox.Action != null)
			{
				goto IL_0091;
			}
			if (this.m_toolTip != null && textBox.ToolTip != null)
			{
				goto IL_0091;
			}
			if (this.m_action != null)
			{
				textBox.Action = this.m_action;
				this.m_action = null;
			}
			if (this.m_toolTip != null)
			{
				textBox.ToolTip = this.m_toolTip;
				this.m_toolTip = null;
			}
			return true;
			IL_0091:
			return false;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Label, Token.String));
			list.Add(new MemberInfo(MemberName.MarkupType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Style, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style));
			list.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.DataType, Token.Enum));
			list.Add(new MemberInfo(MemberName.IndexInCollection, Token.Int32));
			list.Add(new MemberInfo(MemberName.Paragraph, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Paragraph, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ValueReferenced, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextRun, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(TextRun.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Value:
					writer.Write(this.m_value);
					break;
				case MemberName.ToolTip:
					writer.Write(this.m_toolTip);
					break;
				case MemberName.Style:
					writer.Write(this.m_styleClass);
					break;
				case MemberName.Label:
					writer.Write(this.m_label);
					break;
				case MemberName.MarkupType:
					writer.Write(this.m_markupType);
					break;
				case MemberName.Action:
					writer.Write(this.m_action);
					break;
				case MemberName.DataType:
					writer.WriteEnum((int)this.m_constantDataType);
					break;
				case MemberName.IndexInCollection:
					writer.Write(this.m_indexInCollection);
					break;
				case MemberName.Paragraph:
					writer.WriteReference(this.m_paragraph);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.ValueReferenced:
					writer.Write(this.m_valueReferenced);
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
			reader.RegisterDeclaration(TextRun.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Value:
					this.m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					this.m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Style:
					this.m_styleClass = (Style)reader.ReadRIFObject();
					break;
				case MemberName.Label:
					this.m_label = reader.ReadString();
					break;
				case MemberName.MarkupType:
					this.m_markupType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Action:
					this.m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.DataType:
					this.m_constantDataType = (DataType)reader.ReadEnum();
					break;
				case MemberName.IndexInCollection:
					this.m_indexInCollection = reader.ReadInt32();
					break;
				case MemberName.Paragraph:
					this.m_paragraph = reader.ReadReference<Paragraph>(this);
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.ValueReferenced:
					this.m_valueReferenced = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(TextRun.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.Paragraph)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						Global.Tracer.Assert(referenceableItems[item.RefID] is Paragraph);
						this.m_paragraph = (Paragraph)referenceableItems[item.RefID];
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextRun;
		}

		internal void SetExprHost(TextRunExprHost textRunExprHost)
		{
			this.m_exprHost = textRunExprHost;
		}

		internal void SetExprHost(ParagraphExprHost paragraphExprHost, ObjectModelImpl reportObjectModel)
		{
			if (this.m_exprHostID >= 0)
			{
				this.m_exprHost = paragraphExprHost.TextRunHostsRemotable[this.m_exprHostID];
				Global.Tracer.Assert(this.m_exprHost != null && reportObjectModel != null);
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
				if (this.m_action != null && this.m_exprHost.ActionInfoHost != null)
				{
					this.m_action.SetExprHost(this.m_exprHost.ActionInfoHost, reportObjectModel);
				}
				if (this.m_styleClass != null)
				{
					this.m_styleClass.SetStyleExprHost(this.m_exprHost);
				}
			}
		}

		internal string EvaluateMarkupType(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_paragraph.TextBox, instance);
			return context.ReportRuntime.EvaluateTextRunMarkupTypeExpression(this);
		}

		internal string EvaluateToolTip(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_paragraph.TextBox, instance);
			return context.ReportRuntime.EvaluateTextRunToolTipExpression(this);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateValue(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			return this.GetTextRunImpl(context).GetResult(instance);
		}

		internal List<string> GetFieldsUsedInValueExpression(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			return this.GetTextRunImpl(context).GetFieldsUsedInValueExpression(romInstance);
		}

		private TextRunImpl GetTextRunImpl(OnDemandProcessingContext context)
		{
			if (this.m_textRunImpl == null)
			{
				this.m_textRunImpl = (TextRunImpl)this.m_paragraph.GetParagraphImpl(context).TextRuns[this.m_indexInCollection];
			}
			return this.m_textRunImpl;
		}

		internal string FormatTextRunValue(AspNetCore.ReportingServices.RdlExpressions.VariantResult textRunResult, OnDemandProcessingContext context)
		{
			string result = null;
			if (textRunResult.ErrorOccurred)
			{
				result = RPRes.rsExpressionErrorValue;
			}
			else if (textRunResult.Value != null)
			{
				result = this.FormatTextRunValue(textRunResult.Value, textRunResult.TypeCode, null, context);
			}
			return result;
		}

		internal string FormatTextRunValue(object textRunValue, TypeCode typeCode, OnDemandProcessingContext context)
		{
			return this.FormatTextRunValue(textRunValue, typeCode, string.Empty, context);
		}

		private string FormatTextRunValue(object textRunValue, TypeCode typeCode, string formatCode, OnDemandProcessingContext context)
		{
			if (this.m_formatter == null)
			{
				this.m_formatter = new Formatter(this.m_styleClass, context, AspNetCore.ReportingServices.ReportProcessing.ObjectType.TextRun, this.Name);
			}
			return this.m_formatter.FormatValue(textRunValue, formatCode, typeCode);
		}
	}
}

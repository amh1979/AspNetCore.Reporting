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
	internal sealed class Paragraph : IDOwner, IPersistable, IStyleContainer
	{
		private List<TextRun> m_textRuns;

		private Style m_styleClass;

		private ExpressionInfo m_leftIndent;

		private ExpressionInfo m_rightIndent;

		private ExpressionInfo m_hangingIndent;

		private ExpressionInfo m_spaceBefore;

		private ExpressionInfo m_spaceAfter;

		private ExpressionInfo m_listLevel;

		private ExpressionInfo m_listStyle;

		private int m_indexInCollection;

		private int m_exprHostID = -1;

		private bool m_textRunValueReferenced;

		[Reference]
		private TextBox m_textBox;

		[NonSerialized]
		private string m_idString;

		[NonSerialized]
		private ParagraphImpl m_paragraphImpl;

		[NonSerialized]
		private string m_name;

		[NonSerialized]
		private ParagraphExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = Paragraph.GetDeclaration();

		internal string IDString
		{
			get
			{
				if (this.m_idString == null)
				{
					this.m_idString = this.m_textBox.GlobalID.ToString(CultureInfo.InvariantCulture) + 'x' + this.m_indexInCollection.ToString(CultureInfo.InvariantCulture);
				}
				return this.m_idString;
			}
		}

		internal List<TextRun> TextRuns
		{
			get
			{
				return this.m_textRuns;
			}
			set
			{
				this.m_textRuns = value;
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

		internal ExpressionInfo LeftIndent
		{
			get
			{
				return this.m_leftIndent;
			}
			set
			{
				this.m_leftIndent = value;
			}
		}

		internal ExpressionInfo RightIndent
		{
			get
			{
				return this.m_rightIndent;
			}
			set
			{
				this.m_rightIndent = value;
			}
		}

		internal ExpressionInfo HangingIndent
		{
			get
			{
				return this.m_hangingIndent;
			}
			set
			{
				this.m_hangingIndent = value;
			}
		}

		internal ExpressionInfo SpaceBefore
		{
			get
			{
				return this.m_spaceBefore;
			}
			set
			{
				this.m_spaceBefore = value;
			}
		}

		internal ExpressionInfo SpaceAfter
		{
			get
			{
				return this.m_spaceAfter;
			}
			set
			{
				this.m_spaceAfter = value;
			}
		}

		internal ExpressionInfo ListStyle
		{
			get
			{
				return this.m_listStyle;
			}
			set
			{
				this.m_listStyle = value;
			}
		}

		internal ExpressionInfo ListLevel
		{
			get
			{
				return this.m_listLevel;
			}
			set
			{
				this.m_listLevel = value;
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

		internal TextBox TextBox
		{
			get
			{
				return this.m_textBox;
			}
			set
			{
				this.m_textBox = value;
			}
		}

		internal bool TextRunValueReferenced
		{
			get
			{
				return this.m_textRunValueReferenced;
			}
			set
			{
				this.m_textRunValueReferenced = value;
			}
		}

		IInstancePath IStyleContainer.InstancePath
		{
			get
			{
				return this.m_textBox;
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
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Paragraph;
			}
		}

		public string Name
		{
			get
			{
				if (this.m_name == null)
				{
					this.m_name = this.m_textBox.Name + ".Paragraphs[" + this.m_indexInCollection.ToString(CultureInfo.InvariantCulture) + "]";
				}
				return this.m_name;
			}
		}

		internal ParagraphExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal Paragraph(TextBox textbox, int index, int id)
			: base(id)
		{
			this.m_indexInCollection = index;
			this.m_textBox = textbox;
			this.m_textRuns = new List<TextRun>();
		}

		internal Paragraph()
		{
		}

		internal bool Initialize(InitializationContext context, out bool aHasExpressionBasedValue)
		{
			bool flag = false;
			bool flag2 = false;
			aHasExpressionBasedValue = false;
			context.ExprHostBuilder.ParagraphStart(this.m_indexInCollection);
			if (this.m_textRuns != null)
			{
				foreach (TextRun textRun in this.m_textRuns)
				{
					flag |= textRun.Initialize(context, out flag2);
					aHasExpressionBasedValue |= flag2;
				}
			}
			if (this.m_styleClass != null)
			{
				this.m_styleClass.Initialize(context);
			}
			if (this.m_leftIndent != null)
			{
				this.m_leftIndent.Initialize("LeftIndent", context);
				context.ExprHostBuilder.ParagraphLeftIndent(this.m_leftIndent);
			}
			if (this.m_rightIndent != null)
			{
				this.m_rightIndent.Initialize("RightIndent", context);
				context.ExprHostBuilder.ParagraphRightIndent(this.m_rightIndent);
			}
			if (this.m_hangingIndent != null)
			{
				this.m_hangingIndent.Initialize("HangingIndent", context);
				context.ExprHostBuilder.ParagraphHangingIndent(this.m_hangingIndent);
			}
			if (this.m_spaceBefore != null)
			{
				this.m_spaceBefore.Initialize("SpaceBefore", context);
				context.ExprHostBuilder.ParagraphSpaceBefore(this.m_spaceBefore);
			}
			if (this.m_spaceAfter != null)
			{
				this.m_spaceAfter.Initialize("SpaceAfter", context);
				context.ExprHostBuilder.ParagraphSpaceAfter(this.m_spaceAfter);
			}
			if (this.m_listStyle != null)
			{
				this.m_listStyle.Initialize("ListStyle", context);
				context.ExprHostBuilder.ParagraphListStyle(this.m_listStyle);
			}
			if (this.m_listLevel != null)
			{
				this.m_listLevel.Initialize("ListLevel", context);
				context.ExprHostBuilder.ParagraphListLevel(this.m_listLevel);
			}
			this.m_exprHostID = context.ExprHostBuilder.ParagraphEnd();
			return flag;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			Paragraph paragraph = (Paragraph)base.PublishClone(context);
			if (this.m_textRuns != null)
			{
				paragraph.m_textRuns = new List<TextRun>(this.m_textRuns.Count);
				foreach (TextRun textRun2 in this.m_textRuns)
				{
					TextRun textRun = (TextRun)textRun2.PublishClone(context);
					textRun.Paragraph = paragraph;
					paragraph.m_textRuns.Add(textRun);
				}
			}
			if (this.m_styleClass != null)
			{
				paragraph.m_styleClass = (Style)this.m_styleClass.PublishClone(context);
			}
			if (this.m_leftIndent != null)
			{
				paragraph.m_leftIndent = (ExpressionInfo)this.m_leftIndent.PublishClone(context);
			}
			if (this.m_rightIndent != null)
			{
				paragraph.m_rightIndent = (ExpressionInfo)this.m_rightIndent.PublishClone(context);
			}
			if (this.m_hangingIndent != null)
			{
				paragraph.m_hangingIndent = (ExpressionInfo)this.m_hangingIndent.PublishClone(context);
			}
			if (this.m_spaceBefore != null)
			{
				paragraph.m_spaceBefore = (ExpressionInfo)this.m_spaceBefore.PublishClone(context);
			}
			if (this.m_spaceAfter != null)
			{
				paragraph.m_spaceAfter = (ExpressionInfo)this.m_spaceAfter.PublishClone(context);
			}
			if (this.m_listStyle != null)
			{
				paragraph.m_listStyle = (ExpressionInfo)this.m_listStyle.PublishClone(context);
			}
			if (this.m_listLevel != null)
			{
				paragraph.m_listLevel = (ExpressionInfo)this.m_listLevel.PublishClone(context);
			}
			return paragraph;
		}

		internal bool DetermineSimplicity()
		{
			if (this.m_textRuns.Count == 1 && this.m_listLevel == null && this.m_listStyle == null && this.m_leftIndent == null && this.m_rightIndent == null && this.m_hangingIndent == null && this.m_spaceBefore == null && this.m_spaceAfter == null)
			{
				return this.m_textRuns[0].DetermineSimplicity();
			}
			return false;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Style, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style));
			list.Add(new MemberInfo(MemberName.TextRuns, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextRun));
			list.Add(new MemberInfo(MemberName.LeftIndent, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.RightIndent, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.HangingIndent, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SpaceBefore, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SpaceAfter, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ListStyle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ListLevel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IndexInCollection, Token.Int32));
			list.Add(new MemberInfo(MemberName.TextBox, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.TextRunValueReferenced, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Paragraph, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(Paragraph.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.TextRuns:
					writer.Write(this.m_textRuns);
					break;
				case MemberName.Style:
					writer.Write(this.m_styleClass);
					break;
				case MemberName.LeftIndent:
					writer.Write(this.m_leftIndent);
					break;
				case MemberName.RightIndent:
					writer.Write(this.m_rightIndent);
					break;
				case MemberName.HangingIndent:
					writer.Write(this.m_hangingIndent);
					break;
				case MemberName.SpaceBefore:
					writer.Write(this.m_spaceBefore);
					break;
				case MemberName.SpaceAfter:
					writer.Write(this.m_spaceAfter);
					break;
				case MemberName.ListStyle:
					writer.Write(this.m_listStyle);
					break;
				case MemberName.ListLevel:
					writer.Write(this.m_listLevel);
					break;
				case MemberName.IndexInCollection:
					writer.Write(this.m_indexInCollection);
					break;
				case MemberName.TextBox:
					writer.WriteReference(this.m_textBox);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.TextRunValueReferenced:
					writer.Write(this.m_textRunValueReferenced);
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
			reader.RegisterDeclaration(Paragraph.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.TextRuns:
					this.m_textRuns = reader.ReadGenericListOfRIFObjects<TextRun>();
					break;
				case MemberName.Style:
					this.m_styleClass = (Style)reader.ReadRIFObject();
					break;
				case MemberName.LeftIndent:
					this.m_leftIndent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.RightIndent:
					this.m_rightIndent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HangingIndent:
					this.m_hangingIndent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SpaceBefore:
					this.m_spaceBefore = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SpaceAfter:
					this.m_spaceAfter = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ListStyle:
					this.m_listStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ListLevel:
					this.m_listLevel = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IndexInCollection:
					this.m_indexInCollection = reader.ReadInt32();
					break;
				case MemberName.TextBox:
					this.m_textBox = reader.ReadReference<TextBox>(this);
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.TextRunValueReferenced:
					this.m_textRunValueReferenced = reader.ReadBoolean();
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
			if (memberReferencesCollection.TryGetValue(Paragraph.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.TextBox)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						Global.Tracer.Assert(referenceableItems[item.RefID] is TextBox);
						this.m_textBox = (TextBox)referenceableItems[item.RefID];
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Paragraph;
		}

		internal void SetExprHost(TextBoxExprHost textBoxExprHost, ObjectModelImpl reportObjectModel)
		{
			if (this.m_exprHostID >= 0)
			{
				this.m_exprHost = textBoxExprHost.ParagraphHostsRemotable[this.m_exprHostID];
				Global.Tracer.Assert(this.m_exprHost != null && reportObjectModel != null);
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
				if (this.m_styleClass != null)
				{
					this.m_styleClass.SetStyleExprHost(this.m_exprHost);
				}
				if (this.m_textRuns != null)
				{
					foreach (TextRun textRun in this.m_textRuns)
					{
						textRun.SetExprHost(this.m_exprHost, reportObjectModel);
					}
				}
			}
			else if (base.m_ID == -1)
			{
				if (this.m_styleClass != null)
				{
					this.m_styleClass.SetStyleExprHost(textBoxExprHost);
					this.m_textRuns[0].StyleClass.SetStyleExprHost(textBoxExprHost);
				}
				this.m_textRuns[0].SetExprHost(new AspNetCore.ReportingServices.RdlExpressions.ReportRuntime.TextRunExprHostWrapper(textBoxExprHost));
			}
		}

		internal string EvaluateSpaceAfter(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_textBox, instance);
			return context.ReportRuntime.EvaluateParagraphSpaceAfterExpression(this);
		}

		internal string EvaluateSpaceBefore(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_textBox, instance);
			return context.ReportRuntime.EvaluateParagraphSpaceBeforeExpression(this);
		}

		internal string EvaluateListStyle(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_textBox, instance);
			return context.ReportRuntime.EvaluateParagraphListStyleExpression(this);
		}

		internal int? EvaluateListLevel(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_textBox, instance);
			return context.ReportRuntime.EvaluateParagraphListLevelExpression(this);
		}

		internal string EvaluateLeftIndent(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_textBox, instance);
			return context.ReportRuntime.EvaluateParagraphLeftIndentExpression(this);
		}

		internal string EvaluateRightIndent(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_textBox, instance);
			return context.ReportRuntime.EvaluateParagraphRightIndentExpression(this);
		}

		internal string EvaluateHangingIndent(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_textBox, instance);
			return context.ReportRuntime.EvaluateParagraphHangingIndentExpression(this);
		}

		internal ParagraphImpl GetParagraphImpl(OnDemandProcessingContext context)
		{
			if (this.m_paragraphImpl == null)
			{
				this.m_paragraphImpl = (ParagraphImpl)this.m_textBox.GetTextBoxImpl(context).Paragraphs[this.m_indexInCollection];
			}
			return this.m_paragraphImpl;
		}
	}
}

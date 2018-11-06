using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Style
	{
		private StyleAttributeHashtable m_styleAttributes;

		private ExpressionInfoList m_expressionList;

		[NonSerialized]
		private StyleExprHost m_exprHost;

		[NonSerialized]
		private int m_customSharedStyleCount = -1;

		internal StyleAttributeHashtable StyleAttributes
		{
			get
			{
				return this.m_styleAttributes;
			}
			set
			{
				this.m_styleAttributes = value;
			}
		}

		internal ExpressionInfoList ExpressionList
		{
			get
			{
				return this.m_expressionList;
			}
			set
			{
				this.m_expressionList = value;
			}
		}

		internal StyleExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal int CustomSharedStyleCount
		{
			get
			{
				return this.m_customSharedStyleCount;
			}
			set
			{
				this.m_customSharedStyleCount = value;
			}
		}

		internal Style(ConstructionPhase phase)
		{
			if (phase == ConstructionPhase.Publishing)
			{
				this.m_styleAttributes = new StyleAttributeHashtable();
			}
		}

		internal void AddAttribute(string name, ExpressionInfo expressionInfo)
		{
			AttributeInfo attributeInfo = new AttributeInfo();
			attributeInfo.IsExpression = (ExpressionInfo.Types.Constant != expressionInfo.Type);
			if (attributeInfo.IsExpression)
			{
				if (this.m_expressionList == null)
				{
					this.m_expressionList = new ExpressionInfoList();
				}
				attributeInfo.IntValue = this.m_expressionList.Add(expressionInfo);
			}
			else
			{
				attributeInfo.Value = expressionInfo.Value;
				attributeInfo.BoolValue = expressionInfo.BoolValue;
				attributeInfo.IntValue = expressionInfo.IntValue;
			}
			Global.Tracer.Assert(null != this.m_styleAttributes);
			this.m_styleAttributes.Add(name, attributeInfo);
		}

		internal void Initialize(InitializationContext context)
		{
			Global.Tracer.Assert(null != this.m_styleAttributes);
			IDictionaryEnumerator enumerator = this.m_styleAttributes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = (string)enumerator.Key;
				AttributeInfo attributeInfo = (AttributeInfo)enumerator.Value;
				Global.Tracer.Assert(null != text);
				Global.Tracer.Assert(null != attributeInfo);
				if (attributeInfo.IsExpression)
				{
					string name = text;
					switch (text)
					{
					case "BorderColorLeft":
					case "BorderColorRight":
					case "BorderColorTop":
					case "BorderColorBottom":
						text = "BorderColor";
						break;
					case "BorderStyleLeft":
					case "BorderStyleRight":
					case "BorderStyleTop":
					case "BorderStyleBottom":
						text = "BorderStyle";
						break;
					case "BorderWidthLeft":
					case "BorderWidthRight":
					case "BorderWidthTop":
					case "BorderWidthBottom":
						text = "BorderWidth";
						break;
					}
					Global.Tracer.Assert(null != this.m_expressionList);
					ExpressionInfo expressionInfo = this.m_expressionList[attributeInfo.IntValue];
					expressionInfo.Initialize(text, context);
					context.ExprHostBuilder.StyleAttribute(name, expressionInfo);
				}
			}
			AttributeInfo attributeInfo2 = this.m_styleAttributes["BackgroundImageSource"];
			if (attributeInfo2 != null)
			{
				Global.Tracer.Assert(!attributeInfo2.IsExpression);
				Image.SourceType intValue = (Image.SourceType)attributeInfo2.IntValue;
				if (Image.SourceType.Embedded == intValue)
				{
					AttributeInfo attributeInfo3 = this.m_styleAttributes["BackgroundImageValue"];
					Global.Tracer.Assert(null != attributeInfo3);
					PublishingValidator.ValidateEmbeddedImageName(attributeInfo3, context.EmbeddedImages, context.ObjectType, context.ObjectName, "BackgroundImageValue", context.ErrorContext);
				}
				else if (intValue == Image.SourceType.External)
				{
					AttributeInfo attributeInfo4 = this.m_styleAttributes["BackgroundImageValue"];
					Global.Tracer.Assert(null != attributeInfo4);
					if (!attributeInfo4.IsExpression)
					{
						context.ImageStreamNames[attributeInfo4.Value] = new ImageInfo(context.ObjectName, null);
					}
				}
			}
			context.CheckInternationalSettings(this.m_styleAttributes);
		}

		internal void SetStyleExprHost(StyleExprHost exprHost)
		{
			Global.Tracer.Assert(null != exprHost);
			this.m_exprHost = exprHost;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.StyleAttributes, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.StyleAttributeHashtable));
			memberInfoList.Add(new MemberInfo(MemberName.ExpressionList, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}

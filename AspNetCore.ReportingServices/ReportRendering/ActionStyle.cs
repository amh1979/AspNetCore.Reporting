using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ActionStyle : StyleBase
	{
		private ActionInfo m_actionInfo;

		public override int Count
		{
			get
			{
				if (base.IsCustomControl)
				{
					return base.Count;
				}
				if (this.m_actionInfo.ActionInfoDef.StyleClass == null)
				{
					return 0;
				}
				return base.Count;
			}
		}

		public override ICollection Keys
		{
			get
			{
				if (base.IsCustomControl)
				{
					return base.Keys;
				}
				if (this.m_actionInfo.ActionInfoDef.StyleClass == null)
				{
					return null;
				}
				return base.Keys;
			}
		}

		public override object this[string styleName]
		{
			get
			{
				if (base.IsCustomControl)
				{
					object obj = null;
					if (base.m_nonSharedProperties != null)
					{
						obj = base.m_nonSharedProperties[styleName];
					}
					if (obj == null && base.m_sharedProperties != null)
					{
						obj = base.m_sharedProperties[styleName];
					}
					return this.CreateProperty(styleName, obj);
				}
				Global.Tracer.Assert(!base.IsCustomControl);
				if (this.m_actionInfo.ActionInfoDef.StyleClass == null)
				{
					return null;
				}
				StyleAttributeHashtable styleAttributes = this.m_actionInfo.ActionInfoDef.StyleClass.StyleAttributes;
				AttributeInfo attributeInfo = null;
				if ("BackgroundImage" == styleName)
				{
					Image.SourceType imageSource = Image.SourceType.External;
					object obj2 = null;
					object obj3 = null;
					bool flag = false;
					bool flag2 = default(bool);
					base.GetBackgroundImageProperties(styleAttributes["BackgroundImageSource"], styleAttributes["BackgroundImageValue"], styleAttributes["BackgroundImageMIMEType"], out imageSource, out obj2, out flag2, out obj3, out flag);
					if (obj2 != null)
					{
						string mimeType = null;
						if (!flag)
						{
							mimeType = (string)obj3;
						}
						return new BackgroundImage(base.m_renderingContext, imageSource, obj2, mimeType);
					}
				}
				else
				{
					attributeInfo = styleAttributes[styleName];
					if (attributeInfo != null)
					{
						return this.CreateProperty(styleName, this.GetStyleAttributeValue(styleName, attributeInfo));
					}
				}
				return null;
			}
		}

		public override StyleProperties SharedProperties
		{
			get
			{
				if (base.IsCustomControl)
				{
					return base.m_sharedProperties;
				}
				if (this.NeedPopulateSharedProps())
				{
					this.PopulateStyleProperties(false);
					this.m_actionInfo.ActionInfoDef.SharedStyleProperties = base.m_sharedProperties;
				}
				return base.m_sharedProperties;
			}
		}

		public override StyleProperties NonSharedProperties
		{
			get
			{
				if (base.IsCustomControl)
				{
					return base.m_nonSharedProperties;
				}
				if (this.NeedPopulateNonSharedProps())
				{
					this.PopulateNonSharedStyleProperties();
					if (base.m_nonSharedProperties == null || base.m_nonSharedProperties.Count == 0)
					{
						this.m_actionInfo.ActionInfoDef.NoNonSharedStyleProps = true;
					}
				}
				return base.m_nonSharedProperties;
			}
		}

		public ActionStyle()
		{
			Global.Tracer.Assert(base.IsCustomControl);
		}

		internal ActionStyle(ActionInfo actionInfo, RenderingContext context)
			: base(context)
		{
			Global.Tracer.Assert(!base.IsCustomControl);
			this.m_actionInfo = actionInfo;
		}

		private bool NeedPopulateSharedProps()
		{
			if (base.m_sharedProperties != null)
			{
				return false;
			}
			if (this.m_actionInfo.ActionInfoDef.SharedStyleProperties != null)
			{
				base.m_sharedProperties = this.m_actionInfo.ActionInfoDef.SharedStyleProperties;
				return false;
			}
			return true;
		}

		private bool NeedPopulateNonSharedProps()
		{
			if (base.m_nonSharedProperties == null && !this.m_actionInfo.ActionInfoDef.NoNonSharedStyleProps)
			{
				return true;
			}
			return false;
		}

		internal override object GetStyleAttributeValue(string styleName, AttributeInfo attribute)
		{
			Global.Tracer.Assert(!base.IsCustomControl);
			if (attribute.IsExpression)
			{
				ActionInstance actionInfoInstance = this.m_actionInfo.ActionInfoInstance;
				if (actionInfoInstance != null)
				{
					return actionInfoInstance.GetStyleAttributeValue(attribute.IntValue);
				}
				return null;
			}
			if ("NumeralVariant" == styleName)
			{
				return attribute.IntValue;
			}
			return attribute.Value;
		}

		internal override void PopulateStyleProperties(bool populateAll)
		{
			if (!base.IsCustomControl)
			{
				bool flag = true;
				bool flag2 = false;
				if (populateAll)
				{
					flag = this.NeedPopulateSharedProps();
					flag2 = this.NeedPopulateNonSharedProps();
					if (!flag && !flag2)
					{
						return;
					}
				}
				AspNetCore.ReportingServices.ReportProcessing.Style styleClass = this.m_actionInfo.ActionInfoDef.StyleClass;
				StyleAttributeHashtable styleAttributeHashtable = null;
				if (styleClass != null)
				{
					styleAttributeHashtable = styleClass.StyleAttributes;
				}
				Global.Tracer.Assert(null != styleAttributeHashtable);
				IDictionaryEnumerator enumerator = styleAttributeHashtable.GetEnumerator();
				while (enumerator.MoveNext())
				{
					AttributeInfo attributeInfo = (AttributeInfo)enumerator.Value;
					string text = (string)enumerator.Key;
					if ("BackgroundImage" == text)
					{
						Image.SourceType imageSource = Image.SourceType.External;
						object obj = null;
						object obj2 = null;
						bool flag3 = false;
						bool flag4 = false;
						base.GetBackgroundImageProperties(styleAttributeHashtable["BackgroundImageSource"], styleAttributeHashtable["BackgroundImageValue"], styleAttributeHashtable["BackgroundImageMIMEType"], out imageSource, out obj, out flag3, out obj2, out flag4);
						if (obj != null)
						{
							string mimeType = null;
							if (!flag4)
							{
								mimeType = (string)obj2;
							}
							object styleProperty = new BackgroundImage(base.m_renderingContext, imageSource, obj, mimeType);
							base.AddStyleProperty(text, flag3 | flag4, flag2, flag, styleProperty);
						}
					}
					else if (!("BackgroundImageValue" == text) && !("BackgroundImageMIMEType" == text))
					{
						base.AddStyleProperty(text, attributeInfo.IsExpression, flag2, flag, this.CreateProperty(text, this.GetStyleAttributeValue(text, attributeInfo)));
					}
				}
			}
		}

		private void PopulateNonSharedStyleProperties()
		{
			if (!base.IsCustomControl)
			{
				AspNetCore.ReportingServices.ReportProcessing.Style styleClass = this.m_actionInfo.ActionInfoDef.StyleClass;
				if (styleClass != null)
				{
					StyleAttributeHashtable styleAttributes = styleClass.StyleAttributes;
					Global.Tracer.Assert(null != styleAttributes);
					this.InternalPopulateNonSharedStyleProperties(styleAttributes);
				}
			}
		}

		private void InternalPopulateNonSharedStyleProperties(StyleAttributeHashtable styleAttributes)
		{
			if (styleAttributes != null)
			{
				IDictionaryEnumerator enumerator = styleAttributes.GetEnumerator();
				while (enumerator.MoveNext())
				{
					AttributeInfo attributeInfo = (AttributeInfo)enumerator.Value;
					string text = (string)enumerator.Key;
					if ("BackgroundImageSource" == text)
					{
						object obj = default(object);
						Image.SourceType imageSource = default(Image.SourceType);
						bool flag = default(bool);
						object obj2 = default(object);
						bool flag2 = default(bool);
						if (base.GetBackgroundImageProperties(attributeInfo, styleAttributes["BackgroundImageValue"], styleAttributes["BackgroundImageMIMEType"], out imageSource, out obj, out flag, out obj2, out flag2) && (flag | flag2) && obj != null)
						{
							string mimeType = null;
							if (!flag2)
							{
								mimeType = (string)obj2;
							}
							object styleProperty = new BackgroundImage(base.m_renderingContext, imageSource, obj, mimeType);
							base.SetStyleProperty("BackgroundImage", true, true, false, styleProperty);
						}
					}
					else if (!("BackgroundImageValue" == text) && !("BackgroundImageMIMEType" == text) && attributeInfo.IsExpression)
					{
						base.SetStyleProperty(text, true, true, false, this.CreateProperty(text, this.GetStyleAttributeValue(text, attributeInfo)));
					}
				}
			}
		}

		private object CreateProperty(string styleName, object styleValue)
		{
			if (styleValue == null)
			{
				return null;
			}
			return StyleBase.CreateStyleProperty(styleName, styleValue);
		}
	}
}

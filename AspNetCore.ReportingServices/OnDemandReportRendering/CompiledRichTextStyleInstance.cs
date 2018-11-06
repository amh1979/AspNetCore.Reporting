using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class CompiledRichTextStyleInstance : StyleInstance, ICompiledStyleInstance
	{
		private List<StyleAttributeNames> m_nonSharedStyles;

		public override List<StyleAttributeNames> StyleAttributes
		{
			get
			{
				if (this.m_nonSharedStyles == null)
				{
					this.CompleteStyle();
				}
				return this.m_nonSharedStyles;
			}
		}

		internal CompiledRichTextStyleInstance(IROMStyleDefinitionContainer styleDefinitionContainer, IReportScope reportScope, RenderingContext context)
			: base(styleDefinitionContainer, reportScope, context)
		{
		}

		private void CompleteStyle()
		{
			List<StyleAttributeNames> styleAttributes = base.StyleAttributes;
			Dictionary<StyleAttributeNames, bool> dictionary = null;
			if (styleAttributes != null)
			{
				dictionary = new Dictionary<StyleAttributeNames, bool>(styleAttributes.Count);
				foreach (StyleAttributeNames item in styleAttributes)
				{
					dictionary[item] = true;
				}
			}
			else
			{
				this.m_nonSharedStyles = new List<StyleAttributeNames>();
			}
			if (base.m_assignedValues != null)
			{
				foreach (KeyValuePair<StyleAttributeNames, bool> assignedValue in base.m_assignedValues)
				{
					if (assignedValue.Value)
					{
						StyleAttributeNames key = assignedValue.Key;
						if (dictionary == null)
						{
							this.m_nonSharedStyles.Add(key);
						}
						else
						{
							dictionary[key] = true;
						}
					}
				}
			}
			if (dictionary != null)
			{
				this.m_nonSharedStyles = new List<StyleAttributeNames>(dictionary.Count);
				foreach (StyleAttributeNames key2 in dictionary.Keys)
				{
					this.m_nonSharedStyles.Add(key2);
				}
			}
		}
	}
}

using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal class HiddenPageItem : PageItem, IStorable, IPersistable
	{
		private Dictionary<string, List<object>> m_textBoxValues;

		private static Declaration m_declaration = HiddenPageItem.GetDeclaration();

		internal override bool ContentOnPage
		{
			get
			{
				return false;
			}
		}

		internal override double OriginalLeft
		{
			get
			{
				return base.ItemPageSizes.Left;
			}
		}

		internal override double OriginalWidth
		{
			get
			{
				return base.ItemPageSizes.Width;
			}
		}

		public override int Size
		{
			get
			{
				return base.Size + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_textBoxValues);
			}
		}

		internal HiddenPageItem()
		{
		}

		internal HiddenPageItem(double top, double left)
			: base(null)
		{
			base.m_itemPageSizes = new ItemSizes(left, top, 0.0, 0.0);
			base.KeepTogetherHorizontal = false;
			base.KeepTogetherVertical = false;
			bool unresolvedKTV = base.UnresolvedKTH = false;
			base.UnresolvedKTV = unresolvedKTV;
		}

		internal HiddenPageItem(ReportItem source, PageContext pageContext, bool checkHiddenState)
			: base(source)
		{
			base.m_itemPageSizes = new ItemSizes(source);
			base.KeepTogetherHorizontal = false;
			base.KeepTogetherVertical = false;
			bool unresolvedKTV = base.UnresolvedKTH = false;
			base.UnresolvedKTV = unresolvedKTV;
			if (pageContext.EvaluatePageHeaderFooter)
			{
				if (base.m_source.Visibility != null && base.m_source.Visibility.ToggleItem == null && base.m_source.Visibility.Hidden.IsExpression)
				{
					if (!checkHiddenState)
					{
						return;
					}
					if (base.m_source.Instance.Visibility.CurrentlyHidden)
					{
						return;
					}
				}
				this.m_textBoxValues = new Dictionary<string, List<object>>();
				HeaderFooterEval.CollectTextBoxes(base.m_source, pageContext, true, this.m_textBoxValues);
			}
		}

		internal void AddToCollection(HiddenPageItem hiddenItem)
		{
			if (hiddenItem != null && hiddenItem.m_textBoxValues != null)
			{
				if (this.m_textBoxValues == null)
				{
					this.m_textBoxValues = hiddenItem.m_textBoxValues;
				}
				else
				{
					foreach (string key in hiddenItem.m_textBoxValues.Keys)
					{
						List<object> list = hiddenItem.m_textBoxValues[key];
						if (this.m_textBoxValues.ContainsKey(key))
						{
							this.m_textBoxValues[key].AddRange(list);
						}
						else
						{
							this.m_textBoxValues[key] = list;
						}
					}
				}
			}
		}

		protected override void DetermineVerticalSize(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			if (base.m_source != null && base.m_source.Visibility != null && base.m_source.Visibility.HiddenState == SharedHiddenState.Sometimes)
			{
				base.m_itemPageSizes.AdjustHeightTo(0.0);
			}
		}

		protected override void DetermineHorizontalSize(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, List<PageItem> ancestors, bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			if (base.m_source != null && base.m_source.Visibility != null && base.m_source.Visibility.HiddenState == SharedHiddenState.Sometimes)
			{
				base.m_itemPageSizes.AdjustWidthTo(0.0);
			}
		}

		internal override void RegisterTextBoxes(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null && this.m_textBoxValues != null && !pageContext.Common.InSubReport)
			{
				if (rplWriter.DelayedTBLevels == 0)
				{
					foreach (string key in this.m_textBoxValues.Keys)
					{
						List<object> list = this.m_textBoxValues[key];
						foreach (object item in list)
						{
							pageContext.AddTextBox(key, item);
						}
					}
				}
				else
				{
					rplWriter.AddTextBoxes(this.m_textBoxValues);
				}
			}
		}

		internal override void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(HiddenPageItem.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.TextBoxValues)
				{
					writer.WriteStringVariantListDictionary(this.m_textBoxValues);
				}
				else
				{
					RSTrace.RenderingTracer.Assert(false, string.Empty);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(HiddenPageItem.m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.TextBoxValues)
				{
					this.m_textBoxValues = reader.ReadStringVariantListDictionary();
				}
				else
				{
					RSTrace.RenderingTracer.Assert(false, string.Empty);
				}
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.HiddenPageItem;
		}

		internal new static Declaration GetDeclaration()
		{
			if (HiddenPageItem.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.TextBoxValues, ObjectType.StringVariantListDictionary));
				return new Declaration(ObjectType.HiddenPageItem, ObjectType.PageItem, list);
			}
			return HiddenPageItem.m_declaration;
		}
	}
}

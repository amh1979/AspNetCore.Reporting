using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TextBoxInstance : ReportItemInstance
	{
		internal InstanceInfo InstanceInfo
		{
			get
			{
				if (base.m_instanceInfo is OffsetInfo)
				{
					Global.Tracer.Assert(false, string.Empty);
					return null;
				}
				if (base.m_instanceInfo is SimpleTextBoxInstanceInfo)
				{
					return (SimpleTextBoxInstanceInfo)base.m_instanceInfo;
				}
				return (TextBoxInstanceInfo)base.m_instanceInfo;
			}
		}

		internal TextBoxInstance(ReportProcessing.ProcessingContext pc, TextBox reportItemDef, int index)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			if (reportItemDef.IsSimpleTextBox())
			{
				base.m_instanceInfo = new SimpleTextBoxInstanceInfo(pc, reportItemDef, this, index);
			}
			else
			{
				base.m_instanceInfo = new TextBoxInstanceInfo(pc, reportItemDef, this, index);
			}
		}

		internal TextBoxInstance()
		{
		}

		internal SimpleTextBoxInstanceInfo UpgradeToSimpleTextbox(TextBoxInstanceInfo instanceInfo, out bool isSimple)
		{
			isSimple = false;
			TextBox textBox = base.ReportItemDef as TextBox;
			if (textBox.IsSimpleTextBox())
			{
				isSimple = true;
				return (SimpleTextBoxInstanceInfo)(base.m_instanceInfo = new SimpleTextBoxInstanceInfo(textBox, instanceInfo));
			}
			return null;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList members = new MemberInfoList();
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, members);
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(base.m_instanceInfo is OffsetInfo);
			if (((TextBox)base.m_reportItemDef).IsSimpleTextBox(reader.IntermediateFormatVersion))
			{
				return null;
			}
			return reader.ReadTextBoxInstanceInfo((TextBox)base.m_reportItemDef);
		}

		internal SimpleTextBoxInstanceInfo GetSimpleInstanceInfo(ChunkManager.RenderingChunkManager chunkManager, bool inPageSection)
		{
			Global.Tracer.Assert(((TextBox)base.m_reportItemDef).IsSimpleTextBox());
			if (base.m_instanceInfo is OffsetInfo)
			{
				Global.Tracer.Assert(null != chunkManager);
				IntermediateFormatReader intermediateFormatReader = null;
				intermediateFormatReader = ((!inPageSection) ? chunkManager.GetReader(((OffsetInfo)base.m_instanceInfo).Offset) : chunkManager.GetPageSectionInstanceReader(((OffsetInfo)base.m_instanceInfo).Offset));
				return intermediateFormatReader.ReadSimpleTextBoxInstanceInfo((TextBox)base.m_reportItemDef);
			}
			return (SimpleTextBoxInstanceInfo)base.m_instanceInfo;
		}
	}
}

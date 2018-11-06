using System.IO;

namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class OffsetItemInfo : OffsetInfo, IRPLItemFactory
	{
		internal OffsetItemInfo(long endOffset, RPLContext context)
			: base(endOffset, context)
		{
		}

		public RPLItem GetRPLItem()
		{
			if (base.m_endOffset <= 0)
			{
				return null;
			}
			byte b = 0;
			BinaryReader binaryReader = base.m_context.BinaryReader;
			long num = RPLReader.ResolveReportItemEnd(base.m_endOffset, binaryReader, ref b);
			switch (b)
			{
			case 16:
			{
				RPLItemMeasurement[] children = RPLReader.ReadItemMeasurements(base.m_context, binaryReader);
				b = RPLReader.ReadItemType(num, binaryReader);
				return RPLContainer.CreateItem(num, base.m_context, children, b);
			}
			case 17:
			{
				RPLTablix rPLTablix = new RPLTablix(num, base.m_context);
				RPLReader.ReadTablixStructure(rPLTablix, base.m_context, binaryReader);
				return rPLTablix;
			}
			case 18:
				return RPLReader.ReadTextBoxStructure(num, base.m_context);
			default:
				return RPLItem.CreateItem(num, base.m_context, b);
			}
		}
	}
}

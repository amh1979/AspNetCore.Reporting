using AspNetCore.ReportingServices.ReportProcessing;
using System.Runtime.InteropServices;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct DataReaderRIFObjectCreator : IRIFObjectCreator
	{
		public IPersistable CreateRIFObject(ObjectType objectType, ref IntermediateFormatReader context)
		{
			IPersistable persistable = null;
			if (objectType == ObjectType.Null)
			{
				return null;
			}
			switch (objectType)
			{
			case ObjectType.RecordSetInfo:
				persistable = new RecordSetInfo();
				break;
			case ObjectType.RecordRow:
				persistable = new RecordRow();
				break;
			case ObjectType.RecordField:
				persistable = new RecordField();
				break;
			case ObjectType.IntermediateFormatVersion:
				persistable = new IntermediateFormatVersion();
				break;
			case ObjectType.RecordSetPropertyNames:
				persistable = new RecordSetPropertyNames();
				break;
			default:
				Global.Tracer.Assert(false);
				break;
			}
			persistable.Deserialize(context);
			return persistable;
		}
	}
}

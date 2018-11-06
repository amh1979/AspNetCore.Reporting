using System.Data;
using System.Xml;

namespace AspNetCore.ReportingServices.Interfaces
{
	internal interface ISemanticModelGenerator : IExtension
	{
		void Generate(IDbConnection connection, XmlWriter newModelWriter);

		void ReGenerateModel(IDbConnection connection, XmlReader currentModelReader, XmlWriter newModelWriter);
	}
}

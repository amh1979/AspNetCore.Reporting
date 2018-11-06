using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal interface IRichTextInstanceCreator
	{
		IList<ICompiledParagraphInstance> CreateParagraphInstanceCollection();

		ICompiledParagraphInstance CreateParagraphInstance();

		ICompiledTextRunInstance CreateTextRunInstance();

		IList<ICompiledTextRunInstance> CreateTextRunInstanceCollection();

		ICompiledStyleInstance CreateStyleInstance(bool isParagraph);

		IActionInstance CreateActionInstance();
	}
}

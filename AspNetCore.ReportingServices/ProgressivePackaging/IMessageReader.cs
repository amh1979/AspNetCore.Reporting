using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ProgressivePackaging
{
	internal interface IMessageReader : IEnumerable<MessageElement>, IEnumerable, IDisposable
	{
	}
}

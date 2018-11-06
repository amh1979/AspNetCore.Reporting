using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IReference : IStorable, IPersistable
	{
		ReferenceID Id
		{
			get;
		}

		IDisposable PinValue();

		void UnPinValue();

		void Free();

		void UpdateSize(int sizeDeltaBytes);

		IReference TransferTo(IScalabilityCache scaleCache);
	}
	internal interface IReference<T> : IReference, IStorable, IPersistable
	{
		T Value();
	}
}

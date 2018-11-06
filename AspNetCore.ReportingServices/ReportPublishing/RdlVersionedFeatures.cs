using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal sealed class RdlVersionedFeatures
	{
		private sealed class FeatureDescriptor
		{
			private readonly int m_addedInCompatVersion;

			private readonly RenderMode m_allowedRenderModes;

			public int AddedInCompatVersion
			{
				get
				{
					return this.m_addedInCompatVersion;
				}
			}

			public RenderMode AllowedRenderModes
			{
				get
				{
					return this.m_allowedRenderModes;
				}
			}

			public FeatureDescriptor(int addedInCompatVersion, RenderMode allowedRenderModes)
			{
				this.m_addedInCompatVersion = addedInCompatVersion;
				this.m_allowedRenderModes = allowedRenderModes;
			}
		}

		private readonly FeatureDescriptor[] m_rdlFeatureVersioningStructure;

		public RdlVersionedFeatures()
		{
			int length = Enum.GetValues(typeof(RdlFeatures)).Length;
			this.m_rdlFeatureVersioningStructure = new FeatureDescriptor[length];
		}

		internal void Add(RdlFeatures featureType, int addedInCompatVersion, RenderMode allowedRenderModes)
		{
			this.m_rdlFeatureVersioningStructure[(int)featureType] = new FeatureDescriptor(addedInCompatVersion, allowedRenderModes);
		}

		internal bool IsRdlFeatureAllowed(RdlFeatures feature, int configVersion, RenderMode renderMode)
		{
			FeatureDescriptor featureDescriptor = this.m_rdlFeatureVersioningStructure[(int)feature];
			bool flag = configVersion == 0 || featureDescriptor.AddedInCompatVersion <= configVersion;
			bool result = (featureDescriptor.AllowedRenderModes & renderMode) == renderMode;
			if (flag)
			{
				return result;
			}
			return false;
		}

		internal void VerifyAllFeaturesAreAdded()
		{
		}

		[Conditional("DEBUG")]
		private void VerifyAllFeaturesAreAdded(FeatureDescriptor[] rdlFeatureVersioningStructure)
		{
			for (int i = 0; i < rdlFeatureVersioningStructure.Length; i++)
			{
				Global.Tracer.Assert(rdlFeatureVersioningStructure[i] != null, "Missing RDL feature for: {0}", (RdlFeatures)i);
			}
		}
	}
}

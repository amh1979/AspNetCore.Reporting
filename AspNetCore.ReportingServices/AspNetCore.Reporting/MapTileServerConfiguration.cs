using AspNetCore.Reporting;
using System;
using System.ComponentModel;

namespace AspNetCore.Reporting
{
    //[TypeConverter(typeof(TypeNameHidingExpandableObjectConverter))]
    internal sealed class MapTileServerConfiguration
	{
		private LocalProcessingHostMapTileServerConfiguration m_underlyingConfiguration;

		[DefaultValue(2)]
		//[SRDescription("MapTileServerConfigurationMaxConnectionsDesc")]
		[NotifyParentProperty(true)]
		public int MaxConnections
		{
			get
			{
				return this.m_underlyingConfiguration.MaxConnections;
			}
			set
			{
				this.m_underlyingConfiguration.MaxConnections = value;
			}
		}

		[DefaultValue(10)]
		[NotifyParentProperty(true)]
		//[SRDescription("MapTileServerConfigurationTimeoutDesc")]
		public int Timeout
		{
			get
			{
				return this.m_underlyingConfiguration.Timeout;
			}
			set
			{
				this.m_underlyingConfiguration.Timeout = value;
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue("(Default)")]
		//[SRDescription("MapTileServerConfigurationAppIDDesc")]
		public string AppID
		{
			get
			{
				return this.m_underlyingConfiguration.AppID;
			}
			set
			{
				this.m_underlyingConfiguration.AppID = value;
			}
		}

		internal MapTileServerConfiguration(LocalProcessingHostMapTileServerConfiguration underlyingConfiguration)
		{
			if (underlyingConfiguration == null)
			{
				throw new ArgumentNullException("underlyingConfiguration");
			}
			this.m_underlyingConfiguration = underlyingConfiguration;
		}
	}
}

using AspNetCore.ReportingServices.RdlObjectModel;
using AspNetCore.ReportingServices.RdlObjectModel.Serialization;
using AspNetCore.ReportingServices.RdlObjectModel2010.Upgrade;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlObjectModel2008.Upgrade
{
	internal class SerializerHost2008 : SerializerHost2010
	{
		private List<IUpgradeable2008> m_upgradeable;

		private static Type[,] m_substituteTypes;

		public List<IUpgradeable2008> Upgradeable2008
		{
			get
			{
				return this.m_upgradeable;
			}
			set
			{
				this.m_upgradeable = value;
			}
		}

		public override Type GetSubstituteType(Type type)
		{
			if (!base.m_serializing)
			{
				for (int i = 0; i < SerializerHost2008.m_substituteTypes.GetLength(0); i++)
				{
					if (type == SerializerHost2008.m_substituteTypes[i, 0])
					{
						return SerializerHost2008.m_substituteTypes[i, 1];
					}
				}
			}
			else
			{
				for (int j = 0; j < SerializerHost2008.m_substituteTypes.GetLength(0); j++)
				{
					if (type == SerializerHost2008.m_substituteTypes[j, 1])
					{
						return SerializerHost2008.m_substituteTypes[j, 0];
					}
				}
			}
			return type;
		}

		public override void OnDeserialization(object value)
		{
			if (this.m_upgradeable != null && value is IUpgradeable2008)
			{
				this.m_upgradeable.Add((IUpgradeable2008)value);
			}
			base.OnDeserialization(value);
		}

		public override IEnumerable<ExtensionNamespace> GetExtensionNamespaces()
		{
			return new ExtensionNamespace[1]
			{
				new ExtensionNamespace("rd", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner", false)
			};
		}

		public SerializerHost2008(bool serializing)
			: base(serializing)
		{
		}

		static SerializerHost2008()
		{
			Type[,] array = new Type[1, 2];
			Type[,] array2 = array;
			Type typeFromHandle = typeof(Report);
			array2[0, 0] = typeFromHandle;
			Type[,] array3 = array;
			Type typeFromHandle2 = typeof(Report2008);
			array3[0, 1] = typeFromHandle2;
			SerializerHost2008.m_substituteTypes = array;
		}
	}
}

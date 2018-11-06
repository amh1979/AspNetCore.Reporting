using AspNetCore.ReportingServices.RdlObjectModel;
using AspNetCore.ReportingServices.RdlObjectModel.RdlUpgrade;
using AspNetCore.ReportingServices.RdlObjectModel.Serialization;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlObjectModel2010.Upgrade
{
	internal class SerializerHost2010 : SerializerHostBase
	{
		private List<IUpgradeable2010> m_upgradeable;

		private static Type[,] m_substituteTypes;

		public List<IUpgradeable2010> Upgradeable2010
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
				for (int i = 0; i < SerializerHost2010.m_substituteTypes.GetLength(0); i++)
				{
					if (type == SerializerHost2010.m_substituteTypes[i, 0])
					{
						return SerializerHost2010.m_substituteTypes[i, 1];
					}
				}
			}
			else
			{
				for (int j = 0; j < SerializerHost2010.m_substituteTypes.GetLength(0); j++)
				{
					if (type == SerializerHost2010.m_substituteTypes[j, 1])
					{
						return SerializerHost2010.m_substituteTypes[j, 0];
					}
				}
			}
			return type;
		}

		public override void OnDeserialization(object value)
		{
			if (this.m_upgradeable != null && value is IUpgradeable2010)
			{
				this.m_upgradeable.Add((IUpgradeable2010)value);
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

		public SerializerHost2010(bool serializing)
			: base(serializing)
		{
		}

		static SerializerHost2010()
		{
			Type[,] array = new Type[2, 2];
			Type[,] array2 = array;
			Type typeFromHandle = typeof(Report);
			array2[0, 0] = typeFromHandle;
			Type[,] array3 = array;
			Type typeFromHandle2 = typeof(Report2010);
			array3[0, 1] = typeFromHandle2;
			Type[,] array4 = array;
			Type typeFromHandle3 = typeof(StateIndicator);
			array4[1, 0] = typeFromHandle3;
			Type[,] array5 = array;
			Type typeFromHandle4 = typeof(StateIndicator2010);
			array5[1, 1] = typeFromHandle4;
			SerializerHost2010.m_substituteTypes = array;
		}
	}
}

using AspNetCore.ReportingServices.RdlObjectModel.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	internal abstract class ReportObject : ReportObjectBase
	{
		protected ReportObject()
		{
		}

		internal ReportObject(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public virtual object DeepClone()
		{
			Type type = base.GetType();
			PropertyStore propertyStore = new PropertyStore();
			ReportObject reportObject = (ReportObject)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[1]
			{
				propertyStore
			}, null);
			propertyStore.SetOwner(reportObject);
			this.CopyTo(reportObject, null);
			return reportObject;
		}

		private void CopyTo(ReportObject clone, ICollection<string> membersToExclude)
		{
			StructMapping structMapping = (StructMapping)TypeMapper.GetTypeMapping(base.GetType());
			foreach (MemberMapping member in structMapping.Members)
			{
				if (member.HasValue(this) && (membersToExclude == null || !membersToExclude.Contains(member.Name)))
				{
					object value = member.GetValue(this);
					member.SetValue(clone, ReportObject.CloneObject(value));
				}
			}
		}

		protected static object CloneObject(object obj)
		{
			if (obj is ReportObject)
			{
				obj = ((ReportObject)obj).DeepClone();
			}
			else if (obj is IList)
			{
				obj = ReportObject.CloneList((IList)obj);
			}
			return obj;
		}

		private static object CloneList(IList obj)
		{
			IList list = (IList)Activator.CreateInstance(obj.GetType());
			foreach (object item in obj)
			{
				list.Add(ReportObject.CloneObject(item));
			}
			return list;
		}

		internal virtual void OnSetObject(int propertyIndex)
		{
		}

		internal T GetAncestor<T>() where T : class
		{
			for (IContainedObject parent = base.Parent; parent != null; parent = parent.Parent)
			{
				if (parent is T)
				{
					return (T)parent;
				}
			}
			return null;
		}

		internal IList<ReportObject> GetDependencies()
		{
			IList<ReportObject> list = new List<ReportObject>();
			this.GetDependenciesCore(list);
			return list;
		}

		protected virtual void GetDependenciesCore(IList<ReportObject> dependencies)
		{
			base.PropertyStore.IterateObjectEntries(delegate(int propertyIndex, object value)
			{
				if (value is IExpression)
				{
					((IExpression)value).GetDependencies(dependencies, this);
				}
				else if (value is ReportObject)
				{
					((ReportObject)value).GetDependenciesCore(dependencies);
				}
				else if (value is IList)
				{
					foreach (object item in (IList)value)
					{
						if (item is ReportObject)
						{
							((ReportObject)item).GetDependenciesCore(dependencies);
						}
						else if (item is ReportExpression)
						{
							((ReportExpression)item).GetDependencies(dependencies, this);
						}
					}
				}
			});
		}
	}
}

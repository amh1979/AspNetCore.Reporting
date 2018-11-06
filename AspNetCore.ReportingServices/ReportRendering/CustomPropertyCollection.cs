using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class CustomPropertyCollection
	{
		private DataValueInstanceList m_instances;

		private DataValueList m_expressions;

		private bool m_isCustomControl;

		private bool m_populated;

		private Hashtable m_uniqueNames;

		private IntList m_expressionIndex;

		public CustomProperty this[string name]
		{
            get
            {
                if (name == null)
                {
                    throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, new object[]
                    {
                "name"
                    });
                }
                if (!this.m_populated && !this.m_isCustomControl)
                {
                    this.Populate();
                }
                object obj = this.m_uniqueNames[name];
                if (obj != null && obj is int)
                {
                    string name2;
                    object value;
                    this.GetNameValue((int)obj, out name2, out value);
                    return new CustomProperty(name2, value);
                }
                return null;
            }
        }

		public CustomProperty this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					string name = null;
					object value = null;
					if (this.IsCustomControl)
					{
						name = this.m_instances[index].Name;
						value = this.m_instances[index].Value;
					}
					else
					{
						if (!this.m_populated)
						{
							this.Populate();
						}
						Global.Tracer.Assert(this.m_expressionIndex.Count <= this.m_expressions.Count && index <= this.m_expressionIndex.Count);
						this.GetNameValue(this.m_expressionIndex[index], out name, out value);
					}
					return new CustomProperty(name, value);
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public int Count
		{
			get
			{
				if (!this.m_populated && !this.m_isCustomControl)
				{
					this.Populate();
				}
				return this.m_uniqueNames.Count;
			}
		}

		internal bool IsCustomControl
		{
			get
			{
				return this.m_isCustomControl;
			}
		}

		public CustomPropertyCollection()
		{
			this.m_isCustomControl = true;
			this.m_instances = new DataValueInstanceList();
			this.m_uniqueNames = new Hashtable();
		}

		internal CustomPropertyCollection(DataValueList expressions, DataValueInstanceList instances)
		{
			this.m_expressions = expressions;
			this.m_instances = instances;
			Global.Tracer.Assert(null != this.m_expressions);
			Global.Tracer.Assert(this.m_instances == null || this.m_instances.Count == this.m_expressions.Count);
			this.m_uniqueNames = new Hashtable(this.m_expressions.Count);
			this.m_expressionIndex = new IntList(this.m_expressions.Count);
		}

		public void Add(string propertyName, object propertyValue)
		{
			if (!this.m_isCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			this.InternalAdd(propertyName, propertyValue);
		}

		public void Add(CustomProperty property)
		{
			if (!this.m_isCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			if (property == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "property");
			}
			this.InternalAdd(property.Name, property.Value);
		}

		internal CustomPropertyCollection DeepClone()
		{
			Global.Tracer.Assert(this.m_isCustomControl && null == this.m_expressions);
			CustomPropertyCollection customPropertyCollection = new CustomPropertyCollection();
			if (this.m_instances != null)
			{
				int count = this.m_instances.Count;
				customPropertyCollection.m_instances = new DataValueInstanceList(count);
				for (int i = 0; i < count; i++)
				{
					customPropertyCollection.m_instances.Add(this.m_instances[i].DeepClone());
				}
			}
			return customPropertyCollection;
		}

		private void Populate()
		{
			Global.Tracer.Assert(!this.m_isCustomControl);
			int count = this.m_expressions.Count;
			for (int i = 0; i < count; i++)
			{
				string text = default(string);
				object obj = default(object);
				this.GetNameValue(i, out text, out obj);
				if (text != null && !this.m_uniqueNames.ContainsKey(text))
				{
					this.m_uniqueNames.Add(text, i);
					this.m_expressionIndex.Add(i);
				}
			}
		}

		internal void GetNameValue(int index, out string name, out object value)
		{
			name = null;
			value = null;
			Global.Tracer.Assert(0 <= index && index < this.m_expressions.Count);
			if (ExpressionInfo.Types.Constant == this.m_expressions[index].Name.Type)
			{
				name = this.m_expressions[index].Name.Value;
			}
			else if (this.m_instances != null)
			{
				name = this.m_instances[index].Name;
			}
			if (ExpressionInfo.Types.Constant == this.m_expressions[index].Value.Type)
			{
				value = this.m_expressions[index].Value.Value;
			}
			else if (this.m_instances != null)
			{
				value = this.m_instances[index].Value;
			}
		}

		internal void GetNameValueExpressions(int index, out ExpressionInfo nameExpression, out ExpressionInfo valueExpression, out string name, out object value)
		{
			this.GetNameValue(index, out name, out value);
			nameExpression = this.m_expressions[index].Name;
			valueExpression = this.m_expressions[index].Value;
		}

		private void InternalAdd(string name, object value)
		{
			DataValueInstance dataValueInstance = new DataValueInstance();
			dataValueInstance.Name = name;
			dataValueInstance.Value = value;
			this.m_uniqueNames.Add(name, dataValueInstance);
			this.m_instances.Add(dataValueInstance);
		}

		internal DataValueInstanceList Deconstruct()
		{
			return this.m_instances;
		}
	}
}

using System;
using System.Collections;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class ShapeCollection : NamedCollection
	{
		private Shape this[int index]
		{
			get
			{
				return (Shape)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private Shape this[string name]
		{
			get
			{
				return (Shape)base.GetByNameCheck(name);
			}
			set
			{
				base.SetByNameCheck(name, value);
			}
		}

		public Shape this[object obj]
		{
			get
			{
				if (obj is string)
				{
					return this[(string)obj];
				}
				if (obj is int)
				{
					return this[(int)obj];
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgument);
			}
			set
			{
				if (obj is string)
				{
					this[(string)obj] = value;
					return;
				}
				if (obj is int)
				{
					this[(int)obj] = value;
					return;
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgument);
			}
		}

		internal ShapeCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			base.elementType = typeof(Shape);
		}

		public Shape Add(string name)
		{
			Shape shape = new Shape();
			shape.Name = name;
			this.Add(shape);
			return shape;
		}

		public int Add(Shape value)
		{
			return base.List.Add(value);
		}

		public void Remove(Shape value)
		{
			base.List.Remove(value);
		}

		public ArrayList Find(string searchFor, bool ignoreCase, bool exactSearch, bool uniqueOnlyFields)
		{
			ArrayList arrayList = new ArrayList();
			if (base.Common != null && base.Common.MapCore != null && base.Common.MapCore.ShapeFields != null)
			{
				if (ignoreCase)
				{
					searchFor = searchFor.ToUpper(CultureInfo.CurrentCulture);
				}
				FieldCollection shapeFields = base.Common.MapCore.ShapeFields;
				{
					foreach (Shape item in this)
					{
						string text = ignoreCase ? item.Name.ToUpper(CultureInfo.CurrentCulture) : item.Name;
						if (exactSearch)
						{
							if (text == searchFor)
							{
								arrayList.Add(item);
								continue;
							}
						}
						else if (text.IndexOf(searchFor, StringComparison.Ordinal) >= 0)
						{
							arrayList.Add(item);
							continue;
						}
						foreach (Field item2 in shapeFields)
						{
							if (!uniqueOnlyFields || item2.UniqueIdentifier)
							{
								try
								{
									if (!base.Common.MapCore.IsDesignMode() || !item2.IsTemporary)
									{
										object obj = item[item2.Name];
										if (obj != null)
										{
											if (item2.Type == typeof(string))
											{
												string text2 = ignoreCase ? ((string)obj).ToUpper(CultureInfo.CurrentCulture) : ((string)obj);
												if (exactSearch)
												{
													if (text2 == searchFor)
													{
														arrayList.Add(item);
														goto IL_01c4;
													}
												}
												else if (text2.IndexOf(searchFor, StringComparison.Ordinal) >= 0)
												{
													arrayList.Add(item);
													goto IL_01c4;
												}
											}
											else
											{
												object obj2 = item2.Parse(searchFor);
												if (obj2 != null && obj2.Equals(obj))
												{
													arrayList.Add(item);
													goto IL_01c4;
												}
											}
										}
									}
								}
								catch
								{
								}
							}
						}
						IL_01c4:;
					}
					return arrayList;
				}
			}
			return arrayList;
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "Shape1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Shape{0}";
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			Shape shape = (Shape)value;
		}

		internal override void Invalidate()
		{
			if (base.Common != null)
			{
				base.Common.MapCore.InvalidateDataBinding();
				base.Common.MapCore.InvalidateCachedBounds();
			}
			base.Invalidate();
		}
	}
}

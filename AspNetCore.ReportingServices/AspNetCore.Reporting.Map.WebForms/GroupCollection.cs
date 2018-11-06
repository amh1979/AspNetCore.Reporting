using System;
using System.Collections;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class GroupCollection : NamedCollection
	{
		private Group this[int index]
		{
			get
			{
				return (Group)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private Group this[string name]
		{
			get
			{
				return (Group)base.GetByNameCheck(name);
			}
			set
			{
				base.SetByNameCheck(name, value);
			}
		}

		public Group this[object obj]
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

		internal GroupCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			base.elementType = typeof(Group);
		}

		public Group Add(string name)
		{
			Group group = new Group();
			group.Name = name;
			this.Add(group);
			return group;
		}

		public int Add(Group value)
		{
			return base.List.Add(value);
		}

		public void Remove(Group value)
		{
			base.List.Remove(value);
		}

		public ArrayList Find(string searchFor, bool ignoreCase, bool exactSearch, bool uniqueOnlyFields)
		{
			ArrayList arrayList = new ArrayList();
			if (base.Common != null && base.Common.MapCore != null && base.Common.MapCore.GroupFields != null)
			{
				if (ignoreCase)
				{
					searchFor = searchFor.ToUpper(CultureInfo.CurrentCulture);
				}
				FieldCollection groupFields = base.Common.MapCore.GroupFields;
				{
					foreach (Group item in this)
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
						foreach (Field item2 in groupFields)
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
			return "Group1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Group{0}";
		}

		internal override void Invalidate()
		{
			if (base.Common != null)
			{
				base.Common.MapCore.InvalidateDataBinding();
			}
			base.Invalidate();
		}
	}
}

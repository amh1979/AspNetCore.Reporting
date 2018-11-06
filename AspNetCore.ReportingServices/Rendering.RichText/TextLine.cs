using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal sealed class TextLine
	{
		private List<TextRun> m_prefix;

		private List<TextRun> m_visualRuns;

		private List<TextRun> m_logicalRuns = new List<TextRun>();

		private int m_prefixWidth;

		private int m_width;

		private int m_ascent;

		private int m_descent;

		private bool m_firstLine;

		private bool m_lastLine;

		private bool m_calculatedDimensions;

		private bool m_calculatedHeight;

		internal string Text
		{
			get
			{
				if (this.m_logicalRuns.Count == 0)
				{
					return null;
				}
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < this.m_visualRuns.Count; i++)
				{
					stringBuilder.Append(this.m_visualRuns[i].Text);
				}
				return stringBuilder.ToString();
			}
		}

		internal List<TextRun> VisualRuns
		{
			get
			{
				return this.m_visualRuns;
			}
		}

		internal List<TextRun> LogicalRuns
		{
			get
			{
				return this.m_logicalRuns;
			}
		}

		internal List<TextRun> Prefix
		{
			get
			{
				return this.m_prefix;
			}
			set
			{
				this.m_prefix = value;
			}
		}

		internal bool LastLine
		{
			get
			{
				return this.m_lastLine;
			}
			set
			{
				this.m_lastLine = value;
			}
		}

		internal bool FirstLine
		{
			get
			{
				return this.m_firstLine;
			}
			set
			{
				this.m_firstLine = value;
			}
		}

		internal TextLine()
		{
		}

		internal void ResetHeight()
		{
			this.m_calculatedHeight = false;
			this.m_ascent = 0;
			this.m_descent = 0;
		}

		internal int GetHeight(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			if (!this.m_calculatedHeight)
			{
				this.m_calculatedHeight = true;
				for (int i = 0; i < this.m_logicalRuns.Count; i++)
				{
					TextRun textRun = this.m_logicalRuns[i];
					int ascent = textRun.GetAscent(hdc, fontCache);
					if (ascent > this.m_ascent)
					{
						this.m_ascent = ascent;
					}
					int descent = textRun.GetDescent(hdc, fontCache);
					if (descent > this.m_descent)
					{
						this.m_descent = descent;
					}
				}
				if (this.m_prefix != null)
				{
					for (int j = 0; j < this.m_prefix.Count; j++)
					{
						TextRun textRun2 = this.m_prefix[j];
						int ascent2 = textRun2.GetAscent(hdc, fontCache);
						if (ascent2 > this.m_ascent)
						{
							this.m_ascent = ascent2;
						}
						int descent2 = textRun2.GetDescent(hdc, fontCache);
						if (descent2 > this.m_descent)
						{
							this.m_descent = descent2;
						}
					}
				}
			}
			return this.m_ascent + this.m_descent;
		}

		internal int GetAscent(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			this.CalculateDimensions(hdc, fontCache, true);
			return this.m_ascent;
		}

		internal int GetDescent(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			this.CalculateDimensions(hdc, fontCache, true);
			return this.m_descent;
		}

		internal int GetWidth(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			return this.GetWidth(hdc, fontCache, true);
		}

		internal int GetWidth(Win32DCSafeHandle hdc, FontCache fontCache, bool useVisualRunsIfAvailable)
		{
			this.CalculateDimensions(hdc, fontCache, useVisualRunsIfAvailable);
			return this.m_width;
		}

		internal int GetPrefixWidth(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			if (this.m_prefix == null)
			{
				return 0;
			}
			this.CalculateDimensions(hdc, fontCache, true);
			return this.m_prefixWidth;
		}

		private void CalculateDimensions(Win32DCSafeHandle hdc, FontCache fontCache, bool useVisualRunsIfAvailable)
		{
			if (!this.m_calculatedDimensions)
			{
				bool flag = useVisualRunsIfAvailable && this.m_visualRuns != null;
				List<TextRun> list = flag ? this.m_visualRuns : this.m_logicalRuns;
				this.m_width = 0;
				this.m_prefixWidth = 0;
				int count = list.Count;
				int num = 0;
				for (int i = 0; i < count; i++)
				{
					TextRun textRun = list[i];
					int width = textRun.GetWidth(hdc, fontCache);
					this.m_width += width;
					if (!flag)
					{
						num = Math.Max(textRun.GetWidth(hdc, fontCache, true) - width, num);
					}
					else if (i == count - 1)
					{
						num = textRun.GetWidth(hdc, fontCache, true) - width;
					}
					if (!this.m_calculatedHeight)
					{
						int ascent = textRun.GetAscent(hdc, fontCache);
						if (ascent > this.m_ascent)
						{
							this.m_ascent = ascent;
						}
						int descent = textRun.GetDescent(hdc, fontCache);
						if (descent > this.m_descent)
						{
							this.m_descent = descent;
						}
					}
				}
				this.m_width += num;
				if (this.m_prefix != null)
				{
					for (int j = 0; j < this.m_prefix.Count; j++)
					{
						TextRun textRun2 = this.m_prefix[j];
						if (!this.m_calculatedHeight)
						{
							int ascent2 = textRun2.GetAscent(hdc, fontCache);
							if (ascent2 > this.m_ascent)
							{
								this.m_ascent = ascent2;
							}
							int descent2 = textRun2.GetDescent(hdc, fontCache);
							if (descent2 > this.m_descent)
							{
								this.m_descent = descent2;
							}
						}
						this.m_prefixWidth += textRun2.GetWidth(hdc, fontCache);
					}
				}
				this.m_calculatedDimensions = true;
				this.m_calculatedHeight = true;
			}
		}

		internal void ScriptLayout(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			if (this.m_visualRuns == null)
			{
				int count = this.m_logicalRuns.Count;
				if (count != 0)
				{
					byte[] array = new byte[count];
					int[] array2 = new int[count];
					for (int i = 0; i < count; i++)
					{
						array[i] = (byte)this.m_logicalRuns[i].ScriptAnalysis.s.uBidiLevel;
					}
					int num = Win32.ScriptLayout(count, array, null, array2);
					if (Win32.Failed(num))
					{
						Marshal.ThrowExceptionForHR(num);
					}
					this.m_visualRuns = new List<TextRun>(count);
					for (int j = 0; j < count; j++)
					{
						this.m_visualRuns.Add(null);
					}
					for (int k = 0; k < count; k++)
					{
						this.m_visualRuns[array2[k]] = this.m_logicalRuns[k];
					}
					int num2 = 0;
					int l = -1;
					for (int m = 0; m < count; m++)
					{
						TextRun textRun = this.m_visualRuns[m];
						textRun.UnderlineHeight = 0;
						if (textRun.TextRunProperties.TextDecoration == RPLFormat.TextDecorations.Underline)
						{
							if (l < 0)
							{
								l = m;
							}
							int height = textRun.GetHeight(hdc, fontCache);
							if (height > num2)
							{
								num2 = height;
							}
						}
						else if (l >= 0)
						{
							for (; l < m; l++)
							{
								this.m_visualRuns[l].UnderlineHeight = num2;
							}
							num2 = 0;
							l = -1;
						}
					}
					if (l >= 0)
					{
						for (; l < count; l++)
						{
							this.m_visualRuns[l].UnderlineHeight = num2;
						}
					}
				}
			}
		}
	}
}

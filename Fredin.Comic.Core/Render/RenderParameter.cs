using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fredin.Comic.Render
{
	public class RenderParameter
	{
		public string Name { get; set; }

		public string DisplayName { get; set; }

		public object MinValue { get; set; }

		public object MaxValue { get; set; }

		public object DefaultValue { get; set; }

		public RenderParameter()
		{
		}

		public RenderParameter(string name, string displayName, object defaultValue, object minValue, object maxValue)
		{
			this.Name = name;
			this.DisplayName = displayName;
			this.DefaultValue = defaultValue;
			this.MinValue = minValue;
			this.MaxValue = maxValue;
		}
	}
}

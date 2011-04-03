using System;
using System.Linq;
using System.Data.Linq;
using System.Reflection;
using System.ComponentModel;

namespace Fredin.Util
{
	public static class EnumExtension
	{
		public static bool TryParse<T>(this Enum targetType, string value, out T returnValue)
		{
			returnValue = default(T);
			int intEnumValue;

			if (Int32.TryParse(value, out intEnumValue) && Enum.IsDefined(typeof(T), intEnumValue))
			{
				returnValue = (T)(object)intEnumValue;
				return true;
			}

			return false;
		}

		public static string GetDescription(this Enum value)
		{
			FieldInfo info = value.GetType().GetField(value.ToString());
			DescriptionAttribute[] attributes = (DescriptionAttribute[])info.GetCustomAttributes(typeof(DescriptionAttribute), false);
			return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
		}
	}
}

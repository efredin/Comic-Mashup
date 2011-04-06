using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace Fredin.Util
{
	public static class ObjectExtension
	{
		public static string ToXml(this object target)
		{
			// This is so long winded because .net will serialize using UTF-16 and fail to deserialize
			XmlSerializer serializer = new XmlSerializer(target.GetType());
			using (MemoryStream stream = new MemoryStream())
			{
				using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
				{
					serializer.Serialize(writer, target);
					writer.Flush();
					stream.Seek(0, SeekOrigin.Begin);
					using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
					{
						return reader.ReadToEnd();
					}
				}
			}
		}
	}
}

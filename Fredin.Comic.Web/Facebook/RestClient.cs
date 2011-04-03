// Code From: http://github.com/NikhilK/dynamicrest
// For more about this code view Nilhil's blog post: 
// http://www.nikhilk.net/CSharp-Dynamic-Programming-REST-Services.aspx
// Used with permission of Nikhil Kothari. 
// Updated to be compatible with .Net 4.0 RTM on 5/17/2010

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using System.Drawing;

namespace Fredin.Comic.Web.Facebook
{

	public sealed class RestClient : DynamicObject
	{

		private static readonly Regex TokenFormatRewriteRegex =
			new Regex(@"(?<start>\{)+(?<property>[\w\.\[\]]+)(?<format>:[^}]+)?(?<end>\})+",
					   RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex StripXmlnsRegex =
			new Regex(@"(xmlns:?[^=]*=[""][^""]*[""])",
					  RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private string _uriFormat;
		private string _operationGroup;
		private Dictionary<string, object> _parameters;

		public RestClient(string uriFormat)
			: base()
		{
			_uriFormat = uriFormat;
		}

		private RestClient(string uriFormat, string operationGroup, Dictionary<string, object> inheritedParameters)
			: this(uriFormat)
		{
			_operationGroup = operationGroup;
			_parameters = inheritedParameters;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if (_parameters == null)
			{
				_parameters = new Dictionary<string, object>();
			}

			if (_parameters.TryGetValue(binder.Name, out result))
			{
				return true;
			}

			string operationGroup = binder.Name;
			if (_operationGroup != null)
			{
				operationGroup = _operationGroup + "." + operationGroup;
			}

			RestClient operationGroupClient = new RestClient(_uriFormat, operationGroup, _parameters);
			result = operationGroupClient;
			return true;
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			string operation = binder.Name;
			if (_operationGroup != null)
			{
				operation = _operationGroup + "." + operation;
			}

			return PerformOperation(operation, args, out result);
		}

		public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
		{
			return PerformOperation(String.Empty, args, out result);
		}

		private bool PerformOperation(string operation, object[] args, out object result)
		{
			JsonObject argsObject = null;
			string httpMethod = "GET";
			if ((args != null) && (args.Length != 0))
			{
				argsObject = (JsonObject)args[0];
				if (args.Length > 1)
				{
					httpMethod = args[1].ToString();
				}
			}
			Uri requestUri = CreateRequestUri(operation, argsObject);

			HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(requestUri);
			webRequest.Method = httpMethod;

			HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

			if (webResponse.StatusCode == HttpStatusCode.OK)
			{
				Stream responseStream = webResponse.GetResponseStream();

				switch(webResponse.ContentType)
				{
					case "application/json":
					default:
						result = this.ProcessJsonStream(responseStream);
						break;

					case "image/jpeg":
					case "image/png":
					case "image/gif":
						result = new Bitmap(responseStream);
						break;
				}
				return true;
			}
			else
			{
				result = null;
				return false;
			}
		}

		private Uri CreateRequestUri(string operation, JsonObject parameters)
		{
			StringBuilder uriBuilder = new StringBuilder();

			List<object> values = new List<object>();

			string rewrittenUriFormat = TokenFormatRewriteRegex.Replace(_uriFormat, delegate(Match m)
			{
				Group startGroup = m.Groups["start"];
				Group propertyGroup = m.Groups["property"];
				Group formatGroup = m.Groups["format"];
				Group endGroup = m.Groups["end"];

				if ((operation.Length != 0) && String.CompareOrdinal(propertyGroup.Value, "operation") == 0)
				{
					values.Add(operation);
				}
				else if (_parameters != null)
				{
					values.Add(_parameters[propertyGroup.Value]);
				}

				return new string('{', startGroup.Captures.Count) + (values.Count - 1) + formatGroup.Value + new string('}', endGroup.Captures.Count);
			});

			if (values.Count != 0)
			{
				uriBuilder.AppendFormat(CultureInfo.InvariantCulture, rewrittenUriFormat, values.ToArray());
			}
			else
			{
				uriBuilder.Append(rewrittenUriFormat);
			}
			if (rewrittenUriFormat.IndexOf('?') < 0)
			{
				uriBuilder.Append("?");
			}

			if (parameters != null)
			{
				foreach (KeyValuePair<string, object> param in (IDictionary<string, object>)parameters)
				{
					string value = String.Format(CultureInfo.InvariantCulture, "{0}", param.Value);
					value = HttpUtility.UrlEncode(value);

					uriBuilder.AppendFormat("&{0}={1}", param.Key, value);
				}
			}

			return new Uri(uriBuilder.ToString(), UriKind.Absolute);
		}

		private dynamic ProcessJsonStream(Stream stream)
		{
			dynamic result = null;

			string responseText = (new StreamReader(stream)).ReadToEnd();
			JsonReader jsonReader = new JsonReader(responseText);
			result = jsonReader.ReadValue();

			return result;
		}
		
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			if (_parameters == null)
			{
				_parameters = new Dictionary<string, object>();
			}
			_parameters[binder.Name] = value;
			return true;
		}
	}
}

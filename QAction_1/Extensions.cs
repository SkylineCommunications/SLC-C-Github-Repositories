namespace Skyline.Protocol.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;
	using System.Text.RegularExpressions;

	using Skyline.DataMiner.Scripting;

	public static class Extensions
	{
		private const string StatusCodePattern = "HTTP\\/\\d+\\.\\d+\\s(\\d*)\\s(.*)";

		public static bool IsSuccessStatusCode(this SLProtocol protocol)
		{
			var statusCode = Convert.ToString(protocol.GetParameter(Parameter.statuscode));

			RegexOptions options = RegexOptions.Multiline;

			var match = Regex.Match(statusCode, StatusCodePattern, options);
			var code = Convert.ToInt32(match.Groups[1].Value);
			var message = match.Groups[2].Value.Trim();

			if (code <= 200 && code < 300)
			{
				return true;
			}
			else
			{
				protocol.Log($"QA{protocol.QActionID}|IsSuccessStatusCode|Error {code}: {message}", LogType.Error, LogLevel.NoLogging);
				return false;
			}
		}

		public static int GetStatusCode(this SLProtocol protocol)
		{
			var statusCode = Convert.ToString(protocol.GetParameter(Parameter.statuscode));

			RegexOptions options = RegexOptions.Multiline;

			var match = Regex.Match(statusCode, StatusCodePattern, options);
			var code = Convert.ToInt32(match.Groups[1].Value);

			return code;
		}

		public static string FriendlyDescription<T>(this T requestType) where T : Enum
		{
			var name = requestType.ToString();
			FieldInfo field = typeof(T).GetField(name);
			object[] attribs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
			if (attribs.Length > 0)
			{
				return ((DescriptionAttribute)attribs[0]).Description;
			}

			return name;
		}

		public static T ParseEnumDescription<T>(string description) where T : Enum
		{
			var enumType = typeof(T);
			var descriptions = enumType.GetFields().ToDictionary(field => field, field => field.GetCustomAttribute<DescriptionAttribute>());
			var @enum = descriptions.FirstOrDefault(desc => desc.Value != null && desc.Value.Description == description);
			if (@enum.Value != null)
			{
				return (T)Enum.Parse(enumType, @enum.Key.Name);
			}

			throw new KeyNotFoundException("There is no value for the given description");
		}

		public static List<List<T>> ToRows<T>(this IEnumerable<IEnumerable<T>> columns)
		{
			return columns.SelectMany(x => x)
				.Select((x, i) => new { V = x, Index = i })
				.GroupBy(x => (x.Index + 1) % columns.First().Count())
				.Select(g => g.Select(x => x.V).ToList())
				.ToList();
		}

		public static string Base64Encode(this string plainText)
		{
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		public static string Base64Decode(this string base64EncodedData)
		{
			var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
			return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
		}
	}
}

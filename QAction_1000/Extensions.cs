// Ignore Spelling: Github

namespace Skyline.Protocol.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;
	using System.Text.RegularExpressions;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.PollManager;

	public static class Extensions
	{
		private static readonly Regex StatusCodeRegex = new Regex("HTTP\\/\\d+\\.\\d+\\s(\\d*)\\s(.*)", RegexOptions.Multiline | RegexOptions.Compiled);

		public static bool IsSuccessStatusCode(this SLProtocol protocol)
		{
			var statusCode = Convert.ToString(protocol.GetParameter(Parameter.statuscode));

			var match = StatusCodeRegex.Match(statusCode);
			var code = Convert.ToInt32(match.Groups[1].Value);
			var message = match.Groups[2].Value.Trim();

			if (code >= 200 && code < 300)
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

			var match = StatusCodeRegex.Match(statusCode);
			var code = Convert.ToInt32(match.Groups[1].Value);

			return code;
		}

		public static int[] GetTableIDs<T>(this T requestType) where T : Enum
		{
			var name = requestType.ToString();
			FieldInfo field = typeof(T).GetField(name);
			object[] attribs = field.GetCustomAttributes(typeof(TableAttribute), false);
			if (attribs.Length > 0)
			{
				return ((TableAttribute)attribs[0]).TableIDs;
			}

			return new int[0];
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
			foreach (var field in enumType.GetFields())
			{
				var attrib = field.GetCustomAttribute<DescriptionAttribute>();
				if (attrib != null && attrib.Description == description)
				{
					return (T)Enum.Parse(enumType, field.Name);
				}
			}

			throw new KeyNotFoundException("There is no value for the given description");
		}

		public static List<List<T>> ToRows<T>(this IEnumerable<IEnumerable<T>> columns)
		{
			var columnList = columns.Select(c => c.ToList()).ToList();
			return Enumerable.Range(0, columnList[0].Count)
				.Select(r => columnList.Select(c => c[r]).ToList())
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

		public static string GetGithubPermission(this PermissionType type)
		{
			switch(type)
			{
				case PermissionType.Read:
					return "pull";

				case PermissionType.Triage:
					return "triage";

				case PermissionType.Write:
					return "push";

				case PermissionType.Maintain:
					return "maintain";

				case PermissionType.Admin:
					return "admin";

				default:
					throw new NotSupportedException("The given PermissionType is not supported.");
			}
		}

		public static T Execute<T>(this WorkflowType workflowType, Func<T> execute)
		{
			return execute();
		}
	}
}

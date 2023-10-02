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

            if(code <= 200 && code < 300)
            {
                return true;
            }
            else
            {
                protocol.Log($"QA{protocol.QActionID}|IsSuccessStatusCode|Error {code}: {message}", LogType.Error, LogLevel.NoLogging);
                return false;
            }
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

        public static List<List<T>> ToRows<T>(this IEnumerable<IEnumerable<T>> columns)
        {
            return columns.SelectMany(x => x)
                .Select((x, i) => new { V = x, Index = i })
                .GroupBy(x => (x.Index + 1) % columns.First().Count())
                .Select(g => g.Select(x => x.V).ToList())
                .ToList();
        }
    }
}

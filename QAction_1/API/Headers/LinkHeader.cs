namespace Skyline.Protocol.API.Headers
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using System.Web;

	public static class LinkRel
	{
		public const string First = "FIRST";
		public const string Last = "LAST";
		public const string Next = "NEXT";
		public const string Previous = "PREV";
	}

	public class LinkHeader
	{
		private static readonly Regex RelRegex = new Regex("(?<=rel=\").+?(?=\")", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static readonly Regex LinkRegex = new Regex("(?<=<).+?(?=>)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private readonly string headerRaw;

		public LinkHeader(string header)
		{
			headerRaw = header;
			Parse();
		}

		public bool IsLast { get; private set; }

		public bool IsFirst { get; private set; }

		public bool HasNext { get; private set; }

		public bool HasPrevious { get; private set; }

		public int NextPage { get; private set; }

		public int PreviousPage { get; private set; }

		public int FirstPage { get; private set; }

		public int LastPage { get; private set; }

		public int CurrentPage { get; }

		private static int GetPageFromUrl(string url)
		{
			var uri = new Uri(url, UriKind.Absolute);
			var page = Convert.ToInt32(HttpUtility.ParseQueryString(uri.Query).Get("page"));
			return page;
		}

		private void Parse()
		{
			var entries = headerRaw.Split(',');
			var links = new Dictionary<string, string>();
			foreach (var entry in entries)
			{
				var relMatch = RelRegex.Match(entry);
				var linkMatch = LinkRegex.Match(entry);

				if (relMatch.Success && linkMatch.Success)
				{
					string rel = relMatch.Value.ToUpper();
					string link = linkMatch.Value;

					links.Add(rel.ToUpper(), link);
				}
			}

			IsLast = !links.ContainsKey(LinkRel.Next);
			IsFirst = !links.ContainsKey(LinkRel.Previous);
			HasNext = links.ContainsKey(LinkRel.Next);
			HasPrevious = links.ContainsKey(LinkRel.Previous);

			// Parse NextPage
			if (links.TryGetValue(LinkRel.Next, out var next))
			{
				var page = GetPageFromUrl(next);
				NextPage = page;
			}
			else
			{
				NextPage = -1;
			}

			// Parse PreviousPage
			if (links.TryGetValue(LinkRel.Previous, out var previous))
			{
				var page = GetPageFromUrl(previous);
				PreviousPage = page;
			}
			else
			{
				PreviousPage = -1;
			}

			// Parse FirstPage
			if (links.TryGetValue(LinkRel.First, out var first))
			{
				var page = GetPageFromUrl(first);
				FirstPage = page;
			}
			else if (links.TryGetValue(LinkRel.Next, out _))
			{
				var page = GetPageFromUrl(next);
				FirstPage = page - 1;
			}
			else
			{
				FirstPage = -1;
			}

			// Parse LastPage
			if (links.TryGetValue(LinkRel.Last, out var last))
			{
				var page = GetPageFromUrl(last);
				LastPage = page;
			}
			else if (links.TryGetValue(LinkRel.Previous, out _))
			{
				var page = GetPageFromUrl(previous);
				LastPage = page + 1;
			}
			else
			{
				LastPage = -1;
			}
		}
	}
}

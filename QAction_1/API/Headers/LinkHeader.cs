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
		private readonly string headerRaw;

		private readonly Dictionary<string, string> links = new Dictionary<string, string>();

		public LinkHeader(string header)
		{
			headerRaw = header;
		}

		public bool IsLast
		{
			get
			{
				Parse();
				return !links.ContainsKey(LinkRel.Next);
			}
		}

		public bool IsFirst
		{
			get
			{
				Parse();
				return !links.ContainsKey(LinkRel.Previous);
			}
		}

		public bool HasNext
		{
			get
			{
				Parse();
				return links.ContainsKey(LinkRel.Next);
			}
		}

		public bool HasPrevious
		{
			get
			{
				Parse();
				return links.ContainsKey(LinkRel.Previous);
			}
		}

		public int NextPage
		{
			get
			{
				Parse();
				if (links.TryGetValue(LinkRel.Next, out var next))
				{
					var page = GetPageFromUrl(next);
					return page;
				}

				throw new InvalidOperationException("There is no next page.");
			}
		}

		public int PreviousPage
		{
			get
			{
				Parse();
				if (links.TryGetValue(LinkRel.Previous, out var prev))
				{
					var page = GetPageFromUrl(prev);
					return page;
				}

				throw new InvalidOperationException("There is no previous page.");
			}
		}

		public int FirstPage
		{
			get
			{
				Parse();
				if (links.TryGetValue(LinkRel.First, out var first))
				{
					var page = GetPageFromUrl(first);
					return page;
				}

				if (links.TryGetValue(LinkRel.Next, out var next))
				{
					var page = GetPageFromUrl(next);
					return page - 1;
				}

				throw new InvalidOperationException("There is no first page.");
			}
		}

		public int LastPage
		{
			get
			{
				Parse();
				if (links.TryGetValue(LinkRel.Last, out var last))
				{
					var page = GetPageFromUrl(last);
					return page;
				}

				if (links.TryGetValue(LinkRel.Previous, out var prev))
				{
					var page = GetPageFromUrl(prev);
					return page + 1;
				}

				throw new InvalidOperationException("There is no last page.");
			}
		}

		public int CurrentPage
		{
			get
			{
				Parse();
				if (links.TryGetValue(LinkRel.Previous, out var prev))
				{
					var page = GetPageFromUrl(prev);
					return page + 1;
				}

				if (links.TryGetValue(LinkRel.Next, out var next))
				{
					var page = GetPageFromUrl(next);
					return page - 1;
				}

				throw new InvalidOperationException("Cannot retrieve the current page.");
			}
		}

		private static int GetPageFromUrl(string url)
		{
			var uri = new Uri(url, UriKind.Absolute);
			var page = Convert.ToInt32(HttpUtility.ParseQueryString(uri.Query).Get("page"));
			return page;
		}

		private void Parse()
		{
			if (links.Count != 0) return;

			var entries = headerRaw.Split(',');
			foreach (var entry in entries)
			{
				var relMatch = Regex.Match(entry, "(?<=rel=\").+?(?=\")", RegexOptions.IgnoreCase);
				var linkMatch = Regex.Match(entry, "(?<=<).+?(?=>)", RegexOptions.IgnoreCase);

				if (relMatch.Success && linkMatch.Success)
				{
					string rel = relMatch.Value.ToUpper();
					string link = linkMatch.Value;

					links.Add(rel.ToUpper(), link);
				}
			}
		}
	}
}

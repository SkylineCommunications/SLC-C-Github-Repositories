// Ignore Spelling: Pid

namespace Skyline.Protocol.Tables
{
	using System;
	using System.Collections.Generic;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.SecureCoding.SecureSerialization.Json.Newtonsoft;

	public class PkCache
	{
		private readonly int _cachePid;
		private readonly Dictionary<string, List<string>> _cache;

		private PkCache(int cachePid, Dictionary<string, List<string>> cache)
		{
			_cachePid = cachePid;
			_cache = cache;
		}

		public int CachePid => _cachePid;

		public List<string> this[string repositoryId]
		{
			get
			{
				if (_cache.TryGetValue(repositoryId, out var pks))
				{
					return pks;
				}

				var list = new List<string>();
				_cache[repositoryId] = list;
				return list;
			}
		}

		public static PkCache GetCache(SLProtocol protocol, int tablePid)
		{
			switch (tablePid)
			{
				case Parameter.Repositories.tablePid:
				case Parameter.Repositorytags.tablePid:
				case Parameter.Repositoryreleases.tablePid:
				case Parameter.Repositoryreleaseassets.tablePid:
				case Parameter.Repositoryissues.tablePid:
				case Parameter.Repositoryworkflows.tablePid:
				{
					var cachePid = tablePid - 9;
					var raw = Convert.ToString(protocol.GetParameter(cachePid));
					if (String.IsNullOrEmpty(raw))
					{
						return new PkCache(cachePid, new Dictionary<string, List<string>>());
					}

					var cache = SecureNewtonsoftDeserialization.DeserializeObject<Dictionary<string, List<string>>>(raw);
					return new PkCache(cachePid, cache);
				}

				default:
					throw new NotSupportedException("This table doesn't have a PK cache.");
			}
		}

		public List<string> GetCacheForRepository(string repositoryId)
		{
			return this[repositoryId];
		}

		public void Store(SLProtocol protocol)
		{
			var raw = JsonConvert.SerializeObject(_cache);
			protocol.SetParameter(_cachePid, raw);
		}
	}
}

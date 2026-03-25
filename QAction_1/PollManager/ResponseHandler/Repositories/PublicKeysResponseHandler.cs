namespace Skyline.Protocol.PollManager.ResponseHandler.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Repositories;
	using Skyline.DataMiner.Utils.SecureCoding.SecureSerialization.Json.Newtonsoft;
	using Skyline.Protocol.Extensions;
	using Skyline.Protocol.PollManager.RequestHandler.Repositories;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesResponseHandler
	{
		public static void HandleRepositoriesPublicKeysResponse(SLProtocol protocol)
		{
			// Check status code
			var code = protocol.GetStatusCode();
			if (code >= 400 && code <= 499)
			{
				HandleNextRepositoryPublicKey(protocol);
				return;
			}

			if (!protocol.IsSuccessStatusCode())
			{
				return;
			}

			// Parse response
			var response = SecureNewtonsoftDeserialization.DeserializeObject<PublicKey>(
				Convert.ToString(protocol.GetParameter(Parameter.getrepositorypublickeycontent)));
			var url = Convert.ToString(protocol.GetParameter(Parameter.getrepositorypublickeyurl));

			if (response == null)
			{
				protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryPublicKeyResponse|response was null.", LogType.Error, LogLevel.Level1);
				return;
			}

			// Parse url to check which respository this issue is linked to
			var pattern = "repos\\/(.*)\\/(.*)\\/actions/secrets/public-key";
			var options = RegexOptions.Multiline;

			var match = Regex.Match(url, pattern, options);
			var owner = match.Groups[1].Value;
			var name = match.Groups[2].Value;

			// Update the repositories table
			if (!SLTables.Repositories.TryGetRow(protocol, $"{owner}/{name}", out var rawRow))
			{
				return;
			}

			var repo = RepositoriesRowConverter.Instance.FromRawValue(rawRow);
			if (repo == null)
			{
				return;
			}

			repo.PublicKeyID = response.KeyID;
			repo.PublicKey = response.Key;
			SLTables.Repositories.SetRow(protocol, RepositoriesRowConverter.Instance.ToRawValue(repo));

			HandleNextRepositoryPublicKey(protocol);
		}

		private static void HandleNextRepositoryPublicKey(SLProtocol protocol)
		{
			// Get the next repo in the queue to fetch
			var queue = SecureNewtonsoftDeserialization.DeserializeObject<List<string>>(
				Convert.ToString(protocol.GetParameter(Parameter.getrepositorypublickeyqueue)));
			var next = queue?.FirstOrDefault();

			if (next == null)
			{
				return;
			}

			protocol.SetParameter(Parameter.getrepositorypublickeyqueue, JsonConvert.SerializeObject(queue.Skip(1)));

			////var nextOwner = next.Split('/')[0];
			////var nextName = next.Split('/')[1];
			RepositoriesRequestHandler.HandleRepositoriesPublicKeysRequest(protocol, next);
		}
	}
}

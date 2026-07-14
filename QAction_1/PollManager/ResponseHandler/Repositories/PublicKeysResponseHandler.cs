namespace Skyline.Protocol.PollManager.ResponseHandler.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Repositories;
	using Skyline.DataMiner.Utils.SecureCoding.SecureSerialization.Json.Newtonsoft;
	using Skyline.Protocol.API;
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
				HandleNextRepositoryPublicKey(protocol);
				return;
			}

			var parameterIds = new uint[]
			{
				Parameter.getrepositorypublickeycontent,
				Parameter.getrepositorypublickeyurl,
			};

			var parameterValues = Array.ConvertAll((object[])protocol.GetParameters(parameterIds), Convert.ToString);
			var response = SecureNewtonsoftDeserialization.DeserializeObject<PublicKey>(parameterValues[0]);
			var url = parameterValues[1];

			if (response == null)
			{
				protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryPublicKeyResponse|response was null.", LogType.Error, LogLevel.Level1);
				HandleNextRepositoryPublicKey(protocol);
				return;
			}

			// Parse url to check which repository this public key is linked to
			GithubUrlHelper.TryParseRepoOwnerAndName(url, out var owner, out var name);

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
				SLTables.PollManager.SetPollingStatus(protocol, RequestType.Repositories_PublicKey, PollingStatus.Idle);
				return;
			}

			protocol.SetParameter(Parameter.getrepositorypublickeyqueue, JsonConvert.SerializeObject(queue.Skip(1)));
			RepositoriesRequestHandler.HandleRepositoriesPublicKeysRequest(protocol, next, true);
		}
	}
}

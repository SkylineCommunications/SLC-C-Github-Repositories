// Ignore Spelling: Workflows Workflow

namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using System;
	using System.Reflection;
	using System.Text;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.API.Secrets;

	public static partial class RepositoriesRequestHandler
	{
		public static void CreateRepositorySecret(SLProtocol protocol, string repositoryId, string secretName, string secret)
		{
			var publicKeyId = Convert.ToString(protocol.GetParameterIndexByKey(Parameter.Repositories.tablePid, repositoryId, Parameter.Repositories.Idx.repositoriespublickeyid + 1));
			var publicKey = Convert.ToString(protocol.GetParameterIndexByKey(Parameter.Repositories.tablePid, repositoryId, Parameter.Repositories.Idx.repositoriespublickey + 1));

			var box = Sodium.SealedPublicKeyBox.Create(
				Encoding.UTF8.GetBytes(secret),
				Convert.FromBase64String(publicKey));

			var body = new PutRepositorySecret
			{
				EncryptedValue = Convert.ToBase64String(box),
				KeyID = publicKeyId,
			};

			protocol.SetParameter(Parameter.putrepositorysecreturl, $"repos/{repositoryId}/actions/secrets/{secretName}");
			protocol.SetParameter(Parameter.putrepositorysecretbody, JsonConvert.SerializeObject(body));
			protocol.CheckTrigger(226);
		}
	}
}

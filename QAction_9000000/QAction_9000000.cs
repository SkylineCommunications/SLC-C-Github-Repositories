using System;

using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
using Skyline.DataMiner.Core.InterAppCalls.Common.CallBulk;
using Skyline.DataMiner.Scripting;
using Skyline.Protocol.InterApp;

/// <summary>
/// DataMiner QAction Class.
/// </summary>
public static class QAction
{
    /// <summary>
    /// The QAction entry point.
    /// </summary>
    /// <param name="protocol">Link with SLProtocol process.</param>
    public static void Run(SLProtocol protocol)
    {
        try
        {
			// Get the incoming InterApp Message
			var raw = Convert.ToString(protocol.GetParameter(protocol.GetTriggerParameter()));
			var receivedCall = InterAppCallFactory.CreateFromRaw(raw, Types.KnownTypes);

			// Handle the message
			foreach (var message in receivedCall.Messages)
			{
				message.TryExecute(protocol, protocol, Mapping.MessageToExecutorMapping, out var returnMessage);
				if (returnMessage != null)
				{
					message.Reply(protocol.SLNet.RawConnection, returnMessage, Types.KnownTypes);
				}
			}
		}
        catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
    }
}

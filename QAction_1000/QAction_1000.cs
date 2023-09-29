using System;
using Skyline.DataMiner.Scripting;
using Skyline.Protocol;
using Skyline.Protocol.Tables;

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
            var ids = new[]
            {
                Parameter.addrepositoryname,
                Parameter.addrepositoryowner,
            };
            var parameters = (object[])protocol.GetParameters(Array.ConvertAll(ids, Convert.ToUInt32));

            // Add through name and owner
            var row = new RepositoriesTableRow
            {
                FullName = $"{parameters[1]}/{parameters[0]}",
                Name = Convert.ToString(parameters[0]),
                Owner = Convert.ToString(parameters[1]),
            };

            row.SaveToProtocol(protocol);

            protocol.SetParameters(ids, new object[] { Exceptions.NotAvailable, Exceptions.NotAvailable });
        }
        catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
    }
}

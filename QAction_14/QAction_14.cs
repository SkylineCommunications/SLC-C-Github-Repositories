using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Skyline.DataMiner.Scripting;

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
            var ids = new uint[]
            {
                Parameter.repositoryconfigurationowner,
                Parameter.repositoryconfigurationname,
            };
            var parameters = Array.ConvertAll((object[])protocol.GetParameters(ids), Convert.ToString);
            var sets = new Dictionary<int, object>
            {
                { Parameter.getrepositoryurl,           $"repos/{parameters[0]}/{parameters[1]}" },
                { Parameter.getrepositoryissuesurl,     $"repos/{parameters[0]}/{parameters[1]}/issues?state=all" },
            };
            protocol.SetParameters(sets.Keys.ToArray(), sets.Values.ToArray());
        }
        catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
    }
}

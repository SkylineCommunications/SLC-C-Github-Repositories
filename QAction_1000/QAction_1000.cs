using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                Parameter.addrepositoryurl,
            };
            var parameters = (object[])protocol.GetParameters(Array.ConvertAll(ids, Convert.ToUInt32));

            protocol.Log($"QA{protocol.QActionID}|Run|parameters: {parameters}", LogType.DebugInfo, LogLevel.NoLogging);
            protocol.Log($"QA{protocol.QActionID}|Run|name: {parameters[0]}", LogType.DebugInfo, LogLevel.NoLogging);
            protocol.Log($"QA{protocol.QActionID}|Run|owner: {parameters[1]}", LogType.DebugInfo, LogLevel.NoLogging);
            protocol.Log($"QA{protocol.QActionID}|Run|url: {parameters[2]}", LogType.DebugInfo, LogLevel.NoLogging);

            // Add through name and owner
            if (Convert.ToString(parameters[2]) == Exceptions.NotAvailable)
            {
                var row = new RepositoriesTableRow
                {
                    FullName = $"{parameters[1]}/{parameters[0]}",
                    Name = Convert.ToString(parameters[0]),
                    Owner = Convert.ToString(parameters[1]),
                };

                row.SaveToProtocol(protocol);
            }

            // Add through url
            else
            {
                string pattern = @"https:\/\/(www\.)?github\.com\/(.*)\/(.*)";
                RegexOptions options = RegexOptions.Multiline;
                var match = Regex.Match(pattern, Convert.ToString(parameters[2]).Trim(), options);

                if(!match.Success)
                {
                    protocol.Log($"QA{protocol.QActionID}|Run|Could not parse url.", LogType.Error, LogLevel.NoLogging);
                    return;
                }

                var owner = match.Groups[2].Value;
                var name = match.Groups[3].Value.Replace(".git", String.Empty);

                var row = new RepositoriesTableRow
                {
                    FullName = $"{owner}/{name}",
                    Name = name,
                    Owner = owner,
                };

                row.SaveToProtocol(protocol);
            }

            protocol.SetParameters(ids, new object[] { Exceptions.NotAvailable, Exceptions.NotAvailable, Exceptions.NotAvailable });
        }
        catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
    }
}

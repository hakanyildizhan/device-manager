// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace DeviceManager.Setup.Server.CustomActions
{
    public partial class CustomActions
    {
        /// <summary>
        /// After install completes, runs an SQL file to give the IIS App Pool necessary permissions on the application database.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult GrantDatabasePermissions(Session session)
        {
            //System.Diagnostics.Debugger.Launch();
            session.Log("Begin GrantDatabasePermissions");
            string connString = Utility.GenerateConnectionString(session);

            if (string.IsNullOrEmpty(connString))
            {
                session.Log("Invalid input for GrantDatabasePermissions");
                return ActionResult.Success;
            }

            string sqlCommand = GetEmbeddedSQLFileContents("UserPermissions.sql");
            if (string.IsNullOrEmpty(sqlCommand))
            {
                session.Log("Could not get SQL file contents for GrantDatabasePermissions");
                return ActionResult.Success;
            }

            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction("AfterInstallUserPermissions");
                SqlCommand cmd = new SqlCommand(sqlCommand, connection);
                cmd.Transaction = transaction;
                try
                {
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                    session.Log("Granted database permissions successfully");
                    return ActionResult.Success;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    session.Log("Error occured. Could not grant database permissions");
                    return ActionResult.Success;
                }
            }
        }



        /// <summary>
        /// Gets the contents of the embedded file.
        /// </summary>
        /// <returns></returns>
        private static string GetEmbeddedSQLFileContents(string resourceFileName)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "DeviceManager.Setup.Server.CustomActions.Resources." + resourceFileName;

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}

using System;
using System.Data;
using System.Data.SQLite;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace QBot
{
    public class DataAccess
    {
        public static readonly string _con = $"Data Source = {_DATABASENAME};Version=3";

        public const string _DATABASENAME = "guilds.sqlite";

        public static string DataPath(string path)
        {
            return $"Data Source = {path};Version=3";
        }

        /// <summary>This method fills a DataSet with data from a table.</summary>
        /// <param name="sql">SQL query to be executed</param>
        /// <param name="con">Connection information</param>
        /// <returns>Returns DataSet with queried results</returns>
        public static async Task<DataSet> FillDataSet(string sql, string con)
        {
            DataSet ds = new DataSet();
            SQLiteConnection connection = new SQLiteConnection(con);
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            await Task.Run(() =>
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                    da.Fill(ds);
                    da.Dispose();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                    command.Dispose();
                }
            });
            return ds;
        }

        /// <summary>
        /// Checks to see if database exists, returns true if it makes it.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool CreateDatabase(string name)
        {
            if(!System.IO.File.Exists(name))
            {
                SQLiteConnection.CreateFile(name);
                return true;
            }
            return false;
        }

        /// <summary>Executes commands.</summary>
        /// <param name="con">Connection information</param>
        /// <param name="commands">Commands to be executed</param>
        /// <returns>Returns true if command(s) executed successfully</returns>
        public static async Task<bool> ExecuteCommand(string con, params SQLiteCommand[] commands)
        {
            SQLiteConnection connection = new SQLiteConnection(con);
            bool success = false;
            await Task.Run(() =>
            {
                try
                {
                    connection.Open();
                    foreach(SQLiteCommand command in commands)
                    {
                        command.Connection = connection;
                        command.Prepare();
                        command.ExecuteNonQuery();
                    }
                    success = true;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            });
            return success;
        }
    }
}
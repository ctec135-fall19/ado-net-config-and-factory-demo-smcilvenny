using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration; //update refrences when adding this
using System.Data.Common;
using System.Data.SqlClient;
using static System.Console;

namespace FactoryExample
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Get app config info
            string dataProvider = ConfigurationManager.AppSettings["provider"];
            WriteLine("   provider: {0}", dataProvider);

            //alternate way 
            var cnStringBuilder = new SqlConnectionStringBuilder
            {
                InitialCatalog = "NorthWind",
                DataSource = @"(localdb)\msqllocaldb",
                ConnectTimeout = 30,
                IntegratedSecurity = true
            };
            WriteLine($"\tBuilt Connection String: \n{cnStringBuilder.ConnectionString}\n");

            #endregion

            //get factory
            DbProviderFactory factory = DbProviderFactories.GetFactory(dataProvider);

            using (DbConnection connectiong = factory.CreateConnection())
            {
                if (connectiong == null)
                {
                    WriteLine("There was an issue creating the connection");
                    return; //returns out of the main method call
                }
                else WriteLine("Connection created");

                connectiong.ConnectionString = cnStringBuilder.ConnectionString;
                connectiong.Open();

                DbCommand myCommand = factory.CreateCommand();
                if (myCommand == null)
                {
                    WriteLine("There is an issue connecting");
                    return;
                }
                else WriteLine($"Your command object is a {myCommand.GetType().Name}");

                myCommand.Connection = connectiong;
                myCommand.CommandText = "Select * from Shippers";

                using (DbDataReader dataReader = myCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        WriteLine($"->shiper # {dataReader["ShipperId"]}" +
                            $"name is a {dataReader[1]} phone {dataReader[2]}");
                    }
                }
            }
        }
    }
}

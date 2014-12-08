using Microsoft.VisualStudio.Data.Framework;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.DdexProvider
{

    internal static class AdoDotNetProvider
    {
        public static DbProviderFactory GetProviderFactory(string providerInvariantName)
        {
            return DbProviderFactories.GetFactory(providerInvariantName);
        }

        public static void ApplyMappings(DataTable dataTable, IDictionary<string, object> mappings)
        {
            if (dataTable == null)
            {
                throw new ArgumentNullException("dataTable");
            }
            if (mappings == null)
            {
                return;
            }
            foreach (KeyValuePair<string, object> mapping in mappings)
            {
                DataColumn item = dataTable.Columns[mapping.Key];
                if (item != null)
                {
                    continue;
                }
                string value = mapping.Value as string;
                if (value == null)
                {
                    int num = (int)mapping.Value;
                    if (num >= 0 && num < dataTable.Columns.Count)
                    {
                        item = dataTable.Columns[num];
                    }
                }
                else
                {
                    item = dataTable.Columns[value];
                }
                if (item == null)
                {
                    continue;
                }
                dataTable.Columns.Add(new DataColumn(mapping.Key, item.DataType, string.Concat("[", item.ColumnName.Replace("]", "]]"), "]")));
            }
        }

        public static T CreateObject<T>(string invariantName)
        where T : class
        {
            if (invariantName == null)
            {
                throw new DataException("");
            }
            DbProviderFactory providerFactory = GetProviderFactory(invariantName);
            if (providerFactory == null)
            {
                string adoDotNetProviderNotRegistered = "";
                object[] objArray = new object[] { invariantName };
                throw new DataException(string.Format((IFormatProvider)null, adoDotNetProviderNotRegistered, objArray));
            }
            T t = default(T);
            if (typeof(T) == typeof(DbCommandBuilder))
            {
                t = (T)(providerFactory.CreateCommandBuilder() as T);
            }
            if (typeof(T) == typeof(DbConnection))
            {
                t = (T)(providerFactory.CreateConnection() as T);
            }
            if (typeof(T) == typeof(DbConnectionStringBuilder))
            {
                t = (T)(providerFactory.CreateConnectionStringBuilder() as T);
            }
            if (typeof(T) == typeof(DbParameter))
            {
                t = (T)(providerFactory.CreateParameter() as T);
            }
            if (t == null)
            {
                string adoDotNetProviderObjectNotImplemented = "";
                object[] objArray1 = new object[] { invariantName, typeof(T).Name };
                throw new DataException(string.Format((IFormatProvider)null, adoDotNetProviderObjectNotImplemented, objArray1));
            }
            return t;
        }
    }

    /// <summary>Provides an implementation of the <see cref="T:Microsoft.VisualStudio.Data.Services.SupportEntities.IVsDataObjectSelector" /> interface using the ADO.NET <see cref="M:System.Data.Common.DbConnection.GetSchema" /> method.</summary>
    public class AdoDotNetObjectSelector : DataObjectSelector
    {
        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.VisualStudio.Data.Framework.AdoDotNet.AdoDotNetObjectSelector" /> class.</summary>
        public AdoDotNetObjectSelector()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.VisualStudio.Data.Framework.AdoDotNet.AdoDotNetObjectSelector" /> class with the data connection object.</summary>
        /// <param name="connection">An <see cref="T:Microsoft.VisualStudio.Data.Services.IVsDataConnection" /> object representing the communication to the data source.</param>
        public AdoDotNetObjectSelector(IVsDataConnection connection)
            : base(connection)
        {
        }

        /// <summary>Applies the selector mappings.</summary>
        /// <param name="dataTable">The schema returned by the call to the <see cref="M:System.Data.Common.DbConnection.GetSchema" /> method.</param>
        /// <param name="mappings">Key/value pairs containing the selector mappings.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="schema" /> parameter is null.</exception>
        protected static void ApplyMappings(DataTable dataTable, IDictionary<string, object> mappings)
        {

            AdoDotNetProvider.ApplyMappings(dataTable, mappings);
        }

        /// <summary>Returns a data reader for the data objects retrieved from the object store, which are filtered by the specified restrictions, properties, and parameters.</summary>
        /// <returns>An <see cref="T:Microsoft.VisualStudio.Data.Services.SupportEntities.IVsDataReader" /> object representing a data reader for the selected data objects.</returns>
        /// <param name="typeName">The data source–specific name of the specified type to retrieve data objects for.</param>
        /// <param name="restrictions">The restrictions for filtering the data objects returned.</param>
        /// <param name="properties">Specifies the property values of the requested data objects. This is not supported in the current version of DDEX.</param>
        /// <param name="parameters">An array containing the parameters for the specified type.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="typeName" /> parameter is null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="parameters" /> parameter is not valid. Either it is null, or the number of elements contained in it is not 1 or 2, or the first element is not a string.</exception>
        /// <exception cref="T:System.InvalidOperationException">The site is null.</exception>
        /// <exception cref="T:System.NotImplementedException">The provider cannot be obtained.</exception>
        protected override IVsDataReader SelectObjects(string typeName, object[] restrictions, string[] properties, object[] parameters)
        {
            IVsDataReader adoDotNetTableReader;
            string str;
            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }
            if (parameters == null || (int)parameters.Length < 1 || (int)parameters.Length > 2 || !(parameters[0] is string))
            {
                throw new ArgumentException();
            }
            if (base.Site == null)
            {
                throw new InvalidOperationException();
            }
            object lockedProviderObject = base.Site.GetLockedProviderObject();
            if (lockedProviderObject == null)
            {
                throw new NotImplementedException();
            }
            try
            {
                DbConnection dbConnection = lockedProviderObject as DbConnection;
                if (dbConnection == null)
                {
                    throw new NotImplementedException();
                }
                string[] strArrays = null;
                if (restrictions != null)
                {
                    strArrays = new string[(int)restrictions.Length];
                    for (int i = 0; i < (int)strArrays.Length; i++)
                    {
                        string[] strArrays1 = strArrays;
                        int num = i;
                        if (restrictions[i] != null)
                        {
                            str = restrictions[i].ToString();
                        }
                        else
                        {
                            str = null;
                        }
                        strArrays1[num] = str;
                    }
                }
                base.Site.EnsureConnected();
                DataTable schema = dbConnection.GetSchema(parameters[0].ToString(), strArrays);
                if ((int)parameters.Length == 2 && parameters[1] is DictionaryEntry)
                {
                    object[] value = ((DictionaryEntry)parameters[1]).Value as object[];
                    if (value != null)
                    {
                        AdoDotNetObjectSelector.ApplyMappings(schema, DataObjectSelector.GetMappings(value));
                    }
                }
                adoDotNetTableReader = new AdoDotNetTableReader(schema);
            }
            finally
            {
                base.Site.UnlockProviderObject();
            }
            return adoDotNetTableReader;
        }
    }



    /// <summary>
    /// Represents a custom data object selector to supplement or replace
    /// the schema collections supplied by the .NET Framework Data Provider
    /// for Crm.  Many of the enumerations here are required for full
    /// support of the built in data design scenarios.
    /// </summary>
    public class CrmObjectSelector : AdoDotNetObjectSelector
    {
        public CrmObjectSelector()
            : base()
        {

        }

        public CrmObjectSelector(IVsDataConnection connection)
            : base(connection)
        {

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        protected override IVsDataReader SelectObjects(string typeName, object[] restrictions, string[] properties, object[] parameters)
        {
            // DataSourceInformation;
            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }
            //   DataSourceInformation.IdentifierCloseQuote
            // Execute a SQL statement to get the property values
            DbConnection conn = Site.GetLockedProviderObject() as DbConnection;
            Debug.Assert(conn != null, "Invalid provider object.");
            if (conn == null)
            {
                // This should never occur
                throw new NotSupportedException();
            }
            try
            {
                // Ensure the connection is open
                if (Site.State != DataConnectionState.Open)
                {
                    Site.Open();
                }

                // Create a command object       
                DbCommand comm = (DbCommand)conn.CreateCommand();

                // Choose and format SQL based on the type
                bool isHandled;
                comm.CommandText = GetCommandText(typeName, out isHandled, restrictions);
                if (isHandled)
                {
                    var r = comm.ExecuteReader();
                    var reader = new AdoDotNetReader(r);
                    return reader;
                }
                else
                {
                    //  object[] par = parameters
                    if (parameters == null)
                    {
                        parameters = new object[] { typeName };
                    }


                    //if(typeName.ToLower()  == "Columns")
                    //{
                    //    par[0] = typeName;
                    //}

                    var tables = base.SelectObjects(typeName, restrictions, properties, parameters);
                    //#if DEBUG
                    //                    while (tables.Read())
                    //                    {
                    //                        Debug.Write(tables.GetItem("table_name"));
                    //                    }
                    //#endif

                    return tables;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                throw;

            }
            finally
            {
                Site.UnlockProviderObject();
            }
        }



        private string GetCommandText(string typeName, out bool isHandled, object[] restrictions)
        {
            isHandled = false;
            if (typeName.Equals(CrmObjectTypes.Root, StringComparison.OrdinalIgnoreCase))
            {
                isHandled = true;
                return "SELECT name FROM organization";
            }

            //if (typeName.Equals(CrmObjectTypes.CrmTable, StringComparison.OrdinalIgnoreCase))
            //{
            //    return "SELECT * FROM entitymetadata";
            //}

            //if (typeName.Equals(CrmObjectTypes.CrmColumn, StringComparison.OrdinalIgnoreCase))
            //{
            //    if (restrictions == null)
            //    {
            //        throw new ArgumentNullException("must provide entity name restriction");
            //    }
            //    var entityName = restrictions.First();
            //    var commandText = "SELECT entitymetadata.PrimaryIdAttribute, attributemetadata.* FROM entitymetadata INNER JOIN attributemetadata ON entitymetadata.MetadataId = attributemetadata.MetadataId WHERE entitymetadata.LogicalName = '{0}'";
            //    return string.Format(commandText, entityName);
            //}

            if (typeName.Equals(CrmObjectTypes.PluginAssembly, StringComparison.OrdinalIgnoreCase))
            {
                isHandled = true;
                //if (restrictions == null)
                //{
                //    throw new ArgumentNullException("must provide entity name restriction");
                //}
                //  var entityName = restrictions.First();
                var commandText = "SELECT * FROM pluginassembly";
                return commandText;
                //   return string.Format(commandText, entityName);
            }

            return string.Empty;
        }



    }
}

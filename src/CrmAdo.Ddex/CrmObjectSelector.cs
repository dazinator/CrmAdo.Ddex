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

    //public class CrmAdoDataMappedObjectConverter : AdoDotNetMappedObjectConverter
    //{



    //    protected override System.Data.DbType GetDbTypeFromNativeType(string nativeType)
    //    {           
    //        var result = base.GetDbTypeFromNativeType(nativeType);
    //        return result;
    //    }

    //    protected override Type GetFrameworkTypeFromNativeType(string nativeType)
    //    {
    //        var result = base.GetFrameworkTypeFromNativeType(nativeType);
    //        return result;

    //    }

    //    protected override int GetProviderTypeFromNativeType(string nativeType)
    //    {
    //        var result = base.GetProviderTypeFromNativeType(nativeType);
    //        return result;
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        var result = base.Equals(obj);
    //        return result;
    //    }

    //    protected override object ConvertToMappedMember(string typeName, string mappedMemberName, object[] underlyingValues, object[] parameters)
    //    {
    //        var result = base.ConvertToMappedMember(typeName, mappedMemberName, underlyingValues);
    //        return result;
    //    }

    //    protected override object ConvertToUnderlyingRestriction(string mappedTypeName, int substitutionValueIndex, object[] mappedRestrictions, object[] parameters)
    //    {
    //        var result = base.ConvertToUnderlyingRestriction(mappedTypeName, substitutionValueIndex, mappedRestrictions);
    //        return result;
    //    }


    //}

    public class CrmAdoObjectIdentifierConverter : AdoDotNetObjectIdentifierConverter
    {
        protected override string BuildString(string typeName, string[] identifierParts, Microsoft.VisualStudio.Data.Services.DataObjectIdentifierFormat format)
        {
            var result = base.BuildString(typeName, identifierParts, format);
            return result;
        }

        protected override string FormatPart(string typeName, object identifierPart, Microsoft.VisualStudio.Data.Services.DataObjectIdentifierFormat format)
        {
            var result = base.FormatPart(typeName, identifierPart, format);
            return result;
        }

        protected override bool RequiresQuoting(string identifierPart)
        {
            var result = base.RequiresQuoting(identifierPart);
            return result;
        }

        protected override string[] SplitIntoParts(string typeName, string identifier)
        {
            var result = base.SplitIntoParts(typeName, identifier);
            return result;
        }

        protected override object UnformatPart(string typeName, string identifierPart)
        {
            var result = base.UnformatPart(typeName, identifierPart);
            return result;
        }

        public override bool Equals(object obj)
        {
            var result = base.Equals(obj);
            return result;
        }

    }

    public class CrmAdoDataObjectMemberComparer : AdoDotNetObjectMemberComparer
    {
        protected override bool RequiresQuoting(string identifierPart)
        {
            return base.RequiresQuoting(identifierPart);
        }
        public override int Compare(string typeName, object[] identifier, int identifierPart, object value)
        {
            var result = base.Compare(typeName, identifier, identifierPart, value);
            return result;
        }

        public override int Compare(string typeName, string propertyName, object value1, object value2)
        {
            var result = base.Compare(typeName, propertyName, value1, value2);
            return result;
        }
        public override bool Equals(object obj)
        {
            var result = base.Equals(obj);
            return result;
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
            if (string.IsNullOrWhiteSpace(typeName) || typeName.Equals(CrmObjectTypes.Root, StringComparison.OrdinalIgnoreCase))
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


    public class CrmAdoDataParameter : AdoDotNetParameter
    {
        public CrmAdoDataParameter(DbParameter parameter)
            : base(parameter)
        {

        }

        public CrmAdoDataParameter(string providerInvariantName)
            : base(providerInvariantName)
        {

        }

        public CrmAdoDataParameter(DbParameter parameter, bool isDerived)
            : base(parameter, isDerived)
        {

        }

        public CrmAdoDataParameter(string providerInvariantName, bool isDerived)
            : base(providerInvariantName, isDerived)
        {

        }




        protected override int DefaultSize
        {
            get
            {
                var result = base.DefaultSize;
                return result;
            }
        }

        public override bool Equals(object obj)
        {
            var result = base.Equals(obj);
            return result;
        }

        protected override DataParameterDirection GetDirectionCore()
        {
            var result = base.GetDirectionCore();
            return result;
        }

        protected override bool GetIsNullableCore()
        {
            var result = base.GetIsNullableCore();
            return result;
        }

        protected override bool GetIsOptionalCore()
        {
            var result = base.GetIsOptionalCore();
            return result;
        }

        protected override string GetNameCore()
        {
            var result = base.GetNameCore();
            return result;
        }

        protected override int GetSizeCore()
        {
            var result = base.GetSizeCore();
            return result;
        }

        protected override string GetTypeCore()
        {
            var result = base.GetTypeCore();
            return result;
        }

        protected override string GetTypeFrom(object value)
        {
            var result = base.GetTypeFrom(value);
            return result;
        }

        protected override object GetValueCore()
        {
            var result = base.GetValueCore();
            return result;
        }

        protected override bool HasDescriptor
        {
            get
            {
                var result = base.HasDescriptor;
                return result;
            }
        }

        protected override bool IsFixedSize
        {
            get
            {
                var result = base.IsFixedSize;
                return result;
            }
        }

        protected override bool IsSupportedDirection(DataParameterDirection direction)
        {
            var result = base.IsSupportedDirection(direction);
            return result;
        }

        protected override bool IsValidType(string type)
        {
            var result = base.IsValidType(type);
            return result;
        }

        public override void Parse(string value)
        {
            base.Parse(value);
        }

        protected override object TryConvertValue(object value, string type)
        {
            var result = base.TryConvertValue(value, type);
            return result;
        }
    }

    public class CrmAdoConnectionSupport : AdoDotNetConnectionSupport
    {
        protected override void OnStateChanged(DataConnectionStateChangedEventArgs e)
        {
            base.OnStateChanged(e);
        }
        public override void Initialize(object providerObj)
        {
            base.Initialize(providerObj);
        }
        protected override string PrepareCore(string command, DataCommandType commandType, IVsDataParameter[] parameters, int commandTimeout)
        {
            var result = base.PrepareCore(command, commandType, parameters, commandTimeout);
            return result;
        }
        protected override void OnMessageReceived(DataConnectionMessageReceivedEventArgs e)
        {
            base.OnMessageReceived(e);
        }
        public override object GetService(Guid serviceGuid)
        {
            return base.GetService(serviceGuid);
        }
        protected override int ExecuteWithoutResultsCore(string command, DataCommandType commandType, IVsDataParameter[] parameters, int commandTimeout)
        {
            var result = base.ExecuteWithoutResultsCore(command, commandType, parameters, commandTimeout);
            return result;
        }
        protected override IVsDataReader ExecuteCore(string command, DataCommandType commandType, IVsDataParameter[] parameters, int commandTimeout)
        {
            var result = base.ExecuteCore(command, commandType, parameters, commandTimeout);
            return result;
        }
        protected override IVsDataParameter[] DeriveParametersCore(string command, DataCommandType commandType, int commandTimeout)
        {
            var result = base.DeriveParametersCore(command, commandType, commandTimeout);
            return result;
        }
        protected override object CreateService(System.ComponentModel.Design.IServiceContainer container, Type serviceType)
        {
            var result = base.CreateService(container, serviceType);
            return result;
        }
        protected override IVsDataParameter CreateParameterFrom(DbParameter parameter)
        {
            var result = base.CreateParameterFrom(parameter);
            return result;
        }
        protected override IVsDataParameter CreateParameterCore()
        {
            var result = base.CreateParameterCore();
            return result;
        }

        public override void AddService(Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback, bool promote)
        {
            base.AddService(serviceType, callback, promote);
        }
        public override void AddService(Type serviceType, object serviceInstance, bool promote)
        {
            base.AddService(serviceType, serviceInstance, promote);
        }
        protected override void DeriveParametersOn(DbCommand command)
        {
            base.DeriveParametersOn(command);
        }
        protected override IVsDataReader DeriveSchemaCore(string command, DataCommandType commandType, IVsDataParameter[] parameters, int commandTimeout)
        {
            var result = base.DeriveSchemaCore(command, commandType, parameters, commandTimeout);
            return result;
        }

        protected override DbCommand GetCommand(string command, DataCommandType commandType, IVsDataParameter[] parameters, int commandTimeout)
        {
            var result = base.GetCommand(command, commandType, parameters, commandTimeout);
            return result;
        }

    }

    public class CrmAdoDsRefBuilder : DSRefBuilder
    {

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        protected override void AppendToDSRef(object dsRef, string typeName, object[] identifier, object[] parameters)
        {
            base.AppendToDSRef(dsRef, typeName, identifier, parameters);
            return;
        }

    }


    public class CrmAdoDataSourceVersionNumberComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return x.CompareTo(y);
        }
    }

    public class CrmAdoDataSourceVersionComparer : DataSiteableObject<Microsoft.VisualStudio.Data.Services.IVsDataConnection>, IVsDataSourceVersionComparer, IComparable<string>
    {
        private bool _HasSourceVersion = false;

        public CrmAdoDataSourceVersionComparer()
            : this(null)
        {
        }

        public CrmAdoDataSourceVersionComparer(IVsDataConnection site)
            : this(site, new CrmAdoDataSourceVersionNumberComparer())
        {
        }

        public CrmAdoDataSourceVersionComparer(IVsDataConnection site, IComparer<string> comparer)
            : base(site)
        {
            Comparer = comparer;
        }

        protected IComparer<string> Comparer { get; set; }

        private string _SourceVersion = string.Empty;
        public string SourceVersion
        {
            get
            {
                if (!this._HasSourceVersion && base.Site != null)
                {
                    IVsDataSourceInformation service = base.Site.GetService(typeof(IVsDataSourceInformation)) as IVsDataSourceInformation;
                    if (service != null)
                    {
                        this._SourceVersion = service["DataSourceVersion"] as string;
                    }
                    this._HasSourceVersion = true;
                }
                return this._SourceVersion;

            }
            set
            {
                _SourceVersion = value;
            }
        }

        public int CompareTo(string other)
        {
            if (this.SourceVersion == null)
            {
                return 0;
            }
            var result = Comparer.Compare(this.SourceVersion, other);

            return result;
        }

    }

    public class CrmAdoConnectionProperties : AdoDotNetConnectionProperties
    {
        public override string ToDisplayString()
        {
            var result = base.ToDisplayString();
            return result;
        }

        public override string ToSafeString()
        {
            var result = base.ToSafeString();
            return result;
        }
    }


    public class CrmAdoDataConnectionEquivalencyComparer : DataConnectionEquivalencyComparer
    {
        protected override bool AreEquivalent(IVsDataConnectionProperties connectionProperties1, IVsDataConnectionProperties connectionProperties2)
        {
            var result = base.AreEquivalent(connectionProperties1, connectionProperties2);
            return result;
        }
    }

    public class CrmAdoDataObjectIdentifierResolver : DataObjectIdentifierResolver
    {
        public override object[] ContractIdentifier(string typeName, object[] fullIdentifier)
        {
            var result = base.ContractIdentifier(typeName, fullIdentifier);
            return result;
        }

        public override bool Equals(object obj)
        {
            var result = base.Equals(obj);
            return result;
        }

        /// <summary>
        /// SQL Server connections are always within the context of a current
        /// database and default schema.  This method expands identifiers
        /// that are missing database or schema parts by adding the defaults
        /// appropriately.
        /// </summary>
        public override object[] ExpandIdentifier(
            string typeName, object[] partialIdentifier)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }

            // Find the type in the data object support model
            IVsDataObjectType type = null;
            IVsDataObjectSupportModel objectSupportModel = Site.GetService(
                typeof(IVsDataObjectSupportModel)) as IVsDataObjectSupportModel;
            Debug.Assert(objectSupportModel != null);
            if (objectSupportModel != null &&
                objectSupportModel.Types.ContainsKey(typeName))
            {
                type = objectSupportModel.Types[typeName];
            }
            if (type == null)
            {
                throw new ArgumentException("Invalid type " + typeName + ".");
            }

            // Create an identifier array of the correct full length
            object[] identifier = new object[type.Identifier.Count];

            // If the input identifier is not null, copy it to the full
            // identifier array.  If the input identifier's length is less
            // than the full length we assume the more specific parts are
            // specified and thus copy into the rightmost portion of the
            // full identifier array.
            if (partialIdentifier != null)
            {
                if (partialIdentifier.Length > type.Identifier.Count)
                {
                    throw new ArgumentException("Invalid partial identifier.");
                }
                partialIdentifier.CopyTo(identifier,
                    type.Identifier.Count - partialIdentifier.Length);
            }

            // Get the data source information service
            IVsDataSourceInformation sourceInformation =
                Site.GetService(typeof(IVsDataSourceInformation))
                    as IVsDataSourceInformation;
            Debug.Assert(sourceInformation != null);
            if (sourceInformation == null)
            {
                // This should never occur
                return identifier;
            }

            // Now expand the identifier as required
            if (type.Identifier.Count > 0)
            {
                // Fill in the current database if not specified
                if (!(identifier[0] is string))
                {
                    identifier[0] = sourceInformation[
                        DataSourceInformation.DefaultCatalog] as string;
                }
            }
            if (type.Identifier.Count > 1)
            {
                // Fill in the default schema if not specified
                if (!(identifier[1] is string))
                {
                    identifier[1] = sourceInformation[
                        DataSourceInformation.DefaultSchema] as string;
                }
            }

            return identifier;
        }

        //public override object[] ExpandIdentifier(string typeName, object[] partialIdentifier)
        //{
        //    return partialIdentifier;
        //}
    }




}

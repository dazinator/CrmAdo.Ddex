using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using CrmAdo.DdexProvider;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Framework;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Shell;
using EnvDTE;

namespace CrmAdo.DdexProvider
{


    [Guid(GuidList.guidCrmAdo_DdexProviderObjectFactoryString)]
    class CrmAdoProviderObjectFactory : DataProviderObjectFactory
    {

        public Dictionary<Type, Func<object>> TypeMappings { get; set; }

        public CrmAdoProviderObjectFactory()
        {
            TypeMappings = new Dictionary<Type, Func<object>>();
            TypeMappings.Add(typeof(IVsDataConnectionProperties), () => { return CreateNewConnectionProperties(); });
            TypeMappings.Add(typeof(IVsDataConnectionUIProperties), () => { return CreateNewConnectionProperties(); });
            TypeMappings.Add(typeof(IVsDataConnectionSupport), () => { return CreateNewConnectionSupport(); });
            TypeMappings.Add(typeof(IDSRefBuilder), () => { return CreateNewDsRefBuilder(); });
            TypeMappings.Add(typeof(IVsDataObjectSupport), () => { return CreateNewDataObjectSupport(); });
            TypeMappings.Add(typeof(IVsDataViewSupport), () => { return CreateNewDataViewSupport(); });
            TypeMappings.Add(typeof(IVsDataObjectSelector), () => { return CreateNewDataObjectSelector(); });
            TypeMappings.Add(typeof(IVsDataSourceInformation), () => { return CreateNewDataSourceInformation(); });
            TypeMappings.Add(typeof(IVsDataObjectMemberComparer), () => { return CreateNewDataObjectMemberComparer(); });
            TypeMappings.Add(typeof(IVsDataMappedObjectConverter), () => { return CreateNewDataMappedObjectConverter(null); });
            TypeMappings.Add(typeof(IVsDataObjectIdentifierConverter), () => { return CreateNewDataObjectIdentifierConverter(); });
            TypeMappings.Add(typeof(IVsDataSourceVersionComparer), () => { return CreateNewDataSourceVersionComparer(null); });
            TypeMappings.Add(typeof(IVsDataConnectionEquivalencyComparer), () => { return CreateNewDataConnectionEquivalencyComparer(null); });
            TypeMappings.Add(typeof(IVsDataObjectIdentifierResolver), () => { return CreateNewDataObjectIdentifierResolver(null); });
        }



        public override object CreateObject(Type objType)
        {

            var hasMapping = TypeMappings.ContainsKey(objType);
            if (hasMapping)
            {
                var factory = TypeMappings[objType];
                if (factory != null)
                {
                    var instance = factory();
                    return instance;
                }
            }


            //else if (objType == typeof(IVsDataConnectionUIControl))
            ////   return new Microsoft.VisualStudio.Data.Framework.AdoDotNet.data();
            //{

            //}
            //else if (objType == typeof(IVsDataConnectionUIProperties))
            //{

            //}
            //else if (objType == typeof(IVsDataObjectMemberComparer))
            //{

            //}
            //else if (objType == typeof(IVsDataMappedObjectConverter))
            //{
            //    return new CrmAdoDataMappedObjectConverter();
            //}

            return null;

        }


        private object CreateNewDataObjectIdentifierConverter()
        {
            return new CrmAdoObjectIdentifierConverter();
        }

       

        private object CreateNewDataObjectSelector()
        {
            return new CrmObjectSelector();
        }

        private object CreateNewDsRefBuilder()
        {
            return new CrmAdoDsRefBuilder();
        }



        private object CreateNewConnectionSupport()
        {
            // ok
            var connSupport = new CrmAdoConnectionSupport();
            AddDataSourceVersionComparerService(connSupport);
            AddDataMappedObjectConverterService(connSupport);        
            return connSupport;
        }

        private void AddDataMappedObjectConverterService(CrmAdoConnectionSupport connSupport)
        {
            var serviceType = typeof(IVsDataMappedObjectConverter);
            var existingService = connSupport.GetService(serviceType);
            IVsDataConnection existingSite = null;

            if (existingService != null)
            {
                var existingSitable = (DataSiteableObject<IVsDataConnection>)existingService;
                existingSite = existingSitable.Site;
            }

            connSupport.RemoveService(serviceType);
            var newService = CreateNewDataMappedObjectConverter(existingSite);

            connSupport.SiteChanged += (o, e) =>
            {
                newService.Site = connSupport.Site;
            };

            connSupport.AddService(serviceType, newService);
        }

        private AdoDotNetMappedObjectConverter CreateNewDataMappedObjectConverter(IVsDataConnection site)
        {
           // if (site != null)
            return new AdoDotNetMappedObjectConverter(site);
        }

        private void AddDataSourceVersionComparerService(CrmAdoConnectionSupport connSupport)
        {
            var serviceType = typeof(IVsDataSourceVersionComparer);
            var existingService = connSupport.GetService(serviceType);
            IVsDataConnection existingSite = null;

            if (existingService != null)
            {
                var existingSitable = (DataSiteableObject<IVsDataConnection>)existingService;
                existingSite = existingSitable.Site;
            }

            connSupport.RemoveService(serviceType);
            var dsVersionComparer = CreateNewDataSourceVersionComparer(existingSite);

            connSupport.SiteChanged += (o, e) =>
            {
                dsVersionComparer.Site = connSupport.Site;
            };

            connSupport.AddService(serviceType, dsVersionComparer);
        }

        private object CreateNewDataObjectIdentifierResolver(object p)
        {
            // ok
            return new CrmAdoDataObjectIdentifierResolver();
        }

        private CrmAdoDataSourceVersionComparer CreateNewDataSourceVersionComparer(IVsDataConnection site)
        {
            // ok
            return new CrmAdoDataSourceVersionComparer(site);
        }

        private object CreateNewDataConnectionEquivalencyComparer(object p)
        {
            // ok
            return new CrmAdoDataConnectionEquivalencyComparer();
        }

        private object CreateNewDataObjectMemberComparer()
        {
            // ok
            return new CrmAdoDataObjectMemberComparer();
        }

        private object CreateNewDataSourceInformation()
        {
            // ok
            return new CrmSourceInformation();
        }

        private object CreateNewDataViewSupport()
        {
            // ok
            return new DataViewSupport(this.GetType().Namespace + ".CrmViewSupport", System.Reflection.Assembly.GetExecutingAssembly());
        }

        private object CreateNewDataObjectSupport()
        {
            // ok
            var dataObjectSupport = new DataObjectSupport(this.GetType().Namespace + ".CrmObjectSupport", System.Reflection.Assembly.GetExecutingAssembly());
            return dataObjectSupport;
        }

        private object CreateNewConnectionProperties()
        {
            // ok
            return new CrmAdoConnectionProperties();
        }


    }





}

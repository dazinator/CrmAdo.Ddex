using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.DdexProvider
{
   
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
            if (serviceType == typeof(IVsDataMappedObjectConverter))
            {
                return new CrmAdoDataMappedObjectConverter();
            }
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
}

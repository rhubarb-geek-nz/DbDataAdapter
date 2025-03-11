// Copyright (c) 2025 Roger Brown.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace RhubarbGeekNz.DbDataAdapter
{
    [Cmdlet(VerbsCommon.New, "DbDataAdapter")]
    [OutputType(typeof(System.Data.Common.DbDataAdapter))]
    sealed public class NewDbDataAdapter : PSCmdlet
    {
        private const string DataAdapterSuffix = "DataAdapter";

        [Parameter(ParameterSetName = "connection", Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, HelpMessage = "Database Statement Text")]
        public string? CommandText;

        [Parameter(ParameterSetName = "connection", Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = false, HelpMessage = "Database connection")]
        public DbConnection? Connection;

        [Parameter(ParameterSetName = "command", Mandatory = false, HelpMessage = "Database Command")]
        public DbCommand? Command;

        protected override void ProcessRecord()
        {
            Assembly? assembly = null;
            string? typeName = null;
            Type? dbaType = null;
            var parameters = new List<object>();

            if (Connection != null && CommandText != null)
            {
                Type type = Connection.GetType();
                assembly = type.Assembly;
                string name = type.FullName;
                parameters.Add(CommandText);
                parameters.Add(Connection);

                if (name.EndsWith("Connection"))
                {
                    typeName = name.Substring(0, name.Length - 10) + DataAdapterSuffix;
                }
            }
            else
            {
                if (Command != null)
                {
                    Type type = Command.GetType();
                    assembly = type.Assembly;
                    string name = type.FullName;
                    parameters.Add(Command);

                    if (name.EndsWith("Command"))
                    {
                        typeName = name.Substring(0, name.Length - 7) + DataAdapterSuffix;
                    }
                }
            }

            if (assembly != null)
            {
                if (typeName != null)
                {
                    dbaType = assembly.GetType(typeName);
                }

                if (dbaType == null)
                {
                    dbaType = assembly.GetTypes().Where(t => typeof(System.Data.Common.DbDataAdapter).IsAssignableFrom(t)).SingleOrDefault();
                }
            }

            if (dbaType == null)
            {
                dbaType = typeof(DefaultDataAdapter);
            }

            Type[] typeList = parameters.Select(p => p.GetType()).ToArray();
            var factory = dbaType.GetConstructor(typeList);
            WriteObject(factory.Invoke(parameters.ToArray()));
        }
    }

    sealed internal class DefaultDataAdapter : System.Data.Common.DbDataAdapter
    {
        private readonly bool disposeSelect = true;

        public DefaultDataAdapter()
        {
        }

        public DefaultDataAdapter(string commandText, DbConnection connection)
        {
            SelectCommand = connection.CreateCommand();
            SelectCommand.CommandText = commandText;
        }

        public DefaultDataAdapter(DbCommand command)
        {
            SelectCommand = command;
            disposeSelect = false;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (disposeSelect)
                    {
                        IDisposable? disposable = SelectCommand;
                        SelectCommand = null;
                        disposable?.Dispose();
                    }

                    {
                        IDisposable? disposable = DeleteCommand;
                        DeleteCommand = null;
                        disposable?.Dispose();
                    }

                    {
                        IDisposable? disposable = InsertCommand;
                        InsertCommand = null;
                        disposable?.Dispose();
                    }

                    {
                        IDisposable? disposable = UpdateCommand;
                        UpdateCommand = null;
                        disposable?.Dispose();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}

// Copyright (c) 2025 Roger Brown.
// Licensed under the MIT License.

using System;
using System.Data.Common;
using System.Management.Automation;

namespace RhubarbGeekNz.DbDataAdapter
{
    [Cmdlet(VerbsCommunications.Read, "DbCommand")]
    sealed public class ReadDbCommmand : PSCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "Database Command")]
        public DbCommand? Command;

        protected override void ProcessRecord()
        {
            try
            {
                if (Command == null)
                {
                    throw new PSArgumentNullException("Argument \"Command\" is null");
                }

                using (var reader = Command.ExecuteReader())
                {
                    int count = reader.FieldCount, unnamed = 0;
                    string[] fieldNames = new string[count];
                    for (int i = 0; i < count; i++)
                    {
                        string name = reader.GetName(i);

                        while (String.IsNullOrEmpty(name))
                        {
                            name = "Column" + (++unnamed);

                            for (int j = 0; j < count; j++)
                            {
                                string? column = reader.GetName(j);
                                if ((!String.IsNullOrEmpty(column)) && name.Equals(column, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    name = String.Empty;
                                    break;
                                }
                            }
                        }

                        fieldNames[i] = name;
                    }

                    while (reader.Read())
                    {
                        PSObject result = new PSObject();
                        for (int i = 0; i < count; i++)
                        {
                            object? value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            result.Members.Add(new PSNoteProperty(fieldNames[i], value));
                        }
                        WriteObject(result);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, ex.GetType().Name, ErrorCategory.ReadError, null));
            }
        }
    }
}

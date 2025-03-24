# rhubarb-geek-nz.DbDataAdapter
PowerShell cmdlet to create matching DbDataAdapter

Determines assembly for DbConnection or DbCommand and creates appropriate DbDataAdapter.

```
New-DbDataAdapter -CommandText <string> -Connection <DbConnection>

New-DbDataAdapter [-Command <DbCommand>]

Read-DbCommand -Command <DbCommand>
```

Works when the DbConnection is created using assemblies in a different AssemblyLoadContext.

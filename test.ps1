#!/usr/bin/env pwsh
# Copyright (c) 2025 Roger Brown.
# Licensed under the MIT License.

param(
	$NewDbConnection = 'New-SQLiteConnection',
	$ConnectionString = 'Data Source=:memory:;',
	$CommandText = 'SELECT sqlite_version() AS VERSION'
)

$ErrorActionPreference = "Stop"

trap
{
	throw $PSItem
}

$cmd = Get-Command -Name $NewDbConnection

if (-not $cmd)
{
	throw "$NewDbConnection not found"
}

if (1 -ne $cmd.Count)
{
	throw "$NewDbConnection is ambiguous"
}

if (-not ([System.Data.Common.DbConnection].IsAssignableFrom($cmd.OutputType.Type)))
{
	throw "OutputType for $NewDbConnection not assignable to DbConnection"
}

[System.Data.Common.DbConnection]$Connection = & $NewDbConnection -ConnectionString $ConnectionString

try
{
	$Connection.Open()

	$DataTable = New-Object -TypeName System.Data.DataTable

	$DataAdapter = New-DbDataAdapter -Connection $Connection -CommandText $CommandText

	if ($DataAdapter.Fill($DataTable))
	{
		$DataTable | Format-Table
	}

	$Command = $Connection.CreateCommand()

	$Command.CommandText = $CommandText

	Read-DbCommand -Command $Command | Format-Table
}
finally
{
	$Connection.Dispose()
}

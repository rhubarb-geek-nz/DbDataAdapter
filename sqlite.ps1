#!/usr/bin/env pwsh
# Copyright (c) 2025 Roger Brown.
# Licensed under the MIT License.

param(
	$ConnectionString = 'Data Source=:memory:;',
	$CommandText = 'SELECT sqlite_version() AS VERSION'
)

$ErrorActionPreference = "Stop"

trap
{
	throw $PSItem
}

$Connection = New-SQLiteConnection -ConnectionString $ConnectionString

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

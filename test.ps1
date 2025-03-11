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

	$DataSet = New-Object -TypeName System.Data.DataSet

	$DataAdapter = New-DbDataAdapter -Connection $Connection -CommandText $CommandText

	if ($DataAdapter.Fill($DataSet))
	{
		$DataSet.Tables
	}
}
finally
{
	$Connection.Dispose()
}

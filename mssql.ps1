#!/usr/bin/env pwsh
# Copyright (c) 2025 Roger Brown.
# Licensed under the MIT License.

param(
	$ConnectionString = 'Data Source=localhost;Integrated Security=False;Persist Security Info=False;User ID=sa;Password=changeit;TrustServerCertificate=True',
	$CommandText = 'SELECT @@VERSION AS VERSION'
)

$ErrorActionPreference = "Stop"

trap
{
	throw $PSItem
}

$Connection = New-SqlConnection -ConnectionString $ConnectionString

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

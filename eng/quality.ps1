param([switch]$NoRestore)
$ErrorActionPreference = 'Stop'
Push-Location (Join-Path $PSScriptRoot '..')
function Invoke-Dotnet([string[]]$CommandArguments) {
    & dotnet @CommandArguments
    if ($LASTEXITCODE -ne 0) { throw "Quality gate failed with exit code $LASTEXITCODE" }
}
try {
    if (-not $NoRestore) { Invoke-Dotnet -CommandArguments @('restore', '--locked-mode') }
    Invoke-Dotnet -CommandArguments @('format', '--verify-no-changes', '--no-restore')
    Invoke-Dotnet -CommandArguments @('build', '-c', 'Release', '--no-restore')
    Invoke-Dotnet -CommandArguments @('test', '-c', 'Release', '--no-build', '--no-restore',
        '--logger', 'trx', '--results-directory', 'artifacts/test-results',
        '/p:CollectCoverage=true', '/p:CoverletOutput=./coverage/',
        '/p:CoverletOutputFormat=cobertura', '/p:Include=[WarehouseStock.Core]*',
        '/p:ExcludeByAttribute=GeneratedCodeAttribute%2cCompilerGeneratedAttribute',
        '/p:Threshold=80', '/p:ThresholdType=line')
}
finally { Pop-Location }

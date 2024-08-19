$apiKey = "$($env:FdxNugetApiKey)"
$nugetLocalFolder = "..\build-intermediate\nuget"
$localNuget = "gitea"

$solutionName = "..\ExpressionParser.sln"

if (-Not ($apiKey -eq "")) {
	 if (-Not (Test-Path $nugetLocalFolder)) {
         dotnet pack $solutionName -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
     }
     
     $files = Get-ChildItem $nugetLocalFolder -Filter *.*nupkg
     
     foreach ($f in $files){
         $outfile = $f.FullName
         Write-Output "pushing: $f.Name"
         dotnet nuget push --source $localNuget $outfile --api-key $apiKey --skip-duplicate
         Remove-Item -Path $f.FullName
     }

     Remove-Item -Path $nugetLocalFolder
} else {
    Write-Output "Nuget api key not set. Setup FdxNugetApiKey environment variable correctly and run again"
}
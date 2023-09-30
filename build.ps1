param(
	[parameter()]
	[alias("sc")]
	[bool] $selfcontained = $false,

	[parameter()]
	[alias()]
	[string] $logname = "build_log_" + (Get-Date -Format "yyyyMMdd_HHmmss"),
	
	[parameter()]
	[alias()]
	[bool] $showlog = $true
)

$logpath = "$logname.log"

$(

	dotnet build "..\WpfUtilities\WpfUtilities.csproj"
	dotnet build "TinyTotal.sln" '--self-contained' $selfcontained '--no-dependencies'

) | Out-File -FilePath $logpath -Encoding utf8

if ($showlog) {
    Start "$logpath"
}


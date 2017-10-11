$keyfile = "D:\Dropbox\nuget-access-key.txt"
$scriptpath = split-path -parent $MyInvocation.MyCommand.Path
$nugetpath = resolve-path "$scriptpath/../.nuget/nuget.exe"
$packagespath = resolve-path "$scriptpath/../.build"
$pathToDll = resolve-path "$scriptpath/../src/Moq.EntityFrameworkCore/bin/Release/netstandard2.0/Moq.EntityFrameworkCore.dll"

write-host $keyfile

if(-not (test-path $keyfile)) {
  throw "You cannot publish package"
}
else {
  # get our secret key. This is not in the repository.
  $key = get-content $keyfile
  
  pushd $packagespath

  # Find all the packages and display them for confirmation
  $packages = dir "*.nuspec"
  write-host "Packages to process:"
  $packages | % { write-host $_.Name }

  # Ensure we haven't run this by accident.
  $yes = New-Object System.Management.Automation.Host.ChoiceDescription "&Yes", "Uploads the packages."
  $no = New-Object System.Management.Automation.Host.ChoiceDescription "&No", "Does not upload the packages."
  $options = [System.Management.Automation.Host.ChoiceDescription[]]($no, $yes)
  
  $result = $host.ui.PromptForChoice("Upload packages", "Do you want to upload the NuGet packages to the NuGet server?", $options, 0) 
  
  # Cancelled
  if($result -eq 0) {
    "Upload aborted"
  }

  # Generation and upload
  elseif($result -eq 1) {
    # Get dll Version
    $AssemblyVersion = (Get-Command "$pathToDll").Version

		# Generating nuget packages
    $packages | % { 
			write-host "Generating $_"
      [xml] $xml = Get-Content "$_"
      $xml.Package.Metadata.Version = "$AssemblyVersion"
      $xml.save("$_")
			& $nugetpath pack $package
			write-host ""
		}
		
    # Uploading package to nuget
		$packages = dir "*.nupkg"
		$packages | % { 
			write-host ("Uploading "+ $_)
			& $nugetpath push $_ $key -Source "https://www.nuget.org/api/v2/package"
			write-host ""
		}
  }
  popd
}
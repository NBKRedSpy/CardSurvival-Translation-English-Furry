# Creates the release's .zip file

$ModName = "Guil-天降福瑞";
$ModFolder = "./Package/" + $ModName
$ArchiveName = "Guil-Furry.zip"

Remove-Item -ErrorAction SilentlyContinue -Recurse ./Package/*
Remove-Item -ErrorAction SilentlyContinue $ArchiveName
mkdir -ErrorAction SilentlyContinue $ModFolder

dotnet publish .\src\天降福瑞.csproj -o $ModFolder -c Release

Copy-Item -Recurse ./Guil-天降福瑞/* $ModFolder

# English name since github strips Unicode for security purposes.
Compress-Archive -DestinationPath $ArchiveName -Path ./Package/*


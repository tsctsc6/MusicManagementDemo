# 在解决方案目录中：

$output_dir = "";
$runtime = "win-x64";

Remove-Item $output_dir -Recurse -Force

dotnet publish MusicManagementDemo.WebApi/MusicManagementDemo.WebApi.csproj --runtime $runtime --configuration Release --output $output_dir

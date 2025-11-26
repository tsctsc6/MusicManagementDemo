# 设置 PostgreSQL 连接参数（替换为你的实际值）
$pgUser = "postgres"             # PostgreSQL 用户名
$pgPassword = Read-Host "PostgreSQL 密码" -AsSecureString    # PostgreSQL 密码
$pgHost = "localhost"            # 主机（默认为 localhost）
$pgPort = "5432"                 # 端口（默认为 5432）
$prefix = "functional_testing_"  # 数据库前缀

# 设置环境变量以避免密码提示
$env:PGPASSWORD = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto([System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($pgPassword))
$env:PGUSER = $pgUser
$env:PGHOST = $pgHost
$env:PGPORT = $pgPort

$queryString = @"
SELECT row_to_json(t) FROM (
SELECT datname FROM pg_database
WHERE
datname LIKE '$prefix%' AND
datname NOT IN ('postgres', 'template0', 'template1')
) t;
"@

# 获取匹配前缀的数据库列表（连接到系统数据库 'postgres'）
$dbs = & psql -d postgres -Atc $queryString

# 如果没有匹配的数据库，输出消息并退出
if (-not $dbs) {
    Write-Host "没有找到以 '$prefix' 开头的数据库。"
    Pause
    exit
}

$dbs = $dbs.Split("`r`n");

# 遍历每个匹配的数据库
foreach ($db in $dbs) {
    $db = $db | ConvertFrom-Json
    Write-Host "处理数据库: $($db.datname)"

    $disconnectString = @"
SELECT pg_terminate_backend(pid)
FROM pg_stat_activity WHERE datname = '$($db.datname)'
AND pid <> pg_backend_pid();
"@

    # 终止数据库的所有现有连接（除了当前会话）
    & psql -d postgres -c $disconnectString > $null

    # 删除数据库
    & dropdb --if-exists $($db.datname)
    if ($LASTEXITCODE -eq 0) {
        Write-Host "成功删除数据库: $($db.datname)"
    }
    else {
        Write-Host "删除数据库失败: $($db.datname)"
    }
}

# 清理环境变量（可选）
Remove-Item Env:PGPASSWORD
Remove-Item Env:PGUSER
Remove-Item Env:PGHOST
Remove-Item Env:PGPORT

Write-Host "操作完成。"
Pause
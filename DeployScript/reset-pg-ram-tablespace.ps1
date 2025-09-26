$pgUser = "postgres"
$pgPassword = Read-Host "PostgreSQL 密码" -AsSecureString
$pgHost = "localhost"
$pgPort = "5432"

$env:PGPASSWORD = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto([System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($pgPassword))
$env:PGUSER = $pgUser
$env:PGHOST = $pgHost
$env:PGPORT = $pgPort

psql -d postgres -c "DROP TABLESPACE ram_tablespace;"
psql -d postgres -c "CREATE TABLESPACE ram_tablespace LOCATION 'R:/pg_ram_db';"

Remove-Item Env:PGPASSWORD
Remove-Item Env:PGUSER
Remove-Item Env:PGHOST
Remove-Item Env:PGPORT

Pause
psql -h 127.0.0.1 -p 5432 -U postgres -c "DROP TABLESPACE ram_tablespace;"
psql -h 127.0.0.1 -p 5432 -U postgres -c "CREATE TABLESPACE ram_tablespace LOCATION 'R:/pg_ram_db';"
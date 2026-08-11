#!/bin/sh
set -e
# Add your custom logic here (e.g., waiting for database)

echo "Running EF Core migrations..."
CONNECTION_STRING="HOST=postgres; User ID=majid;Password=Alm1ghty1;Port=5432;Database=LMSAccount;Pooling=true;"
./efbundle --connection "$CONNECTION_STRING"
echo "Starting application..."
exec dotnet "BankingApp.LoanManagement.dll"
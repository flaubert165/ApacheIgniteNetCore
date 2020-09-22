# download oracle 11g xe image
docker pull epiclabs/docker-oracle-xe-11g

# create clear docker network
docker network create --subnet=172.255.0.0/16 clear;

# up docker compose
Set-Location .\compose
docker-compose.exe up -d
Set-Location ..\

# copy clear oracle dump
docker cp .\oracle-dump\dumpclear.dmp clear-oraclexe:/u01/app/oracle/admin/XE/dpdump/dumpclear.dmp
# copy sinacor oracle dump
docker cp .\oracle-dump\sinacormetadata.dmp clear-oraclexe:/u01/app/oracle/admin/XE/dpdump/sinacormetadata.dmp

# copy oracle config file
docker cp .\compose\oracleconfig.sh clear-oraclexe:/oracleconfig.sh

# import clear database
docker exec clear-oraclexe chmod 777 oracleconfig.sh
# docker exec clear-oraclexe "./oracleconfig.sh"

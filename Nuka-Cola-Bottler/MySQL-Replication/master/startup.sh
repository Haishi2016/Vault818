echo "CREATE USER '$MYSQL_REPLICA_USER'@'%' IDENTIFIED BY '$MYSQL_REPLICA_PASS'; " | "${mysql[@]}"
echo "GRANT REPLICATION SLAVE ON *.* TO '$MYSQL_REPLICA_USER'@'%'; " | "${mysql[@]}"
echo "GRANT REPLICATION CLIENT ON *.* TO '$MYSQL_REPLICA_USER'@'%'; " | "${mysql[@]}"
echo "FLUSH PRIVILEGES; " | "${mysql[@]}"

if [ "$CREATE_PET_DB" ]; then
	echo "CREATE DATABASE pet_db; " | "${mysql[@]}"
	echo "USE pet_db; CREATE TABLE pet (name VARCHAR(20), owner VARCHAR(20), species VARCHAR(20), sex CHAR(1), birth DATE, death DATE); " | "${mysql[@]}"
fi

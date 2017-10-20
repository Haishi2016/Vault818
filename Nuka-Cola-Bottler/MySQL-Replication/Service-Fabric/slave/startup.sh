if [ "$CREATE_PET_DB" ]; then
        echo "CREATE DATABASE pet_db; " | "${mysql[@]}"
        echo "USE pet_db; CREATE TABLE pet (name VARCHAR(20), owner VARCHAR(20), species VARCHAR(20), sex CHAR(1), birth DATE, death DATE); " | "${mysql[@]}"
fi

for i in $(seq $((MYSQL_MASTER_WAIT_TIME + 1))); do
	if ! mysql "-u$MYSQL_REPLICA_USER" "-p$MYSQL_REPLICA_PASS" "-h$MYSQL_MASTER" -e 'SELECT 1;' | grep -q 1; then
		echo >&2 "Now waiting for $MYSQL_REPLICA_USER@$MYSQL_MASTER"
		sleep 1
	else
		break
	fi
done

MasterPosition=$(mysql "-u$MYSQL_REPLICA_USER" "-p$MYSQL_REPLICA_PASS" "-h$MYSQL_MASTER" -e "SHOW MASTER STATUS \G" | awk '/Position/ {print $2}')
MasterFile=$(mysql "-u$MYSQL_REPLICA_USER" "-p$MYSQL_REPLICA_PASS" "-h$MYSQL_MASTER" -e "SHOW MASTER STATUS \G" | awk '/File/ {print $2}')

echo >&2 "Master File: $MasterFile"
echo >&2 "Master Position: $MasterPosition"

echo "CHANGE MASTER TO MASTER_HOST='$MYSQL_MASTER', MASTER_PORT=$MYSQL_MASTER_PORT, MASTER_USER='$MYSQL_REPLICA_USER', MASTER_PASSWORD='$MYSQL_REPLICA_PASS', MASTER_LOG_FILE='$MasterFile', MASTER_LOG_POS=$MasterPosition;"  | "${mysql[@]}"
echo "START SLAVE;"  | "${mysql[@]}"


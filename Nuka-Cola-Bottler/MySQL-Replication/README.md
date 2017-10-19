# MySQL Master-Slave Replication

1. Launch the compose app:
```bash
cd ~/Vault818/Nuka-Cola-Bottler/MySQL-Replication/compose
docker-compose up
```
2. Connect to the master
```
sudo docker run -it --link compose_master_1 --rm --network compose_default mysql sh -c 'exec mysql -hcompose_master_1 -P3306 -uroot -pmaster_passw0rd'
```
3. In a separate terminal window, connect to the slave
```
sudo docker run -it --link compose_slave_1 --rm --network compose_default mysql sh -c 'exec mysql -hcompose_slave_1 -uroot -pslave_passw0rd'
```
4. Insert a new record to master
```sql
USE pet_db;
INSERT INTO pet VALUES('Puffball', 'Diane','hamster', 'f', '1999-03-30', NULL);
```

5. Observe data is replicated in slave
```sql
USE pet_db;
SELECT * FROM pet;
```
Above query returns
```sql
+----------+-------+---------+------+------------+-------+
| name     | owner | species | sex  | birth      | death |
+----------+-------+---------+------+------------+-------+
| Puffball | Diane | hamster | f    | 1999-03-30 | NULL  |
+----------+-------+---------+------+------------+-------+
1 row in set (0.00 sec)

```

This work is inspired by [this repository](https://github.com/twang2218/mysql-replica).
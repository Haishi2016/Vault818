# MySQL Master-Slave Replication

## Docker Compose

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

## Service Fabric

1. Select cluster to use:

```bash
sfctl cluster select --endpoint http://<your cluster>.westus.cloudapp.azure.com:19080
```
2. Deploy the app:
```bash
cd ~/Vault818/Nuka-Cola-Bottler/MySQL-Replication/Service-Fabric/compose
sfctl compose create --name MySqlHA --file-path ./docker-compose.yml
```
3. SSH into one of the node on cluster.
4. Connect to master:

```
sudo docker run -it --rm mysql sh -c 'exec mysql -h<ip to your master container - look it up on SF Explorer> -P3306 -uroot -pmaster_passw0rd'
```
5. Insert record
6. Connect to client

```
sudo docker run -it --rm mysql sh -c 'exec mysql -h<ip to your slave container - look it up on SF Explorer> -P3307 -uroot -pslave_passw0rd'
```

7. Select record

> **NOTE** This assumes you have DNS service enabled on the cluster. If you don't, you can either add the DNS service or build a new slave image using the slave\startup-without-dns.sh script. Then, in your compose file, you specify a list of possible master IP addresses:

```bash
MYSQL-MASTER: 10.0.0.4-10.0.0.5-10.0.0.6-10.0.0.7-10.0.0.8
```

8. Clean up

```bash
sfctl compose remove --deployment-name MySqlHA
```

This work is inspired by [this repository](https://github.com/twang2218/mysql-replica).
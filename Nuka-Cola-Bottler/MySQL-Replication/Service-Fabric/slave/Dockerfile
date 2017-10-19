FROM mysql:5.7
COPY startup.sh /docker-entrypoint-initdb.d/
CMD ["mysqld", "--log-bin=mysql-bin", "--server-id=2"]

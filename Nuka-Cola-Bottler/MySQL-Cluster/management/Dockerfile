FROM mysql/mysql-cluster

RUN mkdir /usr/mysql-cluster

COPY ./mysql-cluster.cnf /etc/mysql-cluster.cnf
COPY ./my.cnf /etc/my.cnf
COPY ./startup.sh .
COPY ./config.ini /usr/mysql-cluster/config.ini

ENTRYPOINT ["/bin/bash", "./startup.sh"]


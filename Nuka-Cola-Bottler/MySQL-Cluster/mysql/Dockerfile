FROM mysql/mysql-cluster

COPY ./my.cnf /etc/my.cnf
COPY ./startup.sh /

ENTRYPOINT ["/bin/bash", "/startup.sh"]


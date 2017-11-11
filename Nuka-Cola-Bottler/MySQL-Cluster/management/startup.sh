#!/bin/bash

#workaround to set hostname
sed -i 's/localhost.localdomain/management/' /etc/sysconfig/network
HH=`cat /etc/hostname`
tail -1 /etc/hosts | sed -e "s/${HH}/management/" >> /etc/hosts

NDB1=""
NDB2=""
MYSQL=""

while [[ "$NDB1" == "" ]] || [[ "$NDB2" == "" ]] || [[ "$MYSQL" == "" ]]
do
  NDB1=`curl -sL http://gatekeeper:8180/ndb1`
  NDB2=`curl -sL http://gatekeeper:8180/ndb2`
  MYSQL=`curl -sL http://gatekeeper:8180/mysql`
  if [ "$NDB1" == "Bad Request" ]; then NDB1=""; fi
  if [ "$DNB2" == "Bad Request" ]; then NDB2=""; fi
  if [ "$MYSQL" == "Bad Request" ]; then MYSQL=""; fi
  sleep 1
done


ndb_mgmd -f /usr/mysql-cluster/config.ini

curl -sX PUT http://gatekeeper:8180/management/READY

while true; do sleep 1000; done

#!/bin/bash

echo "Updating Host Name"

#workaround to set hostname
sed -i "s/localhost.localdomain/mysql/" /etc/sysconfig/network
HH=`cat /etc/hostname`
tail -1 /etc/hosts | sed -e "s/${HH}/mysql/" >> /etc/hosts

echo "Sending Launched Signal"

curl -sX PUT http://gatekeeper:8180/mysql/LAUNCHED

echo "Start Waiting for Containers"

MANAGEMENT=""
NDB1=""
NDB2=""

while [[ "$MANAGEMENT" != "READY" ]]  || [[ "$NDB1" != "READY" ]] || [[ "$NDB2" != "READY" ]]
do
  MANAGEMENT=`curl -sL http://gatekeeper:8180/management`
  NDB1=`curl -sL http://gatekeeper:8180/ndb1`
  NDB2=`curl -sL http://gatekeeper:8180/ndb2`
  sleep 1
  echo "Waiting: management:$MANAGEMENT; ndb1:$NDB1; ndb2:$NDB2"
done

/entrypoint.sh mysqld
mysqld

echo "Sending Ready Signal"

curl -sX PUT http://gatekeeper:8180/mysql/READY

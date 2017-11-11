#!/bin/bash

#workaround to set hostname
sed -i "s/localhost.localdomain/mysql/" /etc/sysconfig/network
HH=`cat /etc/hostname`
tail -1 /etc/hosts | sed -e "s/${HH}/mysql/" >> /etc/hosts

curl -sX PUT http://gatekeeper:8180/mysql/LAUNCHED

MANAGEMENT=""
NDB1=""
NDB2=""

while [[ "$MANAGEMENT" != "READY" ]]  || [[ "$NDB1" != "READY" ]] || [[ "$NDB2" != "READY" ]]
do
  MANAGEMENT=`curl -sL http://gatekeeper:8180/management`
  NDB1=`curl -sL http://gatekeeper:8180/ndb1`
  NDB2=`curl -sL http://gatekeeper:8180/hdb2`
  sleep 1
done

mysqld

curl -sX PUT http://gatekeeper:8180/mysql/READY

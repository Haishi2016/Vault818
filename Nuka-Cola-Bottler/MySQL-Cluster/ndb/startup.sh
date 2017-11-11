#!/bin/bash

#workaround to set hostname
sed -i "s/localhost.localdomain/ndb${NDBID}/" /etc/sysconfig/network
HH=`cat /etc/hostname`
tail -1 /etc/hosts | sed -e "s/${HH}/ndb${NDBID}/" >> /etc/hosts

curl -sX PUT http://gatekeeper:8180/ndb$NDBID/LAUNCHED

MANAGEMENT=""

while [[ "$MANAGEMENT" != "READY" ]] 
do
  MANAGEMENT=`curl -sL http://gatekeeper:8180/management`
  sleep 1
done

ndbd

curl -sX PUT http://gatekeeper:8180/ndb$NDBID/READY

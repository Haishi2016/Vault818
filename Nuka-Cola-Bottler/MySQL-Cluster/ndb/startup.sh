#!/bin/bash

echo "Updating Host Name"

#workaround to set hostname
sed -i "s/localhost.localdomain/ndb${NDBID}/" /etc/sysconfig/network
HH=`cat /etc/hostname`
tail -1 /etc/hosts | sed -e "s/${HH}/ndb${NDBID}/" >> /etc/hosts

echo "Sending Launched Signal"

curl -sX PUT http://gatekeeper:8180/ndb${NDBID}/LAUNCHED

MANAGEMENT=""

while [[ "$MANAGEMENT" != "READY" ]] 
do
  MANAGEMENT=`curl -sL http://gatekeeper:8180/management`
  sleep 1
  echo "Waiting management:$MANAGEMENT"
done

ndbd

echo "Sending Ready Signal"

curl -sX PUT http://gatekeeper:8180/ndb${NDBID}/READY

while true; do sleep 1000; done


#!/bin/bash

port1="8006"
port2="8007"
url="http://192.168.224.1"

function onKill {    
    response=$(curl -X GET $url:$port1/get_blockchain); echo $response | json_pp -json_opt pretty,canonical
    balance1=$(curl -s -X GET $url:$port1/check_balance)
    balance2=$(curl -s -X GET $url:$port2/check_balance)
    echo "Balance of wallet $(curl -s -X GET $url:$port1/public_key): $(echo $balance1) INF"
    echo "Balance of wallet $(curl -s -X GET $url:$port2/public_key): $(echo $balance2) INF"
    docker rm $(docker stop $(docker ps -q)) > /dev/null
    tput cnorm
}

trap onKill EXIT

tput civis

cd ~/source/repos/infinitum

docker compose up -d

sleep 5 &
PID=$!
i=1
sp="/-\|"
echo -n ' '
while [ -d /proc/$PID ]
do
  printf "\b${sp:i++%${#sp}:1}"
  sleep 0.1
done

echo -en "\r"

curl -X POST -H "Content-Type: application/json" -d "{\"address\": \"$url:$port1\", \"amount\": \"42\"}" $url:$port2/send_transaction/

sleep 1

curl -X POST -H "Content-Type: application/json" -d "{\"address\": \"$url:$port2\", \"amount\": \"3.14159265359\"}" $url:$port1/send_transaction/

sleep 1

while : 
do
    curl -X POST -H "Content-Type: application/json" -d "{\"address\": \"$url:$port1\", \"amount\": \"13.37\"}" $url:$port2/send_transaction/
    sleep 1
    curl -X POST -H "Content-Type: application/json" -d "{\"address\": \"$url:$port2\", \"amount\": \"13.37\"}" $url:$port1/send_transaction/
    sleep 1
done

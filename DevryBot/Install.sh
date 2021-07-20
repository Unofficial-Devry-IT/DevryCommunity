#!/bin/bash

VOLUME_NAME=$1
VOLUME_DATA=/var/lib/docker/volumes/$VOLUME_NAME/_data/

############################################################
# T E A R   D O W N    D E V R Y    C O N T A I N E R      #
############################################################
echo "Obtaining Devry Docker Container Id..."
DEVRY_CONTAINER_ID=$(docker ps --filter=name=devry -aq)
echo "Stopping Container: $DEVRY_CONTAINER_ID"
docker stop $DEVRY_CONTAINER_ID

############################################################
# G E T    L A T E S T   D O C K E R   I M A G E           #
############################################################
echo "Getting the latest bot image..."
docker pull mercenary9312/unofficial-devry-service-bot

############################################################
# S T A R T   T E M P O R A R Y    I M A G E 
############################################################
echo "Starting temporary container..."
docker run --rm -d --name devry-service-bot mercenary9312/unofficial-devry-service-bot:latest

############################################################
# C R E A T I N G   T E M P   D I R 
############################################################
echo "Creating temporary directory for container contents..."
CURRENT=pwd
TEMP_DIR=$CURRENT/temp_container_contents
mkdir -p $TEMP_DIR
chmod 777 $TEMP_DIR

############################################################
# C O P Y I N G    C O N T A I N E R   C O N T E N T S
############################################################
DEVRY_CONTAINER_ID=$(docker ps --filter=name=devry -aq)
docker cp $DEVRY_CONTAINER_ID:/app/Data $TEMP_DIR



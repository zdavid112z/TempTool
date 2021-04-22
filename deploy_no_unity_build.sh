#!/bin/bash

######################
# Must place the Unity build in build/WebGL/WebGL e.g. build/WebGL/WebGL/index.html must exist
######################

# Needs docker and kubectl installed and set up
# Docker: https://docs.docker.com/get-docker/
# Google SDK: https://cloud.google.com/sdk/docs/install#linux
# Link Docker to GCloud: https://cloud.google.com/container-registry/docs/advanced-authentication
# TLDR: sudo usermod -a -G docker ${USER} && gcloud auth login && gcloud auth configure-docker
# Kubernetes: https://kubernetes.io/docs/tasks/tools/included/install-kubectl-gcloud/
# Link Kubernetes to GCloud: https://cloud.google.com/kubernetes-engine/docs/quickstart
# TLDR: gcloud config set project temptool && gcloud config set compute/zone us-central1-c && gcloud config set compute/region us-central1 && gcloud container clusters get-credentials cluster-1

# Setup
PROJECT_ID=temptool
IMAGE=temptool
DEPLOYMENT_NAME=temptool
TAG=$(uuidgen)

# Save kustomization.yaml
cp kustomization.yaml __kustomization.yaml

# Build and publish docker image
echo "#### Building docker ####"
docker build --tag "gcr.io/$PROJECT_ID/$IMAGE:$TAG" .
docker push "gcr.io/$PROJECT_ID/$IMAGE:$TAG"

# Setup kustomize
curl -sfLo kustomize https://github.com/kubernetes-sigs/kustomize/releases/download/v3.1.0/kustomize_3.1.0_linux_amd64
chmod u+x ./kustomize

# Deploy
echo "#### Deploying to kubernetes ####"
./kustomize edit set image gcr.io/temptool/temptool=gcr.io/$PROJECT_ID/$IMAGE:$TAG
./kustomize build . | kubectl apply -f -
kubectl rollout status deployment/$DEPLOYMENT_NAME
kubectl get services -o wide

# Cleanup
rm kustomize kustomization.yaml
mv __kustomization.yaml kustomization.yaml

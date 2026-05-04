# Kubernetes Deployment

This directory contains Kubernetes manifests for deploying the microservices solution on a Kubernetes cluster. The configuration includes deployments, services, and horizontal pod autoscaling for all microservices.

## Overview

The Kubernetes deployment provides:
- **Multi-replica deployments** for high availability (2 replicas per service)
- **Service discovery** via Kubernetes services (ClusterIP and NodePort)
- **Horizontal Pod Autoscaling (HPA)** for dynamic scaling based on CPU utilization
- **Resource management** with requests and limits for each container
- **Isolated namespace** (`microservices`) for all resources

## Architecture

```
┌────────────────────────────────────────────────────────────┐
│                   Kubernetes Cluster                       │
│  ┌──────────────────────────────────────────────────────┐  │
│  │           Namespace: microservices                   │  │
│  │                                                      │  │
│  │  ┌──────────────────────────────────────────────┐   │  │
│  │  │  API Gateway (NodePort: 30007)               │   │  │
│  │  │  ├─ Deployment (replicas: 2)                 │   │  │
│  │  │  └─ Service (type: NodePort)                 │   │  │
│  │  └──────────────────────────────────────────────┘   │  │
│  │         │        │        │                          │  │
│  │    ┌────▼────┬───▼────┬───▼────┐                     │  │
│  │    │          │        │        │                     │  │
│  │  ┌─▼───────┐┌─▼──────┐┌─▼──────┐│                     │  │
│  │  │ User    ││Product ││ Order  │││                     │  │
│  │  │Service  ││Service ││Service │││                     │  │
│  │  │(×2)     ││(×2)    ││(×2)    │││                     │  │
│  │  └─────────┘└────────┘└────────┘│                     │  │
│  │  ┌─────────────────────────────┐│                     │  │
│  │  │   HPA: Auto-scale CPU       ││                     │  │
│  │  │   Min: 2, Max: 6 replicas   ││                     │  │
│  │  └─────────────────────────────┘│                     │  │
│  │                                  │                     │  │
│  └──────────────────────────────────┘                     │  │
│                                                           │  │
└────────────────────────────────────────────────────────────┘
```

## Files

### microservices.yaml
Complete Kubernetes manifest containing:

#### Namespace
- Creates `microservices` namespace for resource isolation

#### Deployments (2 replicas each)
1. **User Service**
   - Image: `rajesakhare/userservice:latest`
   - Port: 8080 (internal)
   - Resources: 100m CPU (request), 500m CPU (limit), 128Mi memory (request), 256Mi memory (limit)

2. **Product Service**
   - Image: `rajesakhare/productservice:latest`
   - Port: 8080 (internal)
   - Same resource configuration as User Service
   - Note: Database connectivity requires separate PostgreSQL setup

3. **Order Service**
   - Image: `rajesakhare/orderservice:latest`
   - Port: 8080 (internal)
   - Same resource configuration as User Service

4. **API Gateway**
   - Image: `rajesakhare/apigateway:latest`
   - Port: 8080 (internal)
   - Same resource configuration as User Service

#### Services
- **userservice**: ClusterIP (internal service discovery)
- **productservice**: ClusterIP (internal service discovery)
- **orderservice**: ClusterIP (internal service discovery)
- **apigateway**: NodePort (external access on :30007)

#### Horizontal Pod Autoscaler (HPA)
- **Target**: User Service Deployment
- **Min replicas**: 2
- **Max replicas**: 6
- **Metric**: CPU utilization at 50% threshold

## Prerequisites

- **Kubernetes Cluster** (v1.19+)
- **kubectl command-line tool** (installed and configured)
- **Docker images** pushed to registry:
  - `rajesakhare/userservice:latest`
  - `rajesakhare/productservice:latest`
  - `rajesakhare/orderservice:latest`
  - `rajesakhare/apigateway:latest`

## Local Development with Minikube

### Start Minikube
```bash
minikube start
```

### Enable Ingress (Optional)
```bash
minikube addons enable ingress
```

### Build Images Locally (Alternative to Registry)
If using local Docker images instead of registry:
```bash
# Set Docker context to Minikube
eval $(minikube docker-env)

# Build images from project root
cd ..
docker-compose build

# Reset Docker context when done
eval $(minikube docker-env -u)
```

If building locally, update image names in `microservices.yaml` to match your built images and set `imagePullPolicy: Never` for all containers.

## Deployment

### Prerequisites

- Kubernetes cluster (Minikube, AKS, EKS, GKE, etc.)
- kubectl configured to access your cluster
- Docker images pushed to registry (see Docker section)

### Deploy All Services

**Option 1: Complete deployment with PostgreSQL (Recommended)**

```bash
# Deploy everything including PostgreSQL
kubectl apply -f microservices.yaml

# Wait for all pods to be ready
kubectl wait --for=condition=ready pod --all -n microservices --timeout=300s
```

**Option 2: Deploy PostgreSQL separately, then services**

```bash
# Deploy PostgreSQL first
kubectl apply -f postgresql.yaml

# Wait for PostgreSQL to be ready
kubectl wait --for=condition=ready pod -l app=postgresql -n microservices --timeout=120s

# Then deploy the microservices
kubectl apply -f microservices.yaml
```

### Verify Deployment
```bash
# Check namespace
kubectl get namespace microservices

# Check all resources in namespace
kubectl get all -n microservices

# Check deployments
kubectl get deployments -n microservices

# Check pods
kubectl get pods -n microservices

# Check services
kubectl get services -n microservices

# Check HPA status
kubectl get hpa -n microservices
```

### Detailed Status
```bash
# Describe a specific deployment
kubectl describe deployment apigateway -n microservices

# Describe a specific pod
kubectl describe pod <pod-name> -n microservices

# Check pod logs
kubectl logs <pod-name> -n microservices

# Stream pod logs
kubectl logs -f <pod-name> -n microservices
```

## Accessing Services

### API Gateway (External Access)

**Local Development (Minikube)**:
```bash
# Get Minikube IP
MINIKUBE_IP=$(minikube ip)
echo $MINIKUBE_IP

# Access API Gateway
curl http://$MINIKUBE_IP:30007/users
curl http://$MINIKUBE_IP:30007/products
curl http://$MINIKUBE_IP:30007/orders
```

**Cloud Kubernetes**:
```bash
# Get External IP
kubectl get service apigateway -n microservices

# Access using External IP
curl http://<EXTERNAL_IP>:80/users
```

### Internal Service Discovery

Within the cluster, other services can access microservices using Kubernetes DNS:
- `http://userservice/api/users` (ClusterIP: port 80 → 8080)
- `http://productservice/api/products` (ClusterIP: port 80 → 8080)
- `http://orderservice/api/orders` (ClusterIP: port 80 → 8080)

Fully qualified domain names:
- `http://userservice.microservices.svc.cluster.local:80`
- `http://productservice.microservices.svc.cluster.local:80`
- `http://orderservice.microservices.svc.cluster.local:80`

## Scaling

### Manual Scaling
```bash
# Scale User Service to 5 replicas
kubectl scale deployment userservice --replicas=5 -n microservices

# Scale all services
kubectl scale deployment --all --replicas=3 -n microservices
```

### Automatic Scaling (HPA)
The User Service is configured with HPA that automatically scales between 2-6 replicas based on CPU utilization:

```bash
# Monitor HPA status
kubectl get hpa -n microservices -w

# Check HPA details
kubectl describe hpa userservice-hpa -n microservices
```

When CPU utilization exceeds 50% threshold, HPA automatically creates additional pods up to the max of 6 replicas.

## Updates and Rollouts

### Update Container Image
```bash
# Update User Service image
kubectl set image deployment/userservice \
  userservice=rajesakhare/userservice:v2.0 \
  -n microservices

# Update all services
kubectl set image deployment/*\
  userservice=rajesakhare/userservice:latest \
  productservice=rajesakhare/productservice:latest \
  orderservice=rajesakhare/orderservice:latest \
  apigateway=rajesakhare/apigateway:latest \
  -n microservices
```

### Check Rollout Status
```bash
# Watch rollout
kubectl rollout status deployment/userservice -n microservices

# View rollout history
kubectl rollout history deployment/userservice -n microservices
```

### Rollback
```bash
# Rollback to previous version
kubectl rollout undo deployment/userservice -n microservices

# Rollback to specific revision
kubectl rollout undo deployment/userservice --to-revision=1 -n microservices
```

## Resource Management

### Check Resource Usage
```bash
# Node resources
kubectl top nodes

# Pod resources (requires metrics-server)
kubectl top pods -n microservices

# Detailed view
kubectl describe node <node-name>
```

### Current Resource Configuration

Each service has:
- **CPU Request**: 100m (0.1 cores) - guaranteed minimum
- **CPU Limit**: 500m (0.5 cores) - maximum allowed
- **Memory Request**: 128Mi - guaranteed minimum
- **Memory Limit**: 256Mi - maximum allowed

Adjust these values in `microservices.yaml` based on actual workload requirements:

```yaml
resources:
  requests:
    cpu: "100m"
    memory: "128Mi"
  limits:
    cpu: "500m"
    memory: "256Mi"
```

## Database Configuration

### ProductService PostgreSQL

ProductService requires a PostgreSQL database. Options:

**Option 1: External PostgreSQL**
- Update connection string in ProductService configuration
- Ensure database is accessible from pods

**Option 2: PostgreSQL in Kubernetes**
- Create PostgreSQL StatefulSet/Deployment in the same namespace
- Update connection string to reference Kubernetes service

**Option 3: Cloud Database**
- Use managed PostgreSQL service (Azure Database, AWS RDS, Google Cloud SQL)
- Configure connection string in ProductService

Example connection string for Kubernetes-hosted PostgreSQL:
```
Host=postgresql.microservices.svc.cluster.local;Port=5432;Database=ProductDb;Username=postgres;Password=password
```

### PostgreSQL in Kubernetes (Recommended for Development)

Create a separate `postgresql.yaml` manifest for PostgreSQL:

```yaml
# ===============================
# PostgreSQL ConfigMap & Secret
# ===============================
apiVersion: v1
kind: ConfigMap
metadata:
  name: postgresql-config
  namespace: microservices
data:
  POSTGRES_DB: "ProductDb"
  POSTGRES_USER: "postgres"
  POSTGRES_PASSWORD: "password"
  PGDATA: "/var/lib/postgresql/data/pgdata"
---
apiVersion: v1
kind: Secret
metadata:
  name: postgresql-secret
  namespace: microservices
type: Opaque
data:
  # Base64 encoded values (echo -n 'password' | base64)
  POSTGRES_PASSWORD: cGFzc3dvcmQ=  # 'password'
  POSTGRES_USER: cG9zdGdyZXM=      # 'postgres'
---
# ===============================
# PostgreSQL Persistent Volume Claim
# ===============================
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: postgresql-pvc
  namespace: microservices
spec:
  accessModes:
    - ReadWriteOnce
  resources:
    requests:
      storage: 5Gi
---
# ===============================
# PostgreSQL StatefulSet
# ===============================
apiVersion: apps/v1
kind: StatefulSet
metadata:
  name: postgresql
  namespace: microservices
spec:
  serviceName: postgresql
  replicas: 1
  selector:
    matchLabels:
      app: postgresql
  template:
    metadata:
      labels:
        app: postgresql
    spec:
      containers:
      - name: postgresql
        image: postgres:15-alpine
        ports:
        - containerPort: 5432
        env:
        - name: POSTGRES_DB
          valueFrom:
            configMapKeyRef:
              name: postgresql-config
              key: POSTGRES_DB
        - name: POSTGRES_USER
          valueFrom:
            configMapKeyRef:
              name: postgresql-config
              key: POSTGRES_USER
        - name: POSTGRES_PASSWORD
          valueFrom:
            secretKeyRef:
              name: postgresql-secret
              key: POSTGRES_PASSWORD
        - name: PGDATA
          valueFrom:
            configMapKeyRef:
              name: postgresql-config
              key: PGDATA
        volumeMounts:
        - name: postgresql-storage
          mountPath: /var/lib/postgresql/data
        resources:
          requests:
            cpu: "200m"
            memory: "256Mi"
          limits:
            cpu: "500m"
            memory: "512Mi"
        livenessProbe:
          exec:
            command:
            - pg_isready
            - -U
            - postgres
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          exec:
            command:
            - pg_isready
            - -U
            - postgres
          initialDelaySeconds: 5
          periodSeconds: 5
      volumes:
      - name: postgresql-storage
        persistentVolumeClaim:
          claimName: postgresql-pvc
---
# ===============================
# PostgreSQL Service
# ===============================
apiVersion: v1
kind: Service
metadata:
  name: postgresql
  namespace: microservices
spec:
  type: ClusterIP
  selector:
    app: postgresql
  ports:
  - port: 5432
    targetPort: 5432
```

### Deploy PostgreSQL

**Option 1: Deploy with all services (Recommended)**

PostgreSQL is now included in `microservices.yaml`. Deploy all services including PostgreSQL:

```bash
kubectl apply -f microservices.yaml
```

**Option 2: Deploy PostgreSQL separately**

If you prefer to manage PostgreSQL separately, use the dedicated manifest:

```bash
kubectl apply -f postgresql.yaml
```

### Verify PostgreSQL Deployment

```bash
# Check all pods
kubectl get pods -n microservices

# Check PostgreSQL specifically
kubectl get pods -n microservices -l app=postgresql
kubectl get pvc -n microservices
kubectl get services -n microservices -l app=postgresql

# Check logs
kubectl logs -l app=postgresql -n microservices
```

### Update ProductService Connection String

After deploying PostgreSQL, update the ProductService deployment to use the Kubernetes service:

```bash
# Update environment variable
kubectl set env deployment/productservice \
  ConnectionStrings__DefaultConnection="Host=postgresql.microservices.svc.cluster.local;Port=5432;Database=ProductDb;Username=postgres;Password=password" \
  -n microservices
```

Or update the deployment manifest directly and reapply.

### Verify Database Connection

```bash
# Check ProductService logs for database connection
kubectl logs -l app=productservice -n microservices

# Test database connectivity from ProductService pod
kubectl exec -it <productservice-pod> -n microservices -- /bin/bash
# Inside pod:
# apt-get update && apt-get install -y postgresql-client
# psql -h postgresql.microservices.svc.cluster.local -U postgres -d ProductDb
```

### Database Backup and Restore

**Backup**:
```bash
# Create backup from running pod
kubectl exec -it <postgresql-pod> -n microservices -- pg_dump -U postgres ProductDb > productdb_backup.sql

# Copy backup to local machine
kubectl cp microservices/<postgresql-pod>:/productdb_backup.sql ./productdb_backup.sql
```

**Restore**:
```bash
# Copy backup to pod
kubectl cp ./productdb_backup.sql microservices/<postgresql-pod>:/productdb_backup.sql

# Restore database
kubectl exec -it <postgresql-pod> -n microservices -- psql -U postgres -d ProductDb < productdb_backup.sql
```

### Scaling PostgreSQL

For production, consider:
- **Read replicas** for read-heavy workloads
- **Connection pooling** with PgBouncer
- **High availability** with PostgreSQL operator (e.g., Zalando Postgres Operator)
- **Backup automation** with scheduled jobs

### Troubleshooting PostgreSQL

**Pod not starting**:
```bash
kubectl describe pod <postgresql-pod> -n microservices
kubectl logs <postgresql-pod> -n microservices
```

**Connection issues**:
```bash
# Test DNS resolution
kubectl exec -it <productservice-pod> -n microservices -- nslookup postgresql.microservices.svc.cluster.local

# Test port connectivity
kubectl exec -it <productservice-pod> -n microservices -- nc -zv postgresql.microservices.svc.cluster.local 5432
```

**Data persistence**:
```bash
kubectl get pvc -n microservices
kubectl describe pvc postgresql-pvc -n microservices
```

### Production Considerations

For production deployments:
1. **Use managed PostgreSQL** (AWS RDS, Azure Database, Google Cloud SQL)
2. **Enable SSL/TLS** for encrypted connections
3. **Configure proper resource limits** based on load testing
4. **Set up automated backups** and monitoring
5. **Use secrets management** (Azure Key Vault, AWS Secrets Manager, etc.)
6. **Configure network policies** for security

## Monitoring and Observability

### View Logs Across Multiple Pods
```bash
# Logs from all API Gateway pods
kubectl logs -l app=apigateway -n microservices

# Tail logs from all pods
kubectl logs -l app=apigateway -n microservices -f

# Logs from last 1 hour
kubectl logs <pod-name> -n microservices --since=1h
```

### Event Monitoring
```bash
# Watch events in namespace
kubectl get events -n microservices -w

# View recent events
kubectl get events -n microservices --sort-by='.lastTimestamp'
```

### Pod Debugging
```bash
# Execute command in pod
kubectl exec -it <pod-name> -n microservices -- /bin/sh

# Copy file from pod
kubectl cp microservices/<pod-name>:/path/to/file ./local-file

# Port forward to local machine
kubectl port-forward <pod-name> 8080:8080 -n microservices
```

## Cleanup

### Delete All Resources
```bash
# Delete entire namespace (deletes all resources in it)
kubectl delete namespace microservices

# Or delete just the manifest
kubectl delete -f microservices.yaml
```

### Delete Specific Resources
```bash
# Delete specific deployment
kubectl delete deployment userservice -n microservices

# Delete specific service
kubectl delete service apigateway -n microservices

# Delete HPA
kubectl delete hpa userservice-hpa -n microservices
```

## Troubleshooting

### Pod Not Starting

**Check pod status**:
```bash
kubectl describe pod <pod-name> -n microservices
```

**Common issues**:
- Image pull errors: Verify image registry and credentials
- Resource constraints: Check node availably
- Port conflicts: Ensure ports are available

### Service Not Accessible

**From local machine**:
```bash
# Check service type and port
kubectl get service apigateway -n microservices

# For NodePort, verify you're using correct port (30007)
# For ClusterIP, use port-forward to test

kubectl port-forward svc/apigateway 8080:80 -n microservices
curl http://localhost:8080
```

**Between pods**:
```bash
# Test DNS resolution
kubectl exec <pod-name> -n microservices -- nslookup userservice

# Test connectivity
kubectl exec <pod-name> -n microservices -- curl http://userservice
```

### High CPU Usage

**Check HPA status**:
```bash
kubectl get hpa -n microservices -v
```

**Monitor metrics**:
```bash
kubectl top pods -n microservices
```

**Increase HPA limits**:
```bash
kubectl patch hpa userservice-hpa \
  -p '{"spec":{"maxReplicas":10}}' \
  -n microservices
```

### Persistent Storage

For ProductService with PostgreSQL, consider:
- **PersistentVolume (PV)**: Actual storage
- **PersistentVolumeClaim (PVC)**: Request for storage
- **StatefulSet**: For stateful PostgreSQL

Add to `microservices.yaml` if needed.

## Next Steps

1. **Update image registry**: Replace `rajesakhare/` with your registry
2. **Configure PostgreSQL**: Set up database for ProductService
3. **Add monitoring**: Deploy Prometheus, Grafana, or similar
4. **Add logging**: Deploy ELK stack or similar for centralized logs
5. **Configure ingress**: Set up Kubernetes Ingress for better routing
6. **Add RBAC**: Configure Role-Based Access Control for security

## Resources

- [Kubernetes Documentation](https://kubernetes.io/docs/)
- [Kubectl Cheat Sheet](https://kubernetes.io/docs/reference/kubectl/cheatsheet/)
- [Deployments](https://kubernetes.io/docs/concepts/workloads/controllers/deployment/)
- [Services](https://kubernetes.io/docs/concepts/services-networking/service/)
- [Horizontal Pod Autoscaler](https://kubernetes.io/docs/tasks/run-application/horizontal-pod-autoscale/)
- [Minikube Documentation](https://minikube.sigs.k8s.io/docs/)

## Support

For issues or questions about Kubernetes deployment:
1. Check pod logs: `kubectl logs <pod-name> -n microservices`
2. Describe pod: `kubectl describe pod <pod-name> -n microservices`
3. Check events: `kubectl get events -n microservices`
4. Review this README for solutions

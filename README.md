# HRSystem

Run the backend with SQL Server using Docker. The setup uses docker-compose (simple) and optional Kubernetes manifests.

## Docker Compose

- Prereqs: Docker Desktop 4+, ports `8080`, `1433` free.
- Start stack:
  - `SA_PASSWORD` must meet SQL Server complexity rules.

```
SA_PASSWORD='Your_strong_password123!' docker compose up -d --build
```

- Or run helper script (auto-opens browser to Swagger):

```
bash scripts/compose-up.sh
```

- Services:
  - API: `http://localhost:8080` (Swagger: `/swagger`)
  - SQL Server: `localhost,1433` (`sa` / `$SA_PASSWORD`)

- Connection string (injected to API):
  - `Server=sqlserver,1433;Database=HRSystemDb;User Id=sa;Password=$SA_PASSWORD;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=True`

- Stop and remove:

```
docker compose down -v
```

## Email

- Email is not configured in this stack.

## Optional: Kubernetes (local cluster)

- Files in `k8s/` provide a minimal setup for the API and SQL Server.
- Create the secret for SA password (replace the literal password):

```
kubectl create ns hr
kubectl create secret generic mssql-secret -n hr \
  --from-literal=SA_PASSWORD='Your_strong_password123!'
```

- Apply manifests:

```
kubectl apply -n hr -f k8s/sqlserver.yaml
kubectl apply -n hr -f k8s/api.yaml
```

- Port-forward for local access:

```
kubectl -n hr port-forward svc/hr-api 8080:8080 &
kubectl -n hr port-forward svc/hr-mssql 1433:1433 &
```

- Or run helper script (build, apply, forward, auto-open browser):

```
bash scripts/k8s-up.sh
```

## Notes

- The API seeds the database on start in Development. Ensure the DB is reachable via the provided connection string.
- For production, replace MailHog with a real SMTP provider and secure all secrets via your orchestrator’s secret store.

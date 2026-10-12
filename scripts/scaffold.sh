#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."
dotnet tool restore
dotnet ef dbcontext scaffold Name=ConnectionStrings:LittleSteps Npgsql.EntityFrameworkCore.PostgreSQL --project src/LittleSteps.Api --output-dir Data/Entities --context-dir Data --context AppDbContext --no-onconfiguring --force

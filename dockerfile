# Stage 1: Build frontend React application
FROM node:14 as frontend-build

WORKDIR /frontend

COPY frontend/package.json frontend/yarn.lock ./
RUN yarn install
COPY frontend ./
RUN yarn build

# Stage 2: Build backend .NET Core application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS backend-build

WORKDIR /backend

# Copy and restore as distinct layers
COPY backend/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY backend/ ./
RUN dotnet publish -c Release -o out

# Stage 3: Production environment with Apache
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

# Install Apache
RUN apt-get update && \
    apt-get install -y apache2 && \
    a2enmod rewrite

# Configure Apache
COPY ./apache-config.conf /etc/apache2/sites-available/000-default.conf
COPY --from=backend-build /backend/out /var/www/html

# Copy frontend build to Apache document root
COPY --from=frontend-build /frontend/build /var/www/html

# Start Apache
EXPOSE 80
CMD ["apache2ctl", "-D", "FOREGROUND"]

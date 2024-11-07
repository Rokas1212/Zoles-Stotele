# Use the official PostgreSQL image as the base image
FROM postgres:15

# Set environment variables for PostgreSQL
ENV POSTGRES_USER=useris
ENV POSTGRES_PASSWORD=password
ENV POSTGRES_DB=db

# Expose the PostgreSQL port
EXPOSE 5432

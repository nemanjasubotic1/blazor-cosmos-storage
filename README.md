# Blazor Server App with Authentication, Authorization, Azure CosmosDb and Azure Storage

This repository demonstrates a Blazor Server application built with .NET 8, showcasing authentication, authorization, and Cloud storage and CosmosDb simple integration with Azure.

## Features

*   **Blazor Server Interactivity**
*   **Authentication and Authorization:** Implements simple authentication and authorization using Microsoft Identity, Entity Framework Core, and SQL Server.
*   **Azure Blob Storage Integration:** Enables storing images and other files using Azure Blob Storage. Employs short-lived Shared Access Signatures (SAS) for secure access.
*   **Azure Cosmos DB Integration:** Utilizes Azure Cosmos DB as a NoSQL database for storing product details and other relevant data.
*   **.NET 8:** Built on the .NET 8 framework.
*   **Configuration:** Uses `appsettings.json` and environment variables for flexible configuration.

## Technologies Used

*   .NET 8
*   Blazor Server
*   Microsoft Identity
*   Entity Framework Core
*   SQL Server
*   Azure Blob Storage
*   Azure Cosmos DB

## Getting Started

1.  **Configure the application:**

    The application requires configuration for database connections, Azure storage accounts, and authentication settings. You can configure these settings in `appsettings.json` or by using environment variables which are already in code, there is a FeatureManagement switch option.


2.  **Database Migrations:**

    If using Entity Framework Core, apply database migrations:

    ```bash
    dotnet ef database update
    ```

## Testing

For local testing, ensure that you have configured the necessary connection strings and Azure settings either in `appsettings.json` or as environment variables.

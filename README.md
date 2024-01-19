# Project Name: Application Form Scanner

## Project Information:
- Genre: Web application
- Link: [Link to Application](https://afsbackendwebapp.azurewebsites.net/Home/Visualization)
- Language: C#

## Project Detail:
- Purpose: Scanning handwritten applications/forms
- Target Audience: BC Housing staff
- Hosting: Azure

## Tech Stack:
- Storage: Azure Blob Storage
- Hosting: Azure App Service
- Database: Azure SQL Database
- Others: Azure Function App

## Project Description:

The primary goal of the Application Form Scanner project is to leverage Document Intelligence (Azure AI service) for the development of a web application designed for BC Housing. This web application is capable of processing handwritten applications, including those for SAFER (Shelter Aid for Elderly Renters), and transforming them into digital versions. The project's objective is to streamline the documentation digitization process at BC Housing, reducing the manual effort required for converting handwritten applications into digital formats.

The web application provides the frontend with a few functionalities, allowing users to submit handwritten applications and store them in the blob storage. After the file is saved in the storage, the AI service will access the file to classify and retrieve data. Users can access and modify the digital version of the application via the web form.

The backend of the application was built in C#. It maintains communication between the frontend and the database for the entire application.

The primary database used in the application is the SQL database Azure SQL Database. Additionally, Azure Blob Storage is employed as the main storage for images and PDF files of handwritten applications within the project.

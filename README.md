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

The primary goal of the **Application Form Scanner** project is to leverage Document Intelligence (Azure AI service) for the development of a web application designed for BC Housing. This web application is capable of processing handwritten applications, including those for SAFER (Shelter Aid for Elderly Renters) - [Safer form](https://www.bchousing.org/sites/default/files/featured_downloads/2022-11/SAFER-Application-Form.pdf), and transforming them into digital versions. The project's objective is to streamline the documentation digitization process at BC Housing, reducing the manual effort required for converting handwritten applications into digital formats.

The web application offers several features on the frontend, enabling users to submit handwritten applications and store them in blob storage. Once the file is saved, the AI service accesses it to classify and retrieve data. Users can then access and modify the digital version of the application through the web form.

Built in C#, the backend of the application facilitates communication between the frontend and the database throughout the entire system.

The primary database utilized in the application is the Azure SQL Database, an SQL database. Additionally, Azure Blob Storage serves as the main repository for images and PDF files of handwritten applications within the project.

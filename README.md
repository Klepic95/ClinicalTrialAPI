The Clinical Trial Data Processor is a system that processes clinical trial data in JSON format. It validates the input data against a predefined schema, transforms it into the appropriate data models, and stores the processed data into a database using Entity Framework Core. The application ensures that clinical trial records conform to the required business rules and database schema before storage.

How to successfully configure the project locally:

Clone repository
Install and configure Docker locally
Open PowerShell and run this exact script, however replace specified parameters:
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=ctaPass1!" -p 1433:1433 --name cta4CreateSqlServer -v C:\Users\{your-name}\DockerVolumes\sqlserver:/var/opt/mssql/data -d mcr.microsoft.com/mssql/server:latest

Note: in above script, replace {your-name} with the actual name of your local machine

After ensuring that connection with the SQL server exists via Docker Container, in the Visual Studio>Package Manager Console, execute following script: "dotnet ef database update" NOTE: Setup startup and Default project in VisualStudio>Package Manager Console to: "ClinicalTrial.DAL". You will need to be in the root of this project!

Change startup project in Visuatl studio to "ClinicalTrial.API" and run the solution!

It is necessary to follow these steps in order to sucessfully run application locally!

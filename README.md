The Clinical Trial Data Processor is a system that processes clinical trial data in JSON format. It validates the input data against a predefined schema, transforms it into the appropriate data models, and stores the processed data into a database using Entity Framework Core. The application ensures that clinical trial records conform to the required business rules and database schema before storage.

How to successfully configure the project locally:

Clone repository
Install and configure Docker locally (Link for download: https://www.docker.com/products/docker-desktop/)
Open PowerShell and run this script, however replace specified parameters:
"docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=ctaPass1!" -p 1433:1433 --name cta4CreateSqlServer -v C:\Users\{your-name}\DockerVolumes\sqlserver:/var/opt/mssql/data -d mcr.microsoft.com/mssql/server:latest"
Note: in above script, replace {your-name} with the actual name of your local machine

After ensuring that connection with the SQL server exists via Docker Container, in the Visual Studio>Package Manager Console, execute following script: "dotnet ef database update". 
NOTE: Setup startup and Default project in VisualStudio>Package Manager Console to: "ClinicalTrial.DAL". You will need to be in the root of this project, so adjust location when you are executing script otherwise the script will fail!
Change startup project in Visuatl studio to "ClinicalTrial.API" and run the solution if you do not want to run it via Docker.

If you want to build and run application using Docker, please use PowerShell and go in the root Directory of the project (Where both Dockerfile and ClinicalTrial.sln are located), and run following command: "docker build -t clinicaltrialapi .". Once this build is finished successfully, please run this command: "docker run -p 8080:8080 clinicaltrialapi" - it will run the build Image in the Docker Container. Now you can access the Swagger page of the application following this link, and also you can create requests via swagger UI: "http://localhost:8080/swagger/index.html"

Additionally, if you would like to check if the container is running, that can be done running this command: "docker ps".

JSON file for testing application can be find in the root of the project and it is called: "testingFile.json". User can use this file for testing, while changing parameters inside it to achieve different results.

It is necessary to follow these steps in order to sucessfully run application locally!

If you run into any issue, do not hessitate to contact me via email: Klepicpredrag@gmail.com

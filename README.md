# AzureFunctionsTemplate
 .Net8 Function app template.

 This is a fairly basic template that can be used when starting a new project. 
 
 I have included some example services and function calls purely to see how the code can be implemented, and services injected. It is not meant to be very functional, more for learning / just as an example.

# DiscordBot-JS
Discord bot in javascript with reddit and twitter api integrations.

You must have developer accounts for Twitter and Reddit, and you must have created an application and recieved the appropriate tokens for your applications from Discord, Twitter and Reddit in order for this bot to build and run. If you don't need either Twitter or Reddit, you can remove or comment out the code that references them.

### Instructions for building and running

1. Clone/Download this repository
2. Install Azure Core Tools if you do not have it already. You must have this installed to run an Azure Function app locally.
3. Make sure .Net8 SDK is installed.
4. Purposefully, this repo/application is missing a local.setting.json file. I have not included it as a file to push to this repo to start, but it is mandatory to run locally. Please add that file to the AzureFunctions project. This is the bare minimum that will allow it to run, ofcourse you would need to provide a DB connection string to perform any calls to the DB, and if you do not have a storage resource created on azure, you can use local storage. Please download the Azure Storage emulator if it is missing from you pc. https://learn.microsoft.com/en-us/azure/storage/common/storage-use-emulator
   please add the following json to your local.settings.json file you have added. 
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "DbConnectionString": ""
  }
}
6. The Program.cs file is our "Start up" or "boot strapping" file. It automattically configures our connection to an Azure storage account and DB connection via entity framework 8. It will use the connection strings provided in our local.settings.json file.
7. The app will not start off the get go because I have included a blobtrigger example function. You must add that container to your storage account (which the function is listening against) or you will need to comment out that function.
8. The app should run, and you will see a "Welcome to Azure" function on an http trigger in your console. You can navigate to that localhost using your browser. If successful, you browser will display a "Welcome to Azure Functions!" message.
9. Congrats its up an running!

### Available Services / Infrastructure

In the "Infrastructure" project, I've included the following services / classes.
>BlobService.cs
- Manipulating Azure Blob storage containers. Add, delete, update, etc.
>QueueService.cs
- Connect to a queue, add messages. I generally haven't needed to delete messages as they're automatically removed once processed so I didn't include that functionality.
>DataService.cs
- Connect to SQL DB using Entity Framework 8. Uses DataContext class to manage connections (open, close) and provides SqlQuery functionality. There is comments in the code to describe behavior.

Examples of how to use the "Infrastructure" services are contained in the "Services" project. 

The classes / services here are purely for examples. They are not really functional. I just included some mock code so you could see how Dependency Injection works in .Net / Azure functions. Ideally you would delete or rename these classes. I may remove the whole project in the future and keep this purely as a template with infrastructure. 

speaking of mocking...

### Unit Tests
I've include a Tests.Unit project to house unit tests. 

I'm using a combination of XUnit as the framework, and the "Moq" library to mock our depedencies. 

The tests here aren't really functional becauset they're based off our Services project classes, which again are meant as examples. The test class in here is just to give you an idea of how to mock and set up your services. 


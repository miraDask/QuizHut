# QuizHut

Web application for a simple educational network with built-in Quiz system.
This project is the defense project for ASP.NET Core course (part of the C# Web Module at Software University).

## Table of Contents
1. [Technology stack](https://github.com/miraDask/QuizHut#technology-stack)
2. [Screenshots](https://github.com/miraDask/QuizHut#screenshots)
3. [Application Configurations](https://github.com/miraDask/QuizHut#application-configurations)
4. [Credits](https://github.com/miraDask/QuizHut#credits)

## Technology stack:

- ASP.NET Core 3.1

- Entity Framework Core 3.1

- SQL Server

- SendGrid

- SignalR

- Hangfire

- JavaScript

- jQuery

- Bootstrap

## Screenshots:

### Home Page (guest)
<img width="953" alt="Home page" src="https://user-images.githubusercontent.com/35633887/90977460-ab6c7900-e53d-11ea-871c-96b11ed7c821.png">

### Home Page (admin)
<img width="959" alt="Admin home page" src="https://user-images.githubusercontent.com/35633887/90977476-b3c4b400-e53d-11ea-9043-b064a5a97322.png">

### Dasboard (admin)
<img width="945" alt="Admin dashboard" src="https://user-images.githubusercontent.com/35633887/90977533-fbe3d680-e53d-11ea-986c-11496508167a.png">

### Quizzes Page (admin/teacher)
<img width="949" alt="Quizzes page" src="https://user-images.githubusercontent.com/35633887/90977554-2d5ca200-e53e-11ea-8ae8-70c4542aae15.png">

### Create Quiz (admin/teacher)
<img width="470" alt="Create quiz-details page" src="https://user-images.githubusercontent.com/35633887/90977557-30f02900-e53e-11ea-9fd0-4154160cf971.png">

### Take Quiz (student)
<img width="960" alt="Take quiz page" src="https://user-images.githubusercontent.com/35633887/90977559-364d7380-e53e-11ea-9971-3f7dace94b18.png">

### Warnings
<img width="942" alt="warnings" src="https://user-images.githubusercontent.com/35633887/90977561-42393580-e53e-11ea-923d-bf18c5b7891b.png">


## Application Configurations:

1. Check connection string in appsettings.json.
   If you don't use SQLEXPRESS you should replace "Server=.\\SQLEXPRESS..." with "Server=.;...".

2. Add new schema for Hangfire in your database using MSSQL Server Manager (CREATE SCHEMA hangfire).

3. In order to use Distributed Sql Cache, you must create Cache table in your.
   Open your command-line in QuizHut.Web folder and run the following command:
   >dotnet sql-cache create "Server=.\SQLEXPRESS;Database=QuizHut;Trusted_Connection=True;MultipleActiveResultSets=true" dbo Cache

4. To work properly the app needs SendGrid key. There are two ways to deal with that:
- You can comment the following line in Startup.cs, but that way you would not be able to send emails:  
  >services.AddTransient<IEmailSender>(x => new SendGridEmailSender(new LoggerFactory(), this.configuration["Sendgrid"])); (line 113)
- For testing email sending you should have your own key for SendGrid and add it in secrets.json 
   (secrets.json file can be found: -> open Solution Explorer -> right click on QuizHut.Web project -> Manage User Secrets).
   Then add object like this:
   > { "Sendgrid": "your api key"}
  

## Credits
  
 Using ASP.NET-MVC-Template originally developed by:
- [Nikolay Kostov](https://github.com/NikolayIT)
- [Vladislav Karamfilov](https://github.com/vladislav-karamfilov)

Using Lazy Kit Bootstrap template : https://demo.themewagon.com/preview/lazy-kit-is-a-free-bootstrap-4-ui-kit-lazy-kits

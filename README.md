# QuizHut

Web application for simple Quiz system.
This project is the defense project for ASP.NET Core (part of the C# Web Module at Software University).

### App Setup for testing:

1. Check connection string in appsettings.json.
   If you don't use SQLEXPRESS you should replace "Server=.\\SQLEXPRESS..." with "Server=.;...".

2. Add new schema for Hangfire in your database using MSSQL Server Manager (CREATE SCHEMA hangfire).

3. In order to use Distributed Sql Cache, you must create Cache table in your.
   Open your command-line in QuizHut.Web folder and run the following command:
   >dotnet sql-cache create "Server=.\SQLEXPRESS;Database=QuizHut;Trusted_Connection=True;MultipleActiveResultSets=true" dbo Cache

4. To work properly the app needs Sendgrid key. There are two ways to deal with that:
- You can comment the following line in Startup.cs, but that way you would not be able to send emails:  
  >services.AddTransient<IEmailSender>(x => new SendGridEmailSender(new LoggerFactory(), this.configuration["Sendgrid"])); (line 113)
- For testing email sending you should have your own key for Sendgrid and add it in secrets.json 
   (secrets.json file can be found when open Solution Explorer -> right click on QuizHut.Web project -> Manage User Secrets).
   Then add object like this:
   > { "Sendgrid": "your api key"}
  

### Credit
  
 Using ASP.NET-MVC-Template originally developed by:
- [Nikolay Kostov](https://github.com/NikolayIT)
- [Vladislav Karamfilov](https://github.com/vladislav-karamfilov)

Using Lazy Kit Bootstrap template : https://demo.themewagon.com/preview/lazy-kit-is-a-free-bootstrap-4-ui-kit-lazy-kits

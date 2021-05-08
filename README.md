CoreNotify is a service for sending scheduled and on-demand email notifications from .NET Core applications. Emails are built from HTML views, so your notifications are as personalized and detailed as necessary, yet as easy to make as any other HTML content in your application. You focus on your email content and recipients, while CoreNotify handles the scheduling and delivery. You must provide a SendGrid API key.

Here's a [PowerPoint](https://1drv.ms/v/s!AvguHRnyJtWMmvg7VYYu2haLfLgfkg?e=tC2zAk) walkthrough.

# How it Would Work
- You create an account at corenotify.dev and note the [AuthorizationKey](https://github.com/adamfoneil/CoreNotify/blob/master/CoreNotify.Database/Account.cs#L28) you receive. This will go in your application configuration. You must also provide a [SendGridApiKey](https://github.com/adamfoneil/CoreNotify/blob/master/CoreNotify.Database/Account.cs#L35). There will also be a CLI for this.
- In your application, add forthcoming NuGet package [CoreNotify.Client](https://github.com/adamfoneil/CoreNotify/blob/master/CoreNotify.Client/CoreNotify.Client.csproj). Create MVC views or Razor pages that you want to send as email. The views you create will become the content of email messages. See [sample controller](https://github.com/adamfoneil/CoreNotify/blob/master/CoreNotify.TestApp/Controllers/EmailController.cs) and [sample app startup](https://github.com/adamfoneil/CoreNotify/blob/master/CoreNotify.TestApp/Startup.cs#L24-L25). This show how to secure your endpoints for use by CoreNotify.
- Create Json endpoints that return a list of recipients for each email. Each recipient implements [ISendRequest](https://github.com/adamfoneil/CoreNotify/blob/master/CoreNotify.Shared/Interfaces/ISendRequest.cs). CoreNotify can send emails one at a time to specific recipients, but it becomes more powerful when you send in bulk on a schedule.
- At corenotify.dev, create a **Notification** for each kind of email you want to send. You specify, among other things, the Cronjob Schedule along with the Content and Recipient endpoints. See [Notification](https://github.com/adamfoneil/CoreNotify/blob/master/CoreNotify.Database/Notification.cs) model.
- When you create Notifications with a [schedule](https://github.com/adamfoneil/CoreNotify/blob/master/CoreNotify.Database/Notification.cs#L32), CoreNotify's backend cronjob functionality (powered by [SetCronJob](https://www.setcronjob.com/) and managed through another API integration [SetCronJob.ApiClient](https://github.com/adamfoneil/CoreNotify/blob/master/CoreNotify.Service/CoreNotify.Service.csproj#L15)) executes your endpoints, building and sending your emails.

## About the Repo
There are some secrets not in source control that you must provide if you're cloning and running this. You'll need to use the Manage Secrets feature.
- [SetCronJob:ApiToken](https://github.com/adamfoneil/CoreNotify/blob/master/Testing.Service/ServiceTests.cs#L52)
- [SendGrid:ApiKey](https://github.com/adamfoneil/CoreNotify/blob/master/Testing.Service/ServiceTests.cs#L34)
- [CallbackFunctionCode](https://github.com/adamfoneil/CoreNotify/blob/master/Testing.Service/ServiceTests.cs#L58)

Also, you'll need to create a blank database manually, but then use [ModelSync](https://aosoftware.net/modelsync/) to create the tables from the CoreNotify.Database assembly.

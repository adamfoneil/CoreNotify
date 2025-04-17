When you create a new .NET web app, an `IdentityNoOpEmailSender` placeholder class is stubbed out so the `IEmailSender<TUser>` has an implementation. By design it doesn't work -- hence the "NoOp" in the name. You need to register with an email service like Mailgun, SendGrid, or others to have actual email capability in your application. Setting that up is not trivial. Email providers typically require you to prove ownership of your domain, and have unique APIs for interacting with their service.

CoreNotify solves this by offering a minimal email service for `IEmailSender<T>`, giving you working account notifications for ASP.NET Core web apps with very little setup. It has a 30-day free trial, and is $5/month thereafter. There's no credit card required, and no automatic recurring payment.

1. Install the [CoreNotify CLI](https://www.nuget.org/packages/CoreNotify.CLI).
```
dotnet tool install --global CoreNotify.CLI
```
2. Use the CLI tool to create an API key:
```
corenotify register <youremail>
```
You will receive your CoreNotify API key at the email you provide.

3. Add the [CoreNotify.MailerSend](https://www.nuget.org/packages/CoreNotify.MailerSend/) package to your project.
```
dotnet add package CoreNotify.MailerSend
```
4. Add your API key from step 2 to your configuration. There are a number of ways to do this. See example in [DemoApp appsettings](https://github.com/adamfoneil/CoreNotify/blob/master/DemoApp/appsettings.json#L3)
5. Replace the `IdentityNoOpEmailSender` in your app with `CoreNotifyEmailSender` using the [AddCoreNotify](https://github.com/adamfoneil/CoreNotify/blob/master/CoreNotify.MailerSend/Extensions/ServiceCollectionExtensions.cs#L11) extension method. See [demo](https://github.com/adamfoneil/CoreNotify/blob/master/DemoApp/Program.cs#L33)

Note that you can also use extension method [AddCoreNotifyGenericEmailSender](https://github.com/adamfoneil/CoreNotify/blob/master/CoreNotify.MailerSend/Extensions/ServiceCollectionExtensions.cs#L20) if you already have a MailerSend API key. This bypasses the CoreNotify dependency, and lets you customize the email content by supplying your own [EmailSenderContent](https://github.com/adamfoneil/CoreNotify/blob/master/CoreNotify.MailerSend/EmailSenderContent.cs) class.

Note that the API key in the demo app no longer works (or may work intermittently between recycles), but I'm showing an example of how you can configure this in your own apps.

A few things to note about how this works:
- CoreNotify uses [MailerSend](https://www.mailersend.com/) under the hood.
- Account notification emails will come from **{yourdomain}.corenotify.net**
- [CoreNotifyEmailSender](https://github.com/adamfoneil/CoreNotify/blob/master/CoreNotify.MailerSend/CoreNotifyEmailSender.cs) is not a generic email client. It sends only `IEmailSender<TUser>` messages (account confirmations, password resets), and you cannot customize the email content, with one exception.
- You can send arbitrary email content to the account email you registered using the [SendAlertAsync](https://github.com/adamfoneil/CoreNotify/blob/master/CoreNotify.Client/CoreNotifyClient.cs#L64) method.
- For generic email capability with MailerSend, use [MailerSendClient](https://github.com/adamfoneil/CoreNotify/blob/master/CoreNotify.MailerSend/MailerSendClient.cs). In that case, you use your own MailerSend API key.

# Serilog Alerts
If you're self-hosting Serilog in your projects using the SQL Server sink, have you wanted to get alerts automatically?

Please see the [Serilog Alerts project](https://github.com/adamfoneil/CoreNotify/tree/master/CoreNotify.SerilogAlerts.SqlServer).

# Payment
All accounts have a 30 day free trial. If you like this service, send $5 * number of months you'd like

[paypal.me/adamosoftware](https://paypal.me/adamosoftware?country.x=US&locale.x=en_US)

I will manually extend your account.

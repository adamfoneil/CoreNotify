When you create a new .NET web app, an `IdentityNoOpEmailSender` placeholder class is stubbed out so the `IEmailSender<TUser>` has an implementation. By design it doesn't work -- hence the "NoOp" in the name. You need to register with an email service like Mailgun, SendGrid, or others to have actual email capability in your application. Setting that up is not trivial. Email providers typically require you to prove ownership of your domain, and have unique APIs for interacting with their service.

CoreNotify solves this by offering a minimal email service for `IEmailSender<T>`, giving you working account notifications for ASP.NET Core web apps with very little setup. It has a 30-day free trial, and is $5/month thereafter. There's no credit card required, and no automatic recurring payment.

1. Install the CoreNotify CLI.
```
dotnet tool install --global CoreNotify.CLI
```
2. Use the CLI tool to create an API key:
```
corenotify register <youremail>
```
You will receive your CoreNotify API key at the email you provide.

3. Add the **CoreNotify.MailerSend** package to your project.
```
dotnet add package CoreNotify.MailerSend
```
4. Add your API key from step 2 to your configuration. There are a number of ways to do this.
5. Replace the `IdentityNoOpEmailSender` in your app with `CoreNotifyEmailSender`

A few things to note about how this works:
- CoreNotify uses [MailerSend](https://www.mailersend.com/) under the hood.
- Account notification emails will come from **{yourdomain}.corenotify.net**
- [CoreNotifyEmailSender](https://github.com/adamfoneil/CoreNotify/blob/master/MailerSend/CoreNotifyEmailSender.cs) is not a generic email client. It sends only `IEmailSender<TUser>` messages (account confirmations, password resets), and you cannot customize the email content.
- For generic email capability with MailerSend, use [MailerSendClient](https://github.com/adamfoneil/CoreNotify/blob/master/MailerSend/MailerSendClient.cs). In that case, you use your own MailerSend API key.

# Payment
If you like this service, send $5 * number of months you'd like

[paypal.me/adamosoftware](https://paypal.me/adamosoftware?country.x=US&locale.x=en_US)

I will manually extend your account.

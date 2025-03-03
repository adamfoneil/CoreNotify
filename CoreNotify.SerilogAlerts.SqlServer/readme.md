Schedule email alerts for Serilog data stored in SQL Server. Requires a [CoreNotify](https://github.com/adamfoneil/CoreNotify) account. Note that the email recipient must be your registered CoreNotify email address. You cannot send to arbitrary email addresses.

Scheduling capability is provided by [Coravel](https://github.com/jamesmh/coravel).

# Getting Started
1. Add CoreNotify to your project by installing NuGet package [CoreNotify.MailerSend](https://www.nuget.org/packages/CoreNotify.MailerSend), and setup your account as described in the CoreNotify readme linked above.
2. Add the [CoreNotify.SerilogAlerts.SqlServer](https://www.nuget.org/packages/CoreNotify.SerilogAlerts.SqlServer/) package to your project.
3. Add a "SerilogAlerts" section to your `appsettings.json` file. See this [example](https://github.com/adamfoneil/CoreNotify/blob/master/DemoApp/appsettings.json#L11) along with all the [configuration options](https://github.com/adamfoneil/CoreNotify/blob/master/CoreNotify.SerilogAlerts.SqlServer/SerilogQuery.cs#L23).
4. Add Serilog Alerts to your project startup, [example](https://github.com/adamfoneil/CoreNotify/blob/master/DemoApp/Program.cs#L34)
5. Add Coravel scheduler service to your project startup, [example](https://github.com/adamfoneil/CoreNotify/blob/master/DemoApp/Program.cs#L28).
6. Use Coravel scheduler in your pipeline, [example](https://github.com/adamfoneil/CoreNotify/blob/master/DemoApp/Program.cs#L55).
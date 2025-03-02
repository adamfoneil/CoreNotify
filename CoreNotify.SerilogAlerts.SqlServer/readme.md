Schedule email alerts for Serilog data stored in SQL Server. Requires a [CoreNotify](https://github.com/adamfoneil/CoreNotify) account. Note that the email recipient must be your registered CoreNotify email address. You cannot send to arbitrary email addresses.

Scheduling capability is provided by [Coravel](https://github.com/jamesmh/coravel).

# Getting Started
1. Add CoreNotify to your project by installing NuGet package [CoreNotify.MailerSend](https://www.nuget.org/packages/CoreNotify.MailerSend), and setup your account as described in the CoreNotify readme linked above.
2. Decide on a strategy for managing the Serilog continuation marker. You can use the built-in class `SqlServerContinuationMarker` or implement your own. 
2. Add the **CoreNotify.SerilogAlerts.SqlServer** package to your project, and add the appropriate services to your startup.

<details>
	<summary>Example</summary>

	```csharp
	```

</details>


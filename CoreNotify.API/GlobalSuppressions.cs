// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1860:Avoid using 'Enumerable.Any()' extension method", Justification = "Any() is fine", Scope = "member", Target = "~M:CoreNotify.API.SerilogAlertService.ProcessResponseAsync(System.Net.Http.HttpResponseMessage,Services.Data.ApplicationDbContext,Services.Data.Entities.Webhook)~System.Threading.Tasks.Task{System.ValueTuple{System.String,System.Boolean}}")]

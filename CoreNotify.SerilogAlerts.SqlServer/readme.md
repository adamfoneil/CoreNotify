This is a minimal alert solution for Serilog data stored in SQL Server. There are two ways to use this:

- Standalone with your own email integration.
- With your CoreNotify account. You configure a webhook in your application to allow CoreNotify to query your Serilog data and generate alerts from that.

# Standalone use
In this approach, you handle the scheduling and email integration yourself. The service
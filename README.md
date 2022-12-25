# MusicServer


## appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultDBConnection": "YourConnectionString"
  },
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": "Debug",
    "WriteTo": [
      { "Name": "Console" } ,
      {
        "Name": "File",
        "Args": { "path": "C:\\Logs\\music_server.txt", "rollingInterval": "Day" }
      }
    ]
  },

  "AppSettings": {
      "AllowedCorsHosts": [ "localhost" ],
    "CookieExpirationTimeInMinutes":  60

  },
  "MailSettings": {

  },
  "FileserverSettings": {

  }
}


```
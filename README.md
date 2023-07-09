# MusicServer


## Configuration for the MusicServer
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
        "Host": "",
        "Email": "",
        "Password": "",
        "Port": "",
        "Sender": "Musicserver",
        "SendMails":  false
    },
  "FileserverSettings": {
        "SongFolder": "/songs",
        "ProfileCoverFolder": "/profiles",
        "AlbumCoverFolder": "/albums",
        "PlaylistCoverFolder": "/playlists"
  },
    "FileServerCredentials": {
        "UserName": "",
        "Password": "",
        "Host": "",
        "Port": 22
    },
}


```

## Configuration for the Importer

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
        "Args": { "path": "C:\\Logs\\music_server_importer.txt", "rollingInterval": "Day" }
      }
    ]
  },
  "FileserverSettings": {
      "SongFolder": "/songs",
      "ProfileCoverFolder": "/profiles",
      "AlbumCoverFolder": "/albums",
      "PlaylistCoverFolder": "/playlists"
  },
  "FileServerCredentials": {
      "UserName": "",
      "Password": "",
      "Host": "",
      "Port": 22
  },
  "MusicDataSettings": {
      "SourceFolder": "D:\\Music_FileServer\\Downloads",
      "MP3InfoPath": "C:\\Tools\\mp3info.exe"
  }
}


```
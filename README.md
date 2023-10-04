# MusicServer

## Why this project?

Firstly this project is done to get experience in the following technologies and frameworks: 
* Angular
* JS
* TS
* .NET
* Entity Framework
* SQL
* Usage of APIs

Secondly this problem should fix some common problems of the Spotify app.
Currently it is not possible for Spotify users to edit a shared playlist from a user.
There are also major problems for users when listening to the same song queue, because the more people are in the queue the easier it is to get randomly kicked. 

## Description

This project is a webapp which allows multiple users to stream their favorite music. 
It also lets them to create own playlists and share those with other users.
Furthermore these playlists can be made editable by those users.
Another use case of this app is to allow multiple users to listen to the same music queue and also manipulate said queue by adding and removing songs.

This project consists of the following 3 parts.

### Importer 

This program alows the admin to import existing songs from his pc into the webapp. 
Currently the importer only supports mp3 files and the files need to have correct id3 tags set for the track name, interpret and album.

### Backend

This is the hearth of the project and responsible for manipulating and accessing the data from the database.
The database that is used needs to be MSQL for the Entity Framework to work. 
And the song files as well as all the user and song images get accessed via SFTP.

### Frontend

The user interface to interact with the webapp over a browser.
Currently only desktop pc resolutions and desktop browsers are supported.

## Used packages and technologies

TODO

## Set up instructions

To use this project at home you first have to set up a MSSQL database via Entity Frameworks code first.
Fill out the configuration.json with your own parameters.
Import your own music via the Importer.
Start or deploy the frontend.
Start or deploy the backend.

To register new users the webapp uses registration codes.
These can easily be created via an api get call. 
But for the first user aka root you have to currently create a registration code in the database manually. 
Then register your root user and then assign the correct role to this user.
This is only needed for the Root user all other registered users will have the correct role.
This will be fixed in future updates.



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
      "AllowedCorsHosts": [ "localhost:4200" ],
    "CookieExpirationTimeInMinutes":  60,
    "FrontendAddress": "http://localhost:4200"

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
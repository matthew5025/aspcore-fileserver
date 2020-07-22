# What is this?
This is an [ASP.NET Core](https://github.com/dotnet/aspnetcore) app that allows the user to upload and download files.  
The upload file is uploaded to Google Drive. When the user wishes to download a file, it is retrieved from Google Drive and sent to the user.

# Any special features?
Yes, two main ones.  
1. All files uploaded are encrypted and automatically decrypted upon download.
2. Files are streamed for upload and download. What this means is that a file is never written to disk, and unless the file is extremely small, the entirety of the file is never in memory.  
This allows this app to support uploading of huge files even when it is ran from a server with limited resources.

# How do I use this?
You will need a [Google Service account](https://cloud.google.com/iam/docs/creating-managing-service-accounts#iam-service-accounts-create-console). This will be used to impersonate a user, and the files will be stored in a folder in the user's Google Drive.

### Edit FileServer/FileServer/appsettings.json  
1. Replace `insert-google-drive-folder-id-here` with the folder id where files should be placed.
2. Replace `path-to-service-creds-json-file` with the path of the service account credentials json file
3. Replace `user-account-files-will-go-to` with the user account to impersonate as.
4. Replace `sqlite-path` with the path the SQLite DB should be placed at.

### Edit /Client/js/upload.js
1. Replace `https://localhost:5001/api/Files` with the location of your web server.
   

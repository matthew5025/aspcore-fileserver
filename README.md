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

### Web Server Setup
Your HTTP server serving up the files in the Client folder should be reachable at <domain.com>, while the asp.net core server serving up the content in the Server folder should be reachable at the <domain.com>/api/ address.

- An example configuration for nginx is below

```    
server {
        listen       443 ssl http2;
        listen       [::]:443 ssl http2;
        server_name  domain.com;
        root         /usr/share/nginx/aspcore-fileserver/Client;
        ssl_certificate "/etc/pki/nginx/server.crt";
        ssl_certificate_key "/etc/pki/nginx/private/server.key";
        ssl_session_cache shared:SSL:1m;
        ssl_session_timeout  10m;
        ssl_ciphers PROFILE=SYSTEM;
        ssl_prefer_server_ciphers on;
        include /etc/nginx/default.d/*.conf;
        location / {
                index upload.html;
        }
        location /api/ {
                proxy_pass http://localhost:5000/api/;
                proxy_http_version 1.1;
                proxy_set_header   Upgrade $http_upgrade;
                proxy_set_header   Connection keep-alive;
                proxy_set_header   Host $host;
                proxy_cache_bypass $http_upgrade;
                proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
                proxy_set_header   X-Forwarded-Proto $scheme;
        }
        error_page 404 /404.html;
            location = /40x.html {
        }
        error_page 500 502 503 504 /50x.html;
            location = /50x.html {
        }
    }
```
   

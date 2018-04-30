# Hermit console client
This file will tell you how to configure your Hermit console client and give you some advice.  
[中文教程](README-zh.md)  

## Summary
Althought Hermit will automatically set all settings when Hermit run firstly, It is necessary that set some config before you run it firstly. Hermit will automatically deploy the worst config to avoid some idiot using this software.  
config.json should be written in advance. You can copy default json and modify it. You can see original json in config.json chapter.
AsymmetricEncryption's key should be set in advance. That file can be gotten from the owner of Hermit server which you want to join. After you get it, rename it to key.json and move it into Hermit console client's root folder.  

## config.json
This file describe each config which will be used in Hermit server.  
This is original json:  

```json
{
    "userName": "Test",
    "userPassword": "password",

    "server": "localhost",
    "port": "6161"
}
```

## key.json
This file include the key to connect server. If you don't put it in root folder, Hermit will remind you to do it.  
No key.json, no any connection.

## Command
Almost command which showed in Teji Protocol can be used in console client. And if you are admin of that server, you can run more commands than normal user. But here are some attached commands which can only be run in Hermit console client. Its aim is providing more flexible operation for Hermit console client.  
Here are attached commands for server:  
```
/cd /ChannelName
/cd ../
/cd /

/room
```

/cd just like that linux command which have the same name. And you can use it in linux's way. It is worthwhile to know that there are only 1 level "folder". So ```/cd /A/B``` is illegal. It will be changed when Hermit console client support multi-server login.  

/room will list all **loacl** room. It is dofferent from ```/ls room```. If you use ```/ls room```, it will pull a data from server and update local room list. At the same time, it will show some words in console.
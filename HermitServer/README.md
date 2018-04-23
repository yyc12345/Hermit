# Hermit server
This file will tell you how to configure your Hermit server and give you some advice.  
[中文教程](README-zh.md)  

## Summary
Althought Hermit will automatically set all settings when Hermit run firstly, It is necessary that set some config before you run it firstly. Hermit will automatically deploy the worst config to avoid some idiot using this software.  
Database will be created automatically. Don't place it in advance unless you want to move your old server.  
config.json should be written in advance. You can copy default json and modify it. You can see original json in config.json chapter.

## config.json
This file describe each config which will be used in Hermit server.  
This is original json:  

```json
{
    "userDatabasePath": "user.db",
    "roomDatabasePath": "room.db",
    "banDatabasePath": "ban.db",
    "emotionDatabasePath": "emotion.db",

    "ipv4Port": "8686",
    "ipv6Port": "6161"
}
```

Tips:  
Althought database's path is relative path in original json, Database's path not support relative url. So, using absolute path is necessary.

## Database
We use sqlite to be Hermit's database. Hermit will store 4 database files and each of them have unique function.

* user.db\(Store user information such as salt password or nickname\)
* room.db\(Store room's settings such as host or member\)
* ban.db\(Store ban list. Support Ip and user name\)
* emotion.db\(Store the association between emotion's name and its matched resources' Guid\)

Tips:  
You can use some sofeware to read it. But don't change it if you couldn't know its mean in advance. Hermit will not check data's legality when reading database. So your changing might led to some unknown issues.  

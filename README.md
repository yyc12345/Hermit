# Hermit
Light, free and secure instant-messaging\(IM\) software.

```
Hermit

A hermit is a person who lives in seclusion from society.
---Wikipedia

Just like a hermit to use Internet. Hide yourself.
```

## Unfinished project warning
This is a subsequent project of Dove Of Peace.  
This project is developing now. And the development are seperated into some steps. The progress of this project will be written in there.  
- [ ] Basic connection
- [ ] Message type
- [ ] Room and role
- [ ] Sticker set and reduce useless data
- [ ] Basic security
- [ ] Various security settings


## Description
This software **allow you** to setup your server which forward each messages.  
You can choose your suitable server type and deploy this on a server which you buy or lease.  

## This project

Packages:  

* Newtonsoft.Json 11.0.2 \(Installed in each repositories\)  
* System.ValueTuple 4.4.0 \(Installed in each repositories\)  
* Portable.BouncyCastle 1.8.2 \(Installed in each repositories\)  
* Microsoft.EntityFrameworkCore.Sqlite 2.0.2 \(Installed in HermitServer\)  

Language:C\# 7  
Basic library: .Net Core 2.0\(for core lib\) or .Net framework 4.5\(for WPF project\)

This project also include protocol called TejiProtocol. The Chinese protocol is always latest. But other language's protocol might is obsolute.  
If you couldn't read it. I suggest that you should use Google Translate to translate it and read it again.  

## Usage
```
git clone https://github.com/yyc12345/Hermit.git
cd ./HermitLib
dotnet restore
dotnet build -c release

//for server
cd ../HermitServer
dotnet restore
dotnet run

//for client
cd ../HermitClient
dotnet restore
dotnet build -c release
```

## Development
CHMOSGroup also accept other server which code by onther language. If you code a server followed by TejiProtocol, please tell us.

The count of client's type is not been limited. If you develop a new application for a new platform, you can use some way to tell us.  
Every client have their person in charge. If you want to make a issue or pull request, you must tell matched person in charge.  
There are all type of clients confirmed by CHMOSGroup.  

|Name|Person in charge|Platform|Language|
|:---:|:---:|:---:|:---:|
|-|-|-|-|

Each client which use TejiProtocol must comply with LICENSE. There are all of applications which break LICENSE and was banned by CHMOSGroup.  
At the same time, CHMOSGroup think this software must be use cautiously. CHMOSGroup make Dove Of Peace to be hard of using by design. The aim is that make sure **only** abilitiable people can use it. So the things that make DOP to be simple and easy to use is **NOT recommended and approval**. If you open your code, because of your behavior, CHMOSGroup couldn't blame you. But your application will still be written here and marked with Not recommended(The banned application will be marked with Illegal application).  

|Name|Developer|Platform|Language|Link|Reason|
|:---:|:---:|:---:|:---:|:---:|:---:|
|-|-|-|-|-|-|


## Release
The application developed by CHMOSGroup don't have any regular release date. When we have important update, we can release a new version.  
If you use another client developed by other people whom CHMOSGroup confirm, you can ask matched person in charge the date of newly released version directly.  

It is worth noting that CHMOSGroup's Dove Of Peace couldn't release binary file. You must only download the source code and compile it by yourself. CHMOSGroup don't recommand that The rest of the developers provide binary file either. But just like the above content. If you don't care this suggest, CHMOSGroup couldn't blame you. CHMOSGroup only can remind you the result of your behavior.  

## Relative project
This project is a implement of the abstract project "Secure IM software". This abstract project is originated from mysterious organisation called BKT. All of the BKT's member have their unique implement and solution of "Secure IM software". There are a list of their projects' link.

* [yyc12345/Hermit](https://github.com/yyc12345/Hermit)
* [ShadowPower/nekochat](https://github.com/ShadowPower/nekochat)
* [jxpxxzj/boss-im](https://github.com/jxpxxzj/boss-im)
* [jxpxxzj/ChinoIM](https://github.com/jxpxxzj/ChinoIM)

## Notification
This project's author need be attended in The Chinese National College Entrance Examination. So the development is **stopped**. If you want to do something. Everything will be done when I finish the exam. The date about 2018/6/8. Bless for myself, and I love this project.  

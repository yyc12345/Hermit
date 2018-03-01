# Teji Protocol
This document will intorduce Teji protocol which is used by Hermit.  
Teji protocol contain 2 layers, IRC layer and Security layer.  
Each layers is independent. So it can be remix freely.  

## IRC Layer
Teji protocol's IRC's function contain:  
* Room
* Various message
* Role\(Admin or user\)

### Room type
* Public\(Everyone can join in it without any permission\)
* Password\(Both invitation and password can join in room\)
* Private\(Only invitation can add new person\)
* Cache\(A room under shadow\)

> Each private messaging\(The communication only between you and your friend, 2 person\) will create a new **Cache** room for you and your friend\(If relative room hasn't been established\). You can destory this room whenever you like.

### Role
Admin have power to using some server command. Such as set black list or restart icr server.
User only have some general power. Such as pull/push file or set themselves' nickname. Also, if you are a owner of room, you can control this room's settings.

### Various message
Teji protocol's message have various types:
* Heart
* Shutdown
* Word
* FileHead
* FileBody
* Request
* Command
* Response
* Broadcast

Each message type's struct will be introduced in the feature...  

## Security Layer

Ths context will coming soon...


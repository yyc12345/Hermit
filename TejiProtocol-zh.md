# Teji Protocol
本文档主要介绍被用于Hermit的Teji Protocol  
Teji Protocol包含2层，类IRC层和安全层。每一个层在协议中都尽量保持了分离，偶尔有耦合较严重的部分  
全文将按一个正常连接的时间顺序进行讲解  
Protocol Version: 1.0 alpha

**目录：**  
* 握手
    * 握手1
    * 握手2
    * 握手3
    * 握手4
* 数据传输
    * 安全层
        * 基本格式
        * 熵增
        * 伪装与混淆
    * 类IRC层
        * 基本格式
        * DATA\_TYPE
            * HEART, SHUTDOWN
            * WORD, COMMAND, RESPONSE, BROADCAST
            * REQUEST
            * FILE\_HEAD
            * FILE\_BODY
* 断连
* 杂项说明
    * RTR
    * IP可信度
    * 超时断开
    * 加盐密码验证
    * 文件传输过程概述
    * 唯一标识符与Room机制
    * 加密方式json格式
    * 支持的加密方式以及其支持的扩展技术
    * 指令列表
        * ban, kick
        * room
        * user
        * ls
            * ban json
            * client json
            * room json
            * user json
        * 杂项指令

## 握手
1. Client向Server传输格式为**握手1**的数据，主要为请求用户的2个盐
2. Server审核Client的请求，如果确认可以连接，传输格式为**握手2**的数据
3. Client向Server返回经过计算的密码作为用户登录验证，传输格式为**握手3**的数据
4. Server确定密码正确之后，传输格式为**握手4**的数据，发送基本初始化数据（该用户所在的Room列表）
5. Client收到初始化数据之后，按数据初始化，然后进入下一步

>第2步确认请求的时候，检查的是IP可信度和黑名单（同时检测IP黑名单和UID黑名单），如果此IP没有记录就新建记录  
>对于目前版本的Teji Protocol来说，发起连接请求时，请使用Socket-SocketType.Stream-ProtocolType.Tcp。在之后的版本可能会使用RawSocket  

### 握手1

|标识符|简要|数据格式/长度|
|:---:|:---:|:---:|
|SIGN|标识符|Int32|
|DATA\_LENGTH|整个数据区长度|Int32|
|DATE\_STAMP|时间戳|Int64|
|HASH1|验证|byte\[256\]|
|-|-|-|
|CLIENT\_VER|客户端Protocol版本号|Int32|
|USER|用于申请盐的用户名|byte\[16\]|
|CACHE\_AES\_KEY|用于后续握手程序的临时AES密钥|byte\[256\]|
|CACHE\_SIGN|用于后续握手程序的临时SIGN|Int32|
|ENCRYPTION\_METHOD|用于正式通讯的加密算法列表|variable|
|HASH2|验证|byte\[256\]|

* 验证区块为SIGN至HASH1的部分，消息区块为CLIENT\_VER至HASH2的部分  
* 验证区块和消息区块**分别**使用服务器提供的公钥加密  
* 服务器的非对称加密的公私钥预先生成，公钥分发用户，私钥保留在服务器。建议定期更换密钥  
* 验证区块长度是定长的，由上述表格可以计算定长长度    
* SIGN必须为指定值，指定值钦定为6161，如果检测不到指定值，RTR  
* DATA\_LENGTH指的是整个消息区块加密后的长度  
* DATE\_STAMP是UNIX时间戳，UTC时间，当相差时间超过2s时判定重放攻击，RTR  
* HASH1用于对验证区块除了HASH1以外的部分取SHA256值；HASH2用于对消息区块除了HASH2以外的部分取SHA256值，所有握手阶段的Hash都钦定使用SHA256  
* CACHE\_AES\_KEY，握手剩余部分的使用的密钥，钦定使用AES-256-CBC模式，且IV为0  
* CLIENT\_VER表示客户端的Protocol版本号，版本号与Server不相同的时候，RTR  
* CACHE\_SIGN是随机生成的，用于后续验证中的包头检测，随机是为了防止有针对性的检测  
* ENCRYPTION\_METHOD是用于后续正式通讯的加密方法列表，本质上是一个json序列。由于Teji Protocol支持连续多次加密，所以才会设置此项来描述加密选项。在杂项说明中可以查阅此json的格式  
* 密钥共有2对。分别用于Server->Client和Client->Server数据传输，这两对秘钥初始化状态相同，中间工作时状态不一定相同  
* 如果给定错误或不存在的用户名，如果服务端开启了自动添加用户功能，自动注册此用户。如果没开启，RTR  

### 握手2

|标识符|简要|数据格式/长度|
|:---:|:---:|:---:|
|CACHE\_SIGN|标识符|Int32|
|DATA\_LENGTH|整个数据区长度|Int32|
|DATE\_STAMP|时间戳|Int64|
|HASH1|验证|byte\[256\]|
|-|-|-|
|SALT1|salt 1|byte\[128\]|
|SALT2|salt 2|byte\[128\]|
|HASH2|验证|byte\[256\]|

*TIPS：一些与上文重复的内容将不再重复说明*  

* 验证区块为SIGN至HASH1的部分，消息区块为SALT1至HASH2的部分  
* CACHE\_SIGN为握手1中消息区块所提供的CACHE\_SIGN  
* 验证区块和消息区块**分别**使用握手1提供的临时AES密钥加密   

### 握手3 

|标识符|简要|数据格式/长度|
|:---:|:---:|:---:|
|CACHE\_SIGN|标识符|Int32|
|DATA\_LENGTH|整个数据区长度|Int32|
|DATE\_STAMP|时间戳|Int64|
|HASH1|验证|byte\[256\]|
|-|-|-|
|PASSWORD|密码区块|variable|
|HASH2|验证|byte\[256\]|

* 验证区块为SIGN至HASH1的部分，消息区块为PASSWORD至HASH2的部分  
* 验证区块和消息区块**分别**使用握手1提供的临时AES密钥加密  
* PASSWORD区块由Client通过加盐密码验证算出来，计算方法可以参考杂项说明  
* 如果密码错误，则执行RTR，如果正确，登入。并对IP可信度进行对应的修改  

### 握手4

|标识符|简要|数据格式/长度|
|:---:|:---:|:---:|
|CACHE\_SIGN|标识符|Int32|
|DATA\_LENGTH|整个数据区长度|Int32|
|DATE\_STAMP|时间戳|Int64|
|HASH1|验证|byte\[256\]|
|-|-|-|
|ROOM\_LIST|Room列表|variable|
|HASH2|验证|byte\[256\]|

* 验证区块为SIGN至HASH1的部分，消息区块为ROOM\_LIST至HASH2的部分  
* 验证区块和消息区块**分别**使用握手1提供的临时AES密钥加密  
* ROOM\_LIST是一段json序列，描述当前用户可以访问的Room列表，相当于执行/ls room   

## 数据传输
握手完毕后，可以正式开始数据传输。数据的发送过程是：
1. Client先按照信息类型以类IRC层的格式组合数据
2. 数据转交给安全层进行包装，然后发送Server
3. Server通过安全层处理得到需要转发的消息，将消息中的一些必要数据补全，然后查询Room列表，再把消息交由不同用户的安全层转发（转发递交过去的数据应当是原始类IRC层数据，原始包头应当丢弃并由不同用户的安全层重新构建）出去，如果消息是针对Server的，进一步解析数据交给Server内核处理。  

>服务器不要求所有用户使用同一种加密方式，不同用户可以有不同的ENCRYPTION\_METHOD来进行加密

### 安全层
安全层是Teji Protocol的核心。在Hermit中，你不能不使用安全层就进行通讯，但在其他的衍生协议中，安全层可能不是必须的  
安全层的目的是使信息不被第三方解密，嗅探，重放攻击  
安全层的相关内容在当前版本的Teji Protocol中无法验证其是否符合上述要求，但是随着协议改进，对上述要求的实现会更加完善  

安全层的加密操作过程是：  
1. 使用基本格式和类IRC格式输出的原始数据构建基本数据
2. 基本数据递交给熵增，由熵增拆包以增加数据的熵值，规避统计学嗅探
3. 每个由熵增输出的数据包交给伪装与混淆来进行必要的操作后发包

#### 基本格式

|标识符|简要|数据格式/长度|
|:---:|:---:|:---:|
|CACHE\_SIGN|标识符|Int32|
|DATA\_LENGTH|整个CONTENT长度|Int32|
|DATE\_STAMP|时间戳|Int64|
|WAIT\_NEXT|等待包标识符|1 byte|
|-|-|-|
|CONTENT|数据区块|variable|

* 验证区块为SIGN至WAIT_NEXT的部分，消息区块为CONTENT部分  
* 验证区块和消息区块**分别**使用加密算法列表指定的加密算法进行加密  
* 验证区块长度是定长的，由上述表格可以计算定长长度  
* CACHE\_SIGN为握手1中所商定的值，如果检测不到指定值，RTR  
* DATA\_LENGTH指的是CONTENT加密后的长度  
* DATE\_STAMP是UNIX时间戳，UTC时间，当相差时间超过2s时判定重放攻击，RTR  
* WAIT\_NEXT指示是否等待下一个包。为了显得包的长度比较随机，Teji Protocol在某些规则的束缚下会拆包，此标识符如果设置为等待，则下一个包来了之后，其CONTENT将会被拼接在此包的CONTENT之后，以此类推，直到所有分片接受完毕再解密。此值设置为0表示等待，设置为1为不等待，检测到其余数值均RTR。  

#### 熵增
熵增的目的是增加数据包长度的混乱度使得第三方难以通过统计学统计流量长度来识别出你使用了Teji Protocol，进而干掉服务器以及客户端  
目前熵增采用随机数据区块长度，支持拆包但不支持并包，由此，包的长度绝对不可能小于等于包头长度，也不会出现非常大的包  
对于单个数据包的熵增操作实际上是对传递过来的基本格式中的消息区块拆分，然后分为多个包发送出去。对于消息区块的熵增遵循先加密后拆分而不是先拆分后加密，对于拆分的包，除了最后一包，其余包必须指定WAIT\_NEXT  

>熵增功能可以通过ENCRYPTION\_METHOD中的某些属性关闭

#### 伪装与混淆
**当前版本的Teji Protocol不支持此选项**  
由熵增输出的数据包，根据不同的伪装规则，伪装成不同的数据包进而发送出去。主要是避免第三方检测到Teji Protocol的使用  
在将来的某些版本中，Teji Protocol将加入一些伪装选项，也许包括但不仅限于：  
* TLS 1.2
* obfs
* http

>伪装与混淆功能可以通过ENCRYPTION\_METHOD中的某些属性指定不同的伪装类型或关闭。在当前版本的Teji Protocol中，此功能始终关闭，无论指定为何值

### 类IRC层
类IRC实现了一系列操作，类似于IRC但比IRC有更多的功能，类IRC表明了它不是对IRC协议的一个扩展或是实现，因此它不兼容IRC  

#### 基本格式

|标识符|简要|数据格式/长度|
|:---:|:---:|:---:|
|DATA\_TYPE|消息类型|Int32|
|ROOM\_UID|Room uid|byte\[16\]|
|DATE|发送时间|Int64|
|USER\_UID|发送者 uid|byte\[16\]|
|-|-|-|
|ACTUAL\_DATA|真正的数据|variable|

* DATA\_TYPE表示了这个消息的类型，这个值在DoveOfPeace内部会被转化为Enum处理  
* ROOM\_UID表示发往的Room的UID  
* DATE表示的是一个UNIX时间戳，UTC时间，可以直接照搬消息中的时间戳  
* USER\_UID表示发送者UID  
* ACTUAL\_DATA的内容与DATA\_TYPE相关，不同的DATA\_TYPE会有不同的ACTUAL\_DATA，格式在后文列出  
* 如果某个数据是可以省略的，请使用-1\(针对Int类型\)或者0\(byte类型\)填充相关区域  
* 此消息是所有消息的通用格式，但是对于Client->Server的数据包，DATE和USER\_ID是省略的。消息到达服务器后，如果是需要转发的数据，由服务器补齐这三个字段然后再转发  

#### DATA\_TYPE
当前版本的Teji Protocol支持以下的DATA\_TYPE，后文将介绍这些DATA\_TYPE对应的ACTUAL\_DATA格式  

|类型|标识符|
|:---:|:---:|
|心跳包|HEART|
|注销|SHUTDOWN|
|文本|WORD|
|文件推送请求|RRQUEST|
|文件头|FILE\_HEAD|
|文件主体|FILE\_BODY|
|命令|COMMAND|
|命令返回|RESPONSE|
|广播|BROADCAST|

##### HEART, SHUTDOWN
ACTUAL\_DATA不需要填写，或可以填写随机数据  

>SHUTDOWN指令发送后，会使Server直接断开连接而不是执行RTR断开  
>心跳包是双向的，即Client需要向Server表示存在，Server也要向Client表示自己没挂  

##### WORD, COMMAND, RESPONSE, BROADCAST

|标识符|简要|大小/数据格式|
|:---:|:---:|:---:|
|DATA|数据|variable|

>WORD, COMMAND, RESPONSE, BROADCAST都是基于文本的，这些文本需要用Unicode格式化作为DATA  
>BROADCAST, RESPONSE总是由Server->Client，且ROOM\_UID和USER\_ID是省略的，即RESPONSE, BROADCAST无视任何规则发布，不属于任何Room  
>COMMAND总是由Client->Server  
>RESPONSE的内容格式参阅杂项说明  

##### REQUEST
REQUEST用于传输文件之前协商传输事宜  

|标识符|简要|大小/数据格式|
|:---:|:---:|:---:|
|STATUS|状态|Int32|
|UNIQUE\_CODE|唯一标识符|byte\[256\]|

* STATUS指定该请求的类别，以下值是合法的：0（请求下载文件），1（请求上传文件），2（接受），3（拒绝），指定意外的值将会默认为忽略或拒绝

##### FILE\_HEAD
FILE\_HEAD是文件传输的起始  

|标识符|简要|大小/数据格式|
|:---:|:---:|:---:|
|SECTION\_COUNT|数据分片个数|Int32|
|WORKSPACE|位置|Int32|
|UNIQUE\_CODE|唯一标识符|byte\[256\]|

* SECTION\_COUNT表明文件被分为了多少片段，即将来接收的FILE消息的个数，表达此意思时数值必须大于等于1  
* WORKSPACE表明该数据属于哪一个位置，有效值为：0（临时文件），1（头像），2（表情），如果指定意外的值，将会默认为临时文件  
* UNIQUE\_CODE为对要发送文件取SHA256的结果，在后文的传输该文件的片段的时候，或者请求该文件的时候需要使用此数据  

##### FILE\_BODY

|标识符|简要|大小/数据格式|
|:---:|:---:|:---:|
|UNIQUE\_CODE|唯一标识符|byte\[256\]|
|SECTION\_POSITION|分片位|Int64|
|DATA|文件数据|variable|

>UNIQUE\_CODE表示这是哪一个资源的一部分  
>SECTION\_POSITION表示这个分片是从文件由头开始偏移多少开始读取的，处理的时候先偏移到流的指定位置，然后操作    

## 断连
正规通信中，通过发送SHUTDOWN数据包来合法地结束一段会话  
Client的掉线，丢包均会使Server对此次会话RTR  

## 杂项说明

### RTR
随机超时抵抗\(Random timeout resistance\)。为了防止服务器遭遇主动探测，我们使用随机超时抵抗来抵御。任何不是以正常方式对待连接的操作，都会被认为遭遇主动探测（某些情况下网络波动造成的丢包或包错位也会导致此结果）。RTR在此处的主要操作为：暂时不断开连接，在一个随机超时（30-60s）之后才断开，这个期间，任何有关这个连接的数据传输都会被强制屏蔽，并且由服务器在随机时间，发送随机长度，随机内容的垃圾数据包。垃圾数据包不经过任何安全层处理，不遵守任何协议，只要发就行  

### IP可信度
每个连接到Dove of peace服务器的IP都会被记录在案，并在第一次记录的时候给予一个初始值0。，当此IP每正确给予一次用户名和密码，此IP的可信度+1，给予错误的用户名和密码，IP可信度-1；当IP可信度\>=10时，此IP可被定义为常用IP，当IP可信度<-10时，此IP立即被加入黑名单  
IP可信度范围为-12~12，超过部分不再扣除或增加  
移入黑名单的IP不再拥有IP可信度，一律不允许连接  
当某IP从黑名单中移除时，此IP具有默认值为0的IP可信度  

### 超时断开
Teji Protocol要求Client在无消息传入1min之后自动直接断开连接  
遵守Teji Protocol协议的Server会在某客户端无消息传入45s后自动将此客户端转为RTR处理  
Teji Protocol会通过心跳包的机制，来保证连接不会断开。心跳包的间隔时间是随机的，在10s-30s间随机超时发送心跳包  

### 加盐密码验证
Client先请求Salt1和Salt2。请求完毕后Client做如下运算  
```
SaltHash1 = bcrypt(SHA256(password), uid + salt1, 10)
SaltHash2 = SHA256(SaltHash1 + uid + salt2)
```
然后使用SaltHash2作为加密密钥，使用AES-256-CBC模式加密如下拼接的数据，加密好的数据称为Ticket  
```uid + SaltHash1```  
加密好后发送到Server  
服务端按如下操作操作：服务端从数据库中找到该uid对应的SaltHash2，解密Ticket，得到SaltHash1，使用SaltHash1重新计算SaltHash2看是否和数据库中的SaltHash2一致，从而验证密码是否正确。  
这也就意味着服务器对于一个用户的存储需要以下字段：UID, Salt1, Salt2, SaltHash2  

### 文件传输过程概述
先由发送者向接收者发送REQUEST包，等待对方回应，如果拒绝，则取消发送，如果接受，就先发送FILE\_HEAD包，然后发送一系列FILE\_BODY包，接收者接收  
如果是用户向服务器提交文件，则需要额外的操作是自动生成一条来自发送文件的人的WORD消息，并发送到指定的Room，消息至少要提供这个文件的UNIQUE\_CODE。其余用户就这样得到文件已上传的消息，其余用户可以使用/pull UNIQUE\_CODE下载文件  

>表情的机制实质上是通过文件机制间接实现的，通过/pull对应的表情的UNIQUE\_CODE来下载表情，然后将一段文本中的某些字符替换为表情（参考Discord的表情机制） 
>成员头像由文件机制实现，由一段GUID指向一个图片文件，根据客户端是否需要显示头像而自行/pull   
>拒绝接受通常发生在接收方的本地存储中有与待接收文件相同UNIQUE\_CODE的文件  
>当用户/pull了一个无效的UNIQUE\_CODE后，Server会通过RESPONSE表明这一情况，如果UNIQUE\_CODE有效，Server会直接向这个用户发送REQUEST等待确认接收  
>文件分三个存储区，临时文件，头像，表情。三个存储区是分离的，且不能互相引用。这意味着如果你上传某张照片到头像区作为头像后，需要在上传一遍到表情区才能使你的照片既作为头像又作为表情来使用。

### 唯一标识符与Room机制
Teji Protocol采用GUID作为唯一标识，为方便使用会增加一些附属数值，这些数值仅仅用于展示给用户，真正传输的时候是传输GUID  
Teji Protocol采用Room制进行对话，无论是群组对话还是两个人的私聊，都会创建Room来实现  
一些名称（例如Room name / Nickname之类的）中不得包含一些字符，因为这些字符会被程序所引用，使用会导致一些不可逆的错误。不得包含的字符有  / \\ : " \* ? < \> \| \# -

### 加密方式json格式
下方将展示一个加密方式json的实例，我们通过实例来分析其写法
```json
{
    "useEntropy": true,
    "useObfuscation": false,

    "encryptionMethod": [
        {
            "name": "aes-256",
            "rotor": 2,
            "package": true,
            "keys": [
                "274794732ab3f23c13bf732ab3f11001274794732ab3f23c13bf732ab3f11001",
                "22ab3f1174794732ab2bf73794732ab3f2ab3f110043f23c133c13bf73001271"
            ],
            "parameter": [
                {
                    "name": "mode",
                    "value": "cbc"
                },
                {
                    "name": "IV",
                    "value": "00000000"
                }
            ]
        },
        {
            "name": "aes-512",
            "rotor": 1,
            "package": false,
            "keys": [
                "274794732ab3f23c13bf732a274794732ab3f23c13bf732ab3f11001274794732ab3f23c13bf732ab3f11001b3f11001274794732ab3f23c13bf732ab3f11001"
            ],
            "parameter": [
                {
                    "name": "mode",
                    "value": "cfb"
                },
                {
                    "name": "IV",
                    "value": "00000000"
                }
            ]
        }

    ]
}
```

* useEntropy和useObfuscation是必须的，在true和false中选填一个。useEntropy指示是否使用熵增，useObfuscation指示是否使用伪装与混淆  
* encryptionMethod描述了加密方法。可以添加多个加密方法Teji Protocol支持连续加密，加密的顺序是由数组最后一项提供的方法开始加密，直到加密到数组第一位。上一级的密文作为下一级的明文。数组第一位必须是支持AEAD的方法，无效的方法会被略过，AEAD方法验证的总是相对于其自己的原始文本而不是真正的原始文本。加密方法列表必须有项，如果想不使用任何加密，请填写后文介绍的raw加密方法  
* 每个加密方法中，name用于索引对应的加密方法，请指定Teji Protocol支持的方法。rotor指示为1表示不启用rotor机制（rotor机制请参考源代码，不好叙述），package表示是否启用包秘钥机制，包秘钥机制是将秘钥与上一包的hash进行xor操作，得到新的秘钥用于加密，如果密钥长于hash，无限重复hash直到长度一致，如果密钥短于hash，从高位向低位截取hash  
* package机制和rotor机制是Teji Protocol的扩展技术，需要对应的加密方法支持，否则设定无效，对于各种加密方法支持的扩展技术，可以在杂项说明中查看  
* keys描述了用于转子的秘钥列表，按顺序归入各个转子  
* parameter指示了一些参数，他是一个数组，其内部可以有多个参数类，每个参数类有name作为参数名称，value作为参数值，这些参数将对加密的一些初始化有作用。如果没指定完全参数，此加密方法会被忽略  
* 如果parameter没有参数需要填写，请使用```"placeHolder": ""```来占位，切勿留空  

### 支持的加密方式以及其支持的扩展技术
Teji Protocol支持以下类型的非对称加密  
* raw
* rsa-15360
* rsa-7680
* rsa-3072
* rsa-2048
* ~~mcEliece~~
* ~~ntru~~

Teji Protocol支持的对称加密和支持的扩展技术如下  

|加密类型|支持rotor\(T/F\)|支持package\(T/F\)|支持AEAD\(T/F\)|
|:---:|:---:|:---:|:---:|
|raw|T|T|T|
|aes-256|T|T|F|
|aes-512|T|T|F|
|aes-256-hmac-sha256|T|T|T|
|~~chacha20-poly1305~~|T|T|T|
|threeAes|T|T|F|

* aes-256, aes-512支持cbc, ecb, ofb模式

>对称与非对称加密的raw选项强烈不建议在实际过程中使用，仅当需要对类IM层进行测试时为减小不必要性能开销而使用，raw选项将不对数据做任何加密

### 指令列表
Teji Protocol中使用指令来让用户操控服务器设置，用户有不同的属性，admin和user，admin和在Server console中操作是等价的，user只能使用一些简单的服务器指令  
本部分仅仅适用于基本的指令，对于一些Teji Protocol的Console实现，为了更好地操作，可能会增加一些操作指令，而那些指令不在本部分叙述范围之内  
命令前标识有[admin]的需要admin权限或直接由Server console输入,标识有[limited]说明此指令限制特定人使用，[admin]优先级高于[limited]，即admin可以执行所有指令  

#### ban, kick

```
[admin]/ban IP_ADDRESS
[admin]/ban USER_NAME
[admin]/kick USER_NAME
```

* 对已经ban的IP或user再执行ban将解除其被ban状态

#### room

```
/room add NAME TYPE
/room del NAME
/room join ROOM_NAME USER_NAME [PASSWORD]
[limited]/room invte ROOM_NAME USER_NAME
[limited]/room kick ROOM_NAME USER_NAME
[limited]/room power ROOM_NAME USER_NAME
```

* 对于添加room时的type有4种支持的类型：public（开放的，所有人都可以自由加入），password（可以通过邀请或者输入密码加入），private（只有通过邀请才能加入）。每一个私密对话（好友2人之间）将会自动创建一个private类型的房间（如果没有建立的话）
* 销毁房间将会同时销毁此房间的消息记录
* join是针对所有人加入房间所设定的指令，对于有密码的房间需要提供密码。invite指令是针对已经在某个房间的人邀请其他人进入房间，Server在操作时需要验证这一点
* kick指令只有房主可以使用，如果房主离开房间，房主权限将随机转让
* power指令只有房主可以使用，用于转让房主权限，必须指定已在房间的人

#### user

```
/user add NAME PASSWORD
/user del NAME
[limited]/user change NAME NEW_PASSWORD
[limited]/user nickname NAME NEW_NICKNAME
[limited]/user avatar NAME AVATAR_GUID
```

* change用于改变用户的密码，由用户本人操作或admin操作才是合法的
* nickname用于修改昵称，只有用户本人或admin操作才是合法的
* avatar用于更改头像，只有用户本人或admin操作才是合法的。AVATAR_GUID可以为空

#### ls

```
[admin]/ls ban
/ls user
[admin]/ls user all
[admin]/ls client
/ls room
[admin]/ls room all
```

* ban会列出黑名单列表
* user会列出当前在线用户列表，指定all列出所有用户列表
* client会列出当前所有连接到服务器的client
* room列出当前用户可以访问的room，指定all列出所有room

ls指令会使用RESPONSE消息，用json序列返回结果，下面列出了各类json在服务端的格式，传输到客户端的数据会做部分舍去，具体舍去内容会在下方表明  
服务端所用json格式指示列出了必须要有的字段，而实际上在服务端存储某一类别数据的时候可能不会将其作为json保存，而是用Mysql之类的大型数据库存储。并且有些数据是不需要存储的（例如client列表）  

##### ban json

```json
[
    {
        "type": "ip",
        "value": "192.168.0.0"
    },
    {
        "type": "user",
        "value": "sampleName"
    }
]
```

>客户端与服务端数据统一

##### client json

```json
[
    {
        "ip": "192.168.0.0",
        "port": 6161,
        "status": "rtr"
    },
    {
        "ip": "192.168.0.1",
        "port": 8181,
        "status": "handshake"
    },
    {
        "ip": "192.168.0.2",
        "port": 1973,
        "status": "normal"
    }
]
```

>客户端与服务端数据统一

##### room json

```json
[
    {
        "name": "general",
        "type": "public",
        "password": "",
        "host": "bl",
        "users": [
            "bl",
            "yyc"
        ]
    },
    {
        "name": "dev",
        "type": "password",
        "password": "teji",
        "host": "jx",
        "users": [
            "chris",
            "jx"
        ]
    },
    {
        "name": "nsfw-driverschool",
        "type": "private",
        "password": "",
        "host": "61",
        "users": [
            "61",
            "oing"
        ]
    }
]
```

>客户端舍去password字段

##### user json

```json
[
    {
        "name": "bl",
        "nickname": "blblb",
        "isAdmin": true,
        "avatarGuid": "3c13bf732ab3f110012727479473bf732ab3f1100132ab3f24794732ab3f23c1",
        
        "salt1": "c13bf732ab3f1274794732ab3f23100127411001794732ab3f23c13bf732ab3f",
        "salt2": "13bf732ab3f1100794732ab327423cf127473f1100194732ab3f23c13bf732ab",
        "saltHash": "b3f1100132ab3f11001274723274794732ab394732ab3ff23c13bf7c13bf732a"
    }
]
```

>客户端舍去salt1, salt2, saltHash字段

#### 杂项指令

```
/pull UNIQUE_CODE
[admin]/wipe
[admin]/admin NAME
[admin]/broadcast WORDS
[admin]/shutdown [TIMEOUT]
```

* pull可以拉取对应的资源到本地，需要提供对应的资源编号
* wipe负责擦除数据，wipe只会擦除临时文件中的数据和所有历史记录，对于表情和头像中的数据仍然保留，执行此指令会使服务器立即重启，且需要在清理完成后才允许再次连接服务器  
* admin赋予指定人管理员权限，再执行一遍即可撤销管理员权限
* broadcast向所有加入服务器的人广播一条消息
* shutdown用于强制关闭服务器，可以指定以毫秒为单位的超时

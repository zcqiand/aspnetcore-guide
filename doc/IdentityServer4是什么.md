# 1 什么是IdentityServer4？
IdentityServer4是用于ASP.NET Core的OpenID Connect和OAuth 2.0框架。

# 2 什么是OAuth 2.0？
OAuth不是一个API或者服务，而是一个授权(Authorization)的开放标准，OAuth2.0是目前广泛使用的版本。

## 2.1 4种角色
* 资源所有者（Resource Owner），又称"用户"。
* 资源服务器（Resource Server），即处理各种业务服务的服务器。
* 客户端（Client）。
* 授权服务器（Authorization Server），即处理授权服务的服务器。

## 2.2 4种授权方式
* 授权码（authorization code）
* 隐式授权（implicit）
* 资源所有者密码凭据（resource owner password credentials）
* 客户端凭据（client credentials）

### 2.2.1 授权码（authorization code）
下图说明了授权码的工作流程。

![image](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210322/20210323145817.png)

（A）客户端通过向授权端点引导资源所有者的用户代理开始流程。客户端包括它的客户端标识、请求范围、本地状态和重定向URI，一旦访问被许可（或拒绝）授权服务器将传送用户代理回到该URI。
```
GET /authorize?response_type=code&client_id=s6BhdRkqt3&state=xyz&redirect_uri=https%3A%2F%2Fclient%2Eexample%2Ecom%2Fcb HTTP/1.1
Host: server.example.com
```
参数说明：
* response_type，表示授权类型，必选项，值必须被设置为“code”。
* client_id，表示客户端标识，必选项
* redirect_uri,表示重定向URL，可选项
* scope，可选的，访问请求的范围，可选项
* state，客户端用于维护请求和回调之间的状态的不透明的值，用于防止跨站点请求伪造，可选项

（B）授权服务器验证资源拥有者的身份（通过用户代理），并确定资源所有者是否授予或拒绝客户端的访问请求。

（C）假设资源所有者许可访问，授权服务器使用之前（在请求时或客户端注册时）提供的重定向URI重定向用户代理回到客户端。重定向URI包括授权码和之前客户端提供的任何本地状态。
```
HTTP/1.1 302 Found
Location: https://client.example.com/cb?code=SplxlOBeZQQYbYS6WxSbIA&state=xyz
```
参数说明：
* code，表示授权服务器生成的授权码，必选项。推荐的最长的授权码生命周期是10分钟。客户端不能使用授权码超过一次。如果一个授权码被使用一次以上，授权服务器必须拒绝该请求并应该撤销（如可能）先前发出的基于该授权码的所有令牌。
* state，若客户端的请求中包含这个参数，服务器的返回也必须一样包含这个参数。

（D）客户端通过包含上一步中收到的授权码从授权服务器的令牌端点请求访问令牌。当发起请求时，客户端与授权服务器进行身份验证。客户端包含用于获得授权码的重定向URI来用于验证。
```
POST /token HTTP/1.1
Host: server.example.com
Authorization: Basic czZCaGRSa3F0MzpnWDFmQmF0M2JW
Content-Type: application/x-www-form-urlencoded
grant_type=authorization_code&code=SplxlOBeZQQYbYS6WxSbIA&redirect_uri=https%3A%2F%2Fclient%2Eexample%2Ecom%2Fcb
```
参数说明：
* grant_type，表示使用的授权模式，必选项。值必须被设置为“authorization_code”，必选项。
* code，表示从授权服务器收到的授权码，必选项。
* redirect_uri，表示重定向URI，必选项。若“redirect_uri”参数包含在授权请求中，且他们的值必须相同。
* client_id，表示客户端标识，必选项。

（E）授权服务器对客户端进行身份验证，验证授权代码，并确保接收的重定向URI与在步骤（C）中用于重定向（资源所有者的用户代理）到客户端的URI相匹配。如果通过，授权服务器响应返回访问令牌与可选的刷新令牌。
```
HTTP/1.1 200 OK
Content-Type: application/json;charset=UTF-8
Cache-Control: no-store
Pragma: no-cache
{
  "access_token":"2YotnFZFEjr1zCsicMWpAA",
  "token_type":"example",
  "expires_in":3600,
  "refresh_token":"tGzv3JOkF0XG5Qx2TlKWIA",
  "example_parameter":"example_value"
}
```
参数说明：
* access_token：表示访问令牌，必选项。
* token_type：表示令牌类型，该值大小写不敏感，必选项，可以是bearer类型或mac类型。
* expires_in：表示过期时间，单位为秒。如果省略该参数，必须其他方式设置过期时间。
* refresh_token：表示更新令牌，用来获取下一次的访问令牌，可选项。
* scope：表示权限范围，如果与客户端申请的范围一致，此项可省略。

### 2.2.2 隐式授权（implicit）
下图说明了隐式授权的工作流程。

![image](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210322/20210323153850.png)

（A）客户端通过向授权端点引导资源所有者的用户代理开始流程。客户端包括它的客户端标识、请求范围、本地状态和重定向URI，一旦访问被许可（或拒绝）授权服务器将传送用户代理回到该URI。
```
GET /authorize?response_type=token&client_id=s6BhdRkqt3&state=xyz&redirect_uri=https%3A%2F%2Fclient%2Eexample%2Ecom%2Fcb HTTP/1.1
Host: server.example.com
```
参数说明：
* response_type，表示授权类型，必选项，值必须被设置为“token”。
* client_id，表示客户端标识，必选项
* redirect_uri,表示重定向URL，可选项
* scope，访问请求的范围，可选项
* state，客户端用于维护请求和回调之间的状态的不透明的值，用于防止跨站点请求伪造，可选项

（B）授权服务器验证资源拥有者的身份（通过用户代理），并确定资源所有者是否授予或拒绝客户端的访问请求。

（C）假设资源所有者许可访问，授权服务器使用之前（在请求时或客户端注册时）提供的重定向URI重定向用户代理回到客户端。重定向URI在URI片段中包含访问令牌。
```
HTTP/1.1 302 Found
Location: http://example.com/cb#access_token=2YotnFZFEjr1zCsicMWpAA&state=xyz&token_type=example&expires_in=3600
```
参数说明：
* access_token：表示访问令牌，必选项。
* token_type：表示令牌类型，该值大小写不敏感，必选项。
* expires_in：表示过期时间，以秒为单位的访问令牌生命周期。例如，值“3600”表示访问令牌将在从生成响应时的1小时后到期。如果省略，则授权服务器应该通过其他方式提供过期时间，或者记录默认值。
* scope：表示权限范围，如果与客户端申请的范围一致，此项可省略。
* state：若客户端的请求中包含这个参数，服务器的返回也必须一样包含这个参数。

（D）用户代理顺着重定向指示向Web托管的客户端资源发起请求（按RFC2616该请求不包含片段）。用户代理在本地保留片段信息。

（E）Web托管的客户端资源返回一个网页（通常是带有嵌入式脚本的HTML文档），该网页能够访问包含用户代理保留的片段的完整重定向URI并提取包含在片段中的访问令牌（和其他参数）。

（F）用户代理在本地执行Web托管的客户端资源提供的提取访问令牌的脚本。

（G）用户代理传送访问令牌给客户端。

### 2.2.3 资源所有者密码凭据（resource owner password credentials）
下图说明了资源所有者密码凭据的工作流程。

![image](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210322/20210323154909.png)

（A）资源所有者提供给客户端它的用户名和密码。

（B）通过包含从资源所有者处接收到的凭据，客户端从授权服务器的令牌端点请求访问令牌。当发起请求时，客户端与授权服务器进行身份验证。
```
POST /token HTTP/1.1
Host: server.example.com
Authorization: Basic czZCaGRSa3F0MzpnWDFmQmF0M2JW
Content-Type: application/x-www-form-urlencoded
grant_type=password&username=johndoe&password=A3ddj3w
```
参数说明：
* grant_type，表示授权类型，必选项，值必须设置为“password”。
* username，表示资源所有者的用户名，必选项。
* password，表示资源所有者的密码，必选项。
* scope，表示权限范围，可选项

（C）授权服务器对客户端进行身份验证，验证资源所有者的凭证，如果有效，颁发访问令牌。
```
HTTP/1.1 200 OK
Content-Type: application/json;charset=UTF-8
Cache-Control: no-store
Pragma: no-cache
{
  "access_token":"2YotnFZFEjr1zCsicMWpAA",
  "token_type":"example",
  "expires_in":3600,
  "refresh_token":"tGzv3JOkF0XG5Qx2TlKWIA",
  "example_parameter":"example_value"
}
```
参数说明：
* access_token：表示访问令牌，必选项。
* token_type：表示令牌类型，该值大小写不敏感，必选项，可以是bearer类型或mac类型。
* expires_in：表示过期时间，单位为秒。如果省略该参数，必须其他方式设置过期时间。
* refresh_token：表示更新令牌，用来获取下一次的访问令牌，可选项。
* scope：表示权限范围，如果与客户端申请的范围一致，此项可省略。

### 2.2.4 客户端凭据（client credentials）
下图说明了客户端凭据的工作流程。

![image](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210322/20210323155450.png)

（A）客户端与授权服务器进行身份验证并向令牌端点请求访问令牌。
```
POST /token HTTP/1.1
Host: server.example.com
Authorization: Basic czZCaGRSa3F0MzpnWDFmQmF0M2JW
Content-Type: application/x-www-form-urlencoded
grant_type=client_credentials
```
参数说明：
* grant_type，表示授权类型，必选项，值必须设置为“client_credentials”。
* scope，表示权限范围，可选项

（B）授权服务器对客户端进行身份验证，如果有效，颁发访问令牌。
```
HTTP/1.1 200 OK
Content-Type: application/json;charset=UTF-8
Cache-Control: no-store
Pragma: no-cache
{
  "access_token":"2YotnFZFEjr1zCsicMWpAA",
  "token_type":"example",
  "expires_in":3600， "example_parameter":"example_value"
}
```
参数说明：
* access_token：表示访问令牌，必选项。
* token_type：表示令牌类型，该值大小写不敏感，必选项，可以是bearer类型或mac类型。
* expires_in：表示过期时间，单位为秒。如果省略该参数，必须其他方式设置过期时间。

# 3 什么是OpenID Connect？
OpenID Connect是由OpenID基金会于2014年发布的一个开放标准，简称OIDC, 它是基于OAuth 2.0协议的简单身份层。它允许客户端根据授权服务器执行的身份验证来验证最终用户的身份，并以可互操作且类似于REST的方式获取有关最终​​用户的基本配置文件信息。

OpenID Connect允许所有类型的客户端（包括基于Web的客户端，移动客户端和JavaScript客户端）请求并接收有关经过身份验证的会话和最终用户的信息。该规范套件是可扩展的，允许参与者在对他们有意义的时候使用可选功能，例如身份数据加密，OpenID提供程序的发现以及会话管理。

OpenID是Authentication，即认证，对用户的身份进行认证，判断其身份是否有效，也就是让网站知道“你是你所声称的那个用户”；

OAuth是Authorization，即授权，在已知用户身份合法的情况下，经用户授权来允许某些操作，也就是让网站知道“你能被允许做那些事情”。

```
(身份验证)+ OAuth 2.0 = OpenID Connect
```

OpenID Connect是“认证”和“授权”的结合，因为其基于OAuth协议，所以OpenID-Connect协议中也包含了client_id、client_secret还有redirect_uri等字段标识。这些信息被保存在“身份认证服务器”，以确保特定的客户端收到的信息只来自于合法的应用平台。

## 3.1 相关定义
* EU：End User，用户。
* RP：Relying Party ，用来代指OAuth2中的受信任的客户端，身份认证和授权信息的消费方；
* OP：OpenID Provider，有能力提供EU身份认证的服务方（比如OAuth2中的授权服务），用来为RP提供EU的身份认证信息；
* ID-Token：JWT格式的数据，包含EU身份认证的信息。
* UserInfo Endpoint：用户信息接口（受OAuth2保护），当RP使用ID-Token访问时，返回授权用户的信息，此接口必须使用HTTPS。


## 3.2 OIDC流程
OIDC是遵循OAuth协议流程，在申请Access-Token的同时，也返回了ID-Token来验证用户身份。

如果是JS应用，其所有的代码都会被加载到浏览器而暴露出来，没有后端可以保证client_secret的安全性，则需要是使用默认模式流程(Implicit Flow)。

如果是传统的客户端应用，后端代码和用户是隔离的，能保证client_secret的不被泄露，就可以使用授权码模式流程（Authentication Flow）。

此外还有混合模式流程(Hybrid Flow)，简而言之就是以上二者的融合。

## 3.2.1 授权码模式流程（Authentication Flow）
和OAuth认证流程类似
```
RP发送一个认证请求给OP，其中附带client_id；
OP对EU进行身份认证；
OP返回响应，发送授权码给RP；
RP使用授权码向OP索要ID-Token和Access-Token，RP验证无误后返回给RP；
RP使用Access-Token发送一个请求到UserInfo EndPoint；UserInfo EndPoint返回EU的Claims。
```

## 3.3 安全令牌 ID-Token
OIDC对OAuth2最主要的扩展就是提供了ID-Token，下面我们就来看看ID-Token的主要构成：

* iss = Issuer Identifier：必须。提供认证信息者的唯一标识。一般是Url的host+path部分；
* sub = Subject Identifier：必须。iss提供的EU的唯一标识；最长为255个ASCII个字符；
* aud = Audience(s)：必须。标识ID-Token的受众。必须包含OAuth2的client_id；
* exp = Expiration time：必须。ID-Token的过期时间；
* iat = Issued At Time：必须。JWT的构建的时间。
* auth_time = AuthenticationTime：EU完成认证的时间。如果RP发送认证请求的时候携带max_age的参数，则此Claim是必须的。
* nonce：RP发送请求的时候提供的随机字符串，用来减缓重放攻击，也可以来关联ID-Token和RP本身的Session信息。
* acr = Authentication Context Class Reference：可选。表示一个认证上下文引用值，可以用来标识认证上下文类。
* amr = Authentication Methods References：可选。表示一组认证方法。
* azp = Authorized party：可选。结合aud使用。只有在被认证的一方和受众（aud）不一致时才使用此值，一般情况下很少使用。
```
{
   "iss": "https://server.example.com",
   "sub": "24400320",
   "aud": "s6BhdRkqt3",
   "nonce": "n-0S6_WzA2Mj",
   "exp": 1311281970,
   "iat": 1311280970,
   "auth_time": 1311280969,
   "acr": "urn:mace:incommon:iap:silver"
}
```
# Another-Mirai-Native2

交流群: 671467200


## 协议实现进度
- [x] 实现[MiraiAPIHttp](https://github.com/project-mirai/mirai-api-http)协议
- [x] 实现[OneBot](https://github.com/botuniverse/onebot-11)协议
- [x] 实现[Satori](https://satori.js.org/zh-CN/introduction.html)协议
- [x] 实现[Lagrange.Core](https://github.com/LagrangeDev/Lagrange.Core)协议
- [ ] 实现官方协议
- [ ] 实现OPQBot协议

以下仅代表本框架实现程度，具体能否发送或调用还需要看协议实现方是否实现了此功能

<details>
  <summary>协议API可用情况</summary>

  ## 协议API可用情况
| | 撤回消息 | Cookie | CsrfToken | 好友列表 | 群组信息 | 群组列表 | 群成员信息 | 群成员列表 | 账号昵称 | 账号 ID | 获取陌生人信息 |发送群组信息  | 发送名片赞 | 发送单聊信息 | 发送讨论组信息 | 主动离开讨论组 | 处理好友添加请求 | 处理群组添加请求 | 设置群管理 | 设置群组匿名 | 禁言群匿名成员 | 禁言群成员 | 设置群组成员名片 | 移除群组成员 | 主动离开群组 |  设置群组成员头衔| 设置群组全员禁言 |
| -- | -- | -- | -- | -- | -- | -- | -- | -- | -- | -- | -- |--  | -- | -- | -- | -- | -- | -- | -- | -- | -- | --| -- | -- | -- |  --| -- |
| MiraiApiHttp | ⭕ | ❌ | ❌ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ❓ |⭕  | ❌ | ⭕ | ❌ | ❌ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ |  ⭕| ⭕ |
| OneBot v11 | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ |⭕  | ⭕ | ⭕ |⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ |  ⭕| ⭕ |
| Satori v1 | ⭕ | ❌ | ❌ | ⭕ | ⭕ | ⭕ | ❓ | ❓ | ⭕ | ⭕ | ❌ |⭕  | ❌ | ⭕ | ❌ | ❌ | ⭕ | ⭕ | ❌ | ❌ | ❌ | ⭕ | ❌ | ⭕ | ⭕ |  ❌| ⭕ |
| Lagrange.Core | ⭕ | ⭕ | ❌ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ |⭕  | ⭕ | ⭕ |❌ | ❌ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ | ⭕ |  ⭕| ⭕ |

## 协议CQ码可发送情况
||MiraiAPIHttp|OneBot v11|Satori v1|Lagrange.Core|
|--|--|--|--|--|
|face|⭕|⭕|⭕|⭕|
|image|⭕|⭕|⭕|⭕|
|record|⭕|⭕|⭕|⭕|
|at|⭕|⭕|⭕|⭕|
|dice|⭕|⭕|❌|❌|
|music|⭕|⭕|❌|❌|
|rich|⭕|⭕|❌|❌|
|reply|⭕|⭕|❌|⭕|

## 协议CQ码可解析情况
||MiraiAPIHttp|OneBot v11|Satori v1|Lagrange.Core|
|--|--|--|--|--|
|face|⭕|⭕|⭕|⭕|
|bigface|⭕|⭕|❌|⭕|
|image|⭕|⭕|⭕|⭕|
|flashimage|⭕|⭕|❌|⭕|
|record|⭕|⭕|⭕|⭕|
|at|⭕|⭕|⭕|⭕|
|atall|⭕|⭕|❌|⭕|
|dice|⭕|⭕|❌|❌|
|music|⭕|⭕|❌|❌|
|xml|⭕|⭕|❌|⭕|
|json|⭕|⭕|❌|⭕|
|app|⭕|⭕|❌|⭕|
|rich|⭕|⭕|❌|⭕|
|reply|⭕|⭕|❌|⭕|
|poke|⭕|⭕|❌|❌|

</details>

## 关于 Lagrange.Core
- 登录逻辑借鉴于Lagrange.Onebot项目
- 仅.net8.0版本可用
- 若控制台输出的二维码无法扫码，可修改`conf\Config.json`配置中的`QRCodeCompatibilityMode`为true
- 协议端会可能有很多的异常输出，正常现象
- 若需要更新Lagrange.Core版本，查看[Wiki](https://github.com/Hellobaka/Another-Mirai-Native2/wiki/%E6%9B%B4%E6%96%B0Lagrange.Core%E7%89%88%E6%9C%AC)

## 可加载插件
- [x] 酷Q
- [x] [小栗子](https://github.com/Hellobaka/Another-Mirai-Native2/wiki/%E5%8A%A0%E8%BD%BD%E5%B0%8F%E6%A0%97%E5%AD%90%E6%8F%92%E4%BB%B6)

## 音频支持
- [音频支持](https://github.com/Hellobaka/Another-Mirai-Native2/wiki/%E9%9F%B3%E9%A2%91%E6%94%AF%E6%8C%81)

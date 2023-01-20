# BiliApi
此仓库为ASP.NET Core Web Api项目，请自行部署。
## 动态 Dynamic
该api用于获取指定uid用户最新的动态<br>
请求方法：GET<br>
请求参数：<br>

| 参数名称 | 类型 | 是否必须 |
| ------ | ------ | ------ |
| uid | string | √ |

返回结果：

| 参数名称 | 类型 | 介绍 |
| ------ | ------ | ------ |
| time | string | 对应动态上的时间 |
| guid | string | 暂时没用，对应本地resource/dynamic/{guid}.png |
| snapshot | string | 动态快照的base64编码 |

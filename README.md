# 音乐管理项目开发文档

## DDD 建模
### 统一语言
* 用户：指听音乐的人。
* 音乐信息：包括音乐标题、专辑名、歌手、文件位置。
* 歌单：每个用户有多个歌单，一个歌单有多个歌曲。
* 存储：一个本地文件目录，系统将会扫描这个文件夹，更新音乐库的信息。

### 功能描述

### 聚合根
* 用户认证
    * ASP\.NET Identity 的 7 个实体。
* 音乐信息
    * 音乐信息实体
    * 用户歌单实体
* 音乐管理
    * 存储实体
    * 任务实体

### 领域事件
* 用户注册时，创建默认歌单，里面包含随机的歌曲。
* 在音乐管理聚合根中，读取存储中的音乐文件的信息，通过领域事件，通知音乐信息聚合根，为音乐信息表添加或更新元组。

## 项目结构图
```mermaid
---
title: 项目结构图
---
flowchart TD
    WebApi --> Application
    WebApi --> Infrastructure
    Application --> Abstractions
    Infrastructure --> Abstractions
    Abstractions --> Domain
```

## 简化的 ER 图
```mermaid
---
title: 简化的 ER 图
---
erDiagram
    AspNetUserRoles }|--|| AspNetRoles : ""
    AspNetRoleClaims }|--|| AspNetRoles : ""
    AspNetUserClaims }|--|| AspNetUsers : ""
    AspNetUsers ||--|{ AspNetUserTokens : ""
    AspNetUserRoles }|--|| AspNetUsers : ""
    AspNetUsers ||--|{ AspNetUserLogins : ""

    style AspNetUserRoles fill:#ff99ff
    style AspNetRoleClaims fill:#ff99ff
    style AspNetRoles fill:#ff99ff
    style AspNetUserClaims fill:#ff99ff
    style AspNetUsers fill:#ff99ff
    style AspNetUserLogins fill:#ff99ff
    style AspNetUserTokens fill:#ff99ff

    AspNetUsers ||--|{ MusicList : ""
    MusicListMusicInfoMap }|--|| MusicList : ""
    MusicInfo ||--|{ MusicListMusicInfoMap : ""

    style MusicListMusicInfoMap fill:#fff666
    style MusicList fill:#fff666
    style MusicInfo fill:#fff666

    Job
    Storage
```

## WebApi
### 管理员
#### 音乐管理
##### 添加存储

##### 删除存储

##### 更新存储

##### 查看所有存储

##### 创建任务

##### 查看所有任务

##### 取消任务

### 用户认证
#### 用户注册

#### 用户登录

#### 用户登出

### 主业务
#### 创建歌单

#### 删除歌单

#### 修改歌单

#### 查询所有歌单

#### 为歌单添加歌曲

#### 从歌单删除歌曲

#### 查询歌单的歌曲

#### 浏览所有歌曲（搜索歌曲）

#### 获取歌曲音频流

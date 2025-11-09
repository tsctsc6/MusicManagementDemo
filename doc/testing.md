## 单元测试方法
由于本项目使用了 pgsql 的特色功能，所以不能使用 Microsoft.EntityFrameworkCore.InMemory 进行测试。

本项目的单元测试方法是，通过把内存映射为虚拟硬盘，把测试用的数据库保存在虚拟硬盘中，加快速度的同时，还能避免物理硬盘的过度使用。

> [!warning]
> 本文档仅说明 Windows 平台的挂载虚拟硬盘方法。

### 挂载虚拟硬盘
首先下载 [ImDisk](https://sourceforge.net/projects/imdisk-toolkit/files/) 。

运行 [DeployScript/testing/add-ram-disk.ps1](../DeployScript/testing/add-ram-disk.ps1) 。

> [!warning]
> 本项目中的 DeployScript/testing 中的脚本，需要 Windows 11 24H2 及以上。

### 创建 pgsql 的 tablespace
简单来说， tablespace 是 pgsql 保存数据库的物理位置。

使用 [DeployScript/testing/add-pg-ram-tablespace.ps1](../DeployScript/testing/add-pg-ram-tablespace.ps1) 创建 tablespace 。

更多相关命令，查看 [DeployScript/testing/pg-ram-disk.txt](../DeployScript/testing/pg-ram-disk.txt)

### 编写 appsetting
单元测试的 appsetting 和平常略有不同。模板如下：

```json
{
  "ConnectionStrings": {
    "Default": "Host=127.0.0.1;Port=5432;Username=postgres;Password=*;",
    "Postgres": "Host=127.0.0.1;Port=5432;Database=postgres;Username=postgres;Password=*;"
  },
  "VirtualTableSpace": "ram_tablespace",
  "DbName": "functional_testing",
  "Jwt": {
    "Key": "*",
    "Issuer": "Functional_Testing",
    "Audience": "Functional_Testing"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

其中， `ConnectionStrings::Default` 没有数据库名称，这是因为测试程序会自动生成随机数据库名称，不需要指定。

### 清理
如果调试单元测试，且中途停止，测试数据库不会清理。此时可以运行 [DeployScript/testing/drop-all-testing-db.ps1](../DeployScript/testing/drop-all-testing-db.ps1) 清理所有测试数据库。

> [!info]
> 重启电脑后，虚拟磁盘会消失。
> 
> 先关机后开机，虚拟磁盘不会消失。

> [!warning]
> 如果测试数据库太多，导致虚拟磁盘空间不足，可能会导致 pgsql 严重错误。

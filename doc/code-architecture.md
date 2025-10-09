## 源代码结构
### Domain 层
定义数据库实体模型，以及领域事件。由于使用 MediatR 库，所以领域事件，其实就是 MediatR 中的 `INotification` 。

数据库实体模型按照领域根分类。

### Abstractions 层
定义接口和抽象类。这些接口和抽象类会在 Infrastructure 层中实现，在 Application 层调用。

这样做的其中一个好处，就是方便测试，因为测试一般使用虚拟环境，只要在测试项目重新实现接口，就能在虚拟环境测试，而不用改动代码。

在这里，还定义了 `IDbContext` ， `I_xxx_DbContext` ，这是领域根的体现。应用程序的 `DbContext` 会继承这几个接口，当某个业务要用到某个领域根，只需要获取领域根对应的接口即可。

### Infrastructure 层
Infrastructure 层分为 AppInfrastructure 和 DbInfrastructure 。

这样划分，是基于测试的： AppInfrastructure 实现要在测试环境中要重新实现的接口； DbInfrastructure 实现在测试环境中不要重新实现的接口。

在 AppInfrastructure 和 DbInfrastructure 中，都有 `AssemblyInfo` 静态类，用于依赖注入。

在 DbInfrastructure 的 DbConfig 文件夹中，使用流式 API 进行数据库实体配置。根据领域根 -> 实体归类。

在 DbInfrastructure 的 DbFunctions 文件夹中，还定义了一个存储过程。

数据库迁移在 DbInfrastructure 的 Migrations 中。

### Application 层
定义业务逻辑。

有 `AssemblyInfo` 静态类，用于依赖注入。

在 UseCase 文件夹中，以 MediatR 的方式定义 WebApi 的业务逻辑。根据领域根归类。

`JobHandler` 处理音乐文件扫描，插入数据库元组。

在 PipelineMediators 文件夹中，定义了 MediatR 事件的装饰器（装饰器模式），包括验证、日志、异常捕获。

### WebApi 层
定义 Web 接口，是应用程序的启动项目。

使用 Minimal Api 定义 Web 接口，在 EndpointExtensions 中编写依赖注入，从而 Map Endpoint 。根据领域根归类。

在 Program.cs 中，调用各层的依赖注入方法，映射 EndPoints ，启动应用程序。

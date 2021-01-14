# 1 什么是AutoMapper？
AutoMapper是一个对象-对象映射器。对象-对象映射通过将一种类型的输入对象转换为另一种类型的输出对象来工作。使AutoMapper变得有趣的是，它提供了一些有趣的约定，以免去弄清楚如何将类型A映射为类型B。只要类型B遵循AutoMapper既定的约定，就需要几乎零配置来映射两个类型。

# 2 为什么要使用AutoMapper？
映射代码很无聊。测试映射代码更加无聊。AutoMapper提供了简单的类型配置以及简单的映射测试。真正的问题可能是“为什么使用对象-对象映射？” 映射可以在应用程序的许多地方发生，但主要发生在层之间的边界中，例如，UI/域层或服务/域层之间。一层的关注点通常与另一层的关注点冲突，因此对象-对象映射导致分离的模型，其中每一层的关注点仅会影响该层中的类型。

# 3 如何使用AutoMapper？
**首先**，将AutoMapper,AutoMapper.Extensions.Microsoft.DependencyInjection的NuGet软件包安装到您的应用程序中。
```
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```

**然后**，创建DomainToDTOProfile，Todo，TodoDTO类
```
namespace AutoMapper.WebApi01
{
    public class DomainToDTOProfile : Profile
    {
        public DomainToDTOProfile()
        {
            CreateMap<Todo, TodoDTO>();
        }
    }
}
public class Todo
{
    #region Public Properties
    public Guid Id { get; set; }
    public string Name { get; set; }
    #endregion
}
public class TodoDTO
{
    #region Public Properties
    public Guid Id { get; set; }
    public string Name { get; set; }
    #endregion                
}
```

**接着**，在Startup.ConfigureServices中增加services.AddAutoMapper;
```
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Autofac.WebApi", Version = "v1" });
    });

    services.AddAutoMapper(typeof(DomainToDTOProfile));
}
```

**最后**，增加HomeController,创建Todo对象，然后将Todo对象映射给TodoDTO
```
using Microsoft.AspNetCore.Mvc;
using System;

namespace AutoMapper.WebApi01.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : Controller
    {

        private readonly IMapper mapper; // <-添加此行
        public HomeController(IMapper mapper)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public TodoDTO Get()
        {
            var todo = new Todo
            {
                Id = new Guid("9a007349-23fa-45da-97a1-2999f923b5a7"),
                Name = "小明",
            };
            var todoDTO = mapper.Map<TodoDTO>(todo);
            return todoDTO;
        }
    }
}
```

让我们来看看输出结果:
```
{
  "id": "9a007349-23fa-45da-97a1-2999f923b5a7",
  "name": "小明"
}
```
# 新建表
```
CREATE TABLE [dbo].[Todo](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NULL,
 CONSTRAINT [PK_Todo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
```

![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/200929/20200929175439.png)

# 添加模型类
- 在“解决方案资源管理器”中，右键单击项目。 选择“添加” > “新建文件夹”。 将文件夹命名为 Models。
- 右键单击 Models 文件夹，然后选择“添加” > “类” 。 将类命名为 Todo，然后选择“添加”。
```
using System;
namespace App001.Models
{
    public class Todo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
```

# 添加数据库上下文
- 右键单击 Models 文件夹，然后选择“添加” > “类” 。 将类命名为 TodoContext，然后单击“添加”。
```
using Microsoft.EntityFrameworkCore;
namespace App001.Models
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }
        public DbSet<Todo> Todos { get; set; }
    }
}
```

# 注册数据库上下文
在 ASP.NET Core 中，服务（如数据库上下文）必须向依赖关系注入 (DI) 容器进行注册。 该容器向控制器提供服务。

在Startup.cs文件中增加services.AddDbContext，代码如下：
```
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<TodoContext>(opt =>opt.UseSqlServer(Configuration.GetConnectionString("TodoContext")));
    services.AddControllers();
}
```

在appsettings.json文件中增加ConnectionStrings，代码如下：
```
"ConnectionStrings": {
    "TodoContext": "server=.\\SQLEXPRESS;database=App001;uid=sa;pwd=123456;Pooling='true';Min Pool Size=3;"
},
```

# 构建控制器，实现增删改查

- 右键单击 Controllers 文件夹。
- 选择“添加”>“新建构建项” 。
- 选择“其操作使用实体框架的 API 控制器”，然后选择“添加” 。
- 在“添加其操作使用实体框架的 API 控制器”对话框中：
- 在“模型类”中选择“Todo (App001.Models)” 。
- 在“数据上下文类”中选择“TodoContext (App001.Models)” 。
- 选择“添加”。
```
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App001.Models;
namespace App001.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly TodoContext context;
        public TodosController(TodoContext context)
        {
            this.context = context;
        }
        /// <summary>
        /// 获取所有待办事项
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
        {
            return await context.Todo.ToListAsync();
        }
        /// <summary>
        /// 按ID获取项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Todo>> GetTodo(Guid id)
        {
            var todo = await context.Todo.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }
            return todo;
        }
        /// <summary>
        /// 添加新项
        /// </summary>
        /// <param name="todo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Todo>> PostTodo(Todo todo)
        {
            todo.Id = Guid.NewGuid();
            context.Todo.Add(todo);
            await context.SaveChangesAsync();
            return CreatedAtAction("GetTodo", new { id = todo.Id }, todo);
        }
        /// <summary>
        /// 更新现有项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="todo"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<Todo>> PutTodo(Guid id, Todo todo)
        {
            var oldTodo = await context.Todo.FindAsync(id);
            oldTodo.Name = todo.Name;
            context.Entry(oldTodo).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return oldTodo;
        }
        /// <summary>
        /// 删除项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Todo>> DeleteTodo(Guid id)
        {
            var todo = await context.Todo.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }
            context.Todo.Remove(todo);
            await context.SaveChangesAsync();
            return todo;
        }
    }
}
```

# 通过 Postman 测试 添加新项
- 创建新请求。
- 将 HTTP 方法设置为“POST”。
- 将请求 URI 设置为 https://localhost:44342/api/todos。
- 选择“正文”选项卡。
- 选择“原始”单选按钮。
- 将类型设置为 JSON (application/json)
- 在请求正文中，输入待办事项的 JSON：
```
{
    "Name":"遛狗"
}
```
- 选择Send。

![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/200930/20200930100100.png)

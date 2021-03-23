# 1 前置阅读
在阅读本文章之前，你可以先阅读：
* DDD领域驱动设计是什么
* DDD领域驱动设计：实体、值对象、聚合根
* DDD领域驱动设计：仓储
* MediatR一个优秀的.NET中介者框架

# 2 什么是CQRS？
CQRS，即命令和查询职责分离，是一种分离数据读取与写入的体系结构模式。 基本思想是把系统划分为两个界限：
* 查询，不改变系统的状态，且没有副作用。
* 命令，更改系统状态。

我们通过Udi Dahan的《Clarified CQRS》文章中的图来介绍一下：

![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210115/2012032222580035.png)

## 2.1 查询 (Query)
上图中，可以看到Query不是通过DB来查询，而是通过一个专门用于查询的Cache（或ReadDB），ReadDB中的表是专门针对UI优化过的，例如最新的产品列表，销量最好的产品列表等，基本属于用空间换时间。

## 2.2 命令 (Command)
上图中，Command类似于Application Service，Command中主要做的事情有两个：1、通过调用领域层，把相关业务数据写入到DB中。2、同时更新ReadDB。

## 2.3 领域事件 (Domain Event)
上图中，更新ReadDB有两种方式，一种是直接在Command中进行更新，还有一种监听领域事件，把相应更改的数据同步到ReadDB中。

# 3 如何实现CQRS？
我们在这里使用最简单的方法：只将查询与命令分离，且执行这两种操作时使用相同的数据库。

## 3.1 命令 (Command)
**首先**，命令类

命令是让系统执行更改系统状态的操作的请求。 命令具有命令性，且应仅处理一次。

由于命令具有命令性，所以通常采用命令语气使用谓词（如“create”或“update”）命名，命令可能包括聚合类型，例如 CreateTodoCommand 与事件不同，命令不是过去发生的事实，它只是一个请求，因此可以拒绝它。

命令可能源自 UI，由用户发出请求而产生，也可能来自进程管理器，由进程管理器指导聚合执行操作而产生。

命令的一个重要特征是它应该由单一接收方处理，且仅处理一次。 这是因为命令是要在应用程序中执行的单个操作或事务。 例如，同一个“创建待办事项”的处理次数不应超过一次。 这是命令和事件之间的一个重要区别。 事件可能会经过多次处理，因为许多系统或微服务可能会对该事件感兴趣。

命令通过包含数据字段或集合（其中包含执行命令所需的所有信息）的类实现。 命令是一种特殊的数据传输对象 (DTO)，专门用于请求更改或事务。 命令本身完全基于处理命令所需的信息，别无其他。

下面的示例显示了简化的 CreateTodoCommand 类。
```
public class CreateTodoCommand : IRequest<TodoDTO>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
```

**然后**，命令处理程序类

应为每个命令实现特定命令处理程序类。 这是该模式的工作原理，是应用命令对象、域对象和基础结构存储库对象的情景。

命令处理程序收到命令，并从使用的聚合获取结果。 结果应为成功执行命令，或者异常。 出现异常时，系统状态应保持不变。

命令处理程序通常执行以下步骤：
* 它接收 DTO 等命令对象。
* 它会验证命令是否有效。
* 它会实例化作为当前命令目标的聚合根实例。
* 它会在聚合根实例上执行方法，从命令获得所需数据。
* 它将聚合的新状态保持到相关数据库。

通常情况下，命令处理程序处理由聚合根（根实体）驱动的单个聚合。 如果多个聚合应受到单个命令接收的影响，可使用域事件跨多个聚合传播状态或操作。

作为命令处理程序类的示例，下面的代码演示本章开头介绍的同一个 CreateTodoCommandHandler 类。 这个示例还强调了 Handle 方法以及域模型对象/聚合的操作。
```
public class CreateTodoCommandHandler
        : IRequestHandler<CreateTodoCommand, TodoDTO>
{
    private readonly IRepository repository;
    private readonly IMapper mapper;

    public CreateTodoCommandHandler(IRepository repository, IMapper mapper)
    {
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<TodoDTO> Handle(CreateTodoCommand message, CancellationToken cancellationToken)
    {
        var todo = Todo.Create(message.Name);
        repository.Entry(todo);
        await repository.SaveAsync();

        var todoForDTO = mapper.Map<TodoDTO>(todo);
        return todoForDTO;
    }
}
```

**最后**，通过MediatR实现命令进程管道
首先，让我们看一下示例 WebAPI 控制器，你会在其中使用MediatR，如以下示例所示：
```
[Route("api/[controller]")]
[ApiController]
public class TodosController : ControllerBase
{
    //...
    private readonly MediatR.IMediator mediator;
    public TodosController(MediatR.IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    //...
}
```

在控制器方法中，将命令发送到MediatR的代码几乎只有一行：
```
[HttpPost]
public async Task<ActionResult<TodoDTO>> Create(CreateTodoCommand param)
{
    var ret = await mediator.Send(param);
    return CreatedAtAction(nameof(Get), new { id = ret.Id }, ret);
}
```

## 3.2 查询 (Query)
**首先**，定义DTO
```
[Table("T_Todo")]
public class TodoDTO
{
    #region Public Properties
    public Guid Id { get; set; }
    public string Name { get; set; }
    #endregion                
}
```

**然后**，创建具体的查询方法
```
public class TodoQueries
{
    private readonly TodoingQueriesContext context;
    public TodoQueries(TodoingQueriesContext context)
    {
        this.context = context;
    }

    //...
    public async Task<PaginatedItems<TodoDTO>> Query(int pageIndex, int pageSize)
    {
        var total = await context.Todos
            .AsNoTracking()
            .CountAsync();

        var todos = await context.Todos
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Skip(pageSize * (pageIndex - 1))
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedItems<TodoDTO>(total, todos);
    }
    //...
}
```
请注意TodoingQueriesContext和命令处理中的Context不是同一个，实现查询端除了用EFCore、还可以用存储过程、视图、具体化视图或Dapper等等。


**最后**，调用查询方法
```
[Route("api/[controller]")]
[ApiController]
public class TodosController : ControllerBase
{
    private readonly TodoQueries todoQueries;
    public TodosController(TodoQueries todoQueries)
    {
        this.todoQueries = todoQueries ?? throw new ArgumentNullException(nameof(todoQueries));
    }

    //...
    [HttpGet]
    public async Task<ActionResult<PaginatedItems<TodoDTO>>> Query(int pageIndex, int pageSize)
    {
        return todoQueries.Query(pageIndex, pageSize).Result;
    }
    //...
}
```
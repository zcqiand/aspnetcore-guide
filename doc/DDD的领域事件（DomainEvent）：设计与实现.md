# 1 前置阅读
在阅读本文章之前，你可以先阅读：
* 什么是DDD
* DDD的实体、值对象、聚合根的基类和接口：设计与实现
* DDD的仓储（Repository）：设计与实现
* MediatR一个优秀的.NET中介者框架

# 2 什么是领域事件？
领域事件是在领域中发生的事，你希望同一个领域（进程）的其他部分了解它。 通知部分通常以某种方式对事件作出反应。

# 3 实现领域事件？
重点强调领域事件发布/订阅是使用 MediatR 同步实现的。

**首先**，定义待办事项已更新的领域事件
```
public class TodoUpdatedDomainEvent : INotification
{
    public Todo Todo { get; }

    public TodoUpdatedDomainEvent(Todo todo)
    {
        Todo = todo;
    }
}
```

**然后**，引发领域事件，将域事件添加到集合，然后在提交事务之前或之后立即调度这些域事件，而不是立即调度到域事件处理程序 。
```
public abstract class Entity
{
    //...
    private List<INotification> domainEvents;
    public IReadOnlyCollection<INotification> DomainEvents => domainEvents?.AsReadOnly();

    public void AddDomainEvent(INotification eventItem)
    {
        domainEvents = domainEvents ?? new List<INotification>();
        domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
        domainEvents?.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        domainEvents?.Clear();
    }
    //... 其他代码
}
```

要引发事件时，只需将其在聚合根实体的方法处添加到代码中的事件集合。
```
public class Todo : AggregateRoot
{
    //...
    public void Update(
        string name)
    {
        Name = name;
        AddDomainEvent(new TodoUpdatedDomainEvent(this));
    }
    //... 其他代码
}
```

请注意 AddDomainEvent 方法的唯一功能是将事件添加到列表。 尚未调度任何事件，尚未调用任何事件处理程序。你需要在稍后将事务提交到数据库时调度事件。 
```
public class Repository : IDisposable, IRepository
{
    //...
    private readonly IMediator mediator;
    private readonly DbContext context;

    public Repository(DbContext context, IMediator mediator)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public void Save()
    {
        mediator.DispatchDomainEvents(context);
        context.SaveChanges();
    }

    public static void DispatchDomainEvents(this IMediator mediator, DbContext ctx)
    {
        var domainEntities = ctx.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            mediator.Publish(domainEvent);
    }
    //... 其他代码
}
```

**最后**，订阅并处理领域事件
```
public class TodoUpdatedDomainEventHandler : INotificationHandler<TodoUpdatedDomainEvent>
{
    private readonly ILoggerFactory logger;
    public TodoUpdatedDomainEventHandler(ILoggerFactory logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Handle(TodoUpdatedDomainEvent todoUpdatedDomainEvent, CancellationToken cancellationToken)
    {
        logger.CreateLogger<TodoUpdatedDomainEvent>().LogDebug("Todo with Id: {TodoId} has been successfully updated",
                todoUpdatedDomainEvent.Todo.Id);
        return Task.CompletedTask;
    }
}
```
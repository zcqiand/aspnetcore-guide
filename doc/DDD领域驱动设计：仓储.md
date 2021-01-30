# 1 前置阅读
在阅读本文章之前，你可以先阅读：
* 什么是DDD
* DDD的实体、值对象、聚合根的基类和接口：设计与实现

# 2 什么是仓储？
仓储封装了基础设施来提供查询和持久化聚合操作。 它们集中提供常见的数据访问功能，从而提供更好的可维护性，并将用于访问数据库的基础结构或技术与领域模型层分离。 创建数据访问层和应用程序的业务逻辑层之间的抽象层。 实现仓储可让你的应用程序对数据存储介质的更改不敏感。

# 3 为什么仓储？
直接访问数据：
* 重复的代码
* 难以集中化与数据相关的策略（例如缓存）
* 编程错误的可能性更高
* 无法独立于外部依赖项轻松测试业务逻辑

使用仓储优点：
* 可以通过将业务逻辑与数据或服务访问逻辑分开来提高代码的可维护性和可读性。
* 可以从许多位置访问数据源，并希望应用集中管理的，一致的访问规则和逻辑。
* 可以通过自动化进行测试的代码量，并隔离数据层以支持单元测试。
* 可以使用强类型的业务实体，以便可以在编译时而不是在运行时识别问题。
* 可以将行为与相关数据相关联。例如，您要计算字段或在实体中的数据元素之间强制执行复杂的关系或业务规则。
* 应用DDD来简化复杂的业务逻辑。


# 4 实现仓储？
实现基本的增删改查及事务的提交和回滚

**首先**，定义接口
```
/// <summary>
/// IRepository提供应用程序仓储模式基本操作的接口
/// </summary>
public interface IRepository
{
    #region Methods
    void Entry<T>(T t) where T : AggregateRoot;
    void Save();
    T Get<T>(Guid id, Func<IQueryable<T>, IQueryable<T>> includes = null) where T : AggregateRoot;
    T Get<T>(Expression<Func<T, bool>> where, Func<IQueryable<T>, IQueryable<T>> includes = null) where T : AggregateRoot;
    IQueryable<T> Query<T>(Expression<Func<T, bool>> filter=null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy=null, Func<IQueryable<T>, IQueryable<T>> includes=null) where T : AggregateRoot;
    IQueryable<T> QueryByPage<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IQueryable<T>> includes = null) where T : AggregateRoot;
    void BeginTransaction();
    void Commit();
    void Rollback();
    #endregion
}
```

**最后**，实现以上接口
```
public class Repository : IDisposable, IRepository
{
    #region Private Fields
    private readonly DbContext context;
    private IDbContextTransaction transaction;
    #endregion

    #region Constructors
    public Repository(DbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }
    #endregion

    #region IRepository<T> Members
    public void Entry<T>(T t) where T : AggregateRoot
    {
        switch (t.AggregateState)
        {
            case AggregateState.Added:
                context.Entry(t).State = EntityState.Added;
                break;
            case AggregateState.Deleted:
                context.Entry(t).State = EntityState.Deleted;
                break;
            default:
                context.Entry(t).State = EntityState.Modified;
                break;
        }
    }

    public void Save()
    {
        context.SaveChanges();
    }

    public T Get<T>(Guid id, Func<IQueryable<T>, IQueryable<T>> includes = null) where T : AggregateRoot
    {
        return Get(w => w.Id.Equals(id), includes: includes);
    }

    public T Get<T>(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IQueryable<T>> includes = null) where T : AggregateRoot
    {
        return Query(filter, includes: includes).SingleOrDefault();
    }

    public IQueryable<T> Query<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IQueryable<T>> includes = null) where T : AggregateRoot
    {
        IQueryable<T> query = context.Set<T>();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (includes != null)
        {
            query = includes(query);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return query;
    }

    public IQueryable<T> QueryByPage<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IQueryable<T>> includes = null) where T : AggregateRoot
    {
        var query = Query(filter, orderBy, includes)
            .Skip(pageSize * (pageIndex - 1))
            .Take(pageSize);

        return query;
    }

    public void BeginTransaction()
    {
        transaction = context.Database.BeginTransaction();
    }

    public void Rollback()
    {
        transaction.Rollback();
    }

    public void Commit()
    {
        transaction.Commit();
    }

    public void Dispose()
    {
        if (transaction != null)
        {
            transaction.Dispose();
        }
        context.Dispose();
    }
    #endregion
}
```

为数据库上下文和事务上下文声明类变量：
```
private readonly DbContext context;
private IDbContextTransaction transaction;
```

构造函数接受数据库上下文实例：
```
public Repository(DbContext context)
{
    this.context = context ?? throw new ArgumentNullException(nameof(context));
}
```

Get分为通过ID查询或过滤条件进行查询，返回序列中的唯一元素：
```
public T Get<T>(Guid id, Func<IQueryable<T>, IQueryable<T>> includes = null) where T : AggregateRoot
{
    return Get(w => w.Id.Equals(id), includes: includes);
}
public T Get<T>(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IQueryable<T>> includes = null) where T : AggregateRoot
{
    return Query(filter, includes: includes).SingleOrDefault();
}
```

Query 方法使用 lambda 表达式来允许调用代码指定筛选条件，使用一列来对结果进行排序，允许调用方为预先加载导航属性列表：
```
// 代码 Expression<Func<T, bool>> filter 意味着调用方将基于 AggregateRoot 类型提供 lambda 表达式，并且此表达式将返回一个布尔值。
// 代码 Func<IQueryable<T>, IOrderedQueryable<T>> orderBy 也意味着调用方将提供 lambda 表达式。 但在这种情况下，表达式的输入是 AggregateRoot 类型的 IQueryable 对象。 表达式将返回 IQueryable 对象的有序版本。 
// 代码 Func<IQueryable<T>, IQueryable<T>> includes 也意味着调用方将提供 lambda 表达式。 允许预先加载导航属性列表。 

public IQueryable<T> Query<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IQueryable<T>> includes = null) where T : AggregateRoot
{
    IQueryable<T> query = context.Set<T>();

    if (filter != null)
    {
        query = query.Where(filter);
    }

    if (includes != null)
    {
        query = includes(query);
    }

    if (orderBy != null)
    {
        query = orderBy(query);
    }

    return query;
}
```

Entry 方法使用 AggregateRoot.AggregateState 来置 context.Entry(t).State 状态，完成增删改
```
public void Entry<T>(T t) where T : AggregateRoot
{
    switch (t.AggregateState)
    {
        case AggregateState.Added:
            context.Entry(t).State = EntityState.Added;
            break;
        case AggregateState.Deleted:
            context.Entry(t).State = EntityState.Deleted;
            break;
        default:
            context.Entry(t).State = EntityState.Modified;
            break;
    }
}
```

Save 等其他方法也类似实现。

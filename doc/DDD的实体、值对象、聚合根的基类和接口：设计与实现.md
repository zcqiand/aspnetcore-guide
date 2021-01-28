# 1 前置阅读
在阅读本文章之前，你可以先阅读：
* 什么是DDD

# 2 实现值对象
值对象有两个主要特征：它们没有任何标识。它们是不可变的。

我们举个例子：小明是“浙江宁波”人，小红也是“浙江宁波”人，小王是“浙江杭州”人，在这个例子中，我们把地址可以独立出一个值对象出来，我们会遇到了多个对象是否相同的问题，例如小明和小红的地址应该是相等，小明和小王应该是不相等，这很好理解，我们来看一下例子；
```
public class Address
{
    public string Province;
    public string City;
}

var xm = new Address { Province = "浙江", City = "宁波" };
var xh = new Address { Province = "浙江", City = "宁波" };
var xw = new Address { Province = "浙江", City = "杭州" };

Console.WriteLine(xm.Equals(xh));
Console.WriteLine(xm.Equals(xw));
```

让我们来看看输出结果:
```
False
False
```
这个显然不符合我们预期，我们需要重写一下Equals，确保地址值相等的情况下对象相等。

```
public class Address
{
    public string Province;
    public string City;

    public bool Equals(Address obj)
    {
        return this.Province.Equals(obj.Province) && this.City.Equals(obj.City);
    }
}

var xm = new Address { Province = "浙江", City = "宁波" };
var xh = new Address { Province = "浙江", City = "宁波" };
var xw = new Address { Province = "浙江", City = "杭州" };

Console.WriteLine(xm.Equals(xh));
Console.WriteLine(xm.Equals(xw));
```

让我们来看看输出结果:
```
True
False
```
这个显然符合我们预期了，接下来我们把值对象的Equals方法封装到基类中。

```
public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();
    public override bool Equals(object obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;

        return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }
}

public class Address : ValueObject
{
    public string Province;
    public string City;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Province;
        yield return City;
    }
}
```

# 3 实现实体
实体主要特征：具有唯一标识。

前面我们讲到值对象将特定值都相等的对象视为相等对象，在实体中比较容易理解，标识相等的对象视为相等对象。
```
public abstract class Entity
{

    #region IEntity Members
    public abstract Guid Id
    {
        get; set;
    }
    #endregion

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Entity))
            return false;

        if (Object.ReferenceEquals(this, obj))
            return true;

        if (this.GetType() != obj.GetType())
            return false;

        Entity item = (Entity)obj;

        return item.Id == this.Id;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
```

# 4 实现聚合根
聚合根与实体的区别，实体只在聚合内部进行操作，聚合根是对外打交道的唯一实体。我们在这里设计时聚合根需要有增改删状态字段。

```
public enum AggregateState
{
    Added = 1,
    Updated = 2,
    Deleted = 3
}

public abstract class AggregateRoot : Entity
{
    #region IAggregateRoot Members
    public AggregateState AggregateState { set; get; }
    #endregion
}
```
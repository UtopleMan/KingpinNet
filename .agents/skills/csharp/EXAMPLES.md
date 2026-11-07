# C# examples

Good and bad examples for the rules in [SKILL.md](SKILL.md), under the same headings.

## Functions

Good example:

```csharp
Shipment? ProcessOrder(Order order)
{
    if (order is null)
    {
        return null;
    }

    if (order.Items.Count == 0 || !order.IsPaid)
    {
        return null;
    }

    return Ship(order);
}
```

Bad example:

```csharp
Shipment? ProcessOrder(Order order)
{
    if (order != null)
    {
        if (order.Items.Count > 0)
        {
            if (order.IsPaid)
            {
                return Ship(order);
            }
        }
    }
    return null;
}
```

## Comments

Good example:

```csharp
var processor = new OrderProcessor();

// Charges the card immediately; calling twice double-bills the customer
var result = processor.ProcessOrder(order);
```

Bad example:

```csharp
// Process the order
ProcessOrder(order);

ProcessOrder(order); // Process the customer order
```

## Error Handling

Good example:

```csharp
try
{
    ProcessOrder(order);
}
catch (ArgumentNullException ex)
{
    Logger.Error("Order is null", ex);
}

using var connection = new SqlConnection(connectionString);
connection.Open();
```

Bad example:

```csharp
try
{
    ProcessOrder(order);
}
catch (Exception ex)
{
    Logger.Error("Error", ex);
}

SqlConnection connection = null;
try
{
    connection = new SqlConnection(connectionString);
    connection.Open();
}
finally
{
    connection?.Dispose();
}
```

## Language Features

### Primary Constructors

Good example:

```csharp
public class Person(string name, int age)
{
    public string Name => name;
    public int Age => age;

    public void Display()
    {
        Console.WriteLine($"{name} is {age} years old");
    }
}
```

Bad example:

```csharp
public class Person
{
    private string name;
    private int age;

    public Person(string name, int age)
    {
        this.name = name;
        this.age = age;
    }

    public string Name => name;
    public int Age => age;
}
```

### Collection Expressions

Good example:

```csharp
int[] array = [1, 2, 3, 4, 5];
List<string> list = ["one", "two", "three"];

int[] row0 = [1, 2, 3];
int[] row1 = [4, 5, 6];

int[] combined = [..row0, ..row1];
```

### Default Lambda Parameters

Good example:

```csharp
var incrementBy = (int source, int increment = 1) => source + increment;

Console.WriteLine(incrementBy(5));
Console.WriteLine(incrementBy(5, 3));
```

### Alias Any Type

Good example:

```csharp
using Point = (int x, int y);
using ProductList = System.Collections.Generic.List<(string Name, decimal Price)>;

Point origin = (0, 0);
ProductList products = [("Product1", 100m), ("Product2", 200m)];
```

### Params Collections

Good example:

```csharp
public void ProcessItems(params List<string> items)
{
    foreach (var item in items)
    {
        Console.WriteLine(item);
    }
}

public void ProcessData(params ReadOnlySpan<int> data)
{
    foreach (var value in data)
    {
        Process(value);
    }
}
```

### Locking

Good example:

```csharp
private readonly Lock gate = new();
private int pendingCount;

public void Increment()
{
    lock (gate)
    {
        pendingCount++;
    }
}
```

```csharp
private readonly SemaphoreSlim gate = new(1, 1);

public async Task Refresh(CancellationToken cancellationToken)
{
    await gate.WaitAsync(cancellationToken);
    try
    {
        snapshot = await LoadSnapshot(cancellationToken);
    }
    finally
    {
        gate.Release();
    }
}
```

Bad example:

```csharp
private readonly object gate = new();

public void Increment()
{
    lock (gate)
    {
        pendingCount++;
    }
}
```

```csharp
public async Task Refresh(CancellationToken cancellationToken)
{
    using (gate.EnterScope())
    {
        snapshot = await LoadSnapshot(cancellationToken);
    }
}
```

### Partial Properties and Indexers

Good example:

```csharp
public partial class DataModel
{
    public partial string Name { get; set; }
}

public partial class DataModel
{
    private string name;

    public partial string Name
    {
        get => name;
        set => name = value ?? throw new ArgumentNullException(nameof(value));
    }
}
```

### Implicit Index Access

Good example:

```csharp
var countdown = new TimerBuffer
{
    buffer =
    {
        [^1] = 0,
        [^2] = 1,
        [^3] = 2
    }
};
```

### Ref Struct Enhancements

Good example:

```csharp
public ref struct SpanWrapper<T> : IEnumerable<T>
{
    private Span<T> span;

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var item in span)
        {
            yield return item;
        }
    }
}
```

### Extension Members

Good example:

```csharp
extension<TSource>(IEnumerable<TSource> source)
{
    public bool IsEmpty => !source.Any();
    public int Count => source.Count();
}
```

### Field-Backed Properties

Good example:

```csharp
public string Name
{
    get;
    set => field = value ?? throw new ArgumentNullException(nameof(value));
}
```

```csharp
private string name;

public string Name
{
    get => name;
    set => name = value ?? throw new ArgumentNullException(nameof(value));
}
```

### Null-Conditional Assignment

Good example:

```csharp
customer?.Order = GetCurrentOrder();
```

Bad example:

```csharp
if (customer != null)
{
    customer.Order = GetCurrentOrder();
}
```

## Naming Conventions

### Pascal Casing

Good example:

```csharp
public class CustomerOrder
{
    public string OrderId { get; set; }
    public void ProcessOrder() { }
}
```

### Camel Casing

Good example:

```csharp
public class OrderProcessor
{
    private string customerName;
    private int orderCount;

    public void ProcessOrder(string orderId)
    {
        var customerName = GetCustomerName(orderId);
        string processedResult = Process(customerName);
    }
}
```

Bad example:

```csharp
public class OrderProcessor
{
    private string _customerName;
    private int _orderCount;
}
```

### Interface Naming

Good example:

```csharp
public interface IOrderProcessor
{
    void Process(Order order);
}
```

### Type Parameter Naming

Good example:

```csharp
public class Repository<TEntity> where TEntity : class
{
    public void Add(TEntity entity) { }
}
```

## Code Layout

### Curly Braces

Good example:

```csharp
public void ProcessOrder(Order order)
{
    if (order != null)
    {
        order.Process();
    }
}

if (isValid)
{
    Execute();
}

for (int i = 0; i < 10; i++)
{
    Process(i);
}
```

### Line Statements

Good example:

```csharp
public class Order
{
    public string OrderId { get; set; }

    public void Process()
    {
        var result = Validate();
        Execute(result);
    }

    private bool Validate()
    {
        return OrderId != null;
    }
}
```

### Namespaces

Good example:

```csharp
namespace YourProject.Orders;

public class OrderProcessor
{
}
```

Bad example:

```csharp
namespace YourProject.Orders
{
    public class OrderProcessor
    {
    }
}
```

### Using Directives

Good example:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace YourProject.Orders;
```

## Types and Variables

### Type Specification

Good example:

```csharp
string name = "John";
int count = 10;
bool isValid = true;
```

Bad example:

```csharp
String name = "John";
Int32 count = 10;
Boolean isValid = true;
```

### Type Inference (var)

Good example:

```csharp
var orders = new List<Order>();
var customer = GetCustomer();
var activeCustomers = from customer in customers
                      where customer.IsActive
                      select customer;

int count = 10;
string name = "John";
```

Bad example:

```csharp
var count = 10;
var name = "John";
```

## Strings

### String Interpolation

Good example:

```csharp
string message = $"Order {orderId} processed successfully";
```

Bad example:

```csharp
string message = "Order " + orderId + " processed successfully";
```

### StringBuilder

Good example:

```csharp
var builder = new StringBuilder();
for (int i = 0; i < 1000; i++)
{
    builder.Append($"Line {i}\n");
}
```

### Raw String Literals

Good example:

```csharp
string json = """
{
    "name": "John",
    "age": 30
}
""";
```

## Object Initializers

Good example:

```csharp
var customer = new Customer
{
    Name = "John",
    Email = "john@example.com"
};
```

## LINQ

Good example:

```csharp
var result = customers
    .Where(c => c.IsActive)
    .Select(c => c.Name)
    .ToList();
```

## Lambda Expressions

### Event Handlers

Good example:

```csharp
button.Click += (s, e) => ProcessClick();
```

### Parameter Modifiers

Good example:

```csharp
TryParse<int> parse = (text, out result) => int.TryParse(text, out result);
```

## Static Members

### Call Via Class Name

Good example:

```csharp
var result = OrderProcessor.ProcessOrder(order);
```

Bad example:

```csharp
var processor = new OrderProcessor();
var result = processor.ProcessOrder(order);
```

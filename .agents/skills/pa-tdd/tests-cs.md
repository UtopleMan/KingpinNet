# Good and Bad Tests

## Good Tests

**Integration-style**: Test through real interfaces, not mocks of internal parts.

```csharp
// GOOD: Tests observable behavior
[Fact]
public async Task User_can_checkout_with_valid_cart()
{
    var cart = new Cart();
    cart.Add(product);

    var result = await checkout.PlaceOrder(cart, paymentMethod);

    result.Status.Should().Be(OrderStatus.Confirmed);
}
```

Characteristics:

- Tests behavior users/callers care about
- Uses public API only
- Survives internal refactors
- Describes WHAT, not HOW
- One logical assertion per test

## Bad Tests

**Implementation-detail tests**: Coupled to internal structure.

```csharp
// BAD: Tests implementation details
[Fact]
public async Task Checkout_calls_payment_service_process()
{
    var payments = Substitute.For<IPaymentService>();
    var checkout = new Checkout(payments);

    await checkout.PlaceOrder(cart, paymentMethod);

    await payments.Received(1).Process(cart.Total);
}
```

Red flags:

- Mocking internal collaborators
- Testing private methods
- Asserting on call counts/order
- Test breaks when refactoring without behavior change
- Test name describes HOW not WHAT
- Verifying through external means instead of interface

```csharp
// BAD: Bypasses interface to verify
[Fact]
public async Task CreateUser_saves_to_database()
{
    await users.Create(new NewUser("Alice"));

    var row = await db.QuerySingleOrDefaultAsync<UserRow>(
        "SELECT * FROM users WHERE name = @name", new { name = "Alice" });
    row.Should().NotBeNull();
}

// GOOD: Verifies through interface
[Fact]
public async Task Created_user_is_retrievable()
{
    var user = await users.Create(new NewUser("Alice"));

    var retrieved = await users.Get(user.Id);

    retrieved.Name.Should().Be("Alice");
}
```

**Tautological tests**: Expected value restates the implementation, so the test passes by construction.

```csharp
// BAD: Expected value is recomputed the way the code computes it
[Fact]
public void CalculateTotal_sums_line_items()
{
    LineItem[] items = [new(Price: 10m), new(Price: 5m)];
    var expected = items.Sum(i => i.Price);

    Order.CalculateTotal(items).Should().Be(expected);
}

// GOOD: Expected value is an independent, known literal
[Fact]
public void CalculateTotal_sums_line_items()
{
    Order.CalculateTotal([new(Price: 10m), new(Price: 5m)]).Should().Be(15m);
}
```

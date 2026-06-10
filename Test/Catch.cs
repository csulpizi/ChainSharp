using ChainSharp;

namespace Test;

[TestClass]
public sealed class Exceptions
{
    static int CountStringLength(object s) => ((string)s).Length;

    static string SillyString(int x)
    {
        if (x == 0)
            return "0";
        var s = "";
        for (var i = 0; i < x; i++)
            s += x;
        return s;
    }

    // Sync Methods

    [TestMethod]
    public void TestUntypedCatch()
    {
        var Foo = Chain.Init<object>().Then(CountStringLength).Catch((_, _) => 0).Then(SillyString);

        Assert.AreEqual("55555", Foo("silly"));
        Assert.AreEqual("0", Foo(19));
    }

    [TestMethod]
    public void TestTypedCatchSuccess()
    {
        var Foo = Chain
            .Init<object>()
            .Then(CountStringLength)
            .Catch((InvalidCastException e, object _) => 0)
            .Then(SillyString);

        Assert.AreEqual("55555", Foo("silly"));
        Assert.AreEqual("0", Foo(19));
    }

    [TestMethod]
    public void TestTypedCatchFailure()
    {
        var Foo = Chain
            .Init<object>()
            .Then(CountStringLength)
            .Catch((DivideByZeroException e, object _) => 0)
            .Then(SillyString);

        Assert.AreEqual("55555", Foo("silly"));
        Assert.Throws<Exception>(() => Foo(19));
    }

    // Async methods
    static Task<T> Taskify<T>(T x)
    {
        Task<T> t = new(() => x);
        t.Start();
        return t;
    }

    [TestMethod]
    public async Task TestAsyncUntypedCatch()
    {
        var Foo = Chain
            .Init<object>()
            .Then(Taskify)
            .WaitThen(CountStringLength)
            .Catch((_, _) => 0)
            .WaitThen(SillyString);

        Assert.AreEqual("55555", await Foo("silly"));
        Assert.AreEqual("0", await Foo(19));
    }

    [TestMethod]
    public async Task TestAsyncTypedCatchSuccess()
    {
        var Foo = Chain
            .Init<object>()
            .Then(Taskify)
            .WaitThen(CountStringLength)
            .Catch((InvalidCastException e, object _) => 0)
            .WaitThen(SillyString);

        Assert.AreEqual("55555", await Foo("silly"));
        Assert.AreEqual("0", await Foo(19));
    }

    [TestMethod]
    public async Task TestAsyncTypedCatchFailure()
    {
        var Foo = Chain
            .Init<object>()
            .Then(Taskify)
            .WaitThen(CountStringLength)
            .Catch((DivideByZeroException e, object _) => 0)
            .WaitThen(SillyString);

        Assert.AreEqual("55555", await Foo("silly"));
        await Assert.ThrowsAsync<Exception>(() => Foo(19));
    }
}

using static ChainSharp.ChainSharp;

namespace Test;

[TestClass]
public class TestNullables
{
    static int? EvenOrNull(int x) => x % 2 == 0 ? x : null;

    static int Increase(int x) => x + 1;

    static string SillyString(int? x) => "Hi!" + x;

    static Task<T> Taskify<T>(T x)
    {
        Task<T> t = new(() => x);
        t.Start();
        return t;
    }

    [TestMethod]
    public void TestThenOrNull()
    {
        var foo = Chain<int>().Then(EvenOrNull).ThenOrNull(Increase).Then(SillyString);

        Assert.AreEqual("Hi!5", foo(4));
        Assert.AreEqual("Hi!", foo(5));
    }

    [TestMethod]
    public async Task TestWaitThenOrNull()
    {
        var foo = Chain<int>()
            .Then(EvenOrNull)
            .Then(Taskify)
            .WaitThenOrNull(Increase)
            .WaitThen(SillyString);

        Assert.AreEqual("Hi!5", await foo(4));
        Assert.AreEqual("Hi!", await foo(5));
    }
}

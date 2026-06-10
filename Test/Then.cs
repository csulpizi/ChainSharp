using System.Collections.Specialized;
using ChainSharp;

namespace Test;

[TestClass]
public sealed class Core
{
    static string Caps(string s) => s.ToUpper();

    static string Cat(string s) => s + " hello!";

    static Task<T> Taskify<T>(T x)
    {
        Task<T> t = new(() => x);
        t.Start();
        return t;
    }

    [TestMethod]
    public void SyncTest()
    {
        var foo = Chain.Init<string>().Then(Caps).Then(Cat).Then(Caps);

        Assert.AreEqual("WOAH HELLO!", foo("woah"));
    }

    [TestMethod]
    public async Task AsyncTest()
    {
        var foo = Chain.Init<string>().Then(Caps).Then(Taskify).WaitThen(Cat).WaitThen(Caps);

        Assert.AreEqual("WOAH HELLO!", await foo("woah"));
    }
}

using System;
using System.Linq;
using NUnit.Framework;

namespace NonPlusUltra.Cmd
{
    public class Fixture
    {
    [Test]
    public void BinaryCommand_Test()
    {
        var underlying = "Some Object That Happens To Be A String At Compile Time";

        var command1 = underlying.Command()
            .AddParameter<int>()
            .AddParameter<int>()
            .Build((s, startIndex, length) => s.Substring(startIndex, length));
            
        //-- works for extension method invocations
        Console.WriteLine(command1.Invoke(5, 6)); // prints "Object"
        Assert.AreEqual(underlying.Substring(5, 6), command1.Invoke(5, 6));

        var command2 = underlying.Command()
            .AddParameter<int>()
            .AddParameter<string>()
            .Build((s, startIndex, value) => s.Insert(startIndex, value));

        //-- works for extension method invocations
        Console.WriteLine(command2.Invoke(5, "Hacked")); // prints "Some HackedObject That Happens To Be A String At Compile Time"
        Assert.AreEqual(underlying.Insert(5, "Hacked"), command2.Invoke(5, "Hacked"));
    }
    }

}

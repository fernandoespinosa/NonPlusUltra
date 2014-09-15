using NUnit.Framework;
using System;
using System.Linq;

namespace NonPlusUltra.Commands
{
    public class Fixture
    {
        [Test]
        public void NullaryCommand_Test()
        {
            var underlying = "Some Object That Happens To Be A String At Compile Time";

            var command = underlying.Command().Build(s => s.Length);

            //-- works for property access
            Console.WriteLine(command.Invoke()); // prints 55
            Assert.AreEqual(underlying.Length, command.Invoke());

            command = underlying.Command().Build(s => s.LastIndexOf('A'));

            //-- works for method invocations (and captured variables)
            Console.WriteLine(command.Invoke()); // prints 40
            Assert.AreEqual(underlying.LastIndexOf('A'), command.Invoke());

            command = underlying.Command().Build(s => s.Count(c => c == 'T'));

            //-- works for extension method invocations (and captured variables)
            Console.WriteLine(command.Invoke()); // prints 3
            Assert.AreEqual(underlying.Count(c => c == 'T'), command.Invoke());
        }

        [Test]
        public void UnaryCommand_Test()
        {
            var underlying = "Some Object That Happens To Be A String At Compile Time";

            var command = underlying.Command().AddParameter<char>().Build((s, ch) => s.LastIndexOf(ch));

            //-- works for method invocations
            Console.WriteLine(command.Invoke('A')); // prints 40
            Assert.AreEqual(underlying.LastIndexOf('A'), command.Invoke('A'));

            command = underlying.Command().AddParameter<char>().Build((s, ch) => s.Count(c => c == ch));

            //-- works for extension method invocations
            Console.WriteLine(command.Invoke('T')); // prints 3
            Assert.AreEqual(underlying.Count(c => c == 'T'), command.Invoke('T'));
        }

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
            Console.WriteLine(command2.Invoke(5, "Hacked")); // prints "Object That"
            Assert.AreEqual(underlying.Insert(5, "Hacked"), command2.Invoke(5, "Hacked"));
        }
    }

}

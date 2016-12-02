using NUnit.Framework;
using System;

namespace NonPlusUltra.MoreCode
{
    class Fixture
    {
        [Test]
        public void Fluent_Test()
        {
            "Foo".Foo(0).Bar();
            "Foo".Foo(false).Bar();
            "Foo".Foo(new object()).Bar();
            "Foo".Foo("Bar").Bar();
            "Foo".Foo(1.0).Bar();

            //"Foo".Foo(i => 1).Bar<int, int>(); // cannot do this
        }
    }

    static class Extensions
    {
        public static Foo<T> Foo<T>(this object @object, T t)
        {
            return new Foo<T>();
        }
        public static Foo<T, S> Foo<T, S>(this object @object, Func<T, S> f)
        {
            return new Foo<T, S>();
        }
        public static void Bar<T>(this Foo<T> foo)
        {
        }
        public static void Bar<T, S>(this Foo<T, S> foo)
        {
        }
    }

    internal class Foo<T>
    {
    }

    internal class Foo<T, S>
    {
    }
}

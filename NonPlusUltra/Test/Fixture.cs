using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace NonPlusUltra.Test
{
    public class Fixture
    {
        [Test]
        public void NullaryCommand_Test()
        {
            var underlying = "Some Object That Happens To Be A String At Compile Time";

            var command = underlying.Wrap().Command(s => s.Length);

            //-- works for property access
            Console.WriteLine(command.Invoke()); // prints 55
            Assert.AreEqual(underlying.Length, command.Invoke());

            command = underlying.Wrap().Command(s => s.LastIndexOf('A'));

            //-- works for method invocations (and captured variables)
            Console.WriteLine(command.Invoke()); // prints 40
            Assert.AreEqual(underlying.LastIndexOf('A'), command.Invoke());

            command = underlying.Wrap().Command(s => s.Count(c => c == 'T'));

            //-- works for extension method invocations (and captured variables)
            Console.WriteLine(command.Invoke()); // prints 3
            Assert.AreEqual(underlying.Count(c => c == 'T'), command.Invoke());
        }

        [Test]
        public void UnaryCommand_Test()
        {
            var underlying = "Some Object That Happens To Be A String At Compile Time";

            var command = underlying.Wrap().Command<string, char, int>((s, ch) => s.LastIndexOf(ch));

            //-- works for method invocations
            Console.WriteLine(command.Invoke('A')); // prints 40
            Assert.AreEqual(underlying.LastIndexOf('A'), command.Invoke('A'));

            command = underlying.Wrap().Command<string, char, int>((s, ch) => s.Count(c => c == ch));

            //-- works for extension method invocations
            Console.WriteLine(command.Invoke('T')); // prints 3
            Assert.AreEqual(underlying.Count(c => c == 'T'), command.Invoke('T'));
        }

        [Test]
        public void UnaryCommand_WIP_Test()
        {
            var underlying = "Some Object That Happens To Be A String At Compile Time";

            var command = underlying.Builder().AddParam<char>().Command((s, ch) => s.LastIndexOf(ch));

            //-- works for method invocations
            Console.WriteLine(command.Invoke('A')); // prints 40
            Assert.AreEqual(underlying.LastIndexOf('A'), command.Invoke('A'));

            command = underlying.Builder().AddParam<char>().Command((s, ch) => s.Count(c => c == ch));

            //-- works for extension method invocations
            Console.WriteLine(command.Invoke('T')); // prints 3
            Assert.AreEqual(underlying.Count(c => c == 'T'), command.Invoke('T'));

            var builder = underlying.Builder()
                .AddParam<int>()
                .AddParam<int>();

            Console.WriteLine(builder);
            //var c = builder.Command((s, startIndex, length) => s.Substring(startIndex, length));
        }

        [Test]
        public void Infer_2_Generic_Arguments_Test()
        {
            Foo("this is a string", () => "and this returns some int".Length);
        }

        private static void Foo<T, U>(T t, Func<U> f)
        {
        }

        [Test]
        public void Infer_3_Generic_Arguments_Cannot_Compile_Test()
        {
            //Foo("this is a string", s => "and this returns some int".Length);
        }

        private static void Foo<T, U, V>(T t, Func<U, V> f)
        {
        }

        [Test]
        public void Infer_3_Generic_Arguments_Test()
        {
            FooWithPrototype("this is a string", s => "and this returns some int".Length, "do I need to do something like this to resolve?");
        }

        private static void FooWithPrototype<T, U, V>(T t, Func<U, V> f, U prototype = default (U))
        {
        }

        [Test]
        public void Create_Bar_Test()
        {
            var data = new { Name = "Bar1" };
            //var bar = new Bar<? > (data); // can't do this
            var bar = CreateBar(data);
        }

        public static Bar<T> CreateBar<T>(T data)
        {
            return new Bar<T>(data);
        }
    }

    public class Bar<T>
    {
        public Bar(T data) { }
    }

    public interface ICommand<out TOuput>
    {
        TOuput Invoke();
    }

    public class FuncCommand<TOuput> : ICommand<TOuput>
    {
        private readonly Func<TOuput> _func;
        public FuncCommand(Func<TOuput> func) { _func = func; }
        public TOuput Invoke() { return _func(); }
    }

    public interface ICommand<in TInput, out TOuput>
    {
        TOuput Invoke(TInput input);
    }

    public class FuncCommand<TInput, TOuput> : ICommand<TInput, TOuput>
    {
        private readonly Func<TInput, TOuput> _func;
        public FuncCommand(Func<TInput, TOuput> func) { _func = func; }
        public TOuput Invoke(TInput input) { return _func(input); }
    }

    public interface ICommand<in TArg1, in TArg2, out TOuput>
    {
        TOuput Invoke(TArg1 arg1, TArg2 arg2);
    }

    public class FuncCommand<TArg1, TArg2, TOuput> : ICommand<TArg1, TArg2, TOuput>
    {
        private readonly Func<TArg1, TArg2, TOuput> _func;
        public FuncCommand(Func<TArg1, TArg2, TOuput> func) { _func = func; }
        public TOuput Invoke(TArg1 arg1, TArg2 arg2) { return _func(arg1, arg2); }
    }

    public class Builder<T, TParam> : Builder<T>
    {
        internal Builder() { }
    }

    public class Builder<T>
    {
        internal T Underlying { get; set; }
        internal Builder() { }

        public Builder<T, TParam> AddParam<TParam>()
        {
            return new Builder<T, TParam>() { Underlying = Underlying };
        }
    }

    public static partial class CommandBuilder
    {
        public class Token<T>
        {
            internal T Underlying { get; set; }
            internal Token() { }
        }

        public class Token<T, TParam> : Token<T>
        {
        }
        public static Builder<T> Builder<T>(this T underlying)
        {
            return new Builder<T> { Underlying = underlying };
        }

        public static Token<T> Wrap<T>(this T underlying)
        {
            return new Token<T> { Underlying = underlying };
        }

        public static Token<T, TParam> WithParameter<T, TParam>(this Token<T> token)
        {
            return new Token<T, TParam> { Underlying = token.Underlying };
        }
    }

    public static partial class CommandBuilder
    {
        public static ICommand<TOuput> Command<T, TOuput>(this Token<T> token, Expression<Func<T, TOuput>> expression)
        {
            var func = expression.Compile();
            return new FuncCommand<TOuput>(() => func(token.Underlying));
        }
    }

    public static partial class CommandBuilder
    {
        public static ICommand<TInput, TOuput> Command<T, TInput, TOuput>(this Token<T> token, Expression<Func<T, TInput, TOuput>> expression)
        {
            var func = expression.Compile();
            return new FuncCommand<TInput, TOuput>(i => func(token.Underlying, i));
        }
    }

    public static partial class CommandBuilder
    {
        public static ICommand<TInput, TOuput> CommandWithPrototype<T, TInput, TOuput>(this Token<T> token, Expression<Func<T, TInput, TOuput>> expression, TInput input = default(TInput))
        {
            var func = expression.Compile();
            return new FuncCommand<TInput, TOuput>(i => func(token.Underlying, i));
        }
    }

    public static partial class CommandBuilder
    {
        public static ICommand<TArg1, TOuput> Command<T, TArg1, TOuput>(this Builder<T, TArg1> token, Expression<Func<T, TArg1, TOuput>> expression)
        {
            var func = expression.Compile();
            return new FuncCommand<TArg1, TOuput>(i => func(token.Underlying, i));
        }

        public static ICommand<TArg1, TArg2, TOuput> Command<T, TArg1, TArg2, TOuput>(this Builder<Builder<T, TArg1>, TArg2> token, Expression<Func<T, TArg1, TArg2, TOuput>> expression)
        {
            var func = expression.Compile();
            return new FuncCommand<TArg1, TArg2, TOuput>((arg1, arg2) => func(token.Underlying.Underlying, arg1, arg2));
        }
    }

}

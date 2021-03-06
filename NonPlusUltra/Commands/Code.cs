﻿using System.Linq.Expressions;
using NonPlusUltra.Test;
using System;

namespace NonPlusUltra.Commands
{
    public interface ICommand<out TResult>
    {
        TResult Invoke();
    }

    public interface ICommand<in TArg1, out TResult>
    {
        TResult Invoke(TArg1 input);
    }

    public interface ICommand<in TArg1, in TArg2, out TResult>
    {
        TResult Invoke(TArg1 arg1, TArg2 arg2);
    }

    public interface ICommand<in TArg1, in TArg2, in TArg3, out TResult>
    {
        TResult Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }

    public class FuncCommand<TResult> : ICommand<TResult>
    {
        private readonly Func<TResult> _func;
        public FuncCommand(Func<TResult> func) { _func = func; }
        public TResult Invoke() { return _func(); }
    }

    public class FuncCommand<TArg1, TResult> : ICommand<TArg1, TResult>
    {
        private readonly Func<TArg1, TResult> _func;
        public FuncCommand(Func<TArg1, TResult> func) { _func = func; }
        public TResult Invoke(TArg1 input) { return _func(input); }
    }

    public class FuncCommand<TArg1, TArg2, TResult> : ICommand<TArg1, TArg2, TResult>
    {
        private readonly Func<TArg1, TArg2, TResult> _func;
        public FuncCommand(Func<TArg1, TArg2, TResult> func) { _func = func; }
        public TResult Invoke(TArg1 arg1, TArg2 arg2) { return _func(arg1, arg2); }
    }

    public class FuncCommand<TArg1, TArg2, TArg3, TResult> : ICommand<TArg1, TArg2, TArg3, TResult>
    {
        private readonly Func<TArg1, TArg2, TArg3, TResult> _func;
        public FuncCommand(Func<TArg1, TArg2, TArg3, TResult> func) { _func = func; }
        public TResult Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3) { return _func(arg1, arg2, arg3); }
    }

    public class CommandBuilder<T>
    {
        internal T Underlying { get; set; }
        public CommandBuilder<T, TArg1> AddParameter<TArg1>()
        {
            return new CommandBuilder<T, TArg1> { Underlying = Underlying };
        }
    }

    public class CommandBuilder<T, TArg1>
    {
        internal T Underlying { get; set; }
        public CommandBuilder<T, TArg1, TArg2> AddParameter<TArg2>()
        {
            return new CommandBuilder<T, TArg1, TArg2> { Underlying = Underlying };
        }
    }

    public class CommandBuilder<T, TArg1, TArg2>
    {
        internal T Underlying { get; set; }
        public CommandBuilder<T, TArg1, TArg2, TArg3> AddParameter<TArg3>()
        {
            return new CommandBuilder<T, TArg1, TArg2, TArg3> { Underlying = Underlying };
        }
    }

    public class CommandBuilder<T, TArg1, TArg2, TArg3>
    {
        internal T Underlying { get; set; }
    }

    public static class CommandBuilderExtensions
    {
        public static CommandBuilder<T> Command<T>(this T underlying)
        {
            return new CommandBuilder<T> { Underlying = underlying };
        }
        public static ICommand<TResult> Build<T, TResult>(this CommandBuilder<T> builder, Expression<Func<T, TResult>> expression)
        {
            var func = expression.Compile();
            return new FuncCommand<TResult>(() => func(builder.Underlying));
        }
        public static ICommand<TArg1, TResult> Build<T, TArg1, TResult>(this CommandBuilder<T, TArg1> builder, Expression<Func<T, TArg1, TResult>> expression)
        {
            var func = expression.Compile();
            return new FuncCommand<TArg1, TResult>(i => func(builder.Underlying, i));
        }
        public static ICommand<TArg1, TArg2, TResult> Build<T, TArg1, TArg2, TResult>(this CommandBuilder<T, TArg1, TArg2> token, Expression<Func<T, TArg1, TArg2, TResult>> expression)
        {
            var func = expression.Compile();
            return new FuncCommand<TArg1, TArg2, TResult>((arg1, arg2) => func(token.Underlying, arg1, arg2));
        }
        public static ICommand<TArg1, TArg2, TArg3, TResult> Build<T, TArg1, TArg2, TArg3, TResult>(this CommandBuilder<T, TArg1, TArg2, TArg3> token, Expression<Func<T, TArg1, TArg2, TArg3, TResult>> expression)
        {
            var func = expression.Compile();
            return new FuncCommand<TArg1, TArg2, TArg3, TResult>((arg1, arg2, arg3) => func(token.Underlying, arg1, arg2, arg3));
        }
    }
}
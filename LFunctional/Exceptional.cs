using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Unit = System.ValueTuple;

public static partial class LFunctional
{
    // IO wraps actions and functions in Result
    public static Func<Result<R>> IO<R>(Func<R> f) {
        return () => {
            try { return f();} catch(Exception e) {return Fail<R>(new ExceptionError(e));}
        };
    }

    public static Func<Result<Unit>> IO(Action a)
        => IO(a.ToFunc());

    public static Func<T,Result<Unit>> IO<T>(Action<T> a) 
        => IO(a.ToFunc());

    public static Func<T1,T2, Result<Unit>> IO<T1, T2>(Action<T1, T2> a)
        => IO(a.ToFunc());

    public static Func<T, Result<R>> IO<T,R>(Func<T, R> f)
        => t => IO(() => f(t))();

    public static Func<T1, T2, Result<R>> IO<T1, T2,R>(Func<T1, T2, R> f)
        => (t1, t2) => IO((Func<T2, R>)(t2 => f(t1, t2)))(t2);

    public static Func<T1, T2, T3, Result<R>> IO<T1, T2, T3, R>(Func<T1, T2, T3, R> f)
        => (t1, t2, t3) => IO((Func<T2, T3, R>)((t2, t3) => f(t1, t2, t3)))(t2, t3);

// from https://stackoverflow.com/questions/37093261/attach-stacktrace-to-exception-without-throwing-in-c-sharp-net
// need to understand, refactor and test. It might work.
    internal static Exception GetException(string message, int skipFrames)
        => new Exception(message).SetStackTrace(new StackTrace(skipFrames, true));

    internal static Exception SetStackTrace(this Exception target, StackTrace stack) => _SetStackTrace(target, stack);

    private static readonly Func<Exception, StackTrace, Exception> _SetStackTrace = new Func<Func<Exception, StackTrace, Exception>>(() =>
    {
        ParameterExpression target = Expression.Parameter(typeof(Exception));
        ParameterExpression stack = Expression.Parameter(typeof(StackTrace));

#pragma warning disable 8600, 8604, 8620
        Type traceFormatType = typeof(StackTrace).GetNestedType("TraceFormat", BindingFlags.NonPublic);
        MethodInfo toString = typeof(StackTrace).GetMethod("ToString", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { traceFormatType }, null);
        object normalTraceFormat = Enum.GetValues(traceFormatType).GetValue(0);

        MethodCallExpression stackTraceString = Expression.Call(stack, toString, Expression.Constant(normalTraceFormat, traceFormatType));
        FieldInfo stackTraceStringField = typeof(Exception).GetField("_stackTraceString", BindingFlags.NonPublic | BindingFlags.Instance);
        BinaryExpression assign = Expression.Assign(Expression.Field(target, stackTraceStringField), stackTraceString);
        return Expression.Lambda<Func<Exception, StackTrace, Exception>>(Expression.Block(assign, target), target, stack).Compile();
    })();
}
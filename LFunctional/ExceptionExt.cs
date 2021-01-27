using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

// from https://stackoverflow.com/questions/37093261/attach-stacktrace-to-exception-without-throwing-in-c-sharp-net
// need to understand, refactor and test. It might work.

public static partial class LFunctional
{
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
using System;
using Unit = System.ValueTuple;

public static partial class LFunctional {

    public abstract record Wraps<T> {

 #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected T Value {get; init;} 
 #pragma warning restore CS8618 

        public static implicit operator T(Wraps<T> a) => a.Value;

        protected virtual bool PrintMembers(System.Text.StringBuilder builder) {
           builder.Append(Value);
           return true; 
        }
    }

    public static Unit Unit() => default;
    public static T id<T>(T t) => t;

    public static Func<Unit> ToFunc(this Action a) => () => { a(); return Unit();};
    public static Func<T, Unit> ToFunc<T>(this Action<T> a) => t => { a(t); return Unit();};
    public static Func<T1, T2, Unit> ToFunc<T1, T2>(this Action<T1, T2> a) => (t1, t2)
        => { a(t1, t2); return Unit();};
    public static Func<T1, T2, T3, Unit> ToFunc<T1, T2, T3>(this Action<T1, T2, T3> a) => (t1, t2, t3)
        => { a(t1, t2, t3); return Unit();};


}

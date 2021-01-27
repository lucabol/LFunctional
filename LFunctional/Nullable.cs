
using System;

public static partial class LFunctional {

    public static R Match<T,R>(this T? t, Func<T,R> success, Func<R> fail)
        where T:struct
        => t is null ? fail() : success((T)t); 
        
    public static R Match<T,R>(this T? t, Func<T,R> success, Func<R> fail)
        where T:class
        => t is null ? fail() : success(t); 


    // Need the additonal parameter trick to overload properly
    public static R? Map<T, R>(this T? t, Func<T,R> f, R? _ = default)
        where T: class where R: class
        => t is null ? default(R?) : f(t);

    public static R? Map<T, R>(this T? t, Func<T,R> f, R? _ = default)
        where T: struct where R: struct
        => t is null ? default(R?) : f((T)t);

    public static R? Map<T, R>(this T? t, Func<T,R> f, R? _ = default)
        where T: class where R: struct
        => t is null ? default(R?) : f((T)t);

    public static R? Map<T, R>(this T? t, Func<T,R> f, R? _ = default)
        where T: struct where R: class
        => t is null ? default(R?) : f((T)t);

    public static R? Bind<T,R>(this T? t, Func<T, R?> f)
        where T:class
        => t is null ? default : f(t);

    public static R? Bind<T,R>(this T? t, Func<T, R?> f)
        where T:struct
        => t is null ? default : f((T)t);

    // for LINQ
    public static R? Select<T, R>(this T? t, Func<T,R> f, R? _ = default)
        where T: class where R: class
        => t is null ? default(R?) : f(t);

    public static R? Select<T, R>(this T? t, Func<T,R> f, R? _ = default)
        where T: struct where R: struct
        => t is null ? default(R?) : f((T)t);

    public static R? Select<T, R>(this T? t, Func<T,R> f, R? _ = default)
        where T: class where R: struct
        => t is null ? default(R?) : f((T)t);

    public static R? Select<T, R>(this T? t, Func<T,R> f, R? _ = default)
        where T: struct where R: class
        => t is null ? default(R?) : f((T)t);


    public static R? SelectMany<S,S1,R>
        (this S? @this, Func<S,S1?> bind, Func<S,S1,R> proj, R? _ = default)
        where S:class where S1:class
        => @this.Match(
            s => bind(s).Match(
                s1 => proj(s, s1),
                ()  => default(R?)
            ),
            () => default(R?));


    public static R? SelectMany<S,S1,R>
        (this S? @this, Func<S,S1?> bind, Func<S,S1,R> proj, R? _ = default)
        where S:class where S1:struct
        => @this.Match(
            s => bind(s).Match(
                s1 => proj(s, s1),
                ()  => default(R?)
            ),
            () => default(R?));
            
    public static R? SelectMany<S,S1,R>
        (this S? @this, Func<S,S1?> bind, Func<S,S1,R> proj, R? _ = default)
        where S:struct where S1:class
        => @this.Match(
            s => bind(s).Match(
                s1 => proj(s, s1),
                ()  => default(R?)
            ),
            () => default(R?));

    public static R? SelectMany<S,S1,R>
        (this S? @this, Func<S,S1?> bind, Func<S,S1,R> proj, R? _ = default)
        where S:struct where S1:struct
        => @this.Match(
            s => bind(s).Match(
                s1 => proj(s, s1),
                ()  => default(R?)
            ),
            () => default(R?));

    // conversions
    public static Result<S> ToResult<S>(this S? s) where S:class
        => s.Match(x => x, () => Fail<S>(new NullReferenceException()));
    public static Result<S> ToResult<S>(this S? s) where S:struct
        => s.Match(x => x, () => Fail<S>(new NullReferenceException()));
}

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
}
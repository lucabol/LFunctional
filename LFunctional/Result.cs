using System;
using System.Collections.Generic;
using System.Linq;

using Unit = System.ValueTuple;

static partial class LFunctional {

    public abstract record Error();

    public record StringError(string Message): Error {

        public static implicit operator StringError(string s) => new StringError(s);
    }
    public record ExceptionError(Exception Exception): Error {
        public static implicit operator ExceptionError(Exception e) => new ExceptionError(e);
    }

    // The elevated type
    public abstract record Result<S>() {
        internal sealed record Success(S Value)                   : Result<S>;
        internal sealed record Failure(IEnumerable<Error> Errors) : Result<S>;

        public static implicit operator Result<S>(S s)  => Success(s);
    }

    // Return functions
    public static Result<S> Success<S>(S s)                  => new Result<S>.Success(s);
    public static Result<S> Fail<S>(IEnumerable<Error> ee)   => new Result<S>.Failure(ee);
    public static Result<S> Fail<S>(Error e)                 => Fail<S>(Enumerable.Repeat(e, 1));
    public static Result<S> Fail<S>(string s)                => Fail<S>(new StringError(s));
    public static Result<S> Fail<S>(Exception e)             => Fail<S>(new ExceptionError(e));

    // Extractor
    public static R Match<S,R>(this Result<S> @this, Func<S,R> success, Func<IEnumerable<Error>, R> failure)
        => @this switch {
            Result<S>.Success s => success(s.Value),
            Result<S>.Failure e    => failure(e.Errors),
            _                   => throw new Exception("Either success or failure.") };

        
    // Classical functional lore
    public static Result<R> Map<S,R>(this Result<S> @this, Func<S,R> f)
        => @this.Match( v => f(v), ee => Fail<R>(ee)); 

    public static Result<S> MapErrors<S>(this Result<S> @this, Func<IEnumerable<Error>, IEnumerable<Error>> f)
        => @this.Match(s => s, e => Fail<S>(f(e)));

    public static Result<R> Bind<S,R>(this Result<S> @this, Func<S,Result<R>> f)
        => @this.Match( s => f(s), Fail<R>);

    public static Result<R> Apply<S,R>(this Result<Func<S,R>> fR, Result<S> xR) =>
        (fR, xR) switch {
            (Result<Func<S,R>>.Success sf,Result<S>.Success sx)
                                                 => sf.Value(sx.Value),
            (Result<Func<S,R>>.Failure sf, _)       => Fail<R>(sf.Errors),
            (_ ,Result<S>.Failure sx)               => Fail<R>(sx.Errors),
            _                                    => throw new Exception("Either Success or Failure")
        };

    public static Result<Func<S2,R>> Apply<S1,S2,R>(this Result<Func<S1,S2,R>> fR, Result<S1> xR) =>
        Apply(fR.Map(Curry), xR);

    public static Result<Func<S2,S3,R>> Apply<S1,S2,S3,R>(this Result<Func<S1,S2,S3,R>> fR, Result<S1> xR) =>
        Apply(fR.Map(CurryFirst), xR);

    // Playing nice with Actions
    public static void ForEach<S>(this Result<S> @this, Action<S> a, Action<IEnumerable<Error>> f) {
        @this.Match<S,Unit>(a.ToFunc(), f.ToFunc());
    }
    public static void ForEach<S>(this Result<S> @this, Action<S> a) {
        if(@this is Result<S>.Success s) a(s.Value);
    }
    public static void ForEachFailure<S>(this Result<S> @this, Action<IEnumerable<Error>> a) {
        if(@this is Result<S>.Failure f) a(f.Errors);
    }

    // for LINQ
    public static Result<S1> Select<S,S1>(this Result<S> @this, Func<S, S1> f)
        => @this.Map(f);

    public static Result<R> SelectMany<S,S1,R>
        (this Result<S> @this, Func<S,Result<S1>> bind, Func<S,S1,R> proj)
        => @this.Match(
            s => bind(s).Match(
                s1 => proj(s, s1),
                e  => Fail<R>(e)
            ),
            e => Fail<R>(e) );

    // Unfortunately no way to specify an error if the Where clause is not satisfied
    private record WhileError(): Error;

    public static Result<S> Where<S>(this Result<S> @this, Func<S, bool> pred)
        => @this.Match(
            s => pred(s) ? Success<S>(s) : Fail<S>(new WhileError()), e => Fail<S>(e));

    public static Result<S> MapWhileErrors<S>(this Result<S> @this, Func<Error> f)
        => @this.MapErrors(ee => ee.Select(e => e is WhileError ? f() : e));
}
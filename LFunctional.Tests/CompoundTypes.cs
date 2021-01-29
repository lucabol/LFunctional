using static SimpleTypes;
using static LFunctional;
using System;

public static partial class CompoundTypes {


    // No need for any cross properties validation
    public record Person(Name Name, Age Age);

    // Cross property validation at construction
    public record Faker {

        public Name Name { get;}
        public Age  RealAge { get;}
        public Age  FakeAge { get;}

        private Faker(Name name, Age realAge, Age fakeAge) =>
            (Name, RealAge, FakeAge) = (name, realAge, fakeAge);

        // Need this because the costructor is not a real function
        private static Faker Create(Name name, Age realAge, Age fakeAge) =>
            new Faker(name, realAge, fakeAge);

        public static Result<Faker> Of(string name, int realAge, int fakeAge)
            => (from n in Name.Of(name)
                from r in Age.Of(realAge)
                from f in Age.Of(fakeAge)
                where r < f // an invariant, more difficult in the applicative case 
                select Create(n, r, f))
                .MapWhileErrors(() => (StringError) 
                    $"Error constructing {nameof(Faker)}. RealAge({realAge}) >= FakeAge({fakeAge})");

        public static Result<Faker> OfApplicative(string name, int realAge, int fakeAge)
            => Success<Func<Name, Age, Age, Faker>>(Create).Apply(Name.Of(name)).Apply(Age.Of(realAge)).Apply(Age.Of(fakeAge));

    }

    // Using nullable

    public record Mixed(Name? Name, Age Age, int AnInt);
}
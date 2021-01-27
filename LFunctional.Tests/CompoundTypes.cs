using static SimpleTypes;
using static LFunctional;

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

        public static Result<Faker> Of(string name, int realAge, int fakeAge)
            => (from n in Name.Of(name)
                from r in Age.Of(realAge)
                from f in Age.Of(fakeAge)
                where r < f // an invariant 
                select new Faker(n, r, f))
                .MapWhileErrors(() => (StringError) 
                    $"Error constructing {nameof(Faker)}. RealAge({realAge}) >= FakeAge({fakeAge})");
    }

    // Using nullable

    public record Mixed(Name? Name, Age Age, int AnInt);
}
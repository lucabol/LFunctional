using System;
using Xunit;

using static LFunctional;
using static SimpleTypes;
using static CompoundTypes;

public partial class Tests
{
    [Fact(Skip = "Run Manually")]
    public void cannot_call_constructors() {
        // Compiler Errors (could be tested with Roslyn)
        //var a = new Age();
        //var a = new Age(400);
        //var b = new Name();
        //var b = new Name("bob");        
    }

    [Fact]
    public void can_bind_and_map() {
        var a = Age.Of(30)
                .Bind<Age, int>(v => v * 2);
        Assert.Equal(30 * 2, a);

        var k = Age.Of(30).Map(v => v * 2);
        AssertValue(60, k);

        var l = Age.Of(30).Bind(v => Success(v * 2));
        Assert.Equal(l,a);
    }
    [Fact]
    public void can_apply() {
        var f = Success<Func<int, int>>(x => x + 1);
        var x = Success(1);
        var y = f.Apply(x);
        AssertValue(2, y);

        var k = Fail<int>("A failure");
        var j = f.Apply(k);
        AssertFailure(j);

        var f2 = Success<Func<int, int, int>>((x, y) => x + y);
        var r = f2.Apply(x).Apply(y);
        AssertValue(2 + 1, r);
    }
    [Fact]
    public void can_construct_and_use_LINQ()
    {
        var a = Age.Of(300);
        AssertFailure(a);

        var s = from bob in Age.Of(80)
                from jon in Age.Of(50)
                select jon + bob;

        AssertValue(130, s);

        var y = from bob in Age.Of(80)
                from jon in Age.Of(50)
                from ron in Age.Of(50)
                select jon + bob + ron;

        AssertValue(180, y);
    }
    [Fact]
    public void can_cast_to_underlying_type()
    {
        var a = Age.Of(60);
        a.ForEach(v => Assert.Equal(60, v), e => Assert.False(true, e.ToString()));

        var r = from bob in Age.Of(80)
                from jon in Name.Of("Jon")
                select (int)bob + jon;

        AssertValue("80Jon", r);
    }
    [Fact]
    public void erroneous_values_cause_failures() {

        var r = from bob in Age.Of(400)
                from jon in Name.Of("Bob")
                select bob + jon;

        AssertFailure(r);

        var k = from bob in Age.Of(40)
                from jon in Age.Of(60)
                from rob in Age.Of(400)
                select bob + jon + rob;

        AssertFailure(k);
    }
    [Fact]
    public void can_create_compound_objects() {

        // Without inter-property validation
        var w = from n in Name.Of("Bob")
                from a in Age.Of(500)
                select new Person(n, a);

        AssertFailure(w); 

        var r = from n in Name.Of("Bob")
                from a in Age.Of(50)
                select new Person(n, a);

        r.ForEach(
            p => Assert.True(p.Age == 50 && p.Name == "Bob"),
            e => Assert.True(false, e.ToString())
        );

        // With inter-property validation

        // One of the properties is invalid
        var w1 = Faker.Of("Wrong", 20, 300);
        AssertFailure(w1);

        // RealAge more than fakeAge
        var w2 = Faker.Of("Wrong", 20, 10);
        AssertFailure(w2);

        // All ok
        var r1 = Faker.Of("Right", 10, 20);
        r1.ForEach(
            f => Assert.True(f.RealAge == 10),
            e => Assert.True(false, e.ToString())
        );

        // Creating props one by one, without an additional helper function
        var na   = Name.Of("john");
        var real = Age.Of(60);
        var fake = Age.Of(70);

        var f1 = from nx in na
                 from rx in real
                 from fx in fake
                 from fa in Faker.Of(nx, rx, fx)
                 select fa;
        
        f1.ForEach(
            f => Assert.True(f.RealAge == 60),
            e => Assert.True(false, e.ToString())
        );


    }

    static void AssertValue<T>(T v, Result<T> rt) =>
        rt.ForEach(t => Assert.Equal(v, t), e => Assert.True(false, e.ToString()));
    static void AssertFailure<T>(Result<T> rt) =>
        rt.ForEach(t => Assert.True(false, t!.ToString()), _ => Assert.True(true));
}
using System;
using Xunit;

using static LFunctional;
using static SimpleTypes;
using static CompoundTypes;

public partial class Tests
{
    Func<int, int?>         i_i = i => i == 0 ? null : i * 2;
    Func<int, string?>      i_s = i => i == 0? null : i.ToString();
    Func<string, string?>   s_s = s => s;
    Func<string, int?>      s_i = s => int.Parse(s);

    [Fact]
    void can_bind() {

        int? y = 2;
        int? z = 0;
        string? s = "2";
        string? r = null;

        // val to val
        Assert.Equal(4, y.Bind(i_i));
        Assert.Null(z.Bind(i_i));

        // val to ref
        Assert.Equal("2", y.Bind(i_s));
        Assert.Null(z.Bind(i_s));

        // ref to ref
        Assert.Equal("2", s.Bind(s_s));
        Assert.Null(r.Bind(s_s));

        // ref to val
        Assert.Equal(2, s.Bind(s_i));
        Assert.Null(r.Bind(s_i));

        // Consecutive bindings
        var x = y
                .Bind(i_i)
                .Bind(i_s)
                .Bind(s_s)
                .Bind(s_i);

        Assert.Equal(4, x);

        var k = z
                .Bind(i_i)
                .Bind(i_s)
                .Bind(s_s)
                .Bind(s_i);

        Assert.Null(k);

    }

    [Fact]
    void can_match() {
        // val to val
        int? y = 2;
        Assert.Equal(2, y.Match(id, () => throw new Exception("No match")));
        int? z = null;
        Assert.Equal(2, z.Match(i => throw new Exception("No match"), () => 2));

        // ref to ref
        string? s = "2";
        Assert.Equal("2", s.Match(id, () => throw new Exception("No match")));
        string? k = null;
        Assert.Equal("2", k.Match(i => throw new Exception("No match"), () => "2"));

        // ref to val
        string? a = "2";
        Assert.Equal(2, a.Match(int.Parse, () => throw new Exception("No match")));
        string? b = null;
        Assert.Equal(2, b.Match(i => throw new Exception("No match"), () => 2));

        // val to ref
        int? c = 2;
        Assert.Equal("2", c.Match(s => s.ToString(), () => throw new Exception("No match")));
        int? d = null;
        Assert.Equal("2", d.Match(i => throw new Exception("No match"), () => "2"));
    }

    [Fact]
    void can_map() {

        // val to val
        int? y = 2;
        Assert.Equal(y, y.Map(id));
        int? z = null;
        Assert.Null(z.Map(i => 3));

        // val to ref
        int? i = 2;
        Assert.Equal(i.ToString(), i.Map(j => j.ToString()));
        int? j = null;
        Assert.Null(j.Map(j => "2"));

        // ref to ref
        string? h = "Bob";
        Assert.Equal("Bob", h.Map(id));
        string? k = null;
        Assert.Null(k.Map(i => "3"));

        // ref to vale
        string? o = "2";
        Assert.Equal(int.Parse(o), o.Map(int.Parse));
        string? t = null;
        Assert.Null(t.Map(i => "3"));
    }
}
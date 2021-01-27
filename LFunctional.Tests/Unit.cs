using System;
using Xunit;
using static LFunctional;

public partial class Tests
{
    [Fact]
    public void JustOneUnit()
    {
        var u1 = Unit();
        var u2 = Unit();
        Assert.Equal(u1, u2);
        Assert.Equal(u1, u1);
    }
}
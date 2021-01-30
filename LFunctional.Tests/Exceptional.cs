
using System;
using Xunit;

using static LFunctional;
using static SimpleTypes;
using static CompoundTypes;
using System.Linq;
using Unit = System.ValueTuple;

public partial class Tests {

    [Fact]
    public void can_wrap_func()
    {
        var r = IO<int, Result<int>>(x => {
            // Any code that can throw
            throw new Exception("test");
        }) (3);
        r.ForEach(_ => Assert.True(false, "Should have failed"));
        r.ForEachFailure(ee => Assert.IsType<ExceptionError>(ee.FirstOrDefault()));
    }
    [Fact]
    public void can_wrap_action()
    {
        var r = IO(() => {
            // Any code that can throw
            throw new Exception("test");
        }) ();
        r.ForEach(_ => Assert.True(false, "Should have failed"));
        r.ForEachFailure(ee => Assert.IsType<ExceptionError>(ee.FirstOrDefault()));
    }
    [Fact]
    public void can_LINQ()
    {
        static string GetFromDB(int id) => id == 0 ? throw new Exception("boom") : id.ToString();
        static void SaveToDB(int id, string changed) {}
        static Result<string> PureFunction(string data) => id(data);
        
        var ident = 3;
        var r = from s  in IO<int, string>(GetFromDB)(ident)
                from d  in PureFunction(s)
                from _1 in IO<int,string>(SaveToDB)(ident, d)
                select d;

        r.ForEachFailure(ee => Assert.True(false, "Can't get here"));
        r.ForEach(d => Assert.Equal(ident.ToString(), d));

        var ident1 = 0;
        var r1 = from s  in IO<int, string>(GetFromDB)(ident1)
                 from d  in PureFunction(s)
                 from _1 in IO<int, string>(SaveToDB)(ident1, d)
                 select d;

        r1.ForEachFailure(ee => Assert.IsType<ExceptionError>(ee.FirstOrDefault()));
        r1.ForEach(d => Assert.True(false, "Can't get here"));

        // Using fields give better syntax (not generic arg on IO)
        Func<int, string> GetFromDB1 =
            id => id == 0 ? throw new Exception("boom") : id.ToString();

        Func<int, string, Unit> SaveToDB1 =
            (id, changed) => Unit(); 
    }
    [Fact]
    public void can_LINQ_fields()
    {
        // Using fields no need for generic parameters on IO
        Func<int, string> GetFromDB =
            id => id == 0 ? throw new Exception("boom") : id.ToString();

        Func<int, string, Unit>SaveToDB =
            (id, changed) => Unit(); 

        Result<string> PureFunction(string data) => id(data);
        
        var ident = 3;
        var r = from s  in IO(GetFromDB)(ident)
                from d  in PureFunction(s)
                from _1 in IO(SaveToDB)(ident, d)
                select d;

        r.ForEachFailure(ee => Assert.True(false, "Can't get here"));
        r.ForEach(d => Assert.Equal(ident.ToString(), d));

        var ident1 = 0;
        var r1 = from s  in IO(GetFromDB)(ident1)
                 from d  in PureFunction(s)
                 from _1 in IO(SaveToDB)(ident1, d)
                 select d;

        r1.ForEachFailure(ee => Assert.IsType<ExceptionError>(ee.FirstOrDefault()));
        r1.ForEach(d => Assert.True(false, "Can't get here"));
    }
}
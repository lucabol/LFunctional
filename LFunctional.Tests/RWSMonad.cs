using System;
using System.Collections.Generic;
using Unit = System.ValueTuple;
using static LFunctional;

record State(string ConnectionString) {

    Result<string> DoSideEffectingStuff(int id) =>
        from c in OpenConnection(ConnectionString)
        from r in Query(c)
        from _ in Log("logged something")
        select r;

    static Result<string> OpenConnection(string s) => "an opened connection";
    static Result<string> Query(object o) => "a result";

    string aLog = "";
    Result<Unit> Log(string s) { aLog +=s; return new System.ValueTuple();}

    static void MyMain() {
        var r = new State("myserver").DoSideEffectingStuff(3);
        // ...    
    }
}
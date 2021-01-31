
using System;
using System.Collections.Generic;
using Unit = System.ValueTuple;

abstract record Result<T>();
record OK<T>(T Value): Result<T>;
record Fail<T>(string Error): Result<T>;

record State(string ConnectionString) {

    Result<string> DoSideEffectingStuff(int id) =>
        from c in OpenConnection(ConnectionString)
        from r in Query(c)
        from _ in Log("logged something")
        select r;

    static Result<string> OpenConnection(string s) => new OK<string>("an opened connection");
    static Result<string> Query(object o) => new OK<string>("a result");

    string aLog = "";
    Result<Unit> Log(string s) { return new OK<Unit>(new ());}

    static void MyMain() {
        var r = new State("myserver").DoSideEffectingStuff(3);
        // ...    
    }
}
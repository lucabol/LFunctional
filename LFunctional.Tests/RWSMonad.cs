
using System;
using System.Collections.Generic;

abstract record Result<T>();
record OK<T>(T Value): Result<T>;
record Fail<T>(string Error): Result<T>;

record State(string ConnectionString) {

    Result<string> DoSideEffectingStuff(int id) =>
        from c in OpenConnection(ConnectionString)
        from r in Query(c)
        from _ in Log(r)
        select new OK<string>(r);

    static object OpenConnection(string s) => "an opened connection";
    static string Query(object o) => "a result";

    string aLog = "";
    string Log(string s) { return aLog += s;}

    static void MyMain() {
        var r = new State("myserver").DoSideEffectingStuff(3);
        // ...    
    }
}
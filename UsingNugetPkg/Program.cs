using System;
using Astator.Script;
using Jint;

namespace UsingNugetPkg;

public class Program
{
    [EntryMethod]
    public static void Main(string workDir)
    {
        var engine = new Engine().SetValue("log", new Action<object>(Console.WriteLine));

        engine.Execute(
            @"
    function hello() { 
        log('Hello World form nuget package: Jint');
    };
 
    hello();
"
        );
    }
}

using System;
using System.Collections.Generic;
using Arango.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Arango.Console
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            /*ArangoClient.AddConnection(
                "localhost",
                8529,
                false,
                "test",
                "test"
            );

            var db = new ArangoDatabase("test");

            var aql = db.Query.ToString();

            System.Console.WriteLine(aql);*/

            ArangoQueryOperation expression = new ArangoQueryOperation()
                .FOR("foo").IN("col", ctx => ctx
                    .FILTER("foo.bar")
                    .LET("ddd").Value("aaa")
                    .FOR("bar").IN("col2", ctx2 => ctx2
                        .FILTER("bar.foo")
                        .LET("xxx").Variable("aaa")
                        .LET("xxx", ctx3 => ctx3
                            .FOR("foo").IN(ctx4 => ctx4
                                .RETURN("bbb"))
                        )
                        .RETURN("foo"))
                );

            System.Console.WriteLine(expression.ToString());

            System.Console.ReadLine();
        }
    }
}


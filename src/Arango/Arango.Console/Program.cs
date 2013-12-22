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
                .FOR("foo").IN(ctx => ctx.Collection("col")
                    .FILTER("foo.bar")
                    .LET("val").Value("aaa")
                    .LET("var").Variable("foo.bar")
                    .LET("docVar").DOCUMENT("foo.bar")
                    .LET("doc").DOCUMENT("aaa/123")
                    .LET("docs").DOCUMENT("aaa/123", "aaa/345")
                    .FOR("bar").IN(ctx2 => ctx2.List("one", 2)
                        .FILTER("bar.foo")
                        .LET("xxx", ctx3 => ctx3
                            .FOR("foo").IN(ctx4 => ctx4.EDGES("colx", "colc/123", ArangoEdgeDirection.Out)
                                .FOR("foo").IN(ctx5 => ctx5.EDGES("colx", "xyz", ArangoEdgeDirection.Any)
                                    .RETURN("bbb")))
                        )
                        .RETURN("foo"))
                );

            System.Console.WriteLine(expression.ToString());

            System.Console.ReadLine();
        }
    }
}


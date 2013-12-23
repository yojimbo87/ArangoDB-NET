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
                .FOR("foo").Collection("col", ctx => ctx
                    .FILTER("foo.bar")
                    .LET("val").Value("aaa")
                    .LET("var").Variable("foo.bar")
                    .LET("obj").Object(ctxx => ctxx
                        .Field("x").Value("y")
                    )
                    .LET("array").List("x", "y", "z")
                    .LET("docVar").DOCUMENT("foo.bar")
                    .LET("docId").DOCUMENT("aaa/123")
                    .LET("docIds").DOCUMENT("aaa/123", "aaa/345")
                    .FOR("bar").List(new List<object> { "one", "two", "three" }, ctx2 => ctx2
                        .FILTER("bar.foo")
                        .LET("xxx").List(ctx3 => ctx3
                            .FOR("foo").EDGES("colx", "colc/123", ArangoEdgeDirection.Out, ctx4 => ctx4
                                .FOR("foo").EDGES("colx", "xyz", ArangoEdgeDirection.Any, ctx5 => ctx5
                                    .RETURN.List(7, 8, 9))))
                        .RETURN.Object(ctx6 => ctx6
                            .Field("foo").Variable("var")
                            .Field("bar").Value("val")
                            .Field("baz").List(1, 2, 3 )
                            .Field("boo").List(ctx7 => ctx7
                                .FOR("x").Collection("coly", ctx8 => ctx8
                                    .FOR("y").Variable("whoa", ctx9 => ctx9
                                        .RETURN.Variable("var")))
                            )
                            .Field("obj").Object(ctx10 => ctx10
                                .Field("foo").Value("bar")
                            )
                            .Field("xxx").Value("yyy")
                        ))
                );

            System.Console.WriteLine(expression.ToString());

            System.Console.ReadLine();
        }
    }
}


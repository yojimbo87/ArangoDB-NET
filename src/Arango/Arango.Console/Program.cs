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
                /*.FOR("foo").Collection("col", ctx => ctx
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
                    .LET("firstVar").FIRST(ctxx => ctxx.Variable("var"))
                    .LET("firstList").FIRST(ctxx => ctxx.List(1, 2, 3))
                    .LET("firstListContext").FIRST(ctxx => ctxx.List(ctxy => ctxy
                        .FOR("foo").Collection("bar", ctxz => ctxz
                            .RETURN.Value("abcd"))
                    ))
                    .FOR("bar").List(new List<object> { "one", "two", "three" }, ctx2 => ctx2
                        .FILTER("bar.foo")
                        .LET("xxx").List(ctx3 => ctx3
                            .FOR("foo").EDGES("colx", "colc/123", ArangoEdgeDirection.Out, ctx4 => ctx4
                                .FOR("foo").EDGES("colx", "xyz", ArangoEdgeDirection.Any, ctx5 => ctx5
                                    .RETURN.List(7, 8, 9))))
                        .RETURN.Object(ctx6 => ctx6
                            .Field("foo").Variable("var")
                            .Field("bar").Value("val")
                            .Field("baz").List(1, 2, 3)
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
                );*/
                .Aql(_ => _
            	    .LET("val1").Value("string")
            	    .LET("val2").Value(123)
                    .LET("list1").List(new List<object> { 1, 2, 3})
                    .LET("list2").List(_
                        .LET("val11").Value("sss")
                        .RETURN.Value("abcd")
                    )
                    .LET("obj").Object(_
                        .Field("x").Value("y")
                    )
                    .LET("docVar").DOCUMENT("foo.bar")
                    .LET("docId").DOCUMENT("aaa/123")
                    .LET("docIds").DOCUMENT("aaa/123", "aaa/345")
                    .LET("xxx").List(_
                        .FOR("foo").EDGES("colx", "colc/123", ArangoEdgeDirection.Out, _
                            .FOR("foo").EDGES("colx", "xyz", ArangoEdgeDirection.Any, _
                                .RETURN.List(new List<object> {"one", "two", "three" }))))
                    .LET("firstList").FIRST(_.List(new List<object> { 1, 2, 3}))
                    .LET("firstListContext").FIRST(_.List(_
                        .FOR("foo").IN("bar", _
                            .LET("xxx").Value("abcd")
                            .RETURN.Variable("xxx"))
                    ))
                    .FOR("foo1").IN("col1", _
                        .LET("val11").Value("string")
                        .LET("val12").Value(123)
                        .LET("list11").List(new List<object> { 1, 2, 3})
                        .LET("list12").List(_
                            .LET("val21").Value("sss")
                            .LET("val22").Value(345)
                            .RETURN.Object(_
                                .Field("foo").Variable("var")
                                .Field("bar").Value("val")
                                .Field("baz").List(new List<object> { 1, 2, 3})
                                .Field("boo").List(_
                                    .FOR("x").IN("coly", _
                                        .FOR("y").IN("whoa", _
                                            .RETURN.Variable("var")))
                                )
                                .Field("obj").Object(_
                                    .Field("foo").Value("bar")
                                )
                                .Field("xxx").Value("yyy")
                            )
                        )
                        .RETURN.Variable("list12"))
                );
            System.Console.WriteLine(expression.ToString());

            System.Console.ReadLine();
        }
    }
}


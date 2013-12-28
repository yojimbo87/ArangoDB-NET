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
                    .LET("val").Val("aaa")
                    .LET("var").Var("foo.bar")
                    .LET("obj").Object(ctxx => ctxx
                        .Field("x").Val("y")
                    )
                    .LET("array").List("x", "y", "z")
                    .LET("docVar").DOCUMENT("foo.bar")
                    .LET("docId").DOCUMENT("aaa/123")
                    .LET("docIds").DOCUMENT("aaa/123", "aaa/345")
                    .LET("firstVar").FIRST(ctxx => ctxx.Var("var"))
                    .LET("firstList").FIRST(ctxx => ctxx.List(1, 2, 3))
                    .LET("firstListContext").FIRST(ctxx => ctxx.List(ctxy => ctxy
                        .FOR("foo").Collection("bar", ctxz => ctxz
                            .RETURN.Val("abcd"))
                    ))
                    .FOR("bar").List(new List<object> { "one", "two", "three" }, ctx2 => ctx2
                        .FILTER("bar.foo")
                        .LET("xxx").List(ctx3 => ctx3
                            .FOR("foo").EDGES("colx", "colc/123", ArangoEdgeDirection.Out, ctx4 => ctx4
                                .FOR("foo").EDGES("colx", "xyz", ArangoEdgeDirection.Any, ctx5 => ctx5
                                    .RETURN.List(7, 8, 9))))
                        .RETURN.Object(ctx6 => ctx6
                            .Field("foo").Var("var")
                            .Field("bar").Val("val")
                            .Field("baz").List(1, 2, 3)
                            .Field("boo").List(ctx7 => ctx7
                                .FOR("x").Collection("coly", ctx8 => ctx8
                                    .FOR("y").Var("whoa", ctx9 => ctx9
                                        .RETURN.Var("var")))
                            )
                            .Field("obj").Object(ctx10 => ctx10
                                .Field("foo").Val("bar")
                            )
                            .Field("xxx").Val("yyy")
                        ))
                );*/
                .Aql(_ => _
                    .LET("concat1").CONCAT(_.Val("xxx"), _.Val(5), _.Var("foo"), _.TO_STRING(_.Var("bar")))
            	    .LET("val1").Val("string")
            	    .LET("val2").Val(123)
                    .LET("list1").List(1, 2, 3)
                    .LET("list2").List(new List<object> { 4, 5, 6})
                    .LET("list3").List(_
                        .LET("val11").Val("sss")
                        .Aql("LET val12 = 'whoa'")
                        .RETURN.Val("abcd")
                    )
                    .LET("obj").Object(_
                        .Field("x").Val("y")
                    )
                    .LET("boolVar").TO_BOOL(_.Var("b"))
                    .LET("boolVal").TO_BOOL(_.Val(0))
                    .LET("listVar").TO_LIST(_.Var("b"))
                    .LET("listVal").TO_LIST(_.Val("a"))
                    .LET("numberVar").TO_NUMBER(_.Var("b"))
                    .LET("numberVal").TO_NUMBER(_.Val("3"))
                    .LET("stringVar").TO_STRING(_.Var("b"))
                    .LET("stringVal").TO_NUMBER(_.Val(4))
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
                            .LET("xxx").Val("abcd")
                            .RETURN.Var("xxx"))
                    ))
                    .FOR("foo1").IN("col1", _
                        .LET("val11").Val("string")
                        .LET("val12").Val(123)
                        .LET("list11").List(new List<object> { 1, 2, 3})
                        .LET("list12").List(_
                            .LET("val21").Val("sss")
                            .LET("val22").Val(345)
                            .RETURN.Object(_
                                .Field("foo").Var("var")
                                .Field("bar").Val("val")
                                .Field("baz").List(new List<object> { 1, 2, 3})
                                .Field("boo").List(_
                                    .FOR("x").IN("coly", _
                                        .FOR("y").IN("whoa", _
                                            .RETURN.Var("var")))
                                )
                                .Field("obj").Object(_
                                    .Field("foo").Val("bar")
                                )
                                .Field("xxx").Val("yyy")
                            )
                        )
                        .FILTER(
                            _.Var("val11"), ArangoOperator.Greater, _.Val(123)
                        ).AND(
                            _.Var("val12"), ArangoOperator.Equal, _.Val("foo")
                        ).OR(
                            _.Val(44), ArangoOperator.In, _.Var("val13")
                        )
                        .COLLECT("city = u.city")
                        .COLLECT("first = u.firstName, age = u.age").INTO("g")
                        .SORT(_.Var("var1")).Direction(ArangoSortDirection.ASC)
                        .SORT(_.Var("var1"), _.TO_NUMBER(_.Var("var2"))).Direction(ArangoSortDirection.DESC)
                        .LIMIT(5)
                        .LIMIT(0, 5)
                        .LIMIT("@count")
                        .LIMIT("@offset", "@count")
                        .RETURN.Var("list12"))
                );

            var pretty = expression.ToString();
            System.Console.WriteLine(pretty);
            System.Console.WriteLine(pretty.Length);

            var dirty = expression.ToString(false);
            System.Console.WriteLine("\n" + dirty);
            System.Console.WriteLine(dirty.Length);

            System.Console.ReadLine();
        }
    }
}


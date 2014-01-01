using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.QueryTests
{
    [TestFixture()]
    public class AqlTests
    {
        [Test()]
        public void Should_generate_query()
        {
            var prettyPrintQuery = 
                "LET concat1 = CONCAT('xxx', 5, foo, TO_STRING(bar))\n" +
                "LET val1 = 'string'\n" +
                "LET val2 = 123\n" +
                "LET lower = LOWER('ABC')\n" +
                "LET upper = UPPER(foo)\n" +
                "LET list1 = [1, 2, 3]\n" +
                "LET list2 = [4, 5, 6]\n" +
                "LET list3 = (\n" +
                "    LET val11 = 'sss'\n" +
                "    LET val12 = 'whoa'\n" +
                "    RETURN 'abcd'\n" +
                ")\n" +
                "LET len1 = LENGTH(var1)\n" +
                "LET len2 = LENGTH([1, 2, 3])\n" +
                "LET contains1 = CONTAINS('abc', foo)\n" +
                "LET contains2 = CONTAINS('abc', foo, true)\n" +
                "LET obj = {\n" +
                "    'x': 'y'\n" +
                "}\n" +
                "LET boolVar = TO_BOOL(b)\n" +
                "LET boolVal = TO_BOOL(0)\n" +
                "LET listVar = TO_LIST(b)\n" +
                "LET listVal = TO_LIST('a')\n" +
                "LET numberVar = TO_NUMBER(b)\n" +
                "LET numberVal = TO_NUMBER('3')\n" +
                "LET stringVar = TO_STRING(b)\n" +
                "LET stringVal = TO_NUMBER(4)\n" +
                "LET docVar = DOCUMENT(foo.bar)\n" +
                "LET docId = DOCUMENT('aaa/123')\n" +
                "LET docIds = DOCUMENT(['aaa/123', 'aaa/345'])\n" +
                "LET xxx = (\n" +
                "    FOR foo EDGES(colx, 'colc/123', outbound)\n" +
                "        FOR foo EDGES(colx, xyz, any)\n" +
                "            RETURN ['one', 'two', 'three']\n" +
                ")\n" +
                "LET firstList = FIRST([1, 2, 3])\n" +
                "LET firstListContext = FIRST((\n" +
                "    FOR foo IN bar\n" +
                "        LET xxx = 'abcd'\n" +
                "        RETURN xxx\n" +
                "))\n" +
                "FOR foo1 IN col1\n" +
                "    LET val11 = 'string'\n" +
                "    LET val12 = 123\n" +
                "    LET list11 = [1, 2, 3]\n" +
                "    LET list12 = (\n" +
                "        LET val21 = 'sss'\n" +
                "        LET val22 = 345\n" +
                "        RETURN {\n" +
                "            'foo': var,\n" +
                "            'bar': 'val',\n" +
                "            'baz': [1, 2, 3],\n" +
                "            'boo': (\n" +
                "                FOR x IN coly\n" +
                "                    FOR y IN whoa\n" +
                "                        RETURN var\n" +
                "            ),\n" +
                "            'obj': {\n" +
                "                'foo': 'bar'\n" +
                "            },\n" +
                "            'xxx': 'yyy'\n" +
                "        }\n" +
                "    )\n" +
                "    FILTER val11 > 123 && val12 == 'foo' || 44 IN val13\n" + 
                "    COLLECT city = u.city\n" +
                "    COLLECT first = u.firstName, age = u.age INTO g\n" +
                "    SORT var1 ASC\n" +
                "    SORT var1, TO_NUMBER(var2) DESC\n" +
                "    LIMIT 5\n" +
                "    LIMIT 0, 5\n" +
                "    LIMIT @count\n" +
                "    LIMIT @offset, @count\n" +
                "    RETURN list12";

            var shrotQuery = "LET concat1 = CONCAT('xxx', 5, foo, TO_STRING(bar)) LET val1 = 'string' LET val2 = 123 LET lower = LOWER('ABC') LET upper = UPPER(foo) LET list1 = [1, 2, 3] LET list2 = [4, 5, 6] LET list3 = ( LET val11 = 'sss' LET val12 = 'whoa' RETURN 'abcd' ) LET len1 = LENGTH(var1) LET len2 = LENGTH([1, 2, 3]) LET contains1 = CONTAINS('abc', foo) LET contains2 = CONTAINS('abc', foo, true) LET obj = { 'x': 'y' } LET boolVar = TO_BOOL(b) LET boolVal = TO_BOOL(0) LET listVar = TO_LIST(b) LET listVal = TO_LIST('a') LET numberVar = TO_NUMBER(b) LET numberVal = TO_NUMBER('3') LET stringVar = TO_STRING(b) LET stringVal = TO_NUMBER(4) LET docVar = DOCUMENT(foo.bar) LET docId = DOCUMENT('aaa/123') LET docIds = DOCUMENT(['aaa/123', 'aaa/345']) LET xxx = ( FOR foo EDGES(colx, 'colc/123', outbound) FOR foo EDGES(colx, xyz, any) RETURN ['one', 'two', 'three'] ) LET firstList = FIRST([1, 2, 3]) LET firstListContext = FIRST(( FOR foo IN bar LET xxx = 'abcd' RETURN xxx )) FOR foo1 IN col1 LET val11 = 'string' LET val12 = 123 LET list11 = [1, 2, 3] LET list12 = ( LET val21 = 'sss' LET val22 = 345 RETURN { 'foo': var, 'bar': 'val', 'baz': [1, 2, 3], 'boo': ( FOR x IN coly FOR y IN whoa RETURN var ), 'obj': { 'foo': 'bar' }, 'xxx': 'yyy' } ) FILTER val11 > 123 && val12 == 'foo' || 44 IN val13 COLLECT city = u.city COLLECT first = u.firstName, age = u.age INTO g SORT var1 ASC SORT var1, TO_NUMBER(var2) DESC LIMIT 5 LIMIT 0, 5 LIMIT @count LIMIT @offset, @count RETURN list12";
            
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                    .LET("concat1").CONCAT(_.Val("xxx"), _.Val(5), _.Var("foo"), _.TO_STRING(_.Var("bar")))
                    .LET("val1").Val("string")
                    .LET("val2").Val(123)
                    .LET("lower").LOWER(_.Val("ABC"))
                    .LET("upper").UPPER(_.Var("foo"))
                    .LET("list1").List(1, 2, 3)
                    .LET("list2").List(new List<object> { 4, 5, 6})
                    .LET("list3").List(_
                        .LET("val11").Val("sss")
                        .Aql("LET val12 = 'whoa'")
                        .RETURN.Val("abcd")
                    )
                    .LET("len1").LENGTH(_.Var("var1"))
                    .LET("len2").LENGTH(_.List(1, 2, 3))
                    .LET("contains1").CONTAINS(_.Val("abc"), _.Var("foo"))
                    .LET("contains2").CONTAINS(_.Val("abc"), _.Var("foo"), true)
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
            
            Assert.AreEqual(prettyPrintQuery, expression.ToString());
            Assert.AreEqual(shrotQuery, expression.ToString(false));
        }
    }
}

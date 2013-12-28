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
                "LET concat1 = CONCAT('xxx', foo, 5)\n" +
                "LET val1 = 'string'\n" +
                "LET val2 = 123\n" +
                "LET list1 = [1, 2, 3]\n" +
                "LET list2 = [4, 5, 6]\n" +
                "LET list3 = (\n" +
                "    LET val11 = 'sss'\n" +
                "    LET val12 = 'whoa'\n" +
                "    RETURN 'abcd'\n" +
                ")\n" +
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
                "    RETURN list12";

            var shrotQuery = "LET concat1 = CONCAT('xxx', foo, 5) LET val1 = 'string' LET val2 = 123 LET list1 = [1, 2, 3] LET list2 = [4, 5, 6] LET list3 = ( LET val11 = 'sss' LET val12 = 'whoa' RETURN 'abcd' ) LET obj = { 'x': 'y' } LET boolVar = TO_BOOL(b) LET boolVal = TO_BOOL(0) LET listVar = TO_LIST(b) LET listVal = TO_LIST('a') LET numberVar = TO_NUMBER(b) LET numberVal = TO_NUMBER('3') LET stringVar = TO_STRING(b) LET stringVal = TO_NUMBER(4) LET docVar = DOCUMENT(foo.bar) LET docId = DOCUMENT('aaa/123') LET docIds = DOCUMENT(['aaa/123', 'aaa/345']) LET xxx = ( FOR foo EDGES(colx, 'colc/123', outbound) FOR foo EDGES(colx, xyz, any) RETURN ['one', 'two', 'three'] ) LET firstList = FIRST([1, 2, 3]) LET firstListContext = FIRST(( FOR foo IN bar LET xxx = 'abcd' RETURN xxx )) FOR foo1 IN col1 LET val11 = 'string' LET val12 = 123 LET list11 = [1, 2, 3] LET list12 = ( LET val21 = 'sss' LET val22 = 345 RETURN { 'foo': var, 'bar': 'val', 'baz': [1, 2, 3], 'boo': ( FOR x IN coly FOR y IN whoa RETURN var ), 'obj': { 'foo': 'bar' }, 'xxx': 'yyy' } ) RETURN list12";
            
            ArangoQueryOperation expression = new ArangoQueryOperation()
                .Aql(_ => _
                    .LET("concat1").CONCAT(_.Value("xxx"), _.Variable("foo"), _.Value(5))
                    .LET("val1").Value("string")
                    .LET("val2").Value(123)
                    .LET("list1").List(1, 2, 3)
                    .LET("list2").List(new List<object> { 4, 5, 6})
                    .LET("list3").List(_
                        .LET("val11").Value("sss")
                        .Aql("LET val12 = 'whoa'")
                        .RETURN.Value("abcd")
                    )
                    .LET("obj").Object(_
                        .Field("x").Value("y")
                    )
                    .LET("boolVar").TO_BOOL(_.Variable("b"))
                    .LET("boolVal").TO_BOOL(_.Value(0))
                    .LET("listVar").TO_LIST(_.Variable("b"))
                    .LET("listVal").TO_LIST(_.Value("a"))
                    .LET("numberVar").TO_NUMBER(_.Variable("b"))
                    .LET("numberVal").TO_NUMBER(_.Value("3"))
                    .LET("stringVar").TO_STRING(_.Variable("b"))
                    .LET("stringVal").TO_NUMBER(_.Value(4))
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
            
            Assert.AreEqual(prettyPrintQuery, expression.ToString());
            Assert.AreEqual(shrotQuery, expression.ToString(false));
        }
    }
}

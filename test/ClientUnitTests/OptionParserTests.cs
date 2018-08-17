using System;
using ClientLib;
using NUnit.Framework;

namespace ClientUnitTests
{
    // https://haacked.com/archive/2012/01/02/structuring-unit-tests.aspx/
    public class TheSenderOptionsItemParser
    {
        [Test]
        public void ParsesEqualsStringAsString()
        {
            Assert.AreEqual(ValueTuple.Create("key", "aString"), SenderOptions.ParseItem("key=aString"));
        }

        [Test]
        public void ParsesEqualsIntAsString()
        {
            Assert.AreEqual(ValueTuple.Create("key", "42"), SenderOptions.ParseItem("key=42"));
        }

        [Test]
        public void ParsesEqualsTildeAsString()
        {
            Assert.AreEqual(ValueTuple.Create("key", "~1"), SenderOptions.ParseItem("key=~1"));
        }

        [Test]
        public void ParsesDoubleTildeAsString()
        {
            Assert.AreEqual(ValueTuple.Create("key", "~1"), SenderOptions.ParseItem("key~~1"));
        }

        [Test]
        public void ParsesDoubleEqualsAsString()
        {
            Assert.AreEqual(ValueTuple.Create("key", "=1"), SenderOptions.ParseItem("key==1"));
        }

        [Test]
        public void ParsesTildeStringAsString()
        {
            Assert.AreEqual(ValueTuple.Create("key", "aString"), SenderOptions.ParseItem("key~aString"));
        }
    }
}
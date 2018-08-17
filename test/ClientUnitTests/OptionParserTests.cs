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
    }
}
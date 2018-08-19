using System;
using ClientLib;
using NUnit.Framework;

namespace ClientUnitTests
{
    // https://haacked.com/archive/2012/01/02/structuring-unit-tests.aspx/
    public class TheSenderOptionsItemParser
    {
        [Test]
        public void ThrowsOnEmptyString()
        {
            Assert.Throws<ArgumentException>(() => SenderOptions.ParseItem(""));
        }
        
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

        [Test]
        public void ParsesEqualsEmptyAsEmptyString()
        {
            Assert.AreEqual(ValueTuple.Create("key", ""), SenderOptions.ParseItem("key="));
        }

        /// <summary>
        /// Note: null key is not supported py ParseItem
        /// </summary>
        [Test]
        public void ParsesEmptyEqualsAsEmptyStringKey()
        {
            Assert.AreEqual(ValueTuple.Create("", "aString"), SenderOptions.ParseItem("=aString"));
        }
        
        [Test]
        public void ParsesTildeEmptyAsNull()
        {
            Assert.AreEqual(ValueTuple.Create("key", (String) null), SenderOptions.ParseItem("key~"));
        }

        [Test]
        public void ParsesTildeDoubleAsDouble()
        {
            Assert.AreEqual(ValueTuple.Create("key", 3.14d), SenderOptions.ParseItem("key~3.14"));
        }

        [Test]
        public void ParsesTildeBoolAsBool()
        {
            Assert.AreEqual(ValueTuple.Create("key", false), SenderOptions.ParseItem("key~False"));
        }

        [Test]
        public void ParseTildeStringAsString()
        {
            Assert.AreEqual(ValueTuple.Create("key", "aString"), SenderOptions.ParseItem("key~aString"));
        }

        [Test]
        public void ParsesTildeIntAsInt()
        {
            Assert.AreEqual(ValueTuple.Create("key", 42), SenderOptions.ParseItem("key~42"));
        }
    }

    public class TheSenderOptionsValueParser
    {
        [Test]
        public void ParsesEmtpyStringAsEmptyString()
        {
            Assert.AreEqual(string.Empty, SenderOptions.ParseValue(""));
        }

        [Test]
        public void ParsesTildeAsNull()
        {
            Assert.AreEqual(null, SenderOptions.ParseValue("~"));
        }

        [Test]
        public void ParsesDoubleTildeAsString()
        {
            Assert.AreEqual("~", SenderOptions.ParseValue("~~"));
        }

        [Test]
        public void ParsesTildeIntAsInt()
        {
            Assert.AreEqual(42, SenderOptions.ParseValue("~42"));
        }
    }
}
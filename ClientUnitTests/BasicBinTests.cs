/*
 * Copyright 2017 Red Hat Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using NUnit.Framework;

namespace ClientUnitTests
{
    [TestFixture]
    public class BasicBinTests
    {
        private ClientRunner clientRunner = new ClientRunner();

        [Test]
        public void TestTrySenderHelp()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--help"));
        }

        [Test]
        public void TestTryReceiverHelp()
        {
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--help"));
        }

        [Test]
        public void TestTryConnectorHelp()
        {
            Assert.AreEqual(0, this.clientRunner.RunConnector("--help"));
        }

        [Test]
        public void TestSenderWrongArgument()
        {
            Assert.AreEqual(2, this.clientRunner.RunSender("--foo"));
        }

        [Test]
        public void TestReceiverWrongArgument()
        {
            Assert.AreEqual(2, this.clientRunner.RunReceiver("--foo"));
        }

        [Test]
        public void TestConnectorWrongArgument()
        {
            Assert.AreEqual(2, this.clientRunner.RunConnector("--foo"));
        }
    }
}

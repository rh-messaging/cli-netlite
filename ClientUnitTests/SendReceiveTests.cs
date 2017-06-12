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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClientUnitTests
{
    [TestClass]
    public class SendReceiveTests
    {
        ClientRunner clientRunner = new ClientRunner();

        [TestMethod]
        public void TestSendMessage()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address send_example --count 1"));
        }

        [TestMethod]
        public void TestSendReceiveMessage()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address send_receive_example --count 1"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address send_receive_example --count 1"));
        }

        [TestMethod]
        public void TestConnectorClient()
        {
            Assert.AreEqual(0, this.clientRunner.RunConnector("--address connector_example --count 5 --timeout 5 --obj-ctrl CESR"));
        }
    }
}

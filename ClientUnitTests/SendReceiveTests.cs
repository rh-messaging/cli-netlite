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
using System.Threading.Tasks;
using NUnit.Framework;


namespace ClientUnitTests
{
    /// <summary>
    /// Class with bin unit tests
    /// </summary>
    [TestFixture]
    public class SendReceiveTests
    {
        /// <summary>
        /// private clientRunner, which control execution of clients
        /// </summary>
        ClientRunner clientRunner = new ClientRunner();

        [Test]
        public void TestSendMessage()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address send_example --count 1"));
        }

        [Test]
        public void TestSendReceiveMessage()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address send_receive_example --count 1"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address send_receive_example --count 1"));
        }

        [Test]
        public void TestConnectorClient()
        {
            Assert.AreEqual(0, this.clientRunner.RunConnector("--address connector_example --count 5 --timeout 5 --obj-ctrl CESR"));
        }

        [Test]
        public void TestDrainQueue()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address drain_queue --count 10"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address drain_queue --count 0"));
        }

        [Test]
        public void TestBrowseQueue()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address browse_queue --count 10"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address browse_queue --count 10 --recv-browse true"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address browse_queue --count 10"));
        }

        [Test]
        public void TestReceiverSelector()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address selector_queue --count 2 --msg-property 'colour~red'"));
            Assert.AreEqual(0, this.clientRunner.RunSender("--address selector_queue --count 2 --msg-property 'colour~blue'"));
            this.clientRunner.RunReceiver("--timeout 2 --address selector_queue --count 0 --msg-selector \"colour=red\"");
            this.clientRunner.RunReceiver("--timeout 2 --address selector_queue --count 0 --msg-selector \"colour=blue\"");
        }

        [Test]
        public void TestSendReceiveMapContent()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address send_receive_map_example --count 1 --msg-content-map-item \"item1=value1\" --msg-content-map-item \"item2~5\""));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address send_receive_map_example --count 1"));
        }

        [Test]
        public void TestSendReceiveListContent()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address send_receive_list_example --count 1 --msg-content-list-item 5 --msg-content-list-item text"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address send_receive_list_example --count 1"));
        }

        [Test]
        public void TestSendReceiveStringContent()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address send_receive_string_example --count 1 --msg-content string_message"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address send_receive_string_example --count 1"));
        }

        [Test]
        public void TestDrainEmptyQueue()
        {
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address empty_queue --count 0"));
        }

        [Test]
        public void TestTransactCommit()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address transaction_commit --count 10 --msg-content string_message --tx-size 2 --tx-action commit"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address transaction_commit --count 10 --tx-size 5 --tx-action commit"));
        }

        [Test]
        public void TestTransactCommitRetire()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address transaction_commit_retire --count 10 --msg-content string_message --tx-size 2 --tx-action commit"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address transaction_commit_retire --count 10 --tx-size 3 --tx-action commit --tx-endloop-action retire"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address transaction_commit_retire --count 1"));
        }

        [Test]
        public void TestTransactRetire()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address transaction_retire --count 10 --msg-content string_message --tx-size 2 --tx-action commit"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address transaction_retire --count 10 --tx-size 5 --tx-action retire"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address transaction_retire --count 10"));
        }

        [Test]
        public void TestP2P()
        {
            Task listener = Task.Run(() => {
                Assert.AreEqual(0, this.clientRunner.RunReceiver("--recv-listen true --recv-listen-port 8888 --count 10"));
            });
            Assert.AreEqual(0, this.clientRunner.RunSender("--broker localhost:8888 --count 10"));
            Task.WaitAll(listener);
        }
    }
}

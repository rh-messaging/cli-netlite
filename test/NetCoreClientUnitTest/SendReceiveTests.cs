using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace NetCoreClientUnitTest
{
    [TestClass]
    public class SendReceiveTests
    {
        /// <summary>
        /// private clientRunner, which control execution of clients
        /// </summary>
        ClientRunner clientRunner = new ClientRunner();

        [TestMethod]
        public void TestSendMessageNetCore()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address send_example --count 1"));
        }

        [TestMethod]
        public void TestSendReceiveMessageNetCore()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address send_receive_example --count 1"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address send_receive_example --count 1"));
        }

        //[TestMethod]
        public void TestConnectorClientNetCore()
        {
            Assert.AreEqual(0, this.clientRunner.RunConnector("--address connector_example --count 5 --timeout 5 --obj-ctrl CESR"));
        }

        [TestMethod]
        public void TestDrainQueueNetCore()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address drain_queue --count 10"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address drain_queue --count 0"));
        }

        [TestMethod]
        public void TestBrowseQueueNetCore()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address browse_queue --count 10"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address browse_queue --count 10 --recv-browse true"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address browse_queue --count 10"));
        }

        [TestMethod]
        public void TestReceiverSelectorNetCore()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address selector_queue --count 2 --msg-property 'colour~red'"));
            Assert.AreEqual(0, this.clientRunner.RunSender("--address selector_queue --count 2 --msg-property 'colour~blue'"));
            this.clientRunner.RunReceiver("--timeout 2 --address selector_queue --count 0 --msg-selector \"colour=red\"");
            this.clientRunner.RunReceiver("--timeout 2 --address selector_queue --count 0 --msg-selector \"colour=blue\"");
        }

        [TestMethod]
        public void TestSendReceiveMapContentNetCore()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address send_receive_map_example --count 1 --msg-content-map-item \"item1=value1\" --msg-content-map-item \"item2~5\""));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address send_receive_map_example --count 1"));
        }

        [TestMethod]
        public void TestSendReceiveListContentNetCore()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address send_receive_list_example --count 1 --msg-content-list-item 5 --msg-content-list-item text"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address send_receive_list_example --count 1"));
        }

        [TestMethod]
        public void TestSendReceiveStringContentNetCore()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address send_receive_string_example --count 1 --msg-content string_message"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address send_receive_string_example --count 1"));
        }

        [TestMethod]
        public void TestDrainEmptyQueueNetCore()
        {
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address empty_queue --count 0"));
        }

        [TestMethod]
        public void TestTransactCommitNetCore()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address transaction_commit --count 10 --msg-content string_message --tx-size 2 --tx-action commit"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address transaction_commit --count 10 --tx-size 5 --tx-action commit"));
        }

        [TestMethod]
        public void TestTransactCommitRetireNetCore()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address transaction_commit_retire --count 10 --msg-content string_message --tx-size 2 --tx-action commit"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address transaction_commit_retire --count 10 --tx-size 3 --tx-action commit --tx-endloop-action retire"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address transaction_commit_retire --count 1"));
        }

        [TestMethod]
        public void TestTransactRetireNetCore()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address transaction_retire --count 10 --msg-content string_message --tx-size 2 --tx-action commit"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address transaction_retire --count 10 --tx-size 5 --tx-action retire"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address transaction_retire --count 10"));
        }

        [TestMethod]
        public void TestP2PNetCore()
        {
            Task listener = Task.Run(() => {
                Assert.AreEqual(0, this.clientRunner.RunReceiver("--recv-listen true --recv-listen-port 8888 --count 10 --timeout 5"));
            });
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(0, this.clientRunner.RunSender("--broker localhost:8888 --count 10"));
            Task.WaitAll(listener);
        }

        [TestMethod]
        public void TestSendDurationNetCore()
        {
            Task receiver = Task.Run(() => {
                Assert.AreEqual(0, this.clientRunner.RunReceiver("--address duration_queue --count 5"));
            });
            Assert.AreEqual(0, this.clientRunner.RunSender("--address duration_queue --count 5 --msg-content string_message --duration 5"));
            Task.WaitAll(receiver);
        }

        [TestMethod]
        public void TestReceiveDurationNetCore()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address duration_queue --count 5 --msg-content string_message"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address duration_queue --count 5 --duration 5"));
        }

        [TestMethod]
        public void TestSendDurableMessageNetCore()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address durable_queue --count 5 --msg-durable True"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address durable_queue --count 5"));
        }

        [TestMethod]
        public void TestRejectMessagesNetCore()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address reject_queue --count 5"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address reject_queue --count 5 --action reject"));
            Assert.AreEqual(0, this.clientRunner.RunReceiver("--address reject_queue --count 5"));
        }

        [TestMethod]
        public void TestSendReceiveTimeoutNetCore()
        {
            Task timeoutReceiver = Task.Run(() => {
                Assert.AreEqual(0, this.clientRunner.RunReceiver("--address timeout_queue --count 5 --timeout 10"));
            });
            Assert.AreEqual(0, this.clientRunner.RunSender("--address timeout_queue --count 5 --timeout 5"));
            Task.WaitAll(timeoutReceiver);
        }
        [TestMethod]
        public void TestMsgContentType()
        {
            Assert.AreEqual(0, this.clientRunner.RunSender("--address content_queue --count 5 --content-type int --msg-content 5"));
        }

        [TestMethod]
        public void TestInfinityReceiving()
        {
            Task listener = Task.Run(() => {
                Assert.AreEqual(0, this.clientRunner.RunReceiver("--timeout -1 --address timeout_queue"));
            });
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(0, this.clientRunner.RunSender("--count 10 --duration 30 --address timeout_queue"));
            Task.WaitAll(listener);
        }
    }
}

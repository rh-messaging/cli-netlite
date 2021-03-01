using System;
using System.Threading.Tasks;
using Amqp;

namespace TcpKeepAliveSettings
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var address = new Address("amqp://admin:admin@localhost:5672");
            var connectionFactory = new ConnectionFactory();
            connectionFactory.TCP.KeepAlive = new Amqp.TcpKeepAliveSettings()
            {
                KeepAliveInterval = 1000,
                KeepAliveTime = 1000
            };
            await connectionFactory.CreateAsync(address);
            var connection = new Connection(address);
            var session = new Session(connection);

            var message = new Message("Hello AMQP!");
            var sender = new SenderLink(session, "sender-link", "q1");
            sender.Send(message);
            Console.WriteLine("Sent Hello AMQP!");

            sender.Close();
            session.Close();
            connection.Close();
        }
    }
}


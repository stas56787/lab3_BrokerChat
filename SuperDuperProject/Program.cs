using System;
using System.Threading;
using Apache.NMS;
using Apache.NMS.Util;

namespace SuperDuperProjectNumberTwo
{
    class Program
    {
        static Thread myThread = new Thread(new ThreadStart(ReadNextMessage));
        static void Main(string[] args)
        {
            myThread.Start(); // запускаем поток
            while (true)
            {
                string text = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(text)) return;
                SendNewMessage(text);
            }
        }

        private static void SendNewMessage(string text)
        {
            string topic = "TextQueue";


            string brokerUri = $"activemq:tcp://localhost:61616";  // Default port
            NMSConnectionFactory factory = new NMSConnectionFactory(brokerUri);

            using (IConnection connection = factory.CreateConnection())
            {
                connection.Start();

                using (ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
                using (IDestination dest = session.GetTopic(topic))
                using (IMessageProducer producer = session.CreateProducer(dest))
                {
                    producer.DeliveryMode = MsgDeliveryMode.NonPersistent;
                    producer.Send(session.CreateTextMessage(text));
                }
            }
        }

        static void ReadNextMessage()
        {
            while (true)
            {
                string topic = "TextQueue1";

                string brokerUri = $"activemq:tcp://localhost:61616";  // Default port

                NMSConnectionFactory factory = null;

                try
                {
                    factory = new NMSConnectionFactory(brokerUri);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Не удалось подключиться.");
                    Console.ResetColor();
                }

                using (IConnection connection = factory.CreateConnection())
                {
                    connection.Start();
                    using (ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
                    using (IDestination dest = session.GetTopic(topic))
                    using (IMessageConsumer consumer = session.CreateConsumer(dest))
                    {
                        IMessage msg = consumer.Receive();
                        if (msg is ITextMessage)
                        {
                            ITextMessage txtMsg = msg as ITextMessage;
                            string body = txtMsg.Text;

                            Console.WriteLine($"{txtMsg.Text}");

                        }
                        else
                        {
                            Console.WriteLine("Unexpected message type: " + msg.GetType().Name);
                        }
                    }
                }
            }
        }
    }
}
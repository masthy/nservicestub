using System.IO;
using System.Messaging;
using System.Threading;

namespace NServiceStub.IntegrationTests
{
    public static class MsmqHelpers
    {
         public static void Purge(string queueName)
         {
             using (var queue = CreateQueue(queueName))
             {
                 queue.Purge();
             }
         }

        private static MessageQueue CreateQueue(string queueName)
        {
            var queue = new MessageQueue(string.Format(@".\Private$\{0}", queueName));
            return queue;
        }

        public static void WaitForMessages(string queueName)
        {
            do
            {
                Thread.Sleep(100);
            } while (GetMessageCount(queueName) == 0);
            
        }

        public static int GetMessageCount(string queueName)
        {
            using (var queue = CreateQueue(queueName))
            {
                return queue.GetAllMessages().Length;
            }
        }

        public static void PutMessageOnQueue(string body, string queueName)
        {
            using (var queue = CreateQueue(queueName))
            {
                if (queue.Transactional)
                {
                    var transaction = new MessageQueueTransaction();
                    transaction.Begin();
                    queue.Send(body, transaction);
                    transaction.Commit();
                }
                else
                    queue.Send(body);
            }

        }

        public static object PickMessageBody(string queueName)
        {
            using (var queue = CreateQueue(queueName))
            {
                Message message = queue.Receive();
                using (TextReader reader = new StreamReader(message.BodyStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
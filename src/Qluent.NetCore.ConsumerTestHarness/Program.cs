using System;
using System.Threading;
using System.Threading.Tasks;
using Qluent.Consumers.Policies;
using Qluent.Queues.Policies.PoisonMessageBehavior;

namespace Qluent.NetCore.ConsumerTestHarness
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //Cancellation Token
            var cancellationTokenSource = new CancellationTokenSource();

            //Create Queue
            var consumerQueue = await Builder
                .CreateAQueueOf<Job>()
                .UsingStorageQueue("my-job-queue")
                .ThatConsidersMessagesPoisonAfter(1)
                .AndHandlesExceptionsOnPoisonMessages(By.SwallowingExceptions)
                .BuildAsync();

            //Create Consumer
            var consumer = Builder
                .CreateAMessageConsumerFor<Job>()
                .UsingQueue(consumerQueue)
                .ThatHandlesMessagesUsing(HandleMessage)
                .AndHandlesFailedMessagesUsing(HandleFailure)
                .AndHandlesExceptionsUsing(HandleException)
                .WithAQueuePolingPolicyOf(new SetIntervalQueuePolingPolicy(5))
                .Build();

            consumer.Start(cancellationTokenSource.Token);

            var producerQueue = await Builder
                .CreateAQueueOf<Job>()
                .UsingStorageQueue("my-int-queue")
                .BuildAsync();

            RunProducer(producerQueue, cancellationTokenSource.Token);

            Console.ReadLine();
            Console.WriteLine("Cancelling Async Processes");
            cancellationTokenSource.Cancel();

            Console.WriteLine("Press enter to end");
            Console.ReadLine();
        }

        public static async Task RunProducer(IAzureStorageQueue<Job> queue, CancellationToken cancellationToken)
        {
            var r = new Random();
            while (!cancellationToken.IsCancellationRequested)
            {
                var waitTime = r.Next(0, 5000);
                var payload = r.Next(0, 10);
                var job = new Job(payload);
                await Task.Delay(waitTime, cancellationToken);
                Console.WriteLine("Producer: Adding Job to Queue");
                await queue.PushAsync(job, cancellationToken);
            }
        }

        public static bool HandleMessage(IMessage<Job> m)
        {
            if (m.Value.Payload > 5)
            {
                Console.WriteLine($"Success {m.Value.Payload} (Id: {m.Value.Id})");
                return true;
            }

            if (m.Value.Payload <= 0) throw new ArgumentException("Payload can't be less than zero. Abort Processing!");

            return false;
        }
        public static bool HandleFailure(IMessage<Job> m)
        {
            Console.WriteLine($"Failed {m.Value.Payload} (Id: {m.Value.Id})");
            return true;
        }

        public static bool HandleException(IMessage<Job> m, Exception ex)
        {
            Console.WriteLine($"Exception {m.Value.Payload} (Id: {m.Value.Id}): {ex.Message}");
            return true;
        }


    }

    public class Job
    {
        public Job(int payload)
        {
            Payload = payload;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public int Payload { get; }
    }
}

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
                .BuildAsync(cancellationTokenSource.Token);

            //Create Consumer
            var consumer = Builder
                .CreateAMessageConsumerFor<Job>()
                .UsingQueue(consumerQueue)
                .ThatHandlesMessagesUsing(HandleMessage)
                .AndHandlesFailedMessagesUsing(HandleFailure)
                .AndHandlesExceptionsUsing(HandleException)
                .WithAQueuePolingPolicyOf(new SetIntervalQueuePolingPolicy(10000))
                .Build();

            consumer.Start(cancellationTokenSource.Token);

            var producerQueue = await Builder
                .CreateAQueueOf<Job>()
                .UsingStorageQueue("my-int-queue")
                .BuildAsync(cancellationTokenSource.Token);

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

        private static async Task<bool> HandleMessage(IMessage<Job> m, CancellationToken token)
        {
            if (m.Value.Payload > 5)
            {
                Console.WriteLine($"Success {m.Value.Payload} (Id: {m.Value.Id})");
                return await Task.FromResult(true);
            }

            if (m.Value.Payload <= 0) throw new ArgumentException("Payload can't be less than zero. Abort Processing!");

            return await Task.FromResult(false);
        }

        private static async Task<bool> HandleFailure(IMessage<Job> m, CancellationToken token)
        {
            Console.WriteLine($"Failed {m.Value.Payload} (Id: {m.Value.Id})");
            return await Task.FromResult(true);
        }

        private static async Task<bool> HandleException(IMessage<Job> m, Exception ex, CancellationToken token)
        {
            Console.WriteLine($"Exception {m.Value.Payload} (Id: {m.Value.Id}): {ex.Message}");
            return await Task.FromResult(true);
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

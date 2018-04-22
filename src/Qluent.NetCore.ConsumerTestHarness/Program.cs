using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;
using Qluent.Consumers.Policies;
using Qluent.Queues.Policies.PoisonMessageBehavior;

namespace Qluent.NetCore.ConsumerTestHarness
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var config = new LoggingConfiguration();

            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);
            consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
            var rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(rule1);
            LogManager.Configuration = config;

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
                .AndHandlesMessageExceptionsUsing(HandleException)
                .WithAQueuePolingPolicyOf(new SetIntervalQueuePolingPolicy(10000))
                .WithAnIdOf("my-custom-id")
                .AndHandlesExceptions(Consumers.Policies.ConsumerExceptionBehavior.By.Continuing)
                .Build();

            consumer.Start(cancellationTokenSource.Token);

            var producerQueue = await Builder
                .CreateAQueueOf<Job>()
                .UsingStorageQueue("my-job-queue")
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
                Console.WriteLine($"         HANDLER: Success Id: {m.Value.Id}");
                return await Task.FromResult(true);
            }

            if (m.Value.Payload <= 0) throw new ArgumentException("Payload can't be less than zero. Abort Processing!");

            return await Task.FromResult(false);
        }

        private static async Task<bool> HandleFailure(IMessage<Job> m, CancellationToken token)
        {
            Console.WriteLine($"         HANDLER FAILED: Id: {m.Value.Id}");
            return await Task.FromResult(true);
        }

        private static async Task<bool> HandleException(IMessage<Job> m, Exception ex, CancellationToken token)
        {
            Console.WriteLine($"         EXCEPTION Id: {m.Value.Id}: {ex.Message}");
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

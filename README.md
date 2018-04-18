# Qluent

## What is this?

Qluent is a library that provides a ***very simple*** Fluent Async API and wrapper 
class around the Microsoft Azure Storage libraries to allow you to interact 
with storage queues using strongly typed objects.

It lets you interact with a queue like this.

```csharp
var q = await Builder
    .CreateAQueueOf<Person>()
    .UsingStorageQueue("my-test-queue")
    .BuildAsync();
    
var person = await q.PopAsync();
```

## Why do I need this?

I find there's a lot of ceremony involved when working with an Azure Storage Queues. 
Create an account, create a client, create a queue reference, make sure it exists,
object serialization/deserialization etc...

```csharp
var storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true;");
var queueClient = storageAccount.CreateCloudQueueClient();
var queue = queueClient.GetQueueReference("myqueue");
queue.CreateIfNotExists();

var person = new Person("John");
var serializedPerson = JsonConvert.Serialize(person)
var message = new CloudQueueMessage(serializedPerson); 
queue.AddMessage(message);

var result = queue.GetMessage();
var deserializedPerson = JsonConvert.Deserialize<Person>(result.AsString);
queue.DeleteMessage(result);
```

This gets even more convoluted when you want to do something a little more complex 
with visibility timeouts, handling poison messages, etc. I wanted a way to simplify 
it.


I'm also not a fan of the architectural decision in the SDK to leave settings like
message visbility up to the developer to decide on at the call site. If you're 
going to create your queues and access them via a DI framework, I'd prefer to 
centralize/standardize these settings at queue creation.


## What is this not?

This API is designed to be simple. It provide you an easy way to 
create a reference to a queue, which you can use to push and pop messages.
It also provides some simple approaches for common queuing scenarios, such as 
handling serialization and dealing with poison messages. That's it.

Azure Storage Queues are meant to be used for simple situations. Basic first 
in first out message passing so that you can distribute load in your application.

If you find yourself needing to do something more complex, then perhaps you should 
be looking at a different technology stack (Azure Service Bus, Event Hubs, Event Grid, 
Kafka, NService Bus, Mulesoft etc...)


## How do I use it?

### Connecting

If you don't specify a storage account, the builder will generate a queue connected 
to development storage by default.

```csharp
var q = await Builder
    .CreateAQueueOf<Person>()
    .UsingStorageQueue("my-test-queue")
    .BuildAsync();
```

Alternatively, you can explicitly provide a connection string to a storage account

```csharp
var q = await Builder
    .CreateAQueueOf<Person>()
    .ConnectedToAccount("UseDevelopmentStorage=true")
    .UsingStorageQueue("my-test-queue")
    .BuildAsync();
```


### Async & Await

The library is built against the .NET Standard 2.0 to target both .NET Framework 
& .NET Core. All Operations are asynchronous and support a method overload to pass
a cancellation token.

```csharp
var person = await q.PopAsync();

CancellationToken ct = new CancellationToken(false);
var person = await q.PopAsync(ct);
```

During queue creation the library will perform an async operation to create the queue 
if it doesn't exist. However if you need to create your queues in a non async manner
e.g. in a DI Container/Bootstrapper you can use the non async `Build` method

```csharp
var q = Builder
    .CreateAQueueOf<Person>()
    .ConnectedToAccount("UseDevelopmentStorage=true")
    .UsingStorageQueue("my-test-queue")
    .Build();
```

### Basic Queue Operations

You can clear all messages from a queue by calling

```csharp
await q.PurgeAsync();
```

You can also check the approximate message count on a queue by calling 

```csharp
var count = await q.CountAsync()
```

### Adding Messages

Queue's are tied to the object type you specify at creation.  
By default will serialize your objects to a Json String using NewtonSoft.Json.

```csharp
var q = await Builder
    .CreateAQueueOf<Person>()
    .UsingStorageQueue("my-test-queue")
    .BuildAsync(); 
    
var person = new Person("John");
await q.PushAsync(person);
``` 

You can also push an entire IEnumerable of messages onto the queue

```csharp
List<Person> people = new List<Person>();
await q.PushAsync(people);
```

### Receiving a Message

To take a message off a queue you can simply Pop the message. 
This will Dequeue a message, attempt to deserialize it and if deserialization succeeds
remove it from the queue.

If deserialization fails, the default behavior is to throw an exception. 
This will result in the message's dequeue count increasing, and it being 
reappearing on the queue after it's visibility timeout.

See: [Handling Poison Messages] for more info.

```csharp
var q = await Builder
    .CreateAQueueOf<Person>()
    .UsingStorageQueue("my-test-queue")
    .BuildAsync(); 
    
var person = await q.PopAsync();
``` 

If you don't want to remove the object from the queue, you can peek at it instead

```csharp
var person = await q.PeekAsync();
``` 


### Receiving multiple Messages

You can also Peek or Pop multiple messages at a time by passing a message count to 
either the method

```csharp
IEnumerable<Person> peekedPeople = await q.PeekAsync(5);

IEnumerable<Person> poppedPeople = await q.PopAsync(5);
```

### Message Visibility

You can provide a number of settings to override the various message visbility 
and time to live settings.

You can set a delay time before it appears to consumers on the queue

```csharp
var q = await Builder
    .CreateAQueueOf<Person>()
    .UsingStorageQueue("my-test-queue")
    .ThatDelaysMessageVisibilityAfterEnqueuingFor(TimeSpan.FromMinutes(1))
    .BuildAsync();
``` 

You can specify the duration that a message remains invisible for after it's 
been dequeued, useful in combination with handling poison messages

```csharp
var q = await Builder
    .CreateAQueueOf<Person>()
    .UsingStorageQueue("my-test-queue")
    .ThatKeepsMessagesInvisibleAfterDequeuingFor(TimeSpan.FromMinutes(1))
    .BuildAsync();
``` 

You can specify the duration that message will remain alive on the queue if 
no consumers dequeue them.

```csharp
var q = await Builder
    .CreateAQueueOf<Person>()
    .UsingStorageQueue("my-test-queue")
    .ThatSetsAMessageTTLOf(TimeSpan.FromDays(1))
    .BuildAsync();
```

### Handling Poison Messages

When a message is removed from a Storage Queue, the wrapper will attempt 
to deserialize it into the `<T>` you specified.

It is possible that deserialization might fail for a number of reasons. e.g. 
An unexpected/corrupted message may have been added to the queue which you cannot parse.

You can control how many times the library will attempt to dequeue and deserialize 
for you before it considers the message poison. Once considered poisonly, you can optionally
choose to route it to another queue for analysis/later processing.

```csharp
var jobQueue = await Builder
    .CreateAQueueOf<Person>()
    .UsingStorageQueue("my-test-queue")
    .ThatConsidersMessagesPoisonAfter(3)
    .AndSendsPoisonMessagesTo("my-poison-queue")
    .BuildAsync();
```

You can specify what should happen when a poison message is detected. The default behavior 
is to throw an exception each time the message fails to deserialize. 
You can override that behavior by specifying that exceptions should be swallowed. 
In this case, the Pop/Peek method will return null (or will remove the null result 
from an `IEnumerable<T>`).


```csharp
var jobQueue = await Builder
    .CreateAQueueOf<Job>()
    .UsingStorageQueue("my-test-queue")
    .ThatConsidersMessagesPoisonAfter(3)
    .AndSendsPoisonMessagesTo("my-poison-queue")
    .AndHandlesExceptionsOnPoisonMessagesBy(PoisonMessageBehavior.SwallowingExceptions)
    .BuildAsync();
```


### Customised Deserialization

By default Qluent will serialize your entities to Json Strings using NewtonSoft.Json.
Serialization is performed using the default `JsonConvert` utility.

For scenarios, where your client does not control both ends of the queue, you may have 
to deal with messages that have been serialized differently. 

To support this, Qluent allows your to pass your own custom binary or string serializer.

To create a custom binary serializer, implement the interface `Qluent.Serialization.IBinaryMessageSerializer<T>`
This will serialize/deserialize your message to a `byte[]` and push/pop it to the queue as bytes.
```csharp
var q = Builder
    .CreateAQueueOf<Person>()
    .ConnectedToAccount("UseDevelopmentStorage=true")
    .UsingStorageQueue("my-test-queue")
    .WithACustomSerializer(new CustomBinarySerializer())
    .Build();
``` 

To create a custom binary serializer, implement the interface `Qluent.Serialization.IStringMessageSerializer<T>`
This will serialize/deserialize your message to a `string` and push/pop it to the queue as string content.

```csharp
var q = Builder
    .CreateAQueueOf<Person>()
    .ConnectedToAccount("UseDevelopmentStorage=true")
    .UsingStorageQueue("my-test-queue")
    .WithACustomSerializer(new CustomStringSerializer())
    .Build();
``` 

### Logging

The Qluent API utilizes NLog. Each internal class instantiates an instance of an an `NLog.Logger` using 
the `GetCurrentClassLogger()` method. 



## Todo List

- ~~Interface based Refactoring~~
- Document calls properlty
- ~~Support Cancellation Tokens so that they can be passed through.~~
- Support Pop Receipts so that the consumer can decide how to handle messages
- ~~Write up the docs around message visibility when the above is done~~
- .NET Core Tests
- Include NLog/ILogger calls so that you can hook in your logging framework
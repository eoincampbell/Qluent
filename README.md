# Qluent

## What is this?

Qluent is a library that provides a ***very simple*** Fluent API and wrapper 
class around the Microsoft Azure Storage libraries to allow you to interact 
with storage queues using strongly typed objects.

It lets you interact with a queue like this.

```csharp
var q = Builder
    .CreateAQueueOf<Person>()
    .UsingStorageQueue("my-test-queue")
    .Build();
    
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
var q = Builder
    .CreateAQueueOf<Person>()
    .UsingStorageQueue("my-test-queue")
    .Build();
```

Alternatively, you can explicitly provide a connection string to a storage account

```csharp
var q = Builder
    .CreateAQueueOf<Person>()
    .ConnectedToAccount("UseDevelopmentStorage=true")
    .UsingStorageQueue("my-test-queue")
    .Build();
```

### Queue Operations

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
var q = Builder
    .CreateAQueueOf<Person>()
    .UsingStorageQueue("my-test-queue")
    .Build(); 
    
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
var q = Builder
    .CreateAQueueOf<Person>()
    .UsingStorageQueue("my-test-queue")
    .Build(); 
    
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

### Handling Poison Messages

When a message is removed from a Storage Queue, the QueueWrapper will attempt to deserialize 
it into the `<T>` you specified.

It is possible that deserialization might fail for a number of reasons. e.g. 
An unexpected/corrupted message may have been added to the queue which you cannot parse.

You can specify what should happen when a poison message is detected. The default behavior 
is to throw an exception if the method fails to deserialize. You can override that behavior
by specifying that exceptions should be swallowed. In this case, the Pop/Peek method will 
return null (or will remove the null result from an `IEnumerable<T>`)


```csharp
var jobQueue = Builder
    .CreateAQueueOf<Job>()
    .UsingStorageQueue("my-test-queue")
    .AndSwallowExceptionsOnPoisonMessages();
```

By default, a message will be deleted after it has failed to be popped and returned 5 times.

You can alternatively change the threshold and specify that poison messages are sent to 
a poison queue for later processing analysis by  using the following option.

```csharp
var jobQueue = Builder
    .CreateAQueueOf<Job>()
    .UsingStorageQueue("my-test-queue")
    .ThatSendsPoisonMessagesTo("my-poison-queue", afterAttempts: 3);
```


### Customised Deserialization

By default Qluent will serialize your entities to Json Strings using NewtonSoft.Json.
Serialization is performed using the default `JsonConvert` utility.

For scenarios, where your client does not control both ends of the queue, you may have 
to deal with messages that have been serialized differently. 

To support this, Qluent allows your to pass your own custom binary or string serializer.

```csharp
var q = Builder
    .CreateAQueueOf<Person>()
    .ConnectedToAccount("UseDevelopmentStorage=true")
    .UsingStorageQueue("my-test-queue")
    .WithACustomBinarySerializer(new CustomBinarySerializer())
    .Build();
``` 

To create a custom binary serializer, implement the interface `Qluent.Serialization.IBinaryMessageSerializer<T>`

```csharp
var q = Builder
    .CreateAQueueOf<Person>()
    .ConnectedToAccount("UseDevelopmentStorage=true")
    .UsingStorageQueue("my-test-queue")
    .WithACustomStringSerializer(new CustomStringSerializer())
    .Build();
``` 

To create a custom binary serializer, implement the interface `Qluent.Serialization.IStringMessageSerializer<T>`


## Todo List

- Support Pop Receipts so that the consumer can decide how to handle messages
- Write up the docs around message visibility when the above is done
- .NET Core Tests
- Include NLog/ILogger calls so that you can hook in your logging framework
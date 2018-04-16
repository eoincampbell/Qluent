# Qluent

## What is this?

Qluent is a library that provides a ***very simple*** Fluent API and wrapper 
class around the Microsoft Azure Storage libraries to allow you to interact 
with storage queues using strongly typed objects.

## Why do I need this?

I find there's a lot of ceremony around interacting with a Azure Storage Queues. 
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

## So what does it do?

It lets you create a wrapper round a queue like this.

```csharp
var q = Builder
    .CreateAQueueOf<Person>()
    .UsingStorageQueue("my-test-queue")
    .Build();
    
var person = await q.PopAsync();
```

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

### Receiving Messages

To take a message off a queue you can simply Pop the message. This will do a Get, Deserialize & Delete of the message. 

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

## Other Features

### Delete Confirmation using PopReceipts

### Handling Poison Messages

### Customised Deserialization
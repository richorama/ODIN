 __WARNING: this is a crazy experiment__

# ODIN

__Open Data Interface for .Net__

An interface which allows the decoupling of application and storage system. The interface also encourages 'middleware' to be used between application code and the database.

# The Concept

Taking inspiration from [OWIN](http://owin.org/) (Open Web Interface for .Net) and what's going on with [LevelDB and Node.js](https://github.com/rvagg/node-levelup) I thought it would be interesting to define a .NET interface for accessing data in a NoSQL, key/value pair style.

This interface is supposed to be simple and provide a lowest-common-denominator for key/value stores. Databases will always offer more exotic features and optimisations for which this interface can't cater for.

However, using the interface has some interesting benefits:

1. You can switch out the underlying database easily. i.e. you can switch from in-memory, to local disk, to Windows Azure Storage.
1. You can introduce middleware in the data read/write pipeline, to allow features such as caching and retry policies.
1. You can build more sophisticated APIs over the top of the interface. A triple store and a JSON serializer are currently available, but more are planned.

An example of building up an ODIN data pipeline:

```cs
// create a store to hold records in memory (this could be Windows Azure, Redis or files)
IOdin odinStore = new OdinMemoryStore();

// create a middleware to write all data access to trace for debugging
IOdin tracer = new OdinTracer(odinStore);

// create a strongly typed api to access the store
var jsonStore = new OdinJsonSerializer<Foo>(tracer);

// you now consume the jsonStore in your application
```

## The Interface

The ODIN interface is designed to be an extremely basic key/value store. It's also async.

```cs
public interface IOdin
{
    Task Put(string key, string value);
    
    Task<string> Get(string key);

    Task Delete(string key);

    Task<IEnumerable<KeyValue>> Search(string start = null, string end = null);
}
```

## Storage providers

Storage providers are classes deriving from IOdin, which store data somewhere. They are designed to run at the bottom of the ODIN stack.

Storage providers currently available:

* In Memory
* File System
* Windows Azure Table Storage
* Redis

Storage providers planned:

* SQL
* STDIN/STDOUT?
* Application Settings?
* ...

## Middleware

A Middleware class derives from IOdin, but will take another IOdin deriving class (or several) on the constructor. The middleware therefore intercepts all calls made through the data stack.

Middleware currently available:

* FanOut (read/write multiple stores)
* Partition (split your key range into smaller partitions)
* Counter (counts the number of requests to each method)
* LoadBalancer (Send the request to a store using a round robin load balancer)
* Cache (uses an in-memory cache)
* Tracer (writes data access activity to trace)
* Versioner (Records a version history of all writes)
* Retry (add retry logic to the requests)

Middleware planned:

* Single Thread (enforce single threaded access policy)
* Read/write events
* Logger (logs all events to another store)

## Consumers

A consumer will take an IOdin object on it's constructor, but will expose a different API, providing a specialisation for a particular querying need.

Consumes currently available:

* Json serializer
* Triple store

Consumers planned:

* Free text index
* Geospatial index

## License

MIT

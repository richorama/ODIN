 __WARNING: this is a crazy experiment__

# ODIN

Open Data Interface for .Net

An interface which allows the decoupling of application and storage system. The interface also encourages 'middleware' to be used between application code and the database.

## The Interface

The ODIN interface is designed to be an extremely basic key-value store, and supports async.

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

Storage providers planned:

* Redis
* Windows Azure Blob Storage
* SQL
* STDIN/STDOUT
* ...

## Middleware

A Middleware class derives from IOdin, but will take another IOdin deriving class (or several) on the constructor. The middleware therefore intercepts all calls made through the data stack.

Middleware currently available:

* FanOut (read/write multiple stores)
* Partition (split your key range into smaller partitions)
* Counter (counts the number of requests to each method)
* LoadBalancer (Send the request to a store using a round robin load balancer)

Middleware planned:

* Retry (add retry logic to the requests)
* Cache (apply a read cache)
* Single Thread (enforce single threaded access policy)
* Load balancing (balance read writes across a number of stores)
* Read/write events
* Logger (logs all events to another store)

## Consumers

A consumer will take an `IOdin` object on it's constructor, but will expose a different API, providing a specialisation for a particular querying need.

Consumes currently available:

* Json serializer
* Triple store

Consumers planned:

* Free text index
* Geospatial index

## License

MIT

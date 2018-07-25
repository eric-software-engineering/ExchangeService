# Exchange Rate Service

In order for a Bookmaker to see their liabilities in a common currency, An exchange rate microservice is required. This exchange rate service will be responsible for collection and delivery of exchange rate values to internal clients.


#### Quickstart ####

Create a local database as shown in the connection string

Press F5 and go to the URL http://localhost:8080/api/GetExchangeRate/USD/AUD


#### Implementation: ####

- Provide a RESTful api to fetch an exchange rate for a specified _base currency_ and _target currency_
- Store the given exchange rate in a database, timestamped with audit trail
- The most recent exchange rate for the requested currency exchange is returned
- Converstion rates are stored to 5 decimal places;


#### Technology stack: ####

- C#7 with Visual Studio 2017
- Entity Framework 6.x+ / Code First Migrations
- Simple Injector for its simplicity and its speed compared to other DI frameworks https://github.com/danielpalme/IocPerformance
- Provide a RESTful endpoint as described below;
- Retrieve exchange rates from https://fixer.io
- Use of the Redis Azure cache for improved performance and solve the load-balancing/caching issues
- SpecFlow & Moq for the testing


#### RESTful principles: ####

- Stateless: each request is independent of all the others
- Cached: for improved performance
- Simple: a single interface to get the exchange rates
- Reliable: API calls, the cache and the database are three mechanisms which can fallback into each other for guaranteed uptime
- Scalable: can support a large number of currencies and requests


#### Data Source ####

The acquire conversion rates via the REST api provided by fixer.io

The application collects all non-matching pairs of _base_ and _target_ currency of the following currencies should be imported:

- AUD;
- SEK;
- USD; 
- GBP;
- EUR.

These settings can be edited in the web.config


#### REST Api: ####

##### Retrieve Exchange Rate #####

The api supports a GET HTTP verb with the following contract:



```json

{
    "baseCurrency": "GBP",
    "targetCurrency": "AUD"
}

```
where

- baseCurrency : string, required;
- targetCurrency : string, required;


with response:



```json

{
    "baseCurrency": "GBP",
    "targetCurrency": "AUD",
    "exchangeRate" : 0.7192,
    "timestamp" : "2018-03-21T12:13:48.00Z"
}

```

#### NFR's ####

- Expected load on this API is ~5 - 15 requests/ sec;
- Can be deployed to a load-balanced environment (~2-3 nodes).


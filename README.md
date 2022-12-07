## Motorway Payments Code Test 

### Prerequisites
Please ensure you have the [.Net 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-6.0.403-windows-x64-installer) on your machine and if not usign docker cd into where you clone the repo :) 

cd into working directory
```

dotnet test
 ```

Alternately, if you have docker desktop, run the following commands one after the other

cd into working directory
```
docker build -t motorwaypaymenttest/colind .
```

```
docker run  motorwaypaymenttest/colind
```

The tests are pretty faithful to the GWTs provided with a bit of tweaking to enable get the SUT into shape as necessary with various state etc.
The test names are PascalCased and stripped of whitespaces from the test document for ease of reference

There are 2 main folders: 
-   ``test``  - contains unit tests and setup 
-  ``src``   - contains the core domain logic

The core logic is split via  `OrderFraudCheck` (command) and `OrderFraudCheckQuery` (query) this `cqrs` style approach is accompanied with `cqs` commands and queries for the database persistence and retrieval.



The code test provides a naive implementation of a Ports and Adapters approach with a really sharp focus on domain and logic within it,  this works reasonably well with a number of test adapter implementations for the secondary ports.
In a larger code base as well as real adapters, integration tests for these adapters would need to be provided 

There is a distinct lack of mappers! Using Ports and Adapters it seems I can work within the confines of the ports and forget everything outside 
Lastly, I don't actually use Ports and Adapters in anger and this was a nice place to experiment with it. 
I actually favour Honeycomb testing using a mixture of in and out of process integration tests (Localstack ftw!)

I am very familiar with test isolation frameworks like Moq but I have chosen to roll my own test doubles (which are the test adapters)

I Tdd'd through the exercise and landed on a kind of chain of responsibility to manage failover from Fraud Away down to ByPass Fraud

The requirement to consider new Fraud providers was factored in when chaining the Fraud Checks and Open/Closed principles and Single Responsibility are adhered to, at least in the context of 
the FraudCheck Services 

I have made a few assumptions:
 - Transactional consistency is not a strong requirement and as such if requests failed to external dependencies 3rd parties and database, if retries were enacted without ` at least once ` messaging we may end up scoring customers more than once
 - The idempotency tests and retrieval tests for SimpleFraud and FraudAway assume the `derived` result was saved
 - Conversely the same tests for default fraud re determine the value as the inclusion of examples tables seem to indicate this is necessary (that is we dont save the actual result returned from the service it seems, i.e the one mapped to FraudCheckStatus - I didn't know if this was deliberate or not )

## Outstanding work
- Although I have all of the tests green, I committed some sins near the end of the test and the query and idempotency checking needs refactoring to utilise code re-use and some desirable characteristics like SRP
- I really wanted to introduce an Aggregate and Event Sourcing, but I could'nt work out how to model transactional consistency (as we have 2 3rd parties to contend with, and both could fail for multiple reasons as well our own database calls failing)
- The Sut's could really use a builder(I started and then backed out as I thought this was overkill as the GWTs kind of structure building of the SUT anyway)

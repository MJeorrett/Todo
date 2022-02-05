# TODO
My take on the classic, including oauth2 authentication, e2e teseting and more goodies.

## What might be of interest?
### Nullable reference types
Demostrates how to handle nullable reference types with EF core and in various other situations.

### E2e tests
Demostrates an e2e test set up with a clean structure that enables writing concise, robust, declarative tests.

### CQRS
Implements a CQRS approach to laying out an Asp.Net core application.

### Auditable Entities
An approach to ensuring CreatedAt, CreatedBy, LastUpdatedAt and LastUpdatedBy properties on entities are consistently updated when creating / saving in the db context.

## Stack
- Asp.Net Core 6
- EF Core 6 (SQL Server)
- Asp.Net Identity
- Openiddict
- Fluent validation
- Noda Time
- NUnit

## TODO
- :heavy_check_mark: HttpClient extensions in e2e tests.
- user management (super admin)
- Fix todo test suites not runing on their own.

## References
- Clean Architecture & CQRS - https://github.com/jasontaylordev/CleanArchitecture
- Setting up Openiddict - https://dev.to/robinvanderknaap/setting-up-an-authorization-server-with-openiddict-part-i-introduction-4jid

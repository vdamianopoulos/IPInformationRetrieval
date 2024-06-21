* C# Project with IP Information Management *

This project is a C# application designed to fetch IP information, utilizing caching and a database for storage and retrieval. 

Features: 

* IP Information Fetching: *

----- Leverages external providers for IP details. *

* Caching: 

----- Employs Redis for external data cache. *

----- Utilizes IMemoryCache for in-memory data cache. *

* Database: 

----- Dynamically creates an SQLite database on startup using raw SQL queries and seed data. *

----- Employs Entity Framework for interacting with the database. *

* Dependency Injection: 

----- Adheres to SOLID principles for maintainability and testability. *

* Design Patterns: 

-----  Utilizes Factory, Dependency Injection, and Facade patterns for efficient code organization. *

* Service Repository Pattern: 

----- Structures data access logic through separation of concerns. *

* Data Validation: 

----- Ensures data integrity using custom validators. *

* Mappers and Helpers: 

----- Provides transformation and utility functions for data manipulation. *

* Abstractions: 

----- Promotes code reusability and decoupling. *

* Asynchronous Data Fetching: 

----- Employs IAsyncEnumerable for efficient handling of large or streamed data. *

* Job Scheduling: 

----- Periodically updates data (e.g., hourly) using a job scheduler. *

* Selector Services: 

----- Offers separate services for data persistence and retrieval, implementing the Facade pattern for streamlined API access. *

* API Versioning: 

----- Manages API versioning for future compatibility. *

* Decorators: 

----- Leverages decorators (e.g., resilience pipeline for HttpClient in Program.cs) for enhanced service behavior. *

* Dockerization: 

----- Containerizes the application for ease of deployment and management. *

* Polly: 

----- Provides resiliency strategies for handling transient failures in external requests. *

* Refit: 

----- Facilitates communication with the external IP provider using a type-safe HTTP client. *

 
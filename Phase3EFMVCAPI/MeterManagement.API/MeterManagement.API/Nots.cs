
// onion architecture:->

/*
 
MyApp.sln
│
├── MyApp.Domain
│   ├── Entities
│   ├── Interfaces Repositories
│   ├── ValueObjects
│   └── Enums
│
├── MyApp.Application
│   ├── Interfaces Services
│   ├── Services
│   ├── DTOs
│   ├── Validators
│   └── Mappers
│
├── MyApp.Infrastructure
│   ├── Repositories
│   ├── Persistence
│   ├── Configurations
│   └── ExternalServices
│   └── AppDbContext
│   └── Migrations
│
└── MyApp.API
    ├── Controllers
    ├── Middlewares
    ├── Filters
    ├── DependencyInjection
    ├── Program.cs
    └── appsettings.json



Depandency flow:-

        API
         ↓
        Application
         ↓
        Infrastructure
         ↓
        Domain


*/


// kont h3ml genaric repo bas l2eet en mfeesh 8eer model wa7ed fmlosh lzma delw2ty 
// bardo kont hst5dem auto mapper bas 7beet a3ml el7aga implementation b2edy 


// pagenation:-
/*
 var meters = _context.Meters
    .OrderBy(m => m.Id)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToList();
 */
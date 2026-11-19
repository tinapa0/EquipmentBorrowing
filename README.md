# Campus Equipment Borrowing System

A desktop application for managing campus equipment loans, built with **Clean Architecture**, **Avalonia UI**, **EF Core**, and **PostgreSQL**.

## Table of Contents
1. [Solution Structure](#1-solution-structure)
2. [Dependency Direction](#2-dependency-direction)
3. [Setup, Installation & Configuration](#3-setup-installation--configuration)
4. [Use Case Mapping](#4-use-case-mapping)
5. [Architecture](#5-architecture)
6. [Persistence & Relational Architecture](#6-persistence--relational-architecture-lab-activity-4)
7. [Architectural & Design Decisions](#7-architectural--design-decisions-lab-activity-4)
8. [Application Flows](#8-application-flows)
9. [Reflection Answers](#9-reflection-answers)
10. [Laboratory Activity 4 Summary](#10-laboratory-activity-4-summary)

---

## 1. Solution Structure
* **`EquipmentBorrowing.Domain`**: Core business entities (`Student`, `Equipment`, `Borrowing`), value objects, domain rules, and repository interfaces. It has zero dependencies on external frameworks or database logic.
* **`EquipmentBorrowing.Application`**: Application use cases, workflows, and service interfaces. It orchestrates domain logic to fulfill system operations without knowing persistence details.
* **`EquipmentBorrowing.Infrastructure`**: Concrete data persistence code (EF Core `DbContext`, entity configurations, migrations, and repository implementations) and external library integration.
* **`EquipmentBorrowing.Desktop`**: The Avalonia presentation layer containing XAML Views, CommunityToolkit-powered ViewModels, application styles, and the composition root for dependency injection.
* **`EquipmentBorrowing.Tests`**: Unit and integration tests (xUnit) that verify business logic, repository interactions, and service behavior.

---

## 2. Dependency Direction
```text
EquipmentBorrowing.Desktop (Executable)
 └──> EquipmentBorrowing.Infrastructure
       └──> EquipmentBorrowing.Application
             └──> EquipmentBorrowing.Domain
```

---

## 3. Setup, Installation & Configuration

### Prerequisites
- .NET 8.0 SDK or higher
- PostgreSQL Server installed locally or accessible via connection string
- EF Core CLI tools (`dotnet tool install --global dotnet-ef`)
- pgAdmin or any PostgreSQL client (optional, for schema inspection)

### Secret-Free Configuration
Configure your database connection string in a local configuration file or environment variable **without committing credentials to version control**. Example `appsettings.json` format (use a placeholder password in the repository):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=equipment_borrowing_db;Username=postgres;Password=your_secure_password"
  }
}
```

To keep real credentials out of the repository, you can instead:
- Use .NET User Secrets: `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your connection string>" --project src/EquipmentBorrowing.Desktop`
- Or set an environment variable: `ConnectionStrings__DefaultConnection`

### Database Migration & Initialization
1. Open a terminal in the solution root directory.
2. Run EF Core migrations to build the relational schema on your PostgreSQL server:
   ```bash
   dotnet ef database update --project src/EquipmentBorrowing.Infrastructure --startup-project src/EquipmentBorrowing.Desktop
   ```
3. Run the desktop application:
   ```bash
   dotnet run --project src/EquipmentBorrowing.Desktop
   ```

---

## 4. Use Case Mapping
| Item | Description |
| :--- | :--- |
| **Use Case** | Borrow Equipment |
| **Primary Actor** | Student |
| **Preconditions** | Student and equipment records exist in the system. |
| **Main Action** | Student submits a request to borrow a specific piece of equipment. |
| **Expected Result** | Borrowing record is created with `Active` status, and equipment status updates to unavailable. |
| **Possible Failure** | Equipment is unavailable, student is barred, or student reached maximum active borrowings limit. |

## Lab Activity 4 Use Cases (UC-06 to UC-09)
| Use Case | Feature | Summary |
| :--- | :--- | :--- |
| **UC-06** | Multi-Item Atomic Checkout | Processes multiple equipment items in a single transaction via BorrowEquipmentService and CheckoutViewModel, rolling back if validation or persistence fails. |
| **UC-07** | Student Borrowing History | Displays borrowing records for a selected student (GetByStudentIdAsync) via StudentHistoryView and StudentHistoryViewModel. |
| **UC-08** | Equipment Borrowing History | Displays borrowing records for a selected piece of equipment (GetByEquipmentIdAsync) via EquipmentHistoryView and EquipmentHistoryViewModel. |
| **UC-09** | Search, Filter & Sort Borrowings | Loads all records (GetAllAsync) in BorrowingSearchViewModel with case-insensitive partial search, status filtering (All, Active, Returned), and ascending/descending sorting by borrowed date or expected return date. |

---

## 5. Architecture
```text
Avalonia View
      │
      │ Binding / Command
      ▼
  ViewModel
      │
      │ Application Operation
      ▼
Application Service
      │
      ├──────────► Domain
      │
      ▼
Repository Interface
      ▲
      │
Infrastructure Implementation
```

---

## 6. Persistence & Relational Architecture (Lab Activity 4)

### Database & EF Core Integration (PostgreSQL)
- **DbContext (`EquipmentBorrowingDbContext`)**: Located in `EquipmentBorrowing.Infrastructure/Persistence/`, this manages database sessions and maps core domain models (`Student`, `Equipment`, `Borrowing`) to PostgreSQL tables.
- **Entity Configurations**: Fluent API configuration classes (`StudentConfiguration.cs`, `EquipmentConfiguration.cs`, `BorrowingConfiguration.cs`) define explicit property constraints, max lengths, delete behaviors (`Restrict`), and foreign key relations safely outside domain entities.
- **Migrations**: Database schema creation and evolution are handled via version-controlled EF Core migrations (`Migrations/` folder) using the Npgsql provider.
- **Repositories**: Abstract repository interfaces are implemented via database-backed classes (`EfStudentRepository`, `EfEquipmentRepository`, `EfBorrowingRepository`) leveraging asynchronous LINQ operations (`ToListAsync`, `SaveChangesAsync`, etc.). IBorrowingRepository and its implementations also support GetByStudentIdAsync, GetByEquipmentIdAsync, and GetAllAsync with Include(b => b.Student) and Include(b => b.Equipment).

### Persistence Verification Workflow
1. **Borrow Operation**: When staff requests equipment through the Avalonia UI, a persistent record is written to the PostgreSQL database, and equipment availability updates atomically.
2. **Restart Durability**: Closing the application completely and reopening it preserves all active borrowings, student eligibility states, and equipment statuses.
3. **Return & Management Operations**: Returning equipment, registering students/equipment (UC-12, UC-15), and updating details (UC-13, UC-16) update the relational database state while maintaining a complete audit history across sessions.

---

## 7. Architectural & Design Decisions (Lab Activity 4)

1. **DbContext & Dependency Injection Lifetimes**
   - `EquipmentBorrowingDbContext` and repository services are registered with appropriate lifetimes to support asynchronous operations safely, without cross-thread contamination or connection leaks across Avalonia UI threads.

2. **Transaction Atomicity & Multi-Item Checkout (UC-06)**
   - Multi-item checkouts are wrapped in EF Core transaction boundaries. If any validation rule fails or an unexpected database exception occurs during multi-item issuance, the transaction rolls back completely to prevent partial records or inconsistent equipment availability states.

3. **Concurrency & Validation Strategy**
   - Domain invariants (such as the 3-item active borrowing limit, student eligibility checks under UC-14, and unique identifier validation) are enforced at the application service layer and backed by PostgreSQL database constraints to guarantee data integrity.

---

## 8. Application Flows

### Borrow Equipment Flow
1. The user selects a student, chooses an available piece of equipment, and specifies an expected return date within the **Equipment View**.
2. The user clicks the **[Borrow Equipment]** button, which triggers a `RelayCommand` on the `EquipmentViewModel`.
3. The ViewModel gathers presentation input and asynchronously invokes `BorrowEquipmentService.BorrowAsync(...)` via dependency injection.
4. The application service evaluates business rules (student eligibility, active borrowing limits, and equipment availability).
5. If valid, a new `Borrowing` record is created with an `Active` status through the repository, and the equipment availability updates to unavailable.
6. The service returns the result to the ViewModel, which updates status messages and refreshes the observable collections to reflect the updated state on the UI.

### Return Equipment Flow
1. The user navigates to the **Active Borrowings View** and selects an active borrowing record.
2. The user clicks the **[Return Equipment]** button, executing the corresponding command in the `BorrowingsViewModel`.
3. The ViewModel invokes `ReturnEquipmentService.ReturnAsync(...)`.
4. The application service locates the active borrowing, validates that it can be returned, updates its status, and marks the corresponding equipment as available again via the repositories.
5. The interface automatically refreshes both the Equipment and Active Borrowings displays to show the latest synchronized state.

### Student & Equipment Borrowing History Flow (UC-07 & UC-08)
1. The user navigates to **Student History** or **Equipment History** via `MainWindowViewModel.`
2. Selecting a student or equipment item loads matching borrowing records asynchronously `(GetByStudentIdAsync or GetByEquipmentIdAsync)` with `Student` and `Equipment` navigation data.
3. The view displays borrowing details `(Id, BorrowedDate, ExpectedReturnDate, and Status)` and updates the status message.

### Search, Filter, and Sort Borrowings Flow (UC-09)
1. Navigating to **Search & Filter** loads all borrowing records via `GetAllAsync` into `BorrowingSearchViewModel`.
2. Entering search text (matching student full name, student number, or equipment name case-insensitively), selecting a status filter `(All, Active, Returned)`, or changing sort criteria `(Borrow Date or Expected Return`, ascending or descending) immediately updates the displayed list.
3. Clicking **Reset** restores default search, filter, and sort values.

---

## 9. Reflection Answers

### Repository Abstraction & Layering
1. **Why should the application service depend on a repository interface instead of directly depending on a database implementation?**
   Depending on interfaces decouples core application logic from specific storage technologies. This enables changing the database provider (e.g., swapping in-memory storage for SQLite or PostgreSQL) without altering business rules or application service logic.

2. **Which parts of your current solution could remain unchanged if another database (e.g., SQLite) were added later?**
   The `Domain` and `Application` projects (domain models, application services, and repository interfaces) would remain untouched. Only the `Infrastructure` layer would be updated to add the new provider's context and implementations.

3. **Which project contains the Avalonia Views?**
   `EquipmentBorrowing.Desktop`, the separate UI presentation project.

4. **Should an Avalonia button directly execute database queries? Why or why not?**
   No. Direct execution of database queries inside UI controls violates the Separation of Concerns principle. The UI layer should only handle user interactions and pass commands to application services, keeping persistence logic encapsulated within the Infrastructure project.

5. **What part of your implementation represents the actual business operation requested by the actor?**
   `BorrowEquipmentService.BorrowAsync()` in the Application layer represents the business operation, as it validates domain rules and coordinates state updates through repository abstractions.

### Presentation Layer
1. **Why should the View not call a repository directly?**
   Calling a repository directly from the View violates separation of concerns by entangling UI components with data access and persistence logic, making the code difficult to maintain and test.

2. **Why should business rules not be implemented in the ViewModel?**
   ViewModels should solely manage presentation state and user interactions. Keeping business rules in the Domain and Application layers ensures consistency and allows the core logic to be reused across different clients or interfaces.

3. **What is the responsibility of the ViewModel?**
   The ViewModel handles presentation state, exposes observable properties and collections for data binding, commands user actions, and bridges the View with backend application services.

4. **Why can the existing Application layer work without knowing that Avalonia is being used?**
   The Application layer relies strictly on abstractions, interfaces, and domain objects, remaining completely decoupled from specific UI frameworks like Avalonia.

5. **What advantage is gained from registering dependencies in one composition point?**
   Centralizing dependency registration in a single composition root makes the application's dependency graph transparent, modular, and easy to maintain or reconfigure (e.g., swapping service lifetimes or implementations).

6. **If the persistence provider were replaced later, which parts of the current solution should remain largely unchanged?**
   The Views, ViewModels, Application services, and Domain models would remain unchanged; modifications would be isolated to the Infrastructure layer.

---

## 10. Laboratory Activity 4 Summary

### Overall Activity Summary
The Campus Equipment Borrowing System evolved from a temporary, volatile state into a robust desktop application backed by a persistent relational database. Using **Entity Framework (EF) Core** and **PostgreSQL** (via the Npgsql provider), the core domain entities (`Student`, `Equipment`, and `Borrowing`) were mapped to relational tables complete with primary keys, foreign key constraints, and normalized structures. By abstracting data access behind repository interfaces (`IStudentRepository`, `IEquipmentRepository`, and `IBorrowingRepository`), the application achieved data durability across restarts without coupling presentation layers or business logic to the storage engine. This foundation also supports multi-item atomic checkout (UC-06), student and equipment borrowing histories (UC-07 and UC-08), and searching, filtering, and sorting borrowings (UC-09) wired through `MainWindow` navigation and dependency injection.

### Core Architectural Reflections
1. **Preservation of Architectural Boundaries**
   Introducing a database did not require a rewrite because Clean Architecture was followed from the start. The Domain, Application, and Presentation layers remained agnostic of PostgreSQL and EF Core. All persistence details were isolated inside the Infrastructure layer, showing that well-designed abstractions protect core business logic from external technology shifts.

2. **Separation of Concerns (Views, ViewModels, and Database Contexts)**
   Direct database handling (raw SQL, connection strings, or `DbContext` instantiation) was kept out of Avalonia Views and ViewModels. Letting UI controls query a database directly would tightly couple presentation to persistence, making code hard to maintain and test. Instead, ViewModels trigger application services via commands, leaving data orchestration and persistence to the repository layer.

3. **The Role of EF Core Migrations and Repositories**
   EF Core migrations provided a version-controlled, reproducible mechanism for building and updating the database schema rather than relying on manual table creation. The database-backed repository implementations (`EfStudentRepository`, `EfEquipmentRepository`, `EfBorrowingRepository`) bridged asynchronous LINQ expressions and relational data storage, executing operations safely behind clean interfaces.

4. **Query Optimization and Tracking Decisions**
   LINQ expressions enabled expressive, type-safe querying of related data (such as active borrowings joined with student and equipment records). Using `.AsNoTracking()` for read-only, display-oriented operations improved performance by bypassing change tracking, while tracked queries were reserved for entities intended for modification (such as returning equipment or updating availability).

5. **Dependency Injection & Maintainability**
   Centralizing dependency registrations in a single composition root made it straightforward to replace temporary in-memory repository singletons with scoped EF Core contexts and repositories. If the application ever needs to move to another relational provider (such as SQL Server or SQLite), the change remains confined to the Infrastructure configuration layer.
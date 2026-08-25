**System:** Campus Equipment Borrowing System

## 1. Solution Structure

* **`EquipmentBorrowing.Domain`**: Contains core business entities (`Student`, `Equipment`, `Borrowing`), value objects, domain rules, and repository interfaces. It has zero dependencies on external frameworks or database logic.
* **`EquipmentBorrowing.Application`**: Contains application use cases, workflows, and service interfaces. It orchestrates domain logic to fulfill system operations without knowing persistence details.
* **`EquipmentBorrowing.Infrastructure`**: Contains concrete data persistence code (e.g., in-memory repositories or database contexts) and external library integration.
* **`EquipmentBorrowing.Tests`**: Contains unit and integration tests (xUnit) to programmatically verify business logic, repository interactions, and service behavior.


## 2. Dependency Direction
EquipmentBorrowing.App (Executable)
│
├──> EquipmentBorrowing.Infrastructure
│               │
│               ▼
└──> EquipmentBorrowing.Application
│
▼
EquipmentBorrowing.Domain


## 3. Use Case Mapping
| Item | Description |
| :--- | :--- |
| **Use Case** | Borrow Equipment |
| **Primary Actor** | Student |
| **Preconditions** | Student and equipment records exist in the system. |
| **Main Action** | Student submits a request to borrow a specific piece of equipment. |
| **Expected Result** | Borrowing record is created with `Active` status, and equipment status updates to unavailable. |
| **Possible Failure** | Equipment is unavailable, student is barred, or student reached maximum active borrowings limit. |

### 4. Reflection Answers
1. **Why should the application service depend on a repository interface instead of directly depending on a database implementation?**  
   Depending on interfaces decouples core application logic from specific storage technologies. This enables changing the database provider (e.g., swapping in-memory storage for SQLite or PostgreSQL) without needing to alter business rules or application service logic.

2. **Which parts of your current solution could remain unchanged if SQLite were added later?**  
   The `Domain` and `Application` projects (including domain models, application services, and repository interfaces) would remain completely untouched. Only the `Infrastructure` layer would be updated to add SQLite contexts and implementations.

3. **Which project would eventually contain Avalonia Views?**  
   A separate UI presentation project (such as `EquipmentBorrowing.UI` or `EquipmentBorrowing.Desktop`) would contain the Avalonia Views.

4. **Should an Avalonia button directly execute database queries? Why or why not?**  
   No. Direct execution of database queries inside UI controls violates the Separation of Concerns principle. The UI layer should only handle user interactions and pass commands to application services, keeping persistence logic encapsulated within the infrastructure project.

5. **What part of your implementation represents the actual business operation requested by the actor?**  
   The `BorrowEquipmentService.ExecuteAsync()` method inside the application layer represents the actual business operation, as it validates domain rules and coordinates state updates through repository abstractions.
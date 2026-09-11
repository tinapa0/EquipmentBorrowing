**System:** Campus Equipment Borrowing System

## 1. Solution Structure

* **`EquipmentBorrowing.Domain`**: Contains core business entities (`Student`, `Equipment`, `Borrowing`), value objects, domain rules, and repository interfaces. It has zero dependencies on external frameworks or database logic.
* **`EquipmentBorrowing.Application`**: Contains application use cases, workflows, and service interfaces. It orchestrates domain logic to fulfill system operations without knowing persistence details.
* **`EquipmentBorrowing.Infrastructure`**: Contains concrete data persistence code (e.g., in-memory repositories or database contexts) and external library integration.
* **`EquipmentBorrowing.Desktop`**: The Avalonia presentation layer containing XAML Views, CommunityToolkit-powered ViewModels, application styles, and the composition root for dependency injection.
* **`EquipmentBorrowing.Tests`**: Contains unit and integration tests (xUnit) to programmatically verify business logic, repository interactions, and service behavior.


## 2. Dependency Direction
```text
EquipmentBorrowing.App (Executable)
 └──> EquipmentBorrowing.Infrastructure
       └──> EquipmentBorrowing.Application
             └──> EquipmentBorrowing.Domain
```

## 3. Use Case Mapping
| Item | Description |
| :--- | :--- |
| **Use Case** | Borrow Equipment |
| **Primary Actor** | Student |
| **Preconditions** | Student and equipment records exist in the system. |
| **Main Action** | Student submits a request to borrow a specific piece of equipment. |
| **Expected Result** | Borrowing record is created with `Active` status, and equipment status updates to unavailable. |
| **Possible Failure** | Equipment is unavailable, student is barred, or student reached maximum active borrowings limit. |

## 4. Updated Architecture
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

### 5. Borrow Equipment Flow
1. The user selects a student, chooses an available piece of equipment, and specifies an expected return date within the **Equipment View**.

2. The user clicks the **[Borrow Equipment]** button, which triggers a `RelayCommand` on the `EquipmentViewModel`.

3. The ViewModel gathers presentation input and asynchronously invokes `BorrowEquipmentService.BorrowAsync(...)` via dependency injection.

4. The application service evaluates business rules (such as checking student eligibility, active borrowing limits, and equipment availability).

5. If valid, a new `Borrowing` record is created with an Active status through the repository, and the equipment availability updates to unavailable.

6. The service returns the result to the ViewModel, which updates status messages and refreshes the observable collections to reflect the updated state on the UI.

### 6. Return Equipment Flow
1. The user navigates to the **Active Borrowings View** and selects an active borrowing record.

2. The user clicks the **[Return Equipment]** button, executing the corresponding command in the `BorrowingsViewModel`.

3. The ViewModel invokes `ReturnEquipmentService.ReturnAsync`(...).

4. The application service locates the active borrowing, validates that it can be returned, updates its status, and marks the corresponding equipment as available again via the repositories.

5. The interface automatically refreshes both the Equipment and Active Borrowings displays to show the latest synchronized state.


### 7. Architectural Reflection
1. **Why should the View not call a repository directly?**

      Calling a repository directly from the View violates the separation of concerns by entangling UI components with data access and persistence logic, making the code difficult to maintain and test.

2. **Why should business rules not be implemented in the ViewModel?**

      ViewModels should solely manage presentation state and user interactions. Keeping business rules in the Domain and Application layers ensures consistency and allows the core logic to be reused across different clients or interfaces.

3. **What is the responsibility of the ViewModel?**

      The ViewModel handles presentation state, exposes observable properties and collections for data binding, commands user actions, and bridges the View with backend application services.

4. **Why can the existing Application layer work without knowing that Avalonia is being used?**

      The Application layer relies strictly on abstractions, interfaces, and domain objects, remaining completely decoupled from specific UI frameworks like Avalonia.

5. **What advantage is gained from registering dependencies in one composition point?**

      Centralizing dependency registration in a single composition root makes the application's dependency graph transparent, modular, and easy to maintain or reconfigure (e.g., swapping service lifetimes or implementations).

6. **If the in-memory repository were replaced by SQLite later, which parts of the current interface should remain largely unchanged?**

      The Views, ViewModels, Application services, and Domain models would remain completely unchanged; modifications would be isolated exclusively to the Infrastructure layer.


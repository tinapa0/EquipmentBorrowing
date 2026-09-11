# ITSD 81 – Desktop Application Development
## Laboratory Activity 2: Extending the Application with Avalonia UI and MVVM

---

## I. Overview

Laboratory Activity 1 established the internal structure of the Campus Equipment Borrowing System using domain models, application services, repository abstractions, dependency injection, and in-memory repository implementations.

In this activity, you will continue the same project by adding an Avalonia desktop user interface.

The goal is **not** to rewrite the system. Instead, you will place a presentation layer on top of the architecture created in Laboratory Activity 1.

The completed application should allow a user to interact with the existing borrowing functionality through a graphical interface while preserving the separation between:

```
View
  ↓
ViewModel
  ↓
Application Service
  ↓
Repository Interface
  ↓
Infrastructure Implementation
```

The existing Domain and Application layers must remain independent of Avalonia.

---

## II. Intended Learning Outcomes

At the end of the activity, students should be able to:

1. Extend an existing .NET solution with an Avalonia desktop project.
2. Construct desktop interfaces using XAML controls and layout panels.
3. Apply the Model-View-ViewModel (MVVM) pattern to separate presentation logic from application logic.
4. Use data binding, observable properties, collections, and commands to connect Views and ViewModels.
5. Connect ViewModels to existing application services through dependency injection.
6. Implement basic input validation and user-facing feedback.
7. Organize navigation between multiple application views or sections.
8. Apply shared styles and resources to produce a consistent desktop interface.
9. Preserve the architectural boundaries established in Laboratory Activity 1.

---

## III. Continuation of Laboratory Activity 1

This activity must use the **same** Campus Equipment Borrowing System developed in Laboratory Activity 1. Do not create a new independent solution.

Your existing solution should already contain components similar to:

```
EquipmentBorrowing/
│
├── EquipmentBorrowing.sln
│
├── src/
│   ├── EquipmentBorrowing.Domain/
│   ├── EquipmentBorrowing.Application/
│   └── EquipmentBorrowing.Infrastructure/
│
├── tests/
│   └── EquipmentBorrowing.Tests/
│
└── README.md
```

In Laboratory Activity 2, add a new Avalonia project:

```
EquipmentBorrowing/
│
├── EquipmentBorrowing.sln
│
├── src/
│   ├── EquipmentBorrowing.Domain/
│   ├── EquipmentBorrowing.Application/
│   ├── EquipmentBorrowing.Infrastructure/
│   └── EquipmentBorrowing.Desktop/
│       ├── Views/
│       ├── ViewModels/
│       ├── App.axaml
│       └── App.axaml.cs
│
├── tests/
│   └── EquipmentBorrowing.Tests/
│
└── README.md
```

> Existing components from Laboratory Activity 1 must be reused whenever applicable. **Do not duplicate** domain models, repository interfaces, or borrowing rules inside the Desktop project.

---

## IV. Part A – Review and Prepare the Existing Application

Before implementing the user interface, review your Laboratory Activity 1 submission. Verify that the existing solution contains:

- domain models for students, equipment, and borrowings;
- repository abstractions;
- in-memory repository implementations;
- an application service for borrowing equipment;
- asynchronous method signatures where appropriate;
- constructor-based dependency injection; and
- a working solution that builds successfully.

Correct architectural problems discovered during the review before continuing. You may refactor existing code when necessary, but the purpose of the refactoring must be documented through meaningful Git commits.

> **Important:** Do not move business rules into the new Avalonia project simply because they are needed by the interface. If a rule belongs to the borrowing process, it should remain in the Domain or Application layer.

---

## V. Part B – Add the Avalonia Desktop Project

Add an Avalonia desktop project to the existing solution.

The Desktop project will be responsible for:

- displaying information;
- collecting user input;
- handling presentation state;
- invoking application operations through ViewModels; and
- providing user-facing feedback.

The Desktop project may reference the projects necessary to compose the application. However:

> **The Domain and Application projects must not reference Avalonia.**

A possible dependency direction is:

```
EquipmentBorrowing.Desktop
│
├──────────────► EquipmentBorrowing.Application
│
└──────────────► EquipmentBorrowing.Infrastructure
                     │
                     ▼
              Repository Interfaces

EquipmentBorrowing.Application
│
▼
EquipmentBorrowing.Domain
```

The Domain project should remain independent of the desktop framework.

---

## VI. Part C – Create the Main Application Interface

Create a usable main window for the Campus Equipment Borrowing System.

The application should provide access to at least the following two major areas:

1. **Equipment** — Displays the available equipment and allows the user to initiate a borrowing transaction.
2. **Active Borrowings** — Displays equipment currently borrowed and allows an active borrowing to be returned.

A possible layout is:

```
┌────────────────────────────────────────────────────────┐
│              Campus Equipment Borrowing System           │
├────────────────┬───────────────────────────────────────┤
│                │                                        │
│   Equipment    │            Current View                │
│                │                                        │
│   Borrowings   │                                        │
│                │                                        │
└────────────────┴───────────────────────────────────────┘
```

Your exact interface design may differ.

The interface must remain organized and understandable at normal desktop window sizes.

---

## VII. Part D – Display Equipment Using Data Binding

The Equipment area must display equipment obtained from the existing application architecture.

At minimum, display:

- equipment identifier;
- equipment name;
- equipment type or description, if available; and
- availability status.

Use XAML data binding rather than manually constructing controls for every item.

The View should receive information through a ViewModel. Conceptually:

```
In-Memory Repository
        ↓
Application Layer
        ↓
Equipment ViewModel
        ↓
Observable Collection
        ↓
Avalonia View
```

Suitable controls may include:

- `ListBox`
- `ItemsControl`
- `ComboBox`
- `TextBlock`
- `Border`
- `Grid`
- `StackPanel`
- or other appropriate Avalonia controls.

The exact control selection is left to the pair.

---

## VIII. Part E – Implement the Borrow Equipment Interface

The graphical interface must allow the user to perform the Borrow Equipment use case implemented in Laboratory Activity 1.

The interface should allow the user to provide or select the information needed by the application, such as:

- student;
- equipment;
- expected return date.

A possible interaction is:

```
Select Student
      ↓
Select Equipment
      ↓
Select Expected Return Date
      ↓
[ Borrow Equipment ]
      ↓
ViewModel invokes BorrowEquipmentService
      ↓
Result displayed to user
```

### Required Rule

The ViewModel **must not** recreate the borrowing rules.

For example, avoid implementing logic similar to this inside the ViewModel:

```csharp
if (equipment.IsAvailable &&
    student.IsAllowedToBorrow &&
    activeBorrowings < maximumBorrowings)
{
    // create borrowing
}
```

Those rules belong to the existing application or domain implementation.

Instead, the ViewModel should collect the required input and invoke the appropriate application service. Conceptually:

```csharp
await _borrowEquipmentService.BorrowAsync(...);
```

The application service determines whether the borrowing operation is valid.

---

## IX. Part F – Implement Return Equipment

Extend the application with the Return Equipment use case.

If this use case was identified but not implemented during Laboratory Activity 1, create the necessary application service now.

A possible class name is: `ReturnEquipmentService`

The operation should:

1. locate the active borrowing;
2. determine whether it can be returned;
3. update the borrowing status;
4. make the equipment available again; and
5. communicate the result to the caller.

The business operation belongs in the Application and/or Domain layers. The graphical interface should only initiate the operation through its ViewModel.

A possible flow is:

```
Active Borrowings View
      ↓
Select Borrowing
      ↓
[ Return Equipment ]
      ↓
ViewModel
      ↓
ReturnEquipmentService
      ↓
Repositories
      ↓
Interface Refresh
```

After a successful return, the Equipment and Active Borrowings displays should reflect the updated state.

---

## X. Part G – Apply MVVM Using CommunityToolkit.Mvvm

Use CommunityToolkit.Mvvm to implement the ViewModels.

ViewModels should use appropriate features such as:

- `ObservableObject`;
- observable properties;
- relay commands;
- asynchronous relay commands; and
- observable collections.

For example:

```csharp
public partial class EquipmentViewModel : ObservableObject
{
    [ObservableProperty]
    private Equipment? selectedEquipment;

    [ObservableProperty]
    private string? statusMessage;

    [RelayCommand]
    private async Task BorrowAsync()
    {
        // Collect presentation input
        // and call the application service.
    }
}
```

The exact implementation is up to the pair.

### Views

Views should primarily contain:

- XAML layout;
- controls;
- bindings;
- styles; and
- presentation-related configuration.

### ViewModels

ViewModels should primarily contain:

- presentation state;
- commands;
- selected values;
- observable collections;
- user-facing messages; and
- calls to application services.

### Application Services

Application services should continue to contain:

- application operations;
- coordination of repositories;
- business workflows; and
- application-level validation.

---

## XI. Part H – Configure Dependency Injection

Laboratory Activity 1 introduced constructor-based dependency injection.

In this activity, configure the application's dependencies in a central composition point within the Desktop project.

The application should register the existing repository implementations and application services. Conceptually:

```csharp
services.AddSingleton<IEquipmentRepository, InMemoryEquipmentRepository>();
services.AddSingleton<IBorrowingRepository, InMemoryBorrowingRepository>();
services.AddTransient<BorrowEquipmentService>();
services.AddTransient<ReturnEquipmentService>();
services.AddTransient<EquipmentViewModel>();
services.AddTransient<BorrowingsViewModel>();
```

Your registrations may differ depending on your implementation.

> **Important:** The ViewModel should receive its dependencies through its constructor. Avoid creating application services directly inside ViewModels:
>
> ```csharp
> private readonly BorrowEquipmentService _service = new BorrowEquipmentService(...);
> ```

The application's composition root should be responsible for assembling the dependency graph.

Because the activity still uses in-memory repositories, repository instances should be configured so that application state is not unintentionally lost when moving between views.

---

## XII. Part I – Validation and User Feedback

The application must provide clear feedback when an operation succeeds or fails.

At minimum, handle situations such as:

- no student selected;
- no equipment selected;
- invalid expected return date;
- equipment unavailable;
- student not permitted to borrow;
- maximum borrowing limit reached;
- borrowing already returned; or
- requested record not found.

Differentiate between:

### Presentation Validation

Examples:

- required field is empty;
- no selection has been made;
- input cannot be interpreted.

These may be handled by the ViewModel.

### Business Validation

Examples:

- equipment is not available;
- student has reached the borrowing limit;
- borrowing has already been returned.

These should remain in the Domain or Application layer.

Do not silently ignore failed operations. The user must receive understandable feedback.

---

## XIII. Part J – Navigation and Application State

Provide a mechanism for moving between the application's primary sections.

At minimum, the user must be able to switch between **Equipment** and **Active Borrowings**.

Navigation may be implemented using:

- ViewModel-based view switching;
- a content area;
- reusable UserControls; or
- another appropriate Avalonia technique.

The application does not require a large navigation framework.

The goal is to demonstrate separation between views while maintaining application state. When a borrowing or return operation is completed, affected information should update appropriately.

---

## XIV. Part K – Styles and Interface Consistency

Apply basic shared styling to the desktop application.

Use Avalonia resources or styles to avoid unnecessarily repeating visual properties. At minimum, establish consistency for:

- buttons;
- text headings;
- spacing;
- form controls; and
- status or feedback messages.

The interface should demonstrate appropriate use of:

- margins;
- padding;
- alignment;
- grouping;
- readable labels; and
- consistent control sizing.

The application does not need to imitate a commercial product. Visual quality will be assessed primarily according to clarity, consistency, and usability.

---

## XV. Part L – Update the Project Documentation

Update the existing `README.md`. Do not replace the Laboratory Activity 1 explanation. **Extend** it.

Add the following sections:

### 1. Desktop Project

Explain the responsibility of `EquipmentBorrowing.Desktop` and how it interacts with the existing projects.

### 2. Updated Architecture

Update the architecture diagram to include: View, ViewModel, Application Service, Domain, Repository Interface, Infrastructure Implementation.

A possible representation is:

```
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

### 3. Borrow Equipment Flow

Explain what happens from the moment the user presses the Borrow Equipment button until the result is displayed.

### 4. Return Equipment Flow

Explain the same process for returning equipment.

### 5. Architectural Reflection

Answer briefly:

1. Why should the View not call a repository directly?
2. Why should business rules not be implemented in the ViewModel?
3. What is the responsibility of the ViewModel?
4. Why can the existing Application layer work without knowing that Avalonia is being used?
5. What advantage is gained from registering dependencies in one composition point?
6. If the in-memory repository were replaced by SQLite later, which parts of the current interface should remain largely unchanged?

---

## XVI. Expected Output

At the end of the activity, the solution should contain approximately:

```
EquipmentBorrowing/
│
├── README.md
├── EquipmentBorrowing.sln
│
├── src/
│   ├── EquipmentBorrowing.Domain/
│   │
│   ├── EquipmentBorrowing.Application/
│   │   ├── Interfaces/
│   │   └── Services/
│   │       ├── BorrowEquipmentService.cs
│   │       └── ReturnEquipmentService.cs
│   │
│   ├── EquipmentBorrowing.Infrastructure/
│   │   └── Repositories/
│   │
│   └── EquipmentBorrowing.Desktop/
│       ├── Views/
│       │   ├── EquipmentView.axaml
│       │   └── BorrowingsView.axaml
│       │
│       ├── ViewModels/
│       │   ├── MainWindowViewModel.cs
│       │   ├── EquipmentViewModel.cs
│       │   └── BorrowingsViewModel.cs
│       │
│       ├── App.axaml
│       ├── App.axaml.cs
│       └── MainWindow.axaml
│
└── tests/
    └── EquipmentBorrowing.Tests/
```

The exact internal structure may differ if the pair can justify the design.

---

## XVII. Submission Requirements

This laboratory activity shall be completed by the same pair from Laboratory Activity 1 and will span two weeks.

Laboratory Activity 2 must be developed as a continuation of the existing Laboratory Activity 1 Git repository.

Each pair must submit:

1. Link or compressed copy of the complete Git repository.
2. Updated .NET solution containing the Avalonia Desktop project.
3. Working Equipment interface.
4. Working Borrow Equipment operation through the graphical interface.
5. Working Active Borrowings interface.
6. Working Return Equipment operation.
7. Updated README.md containing the required architecture explanation.
8. Screenshot of the application running.
9. Screenshot of a successful borrowing transaction.
10. Screenshot of a successful return transaction.
11. Screenshot of at least one handled validation or business-rule failure.
12. Screenshot of a successful `dotnet build`.
13. Git history showing meaningful development throughout the two-week activity.

### Recommended Development History

- Add Avalonia desktop project
- Create main application layout
- Add equipment view and bindings
- Add equipment view model
- Connect borrowing service to UI
- Implement return equipment service
- Add active borrowings view
- Configure dependency injection
- Add navigation and shared styles
- Add validation and user feedback
- Update architecture documentation
- Refactor and finalize application

> Do not submit the entire activity as one final Git commit.

Both members of the pair are expected to understand and explain the complete application, including components originally created during Laboratory Activity 1.

---

## XVIII. Constraints

### Do:

- continue the Laboratory Activity 1 project;
- use Avalonia UI;
- use XAML for the application interface;
- apply MVVM;
- use CommunityToolkit.Mvvm;
- use data binding;
- use commands;
- reuse existing domain models;
- reuse existing repository abstractions;
- reuse the existing borrowing service;
- use dependency injection;
- continue using the in-memory repository;
- preserve meaningful Git history;
- ensure the entire solution builds successfully.

### Do not:

- create a separate unrelated system;
- rewrite Laboratory Activity 1 inside the Desktop project;
- place business rules in Views;
- place business rules in ViewModels;
- directly access repositories from Views;
- execute application logic inside button click handlers;
- add SQLite or another database;
- add Entity Framework Core;
- remove existing architectural layers simply to make the interface easier to implement.

---

## XIX. Assessment Rubric

| Criterion | Weight |
|---|---|
| Continuation and Reuse of Laboratory Activity 1 Architecture | 15% |
| Avalonia XAML Interface and Layout | 15% |
| MVVM Structure and Separation of Responsibilities | 20% |
| Data Binding, Observable State, and Commands | 15% |
| Borrow and Return Application Workflows | 15% |
| Dependency Injection and Application Integration | 10% |
| Validation, Feedback, and Basic UI Consistency | 5% |
| Documentation and Git Development History | 5% |
| **Total** | **100%** |

### General Standard

A visually attractive application alone does not constitute a complete submission.

The application must demonstrate that the interface has been added on top of the architecture developed in Laboratory Activity 1.

Students should be able to trace an operation such as:

```
User Action
      ↓
     View
      ↓
   Command
      ↓
  ViewModel
      ↓
Application Service
      ↓
  Repository
      ↓
    Result
      ↓
  ViewModel
      ↓
Updated Interface
```

and explain why each responsibility belongs to its respective layer.

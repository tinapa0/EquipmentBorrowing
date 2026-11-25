# Lab Activity 4: Evidence

Campus Equipment Borrowing System on PostgreSQL (Npgsql + EF Core).

**Partners:** Maria Kristina C. Sedeno / Ritch Vaughn V. Agne

Screenshots are stored in [`evidence_img/`](evidence_img/). Each heading below carries the acceptance ID (A01–A19) from the lab's Appendix C.

| Database | Used for |
|----------|----------|
| `equipment_empty_test` | Clean/disposable database for migrations and constraint tests (A17) |
| `equipment_borrowing_db` | Working database for the UI workflows and readbacks (A03–A16) |

---

## Stage 1: Migrations and database constraints (A17, BR-16)

Entity Framework Core migrations were applied to a clean PostgreSQL database (`equipment_empty_test`) using Npgsql. The Infrastructure configurations enforce primary keys, foreign keys, unique indexes on `StudentNumber` (BR-01) and `EquipmentIdentifier` (BR-05), and a filtered unique index on `Borrowings` that allows only one `Active` borrowing per equipment item (BR-10, BR-16).

### A17_01: Migration execution

`dotnet ef database update` applies migration `AddEquipmentAndActiveBorrowingConstraints` and finishes with `Done.`

![A17_01 Migration execution](evidence_img/A17_01_Migration_Success.png)

### A17_02: Applied migration history

`public."__EFMigrationsHistory"` lists all four migrations as applied.

![A17_02 Migration history](evidence_img/A17_02_Migration_History.png)

### A17_03: Indexes and constraints

A `pg_indexes` query shows the primary keys (`PK_Students`, `PK_Equipment`, `PK_Borrowings`), the unique indexes (`IX_Students_StudentNumber`, `IX_Equipment_EquipmentIdentifier`) and the partial unique index `IX_Borrowings_EquipmentId` with `WHERE ("Status" = 'Active')`.

![A17_03 Indexes and constraints](evidence_img/A17_03_Indexes_Constraints.png)

### A17_04 to A17_07: Forced rejections by PostgreSQL

Direct SQL `INSERT` statements run in pgAdmin, bypassing the application, confirm that the database rejects invalid data.

**A17_04: Invalid foreign key.** A borrowing with a non-existent `StudentId` (99999) is rejected by `FK_Borrowings_Students_StudentId` (SQL state 23503).

![A17_04 Invalid foreign key](evidence_img/A17_04_Invalid_FK_Rejection.png)

**A17_05: Duplicate student number.** Inserting `2026-0001` again is rejected by `IX_Students_StudentNumber` (SQL state 23505).

![A17_05 Duplicate student number](evidence_img/A17_05_Duplicate_Student_Rejection.png)

**A17_06: Duplicate equipment identifier.** Inserting `EQ-001` again is rejected by `IX_Equipment_EquipmentIdentifier` (SQL state 23505).

![A17_06 Duplicate equipment identifier](evidence_img/A17_06_Duplicate_Equipment_Rejection.png)

**A17_07: Second active borrowing for one item.** A second `Active` borrowing for `EquipmentId = 1` is rejected by `IX_Borrowings_EquipmentId` (SQL state 23505).

![A17_07 Duplicate active borrowing](evidence_img/A17_07_Duplicate_Active_Borrowing_Rejection.png)

---

## Stage 2: Student and equipment management (A12–A16)

Desktop UI workflows against `equipment_borrowing_db`. Validation blocks missing required fields and duplicate identifiers, and saved changes persist in PostgreSQL.

### Students (UC-12, UC-13)

**A12_01: Missing required field.** Submitting the form without a student number shows `Error: Student number is required.`

![A12_01 Missing student number](evidence_img/A12_01_Register_Student_Validation.png)

**A12_02: Valid registration.** Ana Marie Agnilla (2026-0004) appears in the Registered Students list with her eligibility status.

![A12_02 Student registered](evidence_img/A12_02_Register_Student_Success.png)

**A12_03: Duplicate student number.** Registering `2026-0004` again is blocked with `Error: A student with number '2026-0004' already exists.`

![A12_03 Duplicate student rejected](evidence_img/A12_03_Duplicate_Student_Rejection.png)

**A13_01: Update a selected student.** Saving the selected student through UC-13 shows `Updated 'Ana Marie Agnilla' (2026-0004). Existing borrowing history preserved.`

![A13_01 Student updated](evidence_img/A13_01_Update_Student.png)

### Equipment (UC-15, UC-16)

**A15_01: Register equipment.** `EQ-007` (Vacuum) is added to the inventory list as available.

![A15_01 Equipment registered](evidence_img/A15_01_Add_Equipment_Success.png)

**A15_02: Duplicate identifier.** Registering `EQ-001` again shows `Error: Equipment with identifier 'EQ-001' already exists.`

![A15_02 Duplicate equipment rejected](evidence_img/A15_02_Duplicate_Equipment_Rejection.png)

**A16_01: Update equipment.** Editing `EQ-002` changes its name to `DSLR Camera Samsung` while availability and history are preserved.

![A16_01 Equipment updated](evidence_img/A16_01_Update_Equipment_Success.png)

### PostgreSQL readback (A12, A15, A16)

`SELECT * FROM public."Equipment"` and `SELECT * FROM public."Students"` in `equipment_borrowing_db` confirm that the new and edited records (2026-0004, EQ-007 and the renamed EQ-002) are stored in PostgreSQL.

![Equipment table readback](evidence_img/A12_A15_Readback_Equipment_Table.png)

![Students table readback](evidence_img/A12_A15_Readback_Students_Table.png)

---

## Stage 3: Borrowing, return, multi-item checkout, histories and search

### Single-item borrow (UC-03, A03)

**A03_01: Active-limit rejection.** Borrowing one more item for a student who already has 3 active borrowings is blocked with `Borrow Failed: Checkout exceeds limit. Student has 3 active borrowings, trying to check out 1 more (Max allowed: 3).`

![A03_01 Borrow limit rejection](evidence_img/A03_01_Borrow_Limit_Rejection.png)

**A03_02: Successful borrow.** Borrowing an available item with an expected return date shows `Successfully borrowed Extension Wire!`

![A03_02 Successful borrow](evidence_img/A03_02_Borrow_Single_Success.png)

### Return (UC-05, A05)

**A05_01: Return equipment.** From Active Borrowings, returning an item shows `Equipment successfully returned!` and removes it from the active list.

![A05_01 Return success](evidence_img/A05_01_Return_Success.png)

### Multi-item checkout (UC-06, A06)

**A06_01: Checkout form.** The Multi-Item Equipment Checkout screen with three available items selected (EQ-003, EQ-004, EQ-007), student Ana Marie Agnilla and expected return date 02/12/2026.

![A06_01 Multi-item checkout form](evidence_img/A06_01_Multi_Item_Checkout_Form.png)

### Histories (UC-07, UC-08, A07, A08)

**A07_01: Student history.** Ana Marie Agnilla's records show reference IDs, borrowed and expected return dates, and Returned/Active statuses.

![A07_01 Student history](evidence_img/A07_01_Student_History.png)

**A08_01: Equipment history.** EQ-004 Extension Wire's records show reference IDs, dates and Returned/Active statuses.

![A08_01 Equipment history](evidence_img/A08_01_Equipment_History.png)

### Search, filter and sort (UC-09, A09)

**A09_01: Combined criteria.** Search text `agne`, status `Active` and sort by Borrow Date ascending return the matching active borrowings of Ritch Vaughn Agne.

![A09_01 Search, filter and sort](evidence_img/A09_01_Search_Filter_Sort.png)

-- 1. Basic Retrieval
SELECT * FROM Equipment;

-- 2. Filtering
SELECT * FROM Equipment WHERE IsAvailable = 1;

-- 3. Join
SELECT 
    s.FullName AS Student,
    e.Name AS Equipment,
    b.BorrowedAt AS Borrowed,
    b.ExpectedReturnAt AS Due
FROM Borrowings b
JOIN Students s ON b.StudentId = s.StudentId
JOIN Equipment e ON b.EquipmentId = e.EquipmentId
WHERE b.Status = 'Active';

-- 4. Aggregate
SELECT e.Type, COUNT(*) AS TotalCount
FROM Equipment e
GROUP BY e.Type;

-- 5. Update
UPDATE Equipment
SET IsAvailable = 0
WHERE EquipmentId = 1;
-- Requested compact demo: KG2-B and KG1-A, five children each, two active teachers.
-- Run with psql -v ON_ERROR_STOP=1 -f database/reduce-demo.sql after taking a backup.
-- Removes surplus students and their dependent records; audit entries are preserved.
BEGIN;
UPDATE "Staff" SET "FullName" = 'Bakheeta Alameri' WHERE "Username" = 'bakheeta';

DELETE FROM "EarlyExits" WHERE "StudentId" NOT BETWEEN 1 AND 10;
DELETE FROM "AttendanceRecords" WHERE "StudentId" NOT BETWEEN 1 AND 10;
DELETE FROM "StudentGuardians" WHERE "StudentId" NOT BETWEEN 1 AND 10;
DELETE FROM "ClassTransfers" WHERE "StudentId" NOT BETWEEN 1 AND 10;
DELETE FROM "Students" WHERE "StudentId" NOT BETWEEN 1 AND 10;

-- Keep historical attendance in its original class; record today's roster changes.
INSERT INTO "ClassTransfers" ("StudentId", "FromClassId", "ToClassId", "EffectiveDate", "Reason", "ByStaffId")
SELECT "StudentId", "ClassId", 2, (CURRENT_TIMESTAMP AT TIME ZONE 'Asia/Dubai')::date,
       'Demo data simplified to two classes', 1
FROM "Students" WHERE "StudentId" BETWEEN 6 AND 10 AND "ClassId" <> 2;
UPDATE "Students" SET "ClassId" = 2 WHERE "StudentId" BETWEEN 6 AND 10 AND "ClassId" <> 2;

DELETE FROM "Classes" WHERE "ClassId" NOT IN (2, 5);

-- Deactivation retains historical staff references and prevents further sign-in.
UPDATE "Sessions" SET "RevokedAt" = CURRENT_TIMESTAMP
WHERE "RevokedAt" IS NULL AND "StaffId" IN (
    SELECT "StaffId" FROM "Staff" WHERE "Role" = 'Teacher' AND "Username" NOT IN ('r.saleh', 'h.yousef')
);
INSERT INTO "AuditEntries" ("StaffId", "Username", "Area", "Action", "Details", "Result")
SELECT 1, 'bakheeta', 'Staff', 'Staff deactivated', "FullName", 'OK'
FROM "Staff" WHERE "Role" = 'Teacher' AND "Username" NOT IN ('r.saleh', 'h.yousef') AND "IsActive";
UPDATE "Staff" SET "IsActive" = FALSE
WHERE "Role" = 'Teacher' AND "Username" NOT IN ('r.saleh', 'h.yousef');

INSERT INTO "AuditEntries" ("StaffId", "Username", "Area", "Action", "Details", "Result")
VALUES (1, 'bakheeta', 'Staff', 'Staff edited', 'Administrator renamed to Bakheeta Alameri', 'OK'),
       (1, 'bakheeta', 'Students', 'Demo data simplified', 'Kept KG2-B and KG1-A with five students each; surplus students and classes removed.', 'OK');

DO $$
BEGIN
    IF (SELECT count(*) FROM "Classes") <> 2
       OR (SELECT count(*) FROM "Students") <> 10
       OR EXISTS (SELECT 1 FROM "Classes" c LEFT JOIN "Students" s ON s."ClassId" = c."ClassId"
                  GROUP BY c."ClassId" HAVING count(s."StudentId") <> 5)
       OR (SELECT count(*) FROM "Staff" WHERE "Role" = 'Teacher' AND "IsActive") <> 2 THEN
        RAISE EXCEPTION 'Compact demo counts are incorrect; rolling back.';
    END IF;
END $$;
COMMIT;

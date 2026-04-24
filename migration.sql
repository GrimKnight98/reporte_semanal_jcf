CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "Products" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Products" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Price" TEXT NOT NULL
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260227154905_InitialCreate', '8.0.0');

COMMIT;

BEGIN TRANSACTION;

CREATE TABLE "WeeklyActivities" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_WeeklyActivities" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "CreatedBy" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260227164557_AddWeelyActivity', '8.0.0');

COMMIT;

BEGIN TRANSACTION;

CREATE TABLE "Instructors" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Instructors" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Position" TEXT NOT NULL,
    "Department" TEXT NOT NULL,
    "Status" TEXT NOT NULL,
    "CreatedBy" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260227173426_AddInstructor', '8.0.0');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Instructors" ADD "PersonNumber" TEXT NOT NULL DEFAULT '';

ALTER TABLE "Instructors" ADD "SignPath" TEXT NOT NULL DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260227180906_AddPersonNumberAddSignPath', '8.0.0');

COMMIT;

BEGIN TRANSACTION;

CREATE TABLE "Reports" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Reports" PRIMARY KEY AUTOINCREMENT,
    "StartDate" TEXT NOT NULL,
    "EndDate" TEXT NOT NULL,
    "Activities" TEXT NOT NULL,
    "LearnedActivities" TEXT NOT NULL,
    "DoubtSolver" INTEGER NOT NULL,
    "TrainingSessions" TEXT NOT NULL,
    "Status" TEXT NOT NULL,
    "CreatedBy" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL
);

CREATE TABLE "ef_temp_Instructors" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Instructors" PRIMARY KEY AUTOINCREMENT,
    "CreatedAt" TEXT NOT NULL,
    "CreatedBy" INTEGER NOT NULL,
    "Department" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "PersonNumber" TEXT NOT NULL,
    "Position" TEXT NOT NULL,
    "SignPath" TEXT NULL,
    "Status" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL
);

INSERT INTO "ef_temp_Instructors" ("Id", "CreatedAt", "CreatedBy", "Department", "Name", "PersonNumber", "Position", "SignPath", "Status", "UpdatedAt")
SELECT "Id", "CreatedAt", "CreatedBy", "Department", "Name", "PersonNumber", "Position", "SignPath", "Status", "UpdatedAt"
FROM "Instructors";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Instructors";

ALTER TABLE "ef_temp_Instructors" RENAME TO "Instructors";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260227210425_AddReport', '8.0.0');

COMMIT;

BEGIN TRANSACTION;

CREATE TABLE "ReportActivities" (
    "ReportId" INTEGER NOT NULL,
    "WeeklyActivityId" INTEGER NOT NULL,
    CONSTRAINT "PK_ReportActivities" PRIMARY KEY ("ReportId", "WeeklyActivityId"),
    CONSTRAINT "FK_ReportActivities_Reports_ReportId" FOREIGN KEY ("ReportId") REFERENCES "Reports" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ReportActivities_WeeklyActivities_WeeklyActivityId" FOREIGN KEY ("WeeklyActivityId") REFERENCES "WeeklyActivities" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_ReportActivities_WeeklyActivityId" ON "ReportActivities" ("WeeklyActivityId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260302200209_AddReportActivityRelation', '8.0.0');

COMMIT;

BEGIN TRANSACTION;

CREATE TABLE "ef_temp_Reports" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Reports" PRIMARY KEY AUTOINCREMENT,
    "CreatedAt" TEXT NOT NULL,
    "CreatedBy" INTEGER NOT NULL,
    "DoubtSolver" INTEGER NOT NULL,
    "EndDate" TEXT NOT NULL,
    "LearnedActivities" TEXT NOT NULL,
    "StartDate" TEXT NOT NULL,
    "Status" TEXT NULL,
    "TrainingSessions" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL
);

INSERT INTO "ef_temp_Reports" ("Id", "CreatedAt", "CreatedBy", "DoubtSolver", "EndDate", "LearnedActivities", "StartDate", "Status", "TrainingSessions", "UpdatedAt")
SELECT "Id", "CreatedAt", "CreatedBy", "DoubtSolver", "EndDate", "LearnedActivities", "StartDate", "Status", "TrainingSessions", "UpdatedAt"
FROM "Reports";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Reports";

ALTER TABLE "ef_temp_Reports" RENAME TO "Reports";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260303132735_RemoveActivitiesColumnFromReports', '8.0.0');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Reports" RENAME COLUMN "DoubtSolver" TO "InstructorId";

CREATE INDEX "IX_Reports_InstructorId" ON "Reports" ("InstructorId");

CREATE TABLE "ef_temp_Reports" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Reports" PRIMARY KEY AUTOINCREMENT,
    "CreatedAt" TEXT NOT NULL,
    "CreatedBy" INTEGER NOT NULL,
    "EndDate" TEXT NOT NULL,
    "InstructorId" INTEGER NOT NULL,
    "LearnedActivities" TEXT NOT NULL,
    "StartDate" TEXT NOT NULL,
    "Status" TEXT NULL,
    "TrainingSessions" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    CONSTRAINT "FK_Reports_Instructors_InstructorId" FOREIGN KEY ("InstructorId") REFERENCES "Instructors" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_Reports" ("Id", "CreatedAt", "CreatedBy", "EndDate", "InstructorId", "LearnedActivities", "StartDate", "Status", "TrainingSessions", "UpdatedAt")
SELECT "Id", "CreatedAt", "CreatedBy", "EndDate", "InstructorId", "LearnedActivities", "StartDate", "Status", "TrainingSessions", "UpdatedAt"
FROM "Reports";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Reports";

ALTER TABLE "ef_temp_Reports" RENAME TO "Reports";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_Reports_InstructorId" ON "Reports" ("InstructorId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260304160101_UpdateReportInstructorRelation', '8.0.0');

COMMIT;

BEGIN TRANSACTION;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260304195813_AddIdentitySchema', '8.0.0');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Reports" ADD "CreatedById" TEXT NULL;

CREATE TABLE "AspNetRoles" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetRoles" PRIMARY KEY,
    "Name" TEXT NULL,
    "NormalizedName" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL
);

CREATE TABLE "AspNetUsers" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetUsers" PRIMARY KEY,
    "FullName" TEXT NULL,
    "FirstName" TEXT NULL,
    "LastName" TEXT NULL,
    "ProfilePicturePath" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "Department" TEXT NULL,
    "UserName" TEXT NULL,
    "NormalizedUserName" TEXT NULL,
    "Email" TEXT NULL,
    "NormalizedEmail" TEXT NULL,
    "EmailConfirmed" INTEGER NOT NULL,
    "PasswordHash" TEXT NULL,
    "SecurityStamp" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL,
    "PhoneNumber" TEXT NULL,
    "PhoneNumberConfirmed" INTEGER NOT NULL,
    "TwoFactorEnabled" INTEGER NOT NULL,
    "LockoutEnd" TEXT NULL,
    "LockoutEnabled" INTEGER NOT NULL,
    "AccessFailedCount" INTEGER NOT NULL
);

CREATE TABLE "AspNetRoleClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY AUTOINCREMENT,
    "RoleId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserLogins" (
    "LoginProvider" TEXT NOT NULL,
    "ProviderKey" TEXT NOT NULL,
    "ProviderDisplayName" TEXT NULL,
    "UserId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserRoles" (
    "UserId" TEXT NOT NULL,
    "RoleId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserTokens" (
    "UserId" TEXT NOT NULL,
    "LoginProvider" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT NULL,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Reports_CreatedById" ON "Reports" ("CreatedById");

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");

CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");

CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");

CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");

CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");

CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");

CREATE TABLE "ef_temp_Reports" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Reports" PRIMARY KEY AUTOINCREMENT,
    "CreatedAt" TEXT NOT NULL,
    "CreatedById" TEXT NULL,
    "EndDate" TEXT NOT NULL,
    "InstructorId" INTEGER NOT NULL,
    "LearnedActivities" TEXT NOT NULL,
    "StartDate" TEXT NOT NULL,
    "Status" TEXT NULL,
    "TrainingSessions" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    CONSTRAINT "FK_Reports_AspNetUsers_CreatedById" FOREIGN KEY ("CreatedById") REFERENCES "AspNetUsers" ("Id"),
    CONSTRAINT "FK_Reports_Instructors_InstructorId" FOREIGN KEY ("InstructorId") REFERENCES "Instructors" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_Reports" ("Id", "CreatedAt", "CreatedById", "EndDate", "InstructorId", "LearnedActivities", "StartDate", "Status", "TrainingSessions", "UpdatedAt")
SELECT "Id", "CreatedAt", "CreatedById", "EndDate", "InstructorId", "LearnedActivities", "StartDate", "Status", "TrainingSessions", "UpdatedAt"
FROM "Reports";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Reports";

ALTER TABLE "ef_temp_Reports" RENAME TO "Reports";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_Reports_CreatedById" ON "Reports" ("CreatedById");

CREATE INDEX "IX_Reports_InstructorId" ON "Reports" ("InstructorId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260304201353_AddCreatedById', '8.0.0');

COMMIT;


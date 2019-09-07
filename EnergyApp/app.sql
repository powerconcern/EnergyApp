PRAGMA foreign_keys=OFF;
BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);
--INSERT INTO __EFMigrationsHistory VALUES('00000000000000_CreateIdentitySchema','2.2.4-servicing-10062');
--INSERT INTO __EFMigrationsHistory VALUES('20190721213752_InitialCreate','2.2.4-servicing-10062');
--INSERT INTO __EFMigrationsHistory VALUES('20190722151026_Meters','2.2.4-servicing-10062');
CREATE TABLE IF NOT EXISTS "AspNetRoles" (

    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetRoles" PRIMARY KEY,

    "Name" TEXT NULL,

    "NormalizedName" TEXT NULL,

    "ConcurrencyStamp" TEXT NULL

);
CREATE TABLE IF NOT EXISTS "AspNetUsers" (

    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetUsers" PRIMARY KEY,

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
INSERT INTO AspNetUsers VALUES('7688c7a6-1a6d-4ad7-8fb5-e551d0c3ce48','tommy.ekh@gmail.com','TOMMY.EKH@GMAIL.COM','tommy.ekh@gmail.com','TOMMY.EKH@GMAIL.COM',0,'AQAAAAEAACcQAAAAEBMH6Fa6giqte/1T+kCeEjtbP3u55HBDhXOC5I7Tv/J7DGxf58oV9siQoU9E3+MILg==','WM7L57427S2G75NMIYZ6BMXRE3PAKZCG','4c64c53c-ff76-457b-864c-41227e23e700',NULL,0,0,NULL,1,0);
CREATE TABLE IF NOT EXISTS "AspNetRoleClaims" (

    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY AUTOINCREMENT,

    "RoleId" TEXT NOT NULL,

    "ClaimType" TEXT NULL,

    "ClaimValue" TEXT NULL,

    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE

);
CREATE TABLE IF NOT EXISTS "AspNetUserClaims" (

    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY AUTOINCREMENT,

    "UserId" TEXT NOT NULL,

    "ClaimType" TEXT NULL,

    "ClaimValue" TEXT NULL,

    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE

);
CREATE TABLE IF NOT EXISTS "AspNetUserLogins" (

    "LoginProvider" TEXT NOT NULL,

    "ProviderKey" TEXT NOT NULL,

    "ProviderDisplayName" TEXT NULL,

    "UserId" TEXT NOT NULL,

    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),

    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE

);
CREATE TABLE IF NOT EXISTS "AspNetUserRoles" (

    "UserId" TEXT NOT NULL,

    "RoleId" TEXT NOT NULL,

    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),

    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,

    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE

);
CREATE TABLE IF NOT EXISTS "AspNetUserTokens" (

    "UserId" TEXT NOT NULL,

    "LoginProvider" TEXT NOT NULL,

    "Name" TEXT NOT NULL,

    "Value" TEXT NULL,

    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),

    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE

);
CREATE TABLE IF NOT EXISTS "Configurations" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_Configurations" PRIMARY KEY AUTOINCREMENT,
    
    "Key" TEXT NOT NULL,

    "Value" INTEGER NOT NULL

);
INSERT INTO Configurations VALUES(1,'BrokerURL','192.168.222.20');
CREATE TABLE IF NOT EXISTS "Meters" (

    "ID" INTEGER NOT NULL CONSTRAINT "PK_Meters" PRIMARY KEY AUTOINCREMENT,

    "Name" TEXT NULL,

    "MaxCurrent" REAL NOT NULL,

    "Type" INTEGER NULL,

    "ChargerID" INTEGER NOT NULL

);
INSERT INTO Meters VALUES(1,'FredriksMätare',16.0,0,1);
DELETE FROM sqlite_sequence;
INSERT INTO sqlite_sequence VALUES('Meters',1);
--CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");
--CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");
--CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");
--CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");
--CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");
--CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");
--CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");
COMMIT;

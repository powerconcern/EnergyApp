BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "Customers" (
	"ID"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	"Name"	TEXT,
	"CustomerNumber"	TEXT,
	"MeterID"	INTEGER NOT NULL,
	"ChargerID"	INTEGER NOT NULL,
	"Type"	INTEGER NOT NULL
);
CREATE TABLE IF NOT EXISTS "Outlets" (
	"ID"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	"Name"	TEXT,
	"MaxCurrent"	REAL NOT NULL,
	"Type"	INTEGER,
	"ChargerID"	INTEGER,
	CONSTRAINT "FK_Outlets_Chargers_ChargerID" FOREIGN KEY("ChargerID") REFERENCES "Chargers"("ID") ON DELETE RESTRICT
);
CREATE TABLE IF NOT EXISTS "Chargers" (
	"ID"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	"OutletID"	INTEGER NOT NULL,
	"Name"	TEXT,
	"MaxCurrent"	REAL NOT NULL,
	"CustomerID"	INTEGER
);
CREATE TABLE IF NOT EXISTS "Meters" (
	"ID"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	"Name"	TEXT,
	"MaxCurrent"	REAL NOT NULL,
	"Type"	INTEGER,
	"ChargerID"	INTEGER NOT NULL,
	"CustomerID"	INTEGER
);
CREATE TABLE IF NOT EXISTS "Configurations" (
	"ID"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	"Key"	TEXT NOT NULL,
	"Value"	TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS "AspNetUserTokens" (
	"UserId"	TEXT NOT NULL,
	"LoginProvider"	TEXT NOT NULL,
	"Name"	TEXT NOT NULL,
	"Value"	TEXT,
	CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY("UserId","LoginProvider","Name"),
	CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY("UserId") REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS "AspNetUserRoles" (
	"UserId"	TEXT NOT NULL,
	"RoleId"	TEXT NOT NULL,
	CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY("UserId","RoleId"),
	CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY("RoleId") REFERENCES "AspNetRoles"("Id") ON DELETE CASCADE,
	CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY("UserId") REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS "AspNetUserLogins" (
	"LoginProvider"	TEXT NOT NULL,
	"ProviderKey"	TEXT NOT NULL,
	"ProviderDisplayName"	TEXT,
	"UserId"	TEXT NOT NULL,
	CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY("LoginProvider","ProviderKey"),
	CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY("UserId") REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS "AspNetUserClaims" (
	"Id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	"UserId"	TEXT NOT NULL,
	"ClaimType"	TEXT,
	"ClaimValue"	TEXT,
	CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY("UserId") REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS "AspNetRoleClaims" (
	"Id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	"RoleId"	TEXT NOT NULL,
	"ClaimType"	TEXT,
	"ClaimValue"	TEXT,
	CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY("RoleId") REFERENCES "AspNetRoles"("Id") ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS "AspNetUsers" (
	"Id"	TEXT NOT NULL,
	"UserName"	TEXT,
	"NormalizedUserName"	TEXT,
	"Email"	TEXT,
	"NormalizedEmail"	TEXT,
	"EmailConfirmed"	INTEGER NOT NULL,
	"PasswordHash"	TEXT,
	"SecurityStamp"	TEXT,
	"ConcurrencyStamp"	TEXT,
	"PhoneNumber"	TEXT,
	"PhoneNumberConfirmed"	INTEGER NOT NULL,
	"TwoFactorEnabled"	INTEGER NOT NULL,
	"LockoutEnd"	TEXT,
	"LockoutEnabled"	INTEGER NOT NULL,
	"AccessFailedCount"	INTEGER NOT NULL,
	CONSTRAINT "PK_AspNetUsers" PRIMARY KEY("Id")
);
CREATE TABLE IF NOT EXISTS "AspNetRoles" (
	"Id"	TEXT NOT NULL,
	"Name"	TEXT,
	"NormalizedName"	TEXT,
	"ConcurrencyStamp"	TEXT,
	CONSTRAINT "PK_AspNetRoles" PRIMARY KEY("Id")
);
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
	"MigrationId"	TEXT NOT NULL,
	"ProductVersion"	TEXT NOT NULL,
	CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY("MigrationId")
);
INSERT INTO "Customers" ("ID","Name","CustomerNumber","MeterID","ChargerID","Type") VALUES (1,'Johan Sigtuna','74621ca4-a029-4c78-9387-17db8f0ab2bd',1,1,1),
 (2,'Tommys Test','7688c7a6-1a6d-4ad7-8fb5-e551d0c3ce48',1,1,1);
INSERT INTO "Outlets" ("ID","Name","MaxCurrent","Type","ChargerID") VALUES (1,'TEVCharger',16.0,0,NULL);
INSERT INTO "Chargers" ("ID","OutletID","Name","MaxCurrent","CustomerID") VALUES (1,1,'EVCS',16.0,1),
 (2,1,'TestEVCS',16.0,2);
INSERT INTO "Meters" ("ID","Name","MaxCurrent","Type","ChargerID","CustomerID") VALUES (1,'CMS',20.0,0,1,1),
 (2,'TestCMS',20.0,0,2,2);
INSERT INTO "Configurations" ("ID","Key","Value") VALUES (1,'BrokerURL','mqtt.symlink.se'),
 (2,'BrokerUser','johsim:johsim'),
 (3,'BrokerPasswd','UlCoPGgk');
INSERT INTO "AspNetUserRoles" ("UserId","RoleId") VALUES ('74621ca4-a029-4c78-9387-17db8f0ab2bd','4db1b8f8-37b8-4243-9d97-99c376b10fde'),
 ('b7459bb9-0c0a-43dd-aba3-a41c74859726','4db1b8f8-37b8-4243-9d97-99c376b10fde'),
 ('7688c7a6-1a6d-4ad7-8fb5-e551d0c3ce48','4db1b8f8-37b8-4243-9d97-99c376b10fde');
INSERT INTO "AspNetUsers" ("Id","UserName","NormalizedUserName","Email","NormalizedEmail","EmailConfirmed","PasswordHash","SecurityStamp","ConcurrencyStamp","PhoneNumber","PhoneNumberConfirmed","TwoFactorEnabled","LockoutEnd","LockoutEnabled","AccessFailedCount") VALUES ('7688c7a6-1a6d-4ad7-8fb5-e551d0c3ce48','tommy.ekh@gmail.com','TOMMY.EKH@GMAIL.COM','tommy.ekh@gmail.com','TOMMY.EKH@GMAIL.COM',0,'AQAAAAEAACcQAAAAEBMH6Fa6giqte/1T+kCeEjtbP3u55HBDhXOC5I7Tv/J7DGxf58oV9siQoU9E3+MILg==','WM7L57427S2G75NMIYZ6BMXRE3PAKZCG','56c0837b-6ecc-4c30-a139-a5e0fcad8184',NULL,0,0,NULL,1,0),
 ('74621ca4-a029-4c78-9387-17db8f0ab2bd','fredrik@powerconcern.se','FREDRIK@POWERCONCERN.SE','fredrik@powerconcern.se','FREDRIK@POWERCONCERN.SE',0,'AQAAAAEAACcQAAAAENhNQH7dNv1v5FklrbLBpUjG32x2o3gBs7OJxoCRUacV/GwLF2XSFA2+EBEYPKByew==','N6GSBLEEMPKJ4CPUHTJECE3SA6KP76MM','1b37455b-a454-4e22-ad16-a57825377af9',NULL,0,0,NULL,1,0),
 ('b7459bb9-0c0a-43dd-aba3-a41c74859726','tommy@powerconcern.se','TOMMY@POWERCONCERN.SE','tommy@powerconcern.se','TOMMY@POWERCONCERN.SE',0,'AQAAAAEAACcQAAAAEFYlelnlBPNqLI751mZgRDUJ4HvOqcTs1R8IF56uWaiGDouETf5Q9bNpVE/NbqIcaw==','DCXOZWB6TSK5FX6THHMWNWYAWZTL5SKV','038263e9-19a9-4506-8fbf-0b2012056ec2',NULL,0,0,NULL,1,0);
INSERT INTO "AspNetRoles" ("Id","Name","NormalizedName","ConcurrencyStamp") VALUES ('4db1b8f8-37b8-4243-9d97-99c376b10fde','Admin','ADMIN','94e86504-bb4e-4626-95de-2f5df6f7e174'),
 ('3364159d-a82d-4bac-bf05-063322c7f79b','Manager','MANAGER','1192a0e9-151a-4a81-986a-4be774fc504a'),
 ('c8e8c036-cedd-4682-8334-43596078ebc0','Member','MEMBER','831a43da-138d-48e7-a7f1-b0408784b2d0'),
 ('6c34ddef-1d45-47e6-bc95-8b9689c93151','Installer','INSTALLER','c36def2a-03f4-458a-ae13-0abe0574ddd0'),
 ('35419737-7c13-44ee-938d-f347f5739b67','Customer','CUSTOMER','472d7e7f-c4de-45f0-b93f-802b95382cd4');
INSERT INTO "__EFMigrationsHistory" ("MigrationId","ProductVersion") VALUES ('00000000000000_CreateIdentitySchema','2.2.4-servicing-10062'),
 ('20190721213752_InitialCreate','2.2.4-servicing-10062'),
 ('20190722151026_Meters','2.2.4-servicing-10062'),
 ('20190724212142_ChargerOutlet','2.2.4-servicing-10062'),
 ('20190808091717_Customer','2.2.4-servicing-10062');
CREATE INDEX IF NOT EXISTS "IX_Chargers_CustomerID" ON "Chargers" (
	"CustomerID"
);
CREATE INDEX IF NOT EXISTS "IX_Meters_CustomerID" ON "Meters" (
	"CustomerID"
);
CREATE INDEX IF NOT EXISTS "IX_Outlets_ChargerID" ON "Outlets" (
	"ChargerID"
);
CREATE UNIQUE INDEX IF NOT EXISTS "UserNameIndex" ON "AspNetUsers" (
	"NormalizedUserName"
);
CREATE INDEX IF NOT EXISTS "EmailIndex" ON "AspNetUsers" (
	"NormalizedEmail"
);
CREATE INDEX IF NOT EXISTS "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" (
	"RoleId"
);
CREATE INDEX IF NOT EXISTS "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" (
	"UserId"
);
CREATE INDEX IF NOT EXISTS "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" (
	"UserId"
);
CREATE UNIQUE INDEX IF NOT EXISTS "RoleNameIndex" ON "AspNetRoles" (
	"NormalizedName"
);
CREATE INDEX IF NOT EXISTS "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" (
	"RoleId"
);
COMMIT;

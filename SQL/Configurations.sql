-- SQLite
PRAGMA foreign_keys=OFF;
BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "Configurations" (
    "ConfigurationID" INTEGER NOT NULL CONSTRAINT "PK_Configurations" PRIMARY KEY,
    "Key" TEXT NOT NULL,
    "Value" INTEGER NOT NULL
);
INSERT INTO Configurations VALUES(1,'BrokerURL','192.168.222.20');
COMMIT;
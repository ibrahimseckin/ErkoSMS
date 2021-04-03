BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "Purchases" (
	"Id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	"ProductCode"	INTEGER NOT NULL,
	"Amount"	INTEGER,
	"State"	INTEGER NOT NULL,
	"StartDate"	Timestamp,
	"LastModifiedDate"	Timestamp,
	"SaleId"	INTEGER NOT NULL,
	"AssignedUser"	INTEGER,
	"TotalPrice"	NUMERIC,
	FOREIGN KEY("ProductCode") REFERENCES "Products"("Code") ON DELETE CASCADE,
	FOREIGN KEY("AssignedUser") REFERENCES "AspNetUsers"("Id") ON DELETE SET NULL,
	FOREIGN KEY("SaleId") REFERENCES "Sales"("Id") ON DELETE SET NULL
);
CREATE TABLE IF NOT EXISTS "Customers" (
	"Id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	"Name"	TEXT NOT NULL UNIQUE,
	"Comment"	TEXT,
	"Owner"	TEXT,
	"OwnerMobile"	TEXT,
	"OwnerEmail"	TEXT,
	"Manager"	TEXT,
	"ManagerMobile"	TEXT,
	"ManagerEmail"	TEXT,
	"ManagerTitle"	TEXT,
	"Address"	TEXT,
	"City"	TEXT,
	"Country"	TEXT,
	"PostalCode"	TEXT,
	"CountryCode"	TEXT,
	"PhoneNumber"	TEXT,
	"FaxNumber"	TEXT,
	"Condition"	TEXT,
	"CommunicationMethod"	TEXT,
	"StartDate"	Timestamp,
	"ContactPerson"	TEXT,
	"TaxOffice"	TEXT,
	"TaxNumber"	TEXT,
	"Currency"	TEXT,
	"Region"	TEXT,
	"DiscountRate"	NUMERIC,
	"SalesRepresentative"	TEXT
);
CREATE TABLE IF NOT EXISTS "Products" (
	"Code"	varchar(25) NOT NULL,
	"CrossReferenceCode"	varchar(25),
	"Description"	varchar(4000),
	"DescriptionEng"	varchar(4000),
	"Group"	varchar(50),
	"Brand"	varchar(50),
	"Model"	varchar(50),
	"LastPrice"	decimal(6 , 2),
	PRIMARY KEY("Code")
);
CREATE TABLE IF NOT EXISTS "Sales" (
	"Id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	"UserId"	INTEGER NOT NULL,
	"State"	INTEGER NOT NULL,
	"Currency"	INTEGER NOT NULL,
	"TotalPrice"	Decimal(6 , 2) NOT NULL,
	"StartDate"	Timestamp NOT NULL,
	"LastModifiedDate"	Timestamp,
	FOREIGN KEY("UserId") REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS "Sales_Product" (
	"Id"	INTEGER NOT NULL,
	"SalesId"	INTEGER NOT NULL,
	"ProductCode"	INTEGER NOT NULL,
	"Quantity"	INTEGER NOT NULL,
	PRIMARY KEY("Id"),
	FOREIGN KEY("ProductCode") REFERENCES "Products"("Code") ON DELETE CASCADE,
	FOREIGN KEY("SalesId") REFERENCES "Sales"("Id") ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS "Stock" (
	"Id"	INTEGER NOT NULL,
	"ProductCode"	varchar(25) NOT NULL,
	"Amount"	INTEGER NOT NULL,
	"Reserved"	INTEGER,
	FOREIGN KEY("ProductCode") REFERENCES "Products"("Code") ON Delete Cascade,
	PRIMARY KEY("Id")
);
CREATE TABLE IF NOT EXISTS "Stock_History" (
	"Id"	INTEGER NOT NULL,
	"StockId"	INTEGER NOT NULL,
	"Change"	TEXT NOT NULL,
	"ChangeDate"	timestamp NOT NULL,
	FOREIGN KEY("StockId") REFERENCES "Stock"("Id"),
	PRIMARY KEY("Id")
);
CREATE TABLE IF NOT EXISTS "AspNetUserRoles" (
	"UserId"	varchar(128) NOT NULL,
	"RoleId"	varchar(128) NOT NULL,
	PRIMARY KEY("UserId","RoleId"),
	FOREIGN KEY("RoleId") REFERENCES "AspNetRoles"("Id") ON DELETE CASCADE,
	FOREIGN KEY("UserId") REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS "AspNetUserLogins" (
	"UserId"	varchar(128) NOT NULL,
	"LoginProvider"	varchar(128) NOT NULL,
	"ProviderKey"	varchar(128) NOT NULL,
	PRIMARY KEY("UserId","LoginProvider","ProviderKey"),
	FOREIGN KEY("UserId") REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS "AspNetUserClaims" (
	"Id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	"ClaimType"	varchar(256),
	"ClaimValue"	varchar(256),
	"UserId"	varchar(128) NOT NULL,
	FOREIGN KEY("UserId") REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS "AspNetUsers" (
	"Id"	varchar(128) NOT NULL,
	"Email"	varchar(256) DEFAULT NULL,
	"EmailConfirmed"	tinyint(1) NOT NULL,
	"PasswordHash"	longtext,
	"SecurityStamp"	longtext,
	"PhoneNumber"	longtext,
	"PhoneNumberConfirmed"	tinyint(1) NOT NULL,
	"TwoFactorEnabled"	tinyint(1) NOT NULL,
	"LockoutEndDateUtc"	datetime DEFAULT NULL,
	"LockoutEnabled"	tinyint(1) NOT NULL,
	"AccessFailedCount"	int(11) NOT NULL,
	"UserName"	varchar(256) NOT NULL,
	"IsActivated"	tinyint(1) NOT NULL DEFAULT (0),
	PRIMARY KEY("Id")
);
CREATE TABLE IF NOT EXISTS "AspNetRoles" (
	"Id"	varchar(128) NOT NULL,
	"Name"	varchar(256) NOT NULL,
	PRIMARY KEY("Id")
);
CREATE INDEX IF NOT EXISTS "IX_AspNetUserRoles_UserId" ON "AspNetUserRoles" (
	"UserId"
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
COMMIT;

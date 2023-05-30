BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "PlaythroughStatistics" (
	"Id"	INTEGER NOT NULL DEFAULT 1,
	"Last_Level"	INTEGER NOT NULL DEFAULT 0,
	"Current_Health"	INTEGER NOT NULL DEFAULT 0,
	"Play_Time"	INTEGER NOT NULL DEFAULT 0,
	"Death_Count"	INTEGER NOT NULL DEFAULT 0,
	"Kill_Count"	INTEGER NOT NULL DEFAULT 0,
	"Global_Id"	INTEGER NOT NULL DEFAULT 1,
	FOREIGN KEY("Global_Id") REFERENCES "GlobalStatistics"("Id"),
	PRIMARY KEY("Id")
);
CREATE TABLE IF NOT EXISTS "GlobalStatistics" (
	"Id"	INTEGER NOT NULL DEFAULT 1,
	"Max_Death_Count"	INTEGER NOT NULL DEFAULT 0,
	"Max_Play_Time"	INTEGER NOT NULL DEFAULT 0,
	"Max_Level_Reached"	INTEGER NOT NULL DEFAULT 0,
	"Max_Kill_Count"	INTEGER NOT NULL DEFAULT 0,
	PRIMARY KEY("Id")
);
COMMIT;
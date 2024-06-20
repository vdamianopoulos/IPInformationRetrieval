namespace IPInformationRetrieval.Queries
{
    public static class SeedDataQueryInSQLite
    {
        public const string query = @"
            PRAGMA foreign_keys = ON;

            CREATE TABLE Countries (
              Id INTEGER PRIMARY KEY AUTOINCREMENT,
              Name TEXT NOT NULL,
              TwoLetterCode TEXT(2) NOT NULL,
              ThreeLetterCode TEXT(3) NOT NULL,
              CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
            );


            CREATE TABLE IPAddresses (
              Id INTEGER PRIMARY KEY AUTOINCREMENT,
              CountryId INTEGER NOT NULL,
              IP TEXT(15) NOT NULL,
              CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
              UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
              FOREIGN KEY (CountryId) REFERENCES Countries(Id)
            );

            INSERT INTO Countries (Id, Name, TwoLetterCode, ThreeLetterCode, CreatedAt)
            VALUES (1,'Greece', 'GR', 'GRC', '2022-10-12T06:46:10.5000000');
            INSERT INTO Countries (Id, Name, TwoLetterCode, ThreeLetterCode, CreatedAt)
            VALUES (2,'Germany','DE','DEU','2022-10-12T06:46:10.5000000');
            INSERT INTO Countries (Id, Name, TwoLetterCode, ThreeLetterCode, CreatedAt)
            VALUES (3,'Cyprus','CY','CYP','2022-10-12T06:46:10.5000000');
            INSERT INTO Countries (Id, Name, TwoLetterCode, ThreeLetterCode, CreatedAt)
            VALUES (4,'United States','US','USA','2022-10-12T06:46:10.5000000');
            INSERT INTO Countries (Id, Name, TwoLetterCode, ThreeLetterCode, CreatedAt)
            VALUES (6,'Spain','ES','ESP','2022-10-12T06:46:10.5000000');
            INSERT INTO Countries (Id, Name, TwoLetterCode, ThreeLetterCode, CreatedAt)
            VALUES (7,'France','FR','FRA','2022-10-12T06:46:10.5000000');
            INSERT INTO Countries (Id, Name, TwoLetterCode, ThreeLetterCode, CreatedAt)
            VALUES (8,'Italy','IT','IA ','2022-10-12T06:46:10.5000000');
            INSERT INTO Countries (Id, Name, TwoLetterCode, ThreeLetterCode, CreatedAt)
            VALUES (9,'Japan','JP','JPN','2022-10-12T06:46:10.5000000');
            INSERT INTO Countries (Id, Name, TwoLetterCode, ThreeLetterCode, CreatedAt)
            VALUES (10,'China','CN','CHN','2022-10-12T06:46:10.5000000');

            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (6, 1, '44.255.255.254', '2022-10-12T07:04:06.8566667','2022-10-12T07:04:06.8566667');
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (7, 2, '45.255.255.254', '2022-10-12T07:04:06.8566667','2022-10-12T07:04:06.8566667'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (8, 3, '46.255.255.254', '2022-10-12T07:04:06.8566667','2022-10-12T07:04:06.8566667'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (9, 4, '47.255.255.254', '2022-10-12T07:04:06.8566667','2022-10-12T07:04:06.8566667'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (10, 6, '49.255.255.254', '2022-10-12T07:04:06.8566667','2022-10-12T07:04:06.8566667'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (11, 7, '41.255.255.254', '2022-10-12T07:04:06.8566667','2022-10-12T07:04:06.8566667'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (12, 8, '42.255.255.254', '2022-10-12T07:04:06.8566667','2022-10-12T07:04:06.8566667');
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt)  
            VALUES (13, 9, '43.255.255.254', '2022-10-12T07:04:06.8566667','2022-10-12T07:04:06.8566667');
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt)  
            VALUES (14, 10, '50.255.255.254', '2022-10-12T07:04:06.8566667','2022-10-12T07:04:06.8566667'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (15, 1, '44.25.55.254', '2022-10-12T07:04:33.3800000','2022-10-12T07:04:33.3800000'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (16, 2, '45.25.55.254', '2022-10-12T07:04:33.3800000','2022-10-12T07:04:33.3800000');
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt)  
            VALUES (17, 3, '46.25.55.254', '2022-10-12T07:04:33.3800000','2022-10-12T07:04:33.3800000'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (18, 4, '47.25.55.254', '2022-10-12T07:04:33.3800000','2022-10-12T07:04:33.3800000'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (19, 6, '49.25.55.254', '2022-10-12T07:04:33.3800000','2022-10-12T07:04:33.3800000'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (20, 7, '41.25.55.254', '2022-10-12T07:04:33.3800000','2022-10-12T07:04:33.3800000');
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt)  
            VALUES (21, 8, '42.25.55.254', '2022-10-12T07:04:33.3800000','2022-10-12T07:04:33.3800000'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (22, 9, '43.25.55.254', '2022-10-12T07:04:33.3800000','2022-10-12T07:04:33.3800000'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (23, 10, '50.25.55.254', '2022-10-12T07:04:33.3800000','2022-10-12T07:04:33.3800000'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (24, 1, '44.25.55.4', '2022-10-12T07:04:51.3233333','2022-10-12T07:04:51.3233333'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (25, 2, '45.25.55.4', '2022-10-12T07:04:51.3233333','2022-10-12T07:04:51.3233333'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (26, 3, '46.25.55.4', '2022-10-12T07:04:51.3233333','2022-10-12T07:04:51.3233333'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (27, 4, '47.25.55.4', '2022-10-12T07:04:51.3233333','2022-10-12T07:04:51.3233333'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (28, 6, '49.25.55.4', '2022-10-12T07:04:51.3233333','2022-10-12T07:04:51.3233333'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (29, 7, '41.25.55.4', '2022-10-12T07:04:51.3233333','2022-10-12T07:04:51.3233333'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (30, 8, '42.25.55.4', '2022-10-12T07:04:51.3233333','2022-10-12T07:04:51.3233333'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (31, 9, '43.25.55.4', '2022-10-12T07:04:51.3233333','2022-10-12T07:04:51.3233333'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (32, 10, '50.25.55.4', '2022-10-12T07:04:51.3233333','2022-10-12T07:04:51.3233333'); 
            INSERT INTO IPAddresses (Id, CountryId, IP, CreatedAt, UpdatedAt) 
            VALUES (33, 1, '10.20.30.40', '2022-10-12T08:41:37.3100000','2022-10-12T08:41:37.3100000'); 
";
    }
}

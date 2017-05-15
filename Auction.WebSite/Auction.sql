IF DB_ID('Auction') IS NOT NULL
	EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'Auction'
	GO
	USE [master]
	GO
	ALTER DATABASE [Auction] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
	GO
	DROP DATABASE [Auction]
	GO

CREATE DATABASE Auction;
GO
USE Auction;
GO

CREATE TABLE AuctionAdvertiser (
	Name VARCHAR(60) NOT NULL,
    UserName VARCHAR(40) PRIMARY KEY,
    UserPassword BINARY(64) NOT NULL
);
GO

CREATE TABLE AuctionGuest (
	Name VARCHAR(60) NOT NULL,
    UserName VARCHAR(40) PRIMARY KEY,
    UserPassword BINARY(64) NOT NULL,
    PhoneNumber VARCHAR(20) NOT NULL,
    Email VARCHAR(100) NOT NULL
);
GO

CREATE TABLE AuctionCategory
(
	Id INTEGER PRIMARY KEY IDENTITY(1,1),
	Name VARCHAR(30) NOT NULL
);
GO

CREATE TABLE AuctionObject (
    Id INTEGER PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(50) NOT NULL,
	CategoryId INTEGER NOT NULL,
	Description VARCHAR(500) NOT NULL,
    AdvertiserId VARCHAR(40) NOT NULL,
    StartBiddingAmount INTEGER NOT NULL,
	StartDate DATETIME NOT NULL,
	EndDate DATETIME NOT NULL,
	ObjectImage VARCHAR(100)
    CONSTRAINT ObjectToCategory
        FOREIGN KEY (CategoryId) 
        REFERENCES AuctionCategory (Id),
	CONSTRAINT ObjectToAdvertiser
        FOREIGN KEY (AdvertiserId) 
        REFERENCES AuctionAdvertiser (UserName)
);
GO

CREATE TABLE Bidding (
	Id INTEGER PRIMARY KEY IDENTITY(1, 1),
    ObjectId INTEGER NOT NULL,
	UserName VARCHAR(40) NOT NULL,
	BiddingAmount INTEGER NOT NULL,
	BiddingDate DateTIME NOT NULL
    CONSTRAINT BiddingToUser
        FOREIGN KEY (UserName) 
        REFERENCES AuctionGuest (UserName),
    CONSTRAINT BiddingToObject
        FOREIGN KEY (ObjectId) 
        REFERENCES AuctionObject (Id)
);
GO

INSERT INTO AuctionAdvertiser VALUES('Horváth Eszter', 'admin', CAST('123456' AS BINARY(64)));

INSERT INTO AuctionCategory VALUES('Antik tárgyak, régiségek');
INSERT INTO AuctionCategory VALUES('Ékszerek');
INSERT INTO AuctionCategory VALUES('Ingatlanok');
INSERT INTO AuctionCategory VALUES('Járművek');

INSERT INTO AuctionObject VALUES('Antik váza', 1, 'Sötét fekete üveg váza, görög jellegű arany díszítéssel. Kora 1920-as évekre tehető. Állapota ép, hibátlan.', 'admin', 50000, CONVERT(datetime, '2017.04.04', 120), CONVERT(datetime, '2017.06.10', 120), NULL);
INSERT INTO AuctionObject VALUES('Neobarokk stílusú intarziás szalonasztal', 1, 'Átmérője 64 cm, magassága 63 cm. Megkímélt állapotú.', 'admin', 20000, CONVERT(datetime, '2017.04.02', 120), CONVERT(datetime, '2017.05.10', 120), NULL);
INSERT INTO AuctionObject VALUES('Antik falióra', 1, 'Ónémet, oszlopos falióra. Szerkezete működőképes, eredete 1880 körülre tehető. Faragott díszítésű masszív tölgyfa. Mérete 118x52x26 cm', 'admin', 10000, CONVERT(datetime, '2017.03.05', 120), CONVERT(datetime, '2017.04.20', 120), NULL);
INSERT INTO AuctionObject VALUES('1956-os debreceni Néplap, rendkívüli kiadás', 1, 'A debreceni Néplap 1956. október 23-i száma s egyúttal különkiadása, benne a debreceni egyetemi ifjúság követelései tizenkét pontba foglalva.', 'admin', 5000, CONVERT(datetime, '2017.04.12', 120), CONVERT(datetime, '2017.07.16', 120), NULL);
INSERT INTO AuctionObject VALUES('Antik olajfestmény', 1, 'Vary Vojtovity Sajóparti részlet olaj festmény.', 'admin', 200000, CONVERT(datetime, '2017.03.05', 120), CONVERT(datetime, '2017.08.26', 120), NULL);
INSERT INTO AuctionObject VALUES('Antik csillár', 1, '12 ágú, füstarannyal bevont fa csillár.', 'admin', 15000, CONVERT(datetime, '2017.04.08', 120), CONVERT(datetime, '2017.05.01', 120), NULL);

INSERT INTO AuctionObject VALUES('Arany gyűrű gyémánttal', 2, '14K sárga aranyból készült gyűrű gyémánt kővel. Súlya 4,4 gramm. 14K aranyból készült gyémánt gyűrűt tartalmaz.', 'admin', 145000, CONVERT(datetime, '2017.03.08', 120), CONVERT(datetime, '2017.04.14', 120), NULL);
INSERT INTO AuctionObject VALUES('Swarovski kristállyal díszített fülbevaló', 2, 'Divatos világos rózsaszín kerek Swarovski Elements kristállyal díszített akasztós fülbevaló, melyet sok apró fehér kristály tesz még csillogóbbá. Rozé arany bevonattal.', 'admin', 5000, CONVERT(datetime, '2017.04.08', 120), CONVERT(datetime, '2017.05.01', 120), NULL);
INSERT INTO AuctionObject VALUES('Ragyogó mentaszínű karperec', 2, 'Karkötő: 	Ezüst, Anyaga: 	Tűzzománc, Színe: 	Zöld, Drágakövek: 	Cirkónia', 'admin', 20000, CONVERT(datetime, '2017.04.01', 120), CONVERT(datetime, '2017.04.12', 120), NULL);
INSERT INTO AuctionObject VALUES('Fehérarany baglyos nyaklánc', 2, 'Fehérarany bevonatú nyaklánc, áttetsző cirkónia kristályos díszítéssel. Súlya 6 g.', 'admin', 3000, CONVERT(datetime, '2017.04.15', 120), CONVERT(datetime, '2017.04.22', 120), NULL);
INSERT INTO AuctionObject VALUES('Virágos bross', 2, 'Menyasszonyi, alkalmi, fellépő- és táncruhákhoz kiváló díszítő elem.', 'admin', 5000, CONVERT(datetime, '2017.04.08', 120), CONVERT(datetime, '2017.05.01', 120), NULL);
INSERT INTO AuctionObject VALUES('Platinával bevont férfi karóra', 2, 'A számlap méretei: 4,4 cm x 4,4 cm. Az óra teljes hossza a szíjjal: 23 cm. Óraszerkezet : Quartz. Elegáns, klasszikus stílusú karóra.', 'admin', 8000, CONVERT(datetime, '2017.04.15', 120), CONVERT(datetime, '2017.05.15', 120), 'karora.jpg');

INSERT INTO AuctionObject VALUES('Nagykanizsa, Városkapu körút', 3, 'Eladó Nagykanizsán a Városkapu körúton egy 40 nm-s, 1 szobás ingatlan. A 4. emeleti lakás, nyugati tájolásának köszönhetően nagyon világos.', 'admin', 100000, CONVERT(datetime, '2017.03.10', 120), CONVERT(datetime, '2017.05.10', 120), NULL);
INSERT INTO AuctionObject VALUES('Szombathely, Barátság utca', 3, 'Eladó egy első emeleti , francia erkélyes, jó állapotú, ablakcserés, külső szigetelt lakás, csendes környezetben, közel a belvároshoz. ', 'admin', 3500000, CONVERT(datetime, '2017.02.18', 120), CONVERT(datetime, '2017.04.18', 120), NULL);
INSERT INTO AuctionObject VALUES('Kaposvár, József Attila utca', 3, 'Kaposvári, belvárosi, jó elrendezésű, amerikai konyhás, cirkó kazános, gázfűtéses lakásomat eladásra kínálom. Berendezés megegyezés esetén az árban. Rezsije alacsony. A mélygarázsban beálló bérelhető. ', 'admin', 4000000, CONVERT(datetime, '2017.04.12', 120), CONVERT(datetime, '2017.05.12', 120), NULL);
INSERT INTO AuctionObject VALUES('Debrecen, Szoboszlói út', 3, 'A belváros közelében, másfél szobás, első emeleti tégla lakás eladó. A lakás 39 nm, 1+fél szobás, házközponti mértfűtéses, kamrával és saját, zárt 5 nm-es pincével rendelkezik.', 'admin', 2300000, CONVERT(datetime, '2017.04.01', 120), CONVERT(datetime, '2017.05.01', 120), NULL);
INSERT INTO AuctionObject VALUES('Budapest, IX. kerület', 3, '4 emeletes társasház első emeletén eladó ez az 53 nm-es, 1,5 szobás erkélyes lakás.', 'admin', 7000000, CONVERT(datetime, '2017.03.08', 120), CONVERT(datetime, '2017.06.08', 120), NULL);
INSERT INTO AuctionObject VALUES('Békéscsaba, Munkácsy utca', 3, 'Eladó Békéscsaba belvárosában liftes házban I. emeleti 122 m2-es erkélyes lakás. Az ingatlan csendes sétányra néző erkéllyel és ablakokkal egész évben napsütötte, világos.', 'admin', 3900000, CONVERT(datetime, '2017.03.21', 120), CONVERT(datetime, '2017.06.12', 120), NULL);

INSERT INTO AuctionObject VALUES('Opel Vectra', 4, 'Kiváló motorral, 6-7 literes fogyasztással, koppanás mentes futóművel, rozsdamentes karosszériával, 2 db gyári kulcsával eladó.', 'admin', 300000, CONVERT(datetime, '2017.03.02', 120), CONVERT(datetime, '2017.05.02', 120), NULL);
INSERT INTO AuctionObject VALUES('Skoda Octavia', 4, '2008. 06. havi, sérülésmentes, vezetett szervízkönyves.', 'admin', 500000, CONVERT(datetime, '2017.02.05', 120), CONVERT(datetime, '2017.04.25', 120), NULL);
INSERT INTO AuctionObject VALUES('Fiat Punto', 4, 'Rendszeresen karbantartott, frissen szervizelt. Klímás, elektromos ablakokkal.', 'admin', 200000, CONVERT(datetime, '2017.04.15', 120), CONVERT(datetime, '2017.05.15', 120), NULL);
INSERT INTO AuctionObject VALUES('Ford Mondeo', 4, 'Évjárat: 2013/3, Állapot: Újszerű, Kilométeróra állása: 82 000, Üzemanyag: Dízel.', 'admin', 800000, CONVERT(datetime, '2017.03.31', 120), CONVERT(datetime, '2017.05.31', 120), NULL);
INSERT INTO AuctionObject VALUES('Talajmaró kistraktor', 4, 'Diesel, elektromos, önindítós, akkumulátoros.', 'admin', 20000, CONVERT(datetime, '2017.04.15', 120), CONVERT(datetime, '2017.05.15', 120), NULL);
INSERT INTO AuctionObject VALUES('Renault Megane', 4, 'Eladó Renault Megane Cabrio 1.6 benzin 1999-es évjárat, külföldi papírokkal, 1 év műszaki vizsgával, rozsdamentes, jó karosszériával, új gumikkal, új fékekkel.', 'admin', 10000, CONVERT(datetime, '2017.04.01', 120), CONVERT(datetime, '2017.06.01', 120), NULL);

GO
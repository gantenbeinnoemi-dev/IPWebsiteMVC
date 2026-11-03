-- =====================================================================
-- MyBooks - Testdaten (MySQL). Erst schema.sql ausfuehren, dann dieses Skript.
-- Enthaelt die Buecher aus den Figma-Screens plus bekannte Buecher fuer Tests.
-- Cover: Open Library (covers.openlibrary.org), liegen unter wwwroot/images/covers.
-- Benutzer werden NICHT hier angelegt: Registrierung ueber die Website, Admin per appsettings.Local.json.
-- =====================================================================

USE mybooksdb;

INSERT INTO Land (Name) VALUES
    ('USA'),
    ('Australien'),
    ('Grossbritannien'),
    ('Deutschland'),
    ('Schweden'),
    ('Irland'),
    ('Zypern');

INSERT INTO Autor (Vorname, Nachname, LandID) VALUES
    ('Chris', 'Colfer', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Lynette', 'Noni', (SELECT LandID FROM Land WHERE Name = 'Australien')),
    ('J.K.', 'Rowling', (SELECT LandID FROM Land WHERE Name = 'Grossbritannien')),
    ('J.R.R.', 'Tolkien', (SELECT LandID FROM Land WHERE Name = 'Grossbritannien')),
    ('C.S.', 'Lewis', (SELECT LandID FROM Land WHERE Name = 'Grossbritannien')),
    ('George R.R.', 'Martin', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Patrick', 'Rothfuss', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Christopher', 'Paolini', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Rick', 'Riordan', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Philip', 'Pullman', (SELECT LandID FROM Land WHERE Name = 'Grossbritannien')),
    ('Brandon', 'Sanderson', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Cornelia', 'Funke', (SELECT LandID FROM Land WHERE Name = 'Deutschland')),
    ('Leigh', 'Bardugo', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Sarah J.', 'Maas', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Stephenie', 'Meyer', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Jane', 'Austen', (SELECT LandID FROM Land WHERE Name = 'Grossbritannien')),
    ('Nicholas', 'Sparks', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Jojo', 'Moyes', (SELECT LandID FROM Land WHERE Name = 'Grossbritannien')),
    ('John', 'Green', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Diana', 'Gabaldon', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Colleen', 'Hoover', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Sally', 'Thorne', (SELECT LandID FROM Land WHERE Name = 'Australien')),
    ('Casey', 'McQuiston', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Emily', 'Henry', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Charlotte', 'Brontë', (SELECT LandID FROM Land WHERE Name = 'Grossbritannien')),
    ('Stephanie', 'Perkins', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Jenny', 'Han', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Agatha', 'Christie', (SELECT LandID FROM Land WHERE Name = 'Grossbritannien')),
    ('Stieg', 'Larsson', (SELECT LandID FROM Land WHERE Name = 'Schweden')),
    ('Gillian', 'Flynn', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Arthur Conan', 'Doyle', (SELECT LandID FROM Land WHERE Name = 'Grossbritannien')),
    ('Dan', 'Brown', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Liane', 'Moriarty', (SELECT LandID FROM Land WHERE Name = 'Australien')),
    ('Tana', 'French', (SELECT LandID FROM Land WHERE Name = 'Irland')),
    ('Alex', 'Michaelides', (SELECT LandID FROM Land WHERE Name = 'Zypern')),
    ('Richard', 'Osman', (SELECT LandID FROM Land WHERE Name = 'Grossbritannien')),
    ('Patrick', 'Süskind', (SELECT LandID FROM Land WHERE Name = 'Deutschland')),
    ('Frank', 'Herbert', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Douglas', 'Adams', (SELECT LandID FROM Land WHERE Name = 'Grossbritannien')),
    ('Orson Scott', 'Card', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('George', 'Orwell', (SELECT LandID FROM Land WHERE Name = 'Grossbritannien')),
    ('Andy', 'Weir', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Ernest', 'Cline', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('William', 'Gibson', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Isaac', 'Asimov', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Suzanne', 'Collins', (SELECT LandID FROM Land WHERE Name = 'USA')),
    ('Frank', 'Schätzing', (SELECT LandID FROM Land WHERE Name = 'Deutschland'));

-- Altersklassen: Kinder (unter 10), Preteens (10-12), Jugend (12-16), Young adult (16-21), Erwachsene (ab 21)
INSERT INTO Altersklassifikation (Bezeichnung) VALUES ('Kinder'), ('Preteens'), ('Jugend'), ('Young adult'), ('Erwachsene');

INSERT INTO Genre (Bezeichnung) VALUES ('Fantasy'), ('Romance'), ('Krimi'), ('Science-Fiction');

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Die Suche nach dem Wunschzauber', 'Land of Stories', 1, (SELECT AutorID FROM Autor WHERE Vorname = 'Chris' AND Nachname = 'Colfer'), 2012, '/images/covers/land-of-stories-1.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Preteens';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Die Rückkehr der Zauberin', 'Land of Stories', 2, (SELECT AutorID FROM Autor WHERE Vorname = 'Chris' AND Nachname = 'Colfer'), 2013, '/images/covers/land-of-stories-2.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Preteens';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Eine düstere Warnung', 'Land of Stories', 3, (SELECT AutorID FROM Autor WHERE Vorname = 'Chris' AND Nachname = 'Colfer'), 2014, '/images/covers/land-of-stories-3.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Preteens';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Ein Königreich in Gefahr', 'Land of Stories', 4, (SELECT AutorID FROM Autor WHERE Vorname = 'Chris' AND Nachname = 'Colfer'), 2015, '/images/covers/land-of-stories-4.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Preteens';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Die Macht der Geschichten', 'Land of Stories', 5, (SELECT AutorID FROM Autor WHERE Vorname = 'Chris' AND Nachname = 'Colfer'), 2016, '/images/covers/land-of-stories-5.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Preteens';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Der Kampf der Welten', 'Land of Stories', 6, (SELECT AutorID FROM Autor WHERE Vorname = 'Chris' AND Nachname = 'Colfer'), 2017, '/images/covers/land-of-stories-6.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Preteens';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Not My Problem', NULL, NULL, NULL, NULL, '/images/covers/not-my-problem.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Romance');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('The Prison Healer', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Lynette' AND Nachname = 'Noni'), NULL, '/images/covers/the-prison-healer.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Romance');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Die drei ??? und der schwarze Fluch', 'Die drei ???', NULL, NULL, NULL, '/images/covers/die-drei-schwarzer-fluch.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Krimi');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Preteens';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Die drei ??? und das weiße Auge', 'Die drei ???', NULL, NULL, NULL, '/images/covers/die-drei-weisses-auge.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Krimi');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Preteens';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Harry Potter and the Philosopher''s Stone', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'J.K.' AND Nachname = 'Rowling'), 1997, '/images/covers/harry-potter-and-the-philosopher-s-stone.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Preteens';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('The Hobbit', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'J.R.R.' AND Nachname = 'Tolkien'), 1937, '/images/covers/the-hobbit.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Preteens';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('The Fellowship of the Ring', 'The Lord of the Rings', 1, (SELECT AutorID FROM Autor WHERE Vorname = 'J.R.R.' AND Nachname = 'Tolkien'), 1954, '/images/covers/the-fellowship-of-the-ring.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('The Lion, the Witch and the Wardrobe', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'C.S.' AND Nachname = 'Lewis'), 1950, '/images/covers/the-lion-the-witch-and-the-wardrobe.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Preteens';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('A Game of Thrones', 'A Song of Ice and Fire', 1, (SELECT AutorID FROM Autor WHERE Vorname = 'George R.R.' AND Nachname = 'Martin'), 1996, '/images/covers/a-game-of-thrones.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Young adult';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('The Name of the Wind', 'The Kingkiller Chronicle', 1, (SELECT AutorID FROM Autor WHERE Vorname = 'Patrick' AND Nachname = 'Rothfuss'), 2007, '/images/covers/the-name-of-the-wind.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Young adult';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Eragon', 'The Inheritance Cycle', 1, (SELECT AutorID FROM Autor WHERE Vorname = 'Christopher' AND Nachname = 'Paolini'), 2002, '/images/covers/eragon.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('The Lightning Thief', 'Percy Jackson', 1, (SELECT AutorID FROM Autor WHERE Vorname = 'Rick' AND Nachname = 'Riordan'), 2005, '/images/covers/the-lightning-thief.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Preteens';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Northern Lights', 'His Dark Materials', 1, (SELECT AutorID FROM Autor WHERE Vorname = 'Philip' AND Nachname = 'Pullman'), 1995, '/images/covers/northern-lights.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Mistborn: The Final Empire', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Brandon' AND Nachname = 'Sanderson'), 2006, '/images/covers/mistborn-the-final-empire.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Inkheart', 'Inkworld', 1, (SELECT AutorID FROM Autor WHERE Vorname = 'Cornelia' AND Nachname = 'Funke'), 2003, '/images/covers/inkheart.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Preteens';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Six of Crows', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Leigh' AND Nachname = 'Bardugo'), 2015, '/images/covers/six-of-crows.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('A Court of Thorns and Roses', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Sarah J.' AND Nachname = 'Maas'), 2015, '/images/covers/a-court-of-thorns-and-roses.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Fantasy', 'Romance');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Young adult';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('The Way of Kings', 'The Stormlight Archive', 1, (SELECT AutorID FROM Autor WHERE Vorname = 'Brandon' AND Nachname = 'Sanderson'), 2010, '/images/covers/the-way-of-kings.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Twilight', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Stephenie' AND Nachname = 'Meyer'), 2005, '/images/covers/twilight.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Romance', 'Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Pride and Prejudice', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Jane' AND Nachname = 'Austen'), 1813, '/images/covers/pride-and-prejudice.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Romance');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('The Notebook', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Nicholas' AND Nachname = 'Sparks'), 1996, '/images/covers/the-notebook.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Romance');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Me Before You', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Jojo' AND Nachname = 'Moyes'), 2012, '/images/covers/me-before-you.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Romance');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('The Fault in Our Stars', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'John' AND Nachname = 'Green'), 2012, '/images/covers/the-fault-in-our-stars.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Romance');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Outlander', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Diana' AND Nachname = 'Gabaldon'), 1991, '/images/covers/outlander.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Romance', 'Fantasy');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Young adult';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('It Ends with Us', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Colleen' AND Nachname = 'Hoover'), 2016, '/images/covers/it-ends-with-us.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Romance');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Young adult';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('The Hating Game', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Sally' AND Nachname = 'Thorne'), 2016, '/images/covers/the-hating-game.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Romance');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Young adult';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Red, White & Royal Blue', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Casey' AND Nachname = 'McQuiston'), 2019, '/images/covers/red-white-and-royal-blue.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Romance');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Young adult';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Beach Read', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Emily' AND Nachname = 'Henry'), 2020, '/images/covers/beach-read.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Romance');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Young adult';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Jane Eyre', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Charlotte' AND Nachname = 'Brontë'), 1847, '/images/covers/jane-eyre.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Romance');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Anna and the French Kiss', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Stephanie' AND Nachname = 'Perkins'), 2010, '/images/covers/anna-and-the-french-kiss.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Romance');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('To All the Boys I''ve Loved Before', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Jenny' AND Nachname = 'Han'), 2014, '/images/covers/to-all-the-boys-i-ve-loved-before.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Romance');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Murder on the Orient Express', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Agatha' AND Nachname = 'Christie'), 1934, '/images/covers/murder-on-the-orient-express.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Krimi');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('And Then There Were None', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Agatha' AND Nachname = 'Christie'), 1939, '/images/covers/and-then-there-were-none.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Krimi');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('The Girl with the Dragon Tattoo', 'Millennium', 1, (SELECT AutorID FROM Autor WHERE Vorname = 'Stieg' AND Nachname = 'Larsson'), 2005, '/images/covers/the-girl-with-the-dragon-tattoo.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Krimi');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Young adult';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Gone Girl', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Gillian' AND Nachname = 'Flynn'), 2012, '/images/covers/gone-girl.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Krimi');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Young adult';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('The Hound of the Baskervilles', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Arthur Conan' AND Nachname = 'Doyle'), 1902, '/images/covers/the-hound-of-the-baskervilles.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Krimi');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('A Study in Scarlet', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Arthur Conan' AND Nachname = 'Doyle'), 1887, '/images/covers/a-study-in-scarlet.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Krimi');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('The Da Vinci Code', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Dan' AND Nachname = 'Brown'), 2003, '/images/covers/the-da-vinci-code.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Krimi');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Big Little Lies', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Liane' AND Nachname = 'Moriarty'), 2014, '/images/covers/big-little-lies.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Krimi');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Young adult';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('In the Woods', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Tana' AND Nachname = 'French'), 2007, '/images/covers/in-the-woods.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Krimi');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Young adult';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('The Silent Patient', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Alex' AND Nachname = 'Michaelides'), 2019, '/images/covers/the-silent-patient.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Krimi');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Young adult';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('The Thursday Murder Club', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Richard' AND Nachname = 'Osman'), 2020, '/images/covers/the-thursday-murder-club.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Krimi');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Perfume: The Story of a Murderer', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Patrick' AND Nachname = 'Süskind'), 1985, '/images/covers/perfume-the-story-of-a-murderer.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Krimi');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Young adult';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Dune', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Frank' AND Nachname = 'Herbert'), 1965, '/images/covers/dune.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Science-Fiction');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('The Hitchhiker''s Guide to the Galaxy', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Douglas' AND Nachname = 'Adams'), 1979, '/images/covers/the-hitchhiker-s-guide-to-the-galaxy.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Science-Fiction');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Ender''s Game', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Orson Scott' AND Nachname = 'Card'), 1985, '/images/covers/ender-s-game.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Science-Fiction');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('1984', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'George' AND Nachname = 'Orwell'), 1949, '/images/covers/1984.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Science-Fiction');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('The Martian', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Andy' AND Nachname = 'Weir'), 2011, '/images/covers/the-martian.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Science-Fiction');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Project Hail Mary', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Andy' AND Nachname = 'Weir'), 2021, '/images/covers/project-hail-mary.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Science-Fiction');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Ready Player One', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Ernest' AND Nachname = 'Cline'), 2011, '/images/covers/ready-player-one.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Science-Fiction');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Neuromancer', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'William' AND Nachname = 'Gibson'), 1984, '/images/covers/neuromancer.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Science-Fiction');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Young adult';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Foundation', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Isaac' AND Nachname = 'Asimov'), 1951, '/images/covers/foundation.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Science-Fiction');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('The Hunger Games', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Suzanne' AND Nachname = 'Collins'), 2008, '/images/covers/the-hunger-games.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Science-Fiction');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Jugend';

INSERT INTO Buch (Titel, Reihe, Band, AutorID, ErscheinungsJahr, CoverBildUrl) VALUES ('Der Schwarm', NULL, NULL, (SELECT AutorID FROM Autor WHERE Vorname = 'Frank' AND Nachname = 'Schätzing'), 2004, '/images/covers/der-schwarm.jpg');
SET @b = LAST_INSERT_ID();
INSERT INTO Buch_Genre (BuchID, GenreID) SELECT @b, GenreID FROM Genre WHERE Bezeichnung IN ('Science-Fiction');
INSERT INTO Buch_Altersklassifikation (BuchID, AltersklassifikationID) SELECT @b, AltersklassifikationID FROM Altersklassifikation WHERE Bezeichnung = 'Young adult';

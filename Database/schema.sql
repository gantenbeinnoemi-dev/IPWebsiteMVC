-- =====================================================================
-- MyBooks - Datenbankschema (MySQL 8)
--
-- Katalog (fuer alle sichtbar):
--   Buch --- Buch_Genre --- Genre                                (Startseite, Reiter, Dropdown "Genre")
--   Buch --- Buch_Altersklassifikation --- Altersklassifikation  (Dropdown "Altersklasse")
--   Buch --- Autor --- Land                                      (Dropdown "Autor")
--
-- Benutzerbereich (nur wenn angemeldet):
--   Benutzer --- Kategorie --- Kategorie_Buch --- Buch
--   Jeder Benutzer erstellt eigene Kategorien (z.B. "Schon gelesen",
--   "Lieblingsbuecher") und legt Buecher hinein. Der Benutzer ist NICHT
--   mit Genre verknuepft.
--
-- ACHTUNG: Das Skript loescht und erstellt alle Tabellen neu, dabei gehen ALLE Daten
-- verloren, auch registrierte Benutzer. Nur zum Zuruecksetzen der Entwicklungsdatenbank benutzen.
-- (Der Admin wird beim naechsten Start der Website automatisch wieder angelegt.)
-- =====================================================================

CREATE DATABASE IF NOT EXISTS mybooksdb
    CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE mybooksdb;

DROP TABLE IF EXISTS Kategorie_Buch;
DROP TABLE IF EXISTS Kategorie;
DROP TABLE IF EXISTS Benutzer;
DROP TABLE IF EXISTS Buch_Altersklassifikation;
DROP TABLE IF EXISTS Buch_Genre;
DROP TABLE IF EXISTS Buch;
DROP TABLE IF EXISTS Genre;
DROP TABLE IF EXISTS Altersklassifikation;
DROP TABLE IF EXISTS Autor;
DROP TABLE IF EXISTS Land;

CREATE TABLE Land (
    LandID INT AUTO_INCREMENT PRIMARY KEY,
    Name   VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Autor (
    AutorID  INT AUTO_INCREMENT PRIMARY KEY,
    Vorname  VARCHAR(100) NOT NULL,
    Nachname VARCHAR(100) NOT NULL,
    LandID   INT NULL,
    FOREIGN KEY (LandID) REFERENCES Land(LandID)
);

CREATE TABLE Altersklassifikation (
    AltersklassifikationID INT AUTO_INCREMENT PRIMARY KEY,
    Bezeichnung            VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Genre (
    GenreID     INT AUTO_INCREMENT PRIMARY KEY,
    Bezeichnung VARCHAR(100) NOT NULL UNIQUE
);

-- Listenansicht zeigt: "<Reihe> Band <Band>" / "<Titel>"
CREATE TABLE Buch (
    BuchID           INT AUTO_INCREMENT PRIMARY KEY,
    Titel            VARCHAR(255) NOT NULL,
    Reihe            VARCHAR(255) NULL,
    Band             INT NULL,
    AnzahlKapitel    INT NULL,
    AutorID          INT NULL,
    CoverBildUrl     VARCHAR(500) NULL,
    Beschreibung     TEXT NULL,
    ErscheinungsJahr INT NULL,
    FOREIGN KEY (AutorID) REFERENCES Autor(AutorID)
);

CREATE TABLE Buch_Genre (
    BuchID  INT NOT NULL,
    GenreID INT NOT NULL,
    PRIMARY KEY (BuchID, GenreID),
    FOREIGN KEY (BuchID)  REFERENCES Buch(BuchID)   ON DELETE CASCADE,
    FOREIGN KEY (GenreID) REFERENCES Genre(GenreID) ON DELETE CASCADE
);

CREATE TABLE Buch_Altersklassifikation (
    BuchID                 INT NOT NULL,
    AltersklassifikationID INT NOT NULL,
    PRIMARY KEY (BuchID, AltersklassifikationID),
    FOREIGN KEY (BuchID) REFERENCES Buch(BuchID) ON DELETE CASCADE,
    FOREIGN KEY (AltersklassifikationID) REFERENCES Altersklassifikation(AltersklassifikationID) ON DELETE CASCADE
);

-- Registrierung/Login mit Benutzername + Passwort.
--   Name         eindeutig (UNIQUE, Gross-/Kleinschreibung egal): jeder Benutzername existiert nur einmal.
--   PasswortHash nie Klartext; ASP.NET-PasswordHasher (PBKDF2 mit Salt).
--   Rolle        'Benutzer' oder 'Admin' (Admin wird beim Start der Website aus appsettings.Local.json angelegt).
--   Email        optional, wird bei der Registrierung nicht abgefragt.
CREATE TABLE Benutzer (
    BenutzerID   INT AUTO_INCREMENT PRIMARY KEY,
    Name         VARCHAR(100) NOT NULL UNIQUE,
    Email        VARCHAR(255) NULL UNIQUE,
    PasswortHash VARCHAR(255) NOT NULL,
    Rolle        VARCHAR(20) NOT NULL DEFAULT 'Benutzer',
    ErstelltAm   DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Kategorie (
    KategorieID INT AUTO_INCREMENT PRIMARY KEY,
    BenutzerID  INT NOT NULL,
    Name        VARCHAR(100) NOT NULL,
    ErstelltAm  DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UNIQUE (BenutzerID, Name),
    FOREIGN KEY (BenutzerID) REFERENCES Benutzer(BenutzerID) ON DELETE CASCADE
);

CREATE TABLE Kategorie_Buch (
    KategorieID    INT NOT NULL,
    BuchID         INT NOT NULL,
    HinzugefuegtAm DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (KategorieID, BuchID),
    FOREIGN KEY (KategorieID) REFERENCES Kategorie(KategorieID) ON DELETE CASCADE,
    FOREIGN KEY (BuchID)      REFERENCES Buch(BuchID)           ON DELETE CASCADE
);

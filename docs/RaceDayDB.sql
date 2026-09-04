USE master;
GO

IF DB_ID('RaceDayDB') IS NOT NULL
BEGIN
    ALTER DATABASE RaceDayDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE RaceDayDB;
END
GO


Create DATABASE RaceDayDB;
GO


USE RaceDayDB;
GO

Create Table Users
(
    UserID INT Identity(1,1) Primary Key,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(150) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    PhoneNumber VARCHAR(20) NULL,
    Role VARCHAR(20) NOT NULL,

    CONSTRAINT CK_Users_Role
         Check (Role IN ('Participant', 'Organiser'))
);
GO

Create Table Events 
(
     EventID INT Identity (1,1) Primary Key,
     OrganiserID INT NOT NULL,
     EventName VARCHAR(150) NOT NULL,
     Description VARCHAR(1000),
     EventType VARCHAR(50) NOT NULL,
     EventDate DATE NOT NULL,
     StartTime TIME,
     Location VARCHAR(100),
     City VARCHAR(100),
     Province VARCHAR(100),
     Status VARCHAR(30) NOT NULL DEFAULT 'Upcoming',

    CONSTRAINT FK_Events_Users
      FOREIGN KEY (OrganiserID)
      REFERENCES Users(UserID),

    CONSTRAINT CK_Events_Status
      CHECK(Status IN
      ('Upcoming', 'Open', 'Closed', 'Completed', 'Cancelled'))
);
GO

Create Table Categories
(
     CategoryID INT Identity (1,1) Primary Key,
     CategoryName VARCHAR(100) NOT NULL UNIQUE,
     Description VARCHAR(500)
);
GO

Create Table EventCategories
(
     EventCategoryID INT Identity(1,1) Primary Key,
     EventID INT NOT NULL,
     CategoryID INT NOT NULL,
     EntryFee DECIMAL(10,2) DEFAULT 0,
     MaxParticipants INT,

     CONSTRAINT FK_EventCategories_Events
       FOREIGN KEY (EventID)
       REFERENCES Events(EventID),

     CONSTRAINT FK_EventCategories_Categories
       FOREIGN KEY (CategoryID)
       REFERENCES Categories(CategoryID),

     CONSTRAINT UQ_Event_Category
        UNIQUE (EventID, CategoryID),

     CONSTRAINT CK_EventCategories_Fee
        CHECK (EntryFee >= 0),

     CONSTRAINT CK_EventCategories_MaxParticipants
        CHECK (MaxParticipants IS NULL OR MaxParticipants > 0)
);
GO

 Create Table Enrolments
 (
     EnrolmentID INT Identity (1,1) Primary Key,
     EventID INT NOT NULL,
     UserID INT NOT NULL,
     EventCategoryID INT NOT NULL,
     EnrolmentDate DATETIME NOT NULL DEFAULT GETDATE(),
     Status VARCHAR(30) NOT NULL DEFAULT 'Registered',

     CONSTRAINT FK_Enrolments_Events
        FOREIGN KEY (EventID)
        REFERENCES Events(EventID),

     CONSTRAINT FK_Enrolments_Users
        FOREIGN KEY (UserID)
        REFERENCES Users(UserID),

     CONSTRAINT FK_Enrolments_EventCategories
        FOREIGN KEY (EventCategoryID)
        REFERENCES EventCategories(EventCategoryID),

     CONSTRAINT UQ_User_Event
        UNIQUE (UserID, EventID),

     CONSTRAINT CK_Enrolments_Status
     CHECK (Status IN
     ('Registered', 'Cancelled', 'Completed'))
);
GO

Create Table Results
( 
     ResultID INT Identity(1,1) Primary Key,
     EnrolmentID INT NOT NULL,
     FinishTime TIME NOT NULL,
     Position INT,
     Pace VARCHAR(20),
     ResultStatus VARCHAR(30) DEFAULT 'Finished',

     CONSTRAINT FK_Results_Enrolments
        FOREIGN KEY (EnrolmentID)
        REFERENCES Enrolments(EnrolmentID),

     CONSTRAINT UQ_Result_Enrolment
        UNIQUE (EnrolmentID),

     CONSTRAINT CK_Results_Position
        CHECK (Position IS NULL OR Position > 0)
);
GO

Create Table Routes 
(
      RouteID INT Identity(1,1) Primary Key,
      EventID INT NOT NULL,
      DistanceKM Decimal(6,2) NOT NULL,
      ElevationGainM INT,
      StartLocation VARCHAR(200),
      FinishLocation VARCHAR(200),
      RouteDescription VARCHAR(1000),
      MapURL VARCHAR(500),

      CONSTRAINT FK_Routes_Events
         FOREIGN KEY(EventID)
         REFERENCES Events(EventID),

      CONSTRAINT UQ_Routes_Events
         UNIQUE (EventID),
         
      CONSTRAINT CK_Routes_Distance
         CHECK (DistanceKM > 0)
);
GO

Create Table Weather
(
      WeatherID INT Identity(1,1) Primary Key,
      EventID INT NOT NULL,
      RecordedAt DATETIME NOT NULL DEFAULT GETDATE(),
      TemperatureC Decimal(5,2),
      FeelsLikeC Decimal(5,2),
      Humidity INT,
      WindSpeedKPH Decimal(6,2),
      Conditions VARCHAR(100),

      CONSTRAINT FK_Weather_Events
          FOREIGN KEY (EventID)
          REFERENCES Events(EventID),
      
      CONSTRAINT CK_Weather_Humidity
          CHECK(Humidity IS NULL OR
                Humidity BETWEEN 0 AND 100),
      CONSTRAINT CK_Weather_Wind
          CHECK(WindSpeedKPH IS NULL OR 
                WindSpeedKPH >= 0)
);
GO
INSERT INTO Users
    (FirstName, LastName, Email, PasswordHash, PhoneNumber, Role)
VALUES
('Thabo', 'Mokoena', 'thabo.mokoena@email.com', 'HASH001', '0712345678', 'Participant'),
('Lerato', 'Dlamini', 'lerato.dlamini@email.com', 'HASH002', '0723456789', 'Participant'),
('Sipho', 'Nkosi', 'sipho.nkosi@email.com', 'HASH003', '0734567890', 'Participant'),
('Nomsa', 'Khumalo', 'nomsa.khumalo@email.com', 'HASH004', '0745678901', 'Participant'),
('Anele', 'Ndlovu', 'anele.ndlovu@email.com', 'HASH005', '0756789012', 'Participant'),
('Zanele', 'Mthembu', 'zanele.mthembu@email.com', 'HASH006', '0767890123', 'Participant'),
('Bongani', 'Zulu', 'bongani.zulu@email.com', 'HASH007', '0778901234', 'Participant'),
('Ayanda', 'Pillay', 'ayanda.pillay@email.com', 'HASH008', '0789012345', 'Participant'),
('Jason', 'Naidoo', 'jason.naidoo@email.com', 'HASH009', '0790123456', 'Participant'),
('Megan', 'Jacobs', 'megan.jacobs@email.com', 'HASH010', '0711223344', 'Participant'),
('Sibusiso', 'Mkhize', 'sibusiso@raceday.co.za', 'HASH011', '0722334455', 'Organiser'),
('Candice', 'Williams', 'candice@raceday.co.za', 'HASH012', '0733445566', 'Organiser'),
('Lunga', 'Mabena', 'lunga@raceday.co.za', 'HASH013', '0744556677', 'Organiser');
GO


INSERT INTO Events
    (OrganiserID, EventName, Description, EventType, EventDate,
     StartTime, Location, City, Province, Status)
VALUES
(11,
 'Comrades Road Run',
 'An ultra-distance road running event between Pietermaritzburg and Durban.',
 'Road Running',
 '2027-05-30',
 '05:30:00',
 'Pietermaritzburg',
 'Pietermaritzburg',
 'KwaZulu-Natal',
 'Upcoming'),

(12,
 'Cape Town Cycle Tour',
 'A major cycling event taking participants along the scenic Cape Peninsula.',
 'Cycling',
 '2027-03-14',
 '06:00:00',
 'Grand Parade',
 'Cape Town',
 'Western Cape',
 'Upcoming'),

(13,
 'Soweto Marathon',
 'A road marathon celebrating the spirit and community of Soweto.',
 'Road Running',
 '2027-11-07',
 '06:00:00',
 'FNB Stadium',
 'Soweto',
 'Gauteng',
 'Upcoming'),

(12,
 'Two Oceans Half Marathon',
 'A scenic road running event through the Cape Town area.',
 'Road Running',
 '2027-04-18',
 '06:30:00',
 'University of Cape Town',
 'Cape Town',
 'Western Cape',
 'Upcoming'),

(13,
 'Johannesburg Community Park Run',
 'A community-focused weekend park run open to runners and walkers.',
 'Park Run',
 '2026-10-03',
 '08:00:00',
 'Zoo Lake',
 'Johannesburg',
 'Gauteng',
 'Open'),


(11,
 'Durban Charity Walk',
 'A community walking event raising funds for local charities.',
 'Charity Walk',
 '2026-10-24',
 '07:00:00',
 'Moses Mabhida Stadium',
 'Durban',
 'KwaZulu-Natal',
 'Upcoming'),


(12,
 'Cape Town Charity Cycle',
 'A community cycling event supporting local charities.',
 'Cycling',
 '2026-11-21',
 '07:00:00',
 'Green Point',
 'Cape Town',
 'Western Cape',
 'Upcoming'),


(13,
 'Soweto Community 10K',
 'A community road race designed for runners of different experience levels.',
 'Road Running',
 '2026-09-27',
 '06:30:00',
 'Walter Sisulu Square',
 'Soweto',
 'Gauteng',
 'Open');
GO
INSERT INTO Categories
(CategoryName, Description)
VALUES
('5 KM Run',
 'Five kilometre community running event.'),

('10 KM Run',
 'Ten kilometre road running category.'),

('21 KM Half Marathon',
 'Half marathon road running category.'),

('42 KM Marathon',
 'Full marathon road running category.'),

('Ultra Marathon',
 'Long-distance road running category.'),

('Cycling',
 'Road cycling category.'),

('Community Walk',
 'Walking category for community participants.'),

('Charity Event',
 'Event category supporting a charitable cause.'),

('Junior',
 'Category for younger participants.');
GO
INSERT INTO EventCategories
(EventID, CategoryID, EntryFee, MaxParticipants)
VALUES
(1, 5, 600.00, 25000),
(2, 6, 500.00, 35000),
(3, 4, 400.00, 15000),
(3, 3, 300.00, 10000),
(4, 3, 350.00, 12000),
(5, 1, 0.00, 3000),
(5, 9, 0.00, 500),
(6, 7, 100.00, 5000),
(6, 8, 100.00, 5000),
(7, 6, 250.00, 5000),
(7, 8, 250.00, 5000),
(8, 2, 150.00, 5000),
(8, 8, 150.00, 2000);
GO
INSERT INTO Enrolments
(
    EventID,
    UserID,
    EventCategoryID,
    EnrolmentDate,
    Status
)
VALUES
(1, 1, 1, '2026-08-01 09:15:00', 'Registered'),
(1, 2, 1, '2026-08-02 10:30:00', 'Registered'),
(1, 3, 1, '2026-08-03 11:45:00', 'Registered'),

(2, 4, 2, '2026-08-04 09:00:00', 'Registered'),
(2, 5, 2, '2026-08-05 10:20:00', 'Registered'),
(2, 6, 2, '2026-08-06 14:10:00', 'Registered'),

(3, 7, 3, '2026-08-07 08:30:00', 'Registered'),
(3, 8, 3, '2026-08-08 12:45:00', 'Registered'),

(4, 9, 5, '2026-08-09 09:15:00', 'Registered'),
(4, 10, 5, '2026-08-10 13:25:00', 'Registered'),

(5, 1, 6, '2026-09-01 08:00:00', 'Registered'),
(5, 4, 6, '2026-09-01 08:30:00', 'Registered'),

(6, 2, 7, '2026-09-02 09:20:00', 'Registered'),
(6, 5, 7, '2026-09-02 11:15:00', 'Registered'),

(7, 6, 11, '2026-09-02 14:00:00', 'Registered'),
(7, 9, 11, '2026-09-03 09:40:00', 'Registered'),

(8, 3, 13, '2026-08-20 10:30:00', 'Registered'),
(8, 7, 13, '2026-08-21 12:10:00', 'Registered');
GO

UPDATE Enrolments
SET Status = 'Completed'
WHERE EnrolmentID IN
(
    11,
    12
);
GO

INSERT INTO Results
(
    EnrolmentID,
    FinishTime,
    Position,
    Pace,
    ResultStatus
)
VALUES
(11, '00:24:35', 1, '04:55 min/km', 'Finished'),
(12, '00:29:42', 2, '05:56 min/km', 'Finished');
GO

INSERT INTO Routes
(
    EventID,
    DistanceKM,
    ElevationGainM,
    StartLocation,
    FinishLocation,
    RouteDescription,
    MapURL
)
VALUES

(1,
 89.00,
 1500,
 'Pietermaritzburg',
 'Durban',
 'Ultra marathon route connecting Pietermaritzburg and Durban.',
 'https://example.com/routes/comrades'),
(2,
 109.00,
 1200,
 'Cape Town',
 'Cape Town',
 'Cycling route around the Cape Peninsula with scenic coastal sections.',
 'https://example.com/routes/cape-town-cycle'),
(3,
 42.20,
 500,
 'FNB Stadium',
 'FNB Stadium',
 'Road marathon through Soweto and surrounding Johannesburg areas.',
 'https://example.com/routes/soweto-marathon'),
(4,
 21.10,
 250,
 'University of Cape Town',
 'University of Cape Town',
 'Scenic half marathon route through Cape Town and surrounding suburbs.',
 'https://example.com/routes/two-oceans'),

(5,
 5.00,
 50,
 'Zoo Lake',
 'Zoo Lake',
 'Short community park route around Zoo Lake.',
 'https://example.com/routes/zoo-lake'),
(6,
 8.00,
 30,
 'Moses Mabhida Stadium',
 'Moses Mabhida Stadium',
 'Community walking route along the Durban beachfront.',
 'https://example.com/routes/durban-walk'),

(7,
 40.00,
 300,
 'Green Point',
 'Green Point',
 'Community cycling route around Cape Town.',
 'https://example.com/routes/cape-town-charity-cycle'),
(8,
 10.00,
 100,
 'Walter Sisulu Square',
 'Walter Sisulu Square',
 '10 kilometre community road running route through Soweto.',
 'https://example.com/routes/soweto-10k');
GO

INSERT INTO Weather
(
    EventID,
    RecordedAt,
    TemperatureC,
    FeelsLikeC,
    Humidity,
    WindSpeedKPH,
    Conditions
)
VALUES

(1, '2027-05-30 05:00:00', 13.50, 12.80, 72, 8.50, 'Clear'),
(2, '2027-03-14 05:30:00', 17.20, 16.80, 68, 15.40, 'Partly Cloudy'),
(3, '2027-11-07 05:30:00', 16.80, 16.50, 60, 9.20, 'Clear'),
(4, '2027-04-18 06:00:00', 14.60, 13.90, 75, 12.80, 'Cloudy'),
(5, '2026-10-03 07:30:00', 15.30, 14.90, 55, 6.50, 'Sunny'),
(6, '2026-10-24 06:30:00', 20.40, 21.10, 78, 10.30, 'Partly Cloudy'),
(7, '2026-11-21 06:30:00', 16.70, 16.20, 70, 14.60, 'Clear'),
(8, '2026-09-27 06:00:00', 11.80, 11.20, 58, 5.80, 'Clear');
GO


Select * From Users;
Select * From Events;
Select * From Categories;
Select * From EventCategories;
Select * From Enrolments;
Select * From Results;
Select * From Routes;
Select * From Weather;

     

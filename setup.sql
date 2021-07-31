CREATE DATABASE BookSeek;

USE BookSeek;

CREATE TABLE Book
(
	Isbn CHAR(13) PRIMARY KEY,
	Title VARCHAR(80) NOT NULL,
	Author VARCHAR(500) NOT NULL,
	Publisher VARCHAR(80) NOT NULL,
	DateOfPublication DATE NOT NULL,
	BookQuantityOnHand INT NOT NULL,
	Price DECIMAL(10, 2) NOT NULL,
	BookStatus CHAR(10) NOT NULL CHECK (BookStatus IN('Active', 'Deleted')) DEFAULT 'Active',
);


CREATE TABLE Sale
(
	SaleId INT PRIMARY KEY IDENTITY(1, 1),
	TransactionDateTime DATETIME NOT NULL,
	SaleStatus CHAR(10) NOT NULL CHECK (SaleStatus IN('Standard', 'Exchanged')),
	TotalAmount DECIMAL(10, 2) NOT NULL,
	Cash DECIMAL(10, 2) NOT NULL,
	Change DECIMAL(10, 2) NOT NULL,
	ChargePrepaid DECIMAL(10, 2),
	AdditionalRequired DECIMAL(10, 2)
);

/* Sale (0, n) - Book (1, n) */
CREATE TABLE Sale_Book
(
	SaleId INT,
	Isbn CHAR(13),
	BookQuantitySold INT NOT NULL,
	Price DECIMAL(10, 2) NOT NULL

	CONSTRAINT PK_Sale_Book PRIMARY KEY CLUSTERED (SaleId, Isbn),
	CONSTRAINT FK_Sale_Book_SaleId FOREIGN KEY (SaleId)
		REFERENCES Sale(SaleId)
		ON UPDATE CASCADE
		ON DELETE CASCADE,
	CONSTRAINT FK_Sale_Book_Isbn FOREIGN KEY (Isbn)
		REFERENCES Book(Isbn)
		ON UPDATE CASCADE
		ON DELETE NO ACTION
);

CREATE TABLE Ledger
(
	TrackId INT PRIMARY KEY IDENTITY(1, 1),
	TrackTag CHAR(15) NOT NULL CHECK (TrackTag IN('In', 'Out', 'Deactivated')),
	Isbn CHAR(13),
	Quantity INT NOT NULL,
	EventDateTime DATETIME NOT NULL

	CONSTRAINT FK_Ledger_Isbn FOREIGN KEY (Isbn)
		REFERENCES Book(Isbn)
		ON UPDATE CASCADE
		ON DELETE NO ACTION
);

INSERT INTO
	Book (Isbn, Title, Author, Publisher, DateOfPublication, BookQuantityOnHand, Price)
VALUES
	('9780545310604', 'Mockingjay', 'Suzanne Collins', 'Scholastic Press', '2010-09-01', 3, 350),
	('9780525478812', 'The Fault in Our Stars', 'John Green', 'Dutton Books', '2012-01-01', 25, 400),
	('9780525478188', 'Paper Towns','John Green', 'Dutton Books', '2008-10-16', 78, 250),
	('9780316127264', 'Why We Broke Up', 'Daniel Handler', 'Little, Brown Books for Young Readers', '2013-12-03', 17, 400),
	('9781613834381', 'The Night Circus', 'Erin Morgenstern', 'Perfection Learning', '2012-07-03', 5, 200),
	('9780525555377', 'Turtles All the Way Down', 'John Green', 'Penguin Books', '2019-06-11', 20, 699.99),
	('9781419741883', 'Diary of a Wimpy Kid: Dog Days', 'Jeff Kinney', 'Harry N. Abrams', '2009-10-12', 38, 299.99),
	('9780142410707', 'An Abundance of Katherines', 'John Green', 'Penguin Books', '2008-10-16', 15, 399.99),
	('9780316231657', 'Beautiful Creatures', 'Margaret Stohl', 'Little, Brown Books for Young Readers', '2012-11-20', 12, 500),
	('9780141381473', 'The Lightning Thief', 'Rick Riordan', 'Hyperion', '2005-01-01', 0, 150),
	('9780062351432', 'Welcome to Night Vale', 'Joseph Fink', 'Harper Perennial', '2017-01-10', 25, 799.99),
	('9781663617651', 'Red, White, & Royal Blue', 'Casey McQuiston', 'Turtle Back', '2021-06-01', 100, 934),
	('9780385539241', 'The Handmaid''s Tale', 'Margaret Atwood', 'Nan A. Talese', '2019-03-29', 50, 1199),
	('9780525423270', 'Anna and the French Kiss', 'Stephanie Perkins', 'Penguin Putnam Inc.', '2010-12-02', 70, 598.75),
	('9781501166143', 'The Possible World', 'Liese O''Halloran Schwarz', 'Scribner', '2018-05-06', 10, 400),
	('9781501110368', 'It Ends With Us', 'Colleen Hoover', 'Atria', '2016-08-02', 24, 799),
	('9780062331755', 'Fans of the Impossible Life', 'Kate Scelsa', 'Balzer + Bray', '2015-08-15', 7, 299),
	('9780606408424', 'Amina''s Voice', 'Hena Khan', 'Turtleback Books', '2018-05-29', 15, 250),
	('9780399559372', 'Windfall', 'Jennifer E. Smith', 'Delacorte Press', '2017-05-02', 32, 599),
	('9780735231856', 'Aether Bound', 'E.K. Johnston', 'Dutton Books for Young Readers', '2021-05-25', 89, 1500);

INSERT INTO BOOK
VALUES ('9780998358901', 'Clueless in Cleveland', 'Nelle Lewis', 'Hooky Life', '2017-01-29', 0, 300, 'Deleted');

INSERT INTO 
	Sale (TransactionDateTime, SaleStatus, TotalAmount, Cash, Change)
VALUES 
	('2021-06-01 09:30:00 AM', 'Standard', 799, 800, 1), -- 1
	('2021-06-07 01:45:00 PM', 'Standard', 350, 400, 50), -- 2
	('2021-06-12 08:02:00 AM', 'Standard', 2349.98, 2500, 150), -- 3
	('2021-06-12 09:47:00 AM', 'Standard', 799, 1000, 201), -- 4
	('2021-06-12 02:15:00 PM', 'Standard', 799, 1000, 201), -- 5
	('2021-06-12 04:50:00 PM', 'Standard', 800, 800, 0), -- 6
	('2021-06-13 03:33:00 PM', 'Standard', 699.99, 1000, 300), -- 7
	('2021-06-13 03:45:00 PM', 'Standard', 1598, 1600, 2), -- 8
	('2021-06-14 11:44:00 AM', 'Standard', 61470, 62000, 530), -- 9
	('2021-06-17 10:02:00 AM', 'Standard', 599, 600, 1), -- 10
	('2021-06-23 04:44:00 PM', 'Standard', 598.75, 600, 1.25), -- 11
	('2021-06-24 08:00:00 AM', 'Standard', 350, 350, 0), -- 12
	('2021-06-24 02:30:00 PM', 'Standard', 150, 200, 50), -- 13
	('2021-06-30 01:00:00 PM', 'Standard', 299.99, 300, 0), -- 14
	('2021-07-03 09:00:00 AM', 'Standard', 598, 600, 2), -- 15
	('2021-07-05 09:30:00 AM', 'Standard', 400, 500, 100), -- 16
	('2021-07-05 02:00:00 PM', 'Standard', 150, 150, 0), -- 17
	('2021-07-06 01:00:00 PM', 'Standard', 2796, 3000, 204), -- 18
	('2021-07-16 09:22:00 AM', 'Standard', 1050, 1100, 50); -- 19

INSERT INTO
	Sale_Book
VALUES
	(1, '9781501110368', 1, 799),
	(2, '9780545310604', 1, 350),
	(3, '9780525478188', 3, 250),
	(3, '9780525478812', 2, 400),
	(3, '9780142410707', 2, 399.99),
	(4, '9781501110368', 1, 799),
	(5, '9781501110368', 1, 799),
	(6, '9780316127264', 2, 400),
	(7, '9780525555377', 1, 699.99),
	(8, '9781501110368', 2, 799),
	(9, '9780735231856', 25, 1500),
	(9, '9781501110368', 30, 799),
	(10, '9780399559372', 1, 599),
	(11, '9780385539241', 1, 598.75),
	(12, '9780545310604', 1, 350),
	(13, '9780141381473', 1, 150),
	(14, '9781419741883', 1, 299.99),
	(15, '9780062331755', 2, 299),
	(16, '9780525478812', 1, 400),
	(17, '9780141381473', 1, 150),
	(18, '9781501110368', 2, 799),
	(18, '9780399559372', 2, 599),
	(19, '9780525478812', 2, 400),
	(19, '9780525478188', 1, 250);

INSERT INTO
	Ledger (TrackTag, Isbn, Quantity, EventDateTime)
VALUES
	('In', '9780545310604', 5, '2021-05-01 09:01:00 AM'), -- Mockingjay
	('In', '9780525478812', 30, '2021-05-01 09:02:00 AM'), -- The Fault in Our Stars
	('In', '9780525478188', 82, '2021-05-01 09:03:00 AM'), -- Paper Towns
	('In', '9780316127264', 19, '2021-05-01 09:04:00 AM'), -- Why We Broke Up
	('In', '9781613834381', 5, '2021-05-01 09:05:00 AM'), -- The Night Circus
	('In', '9780525555377', 21, '2021-05-01 09:06:00 AM'), -- Turtles All the Way Down
	('In', '9781419741883', 39, '2021-05-01 09:07:00 AM'), -- Diary of a Wimpy Kid: Dog Days
	('In', '9780142410707', 17, '2021-05-01 09:08:00 AM'), -- An Abundance of Katherines
	('In', '9780316231657', 12, '2021-05-01 09:09:00 AM'), -- Beautiful Creatures
	('In', '9780141381473', 2, '2021-05-01 09:10:00 AM'), -- The Lightning Thief
	('In', '9780062351432', 25, '2021-05-01 09:11:00 AM'), -- Welcome to Night Vale
	('In', '9781663617651', 100, '2021-05-01 09:12:00 AM'), -- Red, White, & Royal Blue
	('In', '9780385539241', 51, '2021-05-01 09:13:00 AM'), -- The Handmaid''s Tale
	('In', '9780525423270', 70, '2021-05-01 09:14:00 AM'), -- Anna and the French Kiss
	('In', '9781501166143', 10, '2021-05-01 09:15:00 AM'), -- The Possible World
	('In', '9781501110368', 61, '2021-05-01 09:16:00 AM'), -- It Ends With Us
	('In', '9780062331755', 9, '2021-05-01 09:17:00 AM'), -- Fans of the Impossible Life
	('In', '9780606408424', 15, '2021-05-01 09:18:00 AM'), -- Amina''s Voice
	('In', '9780399559372', 35, '2021-05-01 09:19:00 AM'), -- Windfall
	('In', '9780735231856', 114, '2021-05-01 09:20:00 AM'); -- Aether Bound


/*
Query to Get the All Time Best Seller
	SELECT TOP(1) Isbn, SUM(BookQuantitySold) AS TotalQuantity
	FROM Sale_Book
	GROUP BY Isbn
	ORDER BY SUM(BookQuantitySold) DESC;
*/
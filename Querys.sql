-- User
--User Login
ALTER PROCEDURE [dbo].[PR_Users_Login]
    @UserName VARCHAR(50),
    @Password VARCHAR(50)
AS
BEGIN
    SELECT 
        [dbo].[Users].[UserID], 
		[dbo].[Users].[FirstName],
		[dbo].[Users].[LastName],
        [dbo].[Users].[UserName], 
        [dbo].[Users].[Email], 
        [dbo].[Users].[Password],
        [dbo].[Users].[ProfilePictureURL]
    FROM 
        [dbo].[Users] 
    WHERE 
        [dbo].[Users].[UserName] = @UserName 
        AND [dbo].[Users].[Password] = @Password;
END
--User Signup
CREATE PROCEDURE [dbo].[PR_Users_Signup]
	@FirstName NVARCHAR(100),
	@LastName NVARCHAR(100),
    @UserName NVARCHAR(50),
    @Email NVARCHAR(50),
    @Password NVARCHAR(50),
    @ProfilePictureURL NVARCHAR(MAX) = ''
AS
BEGIN
    INSERT INTO [dbo].[Users]
    (
		[FirstName],
        [LastName],
        [UserName],
		[Email],
        [Password],
		[ProfilePictureURL]
    )
    VALUES
    (
		@FirstName,
		@LastName,
        @UserName,
		@Email,
        @Password,
		@ProfilePictureURL
    );
END
--Add User
Alter PROCEDURE PR_Users_Add
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
	@UserName NVARCHAR(50),
    @Email NVARCHAR(50),
    @Password NVARCHAR(50),
    @ProfilePictureURL NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO Users (FirstName, LastName,UserName, Email, Password, ProfilePictureURL)
    VALUES (@FirstName, @LastName, @UserName, @Email, @Password, @ProfilePictureURL);
END;

--Get all
Alter PROCEDURE PR_Users_GetAll
AS
BEGIN
    SELECT UserID,FirstName,LastName,UserName,Email,Password,ProfilePictureURL 
    FROM Users 
END;

--Get by ID
Alter PROCEDURE PR_Users_GetByID
    @UserID INT
AS
BEGIN
    SELECT UserID,FirstName,LastName,UserName,Email,Password,ProfilePictureURL 
    FROM Users 
    WHERE UserID = @UserID;
END;

--Update by ID
Alter PROCEDURE PR_Users_UpdateByID
    @UserID INT,
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
	@UserName NVARCHAR(50),
    @Email NVARCHAR(50),
    @Password NVARCHAR(50),
    @ProfilePictureURL NVARCHAR(MAX)
AS
BEGIN
    UPDATE Users
    SET 
        FirstName = @FirstName,
        LastName = @LastName,
		UserName = @UserName,
        Email = @Email,
        Password = @Password,
        ProfilePictureURL = @ProfilePictureURL
    WHERE UserID = @UserID;
END;

--Delete user
CREATE PROCEDURE PR_Users_Delete
    @UserID INT
AS
BEGIN
    DELETE FROM Users
    WHERE UserID = @UserID;
END;

--Login user
CREATE PROCEDURE PR_Users_Login
    @Email NVARCHAR(50),
    @Password NVARCHAR(50)
AS
BEGIN
    SELECT UserID, FirstName, LastName, ProfilePictureURL, CreatedAt
    FROM Users
    WHERE Email = @Email AND Password = @Password;
END;

--Properties
--Add property
CREATE PROCEDURE PR_Properties_Add
    @HostID INT,
    @Title NVARCHAR(250),
    @Description NVARCHAR(MAX),
    @Address NVARCHAR(250),
    @City NVARCHAR(100),
    @State NVARCHAR(100),
    @Country NVARCHAR(100),
    @PricePerNight DECIMAL(10,2),
    @MaxGuests INT,
    @Bedrooms INT
AS
BEGIN
    INSERT INTO Properties (HostID, Title, Description, Address, City, State, Country, PricePerNight, MaxGuests, Bedrooms)
    VALUES (@HostID, @Title, @Description, @Address, @City, @State, @Country, @PricePerNight, @MaxGuests, @Bedrooms);
END;

--Get all
CREATE PROCEDURE PR_Properties_GetAll
AS
BEGIN
    SELECT PropertyID,HostID,Title,Description,Address,City,State,Country,PricePerNight,MaxGuests,Bedrooms
    FROM Properties
END;

--Get by ID
CREATE PROCEDURE PR_Properties_GetByID
    @PropertyID INT
AS
BEGIN
    SELECT PropertyID,HostID,Title,Description,Address,City,State,Country,PricePerNight,MaxGuests,Bedrooms
    FROM Properties
	WHERE PropertyID = @PropertyID
END;

--Update by ID
CREATE PROCEDURE PR_Properties_Update
    @PropertyID INT,
	@HostID Int,
    @Title NVARCHAR(250),
    @Description NVARCHAR(MAX),
    @Address NVARCHAR(250),
    @City NVARCHAR(100),
    @State NVARCHAR(100),
    @Country NVARCHAR(100),
    @PricePerNight DECIMAL(10,2),
    @MaxGuests INT,
    @Bedrooms INT
AS
BEGIN
    UPDATE Properties
    SET 
		HostId = @HostID,
        Title = @Title,
        Description = @Description,
        Address = @Address,
        City = @City,
        State = @State,
        Country = @Country,
        PricePerNight = @PricePerNight,
        MaxGuests = @MaxGuests,
        Bedrooms = @Bedrooms,
        ModifiedAt = GETDATE()
    WHERE PropertyID = @PropertyID;
END;

-- Delete property
CREATE PROCEDURE PR_Properties_Delete
    @PropertyID INT
AS
BEGIN
    DELETE FROM Properties
    WHERE PropertyID = @PropertyID;
END;

--Get Properties By Host
CREATE PROCEDURE PR_Properties_GetPropertiesByHost
    @HostID INT
AS
BEGIN
    SELECT PropertyID,HostID,Title,Description,Address,City,State,Country,PricePerNight,MaxGuests,Bedrooms
    FROM Properties
    WHERE HostID = @HostID;
END;

--Search Properties (Filter by City, Price Range, and Guests)
CREATE PROCEDURE PR_Properties_SearchByCity_Price_Guest
    @City NVARCHAR(100),
    @MinPrice DECIMAL(10,2),
    @MaxPrice DECIMAL(10,2),
    @Guests INT
AS
BEGIN
    SELECT PropertyID,HostID,Title,Description,Address,City,State,Country,PricePerNight,MaxGuests,Bedrooms 
    FROM Properties
    WHERE City = @City 
      AND PricePerNight BETWEEN @MinPrice AND @MaxPrice
      AND MaxGuests >= @Guests;
END;

--Bookings
--Add booking
CREATE PROCEDURE PR_Bookings_Add
    @UserID INT,
    @PropertyID INT,
    @CheckInDate DATETIME,
    @CheckOutDate DATETIME,
    @Guests INT,
    @TotalPrice DECIMAL(10,2)
AS
BEGIN
    INSERT INTO Bookings (UserID, PropertyID, CheckInDate, CheckOutDate, Guests, TotalPrice)
    VALUES (@UserID, @PropertyID, @CheckInDate, @CheckOutDate, @Guests, @TotalPrice);
END;

--Get by ID
CREATE PROCEDURE PR_Bookings_GetByID
    @BookingID INT
AS
BEGIN
    SELECT Bookings.BookingID,Users.UserID,Users.FirstName,Properties.PropertyID,Properties.Title,Bookings.CheckInDate,Bookings.CheckOutDate,Bookings.Guests,Bookings.TotalPrice
    FROM Bookings 
	INNER JOIN Users
	on Bookings.UserID = Users.UserID
	INNER JOIN Properties
	on Bookings.PropertyID = Properties.PropertyID
    WHERE BookingID = @BookingID;
END;

--Get all
CREATE PROCEDURE PR_Bookings_GetAll
AS
BEGIN
    SELECT Bookings.BookingID,Users.UserID,Users.FirstName,Properties.PropertyID,Properties.Title,Bookings.CheckInDate,Bookings.CheckOutDate,Bookings.Guests,Bookings.TotalPrice
    FROM Bookings 
	INNER JOIN Users
	on Bookings.UserID = Users.UserID
	INNER JOIN Properties
	on Bookings.PropertyID = Properties.PropertyID
END;

--Get Bookings By User
CREATE PROCEDURE PR_Bookings_GetBookingsByUser
    @UserID INT
AS
BEGIN
    SELECT Bookings.BookingID,Users.UserID,Users.FirstName,Properties.PropertyID,Properties.Title,Bookings.CheckInDate,Bookings.CheckOutDate,Bookings.Guests,Bookings.TotalPrice
    FROM Bookings 
	INNER JOIN Users
	on Bookings.UserID = Users.UserID
	INNER JOIN Properties
	on Bookings.PropertyID = Properties.PropertyID
    WHERE Bookings.UserID = @UserID;
END;

--Update bookings
CREATE PROCEDURE PR_Bookings_Update
    @BookingID INT,
    @CheckInDate DATETIME,
    @CheckOutDate DATETIME,
    @Guests INT,
    @TotalPrice DECIMAL(10,2)
AS
BEGIN
    UPDATE Bookings
    SET 
        CheckInDate = @CheckInDate,
        CheckOutDate = @CheckOutDate,
        Guests = @Guests,
        TotalPrice = @TotalPrice
    WHERE BookingID = @BookingID;
END;

--Delete bookings
CREATE PROCEDURE PR_Bookings_Delete
    @BookingID INT
AS
BEGIN
    DELETE FROM Bookings
    WHERE BookingID = @BookingID;
END;

--Review
--Add review
CREATE PROCEDURE PR_Reviews_Add
    @UserID INT,
    @PropertyID INT,
    @Rating INT,
    @Comment NVARCHAR(MAX) = NULL
AS
BEGIN
    INSERT INTO Reviews (UserID, PropertyID, Rating, Comment)
    VALUES (@UserID, @PropertyID, @Rating, @Comment);
END;

--Get by ID
CREATE PROCEDURE PR_Reviews_GetByID
    @ReviewID INT
AS
BEGIN
    SELECT Reviews.ReviewID,Users.UserID,Users.FirstName,Properties.PropertyID,Reviews.Rating,Reviews.Comment
    FROM Reviews 
	INNER JOIN Users
	on Reviews.UserID = Users.UserID
	INNER JOIN Properties
	on Reviews.PropertyID = Properties.PropertyID
    WHERE ReviewID = @ReviewID;
END;

--Get all
CREATE PROCEDURE PR_Reviews_GetAll
AS
BEGIN
    SELECT Reviews.ReviewID,Users.UserID,Users.FirstName,Properties.PropertyID,Reviews.Rating,Reviews.Comment
    FROM Reviews 
	INNER JOIN Users
	on Reviews.UserID = Users.UserID
	INNER JOIN Properties
	on Reviews.PropertyID = Properties.PropertyID
END;

--Get Reviews By Property
CREATE PROCEDURE PR_Reviews_GetReviewsByProperty
    @PropertyID INT
AS
BEGIN
    SELECT Reviews.ReviewID,Users.UserID,Users.FirstName,Properties.PropertyID,Reviews.Rating,Reviews.Comment
    FROM Reviews 
	INNER JOIN Users
	on Reviews.UserID = Users.UserID
	INNER JOIN Properties
	on Reviews.PropertyID = Properties.PropertyID
    WHERE Reviews.PropertyID = @PropertyID;
END;

--Update review
CREATE PROCEDURE PR_Reviews_Update
    @ReviewID INT,
    @Rating INT,
    @Comment NVARCHAR(MAX)
AS
BEGIN
    UPDATE Reviews
    SET 
        Rating = @Rating,
        Comment = @Comment
    WHERE ReviewID = @ReviewID;
END;

--Delete review
CREATE PROCEDURE PR_Reviews_Delete
    @ReviewID INT
AS
BEGIN
    DELETE FROM Reviews
    WHERE ReviewID = @ReviewID;
END;

--Amenity
--Add amenity
CREATE PROCEDURE PR_Amenities_Add
    @Name NVARCHAR(100)
AS
BEGIN
    INSERT INTO Amenities (Name)
    VALUES (@Name);
END;

--Get all
CREATE PROCEDURE PR_Amenities_GetAll
AS
BEGIN
    SELECT AmenityID,Name FROM Amenities;
END;

--Get by ID
CREATE PROCEDURE PR_Amenities_GetByID
	@AmenityID int
AS
BEGIN
    SELECT AmenityID,Name FROM Amenities
	where AmenityID = @AmenityID;
END;

--Update amenity
CREATE PROCEDURE PR_Amenities_Update
    @AmenityID INT,
    @Name NVARCHAR(100)
AS
BEGIN
    UPDATE Amenities
    SET Name = @Name
    WHERE AmenityID = @AmenityID;
END;

--Delete amenity
CREATE PROCEDURE PR_Amenities_Delete
    @AmenityID INT
AS
BEGIN
    DELETE FROM Amenities
    WHERE AmenityID = @AmenityID;
END;

--PropertyAmenities
--Get PropertyAmenity
CREATE PROCEDURE PR_PropertyAmenities_Get
AS
BEGIN
    SELECT PA.PropertyAmenityID,A.Name,P.PropertyID,P.Title
    FROM PropertyAmenities PA
    INNER JOIN Amenities A ON PA.AmenityID = A.AmenityID
	INNER JOIN Properties P ON PA.PropertyID = P.PropertyID;
END;
--Add PropertyAmenity
CREATE PROCEDURE PR_PropertyAmenities_Add
    @PropertyID INT,
    @AmenityID INT
AS
BEGIN
    INSERT INTO PropertyAmenities (PropertyID, AmenityID)
    VALUES (@PropertyID, @AmenityID);
END;

--GetAmenitiesByProperty
CREATE PROCEDURE PR_PropertyAmenities_GetAmenitiesByProperty
    @PropertyID INT
AS
BEGIN
    SELECT A.Name
    FROM PropertyAmenities PA
    INNER JOIN Amenities A ON PA.AmenityID = A.AmenityID
    WHERE PA.PropertyID = @PropertyID;
END;

--Update PropertyAmenities
CREATE PROCEDURE PR_PropertyAmenities_Update
    @PropertyAmenityID INT,
    @PropertyID INT,
    @AmenityID INT
AS
BEGIN
    UPDATE PropertyAmenities
    SET PropertyID = @PropertyID,
		AmenityID = @AmenityID
    WHERE PropertyAmenityID = @PropertyAmenityID;
END;

--Delete PropertyAmenity
CREATE PROCEDURE PR_PropertyAmenities_Delete
    @PropertyAmenityID INT
AS
BEGIN
    DELETE FROM PropertyAmenities
    WHERE PropertyAmenityID = @PropertyAmenityID;
END;

--Image
--Add image
CREATE PROCEDURE PR_Images_add
    @PropertyID INT,
    @ImageURL NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO Images (PropertyID, ImageURL)
    VALUES (@PropertyID, @ImageURL);
END;

--Get image by property
ALTER PROCEDURE PR_Images_GetImagesByProperty
    @PropertyID INT
AS
BEGIN
    SELECT ImageID,ImageURL,PropertyID
    FROM Images
    WHERE PropertyID = @PropertyID;
END;

--Update image
CREATE PROCEDURE PR_Images_Update
	@ImageID INT,
    @PropertyID INT,
	@ImageURL NVARCHAR(MAX)
AS
BEGIN
    UPDATE Images
	SET PropertyID = @PropertyID,
		ImageURL = @ImageURL
    WHERE ImageID = @ImageID;
END;
--Delete image
CREATE PROCEDURE PR_Images_Delete
    @ImageID INT
AS
BEGIN
    DELETE FROM Images
    WHERE ImageID = @ImageID;
END;






--Dashboard 
CREATE PROCEDURE usp_GetDashboardData
AS
BEGIN
    SET NOCOUNT ON;

    -- Temporary tables
    CREATE TABLE #Counts (
        Metric NVARCHAR(255),
        Value INT
    );

    CREATE TABLE #RecentBookings (
        BookingID INT,
        UserName NVARCHAR(255),
        PropertyTitle NVARCHAR(250),
        CheckInDate DATETIME,
        CheckOutDate DATETIME,
        TotalPrice DECIMAL(10,2)
    );

    CREATE TABLE #TopRatedProperties (
        PropertyID INT,
        Title NVARCHAR(250),
        AvgRating DECIMAL(3,2),
        City NVARCHAR(100),
        Country NVARCHAR(100)
    );

    CREATE TABLE #TopHosts (
        HostID INT,
        HostName NVARCHAR(255),
        TotalProperties INT
    );

    -- Populate #Counts
    INSERT INTO #Counts
    SELECT 'TotalUsers', COUNT(*) FROM Users;
    
    INSERT INTO #Counts
    SELECT 'TotalProperties', COUNT(*) FROM Properties;
    
    INSERT INTO #Counts
    SELECT 'TotalBookings', COUNT(*) FROM Bookings;
    
    INSERT INTO #Counts
    SELECT 'TotalReviews', COUNT(*) FROM Reviews;

    -- Populate #RecentBookings
    INSERT INTO #RecentBookings
    SELECT TOP 10
        B.BookingID,
        U.UserName,
        P.Title AS PropertyTitle,
        B.CheckInDate,
        B.CheckOutDate,
        B.TotalPrice
    FROM Bookings B
    INNER JOIN Users U ON B.UserID = U.UserID
    INNER JOIN Properties P ON B.PropertyID = P.PropertyID
    ORDER BY B.CreatedAt DESC;

    -- Populate #TopRatedProperties
    INSERT INTO #TopRatedProperties
    SELECT TOP 10
        P.PropertyID,
        P.Title,
        AVG(R.Rating) AS AvgRating,
        P.City,
        P.Country
    FROM Reviews R
    INNER JOIN Properties P ON R.PropertyID = P.PropertyID
    GROUP BY P.PropertyID, P.Title, P.City, P.Country
    ORDER BY AVG(R.Rating) DESC;

    -- Populate #TopHosts
    INSERT INTO #TopHosts
    SELECT TOP 10
        H.UserID AS HostID,
        CONCAT(H.FirstName, ' ', H.LastName) AS HostName,
        COUNT(P.PropertyID) AS TotalProperties
    FROM Users H
    INNER JOIN Properties P ON H.UserID = P.HostID
    GROUP BY H.UserID, H.FirstName, H.LastName
    ORDER BY COUNT(P.PropertyID) DESC;

    -- Output results
    SELECT * FROM #Counts;
    SELECT * FROM #RecentBookings;
    SELECT * FROM #TopRatedProperties;
    SELECT * FROM #TopHosts;

    -- Cleanup
    DROP TABLE #Counts;
    DROP TABLE #RecentBookings;
    DROP TABLE #TopRatedProperties;
    DROP TABLE #TopHosts;
END;


-- User

--User dropdown
CREATE PROCEDURE [dbo].[PR_Users_DropDown]
AS
BEGIN
    SELECT
		[dbo].[Users].[UserID],
        [dbo].[Users].[UserName]
    FROM
        [dbo].[Users]
	ORDER BY [dbo].[Users].[UserName]
END
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
        [dbo].[Users].[ProfilePictureURL],
		[dbo].[Roles].[RoleID],
		[dbo].[Roles].[RoleName]
    FROM 
        [dbo].[Users] 
	inner join [dbo].[Roles]
	on [dbo].[Users].[RoleID] = [dbo].[Roles].[RoleID]
    WHERE 
        [dbo].[Users].[UserName] = @UserName 
        AND [dbo].[Users].[Password] = @Password;
END
--User Signup
ALTER PROCEDURE [dbo].[PR_Users_Signup]
	@FirstName NVARCHAR(100),
	@LastName NVARCHAR(100),
    @UserName NVARCHAR(50),
    @Email NVARCHAR(50),
    @Password NVARCHAR(50),
    @ProfilePictureURL NVARCHAR(MAX) = '',
	@RoleID int
AS
BEGIN
    INSERT INTO [dbo].[Users]
    (
		[FirstName],
        [LastName],
        [UserName],
		[Email],
        [Password],
		[ProfilePictureURL],
		[RoleID]
    )
    VALUES
    (
		@FirstName,
		@LastName,
        @UserName,
		@Email,
        @Password,
		@ProfilePictureURL,
		@RoleID
    );
END
--Add User
Alter PROCEDURE PR_Users_Add
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
	@UserName NVARCHAR(50),
    @Email NVARCHAR(50),
    @Password NVARCHAR(50),
    @ProfilePictureURL NVARCHAR(MAX),
	@RoleID int
AS
BEGIN
    INSERT INTO Users (FirstName, LastName,UserName, Email, Password, ProfilePictureURL,RoleID)
    VALUES (@FirstName, @LastName, @UserName, @Email, @Password, @ProfilePictureURL,@RoleID);
END;

--Get all
Alter PROCEDURE PR_Users_GetAll
AS
BEGIN
    SELECT UserID,FirstName,LastName,UserName,Email,Password,ProfilePictureURL ,RoleID
    FROM Users 
END;

--Get by ID
Alter PROCEDURE PR_Users_GetByID
    @UserID INT
AS
BEGIN
    SELECT UserID,FirstName,LastName,UserName,Email,Password,ProfilePictureURL ,RoleID
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
    @ProfilePictureURL NVARCHAR(MAX),
	@RoleID int
AS
BEGIN
    UPDATE Users
    SET 
        FirstName = @FirstName,
        LastName = @LastName,
		UserName = @UserName,
        Email = @Email,
        Password = @Password,
        ProfilePictureURL = @ProfilePictureURL,
		RoleID = @RoleID
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

--Properties
--Properties dropdown
CREATE PROCEDURE [dbo].[PR_Properties_DropDown]
AS
BEGIN
    SELECT
		[dbo].[Properties].[PropertyID],
        [dbo].[Properties].[Title]
    FROM
        [dbo].[Properties]
	ORDER BY [dbo].[Properties].[Title]
END
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
ALTER PROCEDURE PR_Properties_GetAll
AS
BEGIN
    SELECT PropertyID,Properties.HostID,Users.UserName,Title,Description,Address,City,State,Country,PricePerNight,MaxGuests,Bedrooms
    FROM Properties
	INNER JOIN Users
	ON Properties.HostID = Users.UserID
END;

--Get by ID
ALTER PROCEDURE PR_Properties_GetByID
    @PropertyID INT
AS
BEGIN
    SELECT PropertyID,Properties.HostID,Users.UserName,Title,Description,Address,City,State,Country,PricePerNight,MaxGuests,Bedrooms
    FROM Properties
	INNER JOIN Users
	ON Properties.HostID = Users.UserID
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
ALTER PROCEDURE PR_Bookings_GetByID
    @BookingID INT
AS
BEGIN
    SELECT Bookings.BookingID,Users.UserID,Users.UserName,Properties.PropertyID,Properties.Title,Bookings.CheckInDate,Bookings.CheckOutDate,Bookings.Guests,Bookings.TotalPrice
    FROM Bookings 
	INNER JOIN Users
	on Bookings.UserID = Users.UserID
	INNER JOIN Properties
	on Bookings.PropertyID = Properties.PropertyID
    WHERE BookingID = @BookingID;
END;

--Get all
ALTER PROCEDURE PR_Bookings_GetAll
AS
BEGIN
    SELECT Bookings.BookingID,Users.UserID,Users.UserName,Properties.PropertyID,Properties.Title,Bookings.CheckInDate,Bookings.CheckOutDate,Bookings.Guests,Bookings.TotalPrice
    FROM Bookings 
	INNER JOIN Users
	on Bookings.UserID = Users.UserID
	INNER JOIN Properties
	on Bookings.PropertyID = Properties.PropertyID
END;

--Get Bookings By User
ALTER PROCEDURE PR_Bookings_GetBookingsByUser
    @UserID INT
AS
BEGIN
    SELECT Bookings.BookingID,Users.UserID,Users.UserName,Properties.PropertyID,Properties.Title,Bookings.CheckInDate,Bookings.CheckOutDate,Bookings.Guests,Bookings.TotalPrice
    FROM Bookings 
	INNER JOIN Users
	on Bookings.UserID = Users.UserID
	INNER JOIN Properties
	on Bookings.PropertyID = Properties.PropertyID
    WHERE Bookings.UserID = @UserID;
END;

--Update bookings
ALTER PROCEDURE PR_Bookings_Update
    @BookingID INT,
	@UserID INT,
	@PropertyID INT,
    @CheckInDate DATETIME,
    @CheckOutDate DATETIME,
    @Guests INT,
    @TotalPrice DECIMAL(10,2)
AS
BEGIN
    UPDATE Bookings
    SET 
		UserID = @UserID,
		PropertyID = @PropertyID,
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
ALTER PROCEDURE PR_Reviews_GetByID
    @ReviewID INT
AS
BEGIN
    SELECT Reviews.ReviewID,Users.UserID,Users.UserName,Properties.PropertyID,Properties.Title,Reviews.Rating,Reviews.Comment
    FROM Reviews 
	INNER JOIN Users
	on Reviews.UserID = Users.UserID
	INNER JOIN Properties
	on Reviews.PropertyID = Properties.PropertyID
    WHERE ReviewID = @ReviewID;
END;

--Get all
ALTER PROCEDURE PR_Reviews_GetAll
AS
BEGIN
    SELECT Reviews.ReviewID,Users.UserID,Users.UserName,Properties.PropertyID,Properties.Title,Reviews.Rating,Reviews.Comment
    FROM Reviews
	INNER JOIN Users
	on Reviews.UserID = Users.UserID
	INNER JOIN Properties
	on Reviews.PropertyID = Properties.PropertyID
END;

--Get Reviews By Property
ALTER PROCEDURE PR_Reviews_GetReviewsByProperty
    @PropertyID INT
AS
BEGIN
    SELECT Reviews.ReviewID,Users.UserID,Users.UserName,Properties.PropertyID,Properties.Title,Reviews.Rating,Reviews.Comment
    FROM Reviews 
	INNER JOIN Users
	on Reviews.UserID = Users.UserID
	INNER JOIN Properties
	on Reviews.PropertyID = Properties.PropertyID
    WHERE Reviews.PropertyID = @PropertyID;
END;

--Update review
ALTER PROCEDURE PR_Reviews_Update
    @ReviewID INT,
	@UserID INT,
	@PropertyID INT,
    @Rating INT,
    @Comment NVARCHAR(MAX)
AS
BEGIN
    UPDATE Reviews
    SET 
		UserID = @UserID,
		PropertyID = @PropertyID,
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

--Amenity Dropdown
CREATE PROCEDURE [dbo].[PR_Amenities_DropDown]
AS
BEGIN
    SELECT
		[dbo].[Amenities].[AmenityID],
        [dbo].[Amenities].[Name]
    FROM
        [dbo].[Amenities]
	ORDER BY [dbo].[Amenities].[Name]
END
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
CREATE PROCEDURE PR_PropertyAmenities_GetAll
AS
BEGIN
    SELECT PA.PropertyAmenityID,A.AmenityID,A.Name,P.PropertyID,P.Title
    FROM PropertyAmenities PA
    INNER JOIN Amenities A ON PA.AmenityID = A.AmenityID
	INNER JOIN Properties P ON PA.PropertyID = P.PropertyID;
END;
--Get by PK
ALTER PROCEDURE PR_PropertyAmenities_GetByID
@PropertyAmenityID INT
AS
BEGIN
    SELECT PA.PropertyAmenityID,A.AmenityID,A.Name,P.PropertyID,P.Title
    FROM PropertyAmenities PA
    INNER JOIN Amenities A ON PA.AmenityID = A.AmenityID
	INNER JOIN Properties P ON PA.PropertyID = P.PropertyID
	where PA.PropertyAmenityID=@PropertyAmenityID ;
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
ALTER PROCEDURE PR_PropertyAmenities_GetAmenitiesByProperty
    @PropertyID INT
AS
BEGIN
    SELECT PA.PropertyAmenityID,A.AmenityID,A.Name,P.PropertyID,P.Title
	FROM PropertyAmenities PA
    INNER JOIN Amenities A ON PA.AmenityID = A.AmenityID
	INNER JOIN Properties P ON PA.PropertyID = P.PropertyID
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
--Get all
ALTER PROCEDURE PR_Images_Get
AS
BEGIN
    SELECT ImageID,ImageURL,Properties.PropertyID,Properties.Title
    FROM Images
	inner join Properties
	on Images.PropertyID = Properties.PropertyID
END;
--Get image by id
ALTER PROCEDURE PR_Images_GetByID
    @ImageID INT
AS
BEGIN
    SELECT ImageID,ImageURL,Properties.PropertyID,Properties.Title
    FROM Images
	inner join Properties
	on Images.PropertyID = Properties.PropertyID
END;
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
    SELECT ImageID,ImageURL,Properties.PropertyID,Properties.Title
    FROM Images
	inner join Properties
	on Images.PropertyID = Properties.PropertyID
    WHERE Images.PropertyID = @PropertyID;
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

----Role 
--Get all roles
CREATE PROCEDURE PR_Roles_Get
    
AS
BEGIN
    SELECT RoleID,RoleName
    FROM Roles
	order by RoleName
END;
--Add role
CREATE PROCEDURE PR_Roles_Add
    @RoleName NVARCHAR(50)
AS
BEGIN
    INSERT INTO Roles (RoleName)
    VALUES (@RoleName);
END;

--Update role
CREATE PROCEDURE PR_Roles_Update
	@RoleID int,
    @RoleName NVARCHAR(50)
AS
BEGIN
    Update Roles
	set RoleName = @RoleName
	where RoleID = @RoleID
END;

--Delete role
CREATE PROCEDURE PR_Roles_Delete
	@RoleID int
AS
BEGIN
    Delete from Roles
	where RoleID = @RoleID
END;
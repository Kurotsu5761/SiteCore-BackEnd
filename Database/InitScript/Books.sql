Create Table Category (
	Id int IDENTITY (1,1) Not Null,
	Constraint PK_CategoryId PRIMARY KEY CLUSTERED (Id),
	Name nvarchar(500)
)

CREATE TABLE Books (
	Id int IDENTITY(1,1) Not NULL,
	CONSTRAINT PK_BooksId PRIMARY KEY CLUSTERED (Id),
	Title nvarchar(50),
	Subtitle nvarchar(50),
	ImageUrl nvarchar(500),
	BookStatus int,
	CategoryId int,
	CONSTRAINT FK_Books_CategoryId Foreign Key (CategoryId)
	References Category (Id)
);

Create Table Author (
	Id int IDENTITY(1,1) Not NULL,
	CONSTRAINT PK_AuthorId PRIMARY KEY CLUSTERED (Id),
	Name nvarchar(50),
	ImageUrl nvarchar(500)
);

Create Table Publishes (
	BookId int,
	Constraint FK_Publishes_BookId Foreign Key (BookId)
	References Books (Id),
	AuthorId int,
	Constraint FK_Publishes_AuthorId Foreign Key (AuthorId)
	References Author (Id)
)

Create Table [User]	 (
	UserId int IDENTITY (1,1) Not Null,
	Constraint PK_UserId Primary Key Clustered (UserId),
	EmailAddress nvarchar(100),
	IsAdmin bit
)

Create Table LibraryTransaction (
	UserId int,
	Constraint FK_Transaction_UserId Foreign Key (UserId)
	References [User] (UserId),
	BookId int,
	Constraint FK_Transaction_BookId Foreign Key (BookId)
	References Books (id),
	TimeOccured DateTime
)
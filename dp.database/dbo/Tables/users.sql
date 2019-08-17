CREATE TABLE [dbo].[users]
(
	[UserId]     INT            IDENTITY (1, 1) NOT NULL,
	[Password] VARBINARY(50) NULL, 
	[Email]      NVARCHAR (350) NULL,
    [Created]    DATETIME       DEFAULT (GETDATE()) NOT NULL,
    [Updated]    DATETIME       NULL,
    [UserTypeId]   INT            DEFAULT ((1)) NOT NULL,
    [IsActive]   BIT            DEFAULT ((1)) NOT NULL,
	CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
)

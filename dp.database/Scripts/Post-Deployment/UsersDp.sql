/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
--add the initial user
	IF (Select count(*) from users where email='dan@dan.com' and usertypeid=2 ) = 0
	BEGIN
	insert into users (password, email, UserTypeId)
	values (HashBytes('SHA2_256', N'password'),'dan@dan.com',2);

	END

DROP FUNCTION dbo.SpellChecker
GO
DROP FUNCTION dbo.SpellCheckerRefresh
GO

DROP ASSEMBLY SqlSpellChecker
GO
CREATE ASSEMBLY SqlSpellChecker FROM 'E:\SqlSpellChecker\bin\Release\SqlSpellChecker.dll'
--WITH PERMISSION_SET=UNSAFE;
GO

CREATE FUNCTION SpellChecker(@strinput NVARCHAR(1024)) 
RETURNS NVARCHAR(1024)
AS EXTERNAL NAME SqlSpellChecker.UserDefinedFunctions.SpellChecker
GO
CREATE FUNCTION SpellCheckerRefresh(@strinput NVARCHAR(1024)) 
RETURNS NVARCHAR(1024)
AS EXTERNAL NAME SqlSpellChecker.UserDefinedFunctions.SpellCheckerRefresh
GO

### Description.

This is the SQLCLR implementation of the SpellCorrect based on Peter Norvig's idea
https://norvig.com/spell-correct.html 

### Using.

On the first call it loads the words list form the current database and then uses it. 
This words list is an English most usable words.
The assembly should be added to a database and the function declared: 
```
DROP FUNCTION dbo.SpellChecker 
GO 
DROP ASSEMBLY SqlSpellChecker 
GO 
CREATE ASSEMBLY SqlSpellChecker FROM 'E:\SqlSpellChecker\bin\Release\SqlSpellChecker.dll' 
GO 
CREATE FUNCTION SpellChecker(@strinput NVARCHAR(1024)) 
RETURNS NVARCHAR(1024) 
AS EXTERNAL NAME SqlSpellChecker.UserDefinedFunctions.SpellChecker 
GO 

--The function can be used like: 
select dbo.SpellCheck(N'indienz')
-------
indians
```


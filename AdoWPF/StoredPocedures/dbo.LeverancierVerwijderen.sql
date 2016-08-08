CREATE PROCEDURE LeverancierVerwijderen (@LevNr int)
AS
DELETE FROM Leveranciers
WHERE LevNr = @Levnr
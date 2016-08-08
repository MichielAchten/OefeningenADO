CREATE PROCEDURE LeverancierWijzigen(@OudeLevNr int, @NieuweLevNr int)
AS
UPDATE Planten
SET LevNr = @NieuweLevNr
WHERE LevNr = @OudeLevNr
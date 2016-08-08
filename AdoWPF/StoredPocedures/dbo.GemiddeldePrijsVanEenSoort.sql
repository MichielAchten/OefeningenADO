CREATE PROCEDURE GemiddeldePrijsVanEenSoort(@Soort nvarchar(10))
AS
SELECT Avg(VerkoopPrijs) AS GemiddeldeVerkoopPrijs
FROM Soorten INNER JOIN Planten ON Soorten.SoortNr = Planten.SoortNr
WHERE Soorten.Soort = @Soort
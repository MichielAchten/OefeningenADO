CREATE PROCEDURE PlantenGegevens(
	@plantnr		int,
	@plantNaam		nvarchar(30) OUTPUT,
	@soort			nvarchar(10) OUTPUT,
	@leverancier	nvarchar(30) OUTPUT,
	@kleur			nvarchar(10) OUTPUT,
	@kostprijs		money		OUTPUT)
AS
Select @plantnaam = Planten.Naam, @soort = Soort, @kleur = Kleur,
	@leverancier = Leveranciers.Naam, @kostprijs = VerkoopPrijs
from Soorten inner join
	(Planten inner join Leveranciers on Planten.Levnr = Leveranciers.LevNr)
	on Soorten.SoortNr = Planten.SoortNr
Where PlantNr = @plantnr
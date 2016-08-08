CREATE PROCEDURE LeverancierToevoegen(@Naam nvarchar(30),@Adres nvarchar(30),@PostNr nvarchar(10),@Woonplaats nvarchar(30))
AS
	insert into Leveranciers(naam, adres, postNr, woonplaats)
	values (@Naam, @Adres, @PostNr, @Woonplaats)
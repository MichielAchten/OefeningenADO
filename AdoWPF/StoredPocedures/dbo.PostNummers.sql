﻿CREATE PROCEDURE PostNummers
AS
	SELECT PostNr
	FROM Leveranciers
	GROUP BY PostNr
	ORDER BY PostNr
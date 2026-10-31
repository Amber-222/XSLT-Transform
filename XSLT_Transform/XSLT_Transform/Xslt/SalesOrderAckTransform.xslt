<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="text" encoding="UTF-8" media-type="application/json"/>

	<xsl:template match="/SalesOrder">
		{
			"orderReference": "<xsl:value-of select="//orderRef"/>",
			"internalReference": "<xsl:value-of select="//internalRef"/>",
			"acknowledgementStatus": "ACCEPTED",
			"message": "Sales order acknowledgement generated successfully."
		}
	</xsl:template>
</xsl:stylesheet>
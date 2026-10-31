<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!--outputting plain text/JSON format -->
	<xsl:output method="text" encoding="UTF-8" media-type="application/json"/>

	<!--match the root node created by JSON-to-XML-->
	<xsl:template match="/SalesOrder">
		{
		"orderReference": "<xsl:value-of select="//orderRef"/>",
		"category": "<xsl:value-of select="//categoryCode"/>",
		"status": "PROCESSED"
		}
	</xsl:template>
</xsl:stylesheet>
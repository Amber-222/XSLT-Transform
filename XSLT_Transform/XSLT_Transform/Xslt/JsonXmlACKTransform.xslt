<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" indent="yes" encoding="UTF-8"/>

	<xsl:template match="/ExpectedSOACK">
		<SalesOrderAcknowledgement>
			<OrderRef>
				<xsl:value-of select="orderReference"/>
			</OrderRef>
			<InternalRef>
				<xsl:value-of select="internalReference"/>
			</InternalRef>
			<AcknowledgementStatus>
				<xsl:value-of select="acknowledgementStatus"/>
			</AcknowledgementStatus>
			<Details>
				<xsl:value-of select="message"/>
			</Details>
		</SalesOrderAcknowledgement>
	</xsl:template>
</xsl:stylesheet>
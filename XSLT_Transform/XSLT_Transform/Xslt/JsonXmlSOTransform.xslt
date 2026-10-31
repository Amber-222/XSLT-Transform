<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" indent="yes" encoding="UTF-8"/>

	<xsl:template match="/ExpectedSO">
		<SalesOrderConfirmation>
			<OrderRef>
				<xsl:value-of select="orderReference"/>
			</OrderRef>
			<CategoryCode>
				<xsl:value-of select="category"/>
			</CategoryCode>
			<Status>
				<xsl:value-of select="status"/>
			</Status>
		</SalesOrderConfirmation>
	</xsl:template>
</xsl:stylesheet>
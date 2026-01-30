<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" indent="yes" encoding="utf-8"/>

  <xsl:key name="by-employee" match="item" use="concat(@name, '|', @surname)"/>

  <xsl:template match="/">
    <Employees>
      <xsl:for-each select="/Pay/item[generate-id() = generate-id(key('by-employee', concat(@name, '|', @surname))[1])]">
        <Employee>
          <xsl:attribute name="name"><xsl:value-of select="@name"/></xsl:attribute>
          <xsl:attribute name="surname"><xsl:value-of select="@surname"/></xsl:attribute>
          <xsl:for-each select="key('by-employee', concat(@name, '|', @surname))">
            <xsl:sort select="@month"/>
            <salary>
              <xsl:attribute name="amount"><xsl:value-of select="@amount"/></xsl:attribute>
              <xsl:attribute name="month"><xsl:value-of select="@month"/></xsl:attribute>
            </salary>
          </xsl:for-each>
        </Employee>
      </xsl:for-each>
    </Employees>
  </xsl:template>
</xsl:stylesheet>

<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" xmlns:wix="http://schemas.microsoft.com/wix/2006/wi">
  
	<!-- Copy all attributes and elements to the output. -->
  <xsl:template match="@*|*">
    <xsl:copy>
      <xsl:apply-templates select="@*" />
      <xsl:apply-templates select="*" />
    </xsl:copy>
  </xsl:template>

  <xsl:output method="xml" indent="yes" />

  <!-- Remove DeviceManager.WindowsService.exe file from heat-generated harvest -->
  <xsl:key name="executable-search" 
		   match="wix:Component[wix:File/@Source = '$(var.WinServiceOutputPath)\DeviceManager.WindowsService.exe']" 
		   use="@Id" />
	<xsl:template match="wix:Component[key('executable-search', @Id)]" />
	<xsl:template match="wix:ComponentRef[key('executable-search', @Id)]" />

</xsl:stylesheet>
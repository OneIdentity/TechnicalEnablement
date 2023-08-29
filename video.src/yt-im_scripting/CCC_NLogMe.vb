' ******************************************************************************
' 
' !!! IMPORTANT INFORMATION !!!
' This software is free to use and can be shared with everyone.
' This software is part of a public One Identity - Identity Manager training. 
' We recommend to read and test this code carefully before using it. 
' Using these code snippets is ALWAYS ON YOUR OWN RISK!
' 
' ******************************************************************************


   ' VI-KEY(<Key><T>DialogScript</T><P>NLogMe</P></Key>, NLogMe)
    ''' <summary>
    ''' Template to show how to generate NLog messages in a 1Im script
    ''' 
    ''' Created by HRA, 2017-01-19
    ''' Last Change by HRA, 2017-01-19
    ''' </summary>
    ''' 
#If Not SCRIPTDEBUGGER Then
  Reference NLog.dll
  Imports NLog
  Imports VI.Base.Logging
#End If

    Public Sub CCC_NLogMe()

      '--> Define your Logger name or use (YourLoggerName)
      Dim My_logger = LogManager.GetLogger("YourLoggerName")

      '--> Or use the Standard Logger name'
      Dim Std_logger = LogManager.GetLogger(LogNames.ObjectLog)

      '--> Log something (Watch also other options of log.)
      My_logger.Debug("Your Message with Parameter {0}", 42)
      My_logger.Debug("Hello here is an error: {0} {1}", "Parameter 1", "Parameter 2")

    End Sub
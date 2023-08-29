' ******************************************************************************
' 
' !!! IMPORTANT INFORMATION !!!
' This software is free to use and can be shared with everyone.
' This software is part of a public One Identity - Identity Manager training. 
' We recommend to read and test this code carefully before using it. 
' Using these code snippets is ALWAYS ON YOUR OWN RISK!
' 
' ******************************************************************************


		' VI-KEY(<Key><T>DialogScript</T><P>CCC_Person_CSV_File_Export</P></Key>, CCC_Person_CSV_File_Export)
    ''' <summary>
    ''' Exports Person data to a CSV file (using the One Identity Manager API example)
    ''' 
    ''' Created by HRA, 2016-10-24
    ''' Last Change by HRA, 2016-10-24
    ''' </summary>

#If Not SCRIPTDEBUGGER Then
  Imports System.Collections.Generic
#End If

    Public Sub CCC_Person_CSV_File_Export(strFilePath As String, strCondition As String)

      Dim StartTime As DateTime = Now()
      Dim strTimeStamp As String = VID_ISODatetimeForFilename(StartTime)

      Dim strCSVLine As String = ""
      Dim strQuery As String = ""
      Const strDelimeter As String = ";"
      Dim strLocation As String = ""
      Dim strDepartment As String = ""

      Dim strLogFileName As String = String.Format("{0}\{1}_PersonExport.log", strFilePath, strTimeStamp)
      Dim strCSVFileName As String = String.Format("{0}\{1}_PersonExport.csv", strFilePath, strTimeStamp)

      Try
        '--> Start Message
        VID_Write2Log(strLogFileName, String.Format("--> Script started at {0}", StartTime.ToString()))

        '--> Variable declaration
        Dim colPersons As IEntityCollection
        'Dim dbPerson As IEntity

        '--> create the person query object
        Dim qPerson = Query _
                      .From("Person") _
                      .Select("Lastname", "Firstname", "Centralaccount", "uid_department", "uid_locality") _
                      .Where(strCondition)

        '--> Load a collection of Person objects
        colPersons = Session.Source.GetCollection(qPerson)

        '--> Load Department Data into the memory
        '--> Added by performance optimization 2 (avoid to many database queries)
        Dim dicDepartment As New Dictionary(Of String, String)
        Dim qDepartment = Query.From("Department") _
                            .Select("uid_department", "Departmentname")
        Dim colDepartment As IEntityCollection = Session.Source.GetCollection(qDepartment)
        For Each DepElem In colDepartment
          dicDepartment.Add(DepElem.GetValue("uid_department").ToString(), DepElem.GetValue("Departmentname").ToString())
        Next

        '--> Load Locations into the memory
        '--> Added by performance optimization 2 (avoid to many database queries)
        Dim dicLocation As New Dictionary(Of String, String)

        Dim qLocation = Query.From("Locality") _
                            .Select("uid_locality", "Ident_locality")
        Dim colLocality As IEntityCollection = Session.Source.GetCollection(qLocation)
        For Each LocElem In colLocality
          dicLocation.Add(LocElem.GetValue("uid_locality").ToString(), LocElem.GetValue("Ident_locality").ToString())
        Next

        '--> Add number of objects to be affected to the log
        VID_Write2Log(strLogFileName, String.Format("--> {0} person objects are going to be affected.", colPersons.Count().ToString()))


        Using swFile As New StreamWriter(strCSVFileName, True)
          '--> CSV headder Information
          swFile.WriteLine(String.Format("Lastname{0}Firstname{0}Accountname{0}Department{0}Location", strDelimeter))

          '--> Handle all collection items
          For Each colElement As IEntity In colPersons

            ' Create a full Person object from the list element.
            ' Attention: This triggers a database SELECT operation 
            ' to get the missing values in most cases. 

            '--> Get FK information

            '--> Deactivated by Performance optimization 1 (avoid single objects)
            'dbPerson = colElement.Create(Session, EntityLoadType.Interactive)

            '--> Deactivated by Performance optimization 1 (avoid single objects)
            'strDepartment = dbPerson.CreateWalker(Session).GetValue("FK(uid_department).Departmentname").ToString()
            'strLocation = dbPerson.CreateWalker(Session).GetValue("FK(uid_locality).Ident_Locality").ToString()

            '--> Deactivated by Performance optimization 2 (avoid to many database queries)
            'strQuery = String.Format("uid_department='{0}'", colElement.GetValue("uid_department").ToString())
            'Session.Source.TryGetSingleValue(Of String)("Department", "Departmentname", strQuery, strDepartment)

            'strQuery = String.Format("uid_locality='{0}'", colElement.GetValue("uid_locality").ToString())
            'Session.Source.TryGetSingleValue(Of String)("Locality", "Ident_locality", strQuery, strLocation)

            dicDepartment.TryGetValue(colElement.GetValue("uid_department").ToString(), strDepartment)

            dicLocation.TryGetValue(colElement.GetValue("uid_locality").ToString(), strLocation)

            '--> Build CSV line
            '--> Deactivated by Performance optimization 1 (avoid single objects)
            'dbPerson.GetValue("Lastname").ToString(, _
            'dbPerson.GetValue("Firstname").ToString(), _
            'dbPerson.GetValue("Centralaccount").ToString(), _

            strCSVLine = String.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}", _
                                      strDelimeter, _
                                      colElement.GetValue("Lastname").ToString(), _
                                      colElement.GetValue("Firstname").ToString(), _
                                      colElement.GetValue("Centralaccount").ToString(), _
                                      strDepartment, _
                                      strLocation
                                      )


            '--> write the CSV line into a file
            swFile.WriteLine(strCSVLine)

          Next '--> End handling entries
        End Using '--> Close the file

        '--> Script ended + execution time to the log
        VID_Write2Log(strLogFileName, String.Format("--> Person export ended at {0}.{1}--> Execution time : {2}ms", Now.ToString(), vbCrLf, (StartTime - Now()).ToString("ss")))

      Catch ex As Exception
        '--> If an exception occurs this code will be executed 
        VID_Write2Log(strLogFileName, ViException.ErrorString(ex))
        '--> Allows easier debugging should be productively commented out
        'Throw New Exception("==> Debug: Person-Export: ", ex)

      End Try
    End Sub
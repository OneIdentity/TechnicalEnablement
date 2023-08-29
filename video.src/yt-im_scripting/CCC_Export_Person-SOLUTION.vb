' ******************************************************************************
' 
' !!! IMPORTANT INFORMATION !!!
' This software is free to use and can be shared with everyone.
' This software is part of a public One Identity - Identity Manager training. 
' We recommend to read and test this code carefully before using it. 
' Using these code snippets is ALWAYS ON YOUR OWN RISK!
' 
' ******************************************************************************


    ' VI-KEY(<Key><T>DialogScript</T><P>CCC-B5ACC666C50A024C91031C2B5AB641AE</P></Key>, CCC_Export_Person)

#If Not SCRIPTDEBUGGER Then
  Imports System.Collections.Generic
#End If

    Public Sub CCC_Export_Person(ByVal strPath As String, ByVal strWhereClause As String)
      '--> var declarations
      Dim colPersons As IEntityCollection
      Dim colDepartment As IEntityCollection
      Dim colLocality As IEntityCollection

      '--> Commented out because single objects are much more expensive than Collection objects
      'Dim dbPerson As ISingleDbObject 

      Dim startTime As Date = Now()
      Dim strTimeStamp As String = VID_ISODatetimeForFilename(startTime)

      Dim strCSVFileName = String.Format("{0}\{1}_ExportPerson.csv", strPath, strTimeStamp)
      Dim strLOGFileName = String.Format("{0}\{1}_ExportPerson.log", strPath, strTimeStamp)

      Dim strCSVLine As String = ""
      Dim strDep As String = ""
      Dim strLoc As String = ""

      '--> To minimize SQL traffic use dictionaries and store data to memory
      Dim DicDep As IDictionary(Of String, String) = New Dictionary(Of String, String)
      Dim DicLoc As IDictionary(Of String, String) = New Dictionary(Of String, String)

      '--> Start exception handling block
      Try
        ' --> Add starting time to log
        VID_Write2Log(strLOGFileName, String.Format("--> Export started at: {0}", startTime.ToString()))

        '--> Add data to dictionary
        '--> Department: uid_department and Departmentname
        Dim qDepartment = Query.From("Department") _
                          .Select("Departmentname")
        colDepartment = Session.Source.GetCollection(qDepartment, EntityCollectionLoadType.Slim)
        For Each colElm As IEntity In colDepartment
          DicDep.Add(colElm.GetValue("uid_department").String, colElm.Columns("Departmentname").GetDisplayValue(Session))
        Next

        '--> Locality: uid_locality and Ident_locality
        Dim qLocality = Query.From("Locality") _
                        .Select("Ident_locality")
        colLocality = Session.Source.GetCollection(qLocality, EntityCollectionLoadType.Slim)
        For Each colElm As IEntity In colLocality
          DicLoc.Add(colElm.GetValue("UID_Locality").String, colElm.Columns("Ident_Locality").GetDisplayValue(Session))
        Next

        '--> Create the person query
        Dim qPerson = Query.From("Person") _
            .Where(strWhereClause) _
            .Select("Lastname", "Firstname", "Centralaccount", "UID_Department", "UID_Locality")

        '--> Load data to collection
        colPersons = Session.Source.GetCollection(qPerson, EntityCollectionLoadType.BulkReadOnly)

        ' --> Write number of loaded records to log
        VID_Write2Log(strLOGFileName, String.Format("--> Number of records: {0}", colPersons.Count().ToString()))

        '--> Opening csv file for writing
        Using swFile As New System.IO.StreamWriter(strCSVFileName, True)

          '--> Write column header
          swFile.WriteLine("Lastname;Firstname;Central User Account;Department;Location")

          '--> Run export for the whol collection
          For Each colElement As IEntity In colPersons

            '--> Create a single object
            '--> Commented out to increase the performance
            'dbPerson = colElement.Create()

            '--> Read department
            strDep = colElement.GetValue("uid_department").String

            '--> No getsingleproperty use dictionary instead --> performance
            'strDep = Connection.GetSingleProperty("Department", "Departmentname", "uid_department='" + strDep + "'").ToString()
            DicDep.TryGetValue(strDep, strDep)

            '--> Read location
            strLoc = colElement.GetValue("uid_locality").String

            '--> No getsingleproperty use dictionary instead --> performance
            'strLoc = Connection.GetSingleProperty("Locality", "ident_locality", "uid_locality='" + strLoc + "'").ToString()
            DicLoc.TryGetValue(strLoc, strLoc)

            '--> Generate csv line
            strCSVLine = String.Format("{0};{1};{2};{3};{4}", _
                                        colElement.GetValue("Lastname").String, _
                                        colElement.GetValue("Firstname").String, _
                                        colElement.GetValue("Centralaccount").String, _
                                        strDep, _
                                        strLoc _
                                      )

            '--> Debug.print can used for debgging in Code development tools. The line must commented out befor the script is saved to the database'
            'Debug.Print(strCSVLine)

            '--> Write line to csv file
            swFile.WriteLine(strCSVLine)

          Next '--> Loop ends

        End Using '--> Close file

      Catch ex As Exception

        ' --> Write error messages to log
        VID_Write2Log(strLOGFileName, String.Format("==> ERROR: {0}", ViException.ErrorString(ex)))

      Finally
        ' --> Write end time to log
        VID_Write2Log(strLOGFileName, String.Format("--> Export ended at: {0}", Now().ToString()))

      End Try
    End Sub
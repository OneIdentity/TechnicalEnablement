' ******************************************************************************
' 
' !!! IMPORTANT INFORMATION !!!
' This software is free to use and can be shared with everyone.
' This software is part of a public One Identity - Identity Manager training. 
' We recommend to read and test this code carefully before using it. 
' Using these code snippets is ALWAYS ON YOUR OWN RISK!
' 
' ******************************************************************************


    ' VI-KEY(<Key><T>DialogScript</T><P>CCC_Training_ModifyData</P></Key>, CCC_Training_ModifyData)
    Public Sub CCC_Training_ModifyData(strInputText As String)

      '-------------------------------------------------------------------------------------
      '--> One Identity Manager API, what we saw before...
      '-------------------------------------------------------------------------------------

      '--> Collection and Single Object
      Dim colPersons As IEntityCollection

      '--> create the person query object
      '    - First create a sql query statement using sql formatter
      Dim f As ISqlFormatter = Session.SqlFormatter
      Dim strCondition As String = ""

      '    - Use SQL Formatter
      strCondition = f.AndRelation(f.Comparison("Firstname", "H%", ValType.String, CompareOperator.Like), _
                                   f.Comparison("Lastname", "Abele", ValType.String, CompareOperator.Equal))

      '    - Generate the query object
      Dim qPerson = Query _
                    .From("Person") _
                    .SelectDisplays() _
                    .Where(strCondition)

      '    - Load a collection of Person objects
      colPersons = Session.Source.GetCollection(qPerson)


      '-------------------------------------------------------------------------------------
      '--> Now lets add a new object
      '-------------------------------------------------------------------------------------
      Dim dbPerson As New Person(Session)
      With dbPerson
        .LastName = "Mueller"
        .FirstName = "Jack"
        .CentralAccount = CCC_AE_BuildCentralAccount("", "Mueller", "Jack")
        .Description = "Add by script!"
      End With
      '   - Save this object
      dbPerson.Save(Session)


      '-------------------------------------------------------------------------------------
      '--> Create a new Person object (you dont need to know about any order
      '-------------------------------------------------------------------------------------
      Dim person As IEntity = Session.Source.CreateNew("Person")

      '   - Create a property bag and fill it with values
      '   - The sort order doesn't matter here
      Dim bag As New PropertyBag()
      bag.PutValue("Firstname", "John")
      bag.PutValue("Lastname", "Doe")
      bag.PutValue("CentralAccount", "JohnD")

      '   - Push the values into our person object in the right order
      bag.ChangeEntity(Session, person, True)

      '   - And save it
      person.Save(Session)

      '-------------------------------------------------------------------------------------
      '--> Change the Description on all person objects but dont handle each object seperate
      '-------------------------------------------------------------------------------------
      '    - Load all with the collection
      colPersons = Session.Source.GetCollection(qPerson, EntityCollectionLoadType.Bulk)

      '    - Prepare set of objects to get transfered in one operation
      Using uow = Session.StartUnitOfWork()

        '  - Edit list of all objects
        For Each ePerson As IEntity In colPersons
          Dim strDescription As String = ePerson.GetValue("Description").String

          '   - Assign Department to account
          If strDescription.Length > 0 Then
            ePerson.PutValue("Description", String.Format("{0}{1}--> {2}", strDescription, vbCrLf, "Touched by a script"))
          Else
            ePerson.PutValue("Description", "--> Touched by a script")
          End If

          '   - put the object in the unit of work
          uow.Put(ePerson)
        Next

        '   - All person obejcts will be saved here!!!
        uow.Commit()
      End Using
    End Sub
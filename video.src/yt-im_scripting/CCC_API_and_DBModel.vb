' ******************************************************************************
' 
' !!! IMPORTANT INFORMATION !!!
' This software is free to use and can be shared with everyone.
' This software is part of a public One Identity - Identity Manager training. 
' We recommend to read and test this code carefully before using it. 
' Using these code snippets is ALWAYS ON YOUR OWN RISK!
' 
' ******************************************************************************

' VI-KEY(<Key><T>DialogScript</T><P>CCC_API_and_DBModel</P></Key>, CCC_API_and_DBModel)
    Public Sub CCC_API_and_DBModel()
      '--> How to use the API and the db model
      '    On the bases of an AD Account

      '---------------------------------------------------------------------
      '--> 1. Create a single object
      '---------------------------------------------------------------------
      Dim dbADAccount As IEntity

      '--> Create a new object
      dbADaccount = Session.Source.CreateNew("ADSAccount")

      '--> Load an existing one per UID
      Dim strUIDADSAccount As String = "015bb506-c28b-4275-9fb0-892f70bdade6"
      dbADAccount = Session.Source.Get("ADSAccount", strUIDADSAccount)
      '    - As well exist a way to get an empty object without error if the object could not be found
      Dim qADAccount = Query.From("ADSAccount") _
                            .SelectAll() _
                            .Where(String.Format("uid_adsaccount='{0}'", strUIDADSAccount))

      If Session.Source.TryGet(qADAccount, EntityLoadType.Interactive, dbADAccount) Then
        System.Diagnostics.Debug.WriteLine("Element loaded: " + dbADAccount.ToString())
      End If

      '--> Load an existing account per XObjectKey
      Dim strXobjkADSAccount As String = "<Key><T>ADSAccount</T><P>014d86dc-989f-42c6-ab8c-4d91a945f5fc</P></Key>"
      Dim xobjkADSAccount As New DbObjectKey(strXobjkADSAccount)
      dbADaccount = Session.Source.Get(xobjkADSAccount)

      '---------------------------------------------------------------------
      '--> 2. Create and handle a single object out of a db relation
      '---------------------------------------------------------------------
      Dim dbPerson As IEntity

      '--> Get a linked object using the db relation (foreign key pointing to the Person table)
      Dim FKPerson As IEntityForeignKey = dbADAccount.GetFk(Session, "UID_Person")
      If Not FKPerson.IsEmpty Then dbPerson = FKPerson.GetParent()

      '--> Get child related objects
      dbPerson = Session.Source.Get("Person", "f89f934f-4843-4fa3-be55-a336ad19c7a4")

      Dim qADAccounts = Query.From("ADSAccount") _
                             .Where(String.Format("uid_Person='{0}'", dbPerson.GetValue("uid_person").ToString())) _
                             .SelectDisplays()

      Dim colADAccount As IEntityCollection = Session.Source.GetCollection(qADAccounts, EntityCollectionLoadType.Bulk)
      For Each eColADAccount In colADAccount
        '--> Full managability of all ADAccount collection elements because of EntityCollectionLoadType.Bulk
      Next


      '---------------------------------------------------------------------
      '--> 3. Handle different type of properties
      '---------------------------------------------------------------------
      Dim strPropertyValue As String = ""
      Dim strPropertyDisplay As String = ""

      '--> Get standard property
      strPropertyValue = dbADAccount.GetValue("CN").ToString()

      '--> Put standard property
      dbADAccount.PutValue("Description", "My new value")

      '--> Get single value from a related object using UID
      '    example: DN of the related AD container // ADSAccount -> ADSContainer
      strPropertyValue = dbADAccount.CreateWalker(Session).GetValue("FK(uid_adscontainer).Distinguishedname").ToString
      '    example: Domain of the account // ADSAccount -> ADSContainer -> ADSDomain
      strPropertyValue = dbADAccount.CreateWalker(Session).GetValue("FK(uid_adscontainer).FK(uid_adsdomain).ident_domain").ToString

      '--> Bitmask oder List of limited values display
      '--> a. The plain value
      strPropertyValue = dbADAccount.GetValue("XMarkedForDeletion").ToString()
      '--> b. the display value
      strPropertyDisplay = dbADAccount.Columns("XMarkedForDeletion").GetDisplayValue(Session)

      '--> Get an original value of a fresh modified property (pre-saving)
      dbADAccount.PutValue("Description", "New Value")

      '    - The modified property value
      strPropertyValue = dbADAccount.Columns("Description").GetValue().ToString()
      '    - The original property value (as it was in front of the modification)
      Dim strOldPropertyValue As String = dbADAccount.Columns("Description").GetOldValue().ToString()

      '--> Handle multi value properties (e.g. listr of email addresses
      '    Create a multi value property object from the data of column OtherMailbox
      Dim mvpOtherMailbox As MultiValueProperty = New MultiValueProperty(dbADAccount.GetValue("OtherMailbox").String)

      '    - Iterate over all values
      For Each eMvpValue As String In mvpOtherMailbox
        'Print out the entry
        System.Diagnostics.Debug.WriteLine(eMvpValue)
      Next

      '    - Check for existence of an entry
      If Not mvpOtherMailbox.Contains("NewAddress@example.com") Then
        ' Expand the multi value property
        mvpOtherMailbox.Add("NewAddress@example.com")
      End If

      '    - Remove a value. If it's not found no exception is thrown.
      mvpOtherMailbox.Remove("NewAddress@example.com")

      '    - Set the MVP value into the object column
      dbADAccount.PutValue("OtherMailbox", mvpOtherMailbox.Value)

      '    - Get the entry on the position defined by iPos
      Dim iPos As Integer = 0
      If iPos < mvpOtherMailbox.Count Then
        System.Diagnostics.Debug.WriteLine(mvpOtherMailbox(iPos).ToString())
      Else
        System.Diagnostics.Debug.WriteLine("")
      End If

      '---------------------------------------------------------------------
      '--> 4. More special data and object actions
      '---------------------------------------------------------------------
      '--> Get single property and store it to variable strPropertyValue (like a single sql query)
      Dim StrWhereClause As String = String.Format("uid_adsdomain in (select uid_adsdomain from adscontainer where uid_adscontainer in (select uid_adscontainer from adsaccount where uid_adsaccount ='{0}'))", strUIDADSAccount)
      Session.Source.TryGetSingleValue(Of String)("ADSdomain", "Ident_domain", StrWhereClause, strPropertyValue)

      '--> Get a Configuration Parameter value
      Dim strConfigParmReference As String = "Common\Autoupdate\ServiceUpdateType"
      Dim strConfigParm As String = Session.Config.GetConfigParm(strConfigParmReference).ToString()

      '--> Call a customizer method on an object // additionally to call templates on a object
      dbADAccount.CallMethod("ExecuteTemplates")

      '--> Call a function on a object
      Dim strReturn As String = dbADAccount.CallFunction("Name of the function").ToString()

      '--> Call an event to raise fulfillment processes
      '    This is typically done automatically for insert/update/delete
      '    Could be triggered in any case by the API

      Using uow As IUnitOfWork = Session.StartUnitOfWork()
        For Each eADAccount As IEntity In colADAccount
          '- Generate a event
          uow.Generate(eADAccount, "DEACTIVATE")
        Next
      End Using

    End Sub

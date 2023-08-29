' ******************************************************************************
' 
' !!! IMPORTANT INFORMATION !!!
' This software is free to use and can be shared with everyone.
' This software is part of a public One Identity - Identity Manager training. 
' We recommend to read and test this code carefully before using it. 
' Using these code snippets is ALWAYS ON YOUR OWN RISK!
' 
' ******************************************************************************

' --> In Designer vb vode windows use F2 to get code defined snippets

'############################################################
' --> Template on Person.Customproperty01
'############################################################

If $FK(UID_Department).UID_Profitcenter:string$<>"" Then
  '--> Take the accountnumber of the costcenter assigned to the departmentm of my person object
	Value=$FK(UID_Department).FK(uid_ProfitCenter).AccountNumber:string$
Else
	'--> Take the account number of the users costcenter
	Value=$FK(UID_ProfitCenter).AccountNumber:string$
End If


'############################################################
' --> $-Notation -- Template in Designer (from above)
'############################################################
If $FK(UID_Department).UID_Profitcenter:string$<>"" Then
  '--> Take the accountnumber of the costcenter assigned to the departmentm of my person object
	Value=$FK(UID_Department).FK(uid_ProfitCenter).AccountNumber:string$
Else
	'--> Take the account number of the users costcenter
	Value=$FK(UID_ProfitCenter).AccountNumber:string$
End If

'############################################################
' --> $-Notation -- Same template in Visual Studio (VB.NET)
'############################################################
' VI-KEY(<Key><T>DialogColumn</T><P>QER-3423576F3B114BFF8D1EAB2637F7CBF1</P></Key>, Tmpl_Person_CustomProperty01)
Public Sub Tmpl_Person_CustomProperty01 ()
If GetTriggerValue("FK(UID_Department).UID_Profitcenter").String<>"" Then
  '--> Take the accountnumber of the costcenter assigned to the departmentm of my person object
	Value=GetTriggerValue("FK(UID_Department).FK(uid_ProfitCenter).AccountNumber").String
Else
	'--> Take the account number of the users costcenter
	Value=GetTriggerValue("FK(UID_ProfitCenter).AccountNumber").String
End If
End Sub


'############################################################
' --> $-Notation -- Special Examples
'############################################################ 
If Not $(IsLoaded):Bool$ Then
	Value = Connection.GetConfigparm("QER\Attestation\UserApproval\InitialApprovalState")
End If

'-> $(IsLoaded):Bool$ is a logical value
'Metavalue 	Meaning
'------------------------------------------------------------
'IsLoaded			This value specifies whether the object is loaded from the database.
'IsChanged		This value specifies whether the object is altered when it is loaded from the database.
'IsDifferent	This value specifies whether the new value is different from the old value. 
'							You can access to the column through: Columnname[C].
'IsDeleted		This value specifies whether the object is marked for deletion.
'------------------------------------------------------------
'-> Connection.GetConfigparm("Fullpath to the parm") gets access to a specific Configuration parameter value


'############################################################
' --> $-Notation -- Origin values
'############################################################ 
If $ExitDate:Date$ <> $ExitDate[o]:Date$ Then
'If Exitdate updated, DatelastWorked follows automatically
'manual update DatelastWorked must be possible
	Value = $ExitDate:Date$
End If
 
'--> This is similar to  $ExitDate:Date[C]$ which returns a bool with True if the origin and current are different.

'--> Please have look into file Script-SDK\04 Templates\01 Templates.vb for more examples.
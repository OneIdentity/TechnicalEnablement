'
' ONE IDENTITY LLC. PROPRIETARY INFORMATION
'
' This software is confidential.  One Identity, LLC. or one of its affiliates or
' subsidiaries, has supplied this software to you under terms of a
' license agreement, nondisclosure agreement or both.
'
' You may not copy, disclose, or use this software except in accordance with
' those terms.
'
'
' Copyright 2022 One Identity LLC.
' ALL RIGHTS RESERVED.
'
' ONE IDENTITY LLC. MAKES NO REPRESENTATIONS OR
' WARRANTIES ABOUT THE SUITABILITY OF THE SOFTWARE,
' EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE IMPLIED WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE, OR
' NON-INFRINGEMENT.  ONE IDENTITY LLC. SHALL NOT BE
' LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE
' AS A RESULT OF USING, MODIFYING OR DISTRIBUTING
' THIS SOFTWARE OR ITS DERIVATIVES.
'


'*************************************************************************
'*
'* Templates are short VB.NET snippets.
'* 
'* The current value of the column is delived in the Value property.
'* The new value to set has to be written to Value too. The value is 
'* written to the column if the value property changes during the 
'* template script. All templates have the form Value = ...
'* 
'* The Provider property contains a IValueProvider instance allowing 
'* access to properties of the base object and other per foreign key
'* reachable objects.
'*
'*************************************************************************



'*************************************************************************
'*
'* Simple assignment of current date and time
'*
'*************************************************************************

Value = DateTime.Now


'*************************************************************************
'*
'* Usage of preprocessor conditions
'*
'* Preprocessor conditions are administrated by enabling the according
'* configuration parameters in the Designer frontend.
'*
'*************************************************************************
#If ACCOUNTING Then
  Value = 100
#End If


'*************************************************************************
'*
'* Using dollar notation to get values from the base object or other
'* objects reachable by foreign key relations.
'* 
'* Default data type is String. You can request other data types using
'* the :Type suffix.
'*
'*************************************************************************

' Property of the base object
Value = $Firstname$ & " " & $Lastname$

' Properties of related objects
Value = $FK(UID_HardwareType).Ident_HardwareBasictype$

'*************************************************************************
'*
'* Default data type is String. You can request other data types using
'* the :Type suffix.
'*
'*************************************************************************

' Boolean properties
Value = $IsEnabled:Bool$

Value = $FK(UID_HardwareType).IsLocalPeripher:Bool$

'* Valid type specifiers: :String  --> Text  (default)
'*                        :Int     --> Integer value (32Bit)
'*						  :Bool    --> Boolean value
'*                        :Date    --> Datetime
'*						  :Double  --> Floating point (64Bit)  
'*                        :Long    --> Integer value (64Bit)  
'*                        :Decimal --> Decimal number (128Bit)
'*                         



'*************************************************************************
'*
'* Pay attention to object states.
'*
'*************************************************************************

If Not $[IsLoaded]:Bool$ Then 
  Value = DateTime.Now
End If


'* Valid values : $[IsLoaded]:Bool$    --> Object was loaded from database
'*                $[IsDeleted]:Bool$   --> Object is marked for deletion
'*                $[IsChanged]:Bool$   --> Object was changed since Load or Discard
'*                $[IsDifferent]:Bool$ --> Object was changed and contains different values
'*                $[Display]$          --> Display string for the base object



'*************************************************************************
'*
'* Requesting the original value using Property[o]
'*
'* Contains the original value loaded from database. 
'* Attention: Can only be used in the first column reference of the 
'* dollar notation, i.e. $FK(Column1[o]).Column2$ is valid, 
'* $FK(Column1).Column2[o]$ is not valid.
'*
'*************************************************************************

' Sample 1:
' Set Value to true if IsMailObject property has changed and is now set to True
If $IsMailObject[o]:Bool$ <> $IsMailObject:Bool$ and $IsMailObject:Bool$ Then  
  Value = True
End If

' Sample 2:
' Department of the old Org was not empty and Department of the old Org
' is different to the Department of the new Org.
if ($FK(UID_Org[o]).UID_Department$ <> "") and _
   ($FK(UID_Org[o]).UID_Department$ <> $FK(UID_Org).UID_Department$) then
	Value = $FK(UID_Org).FK(UID_Department).DepartmentName$
end if

'*************************************************************************
'*
'* Display values of columns can be requested using the suffix [D].
'* The object layer resolves limited value definitions and multi language 
'* strings in this case.
'*
'* Sample: Person.Gender is a numeric field. The column definition contains
'* a limited value definition resolving the numeric values to display strings.
'* The expression $Gender$ would deliver "1" whereas the expression 
'* $Gender[D]$ would deliver "1 - male".
'*
'*************************************************************************

Value = $NotesRestrictType[D]$



'*************************************************************************
'*
'* It is possible to define column dependencies even in comments.
'* Though it is not recommended to do so.
'*
'*************************************************************************

'$Department$
'$Location$
'$FK(UID_Person).UID_ProfitCenter$
Value = VI_AE_ITDataFromOrg($UID_Person$, Connection.GetConfigParm("Namespace\Notes\Server\ITDATAFrom"),"UID_NotesServer") 


'*************************************************************************
'*
'* Using '$' in Templates
'*
'*************************************************************************

If not CBool(Variables("FULLSYNC")) Then
	If Len($UID_HomeServer$) > 0 Then
	    Value = $Homedirpath$ & "$"
	End If
End If 


'*************************************************************************
'*
'* Checking for empty values using DbVal.IsEmpty
'*
'*************************************************************************

If DbVal.IsEmpty($ExitDate:Date$, ValType.Date) And Not $IsDummyPerson:Bool$ Then
    Value = Date.Today
End If


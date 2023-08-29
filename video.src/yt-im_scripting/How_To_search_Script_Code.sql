/*
' ******************************************************************************
' 
' !!! IMPORTANT INFORMATION !!!
' This software is free to use and can be shared with everyone.
' This software is part of a public One Identity - Identity Manager training. 
' We recommend to read and test this code carefully before using it. 
' Using these code snippets is ALWAYS ON YOUR OWN RISK!
' 
' ******************************************************************************
*/

/*
-----------------------------------------------------------------------
	How to get information about code snippets out of the database
-----------------------------------------------------------------------
*/

--> Get information from Dialogscripts
	select scriptcode 
		from DialogScript 
		where ScriptCode like '%.CallFunction%' 

--> Get information from templates
	select template 
		from dialogcolumn 
		where isnull(template,'')<>''
			and template like '%.comparison%'

--> Get information from FormatScript
	select FormatScript 
		from dialogcolumn 
		where isnull(FormatScript,'')<>''
			and FormatScript like '%xobject%'

--> Get information from methods
	select Methodscript 
		from dialogmethod
		where isnull(Methodscript,'')<>''
			and Methodscript like '%.CallFunction%'


			select name from syscolumns where id in (select id from sysobjects where name = 'person') order by 1



			select * from org where uid_orgroot = (select uid_orgroot from orgroot where ident_orgroot='Job basic Roles')
			select uid_adsaccount, xobjectkey from adsaccount

			select xobjectkey from adscontainer
			select uid_person from person
			
			
----------------------------------------------------
--> Shows all files attached for BusinessAPIServer
----------------------------------------------------

select 
	fr.filename, 
	isnull(fr.SourceDirectory,'.\') as SourceDirectory,
	dt.FullPath,
	dt.Ident_QBMDeployTarget
	from qbmfilerevision fr
		left outer join QBMFileHasDeployTarget fhdt on fr.UID_QBMFileRevision=fhdt.UID_QBMFileRevision
		left outer join QBMDeployTarget dt on dbo.QBM_FCVObjectkeyToElement('ColumnValue1',fhdt.ObjectKeyDeployTarget)=dt.UID_QBMDeployTarget
		where dt.Ident_QBMDeployTarget='BusinessApiServer'
	order by fr.SourceDirectory


----------------------------------------------------
--> Shows entries in the IM FileStore
----------------------------------------------------
select top 5 * 
from qbmfilerevision


----------------------------------------------------
--> Shows all deployment targets
----------------------------------------------------
select Ident_QBMDeployTarget, * from QBMDeployTarget order by 1


----------------------------------------------------
--> Get Table and columns for a specific column name
----------------------------------------------------
select 
	sc.name as ColumnName, 
	so.name as TableName 
	from syscolumns sc 
		join sysobjects so on sc.id = so.id
	where sc.name = 'UID_QBMfilerevision'
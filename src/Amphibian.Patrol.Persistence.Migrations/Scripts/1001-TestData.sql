--this user is not migrated/deployed to the server db, but i suppose someone will try anyway...
insert into users(email,firstname,lastname,passwordsalt,passwordhash,passwordhashiterations)
values ('candidate','test','candidate',0xAC174909DBF93DCE1BC0AD2880391A8BACA2207D84FBBD89DB6D5500A20C7DB0,0x2DC0CDA8863338A36E3EDD1E41238B2542252876C4547C5C860F8D0F47A2B832,5);
--1

insert into users(email,firstname,lastname,passwordsalt,passwordhash,passwordhashiterations)
values ('trainer','test','trainer',0xAC174909DBF93DCE1BC0AD2880391A8BACA2207D84FBBD89DB6D5500A20C7DB0,0x2DC0CDA8863338A36E3EDD1E41238B2542252876C4547C5C860F8D0F47A2B832,5);
--2

insert into users(email,firstname,lastname,passwordsalt,passwordhash,passwordhashiterations)
values ('admin','admin','',0xAC174909DBF93DCE1BC0AD2880391A8BACA2207D84FBBD89DB6D5500A20C7DB0,0x2DC0CDA8863338A36E3EDD1E41238B2542252876C4547C5C860F8D0F47A2B832,5);
--3

insert into patrols(name,EnableTraining,EnableAnnouncements,EnableEvents,EnableScheduling,EnableShiftSwaps,Timezone) values ('Big Mountain Patrol',1,1,1,1,1,'Eastern Standard Time');
--1
insert into patrols(name,EnableTraining,EnableAnnouncements,EnableEvents,EnableScheduling,EnableShiftSwaps,Timezone) values ('Tiny Mountain Patrol',1,1,1,1,1,'Eastern Standard Time');
--2

insert into patrolusers (patrolid,userid) values (1,1);
--1
insert into patrolusers (patrolid,userid) values (2,1);
--2
insert into patrolusers (patrolid,userid) values (1,2);
--3
insert into patrolusers (patrolid,userid) values (2,2);
--4
insert into patrolusers (patrolid,userid,Role) values (1,3,'Administrator');
--3
insert into patrolusers (patrolid,userid,Role) values (2,3,'Administrator');
--4

--plans
--big patrol
insert into plans (name,patrolid) values ('Ski Alpine',1);
--1
insert into plans (name,patrolid) values ('Snowboard Alpine',1);
--2
--tiny patrol
insert into plans (name,patrolid) values ('Red Jacket',2);
--3

--sections
--big patrol
insert into sections (name,patrolid,color) values ('Ski Skills',1,'#A8DFFF');
--1
insert into sections (name,patrolid,color) values ('Snowboard Skills',1,'#A8DFFF');
--2
insert into sections (name,patrolid,color) values ('Toboggan Skills',1,'#2BB1FF');
--3
--tiny patrol
insert into sections (name,patrolid,color) values ('Red Jacket',2,'#A8DFFF');
--4

--big patrol final
insert into sections (name,patrolid,color) values ('Final',1,'#82FF8F');
--5



--4

--plansections
--big patrol
insert into plansections (planid,sectionid) values (1,1);
--1
insert into plansections (planid,sectionid) values (1,3);
--2
insert into plansections (planid,sectionid) values (2,2);
--3
insert into plansections (planid,sectionid) values (2,3);
--4
--tiny patrol
insert into plansections (planid,sectionid) values (3,4);
--5

--big patrol final
insert into plansections (planid,sectionid) values (1,5);

--levels
--big patrol
insert into levels(name,description,patrolid) values ('1','PSIAA Level 1',1);
--1
insert into levels(name,description,patrolid) values ('2/3','PSIAA Level 2/3',1);
--2
insert into levels(name,description,patrolid) values ('Final','Assigned By Coordinator',1);
--3
--tiny patrol
insert into levels(name,description,patrolid) values ('Trainer','',2);
--4

--sectionlevels
--big patrol ski skills
insert into sectionlevels (sectionid,levelid,columnindex) values (1,1,0);
--1
insert into sectionlevels (sectionid,levelid,columnindex) values (1,2,1);
--2
insert into sectionlevels (sectionid,levelid,columnindex) values (1,2,2);
--3
insert into sectionlevels (sectionid,levelid,columnindex) values (5,3,3); --final
--4
--big patrol snowboard skills
insert into sectionlevels (sectionid,levelid,columnindex) values (2,1,0);
--5
insert into sectionlevels (sectionid,levelid,columnindex) values (2,2,1);
--6
insert into sectionlevels (sectionid,levelid,columnindex) values (2,2,2);
--7
insert into sectionlevels (sectionid,levelid,columnindex) values (2,3,3);
--8
--big patrol toboggan skills
insert into sectionlevels (sectionid,levelid,columnindex) values (3,1,0);
--9
insert into sectionlevels (sectionid,levelid,columnindex) values (3,2,1);
--10
insert into sectionlevels (sectionid,levelid,columnindex) values (3,2,2);
--11
--insert into sectionlevels (sectionid,levelid,columnindex) values (3,3,3);
--12
--tiny patrolred jacket
insert into sectionlevels (sectionid,levelid,columnindex) values (4,4,0);
--12

--skills
--big patrol ski skills
insert into skills (name,patrolid) values ('Controlled Parallel Stop (2 Directions)',1);
--1
insert into skills (name,patrolid) values ('Descending Leaves (2 Directions)',1);
--2
insert into skills (name,patrolid) values ('Traverse with Forward Traversing Sideslip (2 Directions)',1);
--3
insert into skills (name,patrolid) values ('Gliding Wedge',1);
--4
insert into skills (name,patrolid) values ('Wedge Sideslip Transition (2 Directions)',1);
--5
insert into skills (name,patrolid) values ('Pivot Sideslip Transitions (2 Directions)',1);
--6
insert into skills (name,patrolid) values ('Confined Area (Parallel Turns)',1);
--7
insert into skills (name,patrolid) values ('Free Skiing',1);
--8
--big patrol snowboard skills
insert into skills (name,patrolid) values ('Controlled Emergency Stop (Toe & Heel Side)',1);
--9
insert into skills (name,patrolid) values ('Descending Leaves (Toe & Heel Side)',1);
--10
insert into skills (name,patrolid) values ('Traverse with Forward Traversing Sideslip (Toe & Heel Side)',1);
--11
insert into skills (name,patrolid) values ('Linked Skidded Turns (Forward)',1);
--12
insert into skills (name,patrolid) values ('Linked Skidded Turns (Switch)',1);
--13
insert into skills (name,patrolid) values ('Pivot Sideslip Transitions (Toe-to-Heel, Heel-to-Toe)',1);
--14
insert into skills (name,patrolid) values ('Confined Area Dynamic Turns',1);
--15
insert into skills (name,patrolid) values ('Free Riding',1);
--16
--big patrol toboggan skills
insert into skills (name,patrolid) values ('Toboggan Approach - Alone',1);
--17
insert into skills (name,patrolid) values ('Loaded Toboggan - Front Operator',1);
--18
insert into skills (name,patrolid) values ('Loaded Toboggan - Tail Rope Operator',1);
--19
--tiny patrol
insert into skills (name,patrolid) values ('Controlled Emergency Stop',2);
--20
insert into skills (name,patrolid) values ('Descending Leaves',2);
--21
insert into skills (name,patrolid) values ('Free Skiing/Riding',2);
--22
insert into skills (name,patrolid) values ('Loaded Toboggan - Front Operator',2);
--23
insert into skills (name,patrolid) values ('Loaded Toboggan - Tail Rope Operator',2);
--24

--sectionskills
--big patrol ski skills
insert into sectionskills (sectionid,skillid,rowindex) values (1,1,0);
--1
insert into sectionskills (sectionid,skillid,rowindex) values (1,2,1);
--2
insert into sectionskills (sectionid,skillid,rowindex) values (1,3,2);
--3
insert into sectionskills (sectionid,skillid,rowindex) values (1,4,3);
--4
insert into sectionskills (sectionid,skillid,rowindex) values (1,5,4);
--5
insert into sectionskills (sectionid,skillid,rowindex) values (1,6,5);
--6
insert into sectionskills (sectionid,skillid,rowindex) values (1,7,6);
--7
insert into sectionskills (sectionid,skillid,rowindex) values (1,8,7);
--8
--big patrol snowboard skills
insert into sectionskills (sectionid,skillid,rowindex) values (2,9,0);
--9
insert into sectionskills (sectionid,skillid,rowindex) values (2,10,1);
--10
insert into sectionskills (sectionid,skillid,rowindex) values (2,11,2);
--11
insert into sectionskills (sectionid,skillid,rowindex) values (2,12,3);
--12
insert into sectionskills (sectionid,skillid,rowindex) values (2,13,4);
--13
insert into sectionskills (sectionid,skillid,rowindex) values (2,14,5);
--14
insert into sectionskills (sectionid,skillid,rowindex) values (2,15,6);
--15
insert into sectionskills (sectionid,skillid,rowindex) values (2,16,7);
--16
--big patrol toboggan skills
insert into sectionskills (sectionid,skillid,rowindex) values (3,17,8);
--17
insert into sectionskills (sectionid,skillid,rowindex) values (3,18,9);
--18
insert into sectionskills (sectionid,skillid,rowindex) values (3,19,10);
--19
--tiny patrol
insert into sectionskills (sectionid,skillid,rowindex) values (4,20,0);
--20
insert into sectionskills (sectionid,skillid,rowindex) values (4,21,1);
--21
insert into sectionskills (sectionid,skillid,rowindex) values (4,22,2);
--22
insert into sectionskills (sectionid,skillid,rowindex) values (4,23,3);
--23
insert into sectionskills (sectionid,skillid,rowindex) values (4,24,4);
--24

--big patrol final
insert into sectionskills (sectionid,skillid,rowindex) values (5,1,0);
--25
insert into sectionskills (sectionid,skillid,rowindex) values (5,2,1);
--26
insert into sectionskills (sectionid,skillid,rowindex) values (5,3,2);
--27
insert into sectionskills (sectionid,skillid,rowindex) values (5,4,3);
--28
insert into sectionskills (sectionid,skillid,rowindex) values (5,5,4);
--29
insert into sectionskills (sectionid,skillid,rowindex) values (5,6,5);
--30
insert into sectionskills (sectionid,skillid,rowindex) values (5,7,6);
--31
insert into sectionskills (sectionid,skillid,rowindex) values (5,8,7);
--32
insert into sectionskills (sectionid,skillid,rowindex) values (5,17,8);
--33
insert into sectionskills (sectionid,skillid,rowindex) values (5,18,9);
--34
insert into sectionskills (sectionid,skillid,rowindex) values (5,19,10);
--35

insert into assignments (planid,userid,assignedat,dueat) values (1,1,getdate(),DATEADD(year, 1, GETDATE()));
--1

insert into signatures (assignmentid,sectionskillid,sectionlevelid,signedbyuserid,signedat) values (1,1,1,2,getdate());
--1
insert into signatures (assignmentid,sectionskillid,sectionlevelid,signedbyuserid,signedat) values (1,1,2,2,getdate());
--2
insert into signatures (assignmentid,sectionskillid,sectionlevelid,signedbyuserid,signedat) values (1,2,1,2,getdate());
--3
insert into signatures (assignmentid,sectionskillid,sectionlevelid,signedbyuserid,signedat) values (1,2,2,2,getdate());
--4

----groups
--big patrol trainers
insert into groups (name,patrolid) values ('trainers',1);
--tiny patrol trainers
insert into groups (name,patrolid) values ('trainers',2);

insert into groupusers (groupid,userid) values (1,2);
insert into groupusers (groupid,userid) values (2,2);

insert into sectiongroups (sectionid,groupid) values (1,1);
insert into sectiongroups (sectionid,groupid) values (2,1);
insert into sectiongroups (sectionid,groupid) values (3,1);
insert into sectiongroups (sectionid,groupid) values (4,2);

--shifts
insert into shifts (patrolid,name,starthour,startminute,endhour,endminute) values (1,'Morning Shift',9,0,13,0);
--1
insert into shifts (patrolid,name,starthour,startminute,endhour,endminute) values (1,'Afternoon Shift',12,0,18,0);
--2
insert into shifts (patrolid,name,starthour,startminute,endhour,endminute) values (2,'Day Shift',9,0,16,0);
--3

--groups for shift
--big patrol shift group
insert into groups (name,patrolid) values ('Patrollers',1);
--3
insert into groupusers (groupid,userid) values (3,1);
insert into groupusers (groupid,userid) values (3,2);
insert into groupusers (groupid,userid) values (3,3);

--tiny patrol shift group
insert into groups (name,patrolid) values ('Patrollers',2);
--4
insert into groupusers (groupid,userid) values (4,1);
insert into groupusers (groupid,userid) values (4,2);
insert into groupusers (groupid,userid) values (4,3);


--training shift
--big patrol
insert into scheduledshifts (patrolid,startsat,endsat,shiftid,groupid) values (1,DATETIMEFROMPARTS(2001,1,1,9,0,0,0),DATETIMEFROMPARTS(2001,1,1,13,0,0,0),1,3);
--1
insert into scheduledshiftassignments (scheduledshiftid,assigneduserid,originalassigneduserid,status) values (1,1,1,'0');
insert into scheduledshiftassignments (scheduledshiftid,assigneduserid,originalassigneduserid,status) values (1,2,2,'1');
insert into scheduledshiftassignments (scheduledshiftid,assigneduserid,originalassigneduserid,status,claimedbyuserid) values (1,3,3,'2',2);
--1

--tiny patrol
insert into scheduledshifts (patrolid,startsat,endsat,shiftid,groupid) values (2,DATETIMEFROMPARTS(2001,1,1,9,0,0,0),DATETIMEFROMPARTS(2001,1,1,16,0,0,0),3,4);
--2
insert into scheduledshiftassignments (scheduledshiftid,assigneduserid,originalassigneduserid,status) values (2,1,1,'0');
insert into scheduledshiftassignments (scheduledshiftid,assigneduserid,originalassigneduserid,status) values (2,2,2,'1');
insert into scheduledshiftassignments (scheduledshiftid,assigneduserid,originalassigneduserid,status,claimedbyuserid) values (2,3,3,'2',2);
--2


--sanity check
--select
--p.name patrol,
--pl.name [plan],
--s.name [section],
--sk.name [skill],
--l.name [level]
--from patrols p
--inner join plans pl on pl.patrolid=p.id
--inner join plansections ps on ps.planid=pl.id
--inner join sections s on s.id=ps.sectionid
--inner join sectionskills ss on ss.sectionid=s.id
--inner join skills sk on sk.id=ss.skillid
--inner join sectionlevels sl on sl.sectionid=s.id
--inner join levels l on l.id=sl.levelid
--order by p.id,pl.id,s.id,sk.id,l.id
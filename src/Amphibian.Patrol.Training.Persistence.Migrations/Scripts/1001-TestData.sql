--this user is not migrated/deployed to the server db, but i suppose someone will try anyway...
insert into users(email,firstname,lastname,passwordsalt,passwordhash,passwordhashiterations)
values ('test','test','test',0xAC174909DBF93DCE1BC0AD2880391A8BACA2207D84FBBD89DB6D5500A20C7DB0,0x2DC0CDA8863338A36E3EDD1E41238B2542252876C4547C5C860F8D0F47A2B832,5);
--1

insert into patrols(name) values ('Big Mountain Patrol');
--1
insert into patrols(name) values ('Tiny Mountain Patrol');
--2

insert into patrolusers (patrolid,userid) values (1,1);
--1
insert into patrolusers (patrolid,userid) values (2,1);
--2

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
insert into sections (name,patrolid) values ('Ski Skills',1);
--1
insert into sections (name,patrolid) values ('Snowboard Skills',1);
--2
insert into sections (name,patrolid) values ('Toboggan Skills',1);
--3
--tiny patrol
insert into sections (name,patrolid) values ('Red Jacket',2);
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
insert into sectionlevels (sectionid,levelid,[order]) values (1,1,1);
--1
insert into sectionlevels (sectionid,levelid,[order]) values (1,2,2);
--2
insert into sectionlevels (sectionid,levelid,[order]) values (1,2,3);
--3
insert into sectionlevels (sectionid,levelid,[order]) values (1,3,4);
--4
--big patrol snowboard skills
insert into sectionlevels (sectionid,levelid,[order]) values (2,1,1);
--5
insert into sectionlevels (sectionid,levelid,[order]) values (2,2,2);
--6
insert into sectionlevels (sectionid,levelid,[order]) values (2,2,3);
--7
insert into sectionlevels (sectionid,levelid,[order]) values (2,3,4);
--8
--big patrol toboggan skills
insert into sectionlevels (sectionid,levelid,[order]) values (3,1,1);
--9
insert into sectionlevels (sectionid,levelid,[order]) values (3,2,2);
--10
insert into sectionlevels (sectionid,levelid,[order]) values (3,2,3);
--11
insert into sectionlevels (sectionid,levelid,[order]) values (3,3,4);
--12
--tiny patrolred jacket
insert into sectionlevels (sectionid,levelid,[order]) values (4,4,1);
--13

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
insert into sectionskills (sectionid,skillid,[order]) values (1,1,1);
--1
insert into sectionskills (sectionid,skillid,[order]) values (1,2,2);
--2
insert into sectionskills (sectionid,skillid,[order]) values (1,3,3);
--3
insert into sectionskills (sectionid,skillid,[order]) values (1,4,4);
--4
insert into sectionskills (sectionid,skillid,[order]) values (1,5,5);
--5
insert into sectionskills (sectionid,skillid,[order]) values (1,6,6);
--6
insert into sectionskills (sectionid,skillid,[order]) values (1,7,7);
--7
insert into sectionskills (sectionid,skillid,[order]) values (1,8,8);
--8
--big patrol snowboard skills
insert into sectionskills (sectionid,skillid,[order]) values (2,9,1);
--9
insert into sectionskills (sectionid,skillid,[order]) values (2,10,2);
--10
insert into sectionskills (sectionid,skillid,[order]) values (2,11,3);
--11
insert into sectionskills (sectionid,skillid,[order]) values (2,12,4);
--12
insert into sectionskills (sectionid,skillid,[order]) values (2,13,5);
--13
insert into sectionskills (sectionid,skillid,[order]) values (2,14,6);
--14
insert into sectionskills (sectionid,skillid,[order]) values (2,15,7);
--15
insert into sectionskills (sectionid,skillid,[order]) values (2,16,8);
--16
--big patrol toboggan skills
insert into sectionskills (sectionid,skillid,[order]) values (3,17,1);
--17
insert into sectionskills (sectionid,skillid,[order]) values (3,18,2);
--18
insert into sectionskills (sectionid,skillid,[order]) values (3,19,3);
--19
--tiny patrol
insert into sectionskills (sectionid,skillid,[order]) values (4,20,1);
--20
insert into sectionskills (sectionid,skillid,[order]) values (4,21,2);
--21
insert into sectionskills (sectionid,skillid,[order]) values (4,22,3);
--22
insert into sectionskills (sectionid,skillid,[order]) values (4,23,4);
--23
insert into sectionskills (sectionid,skillid,[order]) values (4,24,5);
--24

insert into assignments (planid,userid,assignedat,dueat) values (1,1,getdate(),DATEADD(year, 1, GETDATE()));

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
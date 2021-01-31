# Patrol.Ski
## An app for ski patrol.  Free for volunteer patrols.
### Administrator/Coordinator User Guide

------

[TOC]

---

# 1. Creating your patrol

You can create a patrol after signing in the first time, or after clicking the patroller menu in the top right and selecting "New Patrol" 

![Screenshot of new patrol](screenshot({url:'#/app/admin/new-patrol',element:'#new-patrol'}))

### 1.1 Name and Time Zone

Name your patrol and specify the local time zone for your patrol.  

**Times in patrol.ski are almost always shown in the local time zone for the current users device.**  The only exceptions are on administrative screens to manage repeated shifts and shift recurring work item times, which always use the patrol's local time.

### 1.2 Initial Setup

Initial setup provides some time saving functions to quickly create some things for your patrol.  
 - Default creates alpine ski and alpine snowboard training plans with user groups.
 - Empty creates nothing, you must create everything including new training plans, user groups, shifts etc.
 - Demo creates a new patrol with extensive configuration and sample data for all functions.   This is helpful just to see how things work and provide you with a "sandbox" to learn how the system works.

### 1.3 Functionality

Enable or disable each functionality for the patrol.  These may also be changed later on the patrol settings page.  Some elements overlap multiple functionalities, for example the ability to have trainees "sign up" to train with trainers depends on having both training and scheduling functionality enabled, such overlaps are outlined below.

- **Announcements**
  Administrators and Coordinators can create rich text announcements which are shown on the patroller dashboard.  Announcements may also be emailed to the entire patrol.
- **Events**
  Events are shown on the patroller calendar as well as in the "upcoming events" section of the patroller dashboard.
- **Training**
  Training enables creation and management of training plans as well as the ability to assign training to patrollers.  If scheduling is also enabled, trainees will be able to see which trainers can help them complete training in the "Upcoming Trainer Shifts" section on the dashboard, and they may sign up to train with the trainers.  If scheduling is enabled and a trainer releases a shift, any planned training will be cancelled and trainees notified.
- **Scheduling**
  Scheduling allows Administrators to define regular shift hours and administrators and coordinators can both create and maintain the schedule.  If scheduling and training are both enabled, trainees may sign up to train with trainers on the trainers shift.  If Shift exchange is enabled, "Available" shifts may be added to the schedule for patrollers to claim.  If Work Items are also enabled, recurring work items may be made which occur on every shift.
- **Shift Exchange**
  Shift exchange allows patrollers to release and claim shifts.  Administrators or Coordinators must approve shift changes.  If Work items are enabled and a recurring work item is assigned to a user during a shift which is swapped, the recurring work item will be moved to the newly assigned patroller.  If shift exchange is enabled "Available" shifts are allowed to be scheduled for patrollers to claim.
- **Time Clock**
  If Timeclock is enabled patrollers may clock in/out to track hours worked.  If Timeclock and Scheduling are both enabled, the dashboard sections "who's on" and "who's late/mia" are shown.  If timeclock and scheduling are both enabled the administrator report "Time Missing" is available to show actual hours clocked in vs scheduled.
- **Work Items**
  Administrators can create recurring work items, coordinators and administrators can create one-off work items.  Assigned/Due work items are displayed on the patroller dashboard. If scheduling is enabled, recurring work items can be assigned to a shift and automatically assigned to patrollers within the scheduled shift.

---

# 2. People

![Screenshot of new patrol](screenshot({url:'#/app/admin/people',element:'#people', height: 600}))

Although individual people own their own accounts, as a patrol administrator you may add people to your patrol and control their access and abilities inside your patrol.  Email addresses are the primary unique key for all people, and as such you cannot change an email address for a person once an account is created.  However, you may simply add a new person with the desired email address.  People may belong to multiple patrols with the same email/account.  As such, removing a person from one patrol will not remove them from other patrols.  If a person chooses to delete their own account, you may see "removed, removed" as a person in your patrol.  All personally identifying information has been removed from their account, however, they will still be shown in your patrol in situations such as past schedules, past work items, past time clock entries etc.

### 2.1 Managing People

![Screenshot of new patrol](screenshot({url:'#/app/admin/user/null',element:'#edit-user'}))

To add a person to your patrol use the navigation bar to go to the "people" option.  Click "New" in the top right corner of the list and enter the person's information.  Be sure of their email address, as it cannot be changed later.  Select a patrol role (or none) (more in 2.2) and relevant groups (more in 3).

### 2.2 Patrol Roles

Patrol roles grant the user ability to manage aspect of the patrol outside of normal patroller usage. Typical patrollers will have "None" as their patrol role.   See the below table to understand the differences between the roles.



| Permission                 | None (Typical Patrollers) | Coordinator | Administrator |
| -------------------------- | ------------------------- | ----------- | ------------- |
| Maintain Training Plans    |                           |             | Yes           |
| Maintain Users             |                           |             | Yes           |
| Maintain Groups            |                           |             | Yes           |
| Maintain Assignments       |                           | Yes         | Yes           |
| Revoke Training Signatures |                           | Yes         | Yes           |
| Maintain Patrol Settings   |                           |             | Yes           |
| Maintain Announcements     |                           | Yes         | Yes           |
| Maintain Events            |                           | Yes         | Yes           |
| Maintain Schedule          |                           | Yes         | Yes           |
| Maintain TimeClock         |                           | Yes         | Yes           |
| Maintain Work Items        |                           | Yes         | Yes           |

------



# 3. Groups

![Screenshot of new patrol](screenshot({url:'#/app/admin/groups',element:'#groups', height:600}))

Groups are simply groups of people which you may use to organize functionality within your patrol.  

Some examples of groups:

- Groups of different levels of trainers who are allowed to sign off on different sections of skills, eg: PSIA Level 1, PSIA Level 2, Toboggan, Final Ski Along, etc.
- Groups of people who share a schedule, eg "Monday Night Crew", "D Crew", "Bill's Crew"
- Groups of people who are in charge of certain kinds of work items, eg: "Crew Chiefs", "Equipment Owners", "Toboggan Owners"

Groups are used by other functionalities such as scheduling, training, and work items and do not provide functionality on their own.

------

# 4. Schedule

### 4.1 Shifts

![Screenshot of new patrol](screenshot({url:'#/app/admin/shifts',element:'#shifts', height:600}))

A shift is simply a range of hours eg: 9-5, that is repeated multiple times in a schedule.  It is not necessary to define shifts in order to create a schedule, however by defining shifts it will make it quicker and easier to build your schedule.  Additionally, by defining shifts you can allow your patrol to know their schedule by a shared nomenclature eg: "morning shift" or "late night shift".  If you choose to also leverage the work item functionality, you may optionally define recurring work items that occur with every shift, examples include "morning shift toboggan check" or "evening shift sweep".

![Screenshot of new patrol](screenshot({url:'#/app/admin/edit-shift/null',element:'#edit-shift'}))

To define a new shift, use the navigation menu to click "Shifts" and then click "New" in the top right corner.  Enter a name, start and end time (in patrol local time zone) and click save.

### 4.2 Creating a Schedule

![Screenshot of new patrol](screenshot({url:'#/app/calendar',element:'#calendar'}))

To make creating a schedule easier, it is best (but not required) to define your shifts as in 4.1.  Using the navigation sidebar, click Calendar.  Click inside any future calendar date.  To set the start/end time you may either A) select a shift, or B) select "(New)" and manually specify start & end times.  To add people select a group and/or select them in the dropdown and click "Add".  If Schedule swap is enabled for the patrol you may also add "(Available)" to the schedule which creates a scheduled shift assignment that can be claimed by other patrollers.  Click save to schedule the shift.

To modify an existing scheduled shift, click the shift in the calendar.  You may add or remove people to the scheduled shift, or cancel the shift entirely. 

### 4.3 Repeating a Schedule

![Screenshot of new patrol](screenshot({url:'#/app/calendar',element:'#copy-range',circle:true, width:400, height:300}))

To copy a date range of schedule to a new date range, click "Copy Date Range" below the Calendar.  Select the date range you want to copy in the "From Start" and "From End" date fields, and the range you want to copy to in the "To Start" and "To End" fields.

![Screenshot of new patrol](screenshot({url:'#/app/admin/repeat-schedule',element:'#copy'}))

If you enable "Clear To First" any existing shifts between "To Start" and "To End" will be removed

If you enable "Test Only", when you click "Copy" it will simply show you what it WOULD do, but not actually do it.  

Some example use cases include copying one week to another, copying one month to another, repeating a sequence of days over  an extended date range.

### 4.4 Swap Approval

![Screenshot of new patrol](screenshot({url:'#/app/admin/swap-approval',element:'#swap-approval'}))

To approve schedule swaps, use the navigation bar and click "Swap Approval".  An administrator or coordinator must approve all swaps before they become final.  Rejected swaps will revert back to their "released" state so that other patrollers may claim them.

------

# 5. Timeclock

### 5.1 Clocking In/Out

![Screenshot of new patrol](screenshot({url:'#/app',element:'#timeclock'}))

Patrollers may clock in/out on the patrol dashboard.  A current scheduled shift is not required, even if scheduling is enabled.

### 5.2 Who's On

![Screenshot of new patrol](screenshot({url:'#/app',element:'#whos-on'}))

The Who's on dashboard section will show a list of currently clocked-in patrollers.

### 5.3 Who's Late/MIA

![Screenshot of new patrol](screenshot({url:'#/app',element:'#whos-late'}))

If Scheduling is enabled, the "Who's Late/MIA" dashboard section will show a list of currently scheduled patrollers who are not clocked in.

### 5.4 Time/Days

![Screenshot of new patrol](screenshot({url:'#/app/admin/time-days',element:'#time-days'}))

The Time/Days report shows hours worked by day for a date range and the selected patroller.  If scheduling is enabled it will also show the scheduled hours and the scheduled hours worked (time spent clocked in during the scheduled hours).

### 5.5 Time Missing

![Screenshot of new patrol](screenshot({url:'#/app',element:'#time-missing'}))

If scheduling is enabled, the time missing report will show time missed due to being late, leaving early, or being clocked out during a scheduled shift.

------

# 6. Announcements

![Screenshot of new patrol](screenshot({url:'#/app/admin/announcements',element:'#announcements'}))

Announcements are shown on the patrol dashboard and may be emailed to the entire patrol.  To create a new announcement click "Announcements" on the navigation bar and then click "New".  You must specify a subject, a date/time to begin showing the announcement, and a date time to expire (stop showing) the announcement.  You may specify to email it to the entire patrol and provide formatted description of the announcement.

------

# 7. Events

------

# 8. Work Items

### 8.1 Creating a work item

### 8.2 Completion

### 8.3 Ownership

### 8.4 Assignment

### 8.5 Recurring work items

### 8.6 Recurring by time interval

### 8.7 Recurring per shift

------



# 9. Training

### 9.1 Plans

### 9.2 Creating/Editing a Training Plan

1. Sections
2. Skills/Rows
3. Levels/Column

### 9.3 Assignments

### 9.4 Creating an assignment

### 9.5 Completing

### 9.6 Training Assignments

### 9.7 Dashboard
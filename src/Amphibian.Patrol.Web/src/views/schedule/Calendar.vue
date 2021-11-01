<template>
    <div>
        <CCard id="calendar">
            <CCardHeader>
                <slot name="header">
                    <CIcon name="cil-calendar"/>
                </slot>
                Patrol Calendar
            </CCardHeader>
            <CCardBody>
                <CRow style="height:600px;">
                    <CCol>
                        <calendar-view
                        :show-date="selectedDate"
                        class="theme-default holiday-us-traditional holiday-us-official"
                        :events="calendarEvents"
                        :period-changed-callback="periodChanged"
                        :disable-past="false"
                        :disable-future="false"
                        :displayPeriodUom="displayPeriod"
                        :enable-drag-drop="false"
                        :enable-date-selection="false"
                        @click-event="clickEvent"
                        @click-date="clickDate">
                        <calendar-header
                            slot="header"
                            slot-scope="t"
                            :header-props="t.headerProps"
                            @input="setShowDate"/>
                        </calendar-view>
                    </CCol>
                </CRow>
                <CRow>
                    <CCol md="3" lg="3" sm="12">
                        <CSelect
                        :options="['month', 'week']"
                        :value.sync="displayPeriod"
                        label="View"
                        add-wrapper-classes="ml-2"
                        />
                    </CCol>
                    <CCol md="3" lg="3" sm="12">
                        <CSelect
                        :options="userItems"
                        :value.sync="viewUserId"
                        label="Person"
                        add-wrapper-classes="ml-2"
                        />
                    </CCol>
                    <CCol md="3" lg="3" sm="12">
                        <label></label><br/>
                        <CButton id="copy-range" color="success" :to="{name:'RepeatSchedule'}" v-if="hasPermission('MaintainSchedule')">Copy Date Range</CButton>
                    </CCol>
                </CRow>
            </CCardBody>
            <CCardFooter>

            </CCardFooter>
        </CCard>
        <CModal
        :title="selectedEvent.name"
        :show.sync="showSelectedEvent"
        :no-close-on-backdrop="true"
        color="success"
        size="lg"
        >
        {{(new Date(selectedEvent.startsAt)).toLocaleString()}} - {{(new Date(selectedEvent.endsAt)).toLocaleString()}}<br/>
        {{selectedEvent.name}}<br/>
        Location: {{selectedEvent.location}}<br/>
        <hr/>
        <div style="overflow:hidden;" v-html="selectedEvent.eventHtml"/>

        <template #header>
        </template>
        <template #footer>
            <CButton @click="showSelectedEvent = false" color="info">Close</CButton>
        </template>
        </CModal>
        <CModal
        :show.sync="showSelectedShift"
        :no-close-on-backdrop="true"
        color="primary"
        size="lg"
        >
            <strong>{{selectedShift.shiftName}}</strong><br/>
            Start:{{(new Date(selectedShift.startsAt)).toLocaleString()}}<br/>
            End:{{(new Date(selectedShift.endsAt)).toLocaleString()}}<br/>
            <template v-if="selectedShift.groupName">
                <em>{{selectedShift.groupName}}</em><br/>
            </template>
            <CDataTable
                striped
                small
                fixed
                :items="selectedShift.assignments"
                :fields="[{key:'assignedUser',label:''},{key:'claimedByUser',label:''},{key:'status',label:''},{key:'traineeCount',label:''},{key:'buttons',label:''}]"
                >
                <template #assignedUser="data">
                    <td v-if="data.item.assignedUser">{{data.item.assignedUser.lastName}}, {{data.item.assignedUser.firstName}}</td>
                    <td v-if="!data.item.assignedUser"><c-badge color="warning">Available</c-badge></td>
                </template>
                <template #claimedByUser="data">
                    <td>
                        <template v-if="data.item.status=='Claimed'">
                            {{data.item.claimedByUser.lastName}}, {{data.item.claimedByUser.firstName}}
                        </template>
                    </td>
                </template>
                <template #status="data">
                    <td>
                        <template v-if="data.item.assignedUser">
                            <template v-if="data.item.status=='Claimed'">
                                <c-badge color="warning">Pending Approval</c-badge>
                            </template>
                            <template v-if="data.item.status=='Released'">
                                <c-badge color="warning">Released</c-badge>
                            </template>
                        </template>
                    </td>
                </template>
                <template #traineeCount="data">
                    <td><span v-if="data.item.traineeCount>0"><c-badge size="sm" color="info">{{data.item.traineeCount}} Trainees</c-badge></span></td>
                </template>
                <template #buttons="data">
                    <td>
                        <CButtonGroup class="float-right" v-if="isFuture(selectedShift.startsAt)">
                            
                            <CButton size="sm" color="primary" v-if="selectedPatrol.enableShiftSwaps && data.item.status=='Assigned' && data.item.assignedUser && data.item.assignedUser.id == userId" @click="releaseScheduledShiftAssignment(data.item)">Release</CButton>
                            <CButton size="sm" color="success" v-if="selectedPatrol.enableShiftSwaps && data.item.status=='Released' && (!data.item.assignedUser || data.item.assignedUser.id != userId)" @click="claimScheduledShiftAssignment(data.item)">Claim</CButton>
                            <CButton size="sm" color="info" v-if="selectedPatrol.enableShiftSwaps && (data.item.status=='Claimed' || data.item.status=='Released') && data.item.assignedUser && data.item.assignedUser.id == userId" @click="cancelReleaseScheduledShiftAssignment(data.item)">Cancel Release</CButton>
                            <CButton size="sm" color="success" v-if="selectedPatrol.enableShiftSwaps && data.item.status=='Claimed' && hasPermission('MaintainSchedule')" @click="approveScheduledShiftAssignment(data.item)">Approve</CButton>
                            <CButton size="sm" color="warning" v-if="selectedPatrol.enableShiftSwaps && data.item.status=='Claimed' && hasPermission('MaintainSchedule')" @click="rejectScheduledShiftAssignment(data.item)">Reject</CButton>
                            <CButton size="sm" color="danger" v-if="hasPermission('MaintainSchedule')" @click="removeScheduledShiftAssignment(selectedShift,data.item)">Remove</CButton>
                        </CButtonGroup>
                    </td>
                </template>
                <template #footer="data">
                    <template v-if="isFuture(selectedShift.startsAt) && hasPermission('MaintainSchedule')">
                        <tfoot>
                            <tr v-if="filteredUserList && filteredUserList.length>0">
                                <td colspan="4"><CSelect :options="filteredUserList" :value.sync="selectedUserId"></CSelect></td>
                                <td><CButton @click="addScheduledShiftAssignment(selectedShift,selectedUserId)" size="sm" color="success" class="float-right">Add</CButton></td>
                            </tr>
                        </tfoot>
                    </template>
                </template>
            </CDataTable>
            <template #header>Shift Details
            </template>
            <template #footer>
                <CButtonGroup class="float-right">
                    <CButton @click="cancelShift(selectedShift)" v-if="hasPermission('MaintainSchedule') && isFuture(selectedShift.startsAt)" color="warning">Cancel Shift</CButton>
                    <CButton @click="showSelectedShift = false" color="primary">Close</CButton>
                </CButtonGroup>
            </template>
        </CModal>
        <CModal
        :show.sync="newShift.show"
        :no-close-on-backdrop="true"
        color="primary"
        size="lg"
        >

            <template v-if="shifts.length>0">
                <CRow>
                    <CCol md="2">
                        <label for="newShift.shiftId">Shift</label><br/>
                    </CCol>
                    <CCol md="10">
                        <CSelect :options="shiftItems" :value.sync="newShift.shiftId"></CSelect>
                    </CCol>
                </CRow>
                <CRow v-if="newShift.shiftId!=null">
                    <CCol md="2">
                        <label for="newShift.day">Day</label>
                    </CCol>
                    <CCol md="10">
                        <datetime type="date" v-model="newShift.day" :minute-step="5" input-class="form-control"></datetime><br/>
                    </CCol>
                </CRow>
            </template>

            <CRow v-if="newShift.shiftId==null">
                <CCol>
                    <label for="newShift.startsAt">Start</label>
                    <datetime type="datetime" v-model="newShift.startsAt" :minute-step="15" input-class="form-control" :use12-hour="true"></datetime><br/>
                </CCol>
                <CCol>
                    <label for="newShift.endsAt">End</label>
                    <datetime type="datetime" v-model="newShift.endsAt" :minute-step="15" input-class="form-control" :use12-hour="true"></datetime><br/>
                </CCol>
            </CRow>

            <CRow>
                <CCol md="2">
                    <label for="newShift.shiftId">Group</label>
                </CCol>
                <CCol md="10">
                    <CSelect :options="groupItems" :value.sync="newShift.groupId"></CSelect>
                </CCol>
            </CRow>
            <CRow v-if="newShift.groupId!=null">
                <CCol md="2">
                </CCol>
                <CCol md="10">
                    <em>Members of {{getGroupById(newShift.groupId).name}} will be added to the scheduled shift</em>
                </CCol>
            </CRow>
            <CRow>
                <CCol md="12">
                    <CDataTable
                        striped
                        small
                        fixed
                        :items="newShift.assignments"
                        :fields="[{key:'assignedUser',label:''},{key:'buttons',label:''}]"
                        >
                        <template #assignedUser="data">
                            <td v-if="data.item.assignedUser.id">{{data.item.assignedUser.lastName}}, {{data.item.assignedUser.firstName}}</td>
                            <td v-if="!data.item.assignedUser.id">Available</td>
                        </template>
                        <template #buttons="data">
                            <td>
                                <CButtonGroup class="float-right">
                                    <CButton size="sm" color="danger" @click="removeUserFromNewShift(data.item.assignedUser.id)">Remove</CButton>
                                </CButtonGroup>
                            </td>
                        </template>
                        <template #footer="data">
                            <tfoot>
                                <tr v-if="filteredUserList && filteredUserList.length>0">
                                    <td><CSelect :options="filteredUserList" :value.sync="selectedUserId"></CSelect></td>
                                    <td><CButton size="sm" color="success" class="float-right" @click="addUserToNewShift(selectedUserId)">Add</CButton></td>
                                </tr>
                            </tfoot>
                        </template>
                        <template #no-items-view>
                            <span v-if="newShift.groupId!=null">Add additional people...</span>
                            <span v-if="newShift.groupId==null">Add people...</span>
                        </template>
                    </CDataTable>
                </CCol>
            </CRow>
            <template #header>Schedule New Shift
            </template>
            <template #footer>
                <CButtonGroup class="float-right">
                    <CButton @click="newShift.show = false" color="info">Cancel</CButton>
                    <CButton :disabled="!isNewShiftValid()" @click="scheduleShift()" color="primary">Save</CButton>
                </CButtonGroup>
            </template>
        </CModal>
    </div>
</template>

<script>
import { freeSet } from '@coreui/icons'

import { CalendarView } from "vue-simple-calendar"
// The next two lines are processed by webpack. If you're using the component without webpack compilation,
// you should just create <link> elements for these. Both are optional, you can create your own theme if you prefer.
//require("vue-simple-calendar/static/css/default.css")
//require("vue-simple-calendar/static/css/holidays-us.css")

import CalendarHeader from './CalendarHeader';

import { Datetime } from 'vue-datetime';
import 'vue-datetime/dist/vue-datetime.css';

export default {
  name: 'Calendar',
  freeSet,
  components: {  CalendarView,
            CalendarHeader,
            Datetime
  },
  props: [],
  data () {
    return {
        selectedDate:new Date(),
        from:new Date(),
        to:new Date(),
        events:[],
        scheduledShifts:[],
        users:[],
        displayPeriod:'month',
        viewUserId: null,

        shifts:[],
        groups:[],
        
        showSelectedEvent:false,
        selectedEvent:{},

        showSelectedShift:false,
        selectedShift:{},

        selectedUserId:0,
        filteredUserList:[],

        newShift:{
            show: false,
            startsAt: new Date().toUTCString(),
            endsAt: new Date().toUTCString(),
            day: new Date().toUTCString(),
            shiftId: 0,
            groupId: null,
            patrolId: this.selectedPatrolId,
            assignments:[]
        }
    }
  },
  props: ['uid'],
  methods: {
        getGroupById(id){
            return _.find(this.groups,{id:this.newShift.groupId});
        },
        addScheduledShiftAssignment(shift,userId){
            this.$store.dispatch('loading','Loading...');
            this.$http.post('schedule/scheduled-shift-assignment?scheduledShiftId='+shift.scheduledShiftId+(userId ? "&userId="+userId : ""))
                .then(response => {
                    console.log(response);
                    this.scheduledShifts = _.filter(this.scheduledShifts,function(s){return s.scheduledShiftId!=shift.scheduledShiftId});
                    var updatedShift = response.data;
                    this.scheduledShifts.push(updatedShift[0]);
                    this.selectedShift = updatedShift[0];
                    this.filterUserListItems(this.selectedShift.assignments);
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        removeScheduledShiftAssignment(shift,assignment){
            this.$store.dispatch('loading','Loading...');
            this.$http.delete('schedule/scheduled-shift-assignment?scheduledShiftAssignmentId='+assignment.id)
                .then(response => {
                    console.log(response);
                    shift.assignments = _.filter(shift.assignments,function(a){return a.id!=assignment.id;});
                    this.filterUserListItems(shift.assignments);
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        releaseScheduledShiftAssignment(assignment){
            this.$store.dispatch('loading','Loading...');
            this.$http.post('schedule/scheduled-shift-assignment/release?scheduledShiftAssignmentId='+assignment.id)
                .then(response => {
                    console.log(response);
                    assignment.status='Released';
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        cancelReleaseScheduledShiftAssignment(assignment){
            this.$store.dispatch('loading','Loading...');
            this.$http.post('schedule/scheduled-shift-assignment/cancel-release?scheduledShiftAssignmentId='+assignment.id)
                .then(response => {
                    console.log(response);
                    assignment.status='Assigned';
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        claimScheduledShiftAssignment(assignment){
            this.$store.dispatch('loading','Loading...');
            this.$http.post('schedule/scheduled-shift-assignment/claim?scheduledShiftAssignmentId='+assignment.id)
                .then(response => {
                    console.log(response);
                    assignment.status='Claimed';
                    assignment.claimedByUser = response.data.claimedByUser;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        approveScheduledShiftAssignment(assignment){
            this.$store.dispatch('loading','Loading...');
            this.$http.post('schedule/scheduled-shift-assignment/approve?scheduledShiftAssignmentId='+assignment.id)
                .then(response => {
                    console.log(response);
                    assignment.status='Assigned';
                    assignment.assignedUser = assignment.claimedByUser;
                    assignment.claimedByUser = null;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        rejectScheduledShiftAssignment(assignment){
            this.$store.dispatch('loading','Loading...');
            this.$http.post('schedule/scheduled-shift-assignment/reject?scheduledShiftAssignmentId='+assignment.id)
                .then(response => {
                    console.log(response);
                    assignment.status='Released';
                    assignment.claimedByUser = null;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        cancelShift(shift){
            this.$store.dispatch('loading','Loading...');
            this.$http.delete('schedule/scheduled-shift?scheduledShiftId='+shift.scheduledShiftId)
                .then(response => {
                    console.log(response);
                    this.showSelectedShift=false;
                    this.getScheduledShifts();
                    
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        isFuture(date){
            return new Date(date) > new Date();
        },
        periodChanged(range){
            this.from=range.displayFirstDate;
            this.to=range.displayLastDate;
            this.getEvents();
            this.getScheduledShifts();
        },
        clickEvent(event){
            console.log(JSON.stringify(event));
            if(event.originalEvent.type=='event'){
                var e = _.find(this.events,{id:event.id});
                this.selectedEvent = e;
                this.showSelectedEvent=true;
            }
            else if(event.originalEvent.type=='shift'){
                var e = _.find(this.scheduledShifts,{scheduledShiftId:event.id});
                this.selectedShift = e;

                if(this.selectedShift.assignments){
                    this.selectedShift.assignments = _.sortBy(this.selectedShift.assignments,function(a){
                        if(a.assignedUser){
                            return a.assignedUser.lastName+", "+a.assignedUser.firstName;
                        }
                        else if(a.claimedByUser){
                            return "ZZZZZZZZZZZZZZZZZZZZZZZZ"+a.claimedByUser.lastName+", "+a.claimedByUser.firstName;
                        }
                        else{
                            return null;
                        }
                    });
                }
                else{
                    this.selectedShift.assignments=[];
                }

                this.filterUserListItems(this.selectedShift.assignments);
                this.showSelectedShift=true;
            }
            //else: shifts etc
        },
        clickDate(date){
            if(this.isFuture(date) && this.hasPermission('MaintainSchedule')){
                console.log(JSON.stringify(event));

                this.newShift.groupId = null;
                this.newShift.patrolId= this.selectedPatrolId;
                this.newShift.assignments.splice(0,this.newShift.assignments.length);

                this.filterUserListItems(this.newShift.assignments);
                
                if(this.shifts.length>0){
                    this.newShift.shiftId = this.shifts[0].id;
                    this.newShift.day = date.toUTCString();
                    this.newShift.startsAt = date.toUTCString();
                    this.newShift.endsAt = date.toUTCString();
                }
                else {
                    this.newShift.shiftId = null;
                    this.newShift.day = date.toUTCString();
                    this.newShift.startsAt = date.toUTCString();
                    this.newShift.endsAt = date.toUTCString();
                }

                this.newShift.show=true;
            }
        },
        addUserToNewShift(userId){
            var user = {id: null};
            if(userId){
                user = _.find(this.users,{id:userId});
            }
            this.newShift.assignments.push({
                assignedUser:user
            });
            this.filterUserListItems(this.newShift.assignments);
        },
        removeUserFromNewShift(userId){
            var lastIndex = _.findLastIndex(this.newShift.assignments,function(a){return a.assignedUser.id==userId;});
            this.newShift.assignments.splice(lastIndex,1);
            //this.newShift.assignments = _.filter(this.newShift.assignments,function(a){return a.assignedUser.id!=userId;});
            this.filterUserListItems(this.newShift.assignments);
        },
        isNewShiftValid(){
            return (this.newShift.groupId > 0 || this.newShift.assignments.length>0) // people are selected
            && (
                //a day and shift are selected
                (this.newShift.shiftId!=null && this.newShift.day!=null && this.isFuture(this.newShift.day))
                //a start and end are selected
                || (this.newShift.shiftId==null && this.newShift.startsAt!=null && this.newShift.endsAt!=null && this.isFuture(this.newShift.startsAt) && this.isFuture(this.newShift.endsAt))
            );
        },
        scheduleShift(){

            this.newShift.assignUserIds = _.map(this.newShift.assignments,function(a){return a.assignedUser.id;});
            this.newShift.patrolId = this.selectedPatrolId;

            if(this.newShift.shiftId==null){
                this.newShift.day=null;
                this.newShift.startsAt = this.newShift.startsAt;
                this.newShift.endsAt = this.newShift.endsAt;
            }
            else{
                this.newShift.day = this.newShift.day;
                this.newShift.startsAt = null;
                this.newShift.endsAt = null;
            }

            this.$store.dispatch('loading','Loading...');
            this.$http.post('schedule/scheduled-shift',this.newShift)
                .then(response => {
                    console.log(response);
                    this.newShift.show=false;
                    this.getScheduledShifts();
                    
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        setShowDate(d) {
            this.selectedDate = d;
        },
        getEvents() {
            this.$store.dispatch('loading','Loading...');
            this.$http.post('events/search',{patrolId:this.selectedPatrolId,from:this.from,to:this.to, isInternal:true,isPublic:false})
                .then(response => {
                    console.log(response);
                    this.events = response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
            },
        getScheduledShifts() {
            this.$store.dispatch('loading','Loading...');
            this.$http.post('schedule/search',{patrolId:this.selectedPatrolId,from:this.from,to:this.to,userId:this.viewUserId})
                .then(response => {
                    console.log(response);
                    this.scheduledShifts = response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
            },
        getUsers() {
            this.$store.dispatch('loading','Loading...');
            this.$http.get('user/list/'+this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    this.users = response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
            },
        getShifts() {
            this.$store.dispatch('loading','Loading...');
            this.$http.get('schedule/shifts?patrolId='+this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    this.shifts = response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
            },
        getGroups() {
            this.$store.dispatch('loading','Loading...');
            this.$http.get('user/groups/'+this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    this.groups = response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
            },
        deleteEvent(id){
        this.$store.dispatch('loading','Deleting...');
        this.$http.delete('event/'+id)
            .then(response => {
                console.log(response);
                getEvents();
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        hasPermission: function(permission){
          return this.selectedPatrol!=null && this.selectedPatrol.permissions!=null && _.indexOf(this.selectedPatrol.permissions,permission) >= 0;
        },
        filterUserListItems:function(assignments){

            let filtered = [];

            if(assignments){
                filtered = _.filter(this.users,function(u){return _.find(assignments,function(a){return a.assignedUser && a.assignedUser.id==u.id;})==null;});
            }
            else{
                filtered = this.users;
            }

            filtered = _.sortBy(filtered,['lastName','firstName']);

            this.filteredUserList = _.map(filtered,function(u){
                return {
                    label:u.lastName+", "+u.firstName,
                    value:u.id
                };
            });

            //if shift swap is enabled, the admin can choose to assign nobody and just leave a slot open
            if(this.selectedPatrol.enableShiftSwaps){
                this.filteredUserList.splice(0,0,{label:'(Available)',value:null});
            }

            if(this.filteredUserList.length>0){
                this.selectedUserId = this.filteredUserList[0].value;
            }
        }
  },
  beforeMount: function(){
      if(this.uid!=null){
          this.viewUserId = this.uid;
      }
      else{
        if(!this.hasPermission('MaintainSchedule')){
            this.viewUserId = this.userId;
        }
      }
  },
  mounted: function(){
      this.getEvents();
      this.getScheduledShifts();
      if(this.hasPermission('MaintainSchedule')){
        this.getUsers();
        this.getShifts();
        this.getGroups();
      }
  },
  watch: {
      selectedPatrolId: function(){
          this.getEvents();
          this.getScheduledShifts();
          if(this.hasPermission('MaintainSchedule')){
            this.getUsers();
            this.getShifts();
            this.getGroups();
          }
      },
      viewUserId: function(){
          this.getScheduledShifts();
      }
  },
  computed: {
    selectedPatrolId: function () {
      return this.$store.state.selectedPatrolId;
    },
    selectedPatrol: function (){
        return this.$store.getters.selectedPatrol;
    },
    userId: function (){
        return this.$store.getters.userId;
    },
    shiftItems: function(){
        var items = _.map(this.shifts,function(s){
            return {
                value: s.id,
                label: s.name + " ("+s.startHour+":"+(s.startMinute+"").padStart(2,"0") +" - "+s.endHour+":"+(s.endMinute+"").padStart(2,"0")+")"
            }
        });
        items.splice(0,0,{value:null,label:'(New)'})
        return items;
    },
    groupItems: function(){
        var items = _.map(this.groups,function(s){
            return {
                value: s.id,
                label: s.name
            }
        });
        items.splice(0,0,{value:null,label:'(None)'})
        return items;
    },
    userItems: function(){
        var items = _.map(this.users,function(s){
            return {
                value: s.id,
                label: s.lastName+', '+s.firstName
            }
        });

        items.splice(0,0,{value:null,label:'(All)'})

        if(items.length==1){
            items.splice(0,0,{value:this.userId,label:'(Me)'})
        }

        return items;
    },
    calendarEvents: function(){
        let userId = this.userId;
        let isFuture = this.isFuture;

        var cEvents = _.map(this.events,function(e){
            return {
                id: e.id,
                title: e.name,
                startDate: new Date(e.startsAt),
                endDate: new Date(e.endsAt),
                classes:'badge-success',
                type:'event'
            };
        });

        var cShifts = _.map(this.scheduledShifts,function(e){
            var eventClass='';

            var currentUserInShift = _.find(e.assignments,function(a){ 
                return (a.assignedUser && a.assignedUser.id == userId) || (a.claimedByUser && a.claimedByUser.id == userId)
            })!=null;
            
            if(isFuture(e.startsAt)){
                if(currentUserInShift){
                    eventClass = "badge-info";
                }
                else{
                    eventClass="badge-primary";
                }
            }
            else{
                if(currentUserInShift){
                    eventClass = "badge-secondary";
                }
                else{
                    eventClass="badge-dark";
                }
            }

            var startsAt = new Date(e.startsAt);
            var endsAt = new Date(e.endsAt);

            if(e.assignments==null){
                console.log('assignments is null');
            }

            return {
                id: e.scheduledShiftId,
                title: " "+ startsAt.getHours()+":"+(startsAt.getMinutes()+"").padStart(2,"0") 
                        + "-" + endsAt.getHours()+":"+((endsAt).getMinutes()+"").padStart(2,"0")
                        + " ("+(e.groupName ? e.groupName : e.assignments.length + " Assigned") + ") "
                        + (e.shiftName ? e.shiftName : "") ,
                startDate: startsAt,
                endDate: endsAt,
                classes: eventClass,
                type:'shift'
            };
        });

        for(var i=0;i<cShifts.length;i++){
            cEvents.push(cShifts[i]);
        }


        return cEvents;
    }
  }
}
</script>

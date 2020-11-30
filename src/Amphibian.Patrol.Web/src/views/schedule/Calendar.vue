<template>
    <div>
        <CCard>
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
                        @click-event="clickEvent">
                        <calendar-view-header
                            slot="header"
                            slot-scope="t"
                            :header-props="t.headerProps"
                            @input="setShowDate" />
                        </calendar-view>
                    </CCol>
                </CRow>
                <CRow>
                    <CCol md="3" lg="3" sm="12">
                        <CSelect
                        :options="['year', 'month', 'week']"
                        :value.sync="displayPeriod"
                        label="View"
                        add-wrapper-classes="ml-2"
                        />
                    </CCol>
                </CRow>
            </CCardBody>
            <CCardFooter>

            </CCardFooter>
        </CCard>
        <CModal
        :title="selectedEvent.title"
        :show.sync="showSelectedEvent"
        :no-close-on-backdrop="true"
        color="info"
        >
        {{(new Date(selectedEvent.startsAt)).toLocaleString()}} - {{(new Date(selectedEvent.endsAt)).toLocaleString()}}<br/>
        {{selectedEvent.name}}<br/>
        Location: {{selectedEvent.location}}<br/>
        <template #header>
        </template>
        <template #footer>
            <CButton @click="showSelectedEvent = false" color="info">Close</CButton>
        </template>
        </CModal>
        <CModal
        :title="selectedShift.title"
        :show.sync="showSelectedShift"
        :no-close-on-backdrop="true"
        color="primary"
        >
        <template v-if="selectedShift.shiftName">
            <strong>{{selectedShift.shiftName}}</strong><br/>
        </template>
        Start:{{(new Date(selectedShift.startsAt+"Z")).toLocaleString()}}<br/>
        End:{{(new Date(selectedShift.endsAt+"Z")).toLocaleString()}}<br/>
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
                <td>{{data.item.assignedUser.lastName}}, {{data.item.assignedUser.firstName}}</td>
            </template>
            <template #claimedByUser="data">
                <td>
                    <template v-if="data.item.status=='Claimed'">
                        {{data.item.claimedByUser.lastName}}, {{data.item.claimedByUser.firstName}}
                        <c-badge size="sm" color="warning">Pending Approval</c-badge>
                    </template>
                    <template v-if="data.item.status=='Released'">
                        <c-badge size="sm" color="warning">Released</c-badge>
                    </template>
                </td>
            </template>
            <template #status="data">
                <td><span v-if="data.item.status!='Assigned'">{{data.item.status}}</span></td>
            </template>
            <template #traineeCount="data">
                <td><span v-if="data.item.traineeCount>0"><c-badge size="sm" color="info">{{data.item.traineeCount}} Trainees</c-badge></span></td>
            </template>
            <template #buttons="data">
                <td>
                    <CButtonGroup class="float-right" v-if="isFuture(selectedShift.startsAt)">
                        <CButton size="sm" color="danger" v-if="hasPermission('MaintainSchedule')">Remove</CButton>
                        <CButton size="sm" color="primary" v-if="data.item.status=='Assigned' && data.item.assignedUser.id == user.id">Release</CButton>
                        <CButton size="sm" color="success" v-if="data.item.status=='Released' && data.item.assignedUser.id != user.id">Claim</CButton>
                        <CButton size="sm" color="warning" v-if="(data.item.status=='Claimed' || data.item.status=='Released') && data.item.assignedUser.id == user.id">Cancel Release</CButton>
                        <CButton size="sm" color="success" v-if="data.item.status=='Claimed' && hasPermission('MaintainSchedule')">Approve</CButton>
                        <CButton size="sm" color="danger" v-if="data.item.status=='Claimed' && hasPermission('MaintainSchedule')">Reject</CButton>
                    </CButtonGroup>
                </td>
            </template>
            <template #footer="data">
                <template v-if="isFuture(selectedShift.startsAt) && hasPermission('MaintainSchedule')">
                    <tfoot>
                        <tr>
                            <td colspan="4"><!--dropdown of users--></td>
                            <td><CButton size="sm" color="success" class="float-right">Add</CButton></td>
                        </tr>
                    </tfoot>
                </template>
            </template>
        </CDataTable>
        <template #header>Shift Details
        </template>
        <template #footer>
            <CButtonGroup class="float-right">
                <CButton v-if="hasPermission('MaintainSchedule') && isFuture(selectedShift.startsAt)" color="success">Add</CButton>
                <CButton v-if="hasPermission('MaintainSchedule') && isFuture(selectedShift.startsAt)" color="warning">Cancel Shift</CButton>
                <CButton @click="showSelectedShift = false" color="primary">Close</CButton>
            </CButtonGroup>
        </template>
        </CModal>
    </div>
</template>

<script>
import { freeSet } from '@coreui/icons'

import { CalendarView, CalendarViewHeader } from "vue-simple-calendar"
// The next two lines are processed by webpack. If you're using the component without webpack compilation,
// you should just create <link> elements for these. Both are optional, you can create your own theme if you prefer.
//require("vue-simple-calendar/static/css/default.css")
//require("vue-simple-calendar/static/css/holidays-us.css")

export default {
  name: 'Calendar',
  freeSet,
  components: {  CalendarView,
			CalendarViewHeader
  },
  props: [],
  data () {
    return {
        selectedDate:new Date(),
        from:new Date(),
        to:new Date(),
        events:[],
        shifts:[],
        displayPeriod:'month',
        showSelectedEvent:false,
        selectedEvent:{},
        showSelectedShift:false,
        selectedShift:{}
    }
  },
  methods: {
        isFuture(date){
            return new Date(date+"Z") > new Date();
        },
        periodChanged(range){
            this.from=range.displayFirstDate;
            this.to=range.displayLastDate;
            this.getEvents();
            this.getShifts();
        },
        clickEvent(event){
            console.log(JSON.stringify(event));
            if(event.originalEvent.type=='event'){
                var e = _.find(this.events,{id:event.id});
                this.selectedEvent = e;
                this.showSelectedEvent=true;
            }
            else if(event.originalEvent.type=='shift'){
                var e = _.find(this.shifts,{scheduledShiftId:event.id});
                this.selectedShift = e;
                this.showSelectedShift=true;
            }
            //else: shifts etc
        },
        setShowDate(d) {
            this.selectedDate = d;
        },
        getEvents() {
            this.$store.dispatch('loading','Loading...');
            this.$http.post('events/search',{patrolId:this.selectedPatrolId,from:this.from,to:this.to})
                .then(response => {
                    console.log(response);
                    this.events = response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
            },
        getShifts() {
            this.$store.dispatch('loading','Loading...');
            this.$http.post('schedule/search',{patrolId:this.selectedPatrolId,from:this.from,to:this.to})
                .then(response => {
                    console.log(response);
                    this.shifts = response.data;
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
        }
  },
  mounted: function(){
      this.getEvents();
      this.getShifts();
  },
  watch: {
      selectedPatrolId: function(){
          this.getEvents();
          this.getShifts();
      }
  },
  computed: {
    selectedPatrolId: function () {
      return this.$store.state.selectedPatrolId;
    },
    selectedPatrol: function (){
        return this.$store.getters.selectedPatrol;
    },
    user: function (){
        return this.$store.getters.user;
    },
    calendarEvents: function(){
        let user = this.user;
        let isFuture = this.isFuture;

        var cEvents = _.map(this.events,function(e){
            return {
                id: e.id,
                title: e.name,
                startDate: e.startsAt,
                endDate: e.endsAt,
                classes:'badge-success',
                type:'event'
            };
        });

        var cShifts = _.map(this.shifts,function(e){
            return {
                id: e.scheduledShiftId,
                title: (e.shiftName ? e.shiftName : "") 
                        + " "+ (new Date(e.startsAt+"Z")).getHours()+":"+((new Date(e.startsAt+"Z")).getMinutes()+"").padStart(2,"0") 
                        + "-" + (new Date(e.endsAt+"Z")).getHours()+":"+((new Date(e.endsAt+"Z")).getMinutes()+"").padStart(2,"0")
                        + " ("+(e.groupName ? e.groupName : e.assignments.length) + ")",
                startDate: e.startsAt,
                endDate: e.endsAt,
                classes: isFuture(e.startsAt) ? (_.find(e.assignments,function(a){ return a.assignedUser.id == user.id || (a.claimedByUser && a.claimedByUser.id == user.id)})!=null ? 'badge-info' : 'badge-primary') : 'badge-dark',
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

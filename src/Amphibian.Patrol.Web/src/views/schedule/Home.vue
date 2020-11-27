<template>
    <div>
        <template v-if="selectedPatrol.enableAnnouncements">
            <CRow v-for="announcement in announcements" :key="'announcement-'+announcement.id">
                <CCol>
                    <CCard accent-color="info">
                        <CCardHeader>
                            <strong>{{(new Date(announcement.createdAt)).toLocaleDateString()}}</strong>
                            {{announcement.subject}} 
                            <CButtonGroup class="float-right" v-if="hasPermission('MaintainAnnouncements')">
                                <CButton size="sm" color="info" :to="{name:'EditAnnouncement',params:{announcementId:announcement.id}}">Edit</CButton>
                                <CButton size="sm" color="warning" v-on:click="expireAnnouncement(announcement)">Remove</CButton>
                            </CButtonGroup>
                        </CCardHeader>
                        <CCardBody v-html="announcement.announcementHtml"></CCardBody>
                    </CCard>
                </CCol>
            </CRow>
        </template>
        <CCard v-if="upcomingEvents.length>0 && selectedPatrol.enableEvents">
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-calendar"/> Upcoming Events
                <CButton size="sm" color="info" class="float-right" :to="{name:'Calendar'}">Calendar</CButton>
            </slot>
            </CCardHeader>
            <CCardBody>
                <CDataTable
                    striped
                    bordered
                    small
                    fixed
                    :items="upcomingEvents"
                    :fields="upcomingEventsFields"
                    sorter>
                    <template #startsAt="data">
                        <td>{{(data.item.startsAt ? (new Date(data.item.startsAt)).toLocaleString() : '')}}</td>
                    </template>
                    <template #endsAt="data">
                        <td>{{(data.item.endsAt ? (new Date(data.item.endsAt)).toLocaleString() : '')}}</td>
                    </template>
                </CDataTable>
            </CCardBody>
        </CCard>
        <template v-if="selectedPatrol.enableTraining">
            <CRow v-if="hasPermission('MaintainAssignments')">
                <CCol md="6">
                    <CCard v-if="hasPermission('MaintainAssignments') && assignmentCountsByDay.length>0">
                        <CCardHeader>
                        <slot name="header">
                            <CIcon name="cil-grid"/># Incomplete Assignments, Last 30 Days
                        </slot>
                        </CCardHeader>
                        <CCardBody>
                            <CChartLine
                                :datasets="assignmentCountsByDay"
                                :labels="assignmentCountsByDayLabels"
                                :options="assignmentCountsByDayOptions"
                            />
                        </CCardBody>
                    </CCard>
                </CCol>
                <CCol md="6">
                    <CCard v-if="hasPermission('MaintainAssignments') && assignmentProgressByDay.length>0">
                        <CCardHeader>
                        <slot name="header">
                            <CIcon name="cil-grid"/>Assignments % Complete, Last 30 Days
                        </slot>
                        </CCardHeader>
                        <CCardBody>
                            <CChartLine
                                :datasets="assignmentProgressByDay"
                                :labels="assignmentProgressByDayLabels"
                                :options="assignmentProgressByDayOptions"
                            />
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>

            <CCard v-if="openAssignments.length>0 && hasPermission('MaintainAssignments')">
                <CCardHeader>
                <slot name="header">
                    <CIcon name="cil-grid"/> Open Assignments
                </slot>
                </CCardHeader>
                <CCardBody>
                <CDataTable
                    :items="openAssignments"
                    :fields="openAssignmentsFields">
                    <template #assignee="data">
                        <td>{{data.item.userLastName}}, {{data.item.userFirstName}}</td>
                    </template>
                    <template #planName="data">
                        <td><router-link :to="{ name: 'Assignment', params: {assignmentId:JSON.stringify(data.item.id)}}">{{data.item.planName}}</router-link></td>
                    </template>
                    <template #signatures="data">
                        <td>{{data.item.signatures}}/{{data.item.signaturesRequired}}</td>
                    </template>
                    <template #progress="data">
                        <td><CProgress
                                :value="(Math.round((data.item.signatures / data.item.signaturesRequired) * 100))"
                                color="success"
                                striped
                                show-percentage
                                />
                        </td>
                    </template>
                    <template #dueAt="data">
                        <td>{{data.item.dueAt ? (new Date(data.item.dueAt)).toLocaleDateString() : ''}}</td>
                    </template>
                    <template #assignedAt="data">
                        <td>{{(new Date(data.item.assignedAt)).toLocaleDateString()}}</td>
                    </template>
                </CDataTable>
                </CCardBody>
            </CCard>

            <CCard v-if="openAssignments.length>0 && hasPermission('MaintainAssignments')">
                <CCardHeader>
                <slot name="header">
                    <CIcon name="cil-grid"/> Training Plans
                </slot>
                </CCardHeader>
                <CCardBody>
                <CDataTable
                    :items="assignmentsByPlan"
                    :fields="assignmentsByPlanFields">
                    <template #name="data">
                        <td><router-link :to="{ name: 'Assignments', params: {planId:data.item.id}}">{{data.item.name}}</router-link></td>
                    </template>
                    <template #count="data">
                        <td>{{data.item.count}}</td>
                    </template>
                </CDataTable>
                </CCardBody>
            </CCard>

            <CCard v-if="trainerShifts.length>0">
                <CCardHeader>
                <slot name="header">
                    <CIcon name="cil-grid"/> Upcoming Trainer Shifts
                </slot>
                </CCardHeader>
                <CCardBody>
                <CDataTable
                    :items="trainerShifts"
                    :fields="trainerShiftFields">
                    <template #at="data">
                        <td>{{(new Date(data.item.startsAt)).toLocaleDateString()}} {{(new Date(data.item.startsAt)).toLocaleTimeString()}}</td>
                    </template>
                    <template #buttons="data">
                        <td></td>
                    </template>
                </CDataTable>
                </CCardBody>
            </CCard>

            <CCard v-if="myAssignments.length>0">
                <CCardHeader>
                <slot name="header">
                    <CIcon name="cil-grid"/> My Assignments
                </slot>
                </CCardHeader>
                <CCardBody>
                <CDataTable
                    :items="myAssignments"
                    :fields="myAssignmentsFields">
                    <template #planName="data">
                        <td><router-link :to="{ name: 'Assignment', params: {assignmentId:JSON.stringify(data.item.id)}}">{{data.item.planName}}</router-link></td>
                    </template>
                    <template #signatures="data">
                        <td>{{data.item.signatures}}/{{data.item.signaturesRequired}}</td>
                    </template>
                    <template #progress="data">
                        <td><CProgress
                                :value="(Math.round((data.item.signatures / data.item.signaturesRequired) * 100))"
                                color="success"
                                striped
                                show-percentage
                                />
                        </td>
                    </template>
                    <template #dueAt="data">
                        <td>{{data.item.dueAt ? (new Date(data.item.dueAt)).toLocaleDateString() : ''}}</td>
                    </template>
                    <template #assignedAt="data">
                        <td>{{(new Date(data.item.assignedAt)).toLocaleDateString()}}</td>
                    </template>
                </CDataTable>
                </CCardBody>
            </CCard>

            <CCard v-if="trainerIncompleteAssignments.length>0">
                <CCardHeader>
                <slot name="header">
                    <CIcon name="cil-grid"/> Trainees with Incomplete Assignments
                </slot>
                </CCardHeader>
                <CCardBody>
                <CDataTable
                    :items="trainerIncompleteAssignments"
                    :fields="trainerIncompleteAssignmentFields">
                    <template #planName="data">
                        <td><router-link :to="{ name: 'Assignment', params: {assignmentId:JSON.stringify(data.item.id)}}">{{data.item.planName}}</router-link></td>
                    </template>
                    <template #signatures="data">
                        <td>{{data.item.signatures}}/{{data.item.signaturesRequired}}</td>
                    </template>
                    <template #progress="data">
                        <td><CProgress
                                :value="(Math.round((data.item.signatures / data.item.signaturesRequired) * 100))"
                                color="success"
                                striped
                                show-percentage
                                />
                        </td>
                    </template>
                    <template #dueAt="data">
                        <td>{{data.item.dueAt ? (new Date(data.item.dueAt)).toLocaleDateString() : ''}}</td>
                    </template>
                    <template #assignedAt="data">
                        <td>{{(new Date(data.item.assignedAt)).toLocaleDateString()}}</td>
                    </template>
                </CDataTable>
                </CCardBody>
            </CCard>
        </template>

        <template v-if="selectedPatrol.enableScheduling">
            <CCard v-if="committedShifts.length>0">
                <CCardHeader>
                <slot name="header">
                    <CIcon name="cil-grid"/> Upcoming Training Shifts
                </slot>
                </CCardHeader>
                <CCardBody>
                <CDataTable
                    :items="committedShifts"
                    :fields="committedShiftFields">
                    <template #at="data">
                        <td>{{(new Date(data.item.startsAt)).toLocaleDateString()}} {{(new Date(data.item.startsAt)).toLocaleTimeString()}}</td>
                    </template>
                    <template #trainer="data">
                        <td>{{data.item.trainerUser.firstName}} {{data.item.trainerUser.lastName}}</td>
                    </template>
                    <template #buttons="data">
                        <td><CButtonGroup><CButton color="warning" size="sm" v-on:click="cancel(data.item.traineeId)">Cancel</CButton></CButtonGroup></td>
                    </template>
                </CDataTable>
                </CCardBody>
            </CCard>

            <CCard v-if="availableShifts.length>0">
                <CCardHeader>
                <slot name="header">
                    <CIcon name="cil-grid"/> Available Training Shifts
                </slot>
                </CCardHeader>
                <CCardBody>
                <CDataTable
                    :items="availableShifts"
                    :fields="availableShiftFields">
                    <template #at="data">
                        <td>{{(new Date(data.item.startsAt)).toLocaleDateString()}} {{(new Date(data.item.startsAt)).toLocaleTimeString()}}</td>
                    </template>
                    <template #trainer="data">
                        <td>{{data.item.trainerUser.firstName}} {{data.item.trainerUser.lastName}}</td>
                    </template>
                    <template #buttons="data">
                        <td><CButtonGroup><CButton color="primary" size="sm" v-on:click="commit(data.item.id)">Sign Up</CButton></CButtonGroup></td>
                    </template>
                </CDataTable>
                </CCardBody>
            </CCard>
        </template>
    </div>
    
</template>

<script>
import { CChartLine } from '@coreui/vue-chartjs'
import BlockUI from 'vue-blockui'

export default {
  name: 'Home',
  components: { CChartLine, BlockUI
  },
  computed: {
    selectedPatrolId: function () {
      return this.$store.state.selectedPatrolId;
    },
    selectedPatrol: function (){
        return this.$store.getters.selectedPatrol;
    },
    assignmentsByPlan:function(){
        var planGroups = [];

        for(var i=0;i<this.openAssignments.length;i++){
            var group = _.find(planGroups,{id:this.openAssignments[i].planId});
            if(group==null)
            {
                group = {id:this.openAssignments[i].planId,name:this.openAssignments[i].planName,count:0};
                planGroups.push(group);
            }
            group.count++;
        }
        
        return planGroups; 
    }
  },
  watch: {
    selectedPatrolId(){
      this.refresh();
    }
  },
  props: [],
  data () {
    return {
        caption: '',
        myAssignments: [],
        myAssignmentsFields:[
          {key:'planName',label:''},
          {key:'assignedAt', label:'Assigned'},
          {key:'dueAt', label:'Due'},
          {key:'signatures', label:'Signatures'},
          {key:'progress', label:'Progress'}
        ],
        trainerIncompleteAssignments:[],
        trainerIncompleteAssignmentFields:[
          {key:'userLastName',label:'Last'},
          {key:'userFirstName',label:'First'},
          {key:'planName',label:'Assignment'},
          {key:'assignedAt', label:'Assigned'},
          {key:'dueAt', label:'Due'},
          {key:'signatures', label:'Signatures'},
          {key:'progress', label:'Progress'}
        ],
        committedShifts:[],
        committedShiftFields:[
          {key:'at',label:'Date/Time'},
          {key:'trainer',label:'Trainer'},
          {key:'traineeCount',label:'Group Size'},
          {key:'buttons',label:''},
        ],
        availableShifts:[],
        availableShiftFields:[
          {key:'at',label:'Date/Time'},
          {key:'trainer',label:'Trainer'},
          {key:'traineeCount',label:'Group Size'},
          {key:'buttons',label:''},
        ],
        trainerShifts:[],
        trainerShiftFields:[
            {key:'at',label:'Date/Time'},
            {key:'traineeCount',label:'Group Size'},
            {key:'buttons',label:''},
        ],
        assignmentCountsByDay:[],
        assignmentCountsByDayOptions:{
            scales: {
                yAxes: [{
                    ticks: {
                        stepSize: 1,
                        min: 0
                    }
                }]
            },
            title:{
                display: false,
                text: 'Incomplete Assignments, Last 30 Days'
            },
            legend:{
                display: false
            }
        },
        assignmentCountsByDayLabels:[],
        assignmentProgressByDay:[],
        assignmentProgressByDayOptions:{
            scales: {
                yAxes: [{
                    ticks: {
                        stepSize: 25,
                        min: 0,
                        max: 100
                    }
                }]
            },
            title:{
                display: false,
                text: 'Assignment Progress, Last 30 Days'
            },
            legend:{
                display:false
            }
        },
        assignmentProgressByDayLabels:[],
        openAssignments: [],
        openAssignmentsFields:[
          {key:'assignee',label:'Assignee'},
          {key:'planName',label:'Training Plan'},
          {key:'assignedAt', label:'Assigned'},
          {key:'signatures', label:'Signatures'},
          {key:'progress', label:'Progress'},
          {key:'dueAt', label:'Due'}
        ],
        assignmentsByPlanFields:[
          {key:'name',label:'Plan'},
          {key:'count',label:'Open Assignments'}
        ],
        announcements:[],
        upcomingEvents:[],
        upcomingEventsFields:[{key:'name',label:'Event'},
          {key:'location',label:'Location'},
          {key:'startsAt',label:'Date/Time'}]
    }
  },
  methods: {
        getMyAssignments() {
            this.$store.dispatch('loading','Loading...');
            this.$http.get('assignments')
                .then(response => {
                    console.log(response);
                    this.myAssignments = response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        getUpcomingEvents() {
            this.$store.dispatch('loading','Loading...');
            this.$http.get('events/upcoming/'+this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    this.upcomingEvents = response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        getIncompleteTrainerAssignments() {
            this.$store.dispatch('loading','Loading...');
            this.$http.get('assignments/incomplete-for-trainer/'+this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    this.trainerIncompleteAssignments = response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        getCommittedTrainingShifts() {
            this.$store.dispatch('loading','Loading...');
            this.$http.get('trainingshifts/committed/'+this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    this.committedShifts = response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        getAvailableTrainingShifts() {
            this.$store.dispatch('loading','Loading...');
            this.$http.get('trainingshifts/available/'+this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    this.availableShifts = response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        getTrainerShifts(){
            this.$store.dispatch('loading','Loading...');
            this.$http.get('trainingshifts/training/'+this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    this.trainerShifts = response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        getAssignmentCountsByDay(){
            this.$store.dispatch('loading','Loading...');
            this.assignmentCountsByDay=[];
            this.$http.get('assignments/counts-by-day?patrolId='+this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    var counts = response.data;
                    this.assignmentCountsByDay = _.map(counts,function(g){
                        return {
                            label: g.planName,
                            borderColor: g.planColor,
                            data: _.map(g.countsByDay,'openAssignmentCount'),
                            fill:false,
                            lineTension: 0
                        };
                    });
                    if(this.assignmentCountsByDay.length>0){
                        this.assignmentCountsByDayLabels = _.map(counts[0].countsByDay,function(d){
                            return (new Date(d.day)).toLocaleDateString();
                        });
                    }
                    
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        getAssignmentProgressByDay(){
            this.$store.dispatch('loading','Loading...');
            this.assignmentProgressByDay=[];
            this.$http.get('assignments/progress-by-day?patrolId='+this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    var counts = response.data;
                    this.assignmentProgressByDay = _.map(counts,function(g){
                        return {
                            label: g.userLastName+', '+g.userFirstName+' - '+g.planName,
                            borderColor: 'rgba('+(Math.floor(Math.random() * Math.floor(255)))+','+(Math.floor(Math.random() * Math.floor(255)))+','+(Math.floor(Math.random() * Math.floor(255)))+',10)',
                            data: _.map(g.days,function(d){
                                return Math.round((d.completedsignatures / d.requiredsignatures) * 100.0);
                            }),
                            fill:false,
                            lineTension: 0
                        };
                    });
                    if(this.assignmentProgressByDay.length>0){
                        this.assignmentProgressByDayLabels = _.map(counts[0].days,function(d){
                            return (new Date(d.day)).toLocaleDateString();
                        });
                    }
                    
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        getOpenAssignments() {
            this.$store.dispatch('loading','Loading...');
            this.$http.post('assignments/search',{patrolId:this.selectedPatrolId,completed:false})
                .then(response => {
                    console.log(response);
                    this.openAssignments = response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        getAnnouncements() {
            this.$store.dispatch('loading','Loading...');
            this.$http.get('announcements/current/'+this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    this.announcements = response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        commit(id){
            this.$store.dispatch('loading','Loading...');
            this.$http.post('trainingshifts/commit/'+id)
                .then(response => {
                    console.log(response);
                    this.getCommittedTrainingShifts();
                    this.getAvailableTrainingShifts();
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        cancel(id){
            this.$store.dispatch('loading','Loading...');
            this.$http.post('trainingshifts/cancel/'+id)
                .then(response => {
                    console.log(response);
                    this.getCommittedTrainingShifts();
                    this.getAvailableTrainingShifts();
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        hasPermission: function(permission){
          return this.selectedPatrol!=null && this.selectedPatrol.permissions!=null && _.indexOf(this.selectedPatrol.permissions,permission) >= 0;
        },
        expireAnnouncement(announcement){
            this.$store.dispatch('loading','Loading...');
            this.$http.post('announcements/expire/'+announcement.id)
                .then(response => {
                    console.log(response);
                    this.announcements = _.filter(this.announcements,function(a){return a.id!=announcement.id;});
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        refresh:function(){
            if(this.selectedPatrol.enableTraining){
                this.getMyAssignments();
                this.getIncompleteTrainerAssignments();

                if(this.hasPermission('MaintainAssignments')){
                    this.getAssignmentCountsByDay();
                    this.getAssignmentProgressByDay();
                    this.getOpenAssignments();
                }
            }

            if(this.selectedPatrol.enableScheduling){
                this.getCommittedTrainingShifts();
                this.getAvailableTrainingShifts();
                this.getTrainerShifts();
            }
            
            if(this.selectedPatrol.enableAnnouncements){
                this.getAnnouncements();
            }

            if(this.selectedPatrol.enableEvents){
                this.getUpcomingEvents();
            }
        }
  },
  mounted: function(){
      this.refresh();
  }
}
</script>

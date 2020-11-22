<template>
    <div>
        <CRow v-if="hasPermission('MaintainAssignments')">
            <CCol md="6">
                <CCard v-if="hasPermission('MaintainAssignments') && assignmentCountsByDay.length>0">
                    <CCardHeader>
                    <slot name="header">
                        <CIcon name="cil-grid"/>Incomplete Assignments, Last 30 Days
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
                        <CIcon name="cil-grid"/>Assignment % Complete, Last 30 Days
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
                            :animated="animate"
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
                            :animated="animate"
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
                            :animated="animate"
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
    </div>
</template>

<script>
import { CChartLine } from '@coreui/vue-chartjs'

export default {
  name: 'Home',
  components: { CChartLine
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
      this.getMyAssignments();
      this.getIncompleteTrainerAssignments();
      this.getCommittedTrainingShifts();
      this.getAvailableTrainingShifts();
      this.getTrainerShifts();
      if(this.hasPermission('MaintainAssignments')){
          this.getAssignmentCountsByDay();
          this.getAssignmentProgressByDay();
          this.getOpenAssignments();
      }
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
        ]
    }
  },
  methods: {
        getMyAssignments() {
            this.$http.get('assignments')
                .then(response => {
                    console.log(response);
                    this.myAssignments = response.data;
                }).catch(response => {
                    console.log(response);
                });
        },
        getIncompleteTrainerAssignments() {
            this.$http.get('assignments/incomplete-for-trainer/'+this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    this.trainerIncompleteAssignments = response.data;
                }).catch(response => {
                    console.log(response);
                });
        },
        getCommittedTrainingShifts() {
            this.$http.get('trainingshifts/committed/'+this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    this.committedShifts = response.data;
                }).catch(response => {
                    console.log(response);
                });
        },
        getAvailableTrainingShifts() {
            this.$http.get('trainingshifts/available/'+this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    this.availableShifts = response.data;
                }).catch(response => {
                    console.log(response);
                });
        },
        getTrainerShifts(){
            this.$http.get('trainingshifts/training/'+this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    this.trainerShifts = response.data;
                }).catch(response => {
                    console.log(response);
                });
        },
        getAssignmentCountsByDay(){
            this.assignmentCountsByDay=[];
            this.$http.get('assignments/counts-by-day?patrolId='+this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    var counts = response.data;
                    this.assignmentCountsByDay = _.map(counts,function(g){
                        return {
                            label: g.planName,
                            backgroundColor: g.planColor,
                            data: _.map(g.countsByDay,'openAssignmentCount')
                        };
                    });

                    if(this.assignmentCountsByDay.length>0){
                        this.assignmentCountsByDayLabels = _.map(counts[0].countsByDay,function(d){
                            return (new Date(d.day)).toLocaleDateString();
                        });
                    }
                    
                }).catch(response => {
                    console.log(response);
                });
        },
        getAssignmentProgressByDay(){
            this.assignmentProgressByDay=[];
            this.$http.get('assignments/progress-by-day?patrolId='+this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    var counts = response.data;
                    this.assignmentProgressByDay = _.map(counts,function(g){
                        return {
                            label: g.userLastName+', '+g.userFirstName+' - '+g.planName,
                            backgroundColor: 'rgba('+(Math.floor(Math.random() * Math.floor(255)))+','+(Math.floor(Math.random() * Math.floor(255)))+','+(Math.floor(Math.random() * Math.floor(255)))+',10)',
                            data: _.map(g.days,function(d){
                                return Math.round((d.completedsignatures / d.requiredsignatures) * 100.0);
                            })
                        };
                    });
                    if(this.assignmentProgressByDay.length>0){
                        this.assignmentProgressByDayLabels = _.map(counts[0].days,function(d){
                            return (new Date(d.day)).toLocaleDateString();
                        });
                    }
                    
                }).catch(response => {
                    console.log(response);
                });
        },
        getOpenAssignments() {
            this.$http.post('assignment/search',{patrolId:this.selectedPatrolId,completed:false})
                .then(response => {
                    console.log(response);
                    this.openAssignments = response.data;
                }).catch(response => {
                    console.log(response);
                });
        },
        commit(id){
            this.$http.post('trainingshifts/commit/'+id)
                .then(response => {
                    console.log(response);
                    this.getCommittedTrainingShifts();
                    this.getAvailableTrainingShifts();
                }).catch(response => {
                    console.log(response);
                });
        },
        cancel(id){
            this.$http.post('trainingshifts/cancel/'+id)
                .then(response => {
                    console.log(response);
                    this.getCommittedTrainingShifts();
                    this.getAvailableTrainingShifts();
                }).catch(response => {
                    console.log(response);
                });
        },
        hasPermission: function(permission){
          return this.selectedPatrol!=null && this.selectedPatrol.permissions!=null && _.indexOf(this.selectedPatrol.permissions,permission) >= 0;
        }
  },
  mounted: function(){
      this.getMyAssignments();
      this.getIncompleteTrainerAssignments();
      this.getCommittedTrainingShifts();
      this.getAvailableTrainingShifts();
      this.getTrainerShifts();
      if(this.hasPermission('MaintainAssignments')){
          this.getAssignmentCountsByDay();
          this.getAssignmentProgressByDay();
          this.getOpenAssignments();
      }
  }
}
</script>

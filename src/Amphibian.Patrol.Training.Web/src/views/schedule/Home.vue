<template>
    <div>
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
export default {
  name: 'Home',
  components: {
  },
  computed: {
    selectedPatrolId: function () {
      return this.$store.state.selectedPatrolId;
    },
    selectedPatrol: function (){
        return this.$store.getters.selectedPatrol;
    }
  },
  watch: {
    selectedPatrolId(){
      this.getIncompleteTrainerAssignments();
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
        }
  },
  mounted: function(){
      this.getMyAssignments();
      this.getIncompleteTrainerAssignments();
      this.getCommittedTrainingShifts();
      this.getAvailableTrainingShifts();
      this.getTrainerShifts();
  }
}
</script>

<template>
    <div>
        <CCard v-if="myAssignments.length>0">
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-grid"/> My Assignments
            </slot>
            </CCardHeader>
            <CCardBody>
            <CDataTable
                :hover="hover"
                :striped="true"
                :bordered="true"
                :small="small"
                :fixed="fixed"
                :items="myAssignments"
                :fields="myAssignmentsFields"
                :dark="dark">
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
                :hover="hover"
                :striped="true"
                :bordered="true"
                :small="small"
                :fixed="fixed"
                :items="trainerIncompleteAssignments"
                :fields="trainerIncompleteAssignmentFields"
                :dark="dark">
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
        trainerIncompleteAssignments:[],
        myAssignmentsFields:[
          {key:'planName',label:''},
          {key:'assignedAt', label:'Assigned'},
          {key:'dueAt', label:'Due'},
          {key:'signatures', label:'Signatures'},
          {key:'progress', label:'Progress'}
        ],
        trainerIncompleteAssignmentFields:[
          {key:'userLastName',label:'Last'},
          {key:'userFirstName',label:'First'},
          {key:'planName',label:'Assignment'},
          {key:'assignedAt', label:'Assigned'},
          {key:'dueAt', label:'Due'},
          {key:'signatures', label:'Signatures'},
          {key:'progress', label:'Progress'}
        ]
    }
  },
  methods: {
        getMyAssignments(planId) {
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
        }
  },
  mounted: function(){
      this.getMyAssignments(this.planId);
      this.getIncompleteTrainerAssignments();
  }
}
</script>

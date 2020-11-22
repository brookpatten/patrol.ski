<template>
    <div>
        <CCard v-if="assignments.length>0">
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
                :items="assignments"
                :fields="fields"
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
  props: [],
  data () {
    return {
        caption: '',
        assignments: [],
        fields:[
          {key:'planName',label:''},
          {key:'assignedAt', label:'Assigned'},
          {key:'dueAt', label:'Due'},
          {key:'signatures', label:'Signatures'},
          {key:'progress', label:'Progress'}
      ]
    }
  },
  methods: {
        getAssignments(planId) {
            this.$http.get('assignments')
                .then(response => {
                    console.log(response);
                    this.assignments = response.data;
                }).catch(response => {
                    console.log(response);
                });
        }
  },
  mounted: function(){
      this.getAssignments(this.planId);
  }
}
</script>

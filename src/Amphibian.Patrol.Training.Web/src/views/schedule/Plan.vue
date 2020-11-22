<template>
    <div>
        <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-grid"/> {{caption}}
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
                <template #id="data">
                    <td><router-link :to="{ name: 'Assignment', params: {assignmentId:JSON.stringify(data.item.id)}}">View</router-link></td>
                </template>
            </CDataTable>
            </CCardBody>
        </CCard>
    </div>
</template>

<script>
export default {
  name: 'Plan',
  components: {
  },
  props: ['planId'],
  data () {
    return {
        caption: '',
        assignments: [],
        fields:[
          {key:'id',label:''},
          {key:'userId'},
          {key:'assignedAt', label:'Assigned'},
          {key:'dueAt', label:'Due'},
      ]
    }
  },
  methods: {
        getAssignments(planId) {
            this.$http.get('assignments/by-plan/'+planId)
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

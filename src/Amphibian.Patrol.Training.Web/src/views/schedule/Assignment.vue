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
            </CDataTable>
            </CCardBody>
        </CCard>
    </div>
</template>

<script>
export default {
  name: 'Assignment',
  components: {
  },
  props: ['assignmentId'],
  data () {
    return {
        caption: '',
        assignment: {},
        plan:{},
        fields:[
          {key:'userId'},
          {key:'assignedAt', label:'Assigned'},
          {key:'dueAt', label:'Due'},
      ]
    }
  },
  methods: {
        getAssignment(assignmentId) {
            this.$http.get('assignment/'+assignmentId)
                .then(response => {
                    console.log(response);
                    this.assignment = response.data;

                    this.$http.get('plan/'+this.assignment.planId)
                    .then(response=>{
                        this.plan = response.data;
                    }).catch(response=>{
                        console.log(response);
                    });
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

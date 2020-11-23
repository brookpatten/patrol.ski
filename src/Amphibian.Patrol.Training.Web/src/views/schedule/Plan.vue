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
  name: 'Plan',
  components: {
  },
  props: ['planId'],
  data () {
    return {
        caption: '',
        assignments: [],
        fields:[
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
        getAssignments(planId) {
            this.$store.dispatch('loading','Loading...');
            this.$http.get('assignments/by-plan/'+planId)
                .then(response => {
                    console.log(response);
                    this.assignments = response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        }
  },
  mounted: function(){
      this.getAssignments(this.planId);
  }
}
</script>

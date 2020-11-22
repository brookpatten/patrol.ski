<template>
    <div>
        <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-grid"/> Plans
            </slot>
            </CCardHeader>
            <CCardBody>
            <CDataTable
                :items="plans"
                :fields="[{key:'name',label:'Name'},{key:'buttons',label:''}]"
            >
              <template #buttons="data">
                  <td>
                    <CButtonGroup size="sm">
                      <CButton v-if="hasPermission('MaintainPlans')" color="primary" :to="{ name: 'EditPlan', params: { planId: data.item.id } }">Edit</CButton>
                      <CButton v-if="hasPermission('MaintainAssignments')" color="primary" :to="{ name: 'Assignments', params: { planId: data.item.id } }">Assignments</CButton>
                    </CButtonGroup>
                  </td>
              </template>
            </CDataTable>
            </CCardBody>
        </CCard>
    </div>
</template>

<script>
export default {
  name: 'Plans',
  components: {
  },
  data () {
    return {
        plans:[],
    }
  },
  mounted: function(){
    this.loadPlans();
  },
  computed: {
    selectedPatrolId: function () {
      return this.$store.state.selectedPatrolId;
    },
    selectedPatrol: function (){
        return this.$store.getters.selectedPatrol;
    }
  },
  methods: {
        loadPlans() {
            console.log('test');
            this.$http.get('plans?patrolId=' + this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    this.plans=response.data;
                }).catch(response => {
                    console.log(response);
                });
        },
        hasPermission: function(permission){
          return this.selectedPatrol!=null && this.selectedPatrol.permissions!=null && _.indexOf(this.selectedPatrol.permissions,permission) >= 0;
        }
  },
  watch: {
    selectedPatrolId(){
      this.loadPlans();
    }
  }
}
</script>

<template>
    <div>
        <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-grid"/> Plans
                <CButton color="success" size="sm" class="float-right" v-on:click="newPlan()">New</CButton>
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
                      <CButton v-if="hasPermission('MaintainPlans')" v-on:click="newPlan(data.item.id)" color="warning">Copy</CButton>
                      <CButton v-if="hasPermission('MaintainAssignments')" color="info" :to="{ name: 'Assignments', params: { planId: data.item.id } }">Assignments</CButton>
                      <CButton v-if="hasPermission('MaintainAssignments')" color="success" :to="{ name: 'NewAssignment', params: { planId: data.item.id } }">New Assignment(s)</CButton>
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
        newPlan(copyFromId){
          this.$http.post('plan/create?patrolId=' + this.selectedPatrolId+(copyFromId ? ('&copyFromPlanId='+copyFromId) : ''))
                .then(response => {
                    this.$router.push({name:'EditPlan',params:{planId:response.data.id}});
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

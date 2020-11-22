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
                :hover="hover"
                :striped="true"
                :bordered="true"
                :small="small"
                :fixed="fixed"
                :items="plans"
                :fields="demoFields"
                :items-per-page="small ? 10 : 5"
                :dark="dark"
                pagination
            >
                <template #status="{item}">
                <td>
                    <CBadge :color="getBadge(item.status)">{{item.status}}</CBadge>
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
        planFields:[
          {key:'name', label:'Plan', stickyColumn:true}
        ]
    }
  },
  mounted: function(){
    this.loadPlans();
  },
  computed: {
    selectedPatrolId: function () {
      return this.$store.state.selectedPatrolId;
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
        }
  },
  watch: {
    selectedPatrolId(){
      this.loadPlans();
    }
  }
}
</script>

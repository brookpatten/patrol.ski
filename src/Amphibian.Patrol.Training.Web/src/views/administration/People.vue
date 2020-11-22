<template>
    <div>
      <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-grid"/>
            </slot>
            </CCardHeader>
            <CCardBody>
            <CDataTable
                :hover="hover"
                :striped="true"
                :bordered="true"
                :small="small"
                :fixed="fixed"
                :items="people"
                :fields="peopleFields"
                :dark="dark">
            </CDataTable>
            </CCardBody>
        </CCard>
    </div>
</template>

<script>

export default {
  name: 'People',
  components: {
  },
  data () {
    return {
      people: [],
      peopleFields:[
          {key:'lastName',label:'Last'},
          {key:'firstName',label:'First'},
          {key:'email',label:'Assignment'},
          {key:'roles', label:'Roles'},
          {key:'groups', label:'Groups'}
      ]
    }
  },
  methods: {
    getPeople() {
        this.$http.get('user/list/'+this.selectedPatrolId)
            .then(response => {
                console.log(response);
                this.people = response.data;
            }).catch(response => {
                console.log(response);
            });
        },
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
      this.getPeople();
    }
  },
  mounted: function(){
      this.getPeople();
  }
}
</script>

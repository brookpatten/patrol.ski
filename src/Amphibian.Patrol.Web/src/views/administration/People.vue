<template>
    <div>
      <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-user"/>
            </slot>
            </CCardHeader>
            <CCardBody>
            <CDataTable
                striped
                bordered
                small
                fixed
                :items="people"
                :fields="peopleFields"
                column-filter
                sorter>
                <template #role="data">
                    <td><CBadge v-if="data.item.role" color="primary">{{data.item.role}}</CBadge></td>
                </template>
                <template #groups="data">
                    <td>
                      <CBadge v-for="group in data.item.groups" v-bind:key="group.id" color="success">{{group.name}}</CBadge>
                    </td>
                </template>
                <template #buttons="data">
                  <td>
                    <CButtonGroup size="sm">
                      <CButton color="primary" :to="{ name: 'EditUser', params: { userId: data.item.id } }">Edit</CButton>
                      <CButton color="danger" v-on:click="removeUser(data.item.id)">Remove</CButton>
                      <!--<CButton v-if="selectedPatrol.enableTimeClock && hasPermission('MaintainTimeClock')" color="primary" :to="{ name: 'TimeEntries', params: { userId: data.item.id } }">Time Entries</CButton>
                      <CButton v-if="selectedPatrol.enableTraining && hasPermission('MaintainAssignments')" color="info" :to="{ name: 'Assignments', params: { userId: data.item.id } }">Assignments</CButton>
                      <CButton v-if="selectedPatrol.enableScheduling" color="success" :to="{ name: 'Calendar', params: { userId: data.item.id } }">Schedule</CButton>-->
                    </CButtonGroup>
                  </td>
                </template>
                <template #buttons-header>
                  <CButton color="primary" size="sm" :to="{name:'EditUser',params:{userId:null}}">New</CButton>
                </template>
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
          {key:'role', label:'Role'},
          {key:'groups', label:'Groups'},
          {key:'buttons',label:'',sorter:false,filter:false}
      ]
    }
  },
  methods: {
    getPeople() {
        this.$store.dispatch('loading','Loading...');
        this.$http.get('user/list/'+this.selectedPatrolId)
            .then(response => {
                console.log(response);
                this.people = response.data;
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
    removeUser(userId){
      this.$store.dispatch('loading','Removing...');
      this.$http.post('user/remove-from-patrol',{userId:userId,patrolId:this.selectedPatrolId})
        .then(response=>{
          this.people = _.filter(this.people,function(x){return x.id!=userId;});
        }).catch(response=>{
          console.log(response);
        }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    hasPermission: function(permission){
      return this.selectedPatrol!=null && this.selectedPatrol.permissions!=null && _.indexOf(this.selectedPatrol.permissions,permission) >= 0;
    }
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

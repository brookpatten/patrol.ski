<template>
    <div>
      <CCard id="groups">
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-people"/>
            </slot>
            </CCardHeader>
            <CCardBody>
            <CAlert color="info">
              <p>Create groups of people to make administration tasks easier.  People can belong to many groups at the same time</p>
              <p>Some examples:</p>
              <ul>
                <li>Different categories of trainers who might perform training & sign-offs. eg: "PSIA Level 2" or "Instructor Trainer"</li>
                <li>People who are frequently scheduled togethor to make scheduling easier.  a "Crew", eg: "A Crew" or "Smith Crew"</li>
                <li>All alpine patrollers</li>
                <li>All ski or all snowboard patrollers</li>
                <li>Everyone interested in leadership announcements/events</li>
              </ul>
            </CAlert>
            <CDataTable
                striped
                bordered
                small
                fixed
                :items="groups"
                :fields="groupFields"
                sorter>
                <template #buttons="data">
                  <td>
                    <CButtonGroup size="sm">
                      <CButton color="primary" :to="{ name: 'EditGroup', params: { groupId: data.item.id } }">Edit</CButton>
                      <CButton color="danger" v-on:click="removeGroup(data.item.id)">Remove</CButton>
                    </CButtonGroup>
                  </td>
                </template>
                <template #buttons-header>
                  <CButton color="primary" size="sm" :to="{name:'EditGroup',params:{groupId:null}}">New</CButton>
                </template>
            </CDataTable>
            </CCardBody>
        </CCard>
    </div>
</template>

<script>

export default {
  name: 'Groups',
  components: {
  },
  data () {
    return {
      groups: [],
      groupFields:[
          {key:'name',label:'Name'},
          {key:'buttons',label:'',sorter:false,filter:false}
      ]
    }
  },
  methods: {
    getGroups() {
      this.$store.dispatch('loading','Loading...');
        this.$http.get('user/groups/'+this.selectedPatrolId)
            .then(response => {
                console.log(response);
                this.groups = response.data;
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
    removeGroup(groupId){
      this.$store.dispatch('loading','Removing...');
      this.$http.delete('user/groups/'+this.selectedPatrolId+'/'+groupId)
        .then(response=>{
          this.groups = _.filter(this.groups,function(x){return x.id!=groupId;});
        }).catch(response=>{
          console.log(response);
        }).finally(response=>this.$store.dispatch('loadingComplete'));
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
      this.getGroups();
    }
  },
  mounted: function(){
      this.getGroups();
  }
}
</script>

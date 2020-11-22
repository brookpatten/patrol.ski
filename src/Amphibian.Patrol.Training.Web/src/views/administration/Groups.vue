<template>
    <div>
      <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-people"/>
            </slot>
            </CCardHeader>
            <CCardBody>
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
                      <CButton color="primary" :to="{ name: 'ManageGroup', params: { groupId: data.item.id } }">Edit</CButton>
                      <CButton color="danger" v-on:click="removeGroup(data.item.id)">Remove</CButton>
                    </CButtonGroup>
                  </td>
                </template>
                <template #buttons-header>
                  <CButton color="primary" size="sm" :to="{name:'ManageGroup',params:{groupId:null}}">New</CButton>
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
        this.$http.get('user/groups/'+this.selectedPatrolId)
            .then(response => {
                console.log(response);
                this.groups = response.data;
            }).catch(response => {
                console.log(response);
            });
        },
    removeGroup(groupId){
      this.$http.delete('user/groups/'+this.selectedPatrolId+'/'+groupId)
        .then(response=>{
          this.groups = _.filter(this.groups,function(x){return x.id!=groupId;});
        }).catch(response=>{
          console.log(response);
        });
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

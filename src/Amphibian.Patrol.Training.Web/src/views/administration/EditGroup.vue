<template>
    <div>
      <CForm @submit.prevent="save">
      <CCard>
        <CCardHeader>
          <slot name="header">
            <CIcon name="cil-user"/>
          </slot>
        </CCardHeader>
        <CCardBody>
            <CAlert color="danger" v-if="validationMessage">{{validationMessage}}</CAlert>
            <CInput
            label="Name"
            v-model="group.name"
            :invalidFeedback="validationErrors.Name ? validationErrors.Name.join() : 'Invalid'"
            :isValid="validated ? validationErrors.Name==null : null"
            />
        </CCardBody>
        <CCardFooter>
            <CButtonGroup>
              <CButton color="secondary" :to="{ name: 'Groups'}">Back</CButton>
              <CButton type="submit" color="primary">Save</CButton>
            </CButtonGroup>
        </CCardFooter>
      </CCard>
      </CForm>

      <CCard v-if="plans && plans.length>0">
        <CCardHeader>
          <slot name="header">
            <CIcon name="cil-user"/> Plans
          </slot>
        </CCardHeader>
        <CCardBody>
          <CDataTable
                striped
                bordered
                small
                fixed
                :items="plans"
                :fields="[{key:'name',label:'Name'},{key:'buttons',label:''}]"
                sorter>
                <template #buttons="data">
                  <td>
                    <CButtonGroup size="sm">
                      <CButton v-if="hasPermission('MaintainPlans')" color="primary" :to="{ name: 'ManagePlan', params: { planId: data.item.id } }">Edit</CButton>
                    </CButtonGroup>
                  </td>
                </template>
            </CDataTable>
        </CCardBody>
      </CCard>

      <CCard v-if="members && members.length>0">
        <CCardHeader>
          <slot name="header">
            <CIcon name="cil-user"/> Members
          </slot>
        </CCardHeader>
        <CCardBody>
          <CDataTable
                striped
                bordered
                :small="small"
                :fixed="fixed"
                :items="members"
                :fields="[{key:'email',label:'Email'},{key:'firstName',label:'First'},{key:'lastName',label:'Last'},{key:'buttons',label:''}]"
                sorter>
                <template #buttons="data">
                  <td>
                    <CButtonGroup size="sm">
                      <CButton v-if="hasPermission('MaintainUsers')" color="primary" :to="{ name: 'EditUser', params: { userId: data.item.id } }">Edit</CButton>
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
  name: 'EditGroup',
  components: {
  },
  props: ['groupId'],
  data () {
    return {
      group:{},
      members:[],
      plans:[],
      validationMessage:'',
      validationErrors:{},
      validated:false
    }
  },
  methods: {
    hasPermission: function(permission){
      return this.selectedPatrol!=null && this.selectedPatrol.permissions!=null && _.indexOf(this.selectedPatrol.permissions,permission) >= 0;
    },
    getGroup() {
        if(this.groupId==0 || !this.groupId){
          this.group={
            name:'',
            patrolId: this.selectedPatrolId
          };
          this.members=[];
          this.plans=[];
        }
        else{
          this.$http.get('user/group/'+this.groupId)
            .then(response => {
                this.group = response.data.group;
                this.members = response.data.members;
                this.plans = response.data.plans;
                
                console.log(response);
            }).catch(response => {
                console.log(response);
            });
        }
    },
    save(){
        this.$http.put('user/group',this.group)
          .then(response=>{
            this.$router.push({name:'Groups'});
          }).catch(response=>{
            this.validated=true;
            this.validationMessage = response.response.data.title;
            this.validationErrors = response.response.data.errors;
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
        this.getGroup();
    }
  },
  mounted: function(){
    this.getGroup();
  }
}
</script>

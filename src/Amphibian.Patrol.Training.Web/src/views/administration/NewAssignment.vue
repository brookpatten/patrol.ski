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
            label="Assignment Due"
            type="date"
            v-model="selectedDueOn"
            />
            <CSelect
            label="Plan"
            :value.sync="selectedPlanId"
            :options="planSelectOptions"
            placeholder="None"
            />

            <label>User(s)</label>
            <div v-for="user in users" :key="user.id">
                <input type="checkbox" :value="user.id" v-model="user.selected"/> <label>{{user.lastName}} {{user.firstName}}</label>
            </div>

            
        </CCardBody>
        <CCardFooter>
            <CButtonGroup>
              <CButton color="secondary" :to="{name:'Assignments',params:{planId:this.selectedPlanId}}">Back</CButton>
              <CButton type="submit" color="primary">Save</CButton>
            </CButtonGroup>
        </CCardFooter>
      </CCard>
      </CForm>
    </div>
</template>

<script>

export default {
  name: 'NewAssignment',
  components: {
  },
  props: ['planId','userId'],
  data () {
    return {
      plans:[],
      users:[],
      selectedPlanId:0,
      selectedDueOn:'',
      validationMessage:'',
      validationErrors:{},
      validated:false
    }
  },
  methods: {
    getPlans() {
        this.$http.get('plans?patrolId='+this.selectedPatrolId)
            .then(response => {
                this.plans = response.data;
                console.log(response);
            }).catch(response => {
                console.log(response);
            });
    },
    getUsers(){
        this.$http.get('user/list/'+this.selectedPatrolId)
            .then(response => {
                console.log(response);
                this.users = response.data;
                for(var i=0;i<this.users.length;i++){
                  this.users[i].selected=false;
                }
            }).catch(response => {
                console.log(response);
            });
    },
    save(){
        if(this.selectedPlanId && this.selectedUserIds.length>0)
        {
          this.$http.post('assignments/create',{planId:this.selectedPlanId,toUserIds:this.selectedUserIds,dueAt:this.selectedDueOn})
            .then(response=>{
              this.$router.push({name:'Assignments',params:{planId:this.selectedPlanId}});
            }).catch(response=>{
              this.validated=true;
              this.validationMessage = response.response.data.title;
              this.validationErrors = response.response.data.errors;
            });
        }
    }
  },
  computed: {
    planSelectOptions: function(){
       return _.map(this.plans,function(p){
         return {label:p.name,value:p.id};
       });
    },
    selectedPatrolId: function () {
      return this.$store.state.selectedPatrolId;
    },
    selectedPatrol: function (){
        return this.$store.getters.selectedPatrol;
    },
    selectedUserIds: function(){
        return _.map(_.filter(this.users,'selected'),'id');
    }
  },
  watch: {
    selectedPatrolId(){
        this.getUsers();
        this.getPlans();
    }
  },
  mounted: function(){
    this.getPlans();
    this.getUsers();
  }
}
</script>

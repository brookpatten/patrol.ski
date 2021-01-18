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
            label="First Name"
            v-model="user.firstName"
            :invalidFeedback="validationErrors.FirstName ? validationErrors.FirstName.join() : 'Invalid'"
            :isValid="validated ? validationErrors.FirstName==null : null"
            />
            <CInput
            label="Last Name"
            v-model="user.lastName"
            :invalidFeedback="validationErrors.LastName ? validationErrors.LastName.join() : 'Invalid'"
            :isValid="validated ? validationErrors.LastName==null : null"
            />
            <!--validationErrors.Email != null ? validationErrors.Email.join() : null-->
            <CInput
            label="Email"
            v-model="user.email"
            v-if="!user.id || user.id==0"
            :invalidFeedback="validationErrors.Email ? validationErrors.Email.join() : 'Invalid'"
            :isValid="validated ? validationErrors.Email==null : null"
            />
            <CSelect
            label="Patrol Role"
            :value.sync="user.role"
            :options="[{label:'(None)',value:null},{value:'Administrator'},{value:'Coordinator'}]"
            placeholder="None"
            />

            <label>Group(s)</label>
            <div v-for="group in groups" :key="group.id">
                <CSwitch class="mx-1" color="primary" variant="3d" :checked.sync="group.selected" /><label>{{group.name}}</label>
            </div>

            
        </CCardBody>
        <CCardFooter>
            <CButtonGroup>
              <CButton color="secondary" :to="{ name: 'People'}">Back</CButton>
              <CButton type="submit" color="primary">Save</CButton>
            </CButtonGroup>
        </CCardFooter>
      </CCard>
      </CForm>
    </div>
</template>

<script>

export default {
  name: 'EditUser',
  components: {
  },
  props: ['userId'],
  data () {
    return {
      user:{},
      groups:[],
      validationMessage:'',
      validationErrors:{},
      validated:false
    }
  },
  methods: {
    getUser() {
        if(this.userId==0 || !this.userId){
          this.user={
            firstName:'',
            lastName:'',
            email:'',
            groups:[],
            role:null,
            patrolId: this.selectedPatrolId
          };
          this.getGroups();
        }
        else{
          this.$store.dispatch('loading','Loading...');
          this.$http.get('user/'+this.selectedPatrolId+'/'+this.userId)
            .then(response => {
                this.user = response.data;
                if(!this.user.groups){
                    this.user.groups=[];
                }
                console.log(response);
                this.getGroups();
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
        }
        },
    getGroups(){
      this.$store.dispatch('loading','Loading...');
        this.$http.get('user/groups/'+this.selectedPatrolId)
            .then(response => {
                console.log(response);

                var groups = response.data;

                for(var i=0;i<groups.length;i++){
                    groups[i].selected = false;
                    
                    if(this.user.groups && _.find(this.user.groups,{id:groups[i].id}) !=null){
                      groups[i].selected=true;
                    }
                }

                this.groups = groups;

                
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    save(){
      this.$store.dispatch('loading','Saving...');
        this.$http.put('user',this.user)
          .then(response=>{
            this.$router.push({name:'People'});
          }).catch(response=>{
            this.validated=true;
            this.validationMessage = response.response.data.title;
            this.validationErrors = response.response.data.errors;
          }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    isValid(obj,field){
      if(obj!=null){
        if(field!=null){
          return false;
        }
        else{
          return true;
        }
      }
      else{
        return false;
      }
    }
  },
  computed: {
    selectedPatrolId: function () {
      return this.$store.state.selectedPatrolId;
    },
    selectedPatrol: function (){
        return this.$store.getters.selectedPatrol;
    },
    selectedGroups: function(){
        return _.filter(this.groups,'selected');
    }
  },
  watch: {
    selectedGroups(){
      this.user.groups=this.selectedGroups;
    },
    selectedPatrolId(){
        this.getUser();
    }
  },
  mounted: function(){
    this.getUser();
  }
}
</script>

<template>
    <div>
      <CForm @submit.prevent="save">
      <CCard>
        <CCardHeader>
          <slot name="header">
            <CIcon name="cil-user"/>
          </slot> New Patrol
        </CCardHeader>
        <CCardBody>
            <CAlert color="success" v-if="patrols.length==0">You are not currently associated with a patrol.  You may start a new patrol, or you may need to wait until your training administrator adds you to their patrol.  Be sure you have used the same email address.</CAlert>
            <CAlert color="danger" v-if="validationMessage">{{validationMessage}}</CAlert>
            <CInput
            label="New Patrol Name"
            v-model="patrolName"
            />
            

            <label>Initial Setup</label>
            <CInputRadioGroup
            :options="[{value:'default',label:'Default (Create initial training plans and trainer groups which may be changed later)'},{value:'empty',label:'Empty (Create nothing other than an empty patrol, for experienced users only)'},{value:'demo',label:'Demo (Create a patrol with training plans, trainees, trainers, assignments, and sample signatures.  Great if you just want to see how things work)'}]"
            :checked.sync="initialType"
            />

            
        </CCardBody>
        <CCardFooter>
            <CButtonGroup class="float-right">
              <CButton v-if="patrolName.length>0 && patrolName.length<256" type="submit" color="primary">Create</CButton>
            </CButtonGroup>
        </CCardFooter>
      </CCard>
      </CForm>
    </div>
</template>

<script>

export default {
  name: 'NewPatrol',
  components: {
  },
  props: [],
  data () {
    return {
      patrolName:'',
      initialType:'default',
      validationMessage:'',
      validationErrors:{},
      validated:false
    }
  },
  methods: {
    save(){
        if(this.patrolName && this.patrolName.length>0 && this.patrolName.length<256)
        {
          this.$store.dispatch('loading','Loading...');
        
            //create the new patrl
            this.$http.post('patrol/create/'+this.initialType+'?name='+this.patrolName)
                .then(response=>{
                    var patrols = response.data;
                    var newPatrol = _.find(patrols,{'name':this.patrolName});
                    //update the local list of patrols
                    this.$store.dispatch('update_patrols',{patrols,id:newPatrol.id})
                    .then(()=>{
                        //change the currently selected patrol to the one we just created
                        this.$router.push('/');
                    })
                    .catch(err => {
                        console.log(err);
                    });
                }).catch(response=>{
                    this.validated=true;
                    this.validationMessage = response.response.data.title;
                    this.validationErrors = response.response.data.errors;
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        }
    }
  },
  computed: {
      patrols(){
          return this.$store.getters.patrols;
      }
  },
  watch: {
    
  },
  mounted: function(){
    
  }
}
</script>

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
            <CAlert color="success" v-if="patrols.length==0">You have not been added to a patrol.  You may start a new patrol, or you may need to wait until your patrol administrator adds you to their patrol.  Be sure you have logged in using the same email address you gave your patrol administrator.</CAlert>
            <CAlert color="danger" v-if="validationMessage">{{validationMessage}}</CAlert>
            <CInput
            label="New Patrol Name"
            v-model="patrolName"
            />

            <CSelect
            label="TimeZone"
            :value.sync="timeZone"
            :options="timeZones"
            placeholder="None"
            />
            

            <label>Initial Setup</label>
            <CInputRadioGroup
            :options="[{value:'default',label:'Default (Create initial training plans and trainer groups which may be changed later)'},{value:'empty',label:'Empty (Create nothing other than an empty patrol, for experienced users only)'},{value:'demo',label:'Demo (Create a patrol with training plans, trainees, trainers, assignments, and sample signatures.  Great if you just want to see how things work.)'}]"
            :checked.sync="initialType"
            />

            <br/>
            <label>Enabled Functionality</label> <em>(You can change this later)</em>
            <br/>
            <CSwitch class="mx-1" color="primary" variant="3d" shape="3d" :checked.sync="enableAnnouncements"/>
            <label for="enableAnnouncements">Announcements</label>
            <br/>
            <CSwitch class="mx-1" color="primary" variant="3d" shape="3d" :checked.sync="enableEvents"/>
            <label for="enableEvents">Events</label>
            <br/>
            <CSwitch class="mx-1" color="primary" variant="3d" shape="3d" :checked.sync="enableTraining"/>
            <label for="enableTraining">Training</label>
            <br/>
            <CSwitch class="mx-1" color="primary" variant="3d" shape="3d" :checked.sync="enableScheduling"/>
            <label for="enableScheduling">Scheduling</label>
            <br/>
            <template v-if="enableScheduling">
              <CSwitch class="mx-1" color="primary" variant="3d" shape="3d" :checked.sync="enableShiftSwaps"/>
              <label for="enableShiftSwaps">Shift Exchange</label>
              <br/>
            </template>

            
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
      timeZone:'Eastern Standard Time',
      enableAnnouncements:true,
      enableEvents:true,
      enableTraining:true,
      enableScheduling:true,
      enableShiftSwaps:true,
      initialType:'default',
      validationMessage:'',
      validationErrors:{},
      validated:false,
      timeZones: []
    }
  },
  methods: {
    save(){
        if(this.patrolName && this.patrolName.length>0 && this.patrolName.length<256)
        {
          this.$store.dispatch('loading','Loading...');

            var patrol = {
              name : this.patrolName,
              timeZone: this.timeZone,
              enableAnnouncements: this.enableAnnouncements,
              enableEvents: this.enableEvents,
              enableTraining:this.enableTraining,
              enableScheduling:this.enableScheduling,
              enableShiftSwaps:this.enableShiftSwaps
            }
        
            //create the new patrl
            this.$http.post('patrol/create/'+this.initialType,patrol)
                .then(response=>{
                    var patrols = response.data;
                    var newPatrol = _.find(patrols,{'name':this.patrolName});
                    //update the local list of patrols
                    this.$store.dispatch('update_patrols',{patrols,id:newPatrol.id})
                    .then(()=>{
                        //change the currently selected patrol to the one we just created
                        this.$router.push({name:"Home"});
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
    },
    getTimeZones() {
      this.$store.dispatch('loading','Loading...');
        this.$http.get('timezones')
          .then(response => {
              this.timeZones = response.data;
              console.log(response);
          }).catch(response => {
              console.log(response);
          }).finally(response=>this.$store.dispatch('loadingComplete'));
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
    this.getTimeZones();
  }
}
</script>

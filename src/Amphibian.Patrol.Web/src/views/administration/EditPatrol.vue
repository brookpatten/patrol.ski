<template>
    <div>
      <CForm @submit.prevent="save">
      <CCard>
        <CCardHeader>
          <slot name="header">
            <CIcon name="cil-home"/>
          </slot>
        </CCardHeader>
        <CCardBody>
            <em>Note: Other users will need to log out and back in to see changes to the patrol</em>
            <br/>
            <br/>

            <CAlert color="danger" v-if="validationMessage">{{validationMessage}}</CAlert>
            <CInput
            label="Name"
            v-model="editedPatrol.name"
            :invalidFeedback="validationErrors.Name ? validationErrors.Name.join() : 'Invalid'"
            :isValid="validated ? validationErrors.Name==null : null"
            />
            
            <CSelect
            label="TimeZone"
            :value.sync="editedPatrol.timeZone"
            :options="timeZones"
            placeholder="None"
            />

            <strong>Enabled Functionality</strong>
            <br/>
            
            
            <CSwitch class="mx-1" color="primary" variant="3d" shape="3d" :checked.sync="editedPatrol.enableAnnouncements"/>
            <label for="editedPatrol.enableAnnouncements">Announcements</label>
            <br/>
            <CSwitch class="mx-1" color="primary" variant="3d" shape="3d" :checked.sync="editedPatrol.enableEvents"/>
            <label for="editedPatrol.enableEvents">Events</label>
            <br/>
            <CSwitch class="mx-1" color="primary" variant="3d" shape="3d" :checked.sync="editedPatrol.enableTraining"/>
            <label for="editedPatrol.enableTraining">Training</label>
            <br/>
            <CSwitch class="mx-1" color="primary" variant="3d" shape="3d" :checked.sync="editedPatrol.enableScheduling"/>
            <label for="editedPatrol.enableScheduling">Scheduling</label>
            <br/>
            <template v-if="editedPatrol.enableScheduling">
              <CSwitch class="mx-1" color="primary" variant="3d" shape="3d" :checked.sync="editedPatrol.enableShiftSwaps"/>
              <label for="editedPatrol.enableShiftSwaps">Shift Exchange</label>
              <br/>
            </template>
            <CSwitch class="mx-1" color="primary" variant="3d" shape="3d" :checked.sync="editedPatrol.enableTimeClock"/>
            <label for="editedPatrol.enableTimeClock">Time Clock</label>
            <br/>
            <CSwitch class="mx-1" color="primary" variant="3d" shape="3d" :checked.sync="editedPatrol.enableWorkItems"/>
            <label for="editedPatrol.enableWorkItems">Work Items</label>
            <br/>


        </CCardBody>
        <CCardFooter>
            <CButtonGroup>
              <CButton type="submit" color="primary">Save</CButton>
            </CButtonGroup>
        </CCardFooter>
      </CCard>
      </CForm>
    </div>
</template>

<script>

export default {
  name: 'EditPatrol',
  components: {
  },
  props: [],
  data () {
    return {
      editedPatrol:{id:0,name:'',timeZone:'',enableTraining:false,enableAnnouncements:false,enableEvents:false,enableScheduling:false,enableShiftSwaps:false,enableTimeClock:false, enableWorkItems:false},
      validationMessage:'',
      validationErrors:{},
      validated:false,
      timeZones:[]
    }
  },
  methods: {
    save(){
        this.$store.dispatch('loading','Saving...');
        this.$http.post('patrol',this.editedPatrol)
          .then(response=>{
            //this.$store.dispatch('update_patrols',{patrols:response.data,id:this.editedPatrol.id});
            this.$router.push({name:'Home'});
          }).catch(response=>{
            this.validated=true;
            this.validationMessage = response.response.data.title;
            this.validationErrors = response.response.data.errors;
          }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    load(){
        this.editedPatrol={id:0,name:'',enableTraining:false,enableAnnouncements:false,enableEvents:false};
        this.editedPatrol.id = this.selectedPatrol.id;
        this.editedPatrol.name = this.selectedPatrol.name;
        this.editedPatrol.enableTraining = this.selectedPatrol.enableTraining;
        this.editedPatrol.enableAnnouncements = this.selectedPatrol.enableAnnouncements;
        this.editedPatrol.enableEvents = this.selectedPatrol.enableEvents;
        this.editedPatrol.enableScheduling = this.selectedPatrol.enableScheduling;
        this.editedPatrol.enableShiftSwaps = this.selectedPatrol.enableScheduling && this.selectedPatrol.enableShiftSwaps;
        this.editedPatrol.enableTimeClock = this.selectedPatrol.enableTimeClock;
        this.editedPatrol.enableWorkItems = this.selectedPatrol.enableWorkItems;
        this.editedPatrol.timeZone = this.selectedPatrol.timeZone;
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
        this.load();
    }
  },
  mounted: function(){
     this.getTimeZones();
     this.load();
  }
}
</script>

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
            
            
            <CSwitch class="mx-1" color="primary" variant="3d" :checked.sync="editedPatrol.enableAnnouncements"/>
            <label for="editedPatrol.enableAnnouncements">Announcements</label> <em>Publish & email announcements to members</em>
            <br/>
            <CSwitch class="mx-1" color="primary" variant="3d" :checked.sync="editedPatrol.enableEvents"/>
            <label for="editedPatrol.enableEvents">Events</label> <em>Publish & email calendar events to members</em>
            <br/>
            <template v-if="editedPatrol.enableAnnouncements || editedPatrol.enableEvents">
            <CSwitch class="mx-1" color="primary" variant="3d" :checked.sync="editedPatrol.enablePublicSite"/>
            <label for="editedPatrol.enablePublicSite">Public Site</label> <em>Publicly available site can display announcements and/or events marked public</em>
            <br/>
            <CInput :inline="true"
              label="Public Site Address"
              v-if="editedPatrol.enablePublicSite"
              v-model="editedPatrol.subdomain"
              :invalidFeedback="validationErrors.Subdomain ? validationErrors.Subdomain.join() : 'Invalid'"
              :isValid="validated ? validationErrors.Subdomain==null : null" class="col-sm-4"
            >
            <template #append-content>.Patrol.Ski</template>
            </CInput>
            <CRow>
              <CCol>
                <CInputFile
                    label="Logo Image" @change="logoFileUpload"
                />
                <template v-if="editedPatrol.logoImageUrl">
                  <img :src="editedPatrol.logoImageUrl" alt="patrol logo image" class="img-fluid rounded"/>
                  <br/>
                </template>
              </CCol>
              <CCol>
              <CInputFile
                  label="Background Image" @change="backgroundFileUpload"
              />
              <template v-if="editedPatrol.backgroundImageUrl">
                <img :src="editedPatrol.backgroundImageUrl" alt="patrol background image" class="img-fluid rounded"/>
                <br/>
                
              </template>
              </CCol>
            </CRow>
            </template>
            <CSwitch class="mx-1" color="primary" variant="3d" :checked.sync="editedPatrol.enableTraining"/>
            <label for="editedPatrol.enableTraining">Training</label> <em>Design training, create assignments, and track progress to completion</em>
            <br/>
            <CSwitch class="mx-1" color="primary" variant="3d" :checked.sync="editedPatrol.enableScheduling"/>
            <label for="editedPatrol.enableScheduling">Scheduling</label> <em>Assign shifts and hours to members</em>
            <br/>
            <template v-if="editedPatrol.enableScheduling">
              <CSwitch class="mx-1" color="primary" variant="3d" :checked.sync="editedPatrol.enableShiftSwaps"/>
              <label for="editedPatrol.enableShiftSwaps">Shift Exchange</label> <em>Allow members to release, claim, and trade shifts</em>
              <br/>
            </template>
            <CSwitch class="mx-1" color="primary" variant="3d" :checked.sync="editedPatrol.enableTimeClock"/>
            <label for="editedPatrol.enableTimeClock">Time Clock</label> <em>Allow members to clock in and track hours worked</em>
            <br/>
            <CSwitch class="mx-1" color="primary" variant="3d" :checked.sync="editedPatrol.enableWorkItems"/>
            <label for="editedPatrol.enableWorkItems">Work Items</label> <em>Assign work and track completion</em>
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
      editedPatrol:{id:0,name:'',timeZone:'',enableTraining:false,enableAnnouncements:false,enableEvents:false,enableScheduling:false,enableShiftSwaps:false,enableTimeClock:false, enableWorkItems:false,enablePublicSite:false},
      validationMessage:'',
      validationErrors:{},
      validated:false,
      timeZones:[],
      counter:0,
      backgroundFile: {}
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
        //this.editedPatrol={id:0,name:'',enableTraining:false,enableAnnouncements:false,enableEvents:false};
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
        this.editedPatrol.enablePublicSite = (this.selectedPatrol.enableAnnouncements || this.selectedPatrol.enableEvents) && this.selectedPatrol.enablePublicSite;
        this.editedPatrol.subdomain = this.selectedPatrol.subdomain;
        this.editedPatrol.backgroundImageUrl = this.selectedPatrol.backgroundImageUrl;
        this.editedPatrol.logoImageUrl = this.selectedPatrol.logoImageUrl;
    },
    backgroundFileUpload(file,e){
      this.$store.dispatch('loading','Uploading...');
      let formData = new FormData();
      formData.append('formFile', file[0]);
      formData.append('patrolId', this.editedPatrol.id);

      this.$http.post('file/upload',formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      })
      .then(response => {
          this.editedPatrol.backgroundImageUrl = response.data.relativeUrl;
          console.log(response.data.relativeUrl);
      }).catch(response => {
          console.log(response);
      }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    logoFileUpload(file,e){
      this.$store.dispatch('loading','Uploading...');
      let formData = new FormData();
      formData.append('formFile', file[0]);
      formData.append('patrolId', this.editedPatrol.id);

      this.$http.post('file/upload',formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      })
      .then(response => {
          this.editedPatrol.logoImageUrl = response.data.relativeUrl;
          console.log(response.data.relativeUrl);
      }).catch(response => {
          console.log(response);
      }).finally(response=>this.$store.dispatch('loadingComplete'));
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
    },
    backgroundFile(){
      console.log('file changed');
    }
    //editedPatrol: function(){
    //  this.counter++;
    //}
  },
  mounted: function(){
     this.getTimeZones();
     this.load();
  }
}
</script>

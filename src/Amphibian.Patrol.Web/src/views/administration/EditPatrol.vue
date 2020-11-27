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
            <strong>Enabled Functionality</strong>
            <br/>
            
            
            <CSwitch class="mx-1" color="primary" variant="3d" shape="3d" :checked.sync="editedPatrol.enableAnnouncements" v-bind="labelIcon"/>
            <label for="editedPatrol.enableAnnouncements">Announcements</label>
            <br/>
            <CSwitch class="mx-1" color="primary" variant="3d" shape="3d" :checked.sync="editedPatrol.enableEvents" v-bind="labelIcon"/>
            <label for="editedPatrol.enableEvents">Events</label>
            <br/>
            <CSwitch class="mx-1" color="primary" variant="3d" shape="3d" :checked.sync="editedPatrol.enableTraining" v-bind="labelIcon"/>
            <label for="editedPatrol.enableTraining">Training</label>
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
      editedPatrol:{id:0,name:'',enableTraining:false,enableAnnouncements:false,enableEvents:false},
      validationMessage:'',
      validationErrors:{},
      validated:false
    }
  },
  methods: {
    save(){
        this.$store.dispatch('loading','Saving...');
        this.$http.post('patrol',this.editedPatrol)
          .then(response=>{
            this.$store.dispatch('update_patrols',{patrols:response.data,id:this.editedPatrol.id});
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
        this.load();
    }
  },
  mounted: function(){
     this.load();
  }
}
</script>

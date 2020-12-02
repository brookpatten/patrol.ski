<template>
    <div>
      <CForm @submit.prevent="save">
      <CCard>
        <CCardHeader>
          <slot name="header">
              <CIcon name="cil-calendar"/>
          </slot>
        </CCardHeader>
        <CCardBody>
            <CAlert color="danger" v-if="validationMessage">{{validationMessage}}</CAlert>
            <CInput
            label="Name"
            v-model="event.name"
            :invalidFeedback="validationErrors.Name ? validationErrors.Name.join() : 'Invalid'"
            :isValid="validated ? validationErrors.Name==null : null"
            />
            <CInput
            label="Location"
            v-model="event.location"
            :invalidFeedback="validationErrors.Location ? validationErrors.Location.join() : 'Invalid'"
            :isValid="validated ? validationErrors.Location==null : null"
            />
            
            <label for="event.startsAt">Start</label>
            <VueCtkDateTimePicker v-model="event.startsAt" dark noClearButton minute-interval="15" color="#3b2fa4" format="YYYY-MM-DDTHH:mm"></VueCtkDateTimePicker><br/>
            <label for="event.endsAt">End</label>
            <VueCtkDateTimePicker v-model="event.endsAt" dark noClearButton minute-interval="15" color="#3b2fa4" format="YYYY-MM-DDTHH:mm"></VueCtkDateTimePicker><br/>

        </CCardBody>
        <CCardFooter>
            <CButtonGroup>
              <CButton color="secondary" :to="{ name: 'Announcements'}">Back</CButton>
              <CButton type="submit" color="primary">Save</CButton>
            </CButtonGroup>
        </CCardFooter>
      </CCard>
      </CForm>
    </div>
</template>

<script>

import Datepicker from 'vuejs-datepicker';

import VueCtkDateTimePicker from 'vue-ctk-date-time-picker';
import 'vue-ctk-date-time-picker/dist/vue-ctk-date-time-picker.css';

export default {
  name: 'EditEvent',
  components: { Datepicker, VueCtkDateTimePicker
  },
  props: ['eventId'],
  data () {
    return {
      event:{},
      validationMessage:'',
      validationErrors:{},
      validated:false
    }
  },
  methods: {
    hasPermission: function(permission){
      return this.selectedPatrol!=null && this.selectedPatrol.permissions!=null && _.indexOf(this.selectedPatrol.permissions,permission) >= 0;
    },
    getEvent() {
        if(this.eventId==0 || !this.eventId){
          this.event={
            name:'',
            location:'',
            startsAt:new Date(),
            endsAt:new Date(),
            patrolId: this.selectedPatrolId
          };
        }
        else{
          this.$store.dispatch('loading','Loading...');
          this.$http.get('event/'+this.eventId)
            .then(response => {
                this.event = response.data;
                console.log(response);
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
        }
    },
    save(){
        this.$store.dispatch('loading','Saving...');

        
        this.$http.post('events',this.event)
          .then(response=>{
            this.$router.push({name:'Events'});
          }).catch(response=>{
            this.validated=true;
            this.validationMessage = response.response.data.title;
            this.validationErrors = response.response.data.errors;
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
        this.getEvent();
    }
  },
  mounted: function(){
    this.getEvent();
  }
}
</script>

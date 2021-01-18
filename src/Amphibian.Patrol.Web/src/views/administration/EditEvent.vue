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
            <datetime type="datetime" v-model="event.startsAt" :minute-step="15" input-class="form-control" :use12-hour="true"></datetime><br/>
            <label for="event.endsAt">End</label>
            <datetime type="datetime" v-model="event.endsAt" :minute-step="15" input-class="form-control" :use12-hour="true"></datetime><br/>

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


import { Datetime } from 'vue-datetime';
import 'vue-datetime/dist/vue-datetime.css';

export default {
  name: 'EditEvent',
  components: { Datetime
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
            startsAt:new Date().toUTCString(),
            endsAt:new Date().toUTCString(),
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

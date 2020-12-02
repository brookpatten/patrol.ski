<template>
    <div>
      <CForm @submit.prevent="save">
      <CCard>
        <CCardHeader>
          <slot name="header">
              <CIcon name="cil-indent-increase"/>
              <span v-if="shiftId">Edit Shift</span>
              <span v-if="!shiftId">New Shift</span>
          </slot>
        </CCardHeader>
        <CCardBody>
            <CRow v-if="shiftId">
              <CCol>
                <CAlert color="info">Modifying shift times here will NOT update start/end times on schedule entries which were created using this shift.</CAlert>
              </CCol>
            </CRow>
            <CRow v-if="validationMessage">
              <CCol>
                <CAlert color="danger">{{validationMessage}}</CAlert>
              </CCol>
            </CRow>
            <CRow>
              <CCol>
                <CInput
                label="Name"
                v-model="shift.name"
                :invalidFeedback="validationErrors.Name ? validationErrors.Name.join() : 'Invalid'"
                :isValid="validated ? validationErrors.Name==null : null"
                />
              </CCol>
            </CRow>
            <CRow>
              <CCol>
                <label>Start</label>
                <CForm inline>
                  <CSelect
                  :value.sync="shift.startHour"
                  :options = "hours"
                  :invalidFeedback="validationErrors.StartHour ? validationErrors.StartHour.join() : 'Invalid'"
                  :isValid="validated ? validationErrors.StartHour==null : null"
                  />:
                  <CSelect
                  :value.sync="shift.startMinute"
                  :options = "minutes"
                  :invalidFeedback="validationErrors.StartMinute ? validationErrors.StartMinute.join() : 'Invalid'"
                  :isValid="validated ? validationErrors.StartMinute==null : null"
                  />
                  </CForm>
              </CCol>
              <CCol>
                <label>End</label>
                <CForm inline>
                  <CSelect
                  :value.sync="shift.endHour"
                  :options = "hours"
                  :invalidFeedback="validationErrors.EndHour ? validationErrors.EndHour.join() : 'Invalid'"
                  :isValid="validated ? validationErrors.EndHour==null : null"
                  />:
                  <CSelect
                  :value.sync="shift.endMinute"
                  :options = "minutes"
                  :invalidFeedback="validationErrors.EndMinute ? validationErrors.EndMinute.join() : 'Invalid'"
                  :isValid="validated ? validationErrors.EndMinute==null : null"
                  />
                </CForm>
              </CCol>
            </CRow>
        </CCardBody>
        <CCardFooter>
            <CButtonGroup>
              <CButton color="secondary" :to="{ name: 'Shifts'}">Back</CButton>
              <CButton type="submit" color="primary">Save</CButton>
            </CButtonGroup>
        </CCardFooter>
      </CCard>
      </CForm>
    </div>
</template>

<script>

export default {
  name: 'EditShift',
  components: { },
  props: ['shiftId'],
  data () {
    return {
      shift:{},
      validationMessage:'',
      validationErrors:{},
      validated:false,
      hours:[],
      minutes:[]
    }
  },
  methods: {
    hasPermission: function(permission){
      return this.selectedPatrol!=null && this.selectedPatrol.permissions!=null && _.indexOf(this.selectedPatrol.permissions,permission) >= 0;
    },
    getShift() {
        if(this.shiftId==0 || !this.shiftId){
          this.event={
            name:'',
            startHour:0,
            startMinute:0,
            endHour:14,
            endMinute:0,
            patrolId: this.selectedPatrolId
          };
        }
        else{
          this.$store.dispatch('loading','Loading...');
          this.$http.get('schedule/shift?shiftId='+this.shiftId)
            .then(response => {
                this.shift = response.data;
                console.log(response);
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
        }
    },
    save(){
        this.$store.dispatch('loading','Saving...');

        this.shift.patrolId = this.selectedPatrolId;
        this.$http.post('schedule/shift',this.shift)
          .then(response=>{
            this.$router.push({name:'Shifts'});
          }).catch(response=>{
            this.validated=true;
            this.validationMessage = response.response.data.title;
            this.validationErrors = response.response.data.errors;
          }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    populateTimeDropdowns(){
      for(var i=0;i<24;i++)
      {
        this.hours.push(i);
      }
      for(var i=0;i<60;i++)
      {
        this.minutes.push({
          value:i,
          label: (i+"").padStart(2,"0")
        });
      }
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
        this.getShift();
    }
  },
  mounted: function(){
    this.getShift();
    this.populateTimeDropdowns();
  }
}
</script>

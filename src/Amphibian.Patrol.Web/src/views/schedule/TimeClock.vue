<template>
    <div>
        <CCard :border-color="timeEntry.id ? (timeEntry.clockOut ? 'danger' : 'primary') : 'info'">
            <CCardBody>
                <CRow>
                  <CCol md="4">
                    <CButtonGroup>
                      <CButton color="success" size="lg" v-if="!timeEntry.id || timeEntry.clockOut" @click="clockIn">Clock In</CButton>
                      <CButton color="danger" size="lg" v-if="timeEntry.id && !timeEntry.clockOut" @click="clockOut">Clock Out</CButton>
                    </CButtonGroup>
                  </CCol>
                  <CCol md="8">
                    <CRow v-if="timeEntry.id && timeEntry.clockIn">
                      <CCol md="4"><label>Clocked In:</label></CCol>
                      <CCol md="8">{{new Date(timeEntry.clockIn).toLocaleTimeString() }}</CCol>
                    </CRow>
                    <CRow v-if="timeEntry.id && timeEntry.clockOut">
                      <CCol md="4"><label>Clocked Out:</label> </CCol>
                      <CCol md="8">{{new Date(timeEntry.clockOut).toLocaleTimeString() }}</CCol>
                    </CRow>
                    <CRow v-if="timeEntry.id && timeEntry.clockIn && !timeEntry.clockOut">
                      <CCol md="4"><label>Hours:</label></CCol>
                      <CCol md="8">{{duration(timeEntry.clockIn,now)}}</CCol>
                    </CRow>
                    <CRow v-if="timeEntry.id && timeEntry.clockIn && timeEntry.clockOut">
                      <CCol md="4"><label>Hours:</label></CCol>
                      <CCol md="8">{{duration(timeEntry.clockIn,timeEntry.clockOut)}}</CCol>
                    </CRow>
                  </CCol>
                </CRow>
            </CCardBody>
        </CCard>
    </div>
</template>

<script>

export default {
  name: 'TimeClockCurrent',
  components: {
  },
  data () {
    return {
      timeEntry:{},
      timer: '',
      now:new Date()
    }
  },
  methods: {
    duration(from,to){
      var diffMillis = new Date(to) - new Date(from);
      var diffDate = new Date(diffMillis);
      return diffDate.getUTCHours()+":"+(diffDate.getUTCMinutes()+"").padStart(2,"0")+":"+(diffDate.getUTCSeconds()+"").padStart(2,"0");
    },
    getCurrentTimeEntry() {
      this.$store.dispatch('loading','Loading...');
        this.$http.get('timeclock/current/mine/'+this.selectedPatrolId)
            .then(response => {
                console.log("timeEntry:",response.data);
                this.timeEntry = response.data;
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    clockIn() {
      this.$store.dispatch('loading','Loading...');
        this.$http.post('timeclock/clock-in/'+this.selectedPatrolId)
            .then(response => {
                console.log("clock-in",response.data);
                this.timeEntry = response.data;
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    clockOut() {
      this.$store.dispatch('loading','Loading...');
        this.$http.post('timeclock/clock-out/'+this.timeEntry.id)
            .then(response => {
                console.log("clock-out",response.data);
                this.timeEntry = response.data;
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    refresh(){
      this.getCurrentTimeEntry();
    },
    tick(){
      this.now = new Date();
    },
    startTimer(){
      this.timer = setInterval(this.tick,1000);
    },
    stopTimer(){
      clearInterval(this.timer);
    }
  },
  computed: {
    selectedPatrolId: function () {
      return this.$store.state.selectedPatrolId;
    },
    selectedPatrol: function (){
        return this.$store.getters.selectedPatrol;
    },
  },
  watch: {
    selectedPatrolId(){
      this.refresh();
    },
  },
  mounted: function(){
    this.refresh();
    this.startTimer();
  },
  beforeDestroy: function(){
    this.stopTimer();
  }
}
</script>

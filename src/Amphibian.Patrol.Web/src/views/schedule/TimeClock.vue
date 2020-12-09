<template>
    <div>
        <CCard :border-color="timeEntry && timeEntry.id ? (timeEntry.clockOut ? 'danger' : 'primary') : 'info'">
            <CCardHeader>
              <CIcon name="cil-bell"/>Time Clock
              <CButtonGroup class="float-right">
                <CButton color="success" size="sm" v-if="!timeEntry || !timeEntry.id || timeEntry.clockOut" @click="clockIn">Clock In</CButton>
                <CButton color="danger" size="sm" v-if="timeEntry && timeEntry.id && !timeEntry.clockOut" @click="clockOut">Clock Out</CButton>
              </CButtonGroup>
            </CCardHeader>
            <CCardBody v-if="(timeEntry && timeEntry.id) || (scheduledShift && scheduledShift.id)">
                <CRow>
                  <CCol md="6">
                    <CRow v-if="timeEntry && timeEntry.id && timeEntry.clockIn && !timeEntry.clockOut">
                      <CCol><span class="display-3">{{duration(timeEntry.clockIn,now)}}</span></CCol>
                    </CRow>
                    <CRow v-if="timeEntry && timeEntry.id && timeEntry.clockIn && timeEntry.clockOut">
                      <CCol><span class="display-3">{{duration(timeEntry.clockIn,timeEntry.clockOut)}}</span></CCol>
                    </CRow>
                    <CRow v-if="!timeEntry">
                      <CCol><span class="display-3">Out</span></CCol>
                    </CRow>
                  </CCol>
                  <CCol md="6">
                    <CRow v-if="timeEntry && timeEntry.id && timeEntry.clockIn">
                      <CCol><label>Clocked In:</label></CCol>
                      <CCol>{{new Date(timeEntry.clockIn).toLocaleTimeString() }}</CCol>
                    </CRow>
                    <CRow v-if="timeEntry && timeEntry.id && timeEntry.clockOut">
                      <CCol><label>Clocked Out:</label> </CCol>
                      <CCol>{{new Date(timeEntry.clockOut).toLocaleTimeString() }}</CCol>
                    </CRow>
                    <CRow v-if="timeEntry && scheduledShift && scheduledShift.id && ((shift && shift.id) || (group && group.id))">
                      <CCol>
                        <label>Shift:</label>
                      </CCol>
                      <CCol>
                        <template v-if="shift"><strong>{{shift.name}}</strong></template>  <template v-if="group"><em>{{group.name}}</em></template>
                      </CCol>
                    </CRow>
                    <CRow v-if="timeEntry && scheduledShift && scheduledShift.id">
                      <CCol>
                        <label>Scheduled:</label>
                      </CCol>
                      <CCol>
                        {{(new Date(scheduledShift.startsAt)).toLocaleTimeString()}} - {{(new Date(scheduledShift.endsAt)).toLocaleTimeString()}}
                      </CCol>
                    </CRow>

                    <CRow v-if="!timeEntry && scheduledShift && scheduledShift.id">
                      <CCol>
                        <label>Next Shift:</label>
                      </CCol>
                      <CCol>
                        <template v-if="shift"><strong>{{shift.name}}</strong></template>  <template v-if="group"><em>{{group.name}}</em></template>
                      </CCol>
                    </CRow>
                    <CRow v-if="!timeEntry && scheduledShift && scheduledShift.id">
                      <CCol>
                      </CCol>
                      <CCol>
                        {{(new Date(scheduledShift.startsAt)).toLocaleDateString()}} {{(new Date(scheduledShift.startsAt)).toLocaleTimeString()}} - {{(new Date(scheduledShift.endsAt)).toLocaleTimeString()}}
                      </CCol>
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
      scheduledShift:{},
      shift:{},
      group:{},
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
      this.$store.dispatch('loading','Checking Time Clock...');
        this.$http.get('timeclock/current/mine/'+this.selectedPatrolId)
            .then(response => {
                console.log("timeEntry:",response.data);
                this.timeEntry = response.data.timeEntry;
                this.scheduledShift = response.data.scheduledShift;
                this.shift = response.data.shift;
                this.group = response.data.group;
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    clockIn() {
      this.$store.dispatch('loading','Clocking In...');
        this.$http.post('timeclock/clock-in/'+this.selectedPatrolId)
            .then(response => {
                console.log("clock-in",response.data);
                this.timeEntry = response.data.timeEntry;
                this.scheduledShift = response.data.scheduledShift;
                this.shift = response.data.shift;
                this.group = response.data.group;
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    clockOut() {
      this.$store.dispatch('loading','Clocking Out...');
        this.$http.post('timeclock/clock-out/'+this.timeEntry.id)
            .then(response => {
                console.log("clock-out",response.data);
                this.timeEntry = response.data.timeEntry;
                this.scheduledShift = response.data.scheduledShift;
                this.shift = response.data.shift;
                this.group = response.data.group;
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

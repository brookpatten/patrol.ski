<template>
    <div v-if="timeEntrys && timeEntrys.length>0">
        <CCard id="whos-on">
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-bell"/>Who's On
            </slot>
            </CCardHeader>
            <CCardBody>
                <CRow>
                  <CCol>
                    <CDataTable
                        striped
                        bordered
                        small
                        fixed
                        :items="timeEntrys"
                        :fields="timeEntryFields"
                        sorter>
                        <template #time="data">
                            <td>
                                {{new Date(data.item.timeEntry.clockIn).toLocaleTimeString()}}
                            </td>
                        </template>
                        <template #hours="data">
                            <td>
                                {{duration(data.item.timeEntry.clockIn,now)}}
                            </td>
                        </template>
                        <template #user="data">
                            <td>
                                {{data.item.user.lastName}}, {{data.item.user.firstName}}
                            </td>
                        </template>
                        <template #shift="data">
                            <td>
                                <strong v-if="data.item.shift">{{data.item.shift.name}}</strong>&nbsp; 
                                <em v-if="data.item.group">{{data.item.group.name}}</em>
                                <template v-if="data.item.scheduledShift">
                                {{new Date(data.item.scheduledShift.startsAt).getHours()}}:{{(new Date(data.item.scheduledShift.startsAt).getMinutes()+"").padStart(2,"0")}}
                                -
                                {{new Date(data.item.scheduledShift.endsAt).getHours()}}:{{(new Date(data.item.scheduledShift.endsAt).getMinutes()+"").padStart(2,"0")}}
                                </template>
                            </td>
                        </template>
                        
                    </CDataTable>
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
      timeEntrys: [],
      timeEntryFields:[],
      timer: '',
      now:new Date(),
      lastRefresh: new Date()
    }
  },
  methods: {
    duration(from,to){
      var diffMillis = new Date(to) - new Date(from);
      var diffDate = new Date(diffMillis);
      return diffDate.getUTCHours()+":"+(diffDate.getUTCMinutes()+"").padStart(2,"0")+":"+(diffDate.getUTCSeconds()+"").padStart(2,"0");
    },
    getTimeEntrys() {
      if(this.selectedPatrol.enableScheduling){
        this.timeEntryFields=[
          {key:'user',label:''},
          {key:'time',label:'Clocked In'},
          {key:'hours',label:'Hours'},
          {key:'shift',label:'Shift'}
        ];
      }
      else{
        this.timeEntryFields=[
          {key:'user',label:''},
          {key:'time',label:'Clocked In'},
          {key:'hours',label:'Hours'}
        ];
      }
      //this.$store.dispatch('loading','Loading...');
        this.$http.get('timeclock/active/'+this.selectedPatrolId)
            .then(response => {
                console.log(response);
                this.timeEntrys = response.data;
            }).catch(response => {
                console.log(response);
            });//.finally(response=>this.$store.dispatch('loadingComplete'));
    },
    hasPermission: function(permission){
      return this.selectedPatrol!=null && this.selectedPatrol.permissions!=null && _.indexOf(this.selectedPatrol.permissions,permission) >= 0;
    },
    refresh(){
      this.getTimeEntrys();
      this.lastRefresh = new Date();
    },
    tick(){
      this.now = new Date();

      if(this.now - this.lastRefresh > 1000 * 60){
        this.refresh();
      }
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
    this.startTimer();
    this.refresh();
  },
  beforeDestroy: function(){
    this.stopTimer();
  }
}
</script>

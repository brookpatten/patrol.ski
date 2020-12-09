<template>
    <div v-if="timeEntrys && timeEntrys.length>0">
        <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-bell"/>Who's Late / MIA
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
                        <template #late="data">
                            <td>
                                <template v-if="durationMillis(data.item.scheduledShift.startsAt,now) < 1000 * 60 * 10">
                                  {{duration(data.item.scheduledShift.startsAt,now)}}
                                </template>
                                <CBadge v-if="durationMillis(data.item.scheduledShift.startsAt,now) >= 1000 * 60 * 10" color="danger">
                                  {{duration(data.item.scheduledShift.startsAt,now)}}
                                </CBadge>
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
      timeEntryFields:[
          {key:'user',label:''},
          {key:'late',label:'Late'},
          {key:'shift',label:'Shift'}
      ],
      timer: '',
      now:new Date(),
      lastRefresh: new Date()
    }
  },
  methods: {
    durationMillis(from,to){
      return new Date(to) - new Date(from);
    },
    duration(from,to){
      var diffMillis = this.durationMillis(from,to);
      var diffDate = new Date(diffMillis);
      return diffDate.getUTCHours()+":"+(diffDate.getUTCMinutes()+"").padStart(2,"0")+":"+(diffDate.getUTCSeconds()+"").padStart(2,"0");
    },
    getTimeEntrys() {
      //this.$store.dispatch('loading','Loading...');
        this.$http.get('timeclock/late/'+this.selectedPatrolId)
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
      this.refresh();
      this.startTimer();
  },
  beforeDestroy: function(){
    this.stopTimer();
  }
}
</script>

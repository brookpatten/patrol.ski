<template>
    <div>
        <CCard id="time-missing">
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-list"/>Time Missing
            </slot>
            </CCardHeader>
            <CCardBody>
                <CRow>
                  <CCol>
                    <label for="from">Begin</label>
                    <datepicker v-model="from" input-class="form-control" calendar-class="card"></datepicker>
                  </CCol>
                  <CCol>
                    <label for="to">End</label>
                    <datepicker v-model="to" input-class="form-control" calendar-class="card"></datepicker>
                  </CCol>
                  <CCol>
                    <CSelect
                    v-if="userItems.length>0"
                    label="User"
                    :value.sync="selectedUserId"
                    :options="userItems"
                    placeholder="None"
                    />
                  </CCol>
                </CRow>
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
                        <template #date="data">
                            <td>
                                {{new Date(data.item.scheduledShift.startsAt).toLocaleDateString()}}
                            </td>
                        </template>
                        <template #timeEntry="data">
                            <td>
                               <template v-if="data.item.timeEntry && data.item.timeEntry.clockIn && new Date(data.item.timeEntry.clockIn) > new Date(1970,0,0,0,0,0)">
                                  {{new Date(data.item.timeEntry.clockIn).getHours()}}:{{(new Date(data.item.timeEntry.clockIn).getMinutes()+"").padStart(2,"0")}}
                                </template>
                                <template v-if="data.item.timeEntry && data.item.timeEntry.clockOut">
                                  -
                                  {{new Date(data.item.timeEntry.clockOut).getHours()}}:{{(new Date(data.item.timeEntry.clockOut).getMinutes()+"").padStart(2,"0")}}
                                </template>
                            </td>
                        </template>
                        <template #shiftTimes="data">
                            <td>
                                <template v-if="data.item.scheduledShift">
                                  {{new Date(data.item.scheduledShift.startsAt).getHours()}}:{{(new Date(data.item.scheduledShift.startsAt).getMinutes()+"").padStart(2,"0")}}
                                  -
                                  {{new Date(data.item.scheduledShift.endsAt).getHours()}}:{{(new Date(data.item.scheduledShift.endsAt).getMinutes()+"").padStart(2,"0")}}
                                </template>
                            </td>
                        </template>
                        <template #user="data">
                            <td>
                                {{data.item.user.lastName}}, {{data.item.user.firstName}}
                            </td>
                        </template>
                        <template #duration="data">
                            <td>
                                {{duration(data.item.timeEntryScheduledShiftAssignment.durationSeconds)}} / {{duration(data.item.scheduledShift.durationSeconds)}}
                            </td>
                        </template>
                        <template #shiftName="data">
                            <td>
                              <template v-if="data.item.shift">
                                <strong v-if="data.item.shift">{{data.item.shift.name}}</strong>&nbsp; 
                                <em v-if="data.item.group">{{data.item.group.name}}</em>
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

import Datepicker from 'vuejs-datepicker';

export default {
  name: 'TimeEntries',
  components: { Datepicker
  },
  data () {
    return {
      from: new Date(new Date().getTime() - (86400000 * 30)),
      to: new Date(),
      timeEntrys: [],
      timeEntryFields:[
          {key:'date',label:'Date'},
          {key:'user',label:'User'},
          {key:'timeEntry',label:'Clock In/Out'},
          {key:'shiftTimes',label:'Shift Start/End'},
          {key:'duration',label:'Worked/Scheduled'},
          {key:'shiftName',label:'Shift'}
      ],

      users: [],
      selectedUserId: 0
    }
  },
  props: ['uid'],
  methods: {
    duration(seconds){
      var diffDate = new Date(seconds * 1000);
      return diffDate.getUTCHours()+":"+(diffDate.getUTCMinutes()+"").padStart(2,"0")+":"+(diffDate.getUTCSeconds()+"").padStart(2,"0");
    },
    getTimeEntrys() {
      if(this.selectedUserId==0 && this.uid){
        this.selectedUserId = this.uid;
      }
      else if(this.selectedUserId==0){
        this.selectedUserId = this.userId;
      }

      this.$store.dispatch('loading','Loading...');
        this.$http.post('timeclock/missing',{patrolId:this.selectedPatrolId,userId:this.selectedUserId,from:this.from,to:this.to})
            .then(response => {
                console.log(response);
                this.timeEntrys = response.data;
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    getUsers() {
      this.$store.dispatch('loading','Loading...');
      this.$http.get('user/list/'+this.selectedPatrolId)
          .then(response => {
              console.log(response);
              this.users = response.data;
          }).catch(response => {
              console.log(response);
          }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    hasPermission: function(permission){
      return this.selectedPatrol!=null && this.selectedPatrol.permissions!=null && _.indexOf(this.selectedPatrol.permissions,permission) >= 0;
    },
    refresh(){
      if(this.hasPermission('MaintainTimeClock')){
        this.getUsers();
      }
      this.getTimeEntrys();
    }
  },
  computed: {
    selectedPatrolId: function () {
      return this.$store.state.selectedPatrolId;
    },
    selectedPatrol: function (){
        return this.$store.getters.selectedPatrol;
    },
    userId: function (){
        return this.$store.getters.userId;
    },
    userItems: function(){
        var items = _.map(this.users,function(s){
            return {
                value: s.id,
                label: s.lastName+', '+s.firstName
            }
        });
        items.splice(0,0,{value:null,label:'(All)'})
        return items;
    }
  },
  watch: {
    selectedPatrolId(){
      this.refresh();
    },
    from(){
      this.getTimeEntrys();
    },
    to(){
      this.getTimeEntrys();
    },
    selectedUserId(){
      this.getTimeEntrys();
    }
  },
  mounted: function(){
      this.refresh();
  }
}
</script>

<template>
    <div>
        <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-list"/> Schedule Swap
            </slot>
            </CCardHeader>
            <CCardBody>
                <CDataTable
                    striped
                    bordered
                    small
                    fixed
                    :items="scheduledShifts"
                    :fields="scheduledShiftFields"
                    sorter>
                    <template #name="data">
                        <td>
                            <strong v-if="data.item.shiftId>0">{{data.item.shiftName}}</strong>&nbsp;
                            <em v-if="data.item.groupId">{{data.item.groupName}}</em>
                        </td>
                    </template>
                    <template #date="data">
                        <td>{{new Date(data.item.startsAt).toLocaleDateString()}}</td>
                    </template>
                    <template #startsAt="data">
                        <td>{{new Date(data.item.startsAt).getHours()+":"+(new Date(data.item.startsAt).getMinutes()+"").padStart(2,"0")}}</td>
                    </template>
                    <template #endsAt="data">
                        <td>{{new Date(data.item.endsAt).getHours()+":"+(new Date(data.item.endsAt).getMinutes()+"").padStart(2,"0")}}</td>
                    </template>
                    <template #assignee="data">
                        <td>
                            <CAlert color="info" v-for="assignment in data.item.assignments" :key="data.item.scheduledShiftId+'-'+assignment.id">
                              <span v-if="assignment.assignedUser">{{assignment.assignedUser.lastName}}, {{assignment.assignedUser.firstName}}</span>
                              <span v-if="!assignment.assignedUser">Available</span>
                              <CButton size="sm" color="success" v-if="assignment.status=='Released'" @click="claimScheduledShiftAssignment(assignment)" class="float-right">Claim</CButton>
                              <CButton size="sm" color="info" disabled v-if="assignment.status=='Claimed'" class="float-right">Claimed</CButton>
                            </CAlert>
                        </td>
                    </template>
                </CDataTable>
            </CCardBody>
        </CCard>
    </div>
</template>

<script>

export default {
  name: 'ScheduleSwap',
  components: {
  },
  data () {
    return {
      scheduledShifts: [],
      scheduledShiftFields:[
          {key:'name',label:'Name',sortable:true},
          {key:'date',label:'Date'},
          {key:'startsAt',label:'Start',sortable:true},
          {key:'endsAt',label:'End',sortable:true},
          {key:'assignee',label:'Released By'}
      ]
    }
  },
  methods: {
    getScheduledShifts() {
      this.$store.dispatch('loading','Loading...');
        this.$http.post('schedule/search',{patrolId:this.selectedPatrolId,from:new Date(),status:'Released'})
            .then(response => {
                console.log(response);
                this.scheduledShifts = response.data;
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
    claimScheduledShiftAssignment(scheduledShiftAssignment){
      this.$store.dispatch('loading','Claiming...');
      this.$http.post('schedule/scheduled-shift-assignment/claim?scheduledShiftAssignmentId='+scheduledShiftAssignment.id)
        .then(response=>{
          scheduledShiftAssignment.status='Claimed';
          scheduledShiftAssignment.claimedByUser = this.user;
        }).catch(response=>{
          console.log(response);
        }).finally(response=>this.$store.dispatch('loadingComplete'));
    }
  },
  computed: {
    selectedPatrolId: function () {
      return this.$store.state.selectedPatrolId;
    },
    selectedPatrol: function (){
        return this.$store.getters.selectedPatrol;
    },
    user: function (){
        return this.$store.getters.user;
    }
  },
  watch: {
    selectedPatrolId(){
      this.getScheduledShifts();
    }
  },
  mounted: function(){
      this.getScheduledShifts();
  }
}
</script>

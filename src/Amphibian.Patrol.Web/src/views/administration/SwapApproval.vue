<template>
    <div>
        <CCard id="swap-approval">
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-list"/> Schedule Swap Approval
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
                        <td>
                          {{new Date(data.item.startsAt).toLocaleDateString()}}
                          {{new Date(data.item.startsAt).getHours()+":"+(new Date(data.item.startsAt).getMinutes()+"").padStart(2,"0")}}
                          -
                          {{new Date(data.item.endsAt).getHours()+":"+(new Date(data.item.endsAt).getMinutes()+"").padStart(2,"0")}}
                        </td>
                    </template>
                    <template #assignee="data">
                        <td>
                            <CAlert color="info" v-for="assignment in data.item.assignments" :key="data.item.scheduledShiftId+'-'+assignment.id">
                              <span v-if="assignment.assignedUser">{{assignment.assignedUser.lastName}}, {{assignment.assignedUser.firstName}}</span>
                              <span v-if="!assignment.assignedUser">Available</span>
                              => 
                              <span v-if="assignment.claimedByUser">{{assignment.claimedByUser.lastName}}, {{assignment.claimedByUser.firstName}}</span>
                              <CButtonGroup class="float-right">
                                <CButton size="sm" color="success" v-if="assignment.status=='Claimed'" @click="approveScheduledShiftAssignment(assignment)">Approve</CButton>
                                <CButton size="sm" color="info" disabled v-if="assignment.status=='Approved'">Approved</CButton>
                                <CButton size="sm" color="danger" v-if="assignment.status=='Claimed'" @click="rejectScheduledShiftAssignment(assignment)">Reject</CButton>
                                <CButton size="sm" color="warning" disabled v-if="assignment.status=='Rejected'">Rejected</CButton>
                              </CButtonGroup>
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
  name: 'SwapApproval',
  components: {
  },
  data () {
    return {
      scheduledShifts: [],
      scheduledShiftFields:[
          {key:'date',label:'Date'},
          {key:'name',label:'',sortable:false},
          {key:'assignee',label:'From => To'}
      ]
    }
  },
  methods: {
    getScheduledShifts() {
      this.$store.dispatch('loading','Loading...');
        this.$http.post('schedule/search',{patrolId:this.selectedPatrolId,from:new Date(),status:'Claimed'})
            .then(response => {
                console.log(response);
                this.scheduledShifts = response.data;
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
    approveScheduledShiftAssignment(scheduledShiftAssignment){
      this.$store.dispatch('loading','Approving...');
      this.$http.post('schedule/scheduled-shift-assignment/approve?scheduledShiftAssignmentId='+scheduledShiftAssignment.id)
        .then(response=>{
          scheduledShiftAssignment.status='Approved';
          //scheduledShiftAssignment.assignedUser = scheduledShiftAssignment.claimedByUser;
          //scheduledShiftAssignment.claimedByUser = null;
        }).catch(response=>{
          console.log(response);
        }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    rejectScheduledShiftAssignment(scheduledShiftAssignment){
      this.$store.dispatch('loading','Rejecting...');
      this.$http.post('schedule/scheduled-shift-assignment/reject?scheduledShiftAssignmentId='+scheduledShiftAssignment.id)
        .then(response=>{
          scheduledShiftAssignment.status='Rejected';
          //scheduledShiftAssignment.claimedByUser = null;
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
    userId: function (){
        return this.$store.getters.userId;
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

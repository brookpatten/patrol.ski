<template>
    <div>
        <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-task"/>Work Items
                <CButton size="sm" color="success" @click="newWorkItem" class="float-right">New</CButton>
            </slot>
            </CCardHeader>
            <CCardBody>
                <CRow>
                  <CCol>
                    <label for="from">From Date</label>
                    <datepicker v-model="from" input-class="form-control" calendar-class="card"></datepicker>
                  </CCol>
                  <CCol>
                    <label for="to">To Date</label>
                    <datepicker v-model="to" input-class="form-control" calendar-class="card"></datepicker>
                  </CCol>
                  <CCol>
                    <CSelect
                    v-if="completeStatuses.length>0"
                    label="Complete"
                    :value.sync="complete"
                    :options="completeStatuses"
                    />
                  </CCol>
                  <CCol>
                    <CSelect
                    v-if="users.length>0"
                    label="Completed By"
                    :value.sync="completedByUserId"
                    :options="users"
                    />
                  </CCol>
                </CRow>
                <CRow>
                  <CCol>
                    <CInput v-model="name" label="Name"/>
                  </CCol>
                  <CCol>
                    <CInput v-model="location" label="Location"/>
                  </CCol>
                  <CCol>
                    <CSelect
                    v-if="recurringWorkItems.length>0"
                    label="Recurring"
                    :value.sync="recurringWorkItemId"
                    :options="recurringWorkItems"
                    />
                  </CCol>
                  <CCol>
                    <CSelect
                    v-if="selectedPatrol.enableScheduling && shifts.length>0"
                    label="Shift"
                    :value.sync="shiftId"
                    :options="shifts"
                    />
                  </CCol>
                </CRow>
                <CRow>
                  <CCol>
                    <CSelect
                    v-if="users.length>0"
                    label="Owner"
                    :value.sync="createdByUserId"
                    :options="users"
                    />
                  </CCol>
                  <CCol>
                    <CSelect
                    v-if="groups.length>0"
                    label="Owning Group"
                    :value.sync="adminGroupId"
                    :options="groups"
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
                        :items="workItems"
                        :fields="workItemFields"
                        sorter>
                        <template #scheduledAt="data">
                            <td>
                                {{new Date(data.item.scheduledAt).toLocaleTimeString()}}
                            </td>
                        </template>
                        <template #status="data">
                            <td>
                                <template v-if="data.item.completedAt">
                                    <CBadge color="success" shape="pill">Completed</CBadge>
                                    {{new Date(data.item.completedAt).toLocaleDateString()}} {{new Date(data.item.completedAt).toLocaleTimeString()}}
                                    by
                                    {{data.item.completedBy.lastName}}, {{data.item.completedBy.firstName}}
                                </template>
                                <template v-if="data.item.canceledAt">
                                    <CBadge color="danger" shape="pill">Canceled</CBadge>
                                    {{new Date(data.item.canceledAt).toLocaleDateString()}} {{new Date(data.item.canceledAt).toLocaleTimeString()}}
                                    by
                                    {{data.item.canceledBy.lastName}}, {{data.item.canceledBy.firstName}}
                                </template>
                                <template v-if="!data.item.completedAt && !data.item.canceledAt">
                                    <CBadge color="primary" shape="pill">Pending</CBadge>
                                </template>
                            </td>
                        </template>
                        <template #shift="data">
                            <td>
                                <template v-if="data.item.scheduledShift && data.item.scheduledShift.shift">
                                <strong>{{data.item.scheduledShift.shift.name}}</strong>
                                </template> 
                                  
                                <template v-if="data.item.scheduledShift && data.item.scheduledShift.group">
                                <em>{{data.item.scheduledShift.group.name}}</em>
                                </template>

                                <template v-if="!data.item.scheduledShift">Any</template>
                            </td>
                        </template>
                        <template #buttons="data">
                            <td>
                                <CButtonGroup>
                                  <CButton v-if="!data.item.completedAt && !data.item.canceledAt" color="info" size="sm" @click="edit(data.item)">Edit</CButton>
                                  <CButton v-if="data.item.completedAt || data.item.canceledAt" color="info" size="sm" @click="edit(data.item)">Details</CButton>
                                  <CButton v-if="data.item.isDue && !data.item.completedAt && !data.item.canceledAt && (data.item.canComplete || data.item.canAdmin)" color="success" size="sm" @click="doComplete(data.item)">Complete</CButton>
                                  <CButton v-if="!data.item.completedAt && !data.item.canceledAt && data.item.canAdmin" color="warning" size="sm" @click="cancel(data.item)">Cancel</CButton>
                                </CButtonGroup>
                            </td>
                        </template>
                        <template #assigned="data">
                            <td>
                                <template v-if="data.item.completionMode=='Any' && (!data.item.assignments || data.item.assignments.length==0)">
                                  Any
                                </template>
                                <template v-if="data.item.assignments && data.item.assignments.length>0">
                                  <ul>
                                    <li v-for="assigned in data.item.assignments" v-bind:key="assigned.id">
                                      {{assigned.user.lastName}}, {{assigned.user.firstName}}
                                    </li>
                                  </ul>
                                </template>
                            </td>
                        </template>
                    </CDataTable>
                  </CCol>
                </CRow>
            </CCardBody>
        </CCard>
        <work-item-details-modal :workItem="this.editWorkItem" :trigger="showEditWorkitem" @edited="refresh" @complete="showCompleteWorkitem++"/>
        <work-item-complete-modal :workItem="this.editWorkItem" :trigger="showCompleteWorkitem" @completed="refresh"/>
    </div>
</template>

<script>

import Datepicker from 'vuejs-datepicker';
import AuthenticatedPage from '../../mixins/AuthenticatedPage';
import WorkItemDetailsModal from '../schedule/WorkItemDetailsModal';
import WorkItemCompleteModal from '../schedule/WorkItemCompleteModal';

export default {
  name: 'WorkItems',
  components: { Datepicker, WorkItemDetailsModal,WorkItemCompleteModal
  },
  mixins: [AuthenticatedPage],
  data () {
    return {
      from: new Date(new Date().getTime() - (86400000 * 30)),
      to: new Date(),
      workItems: [],
      workItemFields:[],
      users: [],
      shifts: [],
      groups: [],
      recurringWorkItems: [],
      recurringWorkItemId:null,
      createdByUserId: null,
      completedByUserId: null,
      shiftId:null,
      adminGroupId:null,
      name:null,
      location:null,
      completeStatuses:[{label:'(All)',value:null},{label:'Yes',value:true},{label:'No',value:false}],
      complete:true,
      editWorkItem:{},
      showCompleteWorkitem:0,
      showEditWorkitem:0
    }
  },
  props: ['uid'],
  methods: {
    duration(seconds){
      var diffDate = new Date(seconds * 1000);
      return diffDate.getUTCHours()+":"+(diffDate.getUTCMinutes()+"").padStart(2,"0")+":"+(diffDate.getUTCSeconds()+"").padStart(2,"0");
    },
    edit(wi){
      this.editWorkItem = wi;
      this.showEditWorkitem++;
    },
    newWorkItem(){
      this.editWorkItem = {};
      this.showEditWorkitem++;
    },

    cancel(wi){
      this.$store.dispatch('loading','Canceling...');
      this.$http.post('workitem/cancel',{id:wi.id})
          .then(response => {
              console.log(response);
              this.refresh();
              this.show=false;
          }).catch(response => {
              console.log(response);
          }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    doComplete(wi){
      this.editWorkItem = wi;
      this.showCompleteWorkitem++;
    },
    getWorkItems() {
      if(this.selectedPatrol.enableScheduling){
      this.workItemFields=[
          {key:'buttons',label:''},
          {key:'name',label:'Work Item'},
          {key:'location',label:'Location'},
          {key:'scheduledAt',label:'Scheduled'},
          {key:'shift',label:'Shift'},
          {key:'status',label:'Status'}
          
        ];
      }
      else {
        this.workItemFields=[
        {key:'buttons',label:''},
        {key:'name',label:'Work Item'},
        {key:'location',label:'Location'},
        {key:'scheduledAt',label:'Scheduled'},
        {key:'status',label:'Status'}
          
        ];
      }

      this.$store.dispatch('loading','Loading...');
        this.$http.post('workitem/search',{patrolId:this.selectedPatrolId,completedByUserId:this.completedByUserId
        ,scheduledAfter:this.from,scheduledBefore:this.to,complete:this.complete,recurringWorkItemId:this.recurringWorkItemId
        ,shiftId:this.shiftId,adminGroupId:this.adminGroupId,name:this.name,location:this.location})
            .then(response => {
                console.log(response);
                this.workItems = response.data;
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    getUsers() {
      this.$store.dispatch('loading','Loading...');
      this.$http.get('user/list/'+this.selectedPatrolId)
          .then(response => {
              console.log(response);
              this.users = _.map(response.data,function(u){
                  return {label:u.lastName+', '+u.firstName,value:u.id};
              });

              this.users.splice(0,0,{label:'(Any)',value:null});
              this.createdByUserId = this.filteredUserList[0].value;
              this.completedByUserId = this.filteredUserList[0].value;
          }).catch(response => {
              console.log(response);
          }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    getShifts() {
      this.$store.dispatch('loading','Loading...');
      this.$http.get('schedule/shifts?patrolid='+this.selectedPatrolId)
          .then(response => {
              console.log(response);
              this.shifts = _.map(response.data,function(u){
                  return {label:u.name,value:u.id};
              });

              this.shifts.splice(0,0,{label:'(Any)',value:null});
              this.shiftId = this.filteredUserList[0].value;
          }).catch(response => {
              console.log(response);
          }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    getGroups() {
      this.$store.dispatch('loading','Loading...');
      this.$http.get('user/groups/'+this.selectedPatrolId)
          .then(response => {
              console.log(response);
              this.groups = _.map(response.data,function(u){
                  return {label:u.name,value:u.id};
              });

              this.groups.splice(0,0,{label:'(Any)',value:null});
              this.adminGroupId = this.filteredUserList[0].value;
          }).catch(response => {
              console.log(response);
          }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    getRecurringWorkItems() {
      this.$store.dispatch('loading','Loading...');
      this.$http.get('workitem/recurring/patrol/'+this.selectedPatrolId+'/true')
          .then(response => {
              console.log(response);
              this.recurringWorkItems = _.map(response.data,function(u){
                  return {label:u.name+' '+u.location,value:u.id};
              });

              this.recurringWorkItems.splice(0,0,{label:'(Any)',value:null});
              this.recurringWorkItemId = this.filteredUserList[0].value;
          }).catch(response => {
              console.log(response);
          }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    refresh(){
      this.getWorkItems();
    }
  },
  computed: {
    
  },
  watch: {
    selectedPatrolId(){
      this.getRecurringWorkItems();
      this.getGroups();
      this.getUsers();
      this.getShifts();
      this.refresh();
    },
    from(){
      this.refresh();
    },
    to(){
      this.refresh();
    },
    adminGroupId(){
        this.refresh();
    },
    completedByUserId(){
        this.refresh();
    },
    name(){
        this.refresh();
    },
    location(){
        this.refresh();
    },
    shiftId(){
        this.refresh();
    },
    recurringWorkItemId(){
        this.refresh();
    },
    complete(){
        this.refresh();
    }
  },
  mounted: function(){
      this.getRecurringWorkItems();
      this.getGroups();
      this.getUsers();
      this.getShifts();
      this.refresh();
  }
}
</script>

<template>
    <div v-if="workItems && workItems.length>0">
        <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-task"/>Incomplete Work
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
                        :items="workItems"
                        :fields="workItemFields"
                        sorter>
                        <template #scheduledAt="data">
                            <td>
                                {{new Date(data.item.scheduledAt).toLocaleTimeString()}}
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
                                  <CButton v-if="data.item.canComplete || data.item.canAdmin" color="success" size="sm" @click="complete(data.item)">Complete</CButton>
                                  <CButton color="info" size="sm" @click="edit(data.item)">Details</CButton>
                                  <CButton v-if="data.item.canAdmin" color="warning" size="sm" @click="cancel(data.item)">Cancel</CButton>
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

import AuthenticatedPage from '../../mixins/AuthenticatedPage';
import WorkItemDetailsModal from './WorkItemDetailsModal';
import WorkItemCompleteModal from './WorkItemCompleteModal';

export default {
  name: 'WorkCurrent',
  mixins: [AuthenticatedPage],
  components: { 
    WorkItemDetailsModal,
    WorkItemCompleteModal
  },
  data () {
    return {
      workItems: [],
      workItemFields:[],
      timer: '',
      now:new Date(),
      lastRefresh: new Date(),
      editWorkItem:{},
      showEditWorkitem:0,
      showCompleteWorkitem:0
    }
  },
  methods: {
    edit(wi){
      this.editWorkItem = wi;
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
    complete(wi){
      this.editWorkItem = wi;
      this.showCompleteWorkitem++;
    },
    duration(from,to){
      var diffMillis = new Date(to) - new Date(from);
      var diffDate = new Date(diffMillis);
      return diffDate.getUTCHours()+":"+(diffDate.getUTCMinutes()+"").padStart(2,"0")+":"+(diffDate.getUTCSeconds()+"").padStart(2,"0");
    },
    getWorkItems() {
      if(this.selectedPatrol.enableScheduling){
      this.workItemFields=[
          {key:'buttons',label:''},
          {key:'name',label:'Work Item'},
          {key:'location',label:'Location'},
          {key:'scheduledAt',label:'Begin'},
          {key:'shift',label:'Shift'},
          //{key:'assigned',label:'Assigned'}
        ];
      }
      else {
        this.workItemFields=[
          {key:'buttons',label:''},
          {key:'name',label:'Work Item'},
          {key:'location',label:'Location'},
          {key:'scheduledAt',label:'Begin'},
          //{key:'assigned',label:'Assigned'}
        ];
      }
      //this.$store.dispatch('loading','Loading...');
        this.$http.get('workitem/current/'+this.selectedPatrolId)
            .then(response => {
                console.log(response);
                this.workItems = response.data;
            }).catch(response => {
                console.log(response);
            });//.finally(response=>this.$store.dispatch('loadingComplete'));
    },
    refresh(){
      this.getWorkItems();
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

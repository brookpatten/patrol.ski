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
                            </td>
                        </template>
                        <template #buttons="data">
                            <td>
                                <CButtonGroup>
                                  <CButton v-if="data.item.completable" color="success" size="sm">Complete</CButton>
                                  <CButton color="info" size="sm">Details</CButton>
                                  <CButton v-if="data.item.cancelable" color="warning" size="sm">Cancel</CButton>
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
    </div>
</template>

<script>

import AuthenticatedPage from '../../mixins/AuthenticatedPage';

export default {
  name: 'WorkCurrent',
  mixins: [AuthenticatedPage],
  components: {
  },
  data () {
    return {
      workItems: [],
      workItemFields:[],
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
    getWorkItems() {
      if(this.selectedPatrol.enableScheduling){
      this.workItemFields=[
          {key:'name',label:'Work Item'},
          {key:'location',label:'Location'},
          {key:'scheduledAt',label:'Begin'},
          {key:'shift',label:'Shift'},
          {key:'assigned',label:'Assigned'},
          {key:'buttons',label:''}
        ];
      }
      else {
        this.workItemFields=[
          {key:'name',label:'Work Item'},
          {key:'location',label:'Location'},
          {key:'scheduledAt',label:'Begin'},
          {key:'assigned',label:'Assigned'},
          {key:'buttons',label:''}
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

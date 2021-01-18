<template>
    <div>
        <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-task"/>Recurring Work Items
                <CButton size="sm" color="success" @click="newWorkItem" class="float-right">New</CButton>
            </slot>
            </CCardHeader>
            <CCardBody>
                <CRow>
                  <CCol>
                    <CSelect
                    v-if="recurringWorkItemStatuses.length>0"
                    label="Status"
                    :value.sync="recurringWorkItemStatus"
                    :options="recurringWorkItemStatuses"
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
                        <template #buttons="data">
                            <td>
                                <CButtonGroup>
                                  <CButton color="info" size="sm" @click="edit(data.item)">Edit</CButton>
                                  <CButton v-if="data.item.completedWorkItemCount>0 && data.item.workItemCount > data.item.completedWorkItemCount" color="danger" size="sm" @click="end(data.item.id)">End</CButton>
                                  <CButton v-if="data.item.completedWorkItemCount==0 && data.item.workItemCount > data.item.completedWorkItemCount" color="danger" size="sm" @click="end(data.item.id)">Remove</CButton>
                                </CButtonGroup>
                            </td>
                        </template>
                        <template #workItems="data">
                            <td>
                                <CBadge :color="data.item.completedWorkItemCount == data.item.workItemCount ? 'secondary' : 'primary'">
                                {{data.item.completedWorkItemCount}}/{{data.item.workItemCount}}
                                </CBadge>
                            </td>
                        </template>
                        <template #occurrences="data">
                            <td>
                                <template v-if="data.item.firstScheduledAt">
                                  <CRow>
                                    <CCol><strong>First:</strong></CCol>
                                    <CCol>{{new Date(data.item.firstScheduledAt).toLocaleDateString()}}</CCol>
                                  </CRow>
                                </template>
                                <template v-if="data.item.nextScheduledAt">
                                  <CRow>
                                    <CCol><strong>Next:</strong></CCol>
                                    <CCol>{{new Date(data.item.nextScheduledAt).toLocaleDateString()}}</CCol>
                                  </CRow>
                                </template>
                                <template v-if="data.item.lastScheduledAt">
                                  <CRow>
                                    <CCol><strong>Last:</strong></CCol>
                                    <CCol>{{new Date(data.item.lastScheduledAt).toLocaleDateString()}}</CCol>
                                  </CRow>
                                </template>
                            </td>
                        </template>
                        <!-- <template #workItemProgress="data">
                            <td>
                                <CProgress
                                :value="(Math.round((data.item.completedWorkItemCount / data.item.workItemCount) * 100))"
                                color="success"
                                striped
                                show-percentage
                                />
                            </td>
                        </template> -->
                        <template #recurrence="data">
                            <td>
                                <template v-if="data.item.shifts && data.item.shifts.length>0">
                                  Shift(s):
                                  <ul>
                                    <li v-for="shift in data.item.shifts" v-bind:key="data.item.id+'-'+shift.shift.id">
                                      {{shift.shift.name}}
                                    </li>
                                  </ul>
                                </template>
                                <template v-if="data.item.recurStart">
                                  <label>Begin:</label> {{new Date(data.item.recurStart).toLocaleDateString()}} {{new Date(data.item.recurStart).toLocaleTimeString()}}<br/>
                                  <label>Repeat Every:</label> {{data.item.recurIntervalCount}} {{data.item.recurInterval}}(s)<br/>
                                  <label>End:</label> {{new Date(data.item.recurEnd).toLocaleDateString()}} {{new Date(data.item.recurEnd).toLocaleTimeString()}}
                                </template>
                            </td>
                        </template>
                    </CDataTable>
                  </CCol>
                </CRow>
            </CCardBody>
        </CCard>
        <recurring-work-item-details-modal :workItem="editWorkItem" :trigger="showEditWorkitem" @edited="refresh"/>
    </div>
</template>

<script>

import AuthenticatedPage from '../../mixins/AuthenticatedPage';
import RecurringWorkItemDetailsModal from './RecurringWorkItemDetailsModal.vue';

export default {
  name: 'RecurringWorkItems',
  components: { RecurringWorkItemDetailsModal
  },
  mixins: [AuthenticatedPage],
  data () {
   return {
      workItems: [],
      workItemFields:[],
      editWorkItem:{},
      showEditWorkitem:0,
      recurringWorkItemStatuses:[{label:'Current',value:false},{label:'All',value:true}],
      recurringWorkItemStatus:false
    }
  },
  props: ['uid'],
  methods: {
    newWorkItem(){
      this.editWorkItem = {};
      this.showEditWorkitem++;
    },
    duration(seconds){
      var diffDate = new Date(seconds * 1000);
      return diffDate.getUTCHours()+":"+(diffDate.getUTCMinutes()+"").padStart(2,"0")+":"+(diffDate.getUTCSeconds()+"").padStart(2,"0");
    },
    edit(wi){
      this.editWorkItem = wi;
      this.showEditWorkitem++;
    },
    end(id) {
      this.$store.dispatch('loading','Ending...');
      this.$http.post('workitem/recurring/end/'+id)
        .then(response => {
            this.getRecurringWorkItems();
        }).catch(response => {
            console.log(response);
        }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    getRecurringWorkItems() {
      if(this.selectedPatrol.enableScheduling){
      this.workItemFields=[
        {key:'buttons',label:''},
          {key:'name',label:'Recurring Work Item'},
          {key:'location',label:'Location'},
          {key:'recurrence',label:'Recurrence'},
          //{key:'workItemProgress',label:'Work Items'},
          {key:'workItems',label:'Completed/Total'},
          {key:'occurrences',label:'Occurrences'},
        ];
      }
      else {
        this.workItemFields=[
          {key:'buttons',label:''},
          {key:'name',label:'Recurring Work Item'},
          {key:'location',label:'Location'},
          {key:'recurrence',label:'Recurrence'},
          //{key:'workItemProgress',label:'Work Items'},
          {key:'workItems',label:'Completed/Total'},
          {key:'occurrences',label:'Occurrences'},
        ];
      }

      this.$store.dispatch('loading','Loading...');
      this.$http.get('workitem/recurring/patrol/'+this.selectedPatrolId+'/'+this.recurringWorkItemStatus)
          .then(response => {
              console.log(response);
              this.workItems = response.data;
          }).catch(response => {
              console.log(response);
          }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    refresh(){
      this.getRecurringWorkItems();
    }
  },
  computed: {
    
  },
  watch: {
    selectedPatrolId(){
      this.getRecurringWorkItems();
    },
    recurringWorkItemStatus(){
      this.getRecurringWorkItems();
    }
  },
  mounted: function(){
      this.getRecurringWorkItems();
  }
}
</script>

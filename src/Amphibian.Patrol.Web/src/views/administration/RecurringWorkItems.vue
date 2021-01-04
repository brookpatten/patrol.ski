<template>
    <div>
        <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-task"/>Recurring Work Items
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
                        <template #buttons="data">
                            <td>
                                <CButtonGroup>
                                  <CButton color="info" size="sm" @click="edit(data.item)">Edit</CButton>
                                </CButtonGroup>
                            </td>
                        </template>
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
                                  <label>Begin:</label> {{new Date(data.item.recurStart).toLocaleDateString()}} {{new Date(data.item.recurStart).toLocaleTimeString()}}
                                  <label>Repeat Every:</label> {{duration(data.item.recurIntervalSeconds)}}
                                  <label>End:</label> {{new Date(data.item.recurEnd).toLocaleDateString()}} {{new Date(data.item.recurEnd).toLocaleTimeString()}}
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
  name: 'RecurringWorkItems',
  components: { 
  },
  mixins: [AuthenticatedPage],
  data () {
    return {
      workItems: [],
      workItemFields:[],
      editWorkItem:{},
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
    getRecurringWorkItems() {
      if(this.selectedPatrol.enableScheduling){
      this.workItemFields=[
        {key:'buttons',label:''},
          {key:'name',label:'Work Item'},
          {key:'location',label:'Location'},
          {key:'recurrence',label:'Recurrence'},
        ];
      }
      else {
        this.workItemFields=[
          {key:'buttons',label:''},
          {key:'name',label:'Work Item'},
          {key:'location',label:'Location'},
          {key:'recurrence',label:'Recurrence'}
        ];
      }

      this.$store.dispatch('loading','Loading...');
      this.$http.get('workitem/recurring/patrol/'+this.selectedPatrolId)
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
    }
  },
  mounted: function(){
      this.getRecurringWorkItems();
  }
}
</script>

<template>
    <div>
      <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-calendar"/>
            </slot>
            </CCardHeader>
            <CCardBody>
              <CRow>
                <CCol>
                  <label for="from">From Date</label>
                  <datepicker v-model="from" input-class="form-control" calendar-class="card"></datepicker><br/>
                </CCol>
                <CCol>
                  <label for="to">To Date</label>
                  <datepicker v-model="to" input-class="form-control" calendar-class="card"></datepicker><br/>
                </CCol>
              </CRow>
              <CRow>
                <CCol>
                  <CDataTable
                      striped
                      bordered
                      small
                      fixed
                      :items="events"
                      :fields="eventFields"
                      sorter>
                      <template #buttons="data">
                        <td>
                          <CButtonGroup size="sm">
                            <CButton color="primary" :to="{ name: 'EditEvent', params: { eventId: data.item.id } }">Edit</CButton>
                            <CButton color="danger" v-on:click="deleteEvent(data.item.id)">Remove</CButton>
                          </CButtonGroup>
                        </td>
                      </template>
                      <template #buttons-header>
                        <CButton color="primary" size="sm" :to="{name:'EditEvent',params:{eventId:null}}">New</CButton>
                      </template>
                      <template #startsAt="data">
                          <td>{{(data.item.startsAt ? (new Date(data.item.startsAt)).toLocaleString() : '')}}</td>
                      </template>
                      <template #endsAt="data">
                          <td>{{(data.item.endsAt ? (new Date(data.item.endsAt)).toLocaleString() : '')}}</td>
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
  name: 'Events',
  components: { Datepicker
  },
  data () {
    return {
      from: new Date(),
      to: new Date(new Date().getTime() + (86400000 * 90)),
      events: [],
      eventFields:[
          {key:'name',label:'Event'},
          {key:'location',label:'Location'},
          {key:'startsAt',label:'Start'},
          {key:'endsAt',label:'End'},
          {key:'buttons',label:'',sorter:false,filter:false}
      ]
    }
  },
  methods: {
    getEvents() {
      this.$store.dispatch('loading','Loading...');
        this.$http.post('events/search',{patrolId:this.selectedPatrolId,from:this.from,to:this.to})
            .then(response => {
                console.log(response);
                this.events = response.data;
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
    deleteEvent(id){
      this.$store.dispatch('loading','Deleting...');
      this.$http.delete('event/'+id)
          .then(response => {
              console.log(response);
              getEvents();
          }).catch(response => {
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
    }
  },
  watch: {
    selectedPatrolId(){
      this.getEvents();
    },
    from(){
      this.getEvents();
    },
    to(){
      this.getEvents();
    }
  },
  mounted: function(){
      this.getEvents();
  }
}
</script>

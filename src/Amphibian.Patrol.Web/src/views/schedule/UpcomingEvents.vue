<template>
    <CCard v-if="upcomingEvents.length>0">
        <CCardHeader>
        <slot name="header">
            <CIcon name="cil-calendar"/> Upcoming Events
            <CButton size="sm" color="info" class="float-right" :to="{name:'Calendar'}">Calendar</CButton>
        </slot>
        </CCardHeader>
        <CCardBody>
            <CDataTable
                striped
                bordered
                small
                fixed
                :items="upcomingEvents"
                :fields="upcomingEventsFields"
                sorter>
                <template #startsAt="data">
                    <td>{{(data.item.startsAt ? (new Date(data.item.startsAt)).toLocaleString() : '')}}</td>
                </template>
                <template #endsAt="data">
                    <td>{{(data.item.endsAt ? (new Date(data.item.endsAt)).toLocaleString() : '')}}</td>
                </template>
            </CDataTable>
        </CCardBody>
    </CCard>
</template>

<script>

export default {
  name: 'Home',
  components: { },
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
    }
  },
  props: [],
  data () {
    return {
        upcomingEvents:[],
        upcomingEventsFields:[{key:'name',label:'Event'},
          {key:'location',label:'Location'},
          {key:'startsAt',label:'Date/Time'}]
    }
  },
  methods: {
        getUpcomingEvents() {
            this.$store.dispatch('loading','Loading...');
            this.$http.get('events/upcoming/'+this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    this.upcomingEvents = response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        hasPermission: function(permission){
          return this.selectedPatrol!=null && this.selectedPatrol.permissions!=null && _.indexOf(this.selectedPatrol.permissions,permission) >= 0;
        },
        refresh:function(){
            console.log(JSON.stringify(this.selectedPatrol));
            this.getUpcomingEvents();
        }
  },
  mounted: function(){
      this.refresh();
  }
}
</script>

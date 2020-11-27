<template>
    <div>
        <CCard>
            <CCardHeader>
                <slot name="header">
                    <CIcon name="cil-calendar"/>
                </slot>
                Patrol Calendar
            </CCardHeader>
            <CCardBody>
                <CRow>
                    <CCol>
                        <calendar-view
                        :show-date="selectedDate"
                        class="theme-default holiday-us-traditional holiday-us-official"
                        :events="calendarEvents"
                        :period-changed-callback="periodChanged"
                        :disable-past="false"
                        :disable-future="false"
                        :displayPeriodUom="displayPeriod"
                        :enable-drag-drop="false"
                        :enable-date-selection="false"
                        @click-event="clickEvent">
                        <calendar-view-header
                            slot="header"
                            slot-scope="t"
                            :header-props="t.headerProps"
                            @input="setShowDate" />
                        </calendar-view>
                    </CCol>
                </CRow>
                <CRow>
                    <CCol md="3" lg="3" sm="12">
                        <CSelect
                        :options="['year', 'month', 'week']"
                        :value.sync="displayPeriod"
                        label="View"
                        add-wrapper-classes="ml-2"
                        />
                    </CCol>
                </CRow>
            </CCardBody>
            <CCardFooter>

            </CCardFooter>
        </CCard>
        <CModal
        :title="selectedEvent.title"
        :show.sync="showSelectedEvent"
        :no-close-on-backdrop="true"
        color="primary"
        >
        {{(new Date(selectedEvent.startsAt)).toLocaleString()}} - {{(new Date(selectedEvent.endsAt)).toLocaleString()}}<br/>
        {{selectedEvent.name}}<br/>
        Location: {{selectedEvent.location}}<br/>
        <template #header>
        </template>
        <template #footer>
            <CButton @click="showSelectedEvent = false" color="primary">Close</CButton>
        </template>
        </CModal>
    </div>
</template>

<script>
import { freeSet } from '@coreui/icons'

import { CalendarView, CalendarViewHeader } from "vue-simple-calendar"
// The next two lines are processed by webpack. If you're using the component without webpack compilation,
// you should just create <link> elements for these. Both are optional, you can create your own theme if you prefer.
//require("vue-simple-calendar/static/css/default.css")
//require("vue-simple-calendar/static/css/holidays-us.css")

export default {
  name: 'Calendar',
  freeSet,
  components: {  CalendarView,
			CalendarViewHeader
  },
  props: [],
  data () {
    return {
        selectedDate:new Date(),
        from:new Date(),
        to:new Date(),
        events:[],
        displayPeriod:'month',
        showSelectedEvent:false,
        selectedEvent:{}
    }
  },
  methods: {
        periodChanged(range){
            this.from=range.displayFirstDate;
            this.to=range.displayLastDate;
            this.getEvents();
        },
        clickEvent(event){
            console.log(JSON.stringify(event));
            if(event.originalEvent.type=='event'){
                var e = _.find(this.events,{id:event.id});
                this.selectedEvent = e;
                this.showSelectedEvent=true;
            }
            //else: shifts etc
        },
        setShowDate(d) {
            this.selectedDate = d;
        },
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
        },
        hasPermission: function(permission){
          return this.selectedPatrol!=null && this.selectedPatrol.permissions!=null && _.indexOf(this.selectedPatrol.permissions,permission) >= 0;
        }
  },
  mounted: function(){
      this.getEvents();
  },
  watch: {
      selectedPatrolId: function(){
          this.getEvents();
      }
  },
  computed: {
    selectedPatrolId: function () {
      return this.$store.state.selectedPatrolId;
    },
    selectedPatrol: function (){
        return this.$store.getters.selectedPatrol;
    },
    calendarEvents: function(){
        var cEvents = _.map(this.events,function(e){
            return {
                id: e.id,
                title: e.name,
                startDate: e.startsAt,
                endDate: e.endsAt,
                classes:'badge-primary',
                type:'event'
            };
        });
        return cEvents;
    }
  }
}
</script>

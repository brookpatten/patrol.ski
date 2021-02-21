<template>
  <div>
    <CRow class="justify-content-center">
      <CCol md="8" lg="8">
        <CCard>
          <CCardBody>
            <CRow>
              <CCol md="4">
                <img v-if="patrol.logoImageUrl" :src="patrol.logoImageUrl" alt="patrol logo" class="img-fluid rounded"/>
              </CCol>
              <CCol md="8">
                <h1 class="display-4">{{patrol.name}}</h1>
                <h3 v-if="patrol.phone">Phone: <a :href="'tel:'+patrol.phone">{{patrol.phone}}</a></h3>
                <h3 v-if="patrol.email">Email: <a :href="'mailto:'+patrol.email">{{patrol.email}}</a></h3>
              </CCol>
            </CRow>
          </CCardBody>
          <CCardFooter>
            <CButtonGroup class="float-right">
                  <CButton
                    color="success"
                    :to="{name:'Register'}"
                  > Register
                  </CButton>
                  <CButton color="primary" :to="{name:'Login'}">Sign in</CButton>
                </CButtonGroup>
          </CCardFooter>
        </CCard>
      </CCol>
    </CRow>

    <CRow class="justify-content-center">
      <CCol md="4" lg="4" v-if="events && events.length>0">
        <CRow>
          <CCol>
            <CCard>
              <CCardBody><h2>Upcoming Events</h2></CCardBody>
            </CCard>
          </CCol>
        </CRow>
        <CRow v-for="event in events" :key="'e'-event.id">
          <CCol>
            <CCard>
              <CCardHeader>{{event.name}}</CCardHeader>
              <CCardBody>
                When: {{(new Date(event.startsAt)).toLocaleString()}} - {{(new Date(event.endsAt)).toLocaleString()}}<br/>
                Where: {{event.location}}<br/>
                <div style="overflow:hidden;" v-html="event.eventHtml"/>
              </CCardBody>
            </CCard>
          </CCol>
        </CRow>
      </CCol>
      <CCol md="8" lg="8" v-if="announcements && announcements.length>0">
        <CRow v-for="announcement in announcements" :key="'a'-announcement.id">
          <CCard>
            <CCardHeader>{{announcement.subject}}</CCardHeader>
            <CCardBody>
              <div style="overflow:hidden;" v-html="announcement.announcementHtml"/>
            </CCardBody>
          </CCard>
        </CRow>
      </CCol>
    </CRow>    
  </div>
</template>

<script>

import Subdomain from '../../mixins/Subdomain';

export default {
  name: 'Subdomain',
  components: {  },
  mixins: [Subdomain],
  //props: ['subdomain'],
  data () {
    return {
      patrol: {},
      announcements: [],
      events:[]
    }
  },
  methods:{
    getPatrol() {
        this.$store.dispatch('loading','Loading...');
        this.$http.get('public/patrol/'+this.subdomain)
            .then(response => {
                console.log(response);
                this.patrol = response.data;

                if(this.patrol.enableEvents){
                  this.getEvents();
                }
                if(this.patrol.enableAnnouncements){
                  this.getAnnouncements();
                }
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    getEvents() {
        this.$store.dispatch('loading','Loading...');
        this.$http.get('public/events/'+this.subdomain)
            .then(response => {
                console.log(response);
                this.events = response.data;
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    getAnnouncements() {
        this.$store.dispatch('loading','Loading...');
        this.$http.get('public/announcements/'+this.subdomain)
            .then(response => {
                console.log(response);
                this.announcements = response.data;
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
    }
  },
  mounted: function(){
    this.getPatrol();
  }
}
</script>

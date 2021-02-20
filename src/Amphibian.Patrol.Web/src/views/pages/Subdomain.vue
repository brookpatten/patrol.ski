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
              </CCol>
            </CRow>
          </CCardBody>
        </CCard>
      </CCol>
    </CRow>
    
    <CRow v-for="announcement in announcements" :key="'a'-announcement.id">
      <CCard>
        <CCardHeader>{{announcement.subject}}</CCardHeader>
        <CCardBody>
          <div style="overflow:hidden;" v-html="announcement.announcementHtml"/>
        </CCardBody>
      </CCard>
    </CRow>

    <CRow v-for="event in events" :key="'e'-event.id">
      <CCard>
        <CCardHeader>Upcoming Event: {{event.name}}</CCardHeader>
        <CCardBody>
          <div style="overflow:hidden;" v-html="event.eventHtml"/>
        </CCardBody>
      </CCard>
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

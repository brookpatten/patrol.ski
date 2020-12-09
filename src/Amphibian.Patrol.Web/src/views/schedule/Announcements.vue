<template>
    <div>
        <CRow v-for="announcement in announcements" :key="'announcement-'+announcement.id">
            <CCol>
                <CCard accent-color="info">
                    <CCardHeader>
                        <strong>{{(new Date(announcement.createdAt)).toLocaleDateString()}}</strong>
                        {{announcement.subject}} 
                        <CButtonGroup class="float-right" v-if="hasPermission('MaintainAnnouncements')">
                            <CButton size="sm" color="info" :to="{name:'EditAnnouncement',params:{announcementId:announcement.id}}">Edit</CButton>
                            <CButton size="sm" color="warning" v-on:click="expireAnnouncement(announcement)">Remove</CButton>
                        </CButtonGroup>
                    </CCardHeader>
                    <CCardBody v-html="announcement.announcementHtml"></CCardBody>
                </CCard>
            </CCol>
        </CRow>
    </div>
    
</template>

<script>

export default {
  name: 'Announcements',
  components: { 
  },
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
        announcements:[],
    }
  },
  methods: {
        getAnnouncements() {
            this.$store.dispatch('loading','Loading...');
            this.$http.get('announcements/current/'+this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    this.announcements = response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        hasPermission: function(permission){
          return this.selectedPatrol!=null && this.selectedPatrol.permissions!=null && _.indexOf(this.selectedPatrol.permissions,permission) >= 0;
        },
        expireAnnouncement(announcement){
            this.$store.dispatch('loading','Loading...');
            this.$http.post('announcements/expire/'+announcement.id)
                .then(response => {
                    console.log(response);
                    this.announcements = _.filter(this.announcements,function(a){return a.id!=announcement.id;});
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        refresh:function(){
            this.getAnnouncements();
        }
  },
  mounted: function(){
      this.refresh();
  }
}
</script>

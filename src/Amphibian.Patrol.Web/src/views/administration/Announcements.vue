<template>
    <div>
      <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-people"/>
            </slot>
            </CCardHeader>
            <CCardBody>
            <CDataTable
                striped
                bordered
                small
                fixed
                :items="announcements"
                :fields="announcementFields"
                sorter>
                <template #buttons="data">
                  <td>
                    <CButtonGroup size="sm">
                      <CButton color="primary" :to="{ name: 'EditAnnouncement', params: { announcementId: data.item.id } }">Edit</CButton>
                      <CButton color="danger" v-on:click="expireAnnouncement(data.item)">Remove</CButton>
                    </CButtonGroup>
                  </td>
                </template>
                <template #buttons-header>
                  <CButton color="primary" size="sm" :to="{name:'EditAnnouncement',params:{announcementId:null}}">New</CButton>
                </template>
                <template #createdAt="data">
                    <td>{{(new Date(data.item.createdAt)).toLocaleDateString()}}</td>
                </template>
                <template #postAt="data">
                    <td>{{(data.item.postAt ? (new Date(data.item.postAt)).toLocaleDateString() : '')}}</td>
                </template>
                <template #expireAt="data">
                    <td>{{(data.item.expireAt ? (new Date(data.item.expireAt)).toLocaleDateString() : '')}}</td>
                </template>
            </CDataTable>
            </CCardBody>
        </CCard>
    </div>
</template>

<script>

export default {
  name: 'Announcements',
  components: {
  },
  data () {
    return {
      announcements: [],
      announcementFields:[
          {key:'subject',label:'Subject'},
          {key:'createdAt',label:'Created'},
          {key:'postAt',label:'Post(ed)'},
          {key:'expireAt',label:'Expire(d)'},
          {key:'buttons',label:'',sorter:false,filter:false}
      ]
    }
  },
  methods: {
    getAnnouncements() {
      this.$store.dispatch('loading','Loading...');
        this.$http.get('announcements/'+this.selectedPatrolId)
            .then(response => {
                console.log(response);
                this.announcements = response.data;
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
    expireAnnouncement(announcement){
      this.$store.dispatch('loading','Loading...');
      this.$http.post('announcements/expire/'+announcement.id)
          .then(response => {
              console.log(response);
              var index = _.indexOf(this.announcements,function(a){return a.id!=announcement.id;});
              this.announcements[index] = response.data;
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
      this.getAnnouncements();
    }
  },
  mounted: function(){
      this.getAnnouncements();
  }
}
</script>

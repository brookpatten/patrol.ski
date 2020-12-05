<template>
    <div>
      <CForm @submit.prevent="save">
      <CCard>
        <CCardHeader>
          <slot name="header">
              <CIcon name="cil-comment-square"/>
          </slot>
        </CCardHeader>
        <CCardBody>
            <CAlert color="danger" v-if="validationMessage">{{validationMessage}}</CAlert>
            <CInput
            label="Subject"
            v-model="announcement.subject"
            :invalidFeedback="validationErrors.Subject ? validationErrors.Subject.join() : 'Invalid'"
            :isValid="validated ? validationErrors.Subject==null : null"
            />
            
            <label for="announcement.postAt">Begin Showing</label>
            <datepicker v-model="announcement.postAt" input-class="form-control" calendar-class="card"></datepicker>
            <label for="announcement.expireAt">Expire</label>
            <datepicker v-model="announcement.expireAt" input-class="form-control" calendar-class="card"></datepicker><br/>

            <CSwitch class="mx-1" color="primary" variant="3d" shape="3d" :checked.sync="announcement.emailed"/>
            <label for="announcement.emailed">Email to entire patrol</label>
            <br/>

            <quill-editor v-model="announcement.announcementMarkdown"></quill-editor>
        </CCardBody>
        <CCardFooter>
            <CButtonGroup>
              <CButton color="secondary" :to="{ name: 'Announcements'}">Back</CButton>
              <CButton type="submit" color="primary">Save</CButton>
            </CButtonGroup>
        </CCardFooter>
      </CCard>
      </CForm>
    </div>
</template>

<script>

import { quillEditor } from 'vue-quill-editor'
import 'quill/dist/quill.core.css'
import 'quill/dist/quill.snow.css'
import 'quill/dist/quill.bubble.css'
import Datepicker from 'vuejs-datepicker';

export default {
  name: 'EditAnnouncement',
  components: { quillEditor, Datepicker
  },
  props: ['announcementId'],
  data () {
    return {
      announcement:{},
      validationMessage:'',
      validationErrors:{},
      validated:false
    }
  },
  methods: {
    hasPermission: function(permission){
      return this.selectedPatrol!=null && this.selectedPatrol.permissions!=null && _.indexOf(this.selectedPatrol.permissions,permission) >= 0;
    },
    getAnnouncement() {
        if(this.announcementId==0 || !this.announcementId){
          this.announcement={
            subject:'',
            announcementMarkdown:'',
            announcementHtml:'',
            patrolId: this.selectedPatrolId,
            emailed:false
          };
        }
        else{
          this.$store.dispatch('loading','Loading...');
          this.$http.get('announcement/'+this.announcementId)
            .then(response => {
                this.announcement = response.data;

                
                
                console.log(response);
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
        }
    },
    save(){
        this.$store.dispatch('loading','Saving...');

        
        this.$http.post('announcements',this.announcement)
          .then(response=>{
            this.$router.push({name:'Announcements'});
          }).catch(response=>{
            this.validated=true;
            this.validationMessage = response.response.data.title;
            this.validationErrors = response.response.data.errors;
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
        this.getAnnouncement();
    }
  },
  mounted: function(){
    this.getAnnouncement();
  }
}
</script>

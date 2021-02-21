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

            <CSwitch class="mx-1" color="primary" variant="3d" :checked.sync="announcement.isInternal" v-if="selectedPatrol.enablePublicSite"/>
            <label for="announcement.isInternal" v-if="selectedPatrol.enablePublicSite">Show for logged in Patrollers</label><br/>

            <CSwitch class="mx-1" color="primary" variant="3d" :checked.sync="announcement.isPublic" v-if="selectedPatrol.enablePublicSite"/>
            <label for="announcement.isPublic" v-if="selectedPatrol.enablePublicSite">Show on Public Site</label>
            <br/>
            <CSwitch class="mx-1" color="primary" variant="3d" shape="3d" :checked.sync="announcement.emailed"/>
            <label for="announcement.emailed">Email to entire patrol</label>
            <br/>

            <quill-editor v-model="announcement.announcementMarkdown" :options="quillOptions"></quill-editor>
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

import Quill from 'quill'
import { quillEditor } from 'vue-quill-editor'
import ImageUploader from 'quill-image-uploader'
import 'quill/dist/quill.core.css'
import 'quill/dist/quill.snow.css'
import 'quill/dist/quill.bubble.css'
import Datepicker from 'vuejs-datepicker';

Quill.register("modules/imageUploader", ImageUploader);

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
      validated:false,
      quillOptions:{
        modules: {
          toolbar: [
            ['bold', 'italic', 'underline', 'strike'],
            ['blockquote', 'code-block'],
            [{ 'header': 1 }, { 'header': 2 }],
            [{ 'list': 'ordered' }, { 'list': 'bullet' }],
            [{ 'script': 'sub' }, { 'script': 'super' }],
            [{ 'indent': '-1' }, { 'indent': '+1' }],
            [{ 'direction': 'rtl' }],
            [{ 'size': ['small', false, 'large', 'huge'] }],
            [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
            [{ 'color': [] }, { 'background': [] }],
            [{ 'font': [] }],
            [{ 'align': [] }],
            ['clean'],
            ['link', 'image', 'video']
          ],
          imageUploader: {
            upload: file => {
              return new Promise((resolve, reject) => {
                let formData = new FormData();
                formData.append('formFile', file);
                formData.append('patrolId', this.selectedPatrolId);

                this.$http.post('file/upload',formData, {
                  headers: {
                    'Content-Type': 'multipart/form-data'
                  }
                })
                .then(response => {
                    console.log(response.data.relativeUrl);
                    resolve(response.data.relativeUrl);
                }).catch(response => {
                    reject();
                    console.log(response);
                });
              });
            }
          }
        }
      }
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

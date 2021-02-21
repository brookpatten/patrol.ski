<template>
    <div>
      <CForm @submit.prevent="save">
      <CCard>
        <CCardHeader>
          <slot name="header">
              <CIcon name="cil-calendar"/>
          </slot>
        </CCardHeader>
        <CCardBody>
            <CAlert color="danger" v-if="validationMessage">{{validationMessage}}</CAlert>
            <CInput
            label="Name"
            v-model="event.name"
            :invalidFeedback="validationErrors.Name ? validationErrors.Name.join() : 'Invalid'"
            :isValid="validated ? validationErrors.Name==null : null"
            />
            <CInput
            label="Location"
            v-model="event.location"
            :invalidFeedback="validationErrors.Location ? validationErrors.Location.join() : 'Invalid'"
            :isValid="validated ? validationErrors.Location==null : null"
            />
            
            <label for="event.startsAt">Start</label>
            <datetime type="datetime" v-model="event.startsAt" :minute-step="15" input-class="form-control" :use12-hour="true"></datetime><br/>
            <label for="event.endsAt">End</label>
            <datetime type="datetime" v-model="event.endsAt" :minute-step="15" input-class="form-control" :use12-hour="true"></datetime><br/>

            <CSwitch class="mx-1" color="primary" variant="3d" :checked.sync="event.isInternal" v-if="selectedPatrol.enablePublicSite"/>
            <label for="event.isInternal" v-if="selectedPatrol.enablePublicSite">Show for logged in Patrollers</label><br/>

            <CSwitch class="mx-1" color="primary" variant="3d" :checked.sync="event.isPublic" v-if="selectedPatrol.enablePublicSite"/>
            <label for="event.isPublic" v-if="selectedPatrol.enablePublicSite">Show on Public Site</label>

            <!--<CRow>
              <CCol>
                <CSelect :options="eventSignupModes" v-model="event.signupMode" label="Signup"/>
              </CCol>
              <CCol>
                <CInput v-if="event.signupMode!='None'" label="Maximum Participants" v-model="event.maxSignups"/>
              </CCol>
            </CRow>-->

            <quill-editor v-model="event.eventMarkdown" :options="quillOptions"></quill-editor>

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


import { Datetime } from 'vue-datetime';
import 'vue-datetime/dist/vue-datetime.css';

import Quill from 'quill'
import { quillEditor } from 'vue-quill-editor'
import ImageUploader from 'quill-image-uploader'
import 'quill/dist/quill.core.css'
import 'quill/dist/quill.snow.css'
import 'quill/dist/quill.bubble.css'
Quill.register("modules/imageUploader", ImageUploader);

export default {
  name: 'EditEvent',
  components: { quillEditor,Datetime
  },
  props: ['eventId'],
  data () {
    return {
      event:{},
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
    getEvent() {
        if(this.eventId==0 || !this.eventId){
          this.event={
            name:'',
            location:'',
            eventMarkdown:'',
            isInternal: true,
            isPublic:false,
            signupMode:'None',
            maxSignups: null,
            startsAt:new Date().toUTCString(),
            endsAt:new Date().toUTCString(),
            patrolId: this.selectedPatrolId
          };
        }
        else{
          this.$store.dispatch('loading','Loading...');
          this.$http.get('event/'+this.eventId)
            .then(response => {
                this.event = response.data;
                console.log(response);
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
        }
    },
    save(){
        this.$store.dispatch('loading','Saving...');

        
        this.$http.post('events',this.event)
          .then(response=>{
            this.$router.push({name:'Events'});
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
    },
    eventSignupModes: function(){
      var modes = [{label:'No Signup Required',value:'None'}];
      
      if(this.event.isInternal){
        modes.push({label:'Patrol',value:'Patrol'});
      }
      if(this.selectedPatrol.enablePublicSite && this.event.isPublic){
        modes.push({label:'Anyone',value:'Anyone'});
      }

      return modes;
    }
  },
  watch: {
    selectedPatrolId(){
        this.getEvent();
    }
  },
  mounted: function(){
    this.getEvent();
  }
}
</script>

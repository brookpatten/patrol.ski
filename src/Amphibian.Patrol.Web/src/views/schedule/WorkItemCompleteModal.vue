<template>
    <CModal
        :show.sync="show"
        :no-close-on-backdrop="true"
        color="success"
        size="lg"
        >
    <CRow>
        <CCol><CInput label="Name" v-model="this.editWorkItem.name" disabled/></CCol>
        <CCol><CInput label="Location" v-model="this.editWorkItem.location" disabled/></CCol>
    </CRow>
    <CRow>
        <CCol><CInput disabled label="Scheduled At" :value="new Date(editWorkItem.scheduledAt).toLocaleDateString() + ' '+new Date(editWorkItem.scheduledAt).toLocaleTimeString()"/></CCol>
    </CRow>
    <CRow v-if="editWorkItem.assignments && editWorkItem.assignments.length>0 && allowOverride">
      <CCol>
      <label>Assigned</label>
      <CDataTable
        striped
        small
        fixed
        :items="editWorkItem.assignments"
        :fields="[{key:'assignedUser',label:''}]"
        >
        <template #assignedUser="data">
            <td>{{data.item.user.lastName}}, {{data.item.user.firstName}}</td>
        </template>
      </CDataTable>
      </CCol>
    </CRow>
    <CRow v-if="allowOverride">
      <CCol>
        <label>Complete As</label>
        <CSelect :options="filteredUserList" :value.sync="selectedUserId"></CSelect>
      </CCol>
    </CRow>
    <CRow>
      <CCol>
        <label>Work Notes</label>
        <quill-editor v-model="workNotes"></quill-editor>
      </CCol>
    </CRow>
    <template #header>
        <h6 class="modal-title">Complete {{editWorkItem.name}} {{editWorkItem.location}}</h6>
    </template>
    <template #footer>
      <CButton @click="show = false" color="light">Back</CButton>
      <CButton @click="complete" color="success">Complete</CButton>
    </template>
    </CModal>
</template>

<script>

import AuthenticatedPage from '../../mixins/AuthenticatedPage';
import { quillEditor } from 'vue-quill-editor'
import 'quill/dist/quill.core.css'
import 'quill/dist/quill.snow.css'
import 'quill/dist/quill.bubble.css'

export default {
  name: 'WorkItemCompleteModal',
  mixins: [AuthenticatedPage],
  components: { quillEditor
  },
  props:['workItem','trigger'],
  data () {
    return {
      editWorkItem: {},
      users:[],
      filteredUserList:[],
      selectedUserId:null,
      show:false,
      workNotes:''
    }
  },
  methods: {
    complete(){
      this.$store.dispatch('loading','Completing...');

      var userId = null;
      if(this.allowOverride){
        userId = this.selectedUserId;
      }

      this.$http.post('workitem/complete',{id:this.editWorkItem.id,workNotes:this.workNotes,forUserId:userId})
          .then(response => {
              console.log(response);
              this.$emit('completed');
              this.show=false;
              this.selectedUserId = null;
              this.workNotes = '';
          }).catch(response => {
              console.log(response);
          }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    getUsers() {
        this.$store.dispatch('loading','Loading...');
        this.$http.get('user/list/'+this.selectedPatrolId)
            .then(response => {
                console.log(response);
                this.users = response.data;
                this.filterUserListItems();
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    filterUserListItems:function(){
      var filtered = _.sortBy(this.users,['lastName','firstName']);

      this.filteredUserList = _.map(filtered,function(u){
          return {
              label:u.lastName+", "+u.firstName,
              value:u.id
          };
      });

      if(this.selectedPatrol.enableShiftSwaps){
          this.filteredUserList.splice(0,0,{label:'Me',value:null});
      }

      if(this.filteredUserList.length>0){
          this.selectedUserId = this.filteredUserList[0].value;
      }
    }
  },
  computed: {
    allowOverride: function(){
      return (this.editWorkItem.canAdmin || this.hasPermission('MaintainWorkItems'));
    }
  },
  watch: {
    selectedPatrolId(){
      this.show=false;
    },
    workItem(){
      this.editWorkItem = _.clone(this.workItem);
    },
    trigger(){
      this.show=true;
    }
  },
  mounted: function(){
    this.editWorkItem = _.clone(this.workItem);
    this.getUsers();
  }
}
</script>

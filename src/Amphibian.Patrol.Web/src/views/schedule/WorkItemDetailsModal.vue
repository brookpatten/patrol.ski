<template>
    <CModal
        :show.sync="show"
        :no-close-on-backdrop="true"
        color="primary"
        size="lg"
        >
    <CRow>
        <CCol><CInput label="Name" v-model="this.editWorkItem.name" :disabled="!allowEdit"/></CCol>
        <CCol><CInput label="Location" v-model="this.editWorkItem.location" :disabled="!allowEdit"/></CCol>
    </CRow>
    <CRow>
        <CCol><CInput :disabled="!allowEdit" label="Scheduled At" :value="new Date(editWorkItem.scheduledAt).toLocaleDateString() + ' '+new Date(editWorkItem.scheduledAt).toLocaleTimeString()"/></CCol>
        <CCol><CInput :disabled="!allowEdit" label="Completion" :value="this.editWorkItem.completionMode"/></CCol>
    </CRow>
    <CRow>
        <CCol><CInput disabled label="Owner" v-if="editWorkItem.createdBy" :value="editWorkItem.createdBy.lastName+', '+editWorkItem.createdBy.firstName"/></CCol>
        <CCol><CInput :disabled="!allowEdit" label="Owning Group" v-if="this.editWorkItem.adminGroup" v-model="this.editWorkItem.adminGroup.name"/></CCol>
    </CRow>
    <CRow>
        <CCol><CInput v-if="this.editWorkItem.recurringWorkItem" disabled label="Recurring" :value="this.editWorkItem.recurringWorkItem ? 'Yes' : 'No'"/></CCol>
        <CCol>
          <CInput disabled label="Shift" v-if="this.editWorkItem.scheduledShift && this.editWorkItem.scheduledShift.shift" :value="this.editWorkItem.scheduledShift.shift.name"/>
          <CInput disabled label="Group" v-if="this.editWorkItem.scheduledShift && this.editWorkItem.scheduledShift.group" :value="this.editWorkItem.scheduledShift.group.name"/>
        </CCol>
    </CRow>
    <CRow v-if="editWorkItem.canceledAt">
        <CCol><CInput disabled label="Canceled" :value="new Date(editWorkItem.canceledAt).toLocaleDateString() + ' '+new Date(editWorkItem.canceledAt).toLocaleTimeString()"/></CCol>
        <CCol><CInput disabled label="Canceled By" :value="this.editWorkItem.canceledBy.lastName+', '+editWorkItem.canceledBy.firstName"/></CCol>
    </CRow>
    <CRow v-if="editWorkItem.completedAt">
        <CCol><CInput disabled label="Completed" :value="new Date(editWorkItem.completedAt).toLocaleDateString() + ' '+new Date(editWorkItem.completedAt).toLocaleTimeString()"/></CCol>
        <CCol><CInput disabled label="Completed By" :value="this.editWorkItem.completedBy.lastName+', '+editWorkItem.completedBy.firstName"/></CCol>
    </CRow>
    <CRow>
      <CCol>
      <CDataTable
        striped
        small
        fixed
        :items="editWorkItem.assignments"
        :fields="[{key:'assignedUser',label:''},{key:'buttons',label:''}]"
        >
        <template #assignedUser="data">
            <td>{{data.item.user.lastName}}, {{data.item.user.firstName}}</td>
        </template>
        <template #buttons="data">
            <td>
                <CButtonGroup class="float-right" v-if="allowAssign">
                    <CButton size="sm" color="danger" @click="removeUser(data.item)">Remove</CButton>
                </CButtonGroup>
            </td>
        </template>
        <template #footer="data">
            <tfoot>
                <tr v-if="allowAssign && filteredUserList && filteredUserList.length>0">
                    <td><CSelect :options="filteredUserList" :value.sync="selectedUserId"></CSelect></td>
                    <td><CButton size="sm" color="success" class="float-right" @click="addUser(selectedUserId)">Add</CButton></td>
                </tr>
            </tfoot>
        </template>
        <template #no-items-view>
            <span>Assign people...</span>
        </template>
    </CDataTable>
    </CCol>
    </CRow>
    <CRow>
      <CCol v-if="allowEdit">
        <quill-editor v-model="editWorkItem.descriptionMarkup"></quill-editor>
      </CCol>
      <CCol v-if="!allowEdit">
        <CCard><CCardBody>
        <span v-html="editWorkItem.descriptionMarkup"/>
        </CCardBody></CCard>
      </CCol>
    </CRow>
    <template #header>
        <h6 class="modal-title">{{editWorkItem.name}} {{editWorkItem.location}}</h6>
    </template>
    <template #footer>
      <CButton @click="show = false" color="light">Back</CButton>
      <CButton v-if="allowEdit || allowAssign" @click="saveWorkItem" color="info">Save Changes</CButton>
      <CButton v-if="allowCancel" @click="cancel" color="warning">Cancel Work Item</CButton>
      <CButton v-if="allowComplete" @click="complete" color="success">Complete Work Item</CButton>
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
  name: 'WorkItemDetailsModal',
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
      show:false
    }
  },
  methods: {
    removeUser(assignment){
      this.editWorkItem.assignments = _.filter(this.editWorkItem.assignments,function(y){return y!=assignment;});
      this.filterUserListItems();
    },
    addUser(userId){
      var user = _.find(this.users,{id:userId});
      this.editWorkItem.assignments.push({userId:userId,workItemId:this.editWorkItem.id});
      this.filterUserListItems();
    },
    saveWorkItem() {
      this.$store.dispatch('loading','Loading...');

      if(this.allowEdit){
        this.$http.post('workitem',this.editWorkItem)
          .then(response => {
              console.log(response);
              this.$emit('edited');
              this.show=false;
          }).catch(response => {
              console.log(response);
          }).finally(response=>this.$store.dispatch('loadingComplete'));
      }
      else if(this.allowAssign){
        this.$http.post('workitem/assign',this.editWorkItem)
          .then(response => {
              console.log(response);
              this.$emit('edited');
              this.show=false;
          }).catch(response => {
              console.log(response);
          }).finally(response=>this.$store.dispatch('loadingComplete'));
      }
    },
    cancel(){
      this.$store.dispatch('loading','Canceling...');
      this.$http.post('workitem/cancel',{id:this.editWorkItem.id})
          .then(response => {
              console.log(response);
              this.$emit('edited');
              this.show=false;
          }).catch(response => {
              console.log(response);
          }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    complete(){
      this.$emit('complete');
      this.show=false;
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
      let editWorkItem = this.editWorkItem;
      var filtered = _.filter(this.users,function(u){return _.find(editWorkItem.assignments,function(a){return a.user && a.user.id==u.id;})==null;});

      filtered = _.sortBy(filtered,['lastName','firstName']);

      this.filteredUserList = _.map(filtered,function(u){
          return {
              label:u.lastName+", "+u.firstName,
              value:u.id
          };
      });

      if(this.filteredUserList.length>0){
          this.selectedUserId = this.filteredUserList[0].value;
      }
    }
  },
  computed: {
    allowEdit: function(){
      return !this.editWorkItem.completedAt 
      && !this.editWorkItem.canceledAt 
      && (this.editWorkItem.canAdmin || this.hasPermission('MaintainWorkItems'))
      && !this.editWorkItem.recurringWorkItem;
    },
    allowAssign: function(){
      return !this.editWorkItem.completedAt 
      && !this.editWorkItem.canceledAt 
      && (this.editWorkItem.canAdmin || this.hasPermission('MaintainWorkItems'));
    },
    allowComplete: function(){
      return !this.editWorkItem.completedAt 
      && !this.editWorkItem.canceledAt 
      && (this.editWorkItem.canAdmin || this.editWorkItem.canComplete || this.hasPermission('MaintainWorkItems'));
    },
    allowCancel: function(){
      return !this.editWorkItem.completedAt 
      && !this.editWorkItem.canceledAt 
      && (this.editWorkItem.canAdmin || this.hasPermission('MaintainWorkItems'));
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

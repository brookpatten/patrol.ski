<template>
    <CModal
        :show.sync="show"
        :no-close-on-backdrop="true"
        color="primary"
        size="lg"
        >
    <CRow>
        <CCol><CInput label="Name" v-model="editWorkItem.name" :disabled="!allowEdit"/></CCol>
        <CCol><CInput label="Location" v-model="editWorkItem.location" :disabled="!allowEdit"/></CCol>
    </CRow>
    <CRow>
        <CCol>
          <div class="form-group" role="group">
            <label>Scheduled</label>
            <datetime type="datetime" v-model="editWorkItem.scheduledAt" :minute-step="5" input-class="form-control" :use12-hour="true"></datetime>
          </div>
        </CCol>
        <CCol><CSelect :options="completionModes" label="Completion" :value.sync="editWorkItem.completionMode" :disabled="!allowEdit"></CSelect></CCol>
    </CRow>
    <CRow>
        <CCol><CInput disabled label="Owner" v-if="editWorkItem.createdBy" :value="editWorkItem.createdBy.lastName+', '+editWorkItem.createdBy.firstName"/></CCol>
        <CCol><CSelect :disabled="!allowEdit" :options="groups" label="Owning Group" :value.sync="editWorkItem.adminGroupId"></CSelect></CCol>
    </CRow>
    <CRow>
        <CCol><CInput v-if="editWorkItem.recurringWorkItem" disabled label="Recurring" :value="editWorkItem.recurringWorkItem ? 'Yes' : 'No'"/></CCol>
        <CCol>
          <CInput disabled label="Shift" v-if="editWorkItem.scheduledShift && editWorkItem.scheduledShift.shift" :value="editWorkItem.scheduledShift.shift.name"/>
          <CInput disabled label="Group" v-if="editWorkItem.scheduledShift && editWorkItem.scheduledShift.group" :value="editWorkItem.scheduledShift.group.name"/>
        </CCol>
    </CRow>
    <CRow v-if="editWorkItem.canceledAt">
        <CCol><CInput disabled label="Canceled" :value="new Date(editWorkItem.canceledAt).toLocaleDateString() + ' '+new Date(editWorkItem.canceledAt).toLocaleTimeString()"/></CCol>
        <CCol><CInput disabled label="Canceled By" :value="editWorkItem.canceledBy.lastName+', '+editWorkItem.canceledBy.firstName"/></CCol>
    </CRow>
    <CRow v-if="editWorkItem.completedAt">
        <CCol><CInput disabled label="Completed" :value="new Date(editWorkItem.completedAt).toLocaleDateString() + ' '+new Date(editWorkItem.completedAt).toLocaleTimeString()"/></CCol>
        <CCol><CInput disabled label="Completed By" :value="editWorkItem.completedBy.lastName+', '+editWorkItem.completedBy.firstName"/></CCol>
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
        <h6 v-if="editWorkItem.id" class="modal-title">{{editWorkItem.name}} {{editWorkItem.location}}</h6>
        <h6 v-if="!editWorkItem.id" class="modal-title">New Work Item</h6>
    </template>
    <template #footer>
      <CButton @click="show = false" color="light">Back</CButton>
      <CButton v-if="editWorkItem.id && (allowEdit || allowAssign)" @click="saveWorkItem" color="info">Save Changes</CButton>
      <CButton v-if="editWorkItem.id &&allowCancel" @click="cancel" color="warning">Cancel Work Item</CButton>
      <CButton v-if="editWorkItem.id &&allowComplete" @click="complete" color="success">Complete Work Item</CButton>
      <CButton v-if="!editWorkItem.id" @click="saveWorkItem" color="success">Create</CButton>
    </template>
    </CModal>
    
</template>

<script>

import AuthenticatedPage from '../../mixins/AuthenticatedPage';
import { quillEditor } from 'vue-quill-editor';
import 'quill/dist/quill.core.css';
import 'quill/dist/quill.snow.css';
import 'quill/dist/quill.bubble.css';

import { Datetime } from 'vue-datetime';
import 'vue-datetime/dist/vue-datetime.css';


export default {
  name: 'WorkItemDetailsModal',
  mixins: [AuthenticatedPage],
  components: { quillEditor,Datetime
  },
  props:['workItem','trigger'],
  data () {
    return {
      completionModes:[{label:'Anyone',value:'Any'}
                      ,{label:'Any Assigned',value:'AnyAssigned'}
                      ,{label:'All Assigned',value:'AllAssigned'}
                      ,{label:'Owner(s)',value:'AdminOnly'}],
      editWorkItem: {},
      users:[],
      filteredUserList:[],
      selectedUserId:null,
      show:false,
      groups:[]
    }
  },
  methods: {
    removeUser(assignment){
      this.editWorkItem.assignments = _.filter(this.editWorkItem.assignments,function(y){return y!=assignment;});
      this.filterUserListItems();
    },
    addUser(userId){
      var user = _.find(this.users,{id:userId});
      this.editWorkItem.assignments.push({userId:userId,user:user,workItemId:this.editWorkItem.id});
      this.filterUserListItems();
    },
    saveWorkItem() {
      if(!this.editWorkItem.name || this.editWorkItem.name==''){
        return;
      }

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
    getGroups() {
      this.$store.dispatch('loading','Loading...');
      this.$http.get('user/groups/'+this.selectedPatrolId)
          .then(response => {
              console.log(response);
              this.groups = _.map(response.data,function(u){
                  return {label:u.name,value:u.id};
              });

              this.groups.splice(0,0,{label:'(None)',value:null});
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
      && (this.editWorkItem.canAdmin || this.editWorkItem.canComplete || this.hasPermission('MaintainWorkItems'))
      && this.editWorkItem.isDue;
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

      //if it's new, fill it with defaults
      if(!this.editWorkItem.id){
        this.editWorkItem.name = '';
        this.editWorkItem.location='';
        this.editWorkItem.assignments=[];
        this.editWorkItem.completionMode = 'Any';
        this.editWorkItem.recurringWorkItemId = null;
        this.editWorkItem.recurringWorkItemId = null;
        this.editWorkItem.scheduledShiftId=null;
        this.editWorkItem.scheduledShift=null;
        this.editWorkItem.adminGroupId = null;
        this.editWorkItem.adminGroup = null;
        this.editWorkItem.createdByUserId = this.userId;
        this.editWorkItem.createdBy = null;
        this.editWorkItem.createdAt = new Date();
        this.editWorkItem.completedAt = null;
        this.editWorkItem.completedByUserId = null;
        this.editWorkItem.completedBy = null;
        this.editWorkItem.canceledAt = null;
        this.editWorkItem.canceledByUserId = null;
        this.editWorkItem.canceledBy = null;
        this.editWorkItem.canAdmin = true;
        this.editWorkItem.canComplete = false;
        this.editWorkItem.patrolId = this.selectedPatrolId;
        this.editWorkItem.descriptionMarkup = '';
        this.editWorkItem.scheduledAt = new Date().toISOString();
      }
    },
    trigger(){
      this.show=true;
    }
  },
  mounted: function(){
    this.editWorkItem = _.clone(this.workItem);
    this.getUsers();
    this.getGroups();
  }
}
</script>

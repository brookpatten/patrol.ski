<template>
    <CModal
        :show.sync="show"
        :no-close-on-backdrop="true"
        color="primary"
        size="lg"
        :key="key"
        >
    <CRow>
        <CCol><CInput label="Name" v-model="editWorkItem.name" /></CCol>
        <CCol><CInput label="Location" v-model="editWorkItem.location" /></CCol>
    </CRow>
    <CRow>
        <CCol>
          
        </CCol>
        <CCol><CSelect :options="completionModes" label="Completion" :value.sync="editWorkItem.completionMode"></CSelect></CCol>
    </CRow>
    <CRow>
        <CCol><CInput disabled label="Owner" v-if="editWorkItem.createdBy" :value="editWorkItem.createdBy.lastName+', '+editWorkItem.createdBy.firstName"/></CCol>
        <CCol><CSelect :options="groups" label="Owning Group" :value.sync="editWorkItem.adminGroupId"></CSelect></CCol>
    </CRow>
    <CRow v-if="recurModes.length>1 && shifts.length>0">
        <CCol><CSelect :options="recurModes" label="Recur On" :value.sync="recurMode"></CSelect></CCol>
    </CRow>
    <CRow v-if="recurMode=='Timed'">
        <CCol>
          <div class="form-group" role="group">
            <label>From</label>
            <datetime type="datetime" v-model="editWorkItem.recurStart" :minute-step="5" input-class="form-control" :use12-hour="true"></datetime>
          </div>
        </CCol>
        <CCol>
          <div class="form-group" role="group">
            <label>To</label>
            <datetime type="datetime" v-model="editWorkItem.recurEnd" :minute-step="5" input-class="form-control" :use12-hour="true"></datetime>
          </div>
        </CCol>
        <CCol>
          <CSelect :options="sequence" label="Every" :value.sync="editWorkItem.recurIntervalCount"></CSelect>
          <CSelect :options="intervals" label="" :value.sync="editWorkItem.recurInterval"></CSelect>
        </CCol>
    </CRow>
    <CRow v-if="recurMode=='Shift'">
        <CCol>
          <CDataTable
              striped
              small
              fixed
              :items="editWorkItem.shifts"
              :fields="[
                {key:'shift',label:'Shift'},
                {key:'scheduled',label:'Scheduled'},
                {key:'shiftAssignmentMode',label:'Assignment'},
                {key:'buttons',label:''}
              ]"
              >
              <template #shift="data">
                  <td>{{data.item.shift.name}}</td>
              </template>
              <template #scheduled="data">
                  <td>
                    <CSelect :options="hours" :value.sync="data.item.scheduledAtHour" style="width: 65px;display:inline-block;"></CSelect>:<CSelect :options="minutes" :value.sync="data.item.scheduledAtMinute" style="width: 65px;display:inline-block;"></CSelect>
                  </td>
              </template>
              <template #shiftAssignmentMode="data">
                  <td>
                    <CSelect :options="shiftAssignmentModes" :value.sync="data.item.shiftAssignmentMode"></CSelect>
                  </td>
              </template>
              <template #buttons="data">
                  <td>
                      <CButtonGroup class="float-right">
                          <CButton size="sm" color="danger" @click="removeShift(data.item)">Remove</CButton>
                      </CButtonGroup>
                  </td>
              </template>
              <template #footer="data">
                  <tfoot>
                      <tr>
                          <td colspan="3"><CSelect :options="shiftItems" :value.sync="selectedShiftId"></CSelect></td>
                          <td><CButton size="sm" color="success" class="float-right" @click="addShift(selectedShiftId)">Add</CButton></td>
                      </tr>
                  </tfoot>
              </template>
              <template #no-items-view>
                  <span>Add Shifts...</span>
              </template>
          </CDataTable>
        </CCol>
    </CRow>
    <CRow v-if="editWorkItem.createdAt && editWorkItem.createdBy">
        <CCol><CInput disabled label="Created" :value="new Date(editWorkItem.createdAt).toLocaleDateString() + ' '+new Date(editWorkItem.createdAt).toLocaleTimeString()"/></CCol>
        <CCol><CInput disabled label="Created By" :value="editWorkItem.createdBy.lastName+', '+editWorkItem.createdBy.firstName"/></CCol>
    </CRow>
    <CRow v-if="recurMode=='Timed'">
      <CCol>
      <CDataTable
        striped
        small
        fixed
        :items="editWorkItem.nextOccurenceUsers"
        :fields="[{key:'assignedUser',label:''},{key:'buttons',label:''}]"
        >
        <template #assignedUser="data">
            <td>{{data.item.lastName}}, {{data.item.firstName}}</td>
        </template>
        <template #buttons="data">
            <td>
                <CButtonGroup class="float-right">
                    <CButton size="sm" color="danger" @click="removeUser(data.item)">Remove</CButton>
                </CButtonGroup>
            </td>
        </template>
        <template #footer="data">
            <tfoot>
                <tr v-if="filteredUserList && filteredUserList.length>0">
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
      <CCol>
        <quill-editor v-model="editWorkItem.descriptionMarkup"></quill-editor>
      </CCol>
    </CRow>
    <template #header>
        <h6 v-if="editWorkItem.id" class="modal-title">{{editWorkItem.name}} {{editWorkItem.location}}</h6>
        <h6 v-if="!editWorkItem.id" class="modal-title">New Recurring Work Item</h6>
    </template>
    <template #footer>
      <CButton @click="show = false" color="light">Back</CButton>
      <CButton v-if="editWorkItem.id" @click="saveWorkItem" color="info">Save Changes</CButton>
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
  name: 'RecurringWorkItemDetailsModal',
  mixins: [AuthenticatedPage],
  components: { quillEditor,Datetime
  },
  props:['workItem','trigger'],
  data () {
    return {
      key: 0,
      completionModes:[{label:'Anyone',value:'Any'}
                      ,{label:'Any Assigned',value:'AnyAssigned'}
                      ,{label:'All Assigned',value:'AllAssigned'}
                      ,{label:'Owner(s)',value:'AdminOnly'}],
      recurModes:[],
      editWorkItem: {},
      recurMode:'Timed',
      users:[],
      filteredUserList:[],
      selectedUserId:null,
      show:false,
      groups:[],
      shifts:[],
      shiftItems:[],
      selectedShiftId:0,
      intervals:[
        {label:'Year(s)',value:'Year'}
        ,{label:'Month(s)',value:'Month'}
        ,{label:'Week(s)',value:'Week'}
        ,{label:'Day(s)',value:'Day'}
        ,{label:'Hour(s)',value:'Hour'}
        ,{label:'Minute(s)',value:'Minute'}],
      sequence:[],
      hours:[],
      minutes:[],
      shiftAssignmentModes:[
        {label:'Automatic',value:'Auto'},
        {label:'Manual',value:'ManualEachShift'},
        {label:'Copy most recent',value:'CopyPrevious'}
      ]
    }
  },
  methods: {
    removeUser(user){
      _.remove(this.editWorkItem.nextOccurenceUsers,function(y){return y.id==user.id;});
      this.filterUserListItems();
      this.key++;
    },
    addUser(userId){
      var user = _.find(this.users,{id:userId});
      if(!this.editWorkItem.nextOccurenceUsers){
        this.editWorkItem.nextOccurenceUsers = [];
      }
      this.editWorkItem.nextOccurenceUsers.push(user);
      this.filterUserListItems();
      this.key++;
    },
    removeShift(shift){
      _.remove(this.editWorkItem.shifts,function(y){return y==shift;});
      this.key++;
    },
    addShift(shiftId){
      console.log('addShift:'+shiftId);
      var shift = _.find(this.shifts,{id:shiftId});
      if(!this.editWorkItem.shifts){
        this.editWorkItem.shifts = [];
      }
      this.editWorkItem.shifts.push({shiftId:shift.id,shift:shift,scheduledAtHour:shift.startHour,scheduledAtMinute:shift.startMinute,shiftAssignmentMode:'Auto',recurringWorkItemId:this.editWorkItem.id});
      this.key++;
    },
    saveWorkItem() {
      if(!this.editWorkItem.name || this.editWorkItem.name==''){
        return;
      }

      var toSave = _.clone(this.editWorkItem);

      if(this.recurMode=="Timed"){
        toSave.shifts = null;
      }
      else {
        toSave.recurStart = null;
        toSave.recurEnd = null;
        toSave.recurInterval = null;
        toSave.recurIntervalCount = null;
      }

      this.$store.dispatch('loading','Loading...');

      this.$http.post('workitem/recurring',toSave)
          .then(response => {
              console.log(response);
              this.$emit('edited');
              this.show=false;
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
    getShifts() {
      this.$store.dispatch('loading','Loading...');
      this.$http.get('schedule/shifts?patrolid='+this.selectedPatrolId)
          .then(response => {
              console.log(response);
              this.shifts = response.data;
              this.shiftItems = _.map(response.data,function(u){
                  return {label:u.name,value:u.id};
              });
              if(this.shiftItems.length>0){
                this.selectedShiftId = this.shiftItems[0].value;
              }
          }).catch(response => {
              console.log(response);
          }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    filterUserListItems:function(){
      let editWorkItem = this.editWorkItem;
      var filtered = _.filter(this.users,function(u){return _.find(editWorkItem.nextOccurenceUsers,function(a){return a.id==u.id;})==null;});

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
    
  },
  watch: {
    selectedPatrolId(){
      this.show=false;
    },
    workItem(){
      var wi = _.clone(this.workItem);

      

      //if it's new, fill it with defaults
      if(!wi.id){
        var now = new Date();
        var endRange = new Date();
        endRange.setDate(now.getDate() + 60);

        wi.name = '';
        wi.location='';
        wi.completionMode = 'Any';
        wi.maximumRandomCount = null;
        wi.adminGroupId = null;
        wi.adminGroup = null;
        wi.createdByUserId = this.userId;
        wi.createdBy = null;
        wi.createdAt = new Date();
        wi.patrolId = this.selectedPatrolId;
        wi.descriptionMarkup = '';
        wi.recurStart = new Date(now.getFullYear(),now.getMonth(),now.getDate(),now.getHours(),0,0,0).toUTCString();
        wi.recurEnd = new Date(endRange.getFullYear(),endRange.getMonth(),endRange.getDate(),endRange.getHours(),0,0,0).toUTCString();
        wi.recurInterval = 'Day';
        wi.recurIntervalCount = 1;
        this.recurMode = "Timed"
      }
      else
      {
        if(wi.shifts && wi.shifts.length>0){
          this.recurMode = "Shift";
        }
        else {
          this.recurMode = "Timed";
        }

        if(!wi.nextOccurenceUsers){
          wi.nextOccurenceUsers=[];
        }

        if(!wi.shifts){
          wi.shifts=[];
        }
      }

      this.editWorkItem = wi;

      this.filterUserListItems();
    },
    trigger(){
      this.show=true;
    }
  },
  mounted: function(){
    this.editWorkItem = _.clone(this.workItem);
    this.getUsers();
    this.getGroups();

    if(this.selectedPatrol.enableScheduling){
      this.recurModes = [
         {label:'Timed Interval',value:'Timed'}
        ,{label:'Shift',value:'Shift'}];
      this.getShifts();
    }
    else{
      this.recurModes = [
         {label:'Timed Interval',value:'Timed'}];
    }
    this.recurMode = this.recurModes[0].value;

    for(var i=1;i<365;i++){
      this.sequence.push(i);
    }
    
    for(var i=0;i<24;i++){
      this.hours.push({label:i+"",value:i});
    }

    for(var i=0;i<60;i++){
      this.minutes.push({label:(i+"").padStart(2,'0'),value:i});
    }
  }
}
</script>

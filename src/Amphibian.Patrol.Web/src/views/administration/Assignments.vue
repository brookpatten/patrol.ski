<template>
    <div>
        <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-pencil"/> Assignments
            </slot>
            </CCardHeader>
            <CCardBody>
                <CForm>
                 <CRow>
                    <CCol md="3">
                        <CSelect
                        label="Plan"
                        :value.sync="selectedPlanId"
                        :options="planOptions"
                        placeholder="Plan"
                        />
                    </CCol>
                    <CCol md="3">
                        <CSelect
                        label="User"
                        :value.sync="selectedUserId"
                        :options="userOptions"
                        placeholder="User"
                        />
                    </CCol>
                    <CCol md="3">
                        <div><label>Completed</label></div>
                        <CSwitch label="Completed" class="mx-1" color="success" :checked.sync="completed" variant="3d" v-bind="labelIcon" />
                    </CCol>
                    <CCol md="3">
                        <CButton v-if="hasPermission('MaintainAssignments')" color="primary" :to="{ name: 'NewAssignment', params: { planId: selectedPlanId } }">New Assignment(s)</CButton>
                    </CCol>
                 </CRow>
                </CForm>
            <CDataTable
                :items="assignments"
                :fields="assignmentFields"
                sorter>
                <template #user="data">
                    <td>{{data.item.userLastName}}, {{data.item.userFirstName}}</td>
                </template>
                <template #signatures="data">
                    <td>{{data.item.signatures}}/{{data.item.signaturesRequired}}</td>
                </template>
                <template #progress="data">
                    <td><CProgress
                            :value="(Math.round((data.item.signatures / data.item.signaturesRequired) * 100))"
                            color="success"
                            striped
                            show-percentage
                            />
                    </td>
                </template>
                <template #dueAt="data">
                    <td>{{data.item.dueAt ? (new Date(data.item.dueAt)).toLocaleDateString() : ''}}</td>
                </template>
                <template #assignedAt="data">
                    <td>{{(new Date(data.item.assignedAt)).toLocaleDateString()}}</td>
                </template>
                <template #completedAt="data">
                    <td>{{data.item.completedAt ? (new Date(data.item.completedAt)).toLocaleDateString() : ''}}</td>
                </template>
              <template #buttons="data">
                  <td>
                    <CButtonGroup size="sm">
                      <CButton color="info" :to="{ name: 'Assignment', params: { assignmentId: data.item.id } }">View</CButton>
                      <CButton v-if="hasPermission('MaintainPlans')" color="primary" :to="{ name: 'EditPlan', params: { planId: data.item.planId } }">Plan</CButton>
                      <CButton v-if="hasPermission('MaintainAssignments')" color="success" :to="{ name: 'EditAssignment', params: { assignmentId: data.item.id } }">Edit</CButton>
                    </CButtonGroup>
                  </td>
              </template>
            </CDataTable>
            </CCardBody>
        </CCard>
    </div>
</template>

<script>
export default {
  name: 'Plans',
  components: {
  },
  data () {
    return {
        selectedUserId:0,
        selectedPlanId:0,
        assignments:[],
        plans:[],
        users:[],
        assignmentFields:[
            {key:'user',label:'Assignee'},
            {key:'planName',label:'Plan'},
            {key:'assignedAt',label:'Assigned'},
            {key:'dueAt',label:'Due'},
            {key:'completedAt',label:'Completed'},
            {key:'signatures',label:'Signatures'},
            {key:'progress',label:'Progress'},
            {key:'buttons',label:''},
        ],
        completed:false
    }
  },
  props: ['planId','userId'],
  mounted: function(){
    this.selectedUserId = this.userId;
    this.selectedPlanId = this.planId;
    this.loadPlans();
    this.loadUsers();
    this.loadAssignments();
  },
  computed: {
    selectedPatrolId: function () {
      return this.$store.state.selectedPatrolId;
    },
    selectedPatrol: function (){
        return this.$store.getters.selectedPatrol;
    },
    planOptions: function(){
        var options = _.map(this.plans,function(p){return {value:p.id,label:p.name};});
        options.splice(0,0,{label:'Any',value:null});
        return options;
    },
    userOptions: function(){
        var options = _.map(this.users,function(p){return {value:p.id,label:p.lastName+', '+p.firstName};});
        options.splice(0,0,{label:'Any',value:null});
        return options;
    }
  },
  methods: {
        loadPlans() {
            this.$store.dispatch('loading','Loading...');
            this.$http.get('plans?patrolId=' + this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    this.plans=response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        loadUsers() {
            this.$store.dispatch('loading','Loading...');
            this.$http.get('user/list/'+this.selectedPatrolId)
                .then(response => {
                    console.log(response);
                    this.users = response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
            },
        loadAssignments(){
            this.$store.dispatch('loading','Loading...');
            this.$http.post('assignments/search',{patrolId:this.selectedPatrolId, planId:this.selectedPlanId, userId:this.selectedUserId, completed:this.completed})
                .then(response => {
                    console.log(response);
                    this.assignments = response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
            },
        hasPermission: function(permission){
          return this.selectedPatrol!=null && this.selectedPatrol.permissions!=null && _.indexOf(this.selectedPatrol.permissions,permission) >= 0;
        }
  },
  watch: {
    selectedPatrolId(){
      this.loadPlans();
      this.loadUsers();
      this.loadAssignments();
    },
    selectedPlanId(){
        this.loadAssignments();
    },
    selectedUserId(){
        this.loadAssignments();
    },
    completed(){
        this.loadAssignments();
    }
  }
}
</script>

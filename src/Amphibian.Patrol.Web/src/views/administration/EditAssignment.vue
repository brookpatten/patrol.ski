<template>
    <div>
      <CForm @submit.prevent="save">
      <CCard>
        <CCardHeader>
          <slot name="header">
            <CIcon name="cil-user"/>
          </slot>
        </CCardHeader>
        <CCardBody>
            <CAlert color="danger" v-if="validationMessage">{{validationMessage}}</CAlert>
            <CInput
            label="Assignee"
            disabled
            :value="user.firstName+' '+user.lastName"
            />
            <CInput
            label="Plan"
            disabled
            :value="plan.name"
            />
            
            <label for="announcement.expireAt">Assignment Due</label>
            <datepicker v-model="assignment.dueAt" input-class="form-control" calendar-class="card"></datepicker><br/>

            <label for="announcement.expireAt">Completed On</label>
            <datepicker v-model="assignment.completedAt" input-class="form-control" calendar-class="card"></datepicker><br/>

            <CInput
            label="Signatures"
            type="number"
            disabled
            :value="assignment.signatures ? assignment.signatures.length : 0"
            />
        
        </CCardBody>
        <CCardFooter>
            <CButtonGroup>
              <CButton color="danger">Cancel Assignment</CButton>
              <CButton color="success" :to="{name:'Assignment',params:{assignmentId:assignmentId}}">View Assignment</CButton>
              <CButton type="submit" color="primary">Save</CButton>
            </CButtonGroup>
        </CCardFooter>
      </CCard>
      </CForm>
    </div>
</template>

<script>

import Datepicker from 'vuejs-datepicker';

export default {
  name: 'EditAssignment',
  components: { Datepicker
  },
  props: {'assignmentId':Number},
  data () {
    return {
      assignment:{},
      user:{},
      plan:{},
      validationMessage:'',
      validationErrors:{},
      validated:false
    }
  },
  methods: {
    getAssignment(){
        this.$store.dispatch('loading','Loading...');
        this.$http.get('assignment/'+this.assignmentId)
          .then(response=>{
            this.assignment = response.data.assignment;
            if(this.assignment.dueAt){
              this.assignment.dueAt = new Date(this.assignment.dueAt).toLocaleDateString();
            }
            if(this.assignment.completedAt){
              this.assignment.completedAt = new Date(this.assignment.completedAt).toLocaleDateString();
            }
            this.plan = response.data.plan;
            this.user = response.data.user;
          }).catch(response=>{
            this.validated=true;
            this.validationMessage = response.response.data.title;
            this.validationErrors = response.response.data.errors;
          }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    save(){
        this.$store.dispatch('loading','Saving...');
        var saved = {id:parseInt(this.assignmentId)};
        
        if(this.assignment.dueAt){
          saved.dueAt = (new Date(this.assignment.dueAt)).toISOString();
        }

        if(this.assignment.completedAt){
          saved.completedAt = (new Date(this.assignment.completedAt)).toISOString();
        }
        
        console.log(JSON.stringify(saved));
        this.$http.put('assignments/update',saved)
          .then(response=>{
            this.$router.push({name:'Assignments',params:{planId:this.assignment.planId}});
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
    
  },
  mounted: function(){
    this.getAssignment();
  }
}
</script>

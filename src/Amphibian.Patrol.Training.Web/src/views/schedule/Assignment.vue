<template>
    <div>
        <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-grid"/> {{plan.name}}
            </slot>
            </CCardHeader>
        </CCard>

        <AssignmentSection v-for="s in plan.sections" :key="s.id" :assignment="assignment" :section="s">
        </AssignmentSection>
    </div>
</template>

<script>
import AssignmentSection from './AssignmentSection';

export default {
  name: 'Assignment',
  components: { AssignmentSection
  },
  props: ['assignmentId'],
  data () {
    return {
        assignment: {},
        plan:{},
    }
  },
  methods: {
        getAssignment(assignmentId) {
            this.$http.get('assignment/'+assignmentId)
                .then(response => {
                    console.log(response);
                    this.assignment = response.data.assignment;
                    this.plan = response.data.plan;
                }).catch(response => {
                    console.log(response);
                });
        }
  },
  mounted: function(){
      this.getAssignment(this.assignmentId);
  }
}
</script>

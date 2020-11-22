<template>
    <div>
        <CCard>
            <CCardBody>
                <span class="display-3">{{plan.name}}</span>
            </CCardBody>
        </CCard>

        <AssignmentSection v-for="s in sortedSections" :key="s.id" :assignment="assignment" :section="s">
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
  },
  computed: {
      sortedSections: function(){
          //figure out the size and position of each section
          for(var i=0;i<this.plan.sections.length;i++){
              this.plan.sections[i].rowIndex = _.min(_.map(this.plan.sections[i].skills,x=>x.rowIndex));
              this.plan.sections[i].rowCount = this.plan.sections[i].skills.length;
              this.plan.sections[i].columnIndex = _.min(_.map(this.plan.sections[i].levels,x=>x.columnIndex));
              this.plan.sections[i].columnCount = this.plan.sections[i].levels.length;
          }

          //sort by row, then by column
          var sorted = _.orderBy(this.plan.sections,['rowIndex','columnIndex']);

          var rows = [];

          
          for(var i=0;i<sorted.length;i++)
          {

          }

          return sorted;
      }
  }
}
</script>

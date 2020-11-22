<template>
    <CCard>
        <CCardHeader>
        <slot name="header">
            <CIcon name="cil-grid"/> {{section.name}}
        </slot>
        </CCardHeader>
        <CCardBody>
            <CDataTable
                :hover="hover"
                :striped="true"
                :bordered="true"
                :small="small"
                :fixed="fixed"
                :items="items"
                :fields="fields"
                :dark="dark">
            </CDataTable>
        </CCardBody>
    </CCard>
</template>

<script>

export default {
  name: 'AssignmentSection',
  components: {
  },
  props: ['section','assignment'],
  data () {
    return {
        fields:[],
        items:[]
    }
  },
  methods: {
        populateTable() {
            this.fields.push({key:'skillName', label:'Skill'})
            var sortedLevels = _.sortBy(this.section.levels,['columnIndex']);
            for(var i=0;i<sortedLevels.length;i++){
                this.fields.push({key:'level'+'-'+sortedLevels[i].id+'-by',label:sortedLevels[i].level.name});
                // this.fields.push({key:'level'+'-'+this.section.levels[i].id+'-at',label:''});
            }

            var sortedSkills = _.sortBy(this.section.skills,['rowIndex']);
            for(i=0;i<sortedSkills.length;i++){
                var item = {skillName:sortedSkills[i].skill.name};

                for(var l=0;l<sortedLevels.length;l++){
                    var signature = _.find(this.assignment.signatures,{sectionLevelId:sortedLevels[l].id,sectionSkillId:sortedSkills[i].id});
                    if(signature){
                        item['level'+'-'+sortedLevels[l].id+'-by'] = signature.signedBy.firstName +' '+signature.signedBy.lastName;
                        item['level'+'-'+sortedLevels[l].id+'-at'] = signature.signedAt;
                    }
                    else{
                        item['level'+'-'+sortedLevels[l].id+'-by'] = '';
                        item['level'+'-'+sortedLevels[l].id+'-at'] = '';
                    }
                }

                this.items.push(item);
            }
        }
  },
  mounted: function(){
      this.populateTable();
  }
}
</script>

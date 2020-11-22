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
            for(var i=0;i<this.section.levels.length;i++){
                this.fields.push({key:'level'+'-'+this.section.levels[i].id+'-by',label:this.section.levels[i].level.name});
                this.fields.push({key:'level'+'-'+this.section.levels[i].id+'-at',label:''});
            }

            for(i=0;i<this.section.skills.length;i++){
                var item = {skillName:this.section.skills[i].skill.name};

                for(var l=0;l<this.section.levels.length;l++){
                    var signature = _.find(this.assignment.signatures,{sectionLevelId:this.section.levels[l].id,sectionSkillId:this.section.skills[i].id});
                    if(signature){
                        item['level'+'-'+this.section.levels[l].id+'-by'] = signature.signedby.firstname +' '+signature.signedby.lastname;
                        item['level'+'-'+this.section.levels[l].id+'-at'] = signature.signedon;
                    }
                    else{
                        item['level'+'-'+this.section.levels[l].id+'-by'] = '';
                        item['level'+'-'+this.section.levels[l].id+'-at'] = '';
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

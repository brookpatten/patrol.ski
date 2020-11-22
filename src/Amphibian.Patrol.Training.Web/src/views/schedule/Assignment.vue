<template>
    <div>
        <CCard>
            <CCardHeader>{{plan.name}}</CCardHeader>
            <CCardBody v-if="signOffTable.length>0">
                <table class="table table-responsive table-bordered">
                    <thead class="thead-dark">
                        <tr>
                            <th scope="col">Skill</th>
                            <th scope="col" v-for="level in levels" :key="level.id">{{level.name}}</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="signOffRow in signOffTable" :key="signOffRow.skill.id">
                            <td>{{signOffRow.skill.name}}</td>
                            <td v-for="signOff in signOffRow.signOffs" :key="signOff.id" v-bind:class="{'table-dark':signOff==null,  'table-success': signOff!=null && signOff.signature!=null}">
                                <span v-if="signOff!=null && signOff.signature!=null">{{signOff.signature.signedBy.firstName}} {{signOff.signature.signedBy.lastName}} {{new Date(signOff.signature.signedAt).toLocaleDateString()}}</span>
                                <span v-if="signOff!=null && signOff.signature==null && !signOff.currentUserCanSign"><input type="checkbox" disabled/></span>
                                <span v-if="signOff!=null && signOff.signature==null && signOff.currentUserCanSign"><input type="checkbox"/></span>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </CCardBody>
        </CCard>

        
    </div>
</template>

<script>
export default {
  name: 'Assignment',
  components: {
  },
  props: ['assignmentId'],
  data () {
    return {
        assignment: {},
        plan:{},
        sortedSections:[],
        signOffTable:[],
        levels:[]
    }
  },
  methods: {
        getAssignment(assignmentId) {
            this.$http.get('assignment/'+assignmentId)
                .then(response => {
                    console.log(response);
                    this.assignment = response.data.assignment;
                    this.plan = response.data.plan;
                    this.tabelize();
                }).catch(response => {
                    console.log(response);
                });
        },
        sortSections(){
            //figure out the size and position of each section
            for(var i=0;i<this.plan.sections.length;i++){
                this.plan.sections[i].rowIndex = _.min(_.map(this.plan.sections[i].skills,x=>x.rowIndex));
                this.plan.sections[i].rowCount = this.plan.sections[i].skills.length;
                this.plan.sections[i].columnIndex = _.min(_.map(this.plan.sections[i].levels,x=>x.columnIndex));
                this.plan.sections[i].columnCount = this.plan.sections[i].levels.length;
            }

            //sort by row, then by column
            this.sortedSections = _.orderBy(this.plan.sections,['rowIndex','columnIndex']);
        },
        tabelize(){
            //convert the list of sections into a 2 dimensional array for easy html formatting
            this.signOffTable=[];
            this.levels=[];
            var signOffId=1;

            this.sortSections();
            
            for(var i=0;i<this.sortedSections.length;i++){
                for(var s=this.sortedSections[i].rowIndex;s<this.sortedSections[i].rowIndex + this.sortedSections[i].rowCount;s++){
                    while(this.signOffTable.length<=s){
                        this.signOffTable.push({skill:null,signOffs:[]});
                    }

                    var sectionSkill = _.find(this.sortedSections[i].skills,{rowIndex:s});
                    if(this.signOffTable[s].skill == null){
                        this.signOffTable[s].skill = sectionSkill.skill;
                    }

                    for(var l=this.sortedSections[i].columnIndex;l<this.sortedSections[i].columnIndex + this.sortedSections[i].columnCount;l++){
                        var sectionLevel = _.find(this.sortedSections[i].levels,{columnIndex:l});

                        while(this.levels.length<=l){
                            this.levels.push(null);
                        }
                        this.levels[l] = sectionLevel.level;
                        
                        while(this.signOffTable[s].signOffs.length<=l){
                            this.signOffTable[s].signOffs.push({id:signOffId++});
                        }

                        var signature = _.find(this.assignment.signatures,{sectionSkillId:sectionSkill.id,sectionLevelId:sectionLevel.id});
                        
                        this.signOffTable[s].signOffs[l].columnIndex= l;
                        this.signOffTable[s].signOffs[l].rowIndex= s;
                        this.signOffTable[s].signOffs[l].sectionlevel= sectionLevel;
                        this.signOffTable[s].signOffs[l].sectionSkill= sectionSkill;
                        this.signOffTable[s].signOffs[l].signature= signature;
                        this.signOffTable[s].signOffs[l].currentUserCanSign = this.sortedSections[i].currentUserCanSign;
                        
                    }
                }
            }
        }
  },
  mounted: function(){
      this.getAssignment(this.assignmentId);
  },
  computed: {
      
  }
}
</script>

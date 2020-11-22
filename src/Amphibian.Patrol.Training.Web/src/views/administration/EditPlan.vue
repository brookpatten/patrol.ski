<template>
    <div>
        <CCard>
            <CCardHeader>{{plan.name}}</CCardHeader>
            <CCardBody v-if="signOffTable.length>0">
                <table class="table table-responsive table-bordered">
                    <thead class="thead-dark">
                        <tr>
                            <th scope="col">Skill</th>
                            <th scope="col" v-for="level in levels" :key="level.index+'-level'">{{level.level.name}}</th>
                            <th></th>
                        </tr>
                        <tr>
                            <th scope="col"></th>
                            <th scope="col" v-for="level in levels" :key="level.index+'-level-buttons'">
                              <CButtonGroup>
                                <CButton size="sm" color="danger">Remove Level <CIcon :content="$options.freeSet.cilXCircle"/></CButton>
                                <CButton v-if="last(levels)==level" size="sm" color="primary">Add Level <CIcon :content="$options.freeSet.cilArrowThickRight"/></CButton>
                              </CButtonGroup>
                            </th>
                            <th><CButton size="sm" color="primary" v-on:click="addSection(0,signOffTable.length,levels.length,1)">Add Section <CIcon :content="$options.freeSet.cilArrowThickRight"/></CButton></th>
                        </tr>
                    </thead>
                    <tbody>
                        <template v-for="signOffRow in signOffTable">
                          <tr v-if="signOffRow.header" :key="signOffRow.skill.index+'-header-row'">
                            <td></td>
                            <th v-for="section in signOffRow.header.sections" :key="section.columnIndex+'-'+section.rowIndex+'-header'" :colspan="section.columnCount">
                              {{section.name}} 
                            </th>
                            <th></th>
                          </tr>
                          <tr v-if="signOffRow.header" :key="signOffRow.skill.index+'-header2-row'">
                            <td></td>
                            <th v-for="section in signOffRow.header.sections" :key="section.columnIndex+'-'+section.rowIndex+'-header2'" :colspan="section.columnCount">
                              <CButtonGroup>
                                <CButton v-if="section.id || section.id==0" size="sm" color="danger">Remove Section <CIcon :content="$options.freeSet.cilXCircle"/></CButton>
                                <CButton v-if="section.id || section.id==0" size="sm" color="primary">Add Level <CIcon :content="$options.freeSet.cilArrowThickRight"/></CButton>
                              </CButtonGroup>
                            </th>
                            <th>
                              <CButton size="sm" color="primary" v-on:click="addSection(last(signOffRow.header.sections).rowIndex,last(signOffRow.header.sections).rowCount,last(signOffRow.header.sections).columnIndex + last(signOffRow.header.sections).columnCount,1)">Add Section <CIcon :content="$options.freeSet.cilArrowThickRight"/></CButton>
                            </th>
                          </tr>
                          <tr :key="signOffRow.skill.index+'-skill-row'">
                            <td>{{signOffRow.skill.name}}</td>
                            <td v-for="signOff in signOffRow.signOffs" :key="signOff.rowIndex+'-'+signOff.columnIndex+'-skill-buttons'">
                                <span v-if="signOff!=null"><input type="checkbox" disabled/></span>
                                <CButton v-if="signOff!=null && allowSkillAddRemove(signOff)" size="sm" color="danger" class="float-right">Remove Skill <CIcon :content="$options.freeSet.cilXCircle"/></CButton>
                            </td>
                            <td><CButton size="sm" color="danger">Remove Skill <CIcon :content="$options.freeSet.cilXCircle"/></CButton></td>
                          </tr>
                          <tr v-if="signOffRow.footer" :key="signOffRow.skill.index+'-footer'">
                            <td>
                            </td>
                            <th v-for="section in signOffRow.footer.sections" :key="section.columnIndex+'-footer-buttons'" :colspan="section.columnCount">
                              <CButtonGroup>
                                <CButton v-if="section.id || section.id==0" size="sm" color="primary">Add Skill <CIcon :content="$options.freeSet.cilArrowThickTop"/></CButton>
                                <CButton v-if="section.id || section.id==0" size="sm" color="primary" v-on:click="addSection(section.rowIndex + section.rowCount,1,section.columnIndex,section.columnCount)">Add Section <CIcon :content="$options.freeSet.cilArrowThickBottom"/></CButton>
                              </CButtonGroup>
                            </th>
                            <th>
                              <CButtonGroup  class="float-right">
                                <CButton size="sm" color="primary">Add Skill <CIcon :content="$options.freeSet.cilArrowThickTop"/></CButton>
                                <CButton size="sm" color="primary">Add Section <CIcon :content="$options.freeSet.cilArrowThickBottom"/></CButton>
                              </CButtoNGroup>
                            </th>
                          </tr>
                        </template>
                    </tbody>
                </table>
            </CCardBody>
            <CCardFooter>
              <CButton color="primary" class="float-right">Save <CIcon :content="$options.freeSet.cilArrowThickBottom"/></CButton>
            </CCardFooter>
        </CCard>
    </div>
</template>

<script>
import { freeSet } from '@coreui/icons'
export default {
  name: 'EditPlan',
  freeSet,
  components: {
  },
  props: ['planId'],
  data () {
    return {
        plan:{},
        allLevels:[],
        allSkills:[],
        allGroups:[],
        
        sortedSections:[],
        signOffTable:[],
        levels:[]
    }
  },
  methods: {
        //fetch data
        getPlan(planId) {
            this.$http.get('plan/'+planId)
                .then(response => {
                    console.log(response);
                    this.plan = response.data;
                    this.tabelize();
                    this.getGroups();
                    this.getLevels();
                    this.getSkills();
                }).catch(response => {
                    console.log(response);
                });
        },
        getGroups() {
          this.$http.get('user/groups/'+this.plan.patrolId)
            .then(response => {
              this.allGroups = response.data;
            }).catch(response=>{
                console.log(response);
            });
        },
        getLevels() {
          this.$http.get('plan/levels/'+this.plan.patrolId)
            .then(response => {
              this.allLevels = response.data;
            }).catch(response=>{
                console.log(response);
            });
        },
        getSkills() {
          this.$http.get('plan/skills/'+this.plan.patrolId)
            .then(response => {
              this.allSkills = response.data;
            }).catch(response=>{
                console.log(response);
            });
        },

        //utilities
        last(arr){
          return _.last(arr);
        },
        allowSkillAddRemove(signOff){
          return 
          signOff.section != null
          && signOff.columnIndex == signOff.section.columnIndex + signOff.section.columnCount - 1
          && signOff.rowIndex == signOff.section.rowIndex + signOff.section.rowCount -1;
        },

        //modifiers
        addSection(rowIndex,rowCount,columnIndex,columnCount){
          //build up the viewmodel for the new section
          var section = {rowIndex,rowCount,columnIndex,columnCount,id:0,name:'New Section',skills:[],levels:[]};
          for(var i=0;i<rowCount;i++){
            //add skills
            section.skills.push({id:0,rowIndex:rowIndex+i,sectionid:0,skill:{id:0,name:'New Skill'}});
          }
          for(var i=0;i<columnCount;i++){
            //add levels
            section.levels.push({id:0,columnIndex:columnIndex+i,sectionid:0,level:{id:0,name:'New Level'}});
          }

          //insert it into 
          for(var i=0;i<this.plan.sections.length;i++){
            if(this.plan.sections[i].rowIndex>=section.rowIndex){
              this.plan.sections[i].rowIndex += section.rowCount;

              if(this.plan.sections[i].skills){
                for(var n=0;n<this.plan.sections[i].skills.length;n++){
                  if(this.plan.sections[i].skills[n].rowIndex>=section.rowIndex){
                    this.plan.sections[i].skills[n].rowIndex += section.rowCount;
                  }
                }
              }
            }

            if(this.plan.sections[i].columnIndex>=section.columnIndex){
              this.plan.sections[i].columnIndex += section.columnCount;

              if(this.plan.sections[i].levels){
                for(var n=0;n<this.plan.sections[i].levels.length;n++){
                  if(this.plan.sections[i].levels[n].columnIndex>=section.columnIndex){
                    this.plan.sections[i].levels[n].columnIndex += section.columnCount;
                  }
                }
              }
            }
          }

          this.plan.sections.push(section);
          this.tabelize();
        },
        
        //view model buildup
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

            this.sortSections();
            
            for(var i=0;i<this.sortedSections.length;i++){
                for(var s=this.sortedSections[i].rowIndex;s<this.sortedSections[i].rowIndex + this.sortedSections[i].rowCount;s++){
                    //add empty rows until we get to the index needed
                    while(this.signOffTable.length<=s){
                        this.signOffTable.push({skill:null,signOffs:[]});
                    }

                    //if this is a newly added row, set the skill/row header
                    var sectionSkill = _.find(this.sortedSections[i].skills,{rowIndex:s});
                    if(this.signOffTable[s].skill == null){
                        this.signOffTable[s].skill = sectionSkill.skill;
                    }

                    //if we're in the first row of a section, make sure to set up a header row
                    if(s==this.sortedSections[i].rowIndex){
                      if(this.signOffTable[s].header == null) {
                          this.signOffTable[s].header = {sections:[]};
                      }
                      this.signOffTable[s].header.sections.push(this.sortedSections[i]);
                      this.signOffTable[s].header.sections = _.orderBy(this.signOffTable[s].header.sections,['columnIndex']);
                    }

                    //if it's the last row of a section, add a footer
                    if(s==this.sortedSections[i].rowIndex+this.sortedSections[i].rowCount-1){
                      if(this.signOffTable[s].footer == null) {
                          this.signOffTable[s].footer = {sections:[]};
                      }
                      this.signOffTable[s].footer.sections.push(this.sortedSections[i]);
                      this.signOffTable[s].footer.sections = _.orderBy(this.signOffTable[s].footer.sections,['columnIndex']);
                    }

                    for(var l=this.sortedSections[i].columnIndex;l<this.sortedSections[i].columnIndex + this.sortedSections[i].columnCount;l++){
                        var sectionLevel = _.find(this.sortedSections[i].levels,{columnIndex:l});

                        while(this.levels.length<=l){
                            this.levels.push(null);
                        }
                        this.levels[l] = sectionLevel;
                        
                        while(this.signOffTable[s].signOffs.length<=l){
                            this.signOffTable[s].signOffs.push({id:sectionLevel.id+'-'+sectionSkill.id});
                        }

                        //var signature = _.find(this.assignment.signatures,{sectionSkillId:sectionSkill.id,sectionLevelId:sectionLevel.id});
                        
                        this.signOffTable[s].signOffs[l].columnIndex= l;
                        this.signOffTable[s].signOffs[l].rowIndex= s;
                        this.signOffTable[s].signOffs[l].sectionlevel= sectionLevel;
                        this.signOffTable[s].signOffs[l].sectionSkill= sectionSkill;
                        this.signOffTable[s].signOffs[l].section = this.sortedSections[i];

                        //this.signOffTable[s].signOffs[l].signature= signature;
                        //this.signOffTable[s].signOffs[l].currentUserCanSign = this.sortedSections[i].currentUserCanSign;
                        
                    }
                }
            }

            //pad out rows/columns to make things uniform
            for(var r=0;r<this.signOffTable.length;r++){
              this.signOffTable[r].index = r;

              //pad out header
              if(this.signOffTable[r].header!=null){
                for(var h=0;h<this.signOffTable[r].header.sections.length;h++){
                  var current = this.signOffTable[r].header.sections[h];

                  if(h+1<this.signOffTable[r].header.sections.length){
                      var next = this.signOffTable[r].header.sections[h+1];

                      if(next.columnIndex - (current.columnIndex + current.columnCount)>1){
                        var blank = { columnCount: next.columnIndex - (current.columnIndex + current.columnCount) };
                        this.signOffTable[r].header.sections.splice(h,0,blank);
                      }
                  }
                  else if(h+1==this.signOffTable[r].header.sections.length && current.columnIndex + current.columnCount < this.levels.length ){
                    var blank = { columnCount: this.levels.length - (current.columnIndex + current.columnCount) };
                    this.signOffTable[r].header.sections.push(blank);
                  }
                }
              }

              //pad out footer
              if(this.signOffTable[r].footer!=null){
                for(var h=0;h<this.signOffTable[r].footer.sections.length;h++){
                  var current = this.signOffTable[r].footer.sections[h];

                  if(h+1<this.signOffTable[r].footer.sections.length){
                      var next = this.signOffTable[r].footer.sections[h+1];

                      if(next.columnIndex - (current.columnIndex + current.columnCount)>1){
                        var blank = { columnCount: next.columnIndex - (current.columnIndex + current.columnCount) };
                        this.signOffTable[r].footer.sections.splice(h,0,blank);
                      }
                  }
                  else if(h+1==this.signOffTable[r].footer.sections.length && current.columnIndex + current.columnCount < this.levels.length ){
                    var blank = { columnCount: this.levels.length - (current.columnIndex + current.columnCount) };
                    this.signOffTable[r].footer.sections.push(blank);
                  }
                }
              }

              //pad out cells
              while(this.signOffTable[r].signOffs.length<this.levels.length){
                  this.signOffTable[r].signOffs.push({rowIndex:r,columnIndex:_.last(this.signOffTable[r].signOffs).columnIndex+1});
              }
            }

            //index levels
              for(var i=0;i<this.levels.length;i++){
                this.levels[i].index=i;
              }
        }
  },
  mounted: function(){
      this.getPlan(this.planId);
  },
  computed: {
      
  }
}
</script>

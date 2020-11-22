<template>
    <div>
        <CCard>
            <CCardHeader>{{plan.name}}</CCardHeader>
            <CCardBody v-if="signOffTable.length>0">
                <table class="table table-responsive table-bordered">
                    <thead class="thead-dark">
                        <tr>
                            <th scope="col">Skill</th>
                            <th scope="col" v-for="level in levels" :key="level.index+'-level'">
                              {{level.level.name}}
                              <CButtonGroup class="float-right" v-if="edit">
                                <CButton size="sm" color="danger" v-on:click="removeLevel(level.level)"><CIcon :content="$options.freeSet.cilXCircle"/></CButton>
                                <CButton v-if="last(levels)==level" size="sm" color="primary">Add Level <CIcon :content="$options.freeSet.cilArrowThickRight"/></CButton>
                                <CButton v-if="last(levels)==level" size="sm" color="primary" v-on:click="addSection(0,signOffTable.length,levels.length,1)">Add Section <CIcon :content="$options.freeSet.cilArrowThickRight"/></CButton>
                              </CButtonGroup>
                              
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <template v-for="signOffRow in signOffTable">
                          <tr v-if="signOffRow.header" :key="signOffRow.index+'-header-row'">
                            <td></td>
                            <th v-for="section in signOffRow.header.sections" :key="section.columnIndex+'-'+section.rowIndex+'-header'" :colspan="section.columnCount" v-bind:style="{ backgroundColor: section ? section.color : null}">
                              {{section.name}}
                              <CButtonGroup class="float-right" v-if="edit">
                                <CButton v-if="section.id || section.id==0" v-on:click="removeSection(section)" size="sm" color="danger"><CIcon :content="$options.freeSet.cilXCircle"/></CButton>
                                <CButton v-if="section.id || section.id==0" size="sm" color="primary">Add Level <CIcon :content="$options.freeSet.cilArrowThickRight"/></CButton>
                                <CButton v-if="section.id || section.id==0 && (last(signOffRow.header.sections)==section)" size="sm" color="primary" v-on:click="addSection(section.rowIndex,section.rowCount,section.columnIndex + section.columnCount,1,section)">Add Section <CIcon :content="$options.freeSet.cilArrowThickRight"/></CButton>
                              </CButtonGroup>
                            </th>
                          </tr>
                          <tr :key="signOffRow.index+'-skill-row'">
                            <td>
                              {{signOffRow.skill.name}}
                              <CButton  v-if="edit" v-on:click="removeSkill(signOffRow.skill)" size="sm" color="danger" class="float-right"><CIcon :content="$options.freeSet.cilXCircle"/></CButton>
                            </td>
                            <td v-for="signOff in signOffRow.signOffs" :key="signOff.rowIndex+'-'+signOff.columnIndex+'-skill-buttons'" v-bind:style="{ backgroundColor: signOff.section ? signOff.section.color : null}">
                                <span v-if="signOff!=null && signOff.section"><input type="checkbox" disabled/></span>
                                <CButton v-on:click="removeSkill(signOffRow.skill,signOff.section)" v-if="edit && signOff !=null && signOff.section != null && signOff.columnIndex == signOff.section.columnIndex + signOff.section.columnCount -1 && signOff.rowIndex == signOff.section.rowIndex + signOff.section.rowCount -1" size="sm" color="danger" class="float-right"><CIcon :content="$options.freeSet.cilXCircle"/></CButton>
                            </td>
                          </tr>
                          <tr v-if="edit && signOffRow.footer" :key="signOffRow.index+'-footer'">
                            <td>
                              <CButtonGroup  class="float-right">
                                <CButton size="sm" color="primary">Add Skill <CIcon :content="$options.freeSet.cilArrowThickTop"/></CButton>
                                <CButton size="sm" color="primary" v-on:click="addSection(signOffRow.index+1,1,0,levels.length,section)">Add Section <CIcon :content="$options.freeSet.cilArrowThickBottom"/></CButton>
                              </CButtoNGroup>
                            </td>
                            <th v-for="section in signOffRow.footer.sections" :key="section.columnIndex+'-footer-buttons'" :colspan="section.columnCount" v-bind:style="{ backgroundColor: section.color}">
                              <CButtonGroup>
                                <CButton v-if="section.id || section.id==0" size="sm" color="primary">Add Skill <CIcon :content="$options.freeSet.cilArrowThickTop"/></CButton>
                                <CButton v-if="section.id || section.id==0" size="sm" color="primary" v-on:click="addSection(section.rowIndex + section.rowCount,1,section.columnIndex,section.columnCount,section)">Add Section <CIcon :content="$options.freeSet.cilArrowThickBottom"/></CButton>
                              </CButtonGroup>
                            </th>
                          </tr>
                        </template>
                    </tbody>
                </table>
            </CCardBody>
            <CCardFooter>
              <CButtonGroup class="float-right">
                <CButton v-if="!edit" v-on:click="edit=true" color="success">Edit <CIcon :content="$options.freeSet.cilPencil"/></CButton>
                <CButton v-if="edit" v-on:click="edit=false" color="success">Preview <CIcon :content="$options.freeSet.cilMagnifyingGlass"/></CButton>
                <CButton color="primary">Save <CIcon :content="$options.freeSet.cilSave"/></CButton>
              </CButtonGroup>
              
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
        levels:[],
        edit:true
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
          console.log(signOff);
          return 
          signOff !=null
          && signOff.section != null
          && signOff.columnIndex == signOff.section.columnIndex + signOff.section.columnCount -1
          && signOff.rowIndex == signOff.section.rowIndex + signOff.section.rowCount -1;
          
        },
        randomColor(){
          var rand = Math.floor(Math.random()*16777215).toString(16);
          return "#" + rand;
        },

        //modifiers
        addSection(rowIndex,rowCount,columnIndex,columnCount,sourceSection){
          console.log("addSection",rowIndex,rowCount,columnIndex,columnCount,sourceSection);

          //build up the viewmodel for the new section
          var section = {rowIndex,rowCount,columnIndex,columnCount,id:0,name:'New Section',skills:[],levels:[]};
          for(var i=0;i<rowCount;i++){
            //add skills
            var newSkill=null;
            if(rowCount==1 || sourceSection==null){
              newSkill = {id:0,name:'New Skill'};
            }
            else if(sourceSection!=null){
              for(var ss=0;ss<sourceSection.skills.length;ss++){
                if(sourceSection.skills[ss].rowIndex == rowIndex+i)
                {
                  newSkill = sourceSection.skills[ss].skill;
                }
              }
            }
            section.skills.push({id:0,rowIndex:rowIndex+i,sectionid:0,skill:newSkill});
          }
          for(var i=0;i<columnCount;i++){
            //add levels
            var newLevel=null;
            if(columnCount==1 || sourceSection==null){
              newLevel = {id:0,name:'New Level'};
            }
            else if(sourceSection!=null){
              for(var ss=0;ss<sourceSection.levels.length;ss++){
                if(sourceSection.levels[ss].columnIndex == columnIndex+i)
                {
                  newLevel = sourceSection.levels[ss].level;
                }
              }
            }
            section.levels.push({id:0,columnIndex:columnIndex+i,sectionid:0,level:newLevel});
          }

          //adjust column indexes of other sections that get moved
          for(var i=0;i<this.plan.sections.length;i++){
            //loop through skills if we need to move anything down
            if(rowCount==1){
              for(var s=0;s<this.plan.sections[i].skills.length;s++){
                  if(this.plan.sections[i].skills[s].rowIndex>=rowIndex){
                    this.plan.sections[i].skills[s].rowIndex++;
                  }
              }
            }

            //loop through levels if we need to mvoe anything right
            if(columnCount==1){
              for(var s=0;s<this.plan.sections[i].levels.length;s++){
                  if(this.plan.sections[i].levels[s].columnIndex>=columnIndex){
                    this.plan.sections[i].levels[s].columnIndex++;
                  }
              }
            }
          }

          this.plan.sections.push(section);
          this.tabelize();
        },
        removeSection(section){
          //TODO: adjust surrounding indexes?

          this.plan.sections = _.pull(this.plan.sections,section);
          this.tabelize();
        },
        removeSkill(skill,section){
          var removeRowIndex=-1;
          for(var s=0;s<this.plan.sections.length;s++){
            if(section==null || (section!=null && this.plan.sections[s]==section)){
              for(var i=0;i<this.plan.sections[s].skills.length;i++){
                if(this.plan.sections[s].skills[i].skill==skill || (skill.id && this.plan.sections[s].skills[i].skill.id==skill.id)){
                  removeRowIndex = this.plan.sections[s].skills[i].rowIndex;
                  //this.plan.sections[s].skills = 
                  this.plan.sections[s].skills.splice(i,1);
                }
                //only recalc rowindexes if we're removing the skill entirely
                if(section==null 
                && removeRowIndex>-1
                && i< this.plan.sections[s].skills.length
                && removeRowIndex<this.plan.sections[s].skills[i].rowIndex){
                  this.plan.sections[s].skills[i].rowIndex--;
                  //i--;
                }
              }

              if(this.plan.sections[s].skills.length==0){
                _.pull(this.plan.sections,this.plan.sections[s]);
                s--;
              }
            }
          }
          this.tabelize();
        },
        removeLevel(level,section){
          var removeColumnIndex=-1;
          for(var s=0;s<this.plan.sections.length;s++){
            if(section==null || (section!=null && this.plan.sections[s]==section)){
              for(var i=0;i<this.plan.sections[s].levels.length;i++){
                if(this.plan.sections[s].levels[i].level==level || (level.id && this.plan.sections[s].levels[i].level.id==level.id)){
                  removeColumnIndex = this.plan.sections[s].levels[i].columnIndex;
                  //this.plan.sections[s].skills = 
                  this.plan.sections[s].levels.splice(i,1);
                }
                //only recalc rowindexes if we're removing the skill entirely
                if(section==null 
                && removeColumnIndex>-1
                && i < this.plan.sections[s].levels.length
                && removeColumnIndex<this.plan.sections[s].levels[i].columnIndex
                ){
                  this.plan.sections[s].levels[i].columnIndex--;
                  //i--;
                }
              }

              if(this.plan.sections[s].levels.length==0){
                _.pull(this.plan.sections,this.plan.sections[s]);
                s--;
              }
            }
          }
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

                if(this.plan.sections[i] && !this.plan.sections[i].color){
                  this.plan.sections[i].color = this.randomColor();
                }
            }

            //sort by row, then by column
            this.sortedSections = _.orderBy(this.plan.sections,['rowIndex','columnIndex']);
            this.plan.sections = this.sortedSections;
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
                    if(this.signOffTable[s].skill == null && sectionSkill!=null){
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
                            this.signOffTable[s].signOffs.push({id:i+'-'+l});
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
                  var previous = null;
                  if(h>0){
                    previous = this.signOffTable[r].header.sections[h-1]
                  }
                  var previousColumn = previous ? previous.columnIndex + previous.columnCount : 0;
                  if(previousColumn!=current.columnIndex){
                    var blank = { columnCount: current.columnIndex - previousColumn, columnIndex:previousColumn };
                    this.signOffTable[r].header.sections.push(blank);
                    this.signOffTable[r].header.sections = _.orderBy(this.signOffTable[r].header.sections,['columnIndex']);
                    h++;
                  }

                  if(h+1<this.signOffTable[r].header.sections.length){
                      var next = this.signOffTable[r].header.sections[h+1];

                      if(next.columnIndex - (current.columnIndex + current.columnCount)>1){
                        var blank = { columnCount: next.columnIndex - (current.columnIndex + current.columnCount), columnIndex:current.columnIndex+current.columnCount };
                        this.signOffTable[r].header.sections.push(blank);
                        this.signOffTable[r].header.sections = _.orderBy(this.signOffTable[r].header.sections,['columnIndex']);
                        h++;
                      }
                  }
                  else if(h+1==this.signOffTable[r].header.sections.length && current.columnIndex + current.columnCount < this.levels.length ){
                    var blank = { columnCount: this.levels.length - (current.columnIndex + current.columnCount), columnIndex:current.columnIndex+current.columnCount };
                    this.signOffTable[r].header.sections.push(blank);
                    this.signOffTable[r].header.sections = _.orderBy(this.signOffTable[r].header.sections,['columnIndex']);
                    h++;
                  }
                }
              }

              //pad out footer
              if(this.signOffTable[r].footer!=null){
                //pad out any columns that are empty due to sections that only exist below
                // if(this.signOffTable[r].footer.sections
                // && this.signOffTable[r].footer.sections.length>0
                // && this.signOffTable[r].footer.sections[0].columnIndex>0){
                //   this.signOffTable[r].footer.sections = this.signOffTable[r].footer.sections.splice(0,0,{columnCount:this.signOffTable[r].footer.sections[0].columnIndex});
                // }

                for(var h=0;h<this.signOffTable[r].footer.sections.length;h++){
                  var current = this.signOffTable[r].footer.sections[h];

                  var previous = null;
                  if(h>0){
                    previous = this.signOffTable[r].footer.sections[h-1]
                  }
                  var previousColumn = previous ? previous.columnIndex + previous.columnCount : 0;
                  if(previousColumn!=current.columnIndex){
                    var blank = { columnCount: current.columnIndex - previousColumn, columnIndex:previousColumn };
                    //this.signOffTable[r].footer.sections = 
                    this.signOffTable[r].footer.sections.splice(h,0,blank);
                  }

                  if(h+1<this.signOffTable[r].footer.sections.length){
                      var next = this.signOffTable[r].footer.sections[h+1];

                      if(next.columnIndex - (current.columnIndex + current.columnCount)>1){
                        var blank = { columnCount: next.columnIndex - (current.columnIndex + current.columnCount), columnIndex:current.columnIndex+current.columnCount };
                        this.signOffTable[r].footer.sections.splice(h,0,blank);
                      }
                  }
                  else if(h+1==this.signOffTable[r].footer.sections.length && current.columnIndex + current.columnCount < this.levels.length ){
                    var blank = { columnCount: this.levels.length - (current.columnIndex + current.columnCount), columnIndex:current.columnIndex+current.columnCount };
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
